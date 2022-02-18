using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal partial class MeshTool : BaseTool
    {
        private MeshCache m_Mesh;
        private ISelection<int> m_SelectionOverride;
        private SpriteMeshController m_SpriteMeshController;
        private SpriteMeshView m_SpriteMeshView;
        private RectSelectionTool<int> m_RectSelectionTool = new RectSelectionTool<int>();
        private RectVertexSelector m_RectVertexSelector = new RectVertexSelector();
        private UnselectTool<int> m_UnselectTool = new UnselectTool<int>();
        private ITriangulator m_Triangulator;

        public MeshCache mesh
        {
            get { return m_Mesh; }
        }

        public SpriteMeshViewMode mode
        {
            get { return m_SpriteMeshView.mode; }
            set { m_SpriteMeshView.mode = value; }
        }

        public bool disable
        {
            get { return m_SpriteMeshController.disable; }
            set { m_SpriteMeshController.disable = value; }
        }

        public ISelection<int> selectionOverride
        {
            get { return m_SelectionOverride; }
            set { m_SelectionOverride = value; }
        }

        public override int defaultControlID
        {
            get
            {
                if (m_Mesh == null)
                    return 0;

                return m_RectSelectionTool.controlID;
            }
        }

        private ISelection<int> selection
        {
            get
            {
                if(selectionOverride != null)
                    return selectionOverride;
                return skinningCache.vertexSelection;
            }
        }

        internal override void OnCreate()
        {
            m_SpriteMeshController = new SpriteMeshController();
            m_SpriteMeshView = new SpriteMeshView(new GUIWrapper());
            m_Triangulator = new Triangulator();
        }

        protected override void OnActivate()
        {
            m_SpriteMeshController.disable = false;
            m_SelectionOverride = null;

            SetupSprite(skinningCache.selectedSprite);

            skinningCache.events.selectedSpriteChanged.AddListener(OnSelectedSpriteChanged);
        }

        protected override void OnDeactivate()
        {
            skinningCache.events.selectedSpriteChanged.RemoveListener(OnSelectedSpriteChanged);
        }

        private void OnSelectedSpriteChanged(SpriteCache sprite)
        {
            SetupSprite(sprite);
        }

        internal void SetupSprite(SpriteCache sprite)
        {
            var mesh = sprite.GetMesh();

            if (m_Mesh != null 
                && m_Mesh != mesh 
                && selection.Count > 0)
                selection.Clear();

            m_Mesh = mesh;
            m_SpriteMeshController.spriteMeshData = m_Mesh;
        }

        private void SetupGUI()
        {
            m_SpriteMeshController.spriteMeshView = m_SpriteMeshView;
            m_SpriteMeshController.triangulator = m_Triangulator;
            m_SpriteMeshController.cacheUndo = skinningCache;
            m_RectSelectionTool.cacheUndo = skinningCache;
            m_RectSelectionTool.rectSelector = m_RectVertexSelector;
            m_RectVertexSelector.selection = selection;
            m_UnselectTool.cacheUndo = skinningCache;
            m_UnselectTool.selection = selection;

            m_SpriteMeshController.frame = new Rect(Vector2.zero, m_Mesh.sprite.textureRect.size);
            m_SpriteMeshController.selection = selection;
            m_SpriteMeshView.defaultControlID = defaultControlID;
            m_RectVertexSelector.spriteMeshData = m_Mesh;
        }

        protected override void OnGUI()
        {
            if (m_Mesh == null)
                return;

            SetupGUI();

            var handlesMatrix = Handles.matrix;
            Handles.matrix *= m_Mesh.sprite.GetLocalToWorldMatrixFromMode();

            BeginPositionOverride();

            EditorGUI.BeginChangeCheck();

            var guiEnabled = GUI.enabled;
            var moveAction = m_SpriteMeshController.spriteMeshView.IsActionHot(MeshEditorAction.MoveEdge) || m_SpriteMeshController.spriteMeshView.IsActionHot(MeshEditorAction.MoveVertex);
            GUI.enabled = (!skinningCache.IsOnVisualElement() && guiEnabled) || moveAction;
            m_SpriteMeshController.OnGUI();
            GUI.enabled = guiEnabled;

            if (EditorGUI.EndChangeCheck())
                UpdateMesh();

            m_RectSelectionTool.OnGUI();
            m_UnselectTool.OnGUI();

            Handles.matrix = handlesMatrix;

            EndPositionOverride();
        }

        public void BeginPositionOverride()
        {
            if(m_Mesh != null)
            {
                m_Mesh.vertexPositionOverride = null;

                var skeleton = skinningCache.GetEffectiveSkeleton(m_Mesh.sprite);

                Debug.Assert(skeleton != null);

                if (skeleton.isPosePreview)
                    m_Mesh.vertexPositionOverride = m_Mesh.sprite.GetMeshPreview().vertices;
            }
        }

        public void EndPositionOverride()
        {
            if(m_Mesh != null)
                m_Mesh.vertexPositionOverride = null;
        }

        public void UpdateWeights()
        {
            InvokeMeshChanged();
        }

        public void UpdateMesh()
        {
            InvokeMeshChanged();
        }

        private void InvokeMeshChanged()
        {
            if(m_Mesh != null)
                skinningCache.events.meshChanged.Invoke(m_Mesh);
        }
    }
}
