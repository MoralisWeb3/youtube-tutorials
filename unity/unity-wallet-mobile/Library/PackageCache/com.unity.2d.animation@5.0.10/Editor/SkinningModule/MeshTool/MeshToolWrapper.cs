using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class MeshToolWrapper : BaseTool
    {
        private MeshTool m_MeshTool;
        private SkeletonTool m_SkeletonTool;
        private SpriteMeshViewMode m_MeshMode;
        private bool m_Disable = false;
        private SkeletonMode m_SkeletonMode;
        protected MeshPreviewBehaviour m_MeshPreviewBehaviour = new MeshPreviewBehaviour();

        public MeshTool meshTool
        {
            get { return m_MeshTool; }
            set { m_MeshTool = value; }
        }

        public SkeletonTool skeletonTool
        {
            get { return m_SkeletonTool; }
            set { m_SkeletonTool = value; }
        }

        public SpriteMeshViewMode meshMode
        {
            get { return m_MeshMode; }
            set { m_MeshMode = value; }
        }

        public bool disableMeshEditor
        {
            get { return m_Disable; }
            set { m_Disable = value; }
        }

        public SkeletonMode skeletonMode
        {
            get { return m_SkeletonMode; }
            set { m_SkeletonMode = value; }
        }

        public override int defaultControlID
        {
            get
            {
                Debug.Assert(meshTool != null);

                return meshTool.defaultControlID;
            }
        }

        public override IMeshPreviewBehaviour previewBehaviour
        {
            get { return m_MeshPreviewBehaviour; }
        }

        protected override void OnActivate()
        {
            Debug.Assert(meshTool != null);
            skeletonTool.enableBoneInspector = false;
            skeletonTool.Activate();
            meshTool.Activate();
            m_MeshPreviewBehaviour.drawWireframe = true;
            m_MeshPreviewBehaviour.showWeightMap = false;
            m_MeshPreviewBehaviour.overlaySelected = false;
        }

        protected override void OnDeactivate()
        {
            skeletonTool.Deactivate();
            meshTool.Deactivate();
        }

        protected override void OnGUI()
        {
            DoSkeletonGUI();
            DoMeshGUI();
        }

        protected void DoSkeletonGUI()
        {
            Debug.Assert(skeletonTool != null);

            skeletonTool.mode = skeletonMode;
            skeletonTool.editBindPose = false;
            skeletonTool.DoGUI();
        }

        protected void DoMeshGUI()
        {
            Debug.Assert(meshTool != null);

            meshTool.disable = disableMeshEditor;
            meshTool.mode = meshMode;
            meshTool.DoGUI();
        }
    }
}
