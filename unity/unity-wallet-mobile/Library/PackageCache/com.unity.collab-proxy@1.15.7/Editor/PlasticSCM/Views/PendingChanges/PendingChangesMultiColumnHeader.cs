using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui.WorkspaceWindow.PendingChanges;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal class PendingChangesMultiColumnHeader : MultiColumnHeader
    {
        internal PendingChangesMultiColumnHeader(
            PendingChangesTreeView treeView,
            MultiColumnHeaderState headerState, 
            UnityPendingChangesTree tree)
            : base(headerState)
        {
            mPendingChangesTreeView = treeView;
            mPendingChangesTree = tree;
        }

        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            if (columnIndex == 0)
            {
                bool checkAllWasMixed = IsMixedCheckedState();
                bool checkAllWasTrue = IsAllCheckedState();

                var checkRect = new Rect(
                    headerRect.x + UnityConstants.TREEVIEW_BASE_INDENT,
                    headerRect.y + 3 + UnityConstants.TREEVIEW_HEADER_CHECKBOX_Y_OFFSET,  // Custom offset because header labels are not centered
                    UnityConstants.TREEVIEW_CHECKBOX_SIZE,
                    headerRect.height);
                
                EditorGUI.showMixedValue = checkAllWasMixed;
                bool checkAllIsTrue = EditorGUI.Toggle(
                    checkRect,
                    checkAllWasTrue);
                EditorGUI.showMixedValue = false;

                if (checkAllWasTrue != checkAllIsTrue)
                {
                    UpdateCheckedState(checkAllIsTrue);
                    ((PendingChangesTreeHeaderState)state).UpdateItemColumnHeader(mPendingChangesTreeView);
                }

                headerRect.x = checkRect.xMax;
                headerRect.xMax = column.width;
            }
            base.ColumnHeaderGUI(column, headerRect, columnIndex);
        }

        internal bool IsAllCheckedState()
        {
            List<PendingChangeCategory> categories = mPendingChangesTree.GetNodes();

            if (categories == null)
                return false;

            foreach (PendingChangeCategory category in categories)
            {
                if (!category.IsChecked())
                    return false;
            }

            return true;
        }

        protected bool IsMixedCheckedState()
        {
            List<PendingChangeCategory> categories = mPendingChangesTree.GetNodes();

            if (categories == null)
                return false;

            if (IsAllCheckedState())
                return false;

            foreach (PendingChangeCategory category in categories)
            {
                if (category.GetCheckedChangesCount() > 0)
                    return true;
            }

            return false;
        }

        internal void UpdateCheckedState(bool isChecked)
        {
            List<PendingChangeCategory> categories = mPendingChangesTree.GetNodes();

            if (categories == null)
                return;

            foreach (PendingChangeCategory category in categories)
                category.UpdateCheckedState(isChecked);
        }

        readonly PendingChangesTreeView mPendingChangesTreeView;
        protected UnityPendingChangesTree mPendingChangesTree;
    }
}