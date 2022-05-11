using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class HorizontalToggleTools
    {
        private static class Styles
        {
            public static GUIContent visibilityCollapseIcon = new GUIContent(IconUtility.LoadIconResource("Visibility_Tool", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr(TextContent.visibilityIconTooltip));
            public static GUIContent visibilityIcon = new GUIContent(L10n.Tr(TextContent.visibilityIconText), IconUtility.LoadIconResource("Visibility_Tool", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr(TextContent.visibilityIconTooltip));
            public static GUIContent characterCollapseIcon = new GUIContent(IconUtility.LoadIconResource("character_Mode", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr(TextContent.restorePose));
            public static GUIContent characterIcon = new GUIContent(L10n.Tr(TextContent.characterIconText), IconUtility.LoadIconResource("character_Mode", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr(TextContent.restorePose));
            public static GUIContent spriteSheetIcon = new GUIContent(L10n.Tr(TextContent.spriteSheetIconText), IconUtility.LoadIconResource("Sprite_Mode", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr(TextContent.spriteSheetIconTooltip));
            public static GUIContent spriteSheetCollapseIcon = new GUIContent(IconUtility.LoadIconResource("Sprite_Mode", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr(TextContent.spriteSheetIconTooltip));
            public static GUIContent copyIcon = new GUIContent(L10n.Tr(TextContent.copyText), IconUtility.LoadIconResource("Copy", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr(TextContent.copyTooltip));
            public static GUIContent copyCollapseIcon = new GUIContent(IconUtility.LoadIconResource("Copy", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr(TextContent.copyTooltip));
            public static GUIContent pasteIcon = new GUIContent(L10n.Tr(TextContent.pasteText), IconUtility.LoadIconResource("Paste", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr(TextContent.pasteTooltip));
            public static GUIContent pasteCollapseIcon = new GUIContent(IconUtility.LoadIconResource("Paste", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr(TextContent.pasteTooltip));
        }

        private SkinningCache skinningCache { get; set; }

        private CopyTool copyTool
        {
            get { return skinningCache.GetTool(Tools.CopyPaste) as CopyTool; }
        }

        private VisibilityTool visibilityTool
        {
            get { return skinningCache.GetTool(Tools.Visibility) as VisibilityTool; }
        }

        private SwitchModeTool switchmodeTool
        {
            get { return skinningCache.GetTool(Tools.SwitchMode) as SwitchModeTool; }
        }

        private GUIContent spriteSheetIcon
        {
            get { return collapseToolbar ? Styles.spriteSheetCollapseIcon : Styles.spriteSheetIcon; }
        }

        private GUIContent copyIcon
        {
            get { return collapseToolbar ? Styles.copyCollapseIcon : Styles.copyIcon; }
        }
        private GUIContent pasteIcon
        {
            get { return collapseToolbar ? Styles.pasteCollapseIcon : Styles.pasteIcon; }
        }

        internal Action<BaseTool> onActivateTool = (b) => {};
        private BaseTool m_PreviousTool;

        public bool collapseToolbar { get; set; }

        internal HorizontalToggleTools(SkinningCache s)
        {
            skinningCache = s;
        }

        internal void DoGUI(Rect drawArea, BaseTool currentTool, bool isDisabled)
        {
            using (new EditorGUI.DisabledScope(isDisabled))
            {
                GUILayout.BeginArea(drawArea);
                EditorGUILayout.BeginHorizontal();

                DoPreviewToggle();
                DoModeToggle();
                DoCopyToggle(currentTool);
                GUILayout.FlexibleSpace();
                DoVisibilityToggle(currentTool);

                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();                
            }
        }

        private void StorePreviousTool(BaseTool currentTool)
        {
            if(currentTool != copyTool && currentTool != visibilityTool)
                m_PreviousTool = currentTool;
        }

        private void DoModeToggle()
        {
            if (skinningCache.hasCharacter)
            {
                EditorGUI.BeginChangeCheck();
                var isActive = GUILayout.Toggle(switchmodeTool.isActive , spriteSheetIcon, EditorStyles.toolbarButton);
                if (EditorGUI.EndChangeCheck())
                {
                    using (skinningCache.UndoScope(TextContent.setMode))
                    {
                        if (isActive)
                            switchmodeTool.Activate();
                        else
                            switchmodeTool.Deactivate();
                    }
                }
            }
        }

        private void DoCopyToggle(BaseTool currentTool)
        {
            if (GUILayout.Button(copyIcon, EditorStyles.toolbarButton))
            {
                copyTool.OnCopyActivated();
            }
            EditorGUI.BeginChangeCheck();
            GUILayout.Toggle(copyTool.isActive, pasteIcon, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
                TogglePasteTool(currentTool);
        }

        internal void TogglePasteTool(BaseTool currentTool)
        {
            if (!copyTool.isActive)
            {
                onActivateTool(copyTool);
                StorePreviousTool(currentTool);
            }
            else if (m_PreviousTool != null)
            {
                onActivateTool(m_PreviousTool);
            }
        }

        void DoVisibilityToggle(BaseTool currentTool)
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.Toggle(visibilityTool.isActive, visbilityIcon, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
                ToggleVisibilityTool(currentTool);
        }

        GUIContent visbilityIcon { get { return collapseToolbar ? Styles.visibilityCollapseIcon : Styles.visibilityIcon; } }

        internal void ToggleVisibilityTool(BaseTool currentTool)
        {
            onActivateTool(visibilityTool);
        }

        private void DoPreviewToggle()
        {
            var skeleton = skinningCache.GetEffectiveSkeleton(skinningCache.selectedSprite);

            EditorGUI.BeginDisabledGroup(skeleton == null || skeleton.isPosePreview == false);
            EditorGUI.BeginChangeCheck();
            GUILayout.Button(characterIcon, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                using (skinningCache.UndoScope("Restore Pose"))
                {
                    skinningCache.RestoreBindPose();
                    skinningCache.events.restoreBindPose.Invoke();
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        GUIContent characterIcon { get { return collapseToolbar ? Styles.characterCollapseIcon : Styles.characterIcon; } }
    }
}
