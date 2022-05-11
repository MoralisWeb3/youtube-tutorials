using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    [Serializable]
    internal class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> m_Keys;
        [SerializeField]
        private List<TValue> m_Values;
        private Dictionary<TKey, TValue> m_Dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get { return m_Dictionary[key]; }
            set { m_Dictionary[key] = value; }
        }

        public TKey this[TValue value]
        {
            get
            {
                m_Keys = new List<TKey>(m_Dictionary.Keys);
                m_Values = new List<TValue>(m_Dictionary.Values);
                var index = m_Values.FindIndex(x => x.Equals(value));
                if (index < 0)
                    throw new KeyNotFoundException();
                return m_Keys[index];
            }
        }

        public ICollection<TKey> Keys
        {
            get { return m_Dictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return m_Dictionary.Values; }
        }

        public void Add(TKey key, TValue value)
        {
            m_Dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return m_Dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return m_Dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_Dictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            m_Dictionary.Clear();
        }

        public int Count
        {
            get { return m_Dictionary.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return (m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).IsReadOnly; }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            (m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return (m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return (m_Dictionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return (m_Dictionary as IEnumerable<KeyValuePair<TKey, TValue>>).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return m_Dictionary.GetEnumerator();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_Keys = new List<TKey>(m_Dictionary.Keys);
            m_Values = new List<TValue>(m_Dictionary.Values);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Debug.Assert(m_Keys.Count == m_Values.Count);
            Clear();
            for (var i = 0; i < m_Keys.Count; ++i)
                Add(m_Keys[i], m_Values[i]);
        }
    }
}
