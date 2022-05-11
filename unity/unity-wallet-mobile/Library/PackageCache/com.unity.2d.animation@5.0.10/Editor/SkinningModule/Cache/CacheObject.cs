using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class CacheObject : BaseObject, ISerializationCallbackReceiver
    {
        public static T Create<T>(Cache owner) where T : CacheObject
        {
            var cacheObject = CreateInstance<T>();
            cacheObject.hideFlags = HideFlags.HideAndDontSave;
            cacheObject.owner = owner;
            return cacheObject;
        }

        [SerializeField]
        private Cache m_Owner;

        public Cache owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            OnAfterDeserialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            OnBeforeSerialize();
        }

        internal virtual void OnCreate() {}
        protected virtual void OnAfterDeserialize() {}
        protected virtual void OnBeforeSerialize() {}
    }
}
