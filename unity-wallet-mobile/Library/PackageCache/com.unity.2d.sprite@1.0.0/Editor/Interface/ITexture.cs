using UnityEngine;
using UnityTexture2D = UnityEngine.Texture2D;
using System;

namespace UnityEditor.U2D.Sprites
{
    internal abstract class ITexture2D
    {
        public abstract int width { get; }
        public abstract int height { get; }
        public abstract TextureFormat format { get; }
        public abstract Color32[] GetPixels32();
        public abstract FilterMode filterMode { get; set; }
        public abstract string name { get; }
        public abstract void SetPixels(Color[] c);
        public abstract void Apply();
        public abstract float mipMapBias { get; }

        public static bool operator==(ITexture2D t1, ITexture2D t2)
        {
            if (object.ReferenceEquals(t1, null))
            {
                return object.ReferenceEquals(t2, null) || t2 == null;
            }

            return t1.Equals(t2);
        }

        public static bool operator!=(ITexture2D t1, ITexture2D t2)
        {
            if (object.ReferenceEquals(t1, null))
            {
                return !object.ReferenceEquals(t2, null) && t2 != null;
            }

            return !t1.Equals(t2);
        }

        public override bool Equals(object other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static implicit operator UnityEngine.Object(ITexture2D t)
        {
            return object.ReferenceEquals(t, null) ? null : t.ToUnityObject();
        }

        public static implicit operator UnityEngine.Texture2D(ITexture2D t)
        {
            return object.ReferenceEquals(t, null) ? null : t.ToUnityTexture();
        }

        protected abstract UnityEngine.Object ToUnityObject();
        protected abstract UnityEngine.Texture2D ToUnityTexture();
    }

    internal class Texture2DWrapper : ITexture2D
    {
        UnityTexture2D m_Texture;

        public Texture2DWrapper(UnityTexture2D texture)
        {
            m_Texture = texture;
        }

        public override int width
        {
            get { return m_Texture.width; }
        }

        public override int height
        {
            get { return m_Texture.height; }
        }

        public override TextureFormat format
        {
            get { return m_Texture.format; }
        }

        public override Color32[] GetPixels32()
        {
            return m_Texture.GetPixels32();
        }

        public override FilterMode filterMode
        {
            get { return m_Texture.filterMode; }
            set { m_Texture.filterMode = value; }
        }

        public override float mipMapBias
        {
            get { return m_Texture.mipMapBias; }
        }

        public override string name
        {
            get { return m_Texture.name; }
        }

        public override bool Equals(object other)
        {
            Texture2DWrapper t = other as Texture2DWrapper;
            if (object.ReferenceEquals(t, null))
                return m_Texture == null;
            return m_Texture == t.m_Texture;
        }

        public override int GetHashCode()
        {
            return m_Texture.GetHashCode();
        }

        public override void SetPixels(Color[] c)
        {
            m_Texture.SetPixels(c);
        }

        public override void Apply()
        {
            m_Texture.Apply();
        }

        protected override UnityEngine.Object ToUnityObject()
        {
            return m_Texture;
        }

        protected override UnityEngine.Texture2D ToUnityTexture()
        {
            return m_Texture;
        }
    }
}
