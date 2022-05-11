using UnityEngine;
using System;

namespace UnityEditor.U2D.Animation
{
    [Serializable]
    internal class Vertex2D
    {
        public Vector2 position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public EditableBoneWeight editableBoneWeight
        {
            get { return m_EditableBoneWeight; }
            set { m_EditableBoneWeight = value; }
        }

        public Vertex2D(Vector2 position)
        {
            m_Position = position;
            m_EditableBoneWeight = EditableBoneWeightUtility.CreateFromBoneWeight(new BoneWeight());
        }

        public Vertex2D(Vector2 position, BoneWeight weights)
        {
            m_Position = position;
            m_EditableBoneWeight = EditableBoneWeightUtility.CreateFromBoneWeight(weights);
        }

        [SerializeField]
        Vector2 m_Position;

        [SerializeField]
        EditableBoneWeight m_EditableBoneWeight;
    }
}
