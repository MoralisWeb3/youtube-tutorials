using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class DisableUndoScope : IDisposable
    {
        private bool m_Disposed;
        private ICacheUndo m_CacheUndo;
        private IUndo m_UndoOverride;

        public DisableUndoScope(ICacheUndo cacheUndo)
        {
            Debug.Assert(cacheUndo != null);

            m_CacheUndo = cacheUndo;
            m_UndoOverride = m_CacheUndo.undoOverride;
            m_CacheUndo.undoOverride = new DisabledUndo();
        }

        ~DisableUndoScope()
        {
            if (!m_Disposed)
                Debug.LogError("Scope was not disposed! You should use the 'using' keyword or manually call Dispose.");
        }

        public void Dispose()
        {
            if (m_Disposed)
                return;

            m_Disposed = true;
            
            if (m_CacheUndo != null)
                m_CacheUndo.undoOverride = m_UndoOverride;

        }
    }
}
