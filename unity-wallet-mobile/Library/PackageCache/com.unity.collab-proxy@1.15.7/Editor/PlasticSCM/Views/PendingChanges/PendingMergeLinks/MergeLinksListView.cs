using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;

using Codice.Client.Commands;
using Codice.Client.Common;
using Codice.CM.Common;
using Codice.CM.Common.Merge;
using PlasticGui.WorkspaceWindow.PendingChanges;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges.PendingMergeLinks
{
    internal class MergeLinksListView : TreeView
    {
        internal float DesiredHeight
        {
            get
            {
                return rowHeight * (mMergeLinks.Count + 1);
            }
        }

        internal MergeLinksListView()
            : base(new TreeViewState())
        {
            rowHeight = UnityConstants.TREEVIEW_ROW_HEIGHT;
            showAlternatingRowBackgrounds = true;
        }

        public override IList<TreeViewItem> GetRows()
        {
            return mRows;
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1, string.Empty);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem rootItem)
        {
            RegenerateRows(mMergeLinks, rootItem, mRows);
            return mRows;
        }

        internal void BuildModel(
            IDictionary<MountPoint, IList<PendingMergeLink>> pendingMergeLinks)
        {
            mMergeLinks = BuildMountPendingMergeLink(pendingMergeLinks);
        }

        static void RegenerateRows(
            List<MountPendingMergeLink> mergeLinks,
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            ClearRows(rootItem, rows);

            if (mergeLinks.Count == 0)
                return;

            for (int i = 0; i < mergeLinks.Count; i++)
            {
                MergeLinkListViewItem mergeLinkListViewItem =
                    new MergeLinkListViewItem(i + 1, mergeLinks[i]);

                rootItem.AddChild(mergeLinkListViewItem);
                rows.Add(mergeLinkListViewItem);
            }
        }

        static void ClearRows(
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            if (rootItem.hasChildren)
                rootItem.children.Clear();

            rows.Clear();
        }

        static List<MountPendingMergeLink> BuildMountPendingMergeLink(
            IDictionary<MountPoint, IList<PendingMergeLink>> pendingMergeLinks)
        {
            List<MountPendingMergeLink> result = new List<MountPendingMergeLink>();

            foreach (KeyValuePair<MountPoint, IList<PendingMergeLink>> mountLink
                in pendingMergeLinks)
            {
                result.AddRange(BuildMountPendingMergeLinks(
                    mountLink.Key, mountLink.Value));
            }

            return result;
        }

        static List<MountPendingMergeLink> BuildMountPendingMergeLinks(
            MountPoint mount, IList<PendingMergeLink> links)
        {
            List<MountPendingMergeLink> result = new List<MountPendingMergeLink>();

            RepositoryInfo repInfo = RepositorySpecResolverProvider.Get().
                GetRepInfo(mount.RepSpec);

            foreach (PendingMergeLink link in links)
                result.Add(new MountPendingMergeLink(repInfo.GetRepSpec(), link));

            return result;
        }

        List<TreeViewItem> mRows = new List<TreeViewItem>();

        List<MountPendingMergeLink> mMergeLinks = new List<MountPendingMergeLink>();
    }
}
