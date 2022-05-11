using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    [Serializable]
    internal class SpriteBoneData
    {
        public string name;
        public int parentId = -1;
        public Vector2 localPosition;
        public Quaternion localRotation = Quaternion.identity;
        public Vector2 position;
        public Vector2 endPosition;
        public float depth;
        public float length;
    }

    internal interface ISpriteMeshData
    {
        Rect frame { get; set; }
        List<int> indices { get; set; }
        List<Edge> edges { get; set; }
        int vertexCount { get; }
        int boneCount { get; }
        Vector2 GetPosition(int index);
        void SetPosition(int index, Vector2 position);
        EditableBoneWeight GetWeight(int index);
        void SetWeight(int index, EditableBoneWeight weight);
        void AddVertex(Vector2 position, BoneWeight weight);
        void RemoveVertex(int index);
        SpriteBoneData GetBoneData(int index);
        float GetBoneDepth(int index);
        void Clear();
    }

    [Serializable]
    internal class SpriteMeshData : ISpriteMeshData
    {
        public GUID spriteID = new GUID();
        [SerializeField]
        private Rect m_Frame;
        public Vector2 pivot = Vector2.zero;
        [SerializeField]
        private List<Vertex2D> m_Vertices = new List<Vertex2D>();
        [SerializeField]
        private List<int> m_Indices = new List<int>();
        [SerializeField]
        public List<SpriteBoneData> m_Bones = new List<SpriteBoneData>();
        [SerializeField]
        private List<Edge> m_Edges = new List<Edge>();

        public Rect frame
        {
            get { return m_Frame; }
            set { m_Frame = value; }
        }

        public List<Vertex2D> vertices
        {
            get { return m_Vertices; }
            set { m_Vertices = value; }
        }

        public List<int> indices
        {
            get { return m_Indices; }
            set { m_Indices = value; }
        }

        public List<Edge> edges
        {
            get { return m_Edges; }
            set { m_Edges = value; }
        }

        public List<SpriteBoneData> bones
        {
            get { return m_Bones; }
            set { m_Bones = value; }
        }

        public int vertexCount { get { return m_Vertices.Count; } }
        public int boneCount { get { return m_Bones.Count; } }

        public Vector2 GetPosition(int index)
        {
            return m_Vertices[index].position;
        }

        public void SetPosition(int index, Vector2 position)
        {
            m_Vertices[index].position = position;
        }

        public EditableBoneWeight GetWeight(int index)
        {
            return m_Vertices[index].editableBoneWeight;
        }

        public void SetWeight(int index, EditableBoneWeight weight)
        {
            m_Vertices[index].editableBoneWeight = weight;
        }

        public void AddVertex(Vector2 position, BoneWeight weight)
        {
            m_Vertices.Add(new Vertex2D(position, weight));
        }

        public void RemoveVertex(int index)
        {
            m_Vertices.RemoveAt(index);
        }

        public SpriteBoneData GetBoneData(int index)
        {
            return m_Bones[index];
        }

        public float GetBoneDepth(int index)
        {
            return m_Bones[index].depth;
        }

        public void Clear()
        {
            m_Vertices.Clear();
            m_Indices.Clear();
            m_Edges.Clear();
        }
    }
}
