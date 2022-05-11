using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class SpriteCache : TransformCache
    {
        [SerializeField]
        private string m_ID;
        [SerializeField]
        private Rect m_TextureRect;
        [SerializeField]
        private Vector2 m_PivotNormalized;

        public string id
        {
            get { return m_ID; }
            internal set { m_ID = value; }
        }

        public Rect textureRect
        {
            get { return m_TextureRect; }
            set { m_TextureRect = value; }
        }

        public Vector2 pivotNormalized
        {
            get { return m_PivotNormalized; }
            set { m_PivotNormalized = value; }
        }

        public Vector2 pivotRectSpace
        {
            get { return Vector2.Scale(textureRect.size, pivotNormalized); }
        }

        public Vector2 pivotTextureSpace
        {
            get { return localToWorldMatrix.MultiplyPoint3x4(pivotRectSpace); }
        }
    }
}
