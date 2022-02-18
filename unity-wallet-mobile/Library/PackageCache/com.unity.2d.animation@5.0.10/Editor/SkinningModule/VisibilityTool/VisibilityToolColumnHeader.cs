using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class VisibilityToolColumnHeader : MultiColumnHeader
    {
        public Action<bool> SetAllVisibility = (b) => {};
        public Func<bool> GetAllVisibility = () => true;

        public VisibilityToolColumnHeader(MultiColumnHeaderState state)
            : base(state)
        {
            visibilityColumn = -1;
        }

        public int visibilityColumn { private get; set; }

        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            if (columnIndex == visibilityColumn)
            {
                GUIStyle style = DefaultStyles.columnHeaderCenterAligned;
                EditorGUI.BeginChangeCheck();
                var visibility = GetAllVisibility();
                visibility = GUI.Toggle(headerRect, visibility, visibility ? VisibilityTreeViewBase.VisibilityIconStyle.visibilityOnIcon : VisibilityTreeViewBase.VisibilityIconStyle.visibilityOffIcon, style);
                if (EditorGUI.EndChangeCheck())
                    SetAllVisibility(visibility);
            }
            else
                base.ColumnHeaderGUI(column, headerRect, columnIndex);
        }
    }
}
