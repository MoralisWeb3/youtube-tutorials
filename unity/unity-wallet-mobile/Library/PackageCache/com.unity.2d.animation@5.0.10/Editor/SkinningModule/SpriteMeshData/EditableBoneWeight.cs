using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.U2D.Animation
{
    [Serializable]
    internal class BoneWeightChannel : IComparable<BoneWeightChannel>
    {
        [SerializeField]
        private bool m_Enabled;
        [SerializeField]
        private int m_BoneIndex;
        [SerializeField]
        private float m_Weight;

        public bool enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        public int boneIndex
        {
            get { return m_BoneIndex; }
            set { m_BoneIndex = value; }
        }

        public float weight
        {
            get { return m_Weight; }
            set { m_Weight = value; }
        }

        public BoneWeightChannel() : this(0, 0f, false)
        {

        }

        public BoneWeightChannel(int i, float w, bool e)
        {
            enabled = e;
            boneIndex = i;
            weight = w;
        }

        public int CompareTo(BoneWeightChannel other)
        {
            int result = other.enabled.CompareTo(enabled);

            if (result == 0)
                result = other.weight.CompareTo(weight);

            return result;
        }
    }

    [Serializable]
    internal class EditableBoneWeight : IEnumerable<BoneWeightChannel>
    {
        [SerializeField]
        private List<BoneWeightChannel> m_Channels = new List<BoneWeightChannel>(5);

        public BoneWeightChannel this[int i]
        {
            get { return m_Channels[i]; }
            set { m_Channels[i] = value; }
        }

        public int Count
        {
            get { return m_Channels.Count; }
        }

        public void Clear()
        {
            m_Channels.Clear();
        }

        public void AddChannel(int boneIndex, float weight, bool enabled)
        {
            m_Channels.Add(new BoneWeightChannel(boneIndex, weight, enabled));
        }

        public void RemoveChannel(int channelIndex)
        {
            Debug.Assert(channelIndex < Count);

            m_Channels.RemoveAt(channelIndex);
        }

        public void Sort()
        {
            m_Channels.Sort();
        }

        public IEnumerator<BoneWeightChannel> GetEnumerator()
        {
            return ((IEnumerable<BoneWeightChannel>)m_Channels).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<BoneWeightChannel>)m_Channels).GetEnumerator();
        }
    }
}
