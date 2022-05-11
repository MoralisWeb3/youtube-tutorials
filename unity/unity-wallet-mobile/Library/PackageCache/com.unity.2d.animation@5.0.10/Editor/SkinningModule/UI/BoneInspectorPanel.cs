using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class BoneInspectorPanel : VisualElement
    {
        public class BoneInspectorPanelFactory : UxmlFactory<BoneInspectorPanel, BoneInspectorPanelUxmlTraits> {}
        public class BoneInspectorPanelUxmlTraits : UxmlTraits {}
        public event Action<BoneCache, int> onBoneDepthChanged = (bone, depth) => {};
        public event Action<BoneCache, string> onBoneNameChanged = (bone, name) => {};

        private TextField m_BoneNameField;
        private IntegerField m_BoneDepthField;
        
        public string boneName
        {
            get { return m_BoneNameField.value; }
            set { m_BoneNameField.value = value; }
        }

        public BoneCache target { get; set; }
        
        public int boneDepth
        {
            get { return m_BoneDepthField.value; }
            set { m_BoneDepthField.value = value; }
        }

        public BoneInspectorPanel()
        {
            styleSheets.Add(ResourceLoader.Load<StyleSheet>("SkinningModule/BoneInspectorPanelStyle.uss"));

            RegisterCallback<MouseDownEvent>((e) => { e.StopPropagation(); });
            RegisterCallback<MouseUpEvent>((e) => { e.StopPropagation(); });
        }

        public void BindElements()
        {
            m_BoneNameField = this.Q<TextField>("BoneNameField");
            m_BoneDepthField = this.Q<IntegerField>("BoneDepthField");
            m_BoneNameField.RegisterCallback<FocusOutEvent>(BoneNameFocusChanged);
            m_BoneDepthField.RegisterCallback<FocusOutEvent>(BoneDepthFocusChanged);
        }

        private void BoneNameFocusChanged(FocusOutEvent evt)
        {
            onBoneNameChanged(target, boneName);
        }

        private void BoneDepthFocusChanged(FocusOutEvent evt)
        {
            onBoneDepthChanged(target, boneDepth);
        }
        public void HidePanel()
        {
            // We are hidding the panel, sent any unchanged value
            this.SetHiddenFromLayout(true);
            onBoneNameChanged(target, boneName);
            onBoneDepthChanged(target, boneDepth);
        }
        public static BoneInspectorPanel GenerateFromUXML()
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("SkinningModule/BoneInspectorPanel.uxml");
            var clone = visualTree.CloneTree().Q<BoneInspectorPanel>("BoneInspectorPanel");
            clone.BindElements();
            return clone;
        }
    }
}
