using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class WeightPainterToolWrapper : BaseTool
    {
        [SerializeField]
        private WeightPainterMode m_PaintMode;

        [SerializeField]
        private WeightPainterTool m_WeightPainterTool;

        private string m_Title;

        public override IMeshPreviewBehaviour previewBehaviour
        {
            get { return m_WeightPainterTool.previewBehaviour; }
        }

        public WeightPainterTool weightPainterTool
        {
            get { return m_WeightPainterTool; }
            set { m_WeightPainterTool = value; }
        }

        public WeightPainterMode paintMode
        {
            get { return m_PaintMode; }
            set { m_PaintMode = value; }
        }

        public string title
        {
            set { m_Title = value; }
        }

        public override int defaultControlID
        {
            get { return weightPainterTool.defaultControlID; }
        }

        protected override void OnActivate()
        {
            weightPainterTool.Activate();
            weightPainterTool.panelTitle = m_Title;
        }

        protected override void OnDeactivate()
        {
            weightPainterTool.Deactivate();
        }

        protected override void OnGUI()
        {
            weightPainterTool.paintMode = paintMode;

            weightPainterTool.DoGUI();
        }
    }
}
