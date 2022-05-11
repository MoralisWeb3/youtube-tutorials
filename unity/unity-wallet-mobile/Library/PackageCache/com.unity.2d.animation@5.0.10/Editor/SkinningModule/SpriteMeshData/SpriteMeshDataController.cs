using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.U2D.Sprites;

namespace UnityEditor.U2D.Animation
{
    internal struct WeightedTriangle : IComparable<WeightedTriangle>
    {
        public int p1;
        public int p2;
        public int p3;
        public float weight;

        public int CompareTo(WeightedTriangle other)
        {
            return weight.CompareTo(other.weight);
        }
    }

    internal class SpriteMeshDataController
    {
        public ISpriteMeshData spriteMeshData;
        private List<Vector2> m_VerticesTemp = new List<Vector2>();
        private List<Edge> m_EdgesTemp = new List<Edge>();

        public void CreateVertex(Vector2 position)
        {
            CreateVertex(position, -1);
        }

        public void CreateVertex(Vector2 position, int edgeIndex)
        {
            Debug.Assert(spriteMeshData != null);

            spriteMeshData.AddVertex(position, default(BoneWeight));

            if (edgeIndex != -1)
            {
                Edge edge = spriteMeshData.edges[edgeIndex];
                RemoveEdge(edge);
                CreateEdge(edge.index1, spriteMeshData.vertexCount - 1);
                CreateEdge(edge.index2, spriteMeshData.vertexCount - 1);
            }
        }

        public void CreateEdge(int index1, int index2)
        {
            Debug.Assert(spriteMeshData != null);
            Debug.Assert(index1 >= 0);
            Debug.Assert(index2 >= 0);
            Debug.Assert(index1 < spriteMeshData.vertexCount);
            Debug.Assert(index2 < spriteMeshData.vertexCount);
            Debug.Assert(index1 != index2);

            Edge newEdge = new Edge(index1, index2);

            if (!spriteMeshData.edges.Contains(newEdge))
                spriteMeshData.edges.Add(newEdge);
        }

        public void RemoveVertex(int index)
        {
            Debug.Assert(spriteMeshData != null);

            //We need to delete the edges that reference the index
            List<Edge> edgesWithIndex;
            if (FindEdgesContainsIndex(index, out edgesWithIndex))
            {
                //If there are 2 edges referencing the same index we are removing, we can create a new one that connects the endpoints ("Unsplit").
                if (edgesWithIndex.Count == 2)
                {
                    Edge first = edgesWithIndex[0];
                    Edge second = edgesWithIndex[1];

                    int index1 = first.index1 != index ? first.index1 : first.index2;
                    int index2 = second.index1 != index ? second.index1 : second.index2;

                    CreateEdge(index1, index2);
                }

                //remove found edges
                for (int i = 0; i < edgesWithIndex.Count; i++)
                {
                    RemoveEdge(edgesWithIndex[i]);
                }
            }

            //Fix indices in edges greater than the one we are removing
            for (int i = 0; i < spriteMeshData.edges.Count; i++)
            {
                Edge edge = spriteMeshData.edges[i];

                if (edge.index1 > index)
                    edge.index1--;
                if (edge.index2 > index)
                    edge.index2--;

                spriteMeshData.edges[i] = edge;
            }

            spriteMeshData.RemoveVertex(index);
        }

        public void RemoveVertex(IEnumerable<int> indices)
        {
            List<int> sortedIndexList = new List<int>(indices);

            if (sortedIndexList.Count == 0)
                return;

            sortedIndexList.Sort();

            for (int i = sortedIndexList.Count - 1; i >= 0; --i)
            {
                RemoveVertex(sortedIndexList[i]);
            }
        }
        
        public void RemoveEdge(Edge edge)
        {
            Debug.Assert(spriteMeshData != null);

            spriteMeshData.edges.Remove(edge);
        }

        public bool FindEdgesContainsIndex(int index, out List<Edge> result)
        {
            Debug.Assert(spriteMeshData != null);

            bool found = false;

            result = new List<Edge>();

            for (int i = 0; i < spriteMeshData.edges.Count; ++i)
            {
                Edge edge = spriteMeshData.edges[i];

                if (edge.Contains(index))
                {
                    found = true;
                    result.Add(edge);
                }
            }

            return found;
        }

        public void Triangulate(ITriangulator triangulator)
        {
            Debug.Assert(spriteMeshData != null);
            Debug.Assert(triangulator != null);

            m_VerticesTemp.Clear();

            for (int i = 0; i < spriteMeshData.vertexCount; ++i)
                m_VerticesTemp.Add(spriteMeshData.GetPosition(i));

            triangulator.Triangulate(m_VerticesTemp, spriteMeshData.edges, spriteMeshData.indices);
        }

        public void Subdivide(ITriangulator triangulator, float largestAreaFactor)
        {
            Debug.Assert(spriteMeshData != null);
            Debug.Assert(triangulator != null);

            m_VerticesTemp.Clear();
            m_EdgesTemp.Clear();
            m_EdgesTemp.AddRange(spriteMeshData.edges);

            for (int i = 0; i < spriteMeshData.vertexCount; ++i)
                m_VerticesTemp.Add(spriteMeshData.GetPosition(i));
            
            try
            {
                var indices = new List<int>();

                triangulator.Tessellate(0f, 0f, 0f, largestAreaFactor, 100, m_VerticesTemp, m_EdgesTemp, indices);

                spriteMeshData.Clear();

                for (int i = 0; i < m_VerticesTemp.Count; ++i)
                    spriteMeshData.AddVertex(m_VerticesTemp[i], default(BoneWeight));

                spriteMeshData.edges.AddRange(m_EdgesTemp);
                spriteMeshData.indices.AddRange(indices);
            }
            catch (Exception)
            {
                
            }
        }

        public void ClearWeights(ISelection<int> selection)
        {
            Debug.Assert(spriteMeshData != null);

            for (int i = 0; i < spriteMeshData.vertexCount; ++i)
                if (selection == null || (selection.Count == 0 || selection.Contains(i)))
                    spriteMeshData.GetWeight(i).SetFromBoneWeight(default(BoneWeight));
        }

        public void OutlineFromAlpha(IOutlineGenerator outlineGenerator, ITextureDataProvider textureDataProvider, float outlineDetail, byte alphaTolerance)
        {
            Debug.Assert(spriteMeshData != null);
            Debug.Assert(textureDataProvider != null);
            Debug.Assert(textureDataProvider.texture != null);

            int width, height;
            textureDataProvider.GetTextureActualWidthAndHeight(out width, out height);

            Vector2 scale = new Vector2(textureDataProvider.texture.width / (float)width, textureDataProvider.texture.height / (float)height);
            Vector2 scaleInv = new Vector2(1f / scale.x, 1f / scale.y);
            Vector2 rectOffset = spriteMeshData.frame.size * 0.5f + spriteMeshData.frame.position;

            Rect scaledRect = spriteMeshData.frame;
            scaledRect.min = Vector2.Scale(scaledRect.min, scale);
            scaledRect.max = Vector2.Scale(scaledRect.max, scale);

            spriteMeshData.Clear();

            Vector2[][] paths;
            outlineGenerator.GenerateOutline(textureDataProvider, scaledRect, outlineDetail, alphaTolerance, false, out paths);

            int vertexIndexBase = 0;
            for (int i = 0; i < paths.Length; ++i)
            {
                int numPathVertices = paths[i].Length;

                for (int j = 0; j <= numPathVertices; j++)
                {
                    if (j < numPathVertices)
                        spriteMeshData.AddVertex(Vector2.Scale(paths[i][j], scaleInv) + rectOffset, default(BoneWeight));

                    if (j > 0)
                        spriteMeshData.edges.Add(new Edge(vertexIndexBase + j - 1, vertexIndexBase + j % numPathVertices));
                }

                vertexIndexBase += numPathVertices;
            }
        }

        public void NormalizeWeights(ISelection<int> selection)
        {
            Debug.Assert(spriteMeshData != null);

            for (int i = 0; i < spriteMeshData.vertexCount; ++i)
                if (selection == null || (selection.Count == 0 || selection.Contains(i)))
                    spriteMeshData.GetWeight(i).Normalize();
        }

        public void CalculateWeights(IWeightsGenerator weightsGenerator, ISelection<int> selection, float filterTolerance)
        {
            Debug.Assert(spriteMeshData != null);

            Vector2[] controlPoints;
            Edge[] bones;
            int[] pins;

            GetControlPoints(out controlPoints, out bones, out pins);

            Vector2[] vertices = new Vector2[spriteMeshData.vertexCount];

            for (int i = 0; i < spriteMeshData.vertexCount; ++i)
                vertices[i] = spriteMeshData.GetPosition(i);

            BoneWeight[] boneWeights = weightsGenerator.Calculate(vertices, spriteMeshData.edges.ToArray(), controlPoints, bones, pins);

            Debug.Assert(boneWeights.Length == spriteMeshData.vertexCount);

            for (int i = 0; i < spriteMeshData.vertexCount; ++i)
            {
                if (selection == null || (selection.Count == 0 || selection.Contains(i)))
                {
                    EditableBoneWeight editableBoneWeight = EditableBoneWeightUtility.CreateFromBoneWeight(boneWeights[i]);

                    if (filterTolerance > 0f)
                    {
                        editableBoneWeight.FilterChannels(filterTolerance);
                        editableBoneWeight.Normalize();
                    }

                    spriteMeshData.SetWeight(i, editableBoneWeight);
                }
            }
        }

        public void CalculateWeightsSafe(IWeightsGenerator weightsGenerator, ISelection<int> selection, float filterTolerance)
        {
            var tempSelection = new IndexedSelection();
            var vertexSelector = new GenericVertexSelector();

            vertexSelector.spriteMeshData = spriteMeshData;
            vertexSelector.selection = tempSelection;
            vertexSelector.SelectionCallback = (int i) => {
                return spriteMeshData.GetWeight(i).Sum() == 0f && (selection == null || selection.Count == 0 || selection.Contains(i));
            };
            vertexSelector.Select();

            if (tempSelection.Count > 0)
                CalculateWeights(weightsGenerator, tempSelection, filterTolerance);
        }

        public void SmoothWeights(int iterations, ISelection<int> selection)
        {
            var boneWeights = new BoneWeight[spriteMeshData.vertexCount];

            for (var i = 0; i < spriteMeshData.vertexCount; i++)
                boneWeights[i] = spriteMeshData.GetWeight(i).ToBoneWeight(false);
            
            BoneWeight[] smoothedWeights;
            SmoothingUtility.SmoothWeights(boneWeights, spriteMeshData.indices, spriteMeshData.boneCount, iterations, out smoothedWeights);

            for (var i = 0; i < spriteMeshData.vertexCount; i++)
                if (selection == null || (selection.Count == 0 || selection.Contains(i)))
                    spriteMeshData.GetWeight(i).SetFromBoneWeight(smoothedWeights[i]);
        }

        public bool FindTriangle(Vector2 point, out Vector3Int indices, out Vector3 barycentricCoords)
        {
            Debug.Assert(spriteMeshData != null);

            indices = Vector3Int.zero;
            barycentricCoords = Vector3.zero;

            if (spriteMeshData.indices.Count < 3)
                return false;

            int triangleCount = spriteMeshData.indices.Count / 3;

            for (int i = 0; i < triangleCount; ++i)
            {
                indices.x = spriteMeshData.indices[i * 3];
                indices.y = spriteMeshData.indices[i * 3 + 1];
                indices.z = spriteMeshData.indices[i * 3 + 2];

                MathUtility.Barycentric(
                    point,
                    spriteMeshData.GetPosition(indices.x),
                    spriteMeshData.GetPosition(indices.y),
                    spriteMeshData.GetPosition(indices.z),
                    out barycentricCoords);

                if (barycentricCoords.x >= 0f && barycentricCoords.y >= 0f && barycentricCoords.z >= 0f)
                    return true;
            }

            return false;
        }

        private List<float> m_VertexOrderList = new List<float>(1000);
        private List<WeightedTriangle> m_WeightedTriangles = new List<WeightedTriangle>(1000);

        public void SortTrianglesByDepth()
        {
            Debug.Assert(spriteMeshData != null);

            if (spriteMeshData.boneCount == 0)
                return;

            m_VertexOrderList.Clear();
            m_WeightedTriangles.Clear();

            for (var i = 0; i < spriteMeshData.vertexCount; i++)
            {
                var vertexOrder = 0f;
                var boneWeight = spriteMeshData.GetWeight(i);

                for (var j = 0; j < boneWeight.Count; ++j)
                    vertexOrder += spriteMeshData.GetBoneDepth(boneWeight[j].boneIndex) * boneWeight[j].weight;

                m_VertexOrderList.Add(vertexOrder);
            }

            for (int i = 0; i < spriteMeshData.indices.Count; i += 3)
            {
                int p1 = spriteMeshData.indices[i];
                int p2 = spriteMeshData.indices[i + 1];
                int p3 = spriteMeshData.indices[i + 2];
                float weight = (m_VertexOrderList[p1] + m_VertexOrderList[p2] + m_VertexOrderList[p3]) / 3f;

                m_WeightedTriangles.Add(new WeightedTriangle() { p1 = p1, p2 = p2, p3 = p3, weight = weight });
            }

            m_WeightedTriangles.Sort();

            spriteMeshData.indices.Clear();

            for (var i = 0; i < m_WeightedTriangles.Count; ++i)
            {
                var triangle = m_WeightedTriangles[i];
                spriteMeshData.indices.Add(triangle.p1);
                spriteMeshData.indices.Add(triangle.p2);
                spriteMeshData.indices.Add(triangle.p3);
            }
        }

        public void GetMultiEditChannelData(ISelection<int> selection, int channel,
            out bool enabled, out int boneIndex, out float weight,
            out bool isEnabledMixed, out bool isBoneIndexMixed, out bool isWeightMixed)
        {
            Debug.Assert(spriteMeshData != null);

            if (selection == null)
                throw new ArgumentNullException("selection is null");

            var first = true;
            enabled = false;
            boneIndex = -1;
            weight = 0f;
            isEnabledMixed = false;
            isBoneIndexMixed = false;
            isWeightMixed = false;

            var indices = selection.elements;

            foreach (int i in indices)
            {
                var editableBoneWeight = spriteMeshData.GetWeight(i);

                if (first)
                {
                    enabled = editableBoneWeight[channel].enabled;
                    boneIndex = editableBoneWeight[channel].boneIndex;
                    weight = editableBoneWeight[channel].weight;

                    first = false;
                }
                else
                {
                    if (enabled != editableBoneWeight[channel].enabled)
                    {
                        isEnabledMixed = true;
                        enabled = false;
                    }

                    if (boneIndex != editableBoneWeight[channel].boneIndex)
                    {
                        isBoneIndexMixed = true;
                        boneIndex = -1;
                    }

                    if (weight != editableBoneWeight[channel].weight)
                    {
                        isWeightMixed = true;
                        weight = 0f;
                    }
                }
            }
        }
        
        public void SetMultiEditChannelData(ISelection<int> selection, int channel,
            bool oldEnabled, bool newEnabled,  int oldBoneIndex, int newBoneIndex, float oldWeight, float newWeight)
        {
            Debug.Assert(spriteMeshData != null);

            if (selection == null)
                throw new ArgumentNullException("selection is null");

            bool channelEnabledChanged = oldEnabled != newEnabled;
            bool boneIndexChanged = oldBoneIndex != newBoneIndex;
            bool weightChanged = oldWeight != newWeight;

            var indices = selection.elements;

            foreach (int i in indices)
            {
                var editableBoneWeight = spriteMeshData.GetWeight(i);

                if (channelEnabledChanged)
                    editableBoneWeight[channel].enabled = newEnabled;

                if (boneIndexChanged)
                    editableBoneWeight[channel].boneIndex = newBoneIndex;

                if (weightChanged)
                    editableBoneWeight[channel].weight = newWeight;

                if (channelEnabledChanged || weightChanged)
                    editableBoneWeight.CompensateOtherChannels(channel);
            }
        }

        public void GetControlPoints(out Vector2[] points, out Edge[] edges, out int[] pins)
        {
            Debug.Assert(spriteMeshData != null);

            points = null;
            edges = null;

            List<Vector2> pointList = new List<Vector2>();
            List<Edge> edgeList = new List<Edge>();
            List<int> pinList = new List<int>();
            List<SpriteBoneData> bones = new List<SpriteBoneData>(spriteMeshData.boneCount);

            for (int i = 0; i < spriteMeshData.boneCount; ++i)
                bones.Add(spriteMeshData.GetBoneData(i));

            foreach (var bone in bones)
            {
                var length = (bone.endPosition - bone.position).magnitude;

                if (length > 0f)
                {
                    int index1 = FindPoint(pointList, bone.position, 0.01f);
                    int index2 = FindPoint(pointList, bone.endPosition, 0.01f);

                    if (index1 == -1)
                    {
                        pointList.Add(bone.position);
                        index1 = pointList.Count - 1;
                    }

                    if (index2 == -1)
                    {
                        pointList.Add(bone.endPosition);
                        index2 = pointList.Count - 1;
                    }

                    edgeList.Add(new Edge(index1, index2));
                }
                else if (bone.length == 0f)
                {
                    pointList.Add(bone.position);
                    pinList.Add(pointList.Count - 1);
                }
            }

            points = pointList.ToArray();
            edges = edgeList.ToArray();
            pins = pinList.ToArray();
        }

        private int FindPoint(List<Vector2> points, Vector2 point, float distanceTolerance)
        {
            float sqrTolerance = distanceTolerance * distanceTolerance;

            for (int i = 0; i < points.Count; ++i)
            {
                if ((points[i] - point).sqrMagnitude <= sqrTolerance)
                    return i;
            }

            return -1;
        }

        public void SmoothFill()
        {
            var tempSelection = new IndexedSelection();
            var vertexSelector = new GenericVertexSelector();
            var currentWeightSum = 0f;
            var prevWeightSum = 0f;

            vertexSelector.spriteMeshData = spriteMeshData;
            vertexSelector.selection = tempSelection;
            vertexSelector.SelectionCallback = (int i) => {
                var sum = spriteMeshData.GetWeight(i).Sum();
                currentWeightSum += sum;
                return sum < 0.99f;
            };

            do
            {
                prevWeightSum = currentWeightSum;
                currentWeightSum = 0f;
                vertexSelector.Select();

                if (tempSelection.Count > 0)
                    SmoothWeights(1, tempSelection);
            }
            while (currentWeightSum - prevWeightSum > 0.001f);

            if (tempSelection.Count > 0)
                NormalizeWeights(tempSelection);
        }
    }
}
