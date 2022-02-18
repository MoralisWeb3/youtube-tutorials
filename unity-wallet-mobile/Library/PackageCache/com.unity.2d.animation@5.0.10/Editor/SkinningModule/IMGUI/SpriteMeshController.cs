using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class SpriteMeshController
    {
        private const float kSnapDistance = 10f;

        private struct EdgeIntersectionResult
        {
            public int startVertexIndex;
            public int endVertexIndex;
            public int intersectEdgeIndex;
            public Vector2 endPosition;
        }

        private SpriteMeshDataController m_SpriteMeshDataController = new SpriteMeshDataController();
        private EdgeIntersectionResult m_EdgeIntersectionResult;

        public ISpriteMeshView spriteMeshView { get; set; }
        public ISpriteMeshData spriteMeshData
        {
            get { return m_SpriteMeshData; }
            set { m_SpriteMeshData = value; }
        }

        public ISelection<int> selection { get; set; }
        public ICacheUndo cacheUndo { get; set; }
        public ITriangulator triangulator { get; set; }
        public bool disable { get; set; }
        public Rect frame { get; set; }
        private ISpriteMeshData m_SpriteMeshData;
        private bool m_Moved = false;

        public void OnGUI()
        {
            m_SpriteMeshDataController.spriteMeshData = m_SpriteMeshData;

            Debug.Assert(spriteMeshView != null);
            Debug.Assert(m_SpriteMeshData != null);
            Debug.Assert(selection != null);
            Debug.Assert(cacheUndo != null);

            spriteMeshView.selection = selection;
            spriteMeshView.frame = frame;

            EditorGUI.BeginDisabledGroup(disable);

            spriteMeshView.BeginLayout();

            if(spriteMeshView.CanLayout())
            {
                LayoutVertices();
                LayoutEdges();
            }

            spriteMeshView.EndLayout();

            if(spriteMeshView.CanRepaint())
            {
                DrawEdges();
                
                if(GUI.enabled)
                {
                    PreviewCreateVertex();
                    PreviewCreateEdge();
                    PreviewSplitEdge();
                }

                DrawVertices();
            }


            HandleSplitEdge();
            HandleCreateEdge();
            HandleCreateVertex();

            EditorGUI.EndDisabledGroup();

            HandleSelectVertex();

            EditorGUI.BeginDisabledGroup(disable);

            HandleMoveVertex();

            EditorGUI.EndDisabledGroup();

            HandleSelectEdge();

            EditorGUI.BeginDisabledGroup(disable);

            HandleMoveEdge();
            HandleRemoveEdge();
            HandleRemoveVertices();

            spriteMeshView.DoRepaint();

            EditorGUI.EndDisabledGroup();         
        }

        private void LayoutVertices()
        {
            for (int i = 0; i < m_SpriteMeshData.vertexCount; i++)
            {
                Vector2 position = m_SpriteMeshData.GetPosition(i);
                spriteMeshView.LayoutVertex(position, i);
            }
        }

        private void LayoutEdges()
        {
            for (int i = 0; i < m_SpriteMeshData.edges.Count; i++)
            {
                Edge edge = m_SpriteMeshData.edges[i];
                Vector2 startPosition = m_SpriteMeshData.GetPosition(edge.index1);
                Vector2 endPosition = m_SpriteMeshData.GetPosition(edge.index2);

                spriteMeshView.LayoutEdge(startPosition, endPosition, i);
            }
        }

        private void DrawEdges()
        {
            UpdateEdgeInstersection();

            spriteMeshView.BeginDrawEdges();

            for (int i = 0; i < m_SpriteMeshData.edges.Count; ++i)
            {
                if (SkipDrawEdge(i))
                    continue;

                Edge edge = m_SpriteMeshData.edges[i];
                Vector2 startPosition = m_SpriteMeshData.GetPosition(edge.index1);
                Vector2 endPosition = m_SpriteMeshData.GetPosition(edge.index2);

                if (selection.Contains(edge.index1) && selection.Contains(edge.index2))
                    spriteMeshView.DrawEdgeSelected(startPosition, endPosition);
                else
                    spriteMeshView.DrawEdge(startPosition, endPosition);
            }

            if (spriteMeshView.IsActionActive(MeshEditorAction.SelectEdge))
            {
                Edge hoveredEdge = m_SpriteMeshData.edges[spriteMeshView.hoveredEdge];
                Vector2 startPosition = m_SpriteMeshData.GetPosition(hoveredEdge.index1);
                Vector2 endPosition = m_SpriteMeshData.GetPosition(hoveredEdge.index2);

                spriteMeshView.DrawEdgeHovered(startPosition, endPosition);
            }

            spriteMeshView.EndDrawEdges();
        }

        private bool SkipDrawEdge(int edgeIndex)
        {
            if(GUI.enabled == false)
                return false;
                
            return edgeIndex == -1 ||
                spriteMeshView.hoveredEdge == edgeIndex && spriteMeshView.IsActionActive(MeshEditorAction.SelectEdge) ||
                spriteMeshView.hoveredEdge == edgeIndex && spriteMeshView.IsActionActive(MeshEditorAction.CreateVertex) ||
                spriteMeshView.closestEdge == edgeIndex && spriteMeshView.IsActionActive(MeshEditorAction.SplitEdge) ||
                edgeIndex == m_EdgeIntersectionResult.intersectEdgeIndex && spriteMeshView.IsActionActive(MeshEditorAction.CreateEdge);
        }

        private void PreviewCreateVertex()
        {
            if (spriteMeshView.mode == SpriteMeshViewMode.CreateVertex &&
                spriteMeshView.IsActionActive(MeshEditorAction.CreateVertex))
            {
                Vector2 clampedMousePos = ClampToFrame(spriteMeshView.mouseWorldPosition);

                if (spriteMeshView.hoveredEdge != -1)
                {
                    Edge edge = m_SpriteMeshData.edges[spriteMeshView.hoveredEdge];

                    spriteMeshView.BeginDrawEdges();

                    spriteMeshView.DrawEdge(m_SpriteMeshData.GetPosition(edge.index1), clampedMousePos);
                    spriteMeshView.DrawEdge(m_SpriteMeshData.GetPosition(edge.index2), clampedMousePos);

                    spriteMeshView.EndDrawEdges();
                }

                spriteMeshView.DrawVertex(clampedMousePos);
            }
        }

        private void PreviewCreateEdge()
        {
            if (!spriteMeshView.IsActionActive(MeshEditorAction.CreateEdge))
                return;

            spriteMeshView.BeginDrawEdges();

            spriteMeshView.DrawEdge(m_SpriteMeshData.GetPosition(m_EdgeIntersectionResult.startVertexIndex), m_EdgeIntersectionResult.endPosition);

            if (m_EdgeIntersectionResult.intersectEdgeIndex != -1)
            {
                Edge intersectingEdge = m_SpriteMeshData.edges[m_EdgeIntersectionResult.intersectEdgeIndex];
                spriteMeshView.DrawEdge(m_SpriteMeshData.GetPosition(intersectingEdge.index1), m_EdgeIntersectionResult.endPosition);
                spriteMeshView.DrawEdge(m_SpriteMeshData.GetPosition(intersectingEdge.index2), m_EdgeIntersectionResult.endPosition);
            }

            spriteMeshView.EndDrawEdges();

            if (m_EdgeIntersectionResult.endVertexIndex == -1)
                spriteMeshView.DrawVertex(m_EdgeIntersectionResult.endPosition);
        }

        private void PreviewSplitEdge()
        {
            if (!spriteMeshView.IsActionActive(MeshEditorAction.SplitEdge))
                return;

            Vector2 clampedMousePos = ClampToFrame(spriteMeshView.mouseWorldPosition);

            Edge closestEdge = m_SpriteMeshData.edges[spriteMeshView.closestEdge];

            spriteMeshView.BeginDrawEdges();

            spriteMeshView.DrawEdge(m_SpriteMeshData.GetPosition(closestEdge.index1), clampedMousePos);
            spriteMeshView.DrawEdge(m_SpriteMeshData.GetPosition(closestEdge.index2), clampedMousePos);

            spriteMeshView.EndDrawEdges();

            spriteMeshView.DrawVertex(clampedMousePos);
        }

        private void DrawVertices()
        {
            for (int i = 0; i < m_SpriteMeshData.vertexCount; i++)
            {
                Vector3 position = m_SpriteMeshData.GetPosition(i);

                if (selection.Contains(i))
                    spriteMeshView.DrawVertexSelected(position);
                else if (i == spriteMeshView.hoveredVertex && spriteMeshView.IsActionHot(MeshEditorAction.None))
                    spriteMeshView.DrawVertexHovered(position);
                else
                    spriteMeshView.DrawVertex(position);
            }
        }

        private void HandleSelectVertex()
        {
            bool additive;
            if (spriteMeshView.DoSelectVertex(out additive))
                SelectVertex(spriteMeshView.hoveredVertex, additive);
        }

        private void HandleSelectEdge()
        {
            bool additive;
            if (spriteMeshView.DoSelectEdge(out additive))
                SelectEdge(spriteMeshView.hoveredEdge, additive);
        }

        private void HandleMoveVertex()
        {
            if(spriteMeshView.IsActionTriggered(MeshEditorAction.MoveVertex))
                m_Moved = false;

            Vector2 delta;
            if (spriteMeshView.DoMoveVertex(out delta))
            {
                if(!m_Moved)
                {
                    cacheUndo.BeginUndoOperation(TextContent.moveVertices);
                    m_Moved = true;
                }

                MoveSelectedVertices(delta);
            }
        }

        private void HandleCreateVertex()
        {
            if (spriteMeshView.DoCreateVertex())
                CreateVertex(spriteMeshView.mouseWorldPosition, spriteMeshView.hoveredEdge);
        }

        private void HandleSplitEdge()
        {
            if (spriteMeshView.DoSplitEdge())
                SplitEdge(spriteMeshView.mouseWorldPosition, spriteMeshView.closestEdge);
        }

        private void HandleCreateEdge()
        {
            if (spriteMeshView.DoCreateEdge())
                CreateEdge(spriteMeshView.mouseWorldPosition, spriteMeshView.hoveredVertex, spriteMeshView.hoveredEdge);
        }

        private void HandleMoveEdge()
        {
            if(spriteMeshView.IsActionTriggered(MeshEditorAction.MoveEdge))
                m_Moved = false;

            Vector2 delta;
            if (spriteMeshView.DoMoveEdge(out delta))
            {
                if(!m_Moved)
                {
                    cacheUndo.BeginUndoOperation(TextContent.moveVertices);
                    m_Moved = true;
                }
                
                MoveSelectedVertices(delta);
            }
        }

        private void HandleRemoveEdge()
        {
            Edge edge;
            if (GetSelectedEdge(out edge) && spriteMeshView.DoRemove())
                RemoveEdge(edge);
        }

        private void HandleRemoveVertices()
        {
            if (spriteMeshView.DoRemove())
                RemoveSelectedVertices();
        }

        private void CreateVertex(Vector2 position, int edgeIndex)
        {
            position = MathUtility.ClampPositionToRect(position, frame);
            cacheUndo.BeginUndoOperation(TextContent.createVertex);

            BoneWeight boneWeight = new BoneWeight();

            Vector3Int indices;
            Vector3 barycentricCoords;
            if (m_SpriteMeshDataController.FindTriangle(position, out indices, out barycentricCoords))
            {
                EditableBoneWeight bw1 = m_SpriteMeshData.GetWeight(indices.x);
                EditableBoneWeight bw2 = m_SpriteMeshData.GetWeight(indices.y);
                EditableBoneWeight bw3 = m_SpriteMeshData.GetWeight(indices.z);

                EditableBoneWeight result = new EditableBoneWeight();

                foreach (BoneWeightChannel channel in bw1)
                {
                    if (!channel.enabled)
                        continue;

                    var weight = channel.weight * barycentricCoords.x;

                    if (weight > 0f)
                        result.AddChannel(channel.boneIndex, weight, true);
                }

                foreach (BoneWeightChannel channel in bw2)
                {
                    if (!channel.enabled)
                        continue;

                    var weight = channel.weight * barycentricCoords.y;

                    if (weight > 0f)
                        result.AddChannel(channel.boneIndex, weight, true);
                }

                foreach (BoneWeightChannel channel in bw3)
                {
                    if (!channel.enabled)
                        continue;

                    var weight = channel.weight * barycentricCoords.z;

                    if (weight > 0f)
                        result.AddChannel(channel.boneIndex, weight, true);
                }

                result.UnifyChannelsWithSameBoneIndex();
                result.FilterChannels(0f);
                result.Clamp(4, true);

                boneWeight = result.ToBoneWeight(true);
            }
            else if (edgeIndex != -1)
            {
                Edge edge = m_SpriteMeshData.edges[edgeIndex];
                Vector2 pos1 = m_SpriteMeshData.GetPosition(edge.index1);
                Vector2 pos2 = m_SpriteMeshData.GetPosition(edge.index2);
                Vector2 dir1 = (position - pos1);
                Vector2 dir2 = (pos2 - pos1);
                float t = Vector2.Dot(dir1, dir2.normalized) / dir2.magnitude;
                t = Mathf.Clamp01(t);
                BoneWeight bw1 = m_SpriteMeshData.GetWeight(edge.index1).ToBoneWeight(true);
                BoneWeight bw2 = m_SpriteMeshData.GetWeight(edge.index2).ToBoneWeight(true);

                boneWeight = EditableBoneWeightUtility.Lerp(bw1, bw2, t);
            }

            m_SpriteMeshDataController.CreateVertex(position, edgeIndex);
            m_SpriteMeshData.GetWeight(m_SpriteMeshData.vertexCount - 1).SetFromBoneWeight(boneWeight);
            Triangulate();
        }

        private void SelectVertex(int index, bool additiveToggle)
        {
            if (index < 0)
                throw new ArgumentException("Index out of range");

            bool selected = selection.Contains(index);
            if (selected)
            {
                if (additiveToggle)
                {
                    cacheUndo.BeginUndoOperation(TextContent.selection);
                    selection.Select(index, false);
                }
            }
            else
            {
                cacheUndo.BeginUndoOperation(TextContent.selection);

                if (!additiveToggle)
                    ClearSelection();

                selection.Select(index, true);
            }

            cacheUndo.IncrementCurrentGroup();
        }

        private void SelectEdge(int index, bool additiveToggle)
        {
            Debug.Assert(index >= 0);

            Edge edge = m_SpriteMeshData.edges[index];

            cacheUndo.BeginUndoOperation(TextContent.selection);

            bool selected = selection.Contains(edge.index1) && selection.Contains(edge.index2);
            if (selected)
            {
                if (additiveToggle)
                {
                    selection.Select(edge.index1, false);
                    selection.Select(edge.index2, false);
                }
            }
            else
            {
                if (!additiveToggle)
                    ClearSelection();

                selection.Select(edge.index1, true);
                selection.Select(edge.index2, true);
            }

            cacheUndo.IncrementCurrentGroup();
        }

        private void ClearSelection()
        {
            cacheUndo.BeginUndoOperation(TextContent.selection);
            selection.Clear();
        }

        private void MoveSelectedVertices(Vector2 delta)
        {
            delta = MathUtility.MoveRectInsideFrame(CalculateRectFromSelection(), frame, delta);

            var indices = selection.elements;

            foreach (int index in indices)
            {
                Vector2 v = m_SpriteMeshData.GetPosition(index);
                m_SpriteMeshData.SetPosition(index, ClampToFrame(v + delta));
            }

            Triangulate();
        }

        private void CreateEdge(Vector2 position, int hoveredVertexIndex, int hoveredEdgeIndex)
        {
            position = ClampToFrame(position);
            EdgeIntersectionResult edgeIntersectionResult = CalculateEdgeIntersection(selection.activeElement, hoveredVertexIndex, hoveredEdgeIndex, position);

            cacheUndo.BeginUndoOperation(TextContent.createEdge);

            int selectIndex = -1;

            if (edgeIntersectionResult.endVertexIndex == -1)
            {
                CreateVertex(edgeIntersectionResult.endPosition, edgeIntersectionResult.intersectEdgeIndex);
                m_SpriteMeshDataController.CreateEdge(selection.activeElement, m_SpriteMeshData.vertexCount - 1);
                selectIndex = m_SpriteMeshData.vertexCount - 1;
            }
            else
            {
                m_SpriteMeshDataController.CreateEdge(selection.activeElement, edgeIntersectionResult.endVertexIndex);
                Triangulate();
                selectIndex = edgeIntersectionResult.endVertexIndex;
            }

            ClearSelection();
            selection.Select(selectIndex, true);

            cacheUndo.IncrementCurrentGroup();
        }

        private void SplitEdge(Vector2 position, int edgeIndex)
        {
            cacheUndo.BeginUndoOperation(TextContent.splitEdge);

            Vector2 clampedMousePos = ClampToFrame(position);

            CreateVertex(clampedMousePos, edgeIndex);

            cacheUndo.IncrementCurrentGroup();
        }

        private bool GetSelectedEdge(out Edge edge)
        {
            edge = default(Edge);

            if (selection.Count != 2)
                return false;

            var indices = selection.elements;

            int index1 = indices[0];
            int index2 = indices[1];

            edge = new Edge(index1, index2);

            if (!m_SpriteMeshData.edges.Contains(edge))
                return false;

            return true;
        }

        private void RemoveEdge(Edge edge)
        {
            cacheUndo.BeginUndoOperation(TextContent.removeEdge);
            m_SpriteMeshDataController.RemoveEdge(edge);
            Triangulate();
        }

        private void RemoveSelectedVertices()
        {
            cacheUndo.BeginUndoOperation(TextContent.removeVertices);

            m_SpriteMeshDataController.RemoveVertex(selection.elements);

            Triangulate();
            selection.Clear();
        }

        private void Triangulate()
        {
            m_SpriteMeshDataController.Triangulate(triangulator);
            m_SpriteMeshDataController.SortTrianglesByDepth();
        }

        private Vector2 ClampToFrame(Vector2 position)
        {
            return MathUtility.ClampPositionToRect(position, frame);
        }

        private Rect CalculateRectFromSelection()
        {
            Rect rect = new Rect();

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            var indices = selection.elements;

            foreach (int index in indices)
            {
                Vector2 v = m_SpriteMeshData.GetPosition(index);

                min.x = Mathf.Min(min.x, v.x);
                min.y = Mathf.Min(min.y, v.y);

                max.x = Mathf.Max(max.x, v.x);
                max.y = Mathf.Max(max.y, v.y);
            }

            rect.min = min;
            rect.max = max;

            return rect;
        }

        private void UpdateEdgeInstersection()
        {
            if (selection.Count == 1)
                m_EdgeIntersectionResult = CalculateEdgeIntersection(selection.activeElement, spriteMeshView.hoveredVertex, spriteMeshView.hoveredEdge, ClampToFrame(spriteMeshView.mouseWorldPosition));
        }

        private EdgeIntersectionResult CalculateEdgeIntersection(int vertexIndex, int hoveredVertexIndex, int hoveredEdgeIndex, Vector2 targetPosition)
        {
            Debug.Assert(vertexIndex >= 0);

            EdgeIntersectionResult edgeIntersection = new EdgeIntersectionResult();

            edgeIntersection.startVertexIndex = vertexIndex;
            edgeIntersection.endVertexIndex = hoveredVertexIndex;
            edgeIntersection.endPosition = targetPosition;
            edgeIntersection.intersectEdgeIndex = -1;

            Vector2 startPoint = m_SpriteMeshData.GetPosition(edgeIntersection.startVertexIndex);

            bool intersectsEdge = false;
            int lastIntersectingEdgeIndex = -1;

            do
            {
                lastIntersectingEdgeIndex = edgeIntersection.intersectEdgeIndex;

                if (intersectsEdge)
                {
                    Vector2 dir = edgeIntersection.endPosition - startPoint;
                    edgeIntersection.endPosition += dir.normalized * 10f;
                }

                intersectsEdge = SegmentIntersectsEdge(startPoint, edgeIntersection.endPosition, vertexIndex, ref edgeIntersection.endPosition, out edgeIntersection.intersectEdgeIndex);

                //if we are hovering a vertex and intersect an edge indexing it we forget about the intersection
                if (intersectsEdge && m_SpriteMeshData.edges[edgeIntersection.intersectEdgeIndex].Contains(edgeIntersection.endVertexIndex))
                {
                    edgeIntersection.intersectEdgeIndex = -1;
                    intersectsEdge = false;
                    edgeIntersection.endPosition = m_SpriteMeshData.GetPosition(edgeIntersection.endVertexIndex);
                }

                if (intersectsEdge)
                {
                    edgeIntersection.endVertexIndex = -1;

                    Edge intersectingEdge = m_SpriteMeshData.edges[edgeIntersection.intersectEdgeIndex];
                    Vector2 newPointScreen = spriteMeshView.WorldToScreen(edgeIntersection.endPosition);
                    Vector2 edgeV1 = spriteMeshView.WorldToScreen(m_SpriteMeshData.GetPosition(intersectingEdge.index1));
                    Vector2 edgeV2 = spriteMeshView.WorldToScreen(m_SpriteMeshData.GetPosition(intersectingEdge.index2));

                    if ((newPointScreen - edgeV1).magnitude <= kSnapDistance)
                        edgeIntersection.endVertexIndex = intersectingEdge.index1;
                    else if ((newPointScreen - edgeV2).magnitude <= kSnapDistance)
                        edgeIntersection.endVertexIndex = intersectingEdge.index2;

                    if (edgeIntersection.endVertexIndex != -1)
                    {
                        edgeIntersection.intersectEdgeIndex = -1;
                        intersectsEdge = false;
                        edgeIntersection.endPosition = m_SpriteMeshData.GetPosition(edgeIntersection.endVertexIndex);
                    }
                }
            }
            while (intersectsEdge && lastIntersectingEdgeIndex != edgeIntersection.intersectEdgeIndex);

            edgeIntersection.intersectEdgeIndex = intersectsEdge ? edgeIntersection.intersectEdgeIndex : hoveredEdgeIndex;

            if (edgeIntersection.endVertexIndex != -1 && !intersectsEdge)
                edgeIntersection.endPosition = m_SpriteMeshData.GetPosition(edgeIntersection.endVertexIndex);

            return edgeIntersection;
        }

        private bool SegmentIntersectsEdge(Vector2 p1, Vector2 p2, int ignoreIndex, ref Vector2 point, out int intersectingEdgeIndex)
        {
            intersectingEdgeIndex = -1;

            float sqrDistance = float.MaxValue;

            for (int i = 0; i < m_SpriteMeshData.edges.Count; i++)
            {
                Edge edge = m_SpriteMeshData.edges[i];
                Vector2 v1 = m_SpriteMeshData.GetPosition(edge.index1);
                Vector2 v2 = m_SpriteMeshData.GetPosition(edge.index2);
                Vector2 pointTmp = Vector2.zero;

                if (!edge.Contains(ignoreIndex) && MathUtility.SegmentIntersection(p1, p2, v1, v2, ref pointTmp))
                {
                    float sqrMagnitude = (pointTmp - p1).sqrMagnitude;
                    if (sqrMagnitude < sqrDistance)
                    {
                        sqrDistance = sqrMagnitude;
                        intersectingEdgeIndex = i;
                        point = pointTmp;
                    }
                }
            }

            return intersectingEdgeIndex != -1;
        }
    }
}
