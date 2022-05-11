using UnityEditor;
using UnityEngine;

using PlasticGui;
using PlasticGui.WorkspaceWindow.QueryViews.Branches;

namespace Unity.PlasticSCM.Editor.Views.Branches
{
    internal class BranchesViewMenu
    {
        internal BranchesViewMenu(
            IBranchMenuOperations branchMenuOperations)
        {
            mBranchMenuOperations = branchMenuOperations;

            BuildComponents();
        }

        internal void Popup()
        {
            GenericMenu menu = new GenericMenu();

            UpdateMenuItems(menu);

            menu.ShowAsContext();
        }

        void CreateBranchMenuItem_Click()
        {
            mBranchMenuOperations.CreateBranch();
        }

        void SwitchToBranchMenuItem_Click()
        {
            mBranchMenuOperations.SwitchToBranch();
        }

        void RenameBranchMenuItem_Click()
        {
            mBranchMenuOperations.RenameBranch();
        }

        void DeleteBranchMenuItem_Click()
        {
            mBranchMenuOperations.DeleteBranch();
        }
    
        void UpdateMenuItems(GenericMenu menu)
        {
            BranchMenuOperations operations = BranchMenuUpdater.GetAvailableMenuOperations(
                mBranchMenuOperations.GetSelectedBranchesCount());

			AddBranchMenuItem(
                mCreateBranchMenuItemContent,
                menu,
                operations,
                BranchMenuOperations.CreateBranch,
                CreateBranchMenuItem_Click);
			
            AddBranchMenuItem(
                mSwitchToBranchMenuItemContent,
                menu,
                operations,
                BranchMenuOperations.SwitchToBranch,
                SwitchToBranchMenuItem_Click);

            menu.AddSeparator("");

            AddBranchMenuItem(
                mRenameBranchMenuItemContent,
                menu,
                operations,
				BranchMenuOperations.Rename,
                RenameBranchMenuItem_Click);

            AddBranchMenuItem(
                mDeleteBranchMenuItemContent,
                menu,
                operations,
				BranchMenuOperations.Delete,
                DeleteBranchMenuItem_Click);
        }

        static void AddBranchMenuItem(
            GUIContent menuItemContent,
            GenericMenu menu,
            BranchMenuOperations operations,
            BranchMenuOperations operationsToCheck,
            GenericMenu.MenuFunction menuFunction)
        {
            if (operations.HasFlag(operationsToCheck))
            {
                menu.AddItem(
                    menuItemContent,
                    false,
                    menuFunction);
                return;
            }

            menu.AddDisabledItem(menuItemContent);
        }

        void BuildComponents()
        {
            mCreateBranchMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.BranchMenuItemCreateBranch));
            mSwitchToBranchMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.BranchMenuItemSwitchToBranch));
            mRenameBranchMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.BranchMenuItemRenameBranch));
            mDeleteBranchMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.BranchMenuItemDeleteBranch));
        }

        GUIContent mCreateBranchMenuItemContent;
        GUIContent mSwitchToBranchMenuItemContent;
        GUIContent mRenameBranchMenuItemContent;
        GUIContent mDeleteBranchMenuItemContent;

        readonly IBranchMenuOperations mBranchMenuOperations;
    }
}