using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class GenerateWeightsPanel : VisualElement
    {
        public class GenerateWeightsPanelFactory : UxmlFactory<GenerateWeightsPanel, GenerateWeightsPanelUxmlTraits> {}
        public class GenerateWeightsPanelUxmlTraits : UxmlTraits {}

        public event Action onGenerateWeights = () => {};
        public event Action onNormalizeWeights = () => {};
        public event Action onClearWeights = () => {};
        private VisualElement m_AssociateBoneControl;
        private Toggle m_AssociateBonesToggle;
        Button m_GenerateWeightsButton;

        public bool associateBones
        {
            get { return m_AssociateBoneControl.visible && m_AssociateBonesToggle.value; }
            set { m_AssociateBonesToggle.value = value; }
        }

        public GenerateWeightsPanel()
        {
            styleSheets.Add(ResourceLoader.Load<StyleSheet>("SkinningModule/GenerateWeightsPanelStyle.uss"));
            if (EditorGUIUtility.isProSkin)
                AddToClassList("Dark");
            AddToClassList("AssociateBoneEnabled");
            RegisterCallback<MouseDownEvent>((e) => { e.StopPropagation(); });
            RegisterCallback<MouseUpEvent>((e) => { e.StopPropagation(); });
        }

        public void BindElements()
        {
            m_AssociateBoneControl = this.Q<VisualElement>("AssociateBonesControl");
            m_GenerateWeightsButton = this.Q<Button>("GenerateWeightsButton");
            m_GenerateWeightsButton.clickable.clicked += OnGenerateWeights;

            var normalizeWeightsButton = this.Q<Button>("NormalizeWeightsButton");
            normalizeWeightsButton.clickable.clicked += OnNormalizeWeights;

            var clearWeightsButton = this.Q<Button>("ClearWeightsButton");
            clearWeightsButton.clickable.clicked += OnClearWeights;

            m_AssociateBonesToggle = this.Q<Toggle>("AssociateBonesField");
        }

        public string generateButtonText
        {
            set { m_GenerateWeightsButton.text = value; }
        }

        public void Update(bool enableAssociateBones)
        {
            m_AssociateBoneControl.SetHiddenFromLayout(!enableAssociateBones);
            if (enableAssociateBones)
            {
                RemoveFromClassList("AssociateBoneDisabled");
                AddToClassList("AssociateBoneEnabled");
            }
            else
            {
                RemoveFromClassList("AssociateBoneEnabled");
                AddToClassList("AssociateBoneDisabled");
            }
        }

        public void OnGenerateWeights()
        {
            onGenerateWeights();
        }

        public void OnNormalizeWeights()
        {
            onNormalizeWeights();
        }

        public void OnClearWeights()
        {
            onClearWeights();
        }

        public static GenerateWeightsPanel GenerateFromUXML()
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("SkinningModule/GenerateWeightsPanel.uxml");
            var clone = visualTree.CloneTree().Q<GenerateWeightsPanel>("GenerateWeightsPanel");
            clone.BindElements();
            return clone;
        }
    }
}
