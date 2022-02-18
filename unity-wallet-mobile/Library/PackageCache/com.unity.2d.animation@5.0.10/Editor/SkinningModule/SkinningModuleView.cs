using System;
using UnityEditor.U2D.Common;
using UnityEditor.U2D.Layout;
using UnityEngine;
using UnityEditor.ShortcutManagement;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal partial class SkinningModule
    {
        private LayoutOverlay m_LayoutOverlay;
        private BoneToolbar m_BoneToolbar;
        private MeshToolbar m_MeshToolbar;
        private WeightToolbar m_WeightToolbar;

        private InternalEditorBridge.ShortcutContext m_ShortcutContext;

        private static SkinningModule GetModuleFromContext(ShortcutArguments args)
        {
            var sc = args.context as InternalEditorBridge.ShortcutContext;
            if (sc == null)
                return null;

            return sc.context as SkinningModule;
        }

        [Shortcut("2D/Animation/Toggle Tool Text", typeof(InternalEditorBridge.ShortcutContext), KeyCode.BackQuote, ShortcutModifiers.Shift)]
        private static void CollapseToolbar(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null)
            {
                SkinningModuleSettings.compactToolBar = !SkinningModuleSettings.compactToolBar;
            }
        }

        [Shortcut("2D/Animation/Restore Bind Pose", typeof(InternalEditorBridge.ShortcutContext), KeyCode.Alpha1, ShortcutModifiers.Shift)]
        private static void DisablePoseModeKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                var effectiveSkeleton = sm.skinningCache.GetEffectiveSkeleton(sm.skinningCache.selectedSprite);
                if (effectiveSkeleton != null && effectiveSkeleton.isPosePreview)
                {
                    using (sm.skinningCache.UndoScope(TextContent.restorePose))
                    {
                        sm.skinningCache.RestoreBindPose();
                        sm.skinningCache.events.shortcut.Invoke("#1");
                    }   
                }
            }
        }

        [Shortcut("2D/Animation/Toggle Character Mode", typeof(InternalEditorBridge.ShortcutContext), KeyCode.Alpha2, ShortcutModifiers.Shift)]
        private static void ToggleCharacterModeKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled && sm.skinningCache.hasCharacter)
            {
                var tool = sm.skinningCache.GetTool(Tools.SwitchMode);

                using (sm.skinningCache.UndoScope(TextContent.setMode))
                {
                    if (tool.isActive)
                        tool.Deactivate();
                    else
                        tool.Activate();
                }

                sm.skinningCache.events.shortcut.Invoke("#2");
            }
        }

        [Shortcut("2D/Animation/Preview Pose", typeof(InternalEditorBridge.ShortcutContext), KeyCode.Q, ShortcutModifiers.Shift)]
        private static void EditPoseKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetSkeletonTool(Tools.EditPose);
                sm.skinningCache.events.shortcut.Invoke("#q");
            }
        }

        [Shortcut("2D/Animation/Edit Bone", typeof(InternalEditorBridge.ShortcutContext), KeyCode.W, ShortcutModifiers.Shift)]
        private static void EditJointsKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetSkeletonTool(Tools.EditJoints);
                sm.skinningCache.events.shortcut.Invoke("#w");
            }
        }

        [Shortcut("2D/Animation/Create Bone", typeof(InternalEditorBridge.ShortcutContext), KeyCode.E, ShortcutModifiers.Shift)]
        private static void CreateBoneKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetSkeletonTool(Tools.CreateBone);
                sm.skinningCache.events.shortcut.Invoke("#e");
            }
        }

        [Shortcut("2D/Animation/Split Bone", typeof(InternalEditorBridge.ShortcutContext), KeyCode.R, ShortcutModifiers.Shift)]
        private static void SplitBoneKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetSkeletonTool(Tools.SplitBone);
                sm.skinningCache.events.shortcut.Invoke("#r");
            }
        }

        [Shortcut("2D/Animation/Auto Geometry", typeof(InternalEditorBridge.ShortcutContext), KeyCode.A, ShortcutModifiers.Shift)]
        private static void GenerateGeometryKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetMeshTool(Tools.GenerateGeometry);
                sm.skinningCache.events.shortcut.Invoke("#a");
            }
        }

        [Shortcut("2D/Animation/Edit Geometry", typeof(InternalEditorBridge.ShortcutContext), KeyCode.S, ShortcutModifiers.Shift)]
        private static void MeshSelectionKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetMeshTool(Tools.EditGeometry);
                sm.skinningCache.events.shortcut.Invoke("#s");
            }
        }

        [Shortcut("2D/Animation/Create Vertex", typeof(InternalEditorBridge.ShortcutContext), KeyCode.J, ShortcutModifiers.Shift)]
        private static void CreateVertex(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetMeshTool(Tools.CreateVertex);
                sm.skinningCache.events.shortcut.Invoke("#d");
            }
        }

        [Shortcut("2D/Animation/Create Edge", typeof(InternalEditorBridge.ShortcutContext), KeyCode.G, ShortcutModifiers.Shift)]
        private static void CreateEdgeKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetMeshTool(Tools.CreateEdge);
                sm.skinningCache.events.shortcut.Invoke("#g");
            }
        }

        [Shortcut("2D/Animation/Split Edge", typeof(InternalEditorBridge.ShortcutContext), KeyCode.H, ShortcutModifiers.Shift)]
        private static void SplitEdge(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetMeshTool(Tools.SplitEdge);
                sm.skinningCache.events.shortcut.Invoke("#h");
            }
        }

        [Shortcut("2D/Animation/Auto Weights", typeof(InternalEditorBridge.ShortcutContext), KeyCode.Z, ShortcutModifiers.Shift)]
        private static void GenerateWeightsKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetWeightTool(Tools.GenerateWeights);
                sm.skinningCache.events.shortcut.Invoke("#z");
            }
        }

        [Shortcut("2D/Animation/Weight Slider", typeof(InternalEditorBridge.ShortcutContext), KeyCode.X, ShortcutModifiers.Shift)]
        private static void WeightSliderKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetWeightTool(Tools.WeightSlider);
                sm.skinningCache.events.shortcut.Invoke("#x");
            }
        }

        [Shortcut("2D/Animation/Weight Brush", typeof(InternalEditorBridge.ShortcutContext), KeyCode.N, ShortcutModifiers.Shift)]
        private static void WeightBrushKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.SetWeightTool(Tools.WeightBrush);
                sm.skinningCache.events.shortcut.Invoke("#c");
            }
        }

        [Shortcut("2D/Animation/Bone Influence", typeof(InternalEditorBridge.ShortcutContext), KeyCode.V, ShortcutModifiers.Shift)]
        private static void BoneInfluenceKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled && sm.skinningCache.mode == SkinningMode.Character)
            {
                sm.SetWeightTool(Tools.BoneInfluence);
                sm.skinningCache.events.shortcut.Invoke("#v");
            }
        }

        [Shortcut("2D/Animation/Paste Panel Weights", typeof(InternalEditorBridge.ShortcutContext), KeyCode.B, ShortcutModifiers.Shift)]
        private static void PastePanelKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.m_HorizontalToggleTools.TogglePasteTool(sm.currentTool);
                sm.skinningCache.events.shortcut.Invoke("#b");
            }
        }

        [Shortcut("2D/Animation/Visibility Panel", typeof(InternalEditorBridge.ShortcutContext), KeyCode.P, ShortcutModifiers.Shift)]
        private static void VisibilityPanelKey(ShortcutArguments args)
        {
            var sm = GetModuleFromContext(args);
            if (sm != null && !sm.spriteEditor.editingDisabled)
            {
                sm.m_HorizontalToggleTools.ToggleVisibilityTool(sm.currentTool);
                sm.skinningCache.events.shortcut.Invoke("#p");
            }
        }

        private void AddMainUI(VisualElement mainView)
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("LayoutOverlay/LayoutOverlay.uxml");
            VisualElement clone = visualTree.CloneTree();
            m_LayoutOverlay = clone.Q<LayoutOverlay>("LayoutOverlay");

            mainView.Add(m_LayoutOverlay);
            m_LayoutOverlay.hasScrollbar = true;
            m_LayoutOverlay.StretchToParentSize();

            CreateBoneToolbar();
            CreateMeshToolbar();
            CreateWeightToolbar();

            m_ShortcutContext = new InternalEditorBridge.ShortcutContext()
            {
                isActive = isFocused,
                context = this
            };
            InternalEditorBridge.RegisterShortcutContext(m_ShortcutContext);
            InternalEditorBridge.AddEditorApplicationProjectLoadedCallback(OnProjectLoaded);
        }

        private void OnProjectLoaded()
        {
            if (m_ShortcutContext != null)
                InternalEditorBridge.RegisterShortcutContext(m_ShortcutContext);
        }
        
        private void DoViewGUI()
        {
            if (spriteEditor.editingDisabled == m_BoneToolbar.enabledSelf)
            {
                m_BoneToolbar.SetEnabled(!spriteEditor.editingDisabled);
                m_MeshToolbar.SetEnabled(!spriteEditor.editingDisabled);
                m_WeightToolbar.SetEnabled(!spriteEditor.editingDisabled);
            }

            if (spriteEditor.editingDisabled == m_LayoutOverlay.rightOverlay.enabledSelf)
            {
                m_LayoutOverlay.rightOverlay.SetEnabled(!spriteEditor.editingDisabled);
                m_LayoutOverlay.rightOverlay.visible = !spriteEditor.editingDisabled;
            }
        }

        private bool isFocused()
        {
            return spriteEditor != null && (EditorWindow.focusedWindow == spriteEditor as EditorWindow);
        }

        private void CreateBoneToolbar()
        {
            m_BoneToolbar = BoneToolbar.GenerateFromUXML();
            m_BoneToolbar.Setup(skinningCache);
            m_LayoutOverlay.verticalToolbar.AddToContainer(m_BoneToolbar);

            m_BoneToolbar.SetSkeletonTool += SetSkeletonTool;
            m_BoneToolbar.SetEnabled(!spriteEditor.editingDisabled);
        }

        private void CreateMeshToolbar()
        {
            m_MeshToolbar = MeshToolbar.GenerateFromUXML();
            m_MeshToolbar.skinningCache = skinningCache;
            m_LayoutOverlay.verticalToolbar.AddToContainer(m_MeshToolbar);

            m_MeshToolbar.SetMeshTool += SetMeshTool;
            m_MeshToolbar.SetEnabled(!spriteEditor.editingDisabled);
        }

        private void CreateWeightToolbar()
        {
            m_WeightToolbar = WeightToolbar.GenerateFromUXML();
            m_WeightToolbar.skinningCache = skinningCache;
            m_LayoutOverlay.verticalToolbar.AddToContainer(m_WeightToolbar);
            m_WeightToolbar.SetWeightTool += SetWeightTool;
            m_WeightToolbar.SetEnabled(!spriteEditor.editingDisabled);
        }

        private void SetSkeletonTool(Tools toolType)
        {
            var tool = skinningCache.GetTool(toolType) as SkeletonToolWrapper;

            if (currentTool == tool)
                return;

            using (skinningCache.UndoScope(TextContent.setTool))
            {
                ActivateTool(tool);

                if (tool.editBindPose)
                    skinningCache.RestoreBindPose();
            }
        }

        private void SetMeshTool(Tools toolType)
        {
            var tool  = skinningCache.GetTool(toolType);

            if (currentTool == tool)
                return;

            using (skinningCache.UndoScope(TextContent.setTool))
            {
                ActivateTool(tool);
                skinningCache.RestoreBindPose();
                UnselectBones();
            }
        }

        private void SetWeightTool(Tools toolType)
        {
            var tool = skinningCache.GetTool(toolType);

            if (currentTool == tool)
                return;

            using (skinningCache.UndoScope(TextContent.setTool))
            {
                ActivateTool(tool);
            }
        }

        private void ActivateTool(BaseTool tool)
        {
            m_ModuleToolGroup.ActivateTool(tool);
            UpdateToggleState();
            skinningCache.events.toolChanged.Invoke(tool);
        }

        private void UnselectBones()
        {
            skinningCache.skeletonSelection.Clear();
            skinningCache.events.boneSelectionChanged.Invoke();
        }

        private void UpdateToggleState()
        {
            Debug.Assert(m_BoneToolbar != null);
            Debug.Assert(m_MeshToolbar != null);
            Debug.Assert(m_WeightToolbar != null);

            m_BoneToolbar.UpdateToggleState();
            m_MeshToolbar.UpdateToggleState();
            m_WeightToolbar.UpdateToggleState();
        }

        private void RemoveMainUI(VisualElement mainView)
        {
            InternalEditorBridge.RemoveEditorApplicationProjectLoadedCallback(OnProjectLoaded);
            InternalEditorBridge.UnregisterShortcutContext(m_ShortcutContext);
        }
    }
}
