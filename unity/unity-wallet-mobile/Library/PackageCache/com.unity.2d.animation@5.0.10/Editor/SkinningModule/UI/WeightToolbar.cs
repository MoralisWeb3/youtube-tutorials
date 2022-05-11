using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class WeightToolbar : Toolbar
    {
        public class CustomUXMLFactor : UxmlFactory<WeightToolbar, UxmlTraits> {}

        public event Action<Tools> SetWeightTool = (mode) => {};
        public SkinningCache skinningCache { get; set; }

        public WeightToolbar()
        {
            styleSheets.Add(ResourceLoader.Load<StyleSheet>("SkinningModule/WeightToolbarStyle.uss"));
        }

        public void EnableBoneInfluenceWidget()
        {
            this.Q<Button>("BoneInfluenceWidget").SetEnabled(true);
        }

        public void DisableBoneInfluenceWidget()
        {
            this.Q<Button>("BoneInfluenceWidget").SetEnabled(false);
        }

        public void BindElements()
        {
            var button = this.Q<Button>("AutoGenerateWeight");
            button.clickable.clicked += () => SetWeightTool(Tools.GenerateWeights);

            button = this.Q<Button>("WeightPainterSlider");
            button.clickable.clicked += () => SetWeightTool(Tools.WeightSlider);

            button = this.Q<Button>("WeightPainterBrush");
            button.clickable.clicked += () => SetWeightTool(Tools.WeightBrush);

            button = this.Q<Button>("BoneInfluenceWidget");
            button.clickable.clicked += () => SetWeightTool(Tools.BoneInfluence);
        }

        public static WeightToolbar GenerateFromUXML()
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("SkinningModule/WeightToolbar.uxml");
            var clone = visualTree.CloneTree().Q<WeightToolbar>("WeightToolbar");
            clone.BindElements();
            return clone;
        }

        public void UpdateToggleState()
        {
            //TODO: Make UI not be aware of BaseTool, Cache, etc. Use Tool enum
            var button = this.Q<Button>("WeightPainterSlider");
            SetButtonChecked(button, skinningCache.GetTool(Tools.WeightSlider).isActive);

            button = this.Q<Button>("BoneInfluenceWidget");
            SetButtonChecked(button, skinningCache.GetTool(Tools.BoneInfluence).isActive);

            button = this.Q<Button>("WeightPainterBrush");
            SetButtonChecked(button, skinningCache.GetTool(Tools.WeightBrush).isActive);

            button = this.Q<Button>("AutoGenerateWeight");
            SetButtonChecked(button, skinningCache.GetTool(Tools.GenerateWeights).isActive);
        }
    }
}
