using UnityEditor.IMGUI.Controls;

using PlasticGui.WorkspaceWindow.Diff;

namespace Unity.PlasticSCM.Editor.Views.Diff
{
    internal class MergeCategoryTreeViewItem : TreeViewItem
    {
        internal MergeCategory Category { get; private set; }

        internal MergeCategoryTreeViewItem(
            int id, int depth, MergeCategory category)
            : base(id, depth, category.GetHeaderText())
        {
            Category = category;
        }
    }
}
