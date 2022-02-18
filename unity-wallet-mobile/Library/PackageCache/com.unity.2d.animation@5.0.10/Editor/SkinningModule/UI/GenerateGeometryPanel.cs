using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class GenerateGeometryPanel : VisualElement
    {
        public class GenerateGeometryPanelFactory : UxmlFactory<GenerateGeometryPanel, GenerateGeometryPanelUxmlTraits> {}
        public class GenerateGeometryPanelUxmlTraits : UxmlTraits {}

        public enum GenerateMode
        {
            Single,
            Multiple
        }

        private IntegerField m_OutlineDetailField;
        private IntegerField m_AlphaToleranceField;
        private IntegerField m_SubdivideField;
        private Toggle m_Toggle;

        public event Action<float, byte, float> onAutoGenerateGeometry;
        public event Action<float, byte, float> onAutoGenerateGeometryAll;
        public bool generateWeights
        {
            get { return m_Toggle.value; }
            set { m_Toggle.value = value; }
        }

        public SkinningCache skinningCache { get; set; }

        public GenerateGeometryPanel()
        {
            styleSheets.Add(ResourceLoader.Load<StyleSheet>("SkinningModule/GenerateGeometryPanelStyle.uss"));
            RegisterCallback<MouseDownEvent>((e) => { e.StopPropagation(); });
            RegisterCallback<MouseUpEvent>((e) => { e.StopPropagation(); });
        }

        private void BindElements()
        {
            var generateButton = this.Q<Button>("GenerateGeometryButton");
            generateButton.clickable.clicked += GenerateGeometry;
            generateButton.AddManipulator(generateButton.clickable);

            var generateAllButton = this.Q<Button>("GenerateGeometryAllButton");
            generateAllButton.clickable.clicked += GenerateGeometryAll;
            generateAllButton.AddManipulator(generateAllButton.clickable);

            m_OutlineDetailField = this.Q<IntegerField>("OutlineDetailField");
            m_AlphaToleranceField = this.Q<IntegerField>("AlphaToleranceField");
            m_SubdivideField = this.Q<IntegerField>("SubdivideField");
            m_Toggle = this.Q<Toggle>("GenerateWeightsField");

            var slider = this.Q<Slider>("OutlineDetailSlider");
            LinkSliderToFloatField(slider, m_OutlineDetailField, (x) =>
            {
                GenerateGeomertySettings.outlineDetail = x;
            });
            m_OutlineDetailField.SetValueWithoutNotify(GenerateGeomertySettings.outlineDetail);
            slider.SetValueWithoutNotify(GenerateGeomertySettings.outlineDetail);

            slider = this.Q<Slider>("AlphaToleranceSlider");
            LinkSliderToFloatField(slider, m_AlphaToleranceField,(x) =>
            {
                GenerateGeomertySettings.alphaTolerance = x;
            });
            m_AlphaToleranceField.SetValueWithoutNotify(GenerateGeomertySettings.alphaTolerance);
            slider.SetValueWithoutNotify(GenerateGeomertySettings.alphaTolerance);

            slider = this.Q<Slider>("SubdivideSlider");
            LinkSliderToFloatField(slider, m_SubdivideField,(x) =>
            {
                GenerateGeomertySettings.subdivide = x;
            });
            m_SubdivideField.SetValueWithoutNotify(GenerateGeomertySettings.subdivide);
            slider.SetValueWithoutNotify(GenerateGeomertySettings.subdivide);

            m_Toggle.value = GenerateGeomertySettings.generateWeights;
            m_Toggle.RegisterValueChangedCallback((evt) =>
            {
                GenerateGeomertySettings.generateWeights = evt.newValue;
            });
        }

        private void LinkSliderToFloatField(Slider slider, IntegerField field, Action<int> updatePreferenceAction)
        {
            slider.RegisterValueChangedCallback((evt) =>
                {
                    if (!evt.newValue.Equals(field.value))
                    {
                        var intValue = Mathf.RoundToInt(evt.newValue);
                        field.SetValueWithoutNotify(intValue);
                        updatePreferenceAction(intValue);
                    }

                });
            field.RegisterValueChangedCallback((evt) =>
                {
                    var newValue = evt.newValue;
                    if (!newValue.Equals(slider.value))
                    {
                        newValue = Math.Min(newValue, (int)slider.highValue);
                        newValue = Math.Max(newValue, (int)slider.lowValue);
                        slider.value = newValue;
                        field.SetValueWithoutNotify(newValue);
                        updatePreferenceAction(newValue);
                    }
                });
        }

        public void SetMode(GenerateMode mode)
        {
            RemoveFromClassList("Multiple");
            RemoveFromClassList("Single");
            AddToClassList(mode.ToString());
        }

        public void GenerateGeometry()
        {
            if (onAutoGenerateGeometry != null)
                onAutoGenerateGeometry(m_OutlineDetailField.value, Convert.ToByte(m_AlphaToleranceField.value), m_SubdivideField.value);
        }

        public void GenerateGeometryAll()
        {
            if (onAutoGenerateGeometryAll != null)
                onAutoGenerateGeometryAll(m_OutlineDetailField.value, Convert.ToByte(m_AlphaToleranceField.value), m_SubdivideField.value);
        }

        public static GenerateGeometryPanel GenerateFromUXML()
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("SkinningModule/GenerateGeometryPanel.uxml");
            var clone = visualTree.CloneTree().Q<GenerateGeometryPanel>("GenerateGeometryPanel");
            clone.BindElements();
            return clone;
        }
    }
}
