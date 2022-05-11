using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.U2D;

namespace UnityEditor.U2D.PSD
{
    [Serializable]
    internal class SpriteMetaData : SpriteRect
    {
        public List<SpriteBone> spriteBone;
        public List<SpriteOutline> spriteOutline;
        public List<Vertex2DMetaData> vertices;
        public List<SpriteOutline> spritePhysicsOutline;
        public int[] indices;
        public Vector2Int[] edges;
        public float tessellationDetail;
        public int parentGroupIndex = -1;
        public Vector2Int uvTransform = Vector2Int.zero;

        public SpriteMetaData() {}

        public SpriteMetaData(SpriteRect sr)
        {
            alignment = sr.alignment;
            border = sr.border;
            name = sr.name;
            pivot = GetPivotValue(sr.alignment, sr.pivot);
            rect = sr.rect;
            spriteID = sr.spriteID;
        }

        public static GUID GetGUIDFromSerializedProperty(SerializedProperty sp)
        {
            return new GUID(sp.FindPropertyRelative("m_SpriteID").stringValue);
        }

        public static Vector2 GetPivotValue(SpriteAlignment alignment, Vector2 customOffset)
        {
            switch (alignment)
            {
                case SpriteAlignment.BottomLeft:
                    return new Vector2(0f, 0f);
                case SpriteAlignment.BottomCenter:
                    return new Vector2(0.5f, 0f);
                case SpriteAlignment.BottomRight:
                    return new Vector2(1f, 0f);

                case SpriteAlignment.LeftCenter:
                    return new Vector2(0f, 0.5f);
                case SpriteAlignment.Center:
                    return new Vector2(0.5f, 0.5f);
                case SpriteAlignment.RightCenter:
                    return new Vector2(1f, 0.5f);

                case SpriteAlignment.TopLeft:
                    return new Vector2(0f, 1f);
                case SpriteAlignment.TopCenter:
                    return new Vector2(0.5f, 1f);
                case SpriteAlignment.TopRight:
                    return new Vector2(1f, 1f);

                case SpriteAlignment.Custom:
                    return customOffset;
            }
            return Vector2.zero;
        }

        public static implicit operator UnityEditor.AssetImporters.SpriteImportData(SpriteMetaData value)
        {
            var output = new UnityEditor.AssetImporters.SpriteImportData();
            output.name = value.name;
            output.alignment = value.alignment;
            output.rect = value.rect;
            output.border = value.border;
            output.pivot = value.pivot;
            output.tessellationDetail = value.tessellationDetail;
            output.spriteID = value.spriteID.ToString();
            if (value.spriteOutline != null)
                output.outline = value.spriteOutline.Select(x => x.outline).ToList();

            return output;
        }
    }

    [Serializable]
    internal class SpriteOutline
    {
        [SerializeField]
        public Vector2[] outline;
    }
}
