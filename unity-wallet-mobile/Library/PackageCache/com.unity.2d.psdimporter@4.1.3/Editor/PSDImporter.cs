using System;
using System.Collections.Generic;
using System.IO;
using PDNWrapper;
using UnityEngine;
using Unity.Collections;
using System.Linq;
using UnityEditor.AssetImporters;
using UnityEditor.U2D.Animation;
using UnityEditor.U2D.Common;
using UnityEditor.U2D.Sprites;
using UnityEngine.Assertions;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.U2D.PSD
{
    /// <summary>
    /// ScriptedImporter to import Photoshop files
    /// </summary>
    // Version using unity release + 5 digit padding for future upgrade. Eg 2021.2 -> 21200000
    [ScriptedImporter(20300001, "psb", AllowCaching = true)]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.psdimporter@latest")]
    [MovedFrom("UnityEditor.Experimental.AssetImporters")]
    public class PSDImporter : ScriptedImporter, ISpriteEditorDataProvider
    {
        class UniqueNameGenerator
        {
            List<int> m_NameHash = new List<int>();

            public bool ContainHash(int i)
            {
                return m_NameHash.Contains(i);
            }

            public void AddHash(int i)
            {
                m_NameHash.Add(i);
            }
            
            public string GetUniqueName(string name)
            {
                return PSDImporter.GetUniqueName(name, m_NameHash);
            }
        }
        
        class GameObjectCreationFactory : UniqueNameGenerator
        {

            public GameObject CreateGameObject(string name, params System.Type[] components)
            {
                var newName = GetUniqueName(name);
                return new GameObject(newName, components);
            }
        }

        struct BoneGO
        {
            public GameObject go;
            public int index;
        }

        [SerializeField]
        TextureImporterSettings m_TextureImporterSettings = new TextureImporterSettings() {
            mipmapEnabled = true,
            mipmapFilter = TextureImporterMipFilter.BoxFilter,
            sRGBTexture = true,
            borderMipmap = false,
            mipMapsPreserveCoverage = false,
            alphaTestReferenceValue = 0.5f,
            readable = false,

#if ENABLE_TEXTURE_STREAMING
            streamingMipmaps = true,
#endif

            fadeOut = false,
            mipmapFadeDistanceStart = 1,
            mipmapFadeDistanceEnd = 3,

            convertToNormalMap = false,
            heightmapScale = 0.25F,
            normalMapFilter = 0,

            generateCubemap = TextureImporterGenerateCubemap.AutoCubemap,
            cubemapConvolution = 0,

            seamlessCubemap = false,

            npotScale = TextureImporterNPOTScale.ToNearest,

            spriteMode = (int)SpriteImportMode.Multiple,
            spriteExtrude = 1,
            spriteMeshType = SpriteMeshType.Tight,
            spriteAlignment = (int)SpriteAlignment.Center,
            spritePivot = new Vector2(0.5f, 0.5f),
            spritePixelsPerUnit = 100.0f,
            spriteBorder = new Vector4(0.0f, 0.0f, 0.0f, 0.0f),

            alphaSource = TextureImporterAlphaSource.FromInput,
            alphaIsTransparency = true,
            spriteTessellationDetail = -1.0f,

            textureType = TextureImporterType.Sprite,
            textureShape = TextureImporterShape.Texture2D,

            filterMode = FilterMode.Bilinear,
            aniso = 1,
            mipmapBias = 0.0f,
            wrapModeU = TextureWrapMode.Repeat,
            wrapModeV = TextureWrapMode.Repeat,
            wrapModeW = TextureWrapMode.Repeat,
        };
        
        [SerializeField]
        List<SpriteMetaData> m_SpriteImportData = new List<SpriteMetaData>(); // we use index 0 for single sprite and the rest for multiple sprites
        [SerializeField]
        List<SpriteMetaData> m_MosaicSpriteImportData = new List<SpriteMetaData>();
        [SerializeField]
        List<SpriteMetaData> m_RigSpriteImportData = new List<SpriteMetaData>();

        [SerializeField]
        List<TextureImporterPlatformSettings> m_PlatformSettings = new List<TextureImporterPlatformSettings>();
        [SerializeField]
        bool m_MosaicLayers = true;
        [SerializeField]
        Vector2 m_DocumentPivot = Vector2.zero;
        [SerializeField]
        SpriteAlignment m_DocumentAlignment = SpriteAlignment.BottomCenter;
        [SerializeField]
        bool m_ImportHiddenLayers = false;
        [SerializeField]
        int m_ImportedTextureWidth;
        [SerializeField]
        int m_ImportedTextureHeight;
        [SerializeField]
        Vector2Int m_DocumentSize;

        [SerializeField]
        bool m_PaperDollMode = false;

        [SerializeField]
        bool m_KeepDupilcateSpriteName = false;

        [SerializeField]
        SpriteCategoryList m_SpriteCategoryList = new SpriteCategoryList() {categories = new List<SpriteCategory>()};
        GameObjectCreationFactory m_GameObjectFactory = new GameObjectCreationFactory();

        internal SpriteCategoryList spriteCategoryList { get { return m_SpriteCategoryList; } set { m_SpriteCategoryList = value; } }

        [SerializeField]
        int m_TextureActualWidth;
        internal int textureActualWidth
        {
            get { return m_TextureActualWidth; }
            private set { m_TextureActualWidth = value; }
        }

        [SerializeField]
        int m_TextureActualHeight;
        internal int textureActualHeight
        {
            get { return m_TextureActualHeight; }
            private set { m_TextureActualHeight = value; }
        }

        [SerializeField]
        string m_SpritePackingTag = "";

        [SerializeField]
        bool m_ResliceFromLayer = false;
        [SerializeField]
        bool m_CharacterMode = true;

        [SerializeField]
        List<PSDLayer> m_MosaicPSDLayers = new List<PSDLayer>();
        [SerializeField]
        List<PSDLayer> m_RigPSDLayers = new List<PSDLayer>();

        [SerializeField]
        CharacterData m_CharacterData = new CharacterData();

        [SerializeField]
        bool m_GenerateGOHierarchy = false;

        [SerializeField]
        string m_TextureAssetName = null;

        [SerializeField]
        string m_PrefabAssetName = null;

        [SerializeField]
        string m_SpriteLibAssetName = null;

        [SerializeField]
        SecondarySpriteTexture[] m_SecondarySpriteTextures;

        /// <summary>
        /// Implementation of ScriptedImporter.OnImportAsset
        /// </summary>
        /// <param name="ctx">
        /// This argument contains all the contextual information needed to process the import
        /// event and is also used by the custom importer to store the resulting Unity Asset.
        /// </param>
        public override void OnImportAsset(AssetImportContext ctx)
        {
            FileStream fileStream = new FileStream(ctx.assetPath, FileMode.Open, FileAccess.Read);
            Document doc = null;
            try
            {
                UnityEngine.Profiling.Profiler.BeginSample("OnImportAsset");

                UnityEngine.Profiling.Profiler.BeginSample("PsdLoad");
                doc = PaintDotNet.Data.PhotoshopFileType.PsdLoad.Load(fileStream);
                UnityEngine.Profiling.Profiler.EndSample();

                ValidatePSDLayerId(doc);
                
                m_DocumentSize = new Vector2Int(doc.width, doc.height);
                bool singleSpriteMode = m_TextureImporterSettings.textureType == TextureImporterType.Sprite && m_TextureImporterSettings.spriteMode != (int)SpriteImportMode.Multiple;
                EnsureSingleSpriteExist();

                if (m_TextureImporterSettings.textureType != TextureImporterType.Sprite ||
                    m_MosaicLayers == false || singleSpriteMode)
                {
                    var image = new NativeArray<Color32>(doc.width * doc.height, Allocator.Persistent);
                    try
                    {
                        var spriteImportData = GetSpriteImportData();
                        FlattenImageTask.Execute(doc.Layers, m_ImportHiddenLayers, doc.width, doc.height, image);

                        int spriteCount = spriteDataCount;
                        int spriteIndexStart = 1;

                        if (spriteImportData.Count <= 0 || spriteImportData[0] == null)
                        {
                            spriteImportData.Add(new SpriteMetaData());
                        }
                        spriteImportData[0].name = System.IO.Path.GetFileNameWithoutExtension(ctx.assetPath) + "_1";
                        spriteImportData[0].alignment = (SpriteAlignment)m_TextureImporterSettings.spriteAlignment;
                        spriteImportData[0].border = m_TextureImporterSettings.spriteBorder;
                        spriteImportData[0].pivot = m_TextureImporterSettings.spritePivot;
                        spriteImportData[0].rect = new Rect(0, 0, doc.width, doc.height);
                        if (singleSpriteMode)
                        {
                            spriteCount = 1;
                            spriteIndexStart = 0;
                        }
                        textureActualWidth = doc.width;
                        textureActualHeight = doc.height;
                        var output = ImportTexture(ctx, image, doc.width, doc.height, spriteIndexStart, spriteCount);
                        RegisterAssets(ctx, output);
                    }
                    finally
                    {
                        image.Dispose();
                    }
                }
                else
                {
                    ImportFromLayers(ctx, doc);
                }
            }
            finally
            {
                fileStream.Close();
                if (doc != null)
                    doc.Dispose();
                UnityEngine.Profiling.Profiler.EndSample();
                EditorUtility.SetDirty(this);
            }


            // Debug Profiler.
            // UnityEngine.Profiling.Memory.Experimental.MemoryProfiler.TakeSnapshot("snapshot.snap", MemorySnapshotFinish, CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects | CaptureFlags.NativeAllocations | CaptureFlags.NativeStackTraces);
        }

        static void ValidatePSDLayerId(List<BitmapLayer> layers, UniqueNameGenerator uniqueNameGenerator)
        {
            for (int i = 0; i < layers.Count; ++i)
            {
                if (uniqueNameGenerator.ContainHash(layers[i].LayerID))
                {
                    var importWarning = string.Format("Layer {0}: LayerId is not unique. Mapping will be done by Layer's name.", layers[i].Name);
                    var layerName = uniqueNameGenerator.GetUniqueName(layers[i].Name);
                    if (layerName != layers[i].Name)
                        importWarning += "\nLayer names are not unique. Please ensure they are unique to for SpriteRect to be mapped back correctly.";
                    layers[i].LayerID = layerName.GetHashCode();
                    Debug.LogWarning(importWarning);
                }
                else
                    uniqueNameGenerator.AddHash(layers[i].LayerID);
                if (layers[i].ChildLayer != null)
                {
                    ValidatePSDLayerId(layers[i].ChildLayer, uniqueNameGenerator);
                }
            }
        }
        
        
        void ValidatePSDLayerId(Document doc)
        {
            UniqueNameGenerator uniqueNameGenerator = new UniqueNameGenerator();

            ValidatePSDLayerId(doc.Layers, uniqueNameGenerator);
        }
        
        TextureGenerationOutput ImportTexture(AssetImportContext ctx, NativeArray<Color32> imageData, int textureWidth, int textureHeight, int spriteStart, int spriteCount)
        {
            if (!imageData.IsCreated || imageData.Length == 0)
                return new TextureGenerationOutput();
            
            UnityEngine.Profiling.Profiler.BeginSample("ImportTexture");
            var platformSettings = GetPlatformTextureSettings(ctx.selectedBuildTarget);

            var textureSettings = m_TextureImporterSettings.ExtractTextureSettings();
            textureSettings.assetPath = ctx.assetPath;
            textureSettings.enablePostProcessor = true;
            textureSettings.containsAlpha = true;
            textureSettings.hdr = false;

            var textureAlphaSettings = m_TextureImporterSettings.ExtractTextureAlphaSettings();
            var textureMipmapSettings = m_TextureImporterSettings.ExtractTextureMipmapSettings();
            var textureCubemapSettings = m_TextureImporterSettings.ExtractTextureCubemapSettings();
            var textureWrapSettings = m_TextureImporterSettings.ExtractTextureWrapSettings();

            TextureGenerationOutput output;
            switch (m_TextureImporterSettings.textureType)
            {
                case TextureImporterType.Default:
                    output = TextureGeneratorHelper.GenerateTextureDefault(imageData, textureWidth, textureHeight, textureSettings, platformSettings, textureAlphaSettings, textureMipmapSettings, textureCubemapSettings, textureWrapSettings);
                    break;
                case TextureImporterType.NormalMap:
                    var textureNormalSettings = m_TextureImporterSettings.ExtractTextureNormalSettings();
                    output = TextureGeneratorHelper.GenerateNormalMap(imageData, textureWidth, textureHeight, textureSettings, platformSettings, textureNormalSettings, textureMipmapSettings, textureCubemapSettings, textureWrapSettings);
                    break;
                case TextureImporterType.GUI:
                    output = TextureGeneratorHelper.GenerateTextureGUI(imageData, textureWidth, textureHeight, textureSettings, platformSettings, textureAlphaSettings, textureMipmapSettings, textureWrapSettings);
                    break;
                case TextureImporterType.Sprite:
                    var textureSpriteSettings = m_TextureImporterSettings.ExtractTextureSpriteSettings();
                    textureSpriteSettings.packingTag = m_SpritePackingTag;
                    textureSpriteSettings.qualifyForPacking = !string.IsNullOrEmpty(m_SpritePackingTag);
                    textureSpriteSettings.spriteSheetData = new UnityEditor.AssetImporters.SpriteImportData[spriteCount];
                    textureSettings.npotScale = TextureImporterNPOTScale.None;
                    textureSettings.secondaryTextures = secondaryTextures;
                    var spriteImportData = GetSpriteImportData();
                    for (int i = 0; i < spriteCount; ++i)
                    {
                        //AutoGenerateSpriteSkinData(m_SpriteImportData[spriteStart + i]);
                        textureSpriteSettings.spriteSheetData[i] = spriteImportData[spriteStart + i];
                    }
                    output = TextureGeneratorHelper.GenerateTextureSprite(imageData, textureWidth, textureHeight, textureSettings, platformSettings, textureSpriteSettings, textureAlphaSettings, textureMipmapSettings, textureWrapSettings);
                    break;
                case TextureImporterType.Cursor:
                    output = TextureGeneratorHelper.GenerateTextureCursor(imageData, textureWidth, textureHeight, textureSettings, platformSettings, textureAlphaSettings, textureMipmapSettings, textureWrapSettings);
                    break;
                case TextureImporterType.Cookie:
                    output = TextureGeneratorHelper.GenerateCookie(imageData, textureWidth, textureHeight, textureSettings, platformSettings, textureAlphaSettings, textureMipmapSettings, textureCubemapSettings, textureWrapSettings);
                    break;
                case TextureImporterType.Lightmap:
                    output = TextureGeneratorHelper.GenerateLightmap(imageData, textureWidth, textureHeight, textureSettings, platformSettings, textureMipmapSettings, textureWrapSettings);
                    break;
                case TextureImporterType.SingleChannel:
                    output = TextureGeneratorHelper.GenerateTextureSingleChannel(imageData, textureWidth, textureHeight, textureSettings, platformSettings, textureAlphaSettings, textureMipmapSettings, textureCubemapSettings, textureWrapSettings);
                    break;
                default:
                    Debug.LogAssertion("Unknown texture type for import");
                    output = default(TextureGenerationOutput);
                    break;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return output;
        }

        void AutoGenerateSpriteSkinData(SpriteMetaData metaData)
        {
            //If bone data exist but skinning data doesn't exist, auto generate them
            if (metaData.spriteBone != null && metaData.spriteBone.Count > 0 &&
                (metaData.vertices == null || metaData.vertices.Count == 0))
            {
                var spriteMeshDataController = new SpriteMeshDataController();
                var smd = new SpriteMeshData();
                smd.spriteID = metaData.spriteID;
                smd.frame = metaData.rect;
                smd.pivot = metaData.pivot;
                smd.bones = ModuleUtility.CreateSpriteBoneData(metaData.spriteBone.ToArray(), Matrix4x4.TRS(metaData.rect.position, Quaternion.identity, Vector3.one));
                spriteMeshDataController.spriteMeshData = smd;
                spriteMeshDataController.OutlineFromAlpha(new OutlineGenerator(), GetDataProvider<ITextureDataProvider>(), 0.05f, 200);
                spriteMeshDataController.Triangulate(new Triangulator());
                spriteMeshDataController.Subdivide(new Triangulator(), 0.25f);
                spriteMeshDataController.CalculateWeights(new BoundedBiharmonicWeightsGenerator(), null, 0.01f);
                spriteMeshDataController.SortTrianglesByDepth();

                List<Vertex2DMetaData> vmd = new List<Vertex2DMetaData>(smd.vertices.Count);
                foreach (var v in smd.vertices)
                    vmd.Add(new Vertex2DMetaData() { position = v.position - smd.frame.position, boneWeight = v.editableBoneWeight.ToBoneWeight(true) });
                List<Vector2Int> emd = new List<Vector2Int>(smd.edges.Count);
                foreach (var e in smd.edges)
                    emd.Add(new Vector2Int(e.index1, e.index2));

                metaData.vertices = vmd;
                metaData.indices = smd.indices.ToArray();
                metaData.edges = emd.ToArray();
            }
        }

        string GetUniqueSpriteName(string name, List<int> namehash)
        {
            if (m_KeepDupilcateSpriteName)
                return name;
            return GetUniqueName(name, namehash);
        }

        void ImportFromLayers(AssetImportContext ctx, Document doc)
        {
            NativeArray<Color32> output = default(NativeArray<Color32>);

            List<int> layerIndex = new List<int>();
            List<int> spriteNameHash = new List<int>();
            var oldPsdLayers = GetPSDLayers();
            try
            {
                var psdLayers = new List<PSDLayer>();
                ExtractLayerTask.Execute(psdLayers, doc.Layers, m_ImportHiddenLayers);
                var removedLayersSprite = oldPsdLayers.Where(x => psdLayers.FirstOrDefault(y => y.layerID == x.layerID) == null).Select(z => z.spriteID).ToArray();
                for (int i = 0; i < psdLayers.Count; ++i)
                {
                    int j = 0;
                    var psdLayer = psdLayers[i];
                    for (; j < oldPsdLayers.Count; ++j)
                    {
                        if (psdLayer.layerID == oldPsdLayers[j].layerID)
                        {
                            psdLayer.spriteID = oldPsdLayers[j].spriteID;
                            psdLayer.spriteName = oldPsdLayers[j].spriteName;
                            psdLayer.mosaicPosition = oldPsdLayers[j].mosaicPosition;
                            break;
                        }
                    }
                }

                int expectedBufferLength = doc.width * doc.height;
                var layerBuffers = new List<NativeArray<Color32>>();
                for (int i = 0; i < psdLayers.Count; ++i)
                {
                    var l = psdLayers[i];
                    if (l.texture.IsCreated && l.texture.Length == expectedBufferLength)
                    {
                        layerBuffers.Add(l.texture);
                        layerIndex.Add(i);
                    }
                }

                RectInt[] spritedata;
                int width, height;
                int padding = 4;
                Vector2Int[] uvTransform;
                ImagePacker.Pack(layerBuffers.ToArray(), doc.width, doc.height, padding, out output, out width, out height, out spritedata, out uvTransform);
                var spriteImportData = GetSpriteImportData();
                if (spriteImportData.Count <= 0 || shouldResliceFromLayer)
                {
                    var newSpriteMeta = new List<SpriteMetaData>();

                    for (int i = 0; i < spritedata.Length && i < layerIndex.Count; ++i)
                    {
                        var spriteSheet = spriteImportData.FirstOrDefault(x => x.spriteID == psdLayers[layerIndex[i]].spriteID);
                        if (spriteSheet == null)
                        {
                            spriteSheet = new SpriteMetaData();
                            spriteSheet.border = Vector4.zero;
                            spriteSheet.alignment = (SpriteAlignment)m_TextureImporterSettings.spriteAlignment;
                            spriteSheet.pivot = m_TextureImporterSettings.spritePivot;
                        }

                        psdLayers[layerIndex[i]].spriteName = GetUniqueSpriteName(psdLayers[layerIndex[i]].name, spriteNameHash);
                        spriteSheet.name = psdLayers[layerIndex[i]].spriteName;
                        spriteSheet.rect = new Rect(spritedata[i].x, spritedata[i].y, spritedata[i].width, spritedata[i].height);
                        spriteSheet.uvTransform = uvTransform[i];

                        psdLayers[layerIndex[i]].spriteID = spriteSheet.spriteID;
                        psdLayers[layerIndex[i]].mosaicPosition = spritedata[i].position;
                        newSpriteMeta.Add(spriteSheet);
                    }
                    spriteImportData.Clear();
                    spriteImportData.AddRange(newSpriteMeta);
                }
                else
                {
                    spriteImportData.RemoveAll(x => removedLayersSprite.Contains(x.spriteID));

                    // First look for any user created SpriteRect and add those into the name hash
                    foreach (var spriteData in spriteImportData)
                    {
                        var psdLayer = psdLayers.FirstOrDefault(x => x.spriteID == spriteData.spriteID);
                        if (psdLayer == null)
                            spriteNameHash.Add(spriteData.name.GetHashCode());
                    }

                    foreach (var spriteData in spriteImportData)
                    {
                        var psdLayer = psdLayers.FirstOrDefault(x => x.spriteID == spriteData.spriteID);
                        if (psdLayer == null)
                            spriteData.uvTransform = new Vector2Int((int)spriteData.rect.position.x, (int)spriteData.rect.position.y);
                        // If it is user created rect or the name has been changed before
                        // add it into the spriteNameHash and we don't copy it over from the layer
                        if (psdLayer == null || psdLayer.spriteName != spriteData.name)
                            spriteNameHash.Add(spriteData.name.GetHashCode());

                        // If the sprite name has not been changed, we ensure the new
                        // layer name is still unique and use it as the sprite name
                        if (psdLayer != null && psdLayer.spriteName == spriteData.name)
                        {
                            psdLayer.spriteName = GetUniqueSpriteName(psdLayer.name, spriteNameHash);
                            spriteData.name = psdLayer.spriteName;
                        }
                    }

                    //Update names for those user has not changed and add new sprite rect based on PSD file.
                    for (int k = 0; k < layerIndex.Count; ++k)
                    {
                        int i = layerIndex[k];
                        var spriteSheet = spriteImportData.FirstOrDefault(x => x.spriteID == psdLayers[i].spriteID);
                        var inOldLayer = oldPsdLayers.FindIndex(x => x.layerID == psdLayers[i].layerID) != -1;
                        if (spriteSheet == null && !inOldLayer)
                        {
                            spriteSheet = new SpriteMetaData();
                            spriteImportData.Add(spriteSheet);
                            spriteSheet.rect = new Rect(spritedata[k].x, spritedata[k].y, spritedata[k].width, spritedata[k].height);
                            spriteSheet.border = Vector4.zero;
                            spriteSheet.alignment = (SpriteAlignment)m_TextureImporterSettings.spriteAlignment;
                            spriteSheet.pivot = m_TextureImporterSettings.spritePivot;
                            psdLayers[i].spriteName = GetUniqueSpriteName(psdLayers[i].name, spriteNameHash);
                            spriteSheet.name = psdLayers[i].spriteName;
                        }
                        else if (spriteSheet != null)
                        {
                            var r = spriteSheet.rect;
                            r.position = spriteSheet.rect.position - psdLayers[i].mosaicPosition + spritedata[k].position;
                            spriteSheet.rect = r;
                        }

                        if (spriteSheet != null)
                        {
                            spriteSheet.uvTransform = uvTransform[k];
                            psdLayers[i].spriteID = spriteSheet.spriteID;
                            psdLayers[i].mosaicPosition = spritedata[k].position;
                        }
                    }
                }

                foreach (var l in oldPsdLayers)
                    l.Dispose();
                oldPsdLayers.Clear();

                oldPsdLayers.AddRange(psdLayers);
                m_ImportedTextureHeight = textureActualHeight = height;
                m_ImportedTextureWidth = textureActualWidth = width;
                var generatedTexture = ImportTexture(ctx, output, width, height, 0, spriteImportData.Count);
                
                if (generatedTexture.texture)
                {
                    m_ImportedTextureHeight = generatedTexture.texture.height;
                    m_ImportedTextureWidth = generatedTexture.texture.width;
                }

                RegisterAssets(ctx, generatedTexture);
            }
            finally
            {
                if (output.IsCreated)
                    output.Dispose();
                foreach (var l in oldPsdLayers)
                    l.Dispose();
            }
            
        }

        void MemorySnapshotFinish(string path, bool done)
        {

        }

        void EnsureSingleSpriteExist()
        {
            if (m_SpriteImportData.Count <= 0)
                m_SpriteImportData.Add(new SpriteMetaData()); // insert default for single sprite mode
        }

        internal TextureImporterPlatformSettings GetPlatformTextureSettings(BuildTarget buildTarget)
        {
            var buildTargetName = TexturePlatformSettingsHelper.GetBuildTargetGroupName(buildTarget);
            TextureImporterPlatformSettings platformSettings = null;
            platformSettings = m_PlatformSettings.SingleOrDefault(x => x.name == buildTargetName && x.overridden == true);
            platformSettings = platformSettings ?? m_PlatformSettings.SingleOrDefault(x => x.name == TexturePlatformSettingsHelper.defaultPlatformName);

            if (platformSettings == null)
            {
                platformSettings = new TextureImporterPlatformSettings();
                platformSettings.name = buildTargetName;
                platformSettings.overridden = false;
            }
            return platformSettings;
        }

        void RegisterAssets(AssetImportContext ctx, TextureGenerationOutput output)
        {
            List<int> assetNameHash = new List<int>();
            if (!string.IsNullOrEmpty(output.importInspectorWarnings))
            {
                Debug.LogWarning(output.importInspectorWarnings);
            }
            if (output.importWarnings != null && output.importWarnings.Length != 0)
            {
                foreach (var warning in output.importWarnings)
                    Debug.LogWarning(warning);
            }
            if (output.thumbNail == null)
                Debug.LogWarning("Thumbnail generation fail");
            if (output.texture == null)
            {
                throw new Exception("Texture import fail");
            }

            var assetName = GetUniqueName(System.IO.Path.GetFileNameWithoutExtension(ctx.assetPath), assetNameHash, true);
            // Setup all fixed name on the hash table
            if (string.IsNullOrEmpty(m_TextureAssetName))
                m_TextureAssetName = GetUniqueName(string.Format("{0} Texture",assetName), assetNameHash, true);
            if (string.IsNullOrEmpty(m_PrefabAssetName))
                m_PrefabAssetName = GetUniqueName(string.Format("{0} Prefab", assetName), assetNameHash, true);
            if (string.IsNullOrEmpty(m_SpriteLibAssetName))
                m_SpriteLibAssetName = GetUniqueName(string.Format("{0} Sprite Lib", assetName), assetNameHash, true);

            output.texture.name = assetName;
            ctx.AddObjectToAsset(m_TextureAssetName, output.texture, output.thumbNail);
            UnityEngine.Object mainAsset = output.texture;


            if (output.sprites != null)
            {
                var slAsset = ProduceSpriteLibAsset(output.sprites);

                if (shouldProduceGameObject)
                {
                    GameObject prefab = null;
                    if (m_PaperDollMode)
                        prefab = OnProducePaperDollPrefab(m_TextureAssetName, output.sprites, slAsset);
                    else
                        prefab = OnProducePrefab(m_TextureAssetName, output.sprites, slAsset);
                    if (prefab != null)
                    {
                        ctx.AddObjectToAsset(m_PrefabAssetName, prefab);
                        mainAsset = prefab;
                    }
                }

                foreach (var s in output.sprites)
                {
                    var spriteAssetName = GetUniqueName(s.GetSpriteID().ToString(), assetNameHash, false, s);
                    ctx.AddObjectToAsset(spriteAssetName, s);
                }

                if (slAsset != null)
                {
                    slAsset.name = assetName;
                    ctx.AddObjectToAsset(m_SpriteLibAssetName, slAsset);
                }
            }
            ctx.SetMainObject(mainAsset);
        }

        bool SpriteIsMainFromSpriteLib(string spriteId, out string categoryName)
        {
            categoryName = "";
            if (m_SpriteCategoryList.categories != null)
            {
                foreach (var category in m_SpriteCategoryList.categories)
                {
                    var index = category.labels.FindIndex(x => x.spriteId == spriteId);
                    if (index == 0)
                    {
                        categoryName = category.name;
                        return true;
                    }
                    if (index > 0)
                        return false;
                }
            }
            return true;
        }

        void BuildGroupGameObject(List<PSDLayer> psdGroup, int index, Transform root)
        {
            var spriteData = GetSpriteImportData().FirstOrDefault(x => x.spriteID == psdGroup[index].spriteID);
            if (psdGroup[index].gameObject == null)
            {
                if (m_GenerateGOHierarchy || (!psdGroup[index].spriteID.Empty() && psdGroup[index].isGroup == false))
                {
                    // Determine if need to create GameObject i.e. if the sprite is not in a SpriteLib or if it is the first one
                    string categoryName;
                    var b = SpriteIsMainFromSpriteLib(psdGroup[index].spriteID.ToString(), out categoryName);
                    string goName = string.IsNullOrEmpty(categoryName) ? spriteData  != null ? spriteData.name : psdGroup[index].name : categoryName;
                    if (b)
                        psdGroup[index].gameObject = m_GameObjectFactory.CreateGameObject(goName);
                }
                if (psdGroup[index].parentIndex >= 0 && m_GenerateGOHierarchy)
                {
                    BuildGroupGameObject(psdGroup, psdGroup[index].parentIndex, root);
                    root = psdGroup[psdGroup[index].parentIndex].gameObject.transform;
                }

                if (psdGroup[index].gameObject != null)
                {
                    psdGroup[index].gameObject.transform.SetParent(root);
                    psdGroup[index].gameObject.transform.SetSiblingIndex(root.childCount-1);
                }
                    
            }
        }

        bool shouldProduceGameObject
        {
            get { return m_CharacterMode && m_MosaicLayers && spriteImportMode == SpriteImportMode.Multiple; }
        }

        bool shouldResliceFromLayer
        {
            get { return m_ResliceFromLayer && m_MosaicLayers && spriteImportMode == SpriteImportMode.Multiple; }
        }

        bool characterMode
        {
            get { return mosaicMode && m_CharacterMode == true; }
        }

        float definitionScale
        {
            get
            {
                float definitionScaleW = m_ImportedTextureWidth / (float)textureActualWidth;
                float definitionScaleH = m_ImportedTextureHeight / (float)textureActualHeight;
                return Mathf.Min(definitionScaleW, definitionScaleH);
            }
        }

        private Vector2 GetPivotPoint(Rect rect, SpriteAlignment alignment)
        {
            switch (alignment)
            {
                case SpriteAlignment.TopLeft:
                    return new Vector2(rect.xMin, rect.yMax);

                case SpriteAlignment.TopCenter:
                    return new Vector2(rect.center.x, rect.yMax);

                case SpriteAlignment.TopRight:
                    return new Vector2(rect.xMax, rect.yMax);

                case SpriteAlignment.LeftCenter:
                    return new Vector2(rect.xMin, rect.center.y);

                case SpriteAlignment.Center:
                    return new Vector2(rect.center.x, rect.center.y);

                case SpriteAlignment.RightCenter:
                    return new Vector2(rect.xMax, rect.center.y);

                case SpriteAlignment.BottomLeft:
                    return new Vector2(rect.xMin, rect.yMin);

                case SpriteAlignment.BottomCenter:
                    return new Vector2(rect.center.x, rect.yMin);

                case SpriteAlignment.BottomRight:
                    return new Vector2(rect.xMax, rect.yMin);

                case SpriteAlignment.Custom:
                    return new Vector2(m_DocumentPivot.x * rect.width, m_DocumentPivot.y * rect.height);
            }
            return Vector2.zero;
        }

        void CreateBoneGO(int index, SpriteBone[] bones, BoneGO[] bonesGO, Transform defaultRoot)
        {
            if (bonesGO[index].go != null)
                return;
            var bone = bones[index];
            if (bone.parentId != -1 && bonesGO[bone.parentId].go == null)
                CreateBoneGO(bone.parentId, bones, bonesGO, defaultRoot);

            var go = m_GameObjectFactory.CreateGameObject(bone.name);
            if (bone.parentId == -1)
                go.transform.SetParent(defaultRoot);
            else
                go.transform.SetParent(bonesGO[bone.parentId].go.transform);
            go.transform.localPosition = bone.position * 1 / pixelsPerUnit;
            go.transform.localRotation = bone.rotation;
            bonesGO[index] = new BoneGO()
            {
                go = go,
                index = index
            };
        }

        BoneGO[] CreateBonesGO(Transform root)
        {
            if (characterMode)
            {
                var characterSkeleton = GetDataProvider<ICharacterDataProvider>().GetCharacterData();
                var bones = characterSkeleton.bones;
                if (bones != null)
                {
                    var boneGOs = new BoneGO[bones.Length];
                    for (int i = 0; i < bones.Length; ++i)
                    {
                        CreateBoneGO(i, bones, boneGOs, root);
                    }
                    return boneGOs;
                }
            }
            return new BoneGO[0];
        }

        void GetSpriteLiblabel(string spriteId, out string category, out string label)
        {
            category = "";
            label = "";
            var index = -1;
            foreach (var cat in m_SpriteCategoryList.categories)
            {
                index = cat.labels.FindIndex(x => x.spriteId == spriteId);
                if (index != -1)
                {
                    category = cat.name;
                    label = cat.labels[index].name;
                    break;
                }
            }
        }

        GameObject OnProducePaperDollPrefab(string assetname, Sprite[] sprites, SpriteLibraryAsset spriteLib)
        {
            GameObject root = null;
            CharacterData? characterSkeleton = characterMode ? new CharacterData ? (GetDataProvider<ICharacterDataProvider>().GetCharacterData()) : null;
            if (sprites != null && sprites.Length > 0)
            {
                root = new GameObject();
                root.name = assetname + "_GO";
                var spriteImportData = GetSpriteImportData();
                var psdLayers = GetPSDLayers();
                var boneGOs = CreateBonesGO(root.transform);
                if (spriteLib != null)
                    root.AddComponent<SpriteLibrary>().spriteLibraryAsset = spriteLib;
                for (int i = 0; i < sprites.Length; ++i)
                {
                    string categoryName;
                    if (SpriteIsMainFromSpriteLib(sprites[i].GetSpriteID().ToString(), out categoryName))
                    {
                        var spriteBones = m_CharacterData.parts.FirstOrDefault(x => new GUID(x.spriteId) == sprites[i].GetSpriteID()).bones;
                        var rootBone = root;
                        if (spriteBones != null && spriteBones.Any())
                        {
                            var b = spriteBones.Where(x => x >= 0 && x < boneGOs.Length).Select(x => boneGOs[x]).OrderBy(x => x.index);
                            if (b.Any())
                                rootBone = b.First().go;
                        }

                        var srGameObject = m_GameObjectFactory.CreateGameObject(string.IsNullOrEmpty(categoryName) ? sprites[i].name : categoryName);
                        var sr = srGameObject.AddComponent<SpriteRenderer>();
                        sr.sprite = sprites[i];
                        sr.sortingOrder = psdLayers.Count - psdLayers.FindIndex(x => x.spriteID == sprites[i].GetSpriteID());
                        srGameObject.transform.parent = rootBone.transform;
                        var spriteMetaData = spriteImportData.FirstOrDefault(x => x.spriteID == sprites[i].GetSpriteID());
                        if (spriteMetaData != null)
                        {
                            var uvTransform = spriteMetaData.uvTransform;
                            var outlineOffset = new Vector2(spriteMetaData.rect.x - uvTransform.x + (spriteMetaData.pivot.x * spriteMetaData.rect.width),
                                spriteMetaData.rect.y - uvTransform.y + (spriteMetaData.pivot.y * spriteMetaData.rect.height)) * definitionScale / sprites[i].pixelsPerUnit;
                            srGameObject.transform.position = new Vector3(outlineOffset.x, outlineOffset.y, 0);
                        }
                        var category = "";
                        var labelName = "";
                        GetSpriteLiblabel(sprites[i].GetSpriteID().ToString(), out category, out labelName);
                        if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(labelName))
                        {
                            var sresolver = srGameObject.AddComponent<SpriteResolver>();
                            sresolver.SetCategoryAndLabel(category, labelName);
                            sresolver.ResolveSpriteToSpriteRenderer();
                        }
                    }
                }
            }
            return root;
        }

        internal void SetPlatformTextureSettings(TextureImporterPlatformSettings platformSettings)
        {
            var index = m_PlatformSettings.FindIndex(x => x.name == platformSettings.name);
            if(index < 0)
                m_PlatformSettings.Add(platformSettings);
            else
                m_PlatformSettings[index] = platformSettings;
        }

        internal TextureImporterPlatformSettings[] GetAllPlatformSettings()
        {
            return m_PlatformSettings.ToArray();
        }

        GameObject OnProducePrefab(string assetname, Sprite[] sprites, SpriteLibraryAsset spriteLib)
        {
            GameObject root = null;
            CharacterData? characterSkeleton = characterMode ? new CharacterData ? (GetDataProvider<ICharacterDataProvider>().GetCharacterData()) : null;
            if (sprites != null && sprites.Length > 0)
            {
                var spriteImportData = GetSpriteImportData();
                root = new GameObject();
                root.transform.SetSiblingIndex(0);
                root.name = assetname + "_GO";
                if (spriteLib != null)
                    root.AddComponent<SpriteLibrary>().spriteLibraryAsset = spriteLib;

                var psdLayers = GetPSDLayers();
                for (int i = 0; i < psdLayers.Count; ++i)
                {
                    BuildGroupGameObject(psdLayers, i, root.transform);
                }
                var boneGOs = CreateBonesGO(root.transform);
                for (int i = 0; i < psdLayers.Count; ++i)
                {
                    var l = psdLayers[i];
                    GUID layerSpriteID = l.spriteID;
                    var sprite = sprites.FirstOrDefault(x => x.GetSpriteID() == layerSpriteID);
                    var spriteMetaData = spriteImportData.FirstOrDefault(x => x.spriteID == layerSpriteID);
                    if (sprite != null && spriteMetaData != null && l.gameObject != null)
                    {
                        var spriteRenderer = l.gameObject.AddComponent<SpriteRenderer>();
                        spriteRenderer.sprite = sprite;
                        spriteRenderer.sortingOrder = psdLayers.Count - i;
                        var uvTransform = spriteMetaData.uvTransform;
                        var outlineOffset = new Vector2(spriteMetaData.rect.x - uvTransform.x + (spriteMetaData.pivot.x * spriteMetaData.rect.width),
                            spriteMetaData.rect.y - uvTransform.y + (spriteMetaData.pivot.y * spriteMetaData.rect.height)) * definitionScale / sprite.pixelsPerUnit;
                        l.gameObject.transform.position = new Vector3(outlineOffset.x, outlineOffset.y, 0);

                        if (characterSkeleton != null)
                        {
                            var part = characterSkeleton.Value.parts.FirstOrDefault(x => x.spriteId == spriteMetaData.spriteID.ToString());
                            if (part.bones != null && part.bones.Length > 0)
                            {
                                var spriteSkin = l.gameObject.AddComponent<SpriteSkin>();
                                if (spriteRenderer.sprite != null && spriteRenderer.sprite.GetBindPoses().Length > 0)
                                {
                                    var spriteBones = m_CharacterData.parts.FirstOrDefault(x => new GUID(x.spriteId) == spriteRenderer.sprite.GetSpriteID()).bones.Where(x => x >= 0 && x < boneGOs.Length).Select(x => boneGOs[x]);
                                    if (spriteBones.Any())
                                    {
                                        spriteSkin.rootBone = spriteBones.OrderBy(x => x.index).First().go.transform;
                                        spriteSkin.boneTransforms = spriteBones.Select(x => x.go.transform).ToArray();
                                        if (spriteSkin.isValid)
                                            spriteSkin.CalculateBounds();
                                    }
                                }
                            }
                        }

                        var category = "";
                        var labelName = "";
                        GetSpriteLiblabel(layerSpriteID.ToString(), out category, out labelName);
                        if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(labelName))
                        {
                            var sresolver = l.gameObject.AddComponent<SpriteResolver>();
                            sresolver.SetCategoryAndLabel(category, labelName);
                            sresolver.ResolveSpriteToSpriteRenderer();
                        }
                    }
                }

                var prefabBounds = new Rect(0 , 0, m_DocumentSize.x / pixelsPerUnit, m_DocumentSize.y / pixelsPerUnit);
                var documentPivot = (Vector3)GetPivotPoint(prefabBounds, m_DocumentAlignment);
                for (int i = 0; i < psdLayers.Count; ++i)
                {
                    var l = psdLayers[i];
                    if (l.gameObject == null || l.gameObject.GetComponent<SpriteRenderer>() == null)
                        continue;
                    var p = l.gameObject.transform.localPosition;
                    p -= documentPivot;
                    l.gameObject.transform.localPosition = p;
                }
                for (int i = 0; i < boneGOs.Length; ++i)
                {
                    if (boneGOs[i].go.transform.parent != root.transform)
                        continue;
                    var p = boneGOs[i].go.transform.position;
                    p -= documentPivot;
                    boneGOs[i].go.transform.position = p;
                }
            }

            return root;
        }

        Bounds? GetBoundingBox(GameObject root)
        {
            Bounds? prefabBounds1 = null;
            var sr = root.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                prefabBounds1 = sr.bounds;
            }
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var b = GetBoundingBox(root.transform.GetChild(i).gameObject);
                if (prefabBounds1 == null)
                    prefabBounds1 = b;
                else
                {
                    if (b.HasValue)
                    {
                        var bb = prefabBounds1.Value;
                        bb.Encapsulate(b.Value);
                        prefabBounds1 = bb;
                    }
                }
            }
            return prefabBounds1;
        }

        bool CleanUpGameobjectsWithOutRig(GameObject root)
        {
            var sr = root.GetComponent<SpriteRenderer>();
            var canDelete = true;
            if (sr != null && sr.sprite != null)
            {
                try
                {
                    var bones = GetDataProvider<ISpriteBoneDataProvider>().GetBones(sr.sprite.GetSpriteID());
                    canDelete = bones == null || bones.Count == 0;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }


            List<GameObject> cleanup = new List<GameObject>();
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                var go = root.transform.GetChild(i);
                if (CleanUpGameobjectsWithOutRig(go.gameObject))
                    cleanup.Add(go.gameObject);
            }

            for (int i = 0; i < cleanup.Count; ++i)
                GameObject.DestroyImmediate(cleanup[i]);
            cleanup.Clear();

            if (root.transform.childCount == 0 && canDelete)
                return true;

            return false;
        }

        static string SanitizeName(string name)
        {
            string newName = null;
            // We can't create asset name with these name.
            if ((name.Length == 2 && name[0] == '.' && name[1] == '.')
                || (name.Length == 1 && name[0] == '.')
                || (name.Length == 1 && name[0] == '/'))
                newName += name + "_";

            if (!string.IsNullOrEmpty(newName))
            {
                Debug.LogWarning(string.Format("File contains layer with invalid name for generating asset. {0} is renamed to {1}", name, newName));
                return newName;
            }
            return name;
        }

        static string GetUniqueName(string name, List<int> stringHash, bool logNewNameGenerated = false, UnityEngine.Object context = null)
        {
            string uniqueName = string.Copy(SanitizeName(name));
            int index = 1;
            while (true)
            {
                int hash = uniqueName.GetHashCode();
                var p = stringHash.Where(x => x == hash);
                if (!p.Any())
                {
                    stringHash.Add(hash);
                    if (logNewNameGenerated && name != uniqueName)
                        Debug.Log(string.Format("Asset name {0} is changed to {1} to ensure uniqueness", name, uniqueName), context);
                    return uniqueName;
                }
                uniqueName = string.Format("{0}_{1}", name, index);
                ++index;
            }
        }

        // ISpriteEditorDataProvider interface
        internal SpriteImportMode spriteImportMode
        {
            get
            {
                return m_TextureImporterSettings.textureType != TextureImporterType.Sprite ?
                    SpriteImportMode.None :
                    (SpriteImportMode)m_TextureImporterSettings.spriteMode;
            }
        }

        SpriteImportMode ISpriteEditorDataProvider.spriteImportMode => spriteImportMode;

        internal int spriteDataCount
        {
            get
            {
                var spriteImportData = GetSpriteImportData();
                if (mosaicMode)
                    return spriteImportData.Count;
                if (spriteImportMode != SpriteImportMode.Multiple)
                    return 1;
                return spriteImportData.Count - 1;
            }
        }

        internal UnityEngine.Object targetObject
        {
            get { return this; }
        }
        UnityEngine.Object ISpriteEditorDataProvider.targetObject => targetObject;

        internal float pixelsPerUnit
        {
            get { return m_TextureImporterSettings.spritePixelsPerUnit; }
        }

        float ISpriteEditorDataProvider.pixelsPerUnit =>pixelsPerUnit;

        internal T GetDataProvider<T>() where T : class
        {
            if (typeof(T) == typeof(ISpriteBoneDataProvider))
            {
                return new SpriteBoneDataProvider { dataProvider = this } as T;
            }
            if (typeof(T) == typeof(ISpriteMeshDataProvider))
            {
                return new SpriteMeshDataProvider { dataProvider = this } as T;
            }
            if (typeof(T) == typeof(ISpriteOutlineDataProvider))
            {
                return new SpriteOutlineDataProvider { dataProvider = this } as T;
            }
            if (typeof(T) == typeof(ISpritePhysicsOutlineDataProvider))
            {
                return new SpritePhysicsOutlineProvider { dataProvider = this } as T;
            }
            if (typeof(T) == typeof(ITextureDataProvider))
            {
                return new TextureDataProvider { dataProvider = this } as T;
            }
            if (typeof(T) == typeof(ICharacterDataProvider))
            {
                return characterMode ? new CharacterDataProvider { dataProvider = this } as T : null;
            }
            if (typeof(T) == typeof(ISpriteLibDataProvider))
            {
                return new SpriteLibraryDataProvider() { dataProvider = this } as T;
            }
            if (typeof(T) == typeof(ISecondaryTextureDataProvider))
            {
                return new SecondaryTextureDataProvider() { dataProvider = this } as T;
            }
            else
                return this as T;
        }

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.GetDataProvider.
        /// </summary>
        /// <typeparam name="T">Data provider type to retrieve.</typeparam>
        /// <returns></returns>
        T ISpriteEditorDataProvider.GetDataProvider<T>()
        {
            return GetDataProvider<T>();
        }

        internal bool HasDataProvider(Type type)
        {
            if (characterMode && type == typeof(ICharacterDataProvider))
                return true;
            if (type == typeof(ISpriteBoneDataProvider) ||
                type == typeof(ISpriteMeshDataProvider) ||
                type == typeof(ISpriteOutlineDataProvider) ||
                type == typeof(ISpritePhysicsOutlineDataProvider) ||
                type == typeof(ITextureDataProvider) ||
                type == typeof(ISpriteLibDataProvider) ||
                type == typeof(ISecondaryTextureDataProvider))
            {
                return true;
            }
            else
                return type.IsAssignableFrom(GetType());
        }

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.HasDataProvider.
        /// </summary>
        /// <param name="type">Data provider type to query.</param>
        /// <returns>True if data provider is supported, false otherwise.</returns>
        bool ISpriteEditorDataProvider.HasDataProvider(Type type)
        {
            return HasDataProvider(type);
        }

        internal void AddSpriteData(SpriteRect spriteRect)
        {
            if (spriteImportMode != SpriteImportMode.Multiple)
                Debug.LogWarning("Can only add sprite data when import mode is multiple");
            else
            {
                GetSpriteImportData().Add(new SpriteMetaData(spriteRect));
            }
        }

        internal void DeleteSpriteData(SpriteRect spriteRect)
        {
            if (spriteImportMode != SpriteImportMode.Multiple)
                Debug.LogWarning("Can only add sprite data when import mode is multiple");
            else
            {
                var spriteImportData = GetSpriteImportData();
                int index = spriteImportData.FindIndex(x => x.spriteID == spriteRect.spriteID);
                Assert.AreEqual(0, index, "Cannot delete Sprite from single sprite mode");
                spriteImportData.RemoveAt(index);
            }
        }

        internal int GetSpriteDataIndex(GUID guid)
        {
            switch (spriteImportMode)
            {
                case SpriteImportMode.Single:
                case SpriteImportMode.Polygon:
                    return 0;
                case SpriteImportMode.Multiple:
                {
                    var spriteImportData = GetSpriteImportData();
                    return spriteImportData.FindIndex(x => x.spriteID == guid);
                }
                default:
                    throw new InvalidOperationException("GUID not found");
            }
        }

        internal void Apply()
        {
            // Do this so that asset change save dialog will not show
            var originalValue = EditorPrefs.GetBool("VerifySavingAssets", false);
            EditorPrefs.SetBool("VerifySavingAssets", false);
            AssetDatabase.ForceReserializeAssets(new string[] { assetPath }, ForceReserializeAssetsOptions.ReserializeMetadata);
            EditorPrefs.SetBool("VerifySavingAssets", originalValue);
        }


        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.Apply.
        /// </summary>
        void ISpriteEditorDataProvider.Apply()
        {
            Apply();
        }

        internal void InitSpriteEditorDataProvider() {}


        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.InitSpriteEditorDataProvider.
        /// </summary>
        void ISpriteEditorDataProvider.InitSpriteEditorDataProvider()
        {
            InitSpriteEditorDataProvider();
        } 

        internal SpriteRect[] GetSpriteRects()
        {
            var spriteImportData = GetSpriteImportData();
            var skip = mosaicMode ? 0 : 1;
            return spriteImportMode == SpriteImportMode.Multiple ? spriteImportData.Skip(skip).Select(x => new SpriteMetaData(x) as SpriteRect).ToArray() : new[] {new SpriteMetaData(spriteImportData[0]) };
        }

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.GetSpriteRects.
        /// </summary>
        /// <returns>An array of SpriteRect for the current import mode.</returns>
        SpriteRect[] ISpriteEditorDataProvider.GetSpriteRects()
        {
            return GetSpriteRects();
        }

        List<SpriteMetaData> GetSpriteImportData()
        {
            return mosaicMode ? (characterMode ? m_RigSpriteImportData : m_MosaicSpriteImportData) : m_SpriteImportData;
        }

        internal List<PSDLayer> GetPSDLayers()
        {
            return mosaicMode ? (characterMode ? m_RigPSDLayers : m_MosaicPSDLayers) : null;
        }

        internal SpriteMetaData[] GetSpriteMetaData()
        {
            var spriteImportData = GetSpriteImportData();
            var skip = mosaicMode ? 0 : 1;
            return spriteImportMode == SpriteImportMode.Multiple ? spriteImportData.Skip(skip).ToArray() : new[] { new SpriteMetaData(spriteImportData[0]) };
        }

        internal SpriteRect GetSpriteDataFromAllMode(GUID guid)
        {
            var spriteMetaData = m_RigSpriteImportData.FirstOrDefault(x => x.spriteID == guid);
            if(spriteMetaData == null)
                spriteMetaData = m_MosaicSpriteImportData.FirstOrDefault(x => x.spriteID == guid);
            if(spriteMetaData == null)
                spriteMetaData = m_SpriteImportData.FirstOrDefault(x => x.spriteID == guid);
            return spriteMetaData;
        }
        
        internal SpriteRect GetSpriteData(GUID guid)
        {
            var spriteImportData = GetSpriteImportData();
            var skip = mosaicMode ? 0 : 1;
            return spriteImportMode == SpriteImportMode.Multiple ? spriteImportData.Skip(skip).FirstOrDefault(x => x.spriteID == guid) : spriteImportData[0];
        }

        internal void SetSpriteRects(SpriteRect[] spriteRects)
        {
            var spriteImportData = GetSpriteImportData();
            if (spriteImportMode == SpriteImportMode.Multiple)
            {
                var singleSpriteID = mosaicMode ? new GUID() : spriteImportData[0].spriteID;
                spriteImportData.RemoveAll(data => data.spriteID != singleSpriteID && spriteRects.FirstOrDefault(x => x.spriteID == data.spriteID) == null);
                foreach (var sr in spriteRects)
                {
                    var importData = spriteImportData.FirstOrDefault(x => x.spriteID == sr.spriteID);
                    if (importData == null)
                        spriteImportData.Add(new SpriteMetaData(sr));
                    else
                    {
                        importData.name = sr.name;
                        importData.alignment = sr.alignment;
                        importData.border = sr.border;
                        importData.pivot = sr.pivot;
                        importData.rect = sr.rect;
                    }
                }
            }
            else if (spriteRects.Length == 1 && (spriteImportMode == SpriteImportMode.Single || spriteImportMode == SpriteImportMode.Polygon))
            {
                if (spriteImportData[0].spriteID == spriteRects[0].spriteID)
                {
                    spriteImportData[0].name = spriteRects[0].name;
                    spriteImportData[0].alignment = spriteRects[0].alignment;
                    m_TextureImporterSettings.spriteAlignment = (int)spriteRects[0].alignment;
                    m_TextureImporterSettings.spriteBorder = spriteImportData[0].border = spriteRects[0].border;
                    m_TextureImporterSettings.spritePivot = spriteImportData[0].pivot = spriteRects[0].pivot;
                    spriteImportData[0].rect = spriteRects[0].rect;
                }
                else
                {
                    spriteImportData[0] = new SpriteMetaData(spriteRects[0]);
                }
            }
        }

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.SetSpriteRects.
        /// </summary>
        /// <param name="spriteRects">Set the SpriteRect data for the current import mode.</param>
        void ISpriteEditorDataProvider.SetSpriteRects(SpriteRect[] spriteRects)
        {
            SetSpriteRects(spriteRects);
        }

        bool mosaicMode
        {
            get { return spriteImportMode == SpriteImportMode.Multiple && m_MosaicLayers; }
        }

        internal CharacterData characterData
        {
            get { return m_CharacterData; }
            set { m_CharacterData = value; }
        }

        internal Vector2Int documentSize
        {
            get { return m_DocumentSize; }
        }

        SpriteLibraryAsset ProduceSpriteLibAsset(Sprite[] sprites)
        {
            if (!characterMode || m_SpriteCategoryList.categories == null)
                return null;
            var sla = ScriptableObject.CreateInstance<SpriteLibraryAsset>();
            sla.name = "Sprite Lib";
            sla.categories = m_SpriteCategoryList.categories.Select(x =>
                new SpriteLibCategory()
                {
                    name = x.name,
                    categoryList = x.labels.Select(y =>
                    {
                        var sprite = sprites.FirstOrDefault(z => z.GetSpriteID().ToString() == y.spriteId);
                        return new Categorylabel()
                        {
                            name = y.name,
                            sprite = sprite
                        };
                    }).ToList()
                }).ToList();
            sla.categories.RemoveAll(x => x.categoryList.Count == 0);
            if (sla.categories.Count > 0)
            {
                sla.UpdateHashes();
                return sla;
            }
            return null;
        }

        internal SecondarySpriteTexture[] secondaryTextures
        {
            get { return m_SecondarySpriteTextures; }
            set { m_SecondarySpriteTextures = value; }
        }

        internal void ReadTextureSettings(TextureImporterSettings dest)
        {
            m_TextureImporterSettings.CopyTo(dest);
        }
    }
}
