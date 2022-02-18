using UnityEngine;
using System;

namespace UnityEditor.U2D.Animation
{
    [Serializable]
    internal struct Edge
    {
        [SerializeField]
        int m_Index1;
        [SerializeField]
        int m_Index2;

        public int index1
        {
            get { return m_Index1; }
            set { m_Index1 = value; }
        }

        public int index2
        {
            get { return m_Index2; }
            set { m_Index2 = value; }
        }

        public Edge(int inIndex1, int inIndex2)
        {
            m_Index1 = inIndex1;
            m_Index2 = inIndex2;
        }

        public bool Contains(int index)
        {
            return index1 == index || index2 == index;
        }

        public static bool operator==(Edge lhs, Edge rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(Edge lhs, Edge rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Edge p = (Edge)obj;

            return (index1 == p.index1) && (index2 == p.index2) || (index1 == p.index2) && (index2 == p.index1);
        }

        public override int GetHashCode()
        {
            return index1.GetHashCode() ^ index2.GetHashCode();
        }
    }
}
