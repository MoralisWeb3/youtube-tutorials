using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor.U2D.Common;
using UnityEditor.U2D.Animation;
using System;
using UnityEditor.U2D.Sprites;
using UnityEngine.U2D;

namespace UnityEditor.U2D.PSD
{
    internal abstract class PSDDataProvider
    {
        public PSDImporter dataProvider;
    }

    internal class SpriteBoneDataProvider : PSDDataProvider, ISpriteBoneDataProvider
    {
        public List<SpriteBone> GetBones(GUID guid)
        {
            var sprite = ((SpriteMetaData)dataProvider.GetSpriteData(guid));
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));
            return sprite.spriteBone != null ? sprite.spriteBone.ToList() : new List<SpriteBone>();
        }

        public void SetBones(GUID guid, List<SpriteBone> bones)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if (sprite != null)
                ((SpriteMetaData)sprite).spriteBone = bones;
        }
    }

    internal class TextureDataProvider : PSDDataProvider, ITextureDataProvider
    {
        Texture2D m_ReadableTexture;
        Texture2D m_OriginalTexture;

        PSDImporter textureImporter { get { return (PSDImporter)dataProvider.targetObject; } }

        public Texture2D texture
        {
            get
            {
                if (m_OriginalTexture == null)
                    m_OriginalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(textureImporter.assetPath);
                return m_OriginalTexture;
            }
        }

        public Texture2D previewTexture
        {
            get { return texture; }
        }

        public Texture2D GetReadableTexture2D()
        {
            if (m_ReadableTexture == null)
            {
                m_ReadableTexture = InternalEditorBridge.CreateTemporaryDuplicate(texture, texture.width, texture.height);
                if (m_ReadableTexture != null)
                    m_ReadableTexture.filterMode = texture.filterMode;
            }
            return m_ReadableTexture;
        }

        public void GetTextureActualWidthAndHeight(out int width, out int height)
        {
            width = dataProvider.textureActualWidth;
            height = dataProvider.textureActualHeight;
        }
    }

    internal class SecondaryTextureDataProvider : PSDDataProvider, ISecondaryTextureDataProvider
    {
        public SecondarySpriteTexture[] textures
        {
            get { return dataProvider.secondaryTextures; }
            set { dataProvider.secondaryTextures = value; }
        }
    }

    internal class SpriteOutlineDataProvider : PSDDataProvider, ISpriteOutlineDataProvider
    {
        public List<Vector2[]> GetOutlines(GUID guid)
        {
            var sprite = ((SpriteMetaData)dataProvider.GetSpriteData(guid));
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));

            var outline = sprite.spriteOutline;
            if (outline != null)
                return outline.Select(x => x.outline).ToList();
            return new List<Vector2[]>();
        }

        public void SetOutlines(GUID guid, List<Vector2[]> data)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if (sprite != null)
                ((SpriteMetaData)sprite).spriteOutline = data.Select(x => new SpriteOutline() {outline = x}).ToList();
        }

        public float GetTessellationDetail(GUID guid)
        {
            return ((SpriteMetaData)dataProvider.GetSpriteData(guid)).tessellationDetail;
        }

        public void SetTessellationDetail(GUID guid, float value)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if (sprite != null)
                ((SpriteMetaData)sprite).tessellationDetail = value;
        }
    }

    internal class SpritePhysicsOutlineProvider : PSDDataProvider, ISpritePhysicsOutlineDataProvider
    {
        public List<Vector2[]> GetOutlines(GUID guid)
        {
            var sprite = ((SpriteMetaData)dataProvider.GetSpriteData(guid));
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));
            var outline = sprite.spritePhysicsOutline;
            if (outline != null)
                return outline.Select(x => x.outline).ToList();

            return new List<Vector2[]>();
        }

        public void SetOutlines(GUID guid, List<Vector2[]> data)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if (sprite != null)
                ((SpriteMetaData)sprite).spritePhysicsOutline = data.Select(x => new SpriteOutline() { outline = x }).ToList();
        }

        public float GetTessellationDetail(GUID guid)
        {
            return ((SpriteMetaData)dataProvider.GetSpriteData(guid)).tessellationDetail;
        }

        public void SetTessellationDetail(GUID guid, float value)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if (sprite != null)
                ((SpriteMetaData)sprite).tessellationDetail = value;
        }
    }

    internal class SpriteMeshDataProvider : PSDDataProvider, ISpriteMeshDataProvider
    {
        public Vertex2DMetaData[] GetVertices(GUID guid)
        {
            var sprite = ((SpriteMetaData)dataProvider.GetSpriteData(guid));
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));
            var v = sprite.vertices;
            if (v != null)
                return v.ToArray();

            return new Vertex2DMetaData[0];
        }

        public void SetVertices(GUID guid, Vertex2DMetaData[] vertices)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if (sprite != null)
                ((SpriteMetaData)sprite).vertices = vertices.ToList();
        }

        public int[] GetIndices(GUID guid)
        {
            var sprite = ((SpriteMetaData)dataProvider.GetSpriteData(guid));
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));
            var v = sprite.indices;
            if (v != null)
                return v;

            return new int[0];
        }

        public void SetIndices(GUID guid, int[] indices)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if (sprite != null)
                ((SpriteMetaData)sprite).indices = indices;
        }

        public Vector2Int[] GetEdges(GUID guid)
        {
            var sprite = ((SpriteMetaData)dataProvider.GetSpriteData(guid));
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));
            var v = sprite.edges;
            if (v != null)
                return v;

            return new Vector2Int[0];
        }

        public void SetEdges(GUID guid, Vector2Int[] edges)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if (sprite != null)
                ((SpriteMetaData)sprite).edges = edges;
        }
    }


    internal class CharacterDataProvider : PSDDataProvider, ICharacterDataProvider
    {
        int ParentGroupInFlatten(int parentIndex, List<PSDLayer> psdLayers)
        {
            int group = -1;
            for (int i = 0; i <= parentIndex; ++i)
                if (psdLayers[i].isGroup)
                    ++group;

            return group;
        }

        public CharacterData GetCharacterData()
        {
            var psdLayers = dataProvider.GetPSDLayers();
            var groups = new List<CharacterGroup>();
            for (int i = 0; i < psdLayers.Count; ++i)
            {
                if (psdLayers[i].isGroup)
                {
                    groups.Add(new CharacterGroup()
                    {
                        name = psdLayers[i].name,
                        parentGroup = ParentGroupInFlatten(psdLayers[i].parentIndex, psdLayers),
                        order = i
                    });
                }
            }

            var cd = dataProvider.characterData;
            var parts = cd.parts == null ? new List<CharacterPart>() : cd.parts.ToList();
            var spriteRects = dataProvider.GetSpriteMetaData();
            parts.RemoveAll(x => Array.FindIndex(spriteRects, y => y.spriteID == new GUID(x.spriteId)) == -1);
            foreach (var spriteMetaData in spriteRects)
            {
                var srIndex = parts.FindIndex(x => new GUID(x.spriteId) == spriteMetaData.spriteID);
                CharacterPart cp = srIndex == -1 ? new CharacterPart() : parts[srIndex];
                cp.spriteId = spriteMetaData.spriteID.ToString();
                cp.order = psdLayers.FindIndex(l => l.spriteID == spriteMetaData.spriteID);
                cp.spritePosition = new RectInt();
                var uvTransform = spriteMetaData.uvTransform;
                var outlineOffset = new Vector2(spriteMetaData.rect.x - uvTransform.x, spriteMetaData.rect.y - uvTransform.y);
                cp.spritePosition.position = new Vector2Int((int)outlineOffset.x, (int)outlineOffset.y);
                cp.spritePosition.size = new Vector2Int((int)spriteMetaData.rect.width, (int)spriteMetaData.rect.height);
                cp.parentGroup = -1;
                //Find group
                var spritePSDLayer = psdLayers.FirstOrDefault(x => x.spriteID == spriteMetaData.spriteID);
                if (spritePSDLayer != null)
                {
                    cp.parentGroup = ParentGroupInFlatten(spritePSDLayer.parentIndex, psdLayers);
                }


                if (srIndex == -1)
                    parts.Add(cp);
                else
                    parts[srIndex] = cp;
            }

            var layers = dataProvider.GetPSDLayers();
            parts.Sort((x, y) =>
            {
                return x.order.CompareTo(y.order);
            });

            parts.Reverse();
            cd.parts = parts.ToArray();
            cd.dimension = dataProvider.documentSize;
            cd.characterGroups = groups.ToArray();
            return cd;
        }

        public void SetCharacterData(CharacterData characterData)
        {
            characterData.parts = characterData.parts.Reverse().ToArray();
            dataProvider.characterData = characterData;
        }
    }

    internal class SpriteLibraryDataProvider : PSDDataProvider, ISpriteLibDataProvider
    {
        public SpriteCategoryList GetSpriteCategoryList()
        {
            return dataProvider.spriteCategoryList;
        }

        public void SetSpriteCategoryList(SpriteCategoryList spriteCategoryList)
        {
            dataProvider.spriteCategoryList = spriteCategoryList;
        }
    }
}
