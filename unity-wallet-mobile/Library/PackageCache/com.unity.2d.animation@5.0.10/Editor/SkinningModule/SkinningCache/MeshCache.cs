using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class MeshCache : SkinningObject, ISpriteMeshData
    {
        [SerializeField]
        private SpriteCache m_Sprite;
        [SerializeField]
        private List<Vertex2D> m_Vertices = new List<Vertex2D>();
        [SerializeField]
        private List<int> m_Indices = new List<int>();
        [SerializeField]
        private List<Edge> m_Edges = new List<Edge>();
        [SerializeField]
        private List<BoneCache> m_Bones = new List<BoneCache>();
        public ITextureDataProvider textureDataProvider { get; set; }

        public SpriteCache sprite
        {
            get { return m_Sprite; }
            set { m_Sprite = value; }
        }

        public List<Vertex2D> vertices
        {
            get { return m_Vertices; }
            set { m_Vertices = value; }
        }

        public List<Vector3> vertexPositionOverride { get; set; }

        public List<Edge> edges
        {
            get { return m_Edges; }
            set { m_Edges = value; }
        }

        public List<int> indices
        {
            get { return m_Indices; }
            set { m_Indices = value; }
        }

        public BoneCache[] bones
        {
            get { return m_Bones.ToArray(); }
            set { SetBones(value); }
        }

        Rect ISpriteMeshData.frame
        {
            get { return sprite.textureRect; }
            set {}
        }

        public int vertexCount
        {
            get { return m_Vertices.Count; }
        }

        public int boneCount
        {
            get { return m_Bones.Count; }
        }

        public Vector2 GetPosition(int index)
        {
            if (vertexPositionOverride != null)
                return vertexPositionOverride[index];

            return m_Vertices[index].position;
        }

        public void SetPosition(int index, Vector2 position)
        {
            if (vertexPositionOverride != null)
                return;

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

        SpriteBoneData ISpriteMeshData.GetBoneData(int index)
        {
            var worldToLocalMatrix = sprite.worldToLocalMatrix;

            //We expect m_Bones to contain character's bones references if character exists. Sprite's skeleton bones otherwise.
            if (skinningCache.hasCharacter)
                worldToLocalMatrix = sprite.GetCharacterPart().worldToLocalMatrix;

            SpriteBoneData spriteBoneData = null;
            var bone = m_Bones[index];

            if (bone == null)
                spriteBoneData = new SpriteBoneData();
            else
            {
                spriteBoneData = new SpriteBoneData()
                {
                    name = bone.name,
                    parentId = bone.parentBone == null ? -1 : m_Bones.IndexOf(bone.parentBone),
                    localPosition = bone.localPosition,
                    localRotation = bone.localRotation,
                    position = worldToLocalMatrix.MultiplyPoint3x4(bone.position),
                    endPosition = worldToLocalMatrix.MultiplyPoint3x4(bone.endPosition),
                    depth = bone.depth,
                    length = bone.localLength
                };
            }

            return spriteBoneData;
        }

        float ISpriteMeshData.GetBoneDepth(int index)
        {
            return m_Bones[index].depth;
        }

        public void Clear()
        {
            m_Vertices.Clear();
            m_Indices.Clear();
            m_Edges.Clear();
        }

        public bool ContainsBone(BoneCache bone)
        {
            return m_Bones.Contains(bone);
        }

        public void SetCompatibleBoneSet(BoneCache[] bones)
        {
            m_Bones = new List<BoneCache>(bones);
        }

        private void SetBones(BoneCache[] bones)
        {
            FixWeights(bones);
            SetCompatibleBoneSet(bones);
        }

        private void FixWeights(BoneCache[] newBones)
        {
            var newBonesList = new List<BoneCache>(newBones);
            var indexMap = new Dictionary<int, int>();

            for (var i = 0; i < m_Bones.Count; ++i)
            {
                var bone = m_Bones[i];
                var newIndex = newBonesList.IndexOf(bone);

                if (newIndex != -1)
                    indexMap.Add(i, newIndex);
            }

            foreach (Vertex2D vertex in vertices)
            {
                var boneWeight = vertex.editableBoneWeight;

                for (var i = 0; i < boneWeight.Count; ++i)
                {
                    var newIndex = 0;
                    var boneRemoved = indexMap.TryGetValue(boneWeight[i].boneIndex, out newIndex) == false;

                    if (boneRemoved)
                    {
                        boneWeight[i].weight = 0f;
                        boneWeight[i].enabled = false;
                    }

                    boneWeight[i].boneIndex = newIndex;

                    if (boneRemoved)
                        boneWeight.CompensateOtherChannels(i);
                }

                boneWeight.UnifyChannelsWithSameBoneIndex();
            }
        }
    }
}
