using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class UndoScope : IDisposable
    {
        private bool m_Disposed;
        private ICacheUndo m_CacheUndo;

        public UndoScope(ICacheUndo cacheUndo, string operationName, bool incrementGroup)
        {
            Debug.Assert(cacheUndo != null);

            if(cacheUndo.isUndoOperationSet == false)
            {
                m_CacheUndo = cacheUndo;

                if(incrementGroup)
                    m_CacheUndo.IncrementCurrentGroup();

                m_CacheUndo.BeginUndoOperation(operationName);
            }
        }

        ~UndoScope()
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
                m_CacheUndo.EndUndoOperation();

        }
    }
}
