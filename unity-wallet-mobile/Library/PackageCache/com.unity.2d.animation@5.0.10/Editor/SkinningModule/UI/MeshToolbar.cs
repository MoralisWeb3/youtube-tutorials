using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class MeshToolbar : Toolbar
    {
        public class MeshToolbarFactory : UxmlFactory<MeshToolbar, MeshToolbarUxmlTraits> {}
        public class MeshToolbarUxmlTraits : UxmlTraits {}

        public event Action<Tools> SetMeshTool = (mode) => {};
        public SkinningCache skinningCache { get; set; }

        public MeshToolbar()
        {
            styleSheets.Add(ResourceLoader.Load<StyleSheet>("SkinningModule/MeshToolbarStyle.uss"));
        }

        public void BindElements()
        {
            var button = this.Q<Button>("SelectGeometry");
            button.clickable.clicked += () => { SetMeshTool(Tools.EditGeometry); };

            button = this.Q<Button>("CreateVertex");
            button.clickable.clicked += () => { SetMeshTool(Tools.CreateVertex); };

            button = this.Q<Button>("CreateEdge");
            button.clickable.clicked += () => { SetMeshTool(Tools.CreateEdge); };

            button = this.Q<Button>("SplitEdge");
            button.clickable.clicked += () => { SetMeshTool(Tools.SplitEdge); };

            button = this.Q<Button>("GenerateGeometry");
            button.clickable.clicked += () => { SetMeshTool(Tools.GenerateGeometry); };
        }

        public void UpdateToggleState()
        {
            //TODO: Make UI not be aware of BaseTool, Cache, etc. Use Tool enum
            var button = this.Q<Button>("SelectGeometry");
            SetButtonChecked(button, skinningCache.GetTool(Tools.EditGeometry).isActive);

            button = this.Q<Button>("CreateVertex");
            SetButtonChecked(button, skinningCache.GetTool(Tools.CreateVertex).isActive);

            button = this.Q<Button>("CreateEdge");
            SetButtonChecked(button, skinningCache.GetTool(Tools.CreateEdge).isActive);

            button = this.Q<Button>("SplitEdge");
            SetButtonChecked(button, skinningCache.GetTool(Tools.SplitEdge).isActive);

            button = this.Q<Button>("GenerateGeometry");
            SetButtonChecked(button, skinningCache.GetTool(Tools.GenerateGeometry).isActive);
        }

        public static MeshToolbar GenerateFromUXML()
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("SkinningModule/MeshToolbar.uxml");
            var clone = visualTree.CloneTree().Q<MeshToolbar>("MeshToolbar");
            clone.BindElements();
            return clone;
        }
    }
}
