using System;
using UnityEditor.U2D.Layout;

namespace UnityEditor.U2D.Animation
{
    internal class SkeletonToolView
    {
        private BoneInspectorPanel m_BoneInspectorPanel;

        public event Action<BoneCache, string> onBoneNameChanged = (b, s) => {};
        public event Action<BoneCache, int> onBoneDepthChanged = (b, i) => {};

        public SkeletonToolView()
        {
            m_BoneInspectorPanel = BoneInspectorPanel.GenerateFromUXML();
            m_BoneInspectorPanel.onBoneNameChanged += (b, n) =>  onBoneNameChanged(b, n);
            m_BoneInspectorPanel.onBoneDepthChanged += (b, d) => onBoneDepthChanged(b, d);
            Hide();
        }
        
        public void Initialize(LayoutOverlay layout)
        {
            layout.rightOverlay.Add(m_BoneInspectorPanel);
        }

        public void Show(BoneCache target)
        {
            m_BoneInspectorPanel.target = target;
            m_BoneInspectorPanel.SetHiddenFromLayout(false);
        }

        public BoneCache target => m_BoneInspectorPanel.target;

        public void Hide()
        {
            m_BoneInspectorPanel.HidePanel();
            m_BoneInspectorPanel.target = null;
        }

        public void Update(string name, int depth)
        {
            m_BoneInspectorPanel.boneName = name;
            m_BoneInspectorPanel.boneDepth = depth;
        }
    }
}
