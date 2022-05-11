using System.Collections.Generic;
using System.IO;
using System.Linq;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using PlasticGui.WorkspaceWindow.Diff;
using PlasticGui.WorkspaceWindow.PendingChanges;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal static class PendingChangesSelection
    {
        internal static void SelectChanges(
            PendingChangesTreeView treeView,
            List<ChangeInfo> changesToSelect)
        {
            if (changesToSelect == null || changesToSelect.Count == 0)
            {
                treeView.SelectFirstPendingChangeOnTree();
                return;
            }

            treeView.SelectPreviouslySelectedPendingChanges(changesToSelect);

            if (treeView.HasSelection())
                return;

            treeView.SelectFirstPendingChangeOnTree();
        }

        internal static List<string> GetSelectedPathsWithoutMeta(
            PendingChangesTreeView treeView)
        {
            return treeView.GetSelectedChanges(false)
                .Select(change => change.GetFullPath()).ToList();
        }

        internal static List<string> GetSelectedPaths(
            PendingChangesTreeView treeView)
        {
            return treeView.GetSelectedChanges(true)
                .Select(change => change.GetFullPath()).ToList();
        }

        internal static List<string> GetSelectedMetaPaths(
            PendingChangesTreeView treeView)
        {
            List<ChangeInfo> selectedChanges = PendingChangesSelection
                .GetSelectedChanges(treeView);

            List<string> result = new List<string>();

            foreach (ChangeInfo change in selectedChanges)
            {
                string path = change.GetFullPath();

                if (!MetaPath.IsMetaPath(path))
                    continue;

                result.Add(path);
            }

            return result;
        }

        internal static List<ChangeInfo> GetAllChanges(
            PendingChangesTreeView treeView)
        {
            return treeView.GetAllChanges();
        }

        internal static List<ChangeInfo> GetChangesToFocus(
            PendingChangesTreeView treeView)
        {
            List<ChangeInfo> selectedChanges = treeView.GetSelectedChanges(true);

            if (selectedChanges.Count == 0)
                return selectedChanges;

            List<ChangeInfo> changesToFocus =
                selectedChanges.Where(change => !IsAddedFile(change)).ToList();

            if (changesToFocus.Count() == 0)
            {
                ChangeInfo nearestAddedChange = treeView.GetNearestAddedChange();
                if (nearestAddedChange != null)
                    changesToFocus.Add(nearestAddedChange);
            }

            return changesToFocus;
        }

        internal static SelectedChangesGroupInfo GetSelectedChangesGroupInfo(
            PendingChangesTreeView treeView)
        {
            SelectedChangesGroupInfo result = treeView.GetSelectedChangesGroupInfo();
            result.IsApplicableDiffWorkspaceContent = IsApplicableDiffWorkspaceContent(treeView);
            return result;
        }

        internal static List<ChangeInfo> GetSelectedChanges(
            PendingChangesTreeView treeView)
        {
            return treeView.GetSelectedChanges(true);
        }

        internal static ChangeInfo GetSelectedChange(
            PendingChangesTreeView treeView)
        {
            return treeView.GetSelectedRow();
        }

        static bool IsApplicableDiffWorkspaceContent(
            PendingChangesTreeView treeView)
        {
            ChangeInfo selectedRow = GetSelectedChange(treeView);

            if (selectedRow == null)
                return false;

            return DiffOperation.IsApplicableDiffWorkspaceContent(selectedRow);
        }

        static bool IsAddedFile(ChangeInfo change)
        {
            return ChangeTypesClassifier.IsInAddedCategory(change.ChangeTypes)
                && !(Directory.Exists(change.Path) || File.Exists(change.Path));
        }
    }
}
