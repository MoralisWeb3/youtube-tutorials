using System;
using UnityEngine.U2D.Common;
using UnityEditor.U2D.Layout;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class VisibilityToolWindow : VisualElement, IVisibilityToolWindow
    {
        public class CustomUxmlFactory : UxmlFactory<VisibilityToolWindow, UxmlTraits> {}

        VisualElement m_SelectorContainer;
        VisualElement m_Container;
        Slider m_BoneOpacitySlider;
        Slider m_MeshOpacitySlider;
        private LayoutOverlay m_Layout;
        
        List<Button> m_Tabs;
        int m_CurrentSelectedTab = 0;

        public event Action<float> onBoneOpacitySliderChange = (f) => {};
        public event Action<float> onMeshOpacitySliderChange = (f) => {};
        public event Action onBoneOpacitySliderChangeBegin = () => {};
        public event Action onBoneOpacitySliderChangeEnd = () => {};
        public event Action onMeshOpacitySliderChangeBegin = () => {};
        public event Action onMeshOpacitySliderChangeEnd = () => {};

        public static VisibilityToolWindow CreateFromUXML()
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("SkinningModule/VisibilityToolWindow.uxml");
            var ve = visualTree.CloneTree().Q("VisibilityToolWindow") as VisibilityToolWindow;
            var resizer = ve.Q("Resizer");
            resizer.AddManipulator(new VisibilityToolResizer());
            ve.styleSheets.Add(ResourceLoader.Load<StyleSheet>("SkinningModule/VisibilityTool.uss"));
            if (EditorGUIUtility.isProSkin)
                ve.AddToClassList("Dark");
            ve.BindElements();
            return ve;
        }

        public void BindElements()
        {
            m_SelectorContainer = this.Q("VisibilityToolSelection");
            m_Container = this.Q("VisibilityToolContainer");
            m_BoneOpacitySlider = this.Q<Slider>("BoneOpacitySlider");
            m_BoneOpacitySlider.RegisterValueChangedCallback(OnBoneOpacitySliderValueChangd);
            m_MeshOpacitySlider = this.Q<Slider>("MeshOpacitySlider");
            m_MeshOpacitySlider.RegisterValueChangedCallback(OnMeshOpacitySliderValueChangd);
            RegisterCallback<MouseDownEvent>(OpacityChangeBegin, TrickleDown.TrickleDown);
            RegisterCallback<MouseCaptureOutEvent>(OpacityChangeEnd, TrickleDown.TrickleDown);
            // case 1200857 StopPropagation when bubbling up so that main IMGUI doesn't get the event
            RegisterCallback<MouseDownEvent>(evt => evt.StopPropagation());
            m_Tabs = new List<Button>();
            m_SelectorContainer.Clear();
        }

        public void Initialize(LayoutOverlay layout)
        {
            m_Layout = layout;
            layout.rightOverlay.Add(this);
            Hide();
        }

        public void Show()
        {
            m_Container.Clear();
            this.SetHiddenFromLayout(false);
            m_Layout.VisibilityWindowOn(true);
        }

        public void Hide()
        {
            m_Container.Clear();
            this.SetHiddenFromLayout(true);
            m_Layout.VisibilityWindowOn(false);
        }

        bool IsOpacityTarget(IEventHandler target, VisualElement opacityTarget)
        {
            var ve = target as VisualElement;
            while (ve != null && ve != this)
            {
                if (ve == opacityTarget)
                    return true;
                ve = ve.parent;
            }
            return false;
        }

        void OpacityChangeBegin(MouseDownEvent evt)
        {
            if (IsOpacityTarget(evt.target, m_BoneOpacitySlider))
                onBoneOpacitySliderChangeBegin();
            else if (IsOpacityTarget(evt.target, m_MeshOpacitySlider))
                onMeshOpacitySliderChangeBegin();
        }

        void OpacityChangeEnd(MouseCaptureOutEvent evt)
        {
            if (IsOpacityTarget(evt.target, m_BoneOpacitySlider))
                onBoneOpacitySliderChangeEnd();
            else if (IsOpacityTarget(evt.target, m_MeshOpacitySlider))
                onMeshOpacitySliderChangeEnd();
        }

        void OnBoneOpacitySliderValueChangd(ChangeEvent<float> evt)
        {
            onBoneOpacitySliderChange(evt.newValue);
        }

        void OnMeshOpacitySliderValueChangd(ChangeEvent<float> evt)
        {
            onMeshOpacitySliderChange(evt.newValue);
        }

        public void SetBoneOpacitySliderValue(float value)
        {
            m_BoneOpacitySlider.value = value;
            m_BoneOpacitySlider.MarkDirtyRepaint();
        }

        public void SetMeshOpacitySliderValue(float value)
        {
            m_MeshOpacitySlider.value = value;
            m_MeshOpacitySlider.MarkDirtyRepaint();
        }

        public void AddToolTab(string name, Action onClick)
        {
            var tab = new Button()
            {
                text = name
            };
            tab.AddToClassList("visibilityToolTab");
            if (EditorGUIUtility.isProSkin)
                tab.AddToClassList("visibilityToolTabDark");
            m_SelectorContainer.Add(tab);
            m_Tabs.Add(tab);
            tab.clickable.clicked += onClick;
        }

        public void SetActiveTab(int toolIndex)
        {
            if (m_Tabs.Count > toolIndex && toolIndex >= 0)
            {
                m_Tabs[m_CurrentSelectedTab].SetChecked(false);
                m_Tabs[toolIndex].SetChecked(true);
                m_CurrentSelectedTab = toolIndex;
            }
        }

        public void SetContent(VisualElement content)
        {
            m_Container.Clear();
            m_Container.Add(content);
        }

        public void SetToolAvailable(int toolIndex, bool available)
        {
            if (m_Tabs.Count > toolIndex && toolIndex >= 0)
            {
                m_Tabs[toolIndex].SetHiddenFromLayout(!available);
                if (available == false && toolIndex == m_CurrentSelectedTab)
                    m_Container.Clear();
            }
        }
    }

    internal class VisibilityTool : BaseTool, IVisibilityToolModel
    {
        private VisibilityToolWindow m_ToolView;

        private MeshPreviewBehaviour m_MeshPreviewBehaviour = new MeshPreviewBehaviour();
        public SkeletonTool skeletonTool { set; private get; }

        VisibilityToolController m_Controller;
        internal override void OnCreate()
        {
            m_ToolView = VisibilityToolWindow.CreateFromUXML();
            m_Controller = new VisibilityToolController(this, new IVisibilityTool[]
            {
                new BoneVisibilityTool(skinningCache),
                new SpriteVisibilityTool(skinningCache)
            },
                () => skeletonTool,
                () => previewBehaviour);
        }

        public override IMeshPreviewBehaviour previewBehaviour
        {
            get { return m_MeshPreviewBehaviour; }
        }

        public override void Initialize(LayoutOverlay layout)
        {
            m_ToolView.Initialize(layout);
        }

        protected override void OnGUI()
        {
            skeletonTool.skeletonStyle = SkeletonStyles.WeightMap;
            skeletonTool.mode = SkeletonMode.EditPose;
            skeletonTool.editBindPose = false;
            skeletonTool.DoGUI();
        }

        protected override void OnActivate()
        {
            m_MeshPreviewBehaviour.showWeightMap = true;
            m_Controller.Activate();
        }

        protected override void OnDeactivate()
        {
            m_Controller.Deactivate();
        }

        int IVisibilityToolModel.currentToolIndex
        {
            get
            {
                return skinningCache.visibililtyToolIndex;
            }
            set
            {
                skinningCache.visibililtyToolIndex = value;
            }
        }

        float IVisibilityToolModel.boneOpacityValue
        {
            get
            {
                return VisibilityToolSettings.boneOpacity;
            }
            set
            {
                VisibilityToolSettings.boneOpacity = value;
            }
        }

        float IVisibilityToolModel.meshOpacityValue
        {
            get
            {
                return VisibilityToolSettings.meshOpacity;
            }
            set
            {
                VisibilityToolSettings.meshOpacity = value;
            }
        }

        UndoScope IVisibilityToolModel.UndoScope(string value)
        {
            return skinningCache.UndoScope(value);
        }

        void IVisibilityToolModel.BeginUndoOperation(string value)
        {
            skinningCache.BeginUndoOperation(value);
        }

        IVisibilityToolWindow IVisibilityToolModel.view { get { return m_ToolView;} }
        SkinningCache IVisibilityToolModel.skinningCache { get { return skinningCache;} }
    }

    internal interface IVisibilityToolModel
    {
        int currentToolIndex { get; set; }
        float meshOpacityValue { get; set; }
        float boneOpacityValue { get; set; }
        UndoScope UndoScope(string value);
        void BeginUndoOperation(string value);
        IVisibilityToolWindow view { get; }
        SkinningCache skinningCache { get; }
    }

    internal interface IVisibilityToolWindow
    {
        void AddToolTab(string name, Action callback);
        void SetToolAvailable(int i, bool available);
        void SetBoneOpacitySliderValue(float value);
        void SetMeshOpacitySliderValue(float value);
        event Action<float> onBoneOpacitySliderChange;
        event Action<float> onMeshOpacitySliderChange;
        event Action onBoneOpacitySliderChangeBegin;
        event Action onBoneOpacitySliderChangeEnd;
        event Action onMeshOpacitySliderChangeBegin;
        event Action onMeshOpacitySliderChangeEnd;
        void Show();
        void Hide();
        void SetActiveTab(int index);
        void SetContent(VisualElement content);
    }

    internal class VisibilityToolController
    {
        IVisibilityTool[] m_Tools;
        IVisibilityToolModel m_Model;
        Func<SkeletonTool> m_SkeletonTool;
        Func<IMeshPreviewBehaviour> m_MeshPreviewBehaviour;
        bool m_DeactivateBoneaTool = false;
        
        private IVisibilityTool currentTool
        {
            get { return m_Model.currentToolIndex == -1 ? null : m_Tools[m_Model.currentToolIndex]; }
            set { m_Model.currentToolIndex = value == null ? -1 : Array.FindIndex(m_Tools, x => x == value); }
        }

        private IVisibilityTool defaultTool
        {
            get { return Array.Find(m_Tools, t => t.isAvailable); }
        }

        public VisibilityToolController(IVisibilityToolModel model, IVisibilityTool[] tools, Func<SkeletonTool> skeletonTool = null, Func<IMeshPreviewBehaviour> meshPreviewBehaviour = null)
        {
            m_Model = model;
            m_Tools = tools;
            for (int i = 0; i < m_Tools.Length; ++i)
            {
                int index = i;
                var tool = m_Tools[i];
                tool.SetAvailabilityChangeCallback(() => OnToolAvailabilityChange(index));
                tool.Setup();
                model.view.AddToolTab(tool.name, () => ActivateToolWithUndo(tool));
                model.view.SetToolAvailable(i, tool.isAvailable);
            }
            m_SkeletonTool = skeletonTool;
            m_MeshPreviewBehaviour = meshPreviewBehaviour;
        }

        public void Activate()
        {
            m_Model.view.Show();

            if (currentTool == null)
                currentTool = defaultTool;
            ActivateTool(currentTool);

            m_Model.view.SetBoneOpacitySliderValue(m_Model.boneOpacityValue);
            m_Model.view.SetMeshOpacitySliderValue(m_Model.meshOpacityValue);
            m_Model.view.onBoneOpacitySliderChange -= OnBoneOpacityChange;
            m_Model.view.onMeshOpacitySliderChange -= OnMeshOpacityChange;
            m_Model.view.onBoneOpacitySliderChange += OnBoneOpacityChange;
            m_Model.view.onMeshOpacitySliderChange += OnMeshOpacityChange;
            m_Model.view.onBoneOpacitySliderChangeBegin -= OnBoneOpacityChangeBegin;
            m_Model.view.onBoneOpacitySliderChangeBegin += OnBoneOpacityChangeBegin;
            m_Model.view.onBoneOpacitySliderChangeEnd -= OnBoneOpacityChangeEnd;
            m_Model.view.onBoneOpacitySliderChangeEnd += OnBoneOpacityChangeEnd;
            m_Model.view.onMeshOpacitySliderChangeBegin -= OnMeshOpacityChangeBegin;
            m_Model.view.onMeshOpacitySliderChangeBegin += OnMeshOpacityChangeBegin;
            m_Model.view.onMeshOpacitySliderChangeEnd -= OnMeshOpacityChangeEnd;
            m_Model.view.onMeshOpacitySliderChangeEnd += OnMeshOpacityChangeEnd;
        }

        public void Deactivate()
        {
            m_Model.view.Hide();

            if (currentTool != null)
                currentTool.Deactivate();

            m_Model.view.onBoneOpacitySliderChange -= OnBoneOpacityChange;
            m_Model.view.onMeshOpacitySliderChange -= OnMeshOpacityChange;
            m_Model.view.onBoneOpacitySliderChangeBegin -= OnBoneOpacityChangeBegin;
            m_Model.view.onBoneOpacitySliderChangeEnd -= OnBoneOpacityChangeEnd;
            m_Model.view.onMeshOpacitySliderChangeBegin -= OnMeshOpacityChangeBegin;
            m_Model.view.onMeshOpacitySliderChangeEnd -= OnMeshOpacityChangeEnd;
        }

        void OnBoneOpacityChangeBegin()
        {
            if (m_SkeletonTool != null && m_SkeletonTool() != null && !m_SkeletonTool().isActive)
            {
                m_SkeletonTool().Activate();
                m_DeactivateBoneaTool = true;
            }
                
        }

        void OnBoneOpacityChangeEnd()
        {
            if (m_SkeletonTool != null && m_SkeletonTool() != null && m_SkeletonTool().isActive && m_DeactivateBoneaTool)
                m_SkeletonTool().Deactivate();
        }

        void OnMeshOpacityChangeBegin()
        {
            m_Model.skinningCache.events.meshPreviewBehaviourChange.Invoke(m_MeshPreviewBehaviour());
        }

        void OnMeshOpacityChangeEnd()
        {
            m_Model.skinningCache.events.meshPreviewBehaviourChange.Invoke(null);
        }

        private void OnBoneOpacityChange(float value)
        {
            m_Model.boneOpacityValue = value;
        }

        private void OnMeshOpacityChange(float value)
        {
            m_Model.meshOpacityValue = value;
        }

        private void OnToolAvailabilityChange(int toolIndex)
        {
            var toolChanged = m_Tools[toolIndex];
            m_Model.view.SetToolAvailable(toolIndex, toolChanged.isAvailable);
            if (toolChanged == currentTool && toolChanged.isAvailable == false)
                ActivateTool(defaultTool);
        }

        private void ActivateToolWithUndo(IVisibilityTool tool)
        {
            if (currentTool != tool && tool.isAvailable)
            {
                m_Model.BeginUndoOperation(TextContent.visibilityTab);
                ActivateTool(tool);
            }
        }

        private void ActivateTool(IVisibilityTool tool)
        {
            if (tool.isAvailable == false)
                return;

            if (currentTool != null)
                currentTool.Deactivate();

            currentTool = tool;
            currentTool.Activate();

            m_Model.view.SetActiveTab(m_Model.currentToolIndex);
            m_Model.view.SetContent(currentTool.view);
        }
    }
}
