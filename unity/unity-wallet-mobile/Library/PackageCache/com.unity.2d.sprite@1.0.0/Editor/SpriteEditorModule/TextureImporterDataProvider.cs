using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Sprites
{
    internal class TextureImporterDataProviderFactory :
        ISpriteDataProviderFactory<Texture2D>,
        ISpriteDataProviderFactory<Sprite>,
        ISpriteDataProviderFactory<TextureImporter>,
        ISpriteDataProviderFactory<GameObject>
    {
        public ISpriteEditorDataProvider CreateDataProvider(Texture2D obj)
        {
            var ti = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as TextureImporter;
            if (ti != null)
                return new TextureImporterDataProvider(ti);
            return null;
        }

        public ISpriteEditorDataProvider CreateDataProvider(Sprite obj)
        {
            var ti = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as TextureImporter;
            if (ti != null)
                return new TextureImporterDataProvider(ti);
            return null;
        }

        public ISpriteEditorDataProvider CreateDataProvider(GameObject obj)
        {
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                var ti = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(spriteRenderer.sprite)) as TextureImporter;
                if (ti != null)
                    return new TextureImporterDataProvider(ti);
            }
            return null;
        }

        public ISpriteEditorDataProvider CreateDataProvider(TextureImporter obj)
        {
            return new TextureImporterDataProvider(obj);
        }

        [SpriteEditorAssetPathProviderAttribute]
        static string GetAssetPathFromSpriteRenderer(UnityEngine.Object obj)
        {
            var go = obj as GameObject;
            if (go != null)
            {
                var spriteRenderer = go.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                    return AssetDatabase.GetAssetPath(spriteRenderer.sprite);
            }
            return null;
        }

        [SpriteObjectProvider]
        static Sprite GetSpriteObjectFromSpriteRenderer(UnityEngine.Object obj)
        {
            var go = obj as GameObject;
            if (go != null)
            {
                var spriteRenderer = go.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                    return spriteRenderer.sprite;
            }
            return null;
        }
    }

    internal class TextureImporterDataProvider : ISpriteEditorDataProvider
    {
        TextureImporter m_TextureImporter;
        List<SpriteDataExt> m_SpritesMultiple;
        SpriteDataExt m_SpriteSingle;
        SpriteImportMode m_SpriteImportMode = SpriteImportMode.None;
        SecondarySpriteTexture[] m_SecondaryTextureDataTransfer;

        internal TextureImporterDataProvider(TextureImporter importer)
        {
            m_TextureImporter = importer;
            if (m_TextureImporter != null)
                m_SpriteImportMode = m_TextureImporter.spriteImportMode;
        }

        float ISpriteEditorDataProvider.pixelsPerUnit
        {
            get { return m_TextureImporter.spritePixelsPerUnit; }
        }

        UnityEngine.Object ISpriteEditorDataProvider.targetObject
        {
            get { return m_TextureImporter; }
        }

        public SpriteImportMode spriteImportMode
        {
            get { return m_SpriteImportMode; }
        }

        SpriteRect[] ISpriteEditorDataProvider.GetSpriteRects()
        {
            return spriteImportMode == SpriteImportMode.Multiple ? m_SpritesMultiple.Select(x => new SpriteDataExt(x) as SpriteRect).ToArray() : new[] { new SpriteDataExt(m_SpriteSingle) };
        }

        public SerializedObject GetSerializedObject()
        {
            return new SerializedObject(m_TextureImporter);
        }

        public string assetPath
        {
            get { return m_TextureImporter.assetPath; }
        }

        public void GetWidthAndHeight(ref int width, ref int height)
        {
            m_TextureImporter.GetWidthAndHeight(ref width, ref height);
        }

        void ISpriteEditorDataProvider.SetSpriteRects(SpriteRect[] spriteRects)
        {
            if (spriteImportMode != SpriteImportMode.Multiple && spriteImportMode != SpriteImportMode.None && spriteRects.Length == 1)
            {
                m_SpriteSingle.CopyFromSpriteRect(spriteRects[0]);
            }
            else if (spriteImportMode == SpriteImportMode.Multiple)
            {
                for (int i = m_SpritesMultiple.Count - 1; i >= 0; --i)
                {
                    var spriteID = m_SpritesMultiple[i].spriteID;
                    if (spriteRects.FirstOrDefault(x => x.spriteID == spriteID) == null)
                        m_SpritesMultiple.RemoveAt(i);
                }
                for (int i = 0; i < spriteRects.Length; i++)
                {
                    var spriteRect = spriteRects[i];
                    var index = m_SpritesMultiple.FindIndex(x => x.spriteID == spriteRect.spriteID);
                    if (-1 == index)
                        m_SpritesMultiple.Add(new SpriteDataExt(spriteRect));
                    else
                        m_SpritesMultiple[index].CopyFromSpriteRect(spriteRects[i]);
                }
            }
        }

        internal SpriteRect GetSpriteData(GUID guid)
        {
            return spriteImportMode == SpriteImportMode.Multiple ? m_SpritesMultiple.FirstOrDefault(x => x.spriteID == guid) : m_SpriteSingle;
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
                    return m_SpritesMultiple.FindIndex(x => x.spriteID == guid);
                }
                default:
                    throw new InvalidOperationException(string.Format("Sprite with GUID {0} not found", guid));
            }
        }

        void ISpriteEditorDataProvider.Apply()
        {
            var so = new SerializedObject(m_TextureImporter);
            m_SpriteSingle.Apply(so);
            var spriteSheetSO = so.FindProperty("m_SpriteSheet.m_Sprites");
            GUID[] guids = new GUID[spriteSheetSO.arraySize];
            for (int i = 0; i < spriteSheetSO.arraySize; ++i)
            {
                var element = spriteSheetSO.GetArrayElementAtIndex(i);
                guids[i] = SpriteRect.GetSpriteIDFromSerializedProperty(element);
                // find the GUID in our sprite list and apply to it;
                var smd = m_SpritesMultiple.Find(x => x.spriteID == guids[i]);
                if (smd == null) // we can't find it, it is already deleted
                {
                    spriteSheetSO.DeleteArrayElementAtIndex(i);
                    --i;
                }
                else
                    smd.Apply(element);
            }

            // Add new ones
            var newSprites = m_SpritesMultiple.Where(x => !guids.Contains(x.spriteID));
            foreach (var newSprite in newSprites)
            {
                spriteSheetSO.InsertArrayElementAtIndex(spriteSheetSO.arraySize);
                var element = spriteSheetSO.GetArrayElementAtIndex(spriteSheetSO.arraySize - 1);
                newSprite.Apply(element);
            }

            SpriteSecondaryTextureDataTransfer.Apply(so, m_SecondaryTextureDataTransfer);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        void ISpriteEditorDataProvider.InitSpriteEditorDataProvider()
        {
            var so = new SerializedObject(m_TextureImporter);
            var spriteSheetSO = so.FindProperty("m_SpriteSheet.m_Sprites");
            m_SpritesMultiple = new List<SpriteDataExt>();
            m_SpriteSingle = new SpriteDataExt(so);

            for (int i = 0; i < spriteSheetSO.arraySize; ++i)
            {
                var sp = spriteSheetSO.GetArrayElementAtIndex(i);
                var data = new SpriteDataExt(sp);
                m_SpritesMultiple.Add(data);
            }
            m_SecondaryTextureDataTransfer = SpriteSecondaryTextureDataTransfer.Load(so);
        }

        T ISpriteEditorDataProvider.GetDataProvider<T>()
        {
            if (typeof(T) == typeof(ISpriteBoneDataProvider))
            {
                return new SpriteBoneDataTransfer(this) as T;
            }
            if (typeof(T) == typeof(ISpriteMeshDataProvider))
            {
                return new SpriteMeshDataTransfer(this) as T;
            }
            if (typeof(T) == typeof(ISpriteOutlineDataProvider))
            {
                return new SpriteOutlineDataTransfer(this) as T;
            }
            if (typeof(T) == typeof(ISpritePhysicsOutlineDataProvider))
            {
                return new SpritePhysicsOutlineDataTransfer(this) as T;
            }
            if (typeof(T) == typeof(ITextureDataProvider))
            {
                return new SpriteTextureDataTransfer(this) as T;
            }
            if (typeof(T) == typeof(ISecondaryTextureDataProvider))
            {
                return new SpriteSecondaryTextureDataTransfer(this) as T;
            }
            else
                return this as T;
        }

        bool ISpriteEditorDataProvider.HasDataProvider(Type type)
        {
            if (type == typeof(ISpriteBoneDataProvider) ||
                type == typeof(ISpriteMeshDataProvider) ||
                type == typeof(ISpriteOutlineDataProvider) ||
                type == typeof(ISpritePhysicsOutlineDataProvider) ||
                type == typeof(ITextureDataProvider) ||
                type == typeof(ISecondaryTextureDataProvider))
            {
                return true;
            }
            else
                return type.IsAssignableFrom(GetType());
        }

        public override bool Equals(object a)
        {
            return this == (a as TextureImporterDataProvider);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator!=(TextureImporterDataProvider t1, TextureImporterDataProvider t2)
        {
            return !(t1 == t2);
        }

        public static bool operator==(TextureImporterDataProvider t1, TextureImporterDataProvider t2)
        {
            if (ReferenceEquals(null, t1) && (!ReferenceEquals(null, t2) && t2.m_TextureImporter == null) ||
                ReferenceEquals(null, t2) && (!ReferenceEquals(null, t1) && t1.m_TextureImporter == null))
                return true;

            if (!ReferenceEquals(null, t1) && !ReferenceEquals(null, t2))
            {
                if (t1.m_TextureImporter == null && t2.m_TextureImporter == null ||
                    t1.m_TextureImporter == t2.m_TextureImporter)
                    return true;
            }
            if (ReferenceEquals(t1, t2))
                return true;

            return false;
        }

        public SecondarySpriteTexture[] secdonaryTextures
        {
            get { return m_SecondaryTextureDataTransfer; }
            set { m_SecondaryTextureDataTransfer = value; }
        }
    }
}
