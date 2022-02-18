using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace UnityEditor.U2D.PSD
{
    internal class PSDImportPostProcessor : AssetPostprocessor
    {
        private static string s_CurrentApplyAssetPath = null;
        
        void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            var dataProviderFactories = new SpriteDataProviderFactories();
            dataProviderFactories.Init();
            PSDImporter psd = AssetImporter.GetAtPath(assetPath) as PSDImporter;
            if (psd == null)
                return;
            ISpriteEditorDataProvider importer = dataProviderFactories.GetSpriteEditorDataProviderFromObject(psd);
            if (importer != null)
            {
                importer.InitSpriteEditorDataProvider();
                var physicsOutlineDataProvider = importer.GetDataProvider<ISpritePhysicsOutlineDataProvider>();
                var textureDataProvider = importer.GetDataProvider<ITextureDataProvider>();
                int actualWidth = 0, actualHeight = 0;
                textureDataProvider.GetTextureActualWidthAndHeight(out actualWidth, out actualHeight);
                float definitionScaleW = (float)texture.width / actualWidth;
                float definitionScaleH = (float)texture.height / actualHeight;
                float definitionScale = Mathf.Min(definitionScaleW, definitionScaleH);
                foreach (var sprite in sprites)
                {
                    var guid = sprite.GetSpriteID();
                    var outline = physicsOutlineDataProvider.GetOutlines(guid);
                    var outlineOffset = sprite.rect.size / 2;
                    if (outline != null && outline.Count > 0)
                    {
                        // Ensure that outlines are all valid.
                        int validOutlineCount = 0;
                        for (int i = 0; i < outline.Count; ++i)
                            validOutlineCount = validOutlineCount + ( (outline[i].Length > 2) ? 1 : 0 );

                        int index = 0;
                        var convertedOutline = new Vector2[validOutlineCount][];
                        for (int i = 0; i < outline.Count; ++i)
                        {
                            if (outline[i].Length > 2)
                            {
                                convertedOutline[index] = new Vector2[outline[i].Length];
                                for (int j = 0; j < outline[i].Length; ++j)
                                {
                                    convertedOutline[index][j] = outline[i][j] * definitionScale + outlineOffset;
                                }
                                index++;
                            }
                        }
                        sprite.OverridePhysicsShape(convertedOutline);
                    }
                }
            }
        }
        
        public static string currentApplyAssetPath
        {
            set { s_CurrentApplyAssetPath = value; }
        }
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
        {
            if (!string.IsNullOrEmpty(s_CurrentApplyAssetPath))
            {
                foreach (var asset in importedAssets)
                {
                    if (asset == s_CurrentApplyAssetPath)
                    {
                        var obj = AssetDatabase.LoadMainAssetAtPath(asset);
                        Selection.activeObject = obj;
                        Unsupported.SceneTrackerFlushDirty();
                        s_CurrentApplyAssetPath = null;
                        break;
                    }
                }
            }
        }
    }
}
