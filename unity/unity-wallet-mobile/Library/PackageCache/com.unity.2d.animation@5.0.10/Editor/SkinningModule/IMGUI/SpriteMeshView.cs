using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class SpriteMeshView : ISpriteMeshView
    {
        readonly int m_VertexHashCode = "Vertex".GetHashCode();
        readonly int m_EdgeHashCode = "Edge".GetHashCode();
        const string kDeleteCommandName = "Delete";
        const string kSoftDeleteCommandName = "SoftDelete";
        static readonly Color kEdgeColor = Color.cyan;
        static readonly Color kEdgeHoveredColor = Color.yellow;
        static readonly Color kEdgeSelectedColor = Color.yellow;
        const float kEdgeWidth = 2f;
        const float kVertexRadius = 2.5f;

        private class Styles
        {
            public readonly GUIStyle pointNormalStyle;
            public readonly GUIStyle pointHoveredStyle;
            public readonly GUIStyle pointSelectedStyle;

            public Styles()
            {
                Texture2D pointNormal = ResourceLoader.Load<Texture2D>("SkinningModule/dotCyan.png");
                Texture2D pointHovered = ResourceLoader.Load<Texture2D>("SkinningModule/dotYellow.png");
                Texture2D pointSelected = ResourceLoader.Load<Texture2D>("SkinningModule/dotYellow.png");

                pointNormalStyle = new GUIStyle();
                pointNormalStyle.normal.background = pointNormal;
                pointNormalStyle.fixedWidth = 8f;
                pointNormalStyle.fixedHeight = 8f;

                pointHoveredStyle = new GUIStyle();
                pointHoveredStyle.normal.background = pointHovered;
                pointHoveredStyle.fixedWidth = 10f;
                pointHoveredStyle.fixedHeight = 10f;

                pointSelectedStyle = new GUIStyle();
                pointSelectedStyle.normal.background = pointSelected;
                pointSelectedStyle.fixedWidth = 10f;
                pointSelectedStyle.fixedHeight = 10f;
            }
        }

        private Styles m_Styles;
        private Styles styles
        {
            get
            {
                if (m_Styles == null)
                    m_Styles = new Styles();

                return m_Styles;
            }
        }

        int m_HoveredEdge = -1;
        int m_HoveredEdgeControlID = -1;
        int m_MoveEdgeControlID = -1;
        int m_HoveredVertex = -1;
        int m_PrevHoveredVertex = -1;
        int m_HoveredVertexControlID = -1;
        int m_MoveVertexControlID = -1;
        Color m_TempColor;
        SliderData m_HotSliderData = SliderData.zero;
        MeshEditorAction m_PreviousActiveAction = MeshEditorAction.None;
        private Vector2 m_MouseWorldPosition;
        private float m_NearestVertexDistance;
        private float m_NearestEdgeDistance;
        private int m_NearestVertex = -1;
        private int m_NearestEdge = -1;


        public SpriteMeshViewMode mode { get; set; }
        public ISelection<int> selection { get; set; }
        public int defaultControlID { get; set; }
        public Rect frame { get; set; }
        private IGUIWrapper guiWrapper { get; set; }

        public Vector2 mouseWorldPosition
        {
            get { return m_MouseWorldPosition; }
        }

        public int hoveredVertex
        {
            get { return m_HoveredVertex; }
        }

        public int hoveredEdge
        {
            get { return m_HoveredEdge; }
        }

        public int closestEdge
        {
            get { return m_NearestEdge; }
        }

        public SpriteMeshView(IGUIWrapper gw)
        {
            guiWrapper = gw;
        }

        public void CancelMode()
        {
            if (mode != SpriteMeshViewMode.EditGeometry)
            {
                if (guiWrapper.IsKeyDown(KeyCode.Escape) || guiWrapper.IsMouseDown(1))
                {
                    mode = SpriteMeshViewMode.EditGeometry;
                    guiWrapper.UseCurrentEvent();
                }
            }
        }

        public void BeginLayout()
        {
            var vertexControlID = guiWrapper.GetControlID(m_VertexHashCode, FocusType.Passive);
            var edgeControlID = guiWrapper.GetControlID(m_EdgeHashCode, FocusType.Passive);

            if (guiWrapper.eventType == EventType.Layout || guiWrapper.eventType == EventType.MouseMove)
            {
                m_NearestVertexDistance = float.MaxValue;
                m_NearestEdgeDistance = float.MaxValue;
                m_NearestVertex = -1;
                m_NearestEdge = -1;
                m_MouseWorldPosition = guiWrapper.GUIToWorld(guiWrapper.mousePosition);
                m_HoveredVertexControlID = vertexControlID;
                m_HoveredEdgeControlID = edgeControlID;
                m_PrevHoveredVertex = m_HoveredVertex;
                m_HoveredVertex = -1;
                m_HoveredEdge = -1;

                if (guiWrapper.IsControlHot(0))
                {
                    m_MoveVertexControlID = -1;
                    m_MoveEdgeControlID = -1;
                }
            }
        }

        public void EndLayout()
        {
            guiWrapper.LayoutControl(m_HoveredEdgeControlID, m_NearestEdgeDistance);
            guiWrapper.LayoutControl(m_HoveredVertexControlID, m_NearestVertexDistance);

            if(guiWrapper.IsControlNearest(m_HoveredVertexControlID))
                m_HoveredVertex = m_NearestVertex;

            if (guiWrapper.IsControlNearest(m_HoveredEdgeControlID))
                m_HoveredEdge = m_NearestEdge;

            if (guiWrapper.eventType == EventType.Layout || guiWrapper.eventType == EventType.MouseMove)
                if (m_PrevHoveredVertex != m_HoveredVertex)
                    guiWrapper.Repaint();
        }

        public void LayoutVertex(Vector2 position, int index)
        {
            if (guiWrapper.eventType == EventType.Layout)
            {
                var distance = guiWrapper.DistanceToCircle(position, kVertexRadius);

                if (distance <= m_NearestVertexDistance)
                {
                    m_NearestVertexDistance = distance;
                    m_NearestVertex = index;
                }
            }
        }

        public void LayoutEdge(Vector2 startPosition, Vector2 endPosition, int index)
        {
            if (guiWrapper.eventType == EventType.Layout)
            {
                var distance = guiWrapper.DistanceToSegment(startPosition, endPosition);

                if (distance < m_NearestEdgeDistance)
                {
                    m_NearestEdgeDistance = distance;
                    m_NearestEdge = index;
                }
            }
        }

        public bool DoCreateVertex()
        {
            if (mode == SpriteMeshViewMode.CreateVertex && IsActionActive(MeshEditorAction.CreateVertex))
                ConsumeMouseMoveEvents();

            if (IsActionTriggered(MeshEditorAction.CreateVertex))
            {
                guiWrapper.SetGuiChanged(true);
                guiWrapper.UseCurrentEvent();

                return true;
            }

            return false;
        }

        public bool DoSelectVertex(out bool additive)
        {
            additive = false;

            if (IsActionTriggered(MeshEditorAction.SelectVertex))
            {
                additive = guiWrapper.isActionKeyDown;
                guiWrapper.Repaint();
                return true;
            }

            return false;
        }

        public bool DoMoveVertex(out Vector2 delta)
        {
            delta = Vector2.zero;

            if (IsActionTriggered(MeshEditorAction.MoveVertex))
            {
                m_MoveVertexControlID = m_HoveredVertexControlID;
                m_HotSliderData.position = mouseWorldPosition;
            }

            Vector3 newPosition;
            if (guiWrapper.DoSlider(m_MoveVertexControlID, m_HotSliderData, out newPosition))
            {
                delta = newPosition - m_HotSliderData.position;
                m_HotSliderData.position = newPosition;
                return true;
            }

            return false;
        }

        public bool DoMoveEdge(out Vector2 delta)
        {
            delta = Vector2.zero;

            if (IsActionTriggered(MeshEditorAction.MoveEdge))
            {
                m_MoveEdgeControlID = m_HoveredEdgeControlID;
                m_HotSliderData.position = mouseWorldPosition;
            }

            Vector3 newPosition;
            if (guiWrapper.DoSlider(m_MoveEdgeControlID, m_HotSliderData, out newPosition))
            {
                delta = newPosition - m_HotSliderData.position;
                m_HotSliderData.position = newPosition;
                return true;
            }

            return false;
        }

        public bool DoCreateEdge()
        {
            if (IsActionActive(MeshEditorAction.CreateEdge))
                ConsumeMouseMoveEvents();

            if (IsActionTriggered(MeshEditorAction.CreateEdge))
            {
                guiWrapper.SetGuiChanged(true);
                guiWrapper.UseCurrentEvent();
                return true;
            }

            return false;
        }

        public bool DoSplitEdge()
        {
            if (IsActionActive(MeshEditorAction.SplitEdge))
                ConsumeMouseMoveEvents();

            if (IsActionTriggered(MeshEditorAction.SplitEdge))
            {
                guiWrapper.UseCurrentEvent();
                guiWrapper.SetGuiChanged(true);
                return true;
            }

            return false;
        }

        public bool DoSelectEdge(out bool additive)
        {
            additive = false;

            if (IsActionTriggered(MeshEditorAction.SelectEdge))
            {
                additive = guiWrapper.isActionKeyDown;
                guiWrapper.Repaint();
                return true;
            }

            return false;
        }

        public bool DoRemove()
        {
            if (IsActionTriggered(MeshEditorAction.Remove))
            {
                guiWrapper.UseCurrentEvent();
                guiWrapper.SetGuiChanged(true);
                return true;
            }

            return false;
        }

        public void DrawVertex(Vector2 position)
        {
            DrawingUtility.DrawGUIStyleCap(0, position, Quaternion.identity, 1f, styles.pointNormalStyle);
        }

        public void DrawVertexHovered(Vector2 position)
        {
            DrawingUtility.DrawGUIStyleCap(0, position, Quaternion.identity, 1f, styles.pointHoveredStyle);
        }

        public void DrawVertexSelected(Vector2 position)
        {
            DrawingUtility.DrawGUIStyleCap(0, position, Quaternion.identity, 1f, styles.pointSelectedStyle);
        }

        public void BeginDrawEdges()
        {
            if (guiWrapper.eventType != EventType.Repaint)
                return;

            DrawingUtility.BeginSolidLines();
            m_TempColor = Handles.color;
        }

        public void EndDrawEdges()
        {
            if (guiWrapper.eventType != EventType.Repaint)
                return;

            DrawingUtility.EndLines();
            Handles.color = m_TempColor;
        }

        public void DrawEdge(Vector2 startPosition, Vector2 endPosition)
        {
            DrawEdge(startPosition, endPosition, kEdgeColor);
        }

        public void DrawEdgeHovered(Vector2 startPosition, Vector2 endPosition)
        {
            DrawEdge(startPosition, endPosition, kEdgeHoveredColor);
        }

        public void DrawEdgeSelected(Vector2 startPosition, Vector2 endPosition)
        {
            DrawEdge(startPosition, endPosition, kEdgeSelectedColor);
        }

        public bool IsActionActive(MeshEditorAction action)
        {
            if (guiWrapper.isAltDown || !guiWrapper.IsControlHot(0))
                return false;

            var canCreateEdge = CanCreateEdge();
            var canSplitEdge = CanSplitEdge();

            if (action == MeshEditorAction.None)
                return guiWrapper.IsControlNearest(defaultControlID);

            if (action == MeshEditorAction.CreateVertex)
            {
                if(!frame.Contains(mouseWorldPosition))
                    return false;

                if (mode == SpriteMeshViewMode.EditGeometry)
                    return guiWrapper.IsControlNearest(defaultControlID);

                if (mode == SpriteMeshViewMode.CreateVertex)
                    return hoveredVertex == -1;
            }

            if (action == MeshEditorAction.MoveVertex)
                return guiWrapper.IsControlNearest(m_HoveredVertexControlID);

            if (action == MeshEditorAction.CreateEdge)
                return canCreateEdge;

            if (action == MeshEditorAction.SplitEdge)
                return canSplitEdge;

            if (action == MeshEditorAction.MoveEdge)
                return guiWrapper.IsControlNearest(m_HoveredEdgeControlID);

            if (action == MeshEditorAction.SelectVertex)
                return guiWrapper.IsControlNearest(m_HoveredVertexControlID);

            if (action == MeshEditorAction.SelectEdge)
                return mode == SpriteMeshViewMode.EditGeometry &&
                    guiWrapper.IsControlNearest(m_HoveredEdgeControlID) &&
                    !canCreateEdge && !canSplitEdge;

            if (action == MeshEditorAction.Remove)
                return true;

            return false;
        }

        public bool IsActionHot(MeshEditorAction action)
        {
            if (action == MeshEditorAction.None)
                return guiWrapper.IsControlHot(0);

            if (action == MeshEditorAction.MoveVertex)
                return guiWrapper.IsControlHot(m_HoveredVertexControlID);

            if (action == MeshEditorAction.MoveEdge)
                return guiWrapper.IsControlHot(m_HoveredEdgeControlID);

            return false;
        }

        public bool IsActionTriggered(MeshEditorAction action)
        {
            if (!IsActionActive(action))
                return false;

            if (action == MeshEditorAction.CreateVertex)
            {
                if (mode == SpriteMeshViewMode.EditGeometry)
                    return guiWrapper.IsMouseDown(0) && guiWrapper.clickCount == 2;
            }

            if (action == MeshEditorAction.Remove)
            {
                if ((guiWrapper.eventType == EventType.ValidateCommand || guiWrapper.eventType == EventType.ExecuteCommand)
                    && (guiWrapper.commandName == kSoftDeleteCommandName || guiWrapper.commandName == kDeleteCommandName))
                {
                    if (guiWrapper.eventType == EventType.ExecuteCommand)
                        return true;

                    guiWrapper.UseCurrentEvent();
                }

                return false;
            }

            if(action != MeshEditorAction.None)
                return guiWrapper.IsMouseDown(0);

            return false;
        }

        public Vector2 WorldToScreen(Vector2 position)
        {
            return HandleUtility.WorldToGUIPoint(position);
        }

        private void ConsumeMouseMoveEvents()
        {
            if (guiWrapper.eventType == EventType.MouseMove || (guiWrapper.eventType == EventType.MouseDrag && guiWrapper.mouseButton == 0))
                guiWrapper.UseCurrentEvent();
        }

        private bool CanCreateEdge()
        {
            if(!frame.Contains(mouseWorldPosition) || !(guiWrapper.IsControlNearest(defaultControlID) || guiWrapper.IsControlNearest(m_HoveredVertexControlID) || guiWrapper.IsControlNearest(m_HoveredEdgeControlID)))
                return false;

            if (mode == SpriteMeshViewMode.EditGeometry)
                return guiWrapper.isShiftDown && selection.Count == 1 && !selection.Contains(hoveredVertex);

            if (mode == SpriteMeshViewMode.CreateEdge)
                return selection.Count == 1 && !selection.Contains(hoveredVertex);

            return false;
        }

        private bool CanSplitEdge()
        {
            if(!frame.Contains(mouseWorldPosition) || !(guiWrapper.IsControlNearest(defaultControlID) || guiWrapper.IsControlNearest(m_HoveredEdgeControlID)))
                return false;

            if (mode == SpriteMeshViewMode.EditGeometry)
                return guiWrapper.isShiftDown && m_NearestEdge != -1 && hoveredVertex == -1 && selection.Count == 0;

            if (mode == SpriteMeshViewMode.SplitEdge)
                return m_NearestEdge != -1 && hoveredVertex == -1;

            return false;
        }

        private void DrawEdge(Vector2 startPosition, Vector2 endPosition, Color color)
        {
            if (guiWrapper.eventType != EventType.Repaint)
                return;

            Handles.color = color;
            float width = kEdgeWidth / Handles.matrix.m00;

            DrawingUtility.DrawSolidLine(width, startPosition, endPosition);
        }

        public void DoRepaint()
        {
            if(guiWrapper.eventType != EventType.Layout)
                return;

            var action = MeshEditorAction.None;

            if(IsActionActive(MeshEditorAction.CreateVertex))
                action = MeshEditorAction.CreateVertex;
            else if(IsActionActive(MeshEditorAction.CreateEdge))
                action = MeshEditorAction.CreateEdge;
            else if(IsActionActive(MeshEditorAction.SplitEdge))
                action = MeshEditorAction.SplitEdge;

            if(m_PreviousActiveAction != action)
            {
                m_PreviousActiveAction = action;
                guiWrapper.Repaint();
            }
        }

        public bool CanRepaint()
        {
            return guiWrapper.eventType == EventType.Repaint;
        }

        public bool CanLayout()
        {
            return guiWrapper.eventType == EventType.Layout;
        }
    }
}
