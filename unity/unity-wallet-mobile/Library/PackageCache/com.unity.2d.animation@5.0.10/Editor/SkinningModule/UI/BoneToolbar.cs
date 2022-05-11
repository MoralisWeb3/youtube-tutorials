using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class BoneToolbar : Toolbar
    {
        public class BoneToolbarFactory : UxmlFactory<BoneToolbar, BoneToolbarUxmlTraits> {}
        public class BoneToolbarUxmlTraits : UxmlTraits {}

        public event Action<Tools> SetSkeletonTool = (mode) => {};
        public SkinningCache skinningCache { get; private set; }

        public BoneToolbar()
        {
            styleSheets.Add(ResourceLoader.Load<StyleSheet>("SkinningModule/BoneToolbarStyle.uss"));
        }

        public void BindElements()
        {
            var editPose = this.Q<Button>("EditPose");
            editPose.clickable.clicked += () => { SetSkeletonTool(Tools.EditPose); };

            var editJoints = this.Q<Button>("EditJoints");
            editJoints.clickable.clicked += () => { SetSkeletonTool(Tools.EditJoints); };

            var createBone = this.Q<Button>("CreateBone");
            createBone.clickable.clicked += () => { SetSkeletonTool(Tools.CreateBone); };

            var splitBone = this.Q<Button>("SplitBone");
            splitBone.clickable.clicked += () => { SetSkeletonTool(Tools.SplitBone); };
        }

        public void Setup(SkinningCache s)
        {
            skinningCache = s;
            skinningCache.events.skinningModeChanged.AddListener(OnSkinningModeChange);
        }

        private void OnSkinningModeChange(SkinningMode mode)
        {
            if (skinningCache.hasCharacter)
            {
                if (mode == SkinningMode.SpriteSheet)
                {
                    this.Q<Button>("EditJoints").SetEnabled(false);
                    this.Q<Button>("CreateBone").SetEnabled(false);
                    this.Q<Button>("SplitBone").SetEnabled(false);
                    
                    if (skinningCache.GetTool(Tools.EditJoints).isActive
                        || skinningCache.GetTool(Tools.CreateBone).isActive
                        || skinningCache.GetTool(Tools.SplitBone).isActive)
                        SetSkeletonTool(Tools.EditPose);
                }
                else if (mode == SkinningMode.Character)
                {
                    this.Q<Button>("EditJoints").SetEnabled(true);
                    this.Q<Button>("CreateBone").SetEnabled(true);
                    this.Q<Button>("SplitBone").SetEnabled(true);
                }
            }
        }

        public void UpdateToggleState()
        {
            //TODO: Make UI not be aware of BaseTool, Cache, etc. Use Tool enum
            var toolButton = this.Q<Button>("EditPose");
            SetButtonChecked(toolButton, skinningCache.GetTool(Tools.EditPose).isActive);

            toolButton = this.Q<Button>("EditJoints");
            SetButtonChecked(toolButton, skinningCache.GetTool(Tools.EditJoints).isActive);

            toolButton = this.Q<Button>("CreateBone");
            SetButtonChecked(toolButton, skinningCache.GetTool(Tools.CreateBone).isActive);

            toolButton = this.Q<Button>("SplitBone");
            SetButtonChecked(toolButton, skinningCache.GetTool(Tools.SplitBone).isActive);

            OnSkinningModeChange(skinningCache.mode);
        }

        public static BoneToolbar GenerateFromUXML()
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("SkinningModule/BoneToolbar.uxml");
            var clone = visualTree.CloneTree().Q<BoneToolbar>("BoneToolbar");
            clone.BindElements();
            return clone;
        }
    }
}
