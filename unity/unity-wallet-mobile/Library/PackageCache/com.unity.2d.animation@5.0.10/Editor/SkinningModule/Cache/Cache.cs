using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class Cache : BaseObject, ICacheUndo
    {
        public static T Create<T>() where T : Cache
        {
            var cache = CreateInstance<T>();
            cache.hideFlags = HideFlags.DontSave;
            return cache;
        }

        public static void Destroy(Cache cache)
        {
            cache.Destroy();
            DestroyImmediate(cache);
        }

        [SerializeField]
        private List<CacheObject> m_CacheObjects = new List<CacheObject>();
        [SerializeField]
        private List<CacheObject> m_RemovedCacheObjects = new List<CacheObject>();
        private string m_UndoOperationName = null;
        private IUndo m_DefaultUndo = new UnityEngineUndo();
        private IUndo m_UndoOverride = null;

        protected IUndo undo
        {
            get
            {
                if (undoOverride != null)
                    return undoOverride;

                return m_DefaultUndo;
            }
        }

        public IUndo undoOverride
        {
            get { return m_UndoOverride; }
            set { m_UndoOverride = value; }
        }

        public bool isUndoOperationSet
        {
            get { return string.IsNullOrEmpty(m_UndoOperationName) == false; }
        }

        public void IncrementCurrentGroup()
        {
            undo.IncrementCurrentGroup();
        }

        public virtual void BeginUndoOperation(string operationName)
        {
            if (isUndoOperationSet == false)
            {
                Debug.Assert(!m_CacheObjects.Contains(null));

                m_UndoOperationName = operationName;
                undo.RegisterCompleteObjectUndo(this, m_UndoOperationName);
                undo.RegisterCompleteObjectUndo(m_CacheObjects.ToArray(), m_UndoOperationName);
                undo.RegisterCompleteObjectUndo(m_RemovedCacheObjects.ToArray(), m_UndoOperationName);
            }
        }

        public void EndUndoOperation()
        {
            m_UndoOperationName = null;
        }

        public bool IsRemoved(CacheObject cacheObject)
        {
            return m_RemovedCacheObjects.Contains(cacheObject);
        }

        public T CreateCache<T>() where T : CacheObject
        {
            var cacheObject = FindRemovedCacheObject<T>();

            if (cacheObject != null)
            {
                m_RemovedCacheObjects.Remove(cacheObject);
                cacheObject.OnEnable();
            }
            else
            {
                cacheObject = CacheObject.Create<T>(this);
            }
            
            m_CacheObjects.Add(cacheObject);

            cacheObject.OnCreate();
            
            return cacheObject;
        }

        private T FindRemovedCacheObject<T>() where T : CacheObject
        {
            return m_RemovedCacheObjects.FirstOrDefault((o) => o.GetType().Equals(typeof(T))) as T;
        }

        public void Destroy(CacheObject cacheObject)
        {
            Debug.Assert(cacheObject != null);
            Debug.Assert(cacheObject.owner == this);
            Debug.Assert(m_CacheObjects.Contains(cacheObject));

            m_CacheObjects.Remove(cacheObject);
            m_RemovedCacheObjects.Add(cacheObject);

            cacheObject.OnDestroy();
        }

        public void Destroy()
        {
            Debug.Assert(!m_CacheObjects.Contains(null));

            EndUndoOperation();

            undo.ClearUndo(this);

            var cacheObjects = m_CacheObjects.ToArray();

            foreach (var cacheObject in cacheObjects)
                DestroyImmediate(cacheObject);

            cacheObjects = m_RemovedCacheObjects.ToArray();

            foreach (var cacheObject in cacheObjects)
                DestroyImmediate(cacheObject);

            m_CacheObjects.Clear();
            m_RemovedCacheObjects.Clear();
        }
    }
}
