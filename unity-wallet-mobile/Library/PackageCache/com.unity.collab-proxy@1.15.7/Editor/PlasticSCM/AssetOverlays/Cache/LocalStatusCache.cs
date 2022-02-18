using System.Collections.Generic;

using Codice;
using Codice.Client.BaseCommands;
using Codice.Client.Commands.WkTree;
using Codice.Client.Common;
using Codice.CM.Common;

using PlasticGui;
using PlasticGui.WorkspaceWindow;

namespace Unity.PlasticSCM.Editor.AssetsOverlays.Cache
{
    internal class LocalStatusCache
    {
        internal LocalStatusCache(WorkspaceInfo wkInfo)
        {
            mWkInfo = wkInfo;
        }

        internal AssetStatus GetStatus(string fullPath)
        {
            AssetStatus result;

            if (mStatusByPathCache.TryGetValue(fullPath, out result))
                return result;

            result = CalculateStatus(
                fullPath,
                mWkInfo.ClientPath,
                FilterManager.Get().GetIgnoredFilter(),
                FilterManager.Get().GetHiddenChangesFilter());

            mStatusByPathCache.Add(fullPath, result);

            return result;
        }

        internal void Clear()
        {
            FilterManager.Get().Reload();

            mStatusByPathCache.Clear();
        }

        static AssetStatus CalculateStatus(
            string fullPath,
            string wkPath,
            IgnoredFilesFilter ignoredFilter,
            HiddenChangesFilesFilter hiddenChangesFilter)
        {
            if (!IsOnWorkspace(fullPath, wkPath))
                return AssetStatus.None;

            WorkspaceTreeNode treeNode = PlasticGui.Plastic.API.GetWorkspaceTreeNode(fullPath);

            if (CheckWorkspaceTreeNodeStatus.IsPrivate(treeNode))
            {
                return ignoredFilter.IsIgnored(fullPath) ?
                    AssetStatus.Ignored : AssetStatus.Private;
            }

            if (CheckWorkspaceTreeNodeStatus.IsAdded(treeNode))
                return AssetStatus.Added;

            AssetStatus result = AssetStatus.Controlled;

            if (CheckWorkspaceTreeNodeStatus.IsCheckedOut(treeNode) &&
                !CheckWorkspaceTreeNodeStatus.IsDirectory(treeNode))
                result |= AssetStatus.Checkout;

            if (hiddenChangesFilter.IsHiddenChanged(fullPath))
                result |= AssetStatus.HiddenChanged;

            return result;
        }

        static bool IsOnWorkspace(string fullPath, string clientPath)
        {
            return PathHelper.IsContainedOn(fullPath, clientPath);
        }

        Dictionary<string, AssetStatus> mStatusByPathCache =
            BuildPathDictionary.ForPlatform<AssetStatus>();

        readonly WorkspaceInfo mWkInfo;
    }
}
