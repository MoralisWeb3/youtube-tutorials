using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class AssociateBonesScope : IDisposable
    {
        private bool m_Disposed;
        private bool m_AssociateBones;
        private SpriteCache m_Sprite;

        public AssociateBonesScope(SpriteCache sprite)
        {
            m_Sprite = sprite;
            m_AssociateBones = m_Sprite.AssociatePossibleBones();
        }

        ~AssociateBonesScope()
        {
            if (!m_Disposed)
                Debug.LogError("Scope was not disposed! You should use the 'using' keyword or manually call Dispose.");
        }

        public void Dispose()
        {
            if (m_Disposed)
                return;

            m_Disposed = true;
            if (m_AssociateBones)
                m_Sprite.DeassociateUnusedBones();
        }
    }
}
