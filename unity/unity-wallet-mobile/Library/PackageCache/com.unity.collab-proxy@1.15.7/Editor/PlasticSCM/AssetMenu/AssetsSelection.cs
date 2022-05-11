using System.Collections.Generic;
using System.IO;

using UnityEditor.VersionControl;

using PlasticGui.WorkspaceWindow.Items;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;

namespace Unity.PlasticSCM.Editor.AssetMenu
{
    internal static class AssetsSelection
    {
        internal static Asset GetSelectedAsset(AssetList assetList)
        {
            if (assetList.Count == 0)
                return null;

            return assetList[0];
        }

        internal static string GetSelectedPath(AssetList assetList)
        {
            if (assetList.Count == 0)
                return null;

            return Path.GetFullPath(assetList[0].path);
        }

        internal static List<string> GetSelectedPaths(AssetList selectedAssets)
        {
            List<string> result = new List<string>();

            foreach (Asset asset in selectedAssets)
            {
                string fullPath = Path.GetFullPath(asset.path);
                result.Add(fullPath);
            }

            return result;
        }

        internal static SelectedPathsGroupInfo GetSelectedPathsGroupInfo(
            AssetList selectedAssets,
            IAssetStatusCache assetStatusCache)
        {
            SelectedPathsGroupInfo result = new SelectedPathsGroupInfo();

            if (selectedAssets.Count == 0)
                return result;

            result.SelectedCount = selectedAssets.Count;

            result.IsRootSelected = false;
            result.IsCheckedoutEverySelected = true;
            result.IsDirectoryEverySelected = true;
            result.IsCheckedinEverySelected = true;
            result.IsChangedEverySelected = true;

            Asset firstAsset = selectedAssets[0];
            string firstAssetName = GetAssetName(firstAsset);
            AssetStatus firstStatus = GetAssetStatus(
                firstAsset,
                assetStatusCache);

            result.FirstIsControlled = ClassifyAssetStatus.IsControlled(firstStatus);
            result.FirstIsDirectory = firstAsset.isFolder;

            result.FilterInfo.CommonName = firstAssetName;
            result.FilterInfo.CommonExtension = Path.GetExtension(firstAssetName);
            result.FilterInfo.CommonFullPath = firstAsset.assetPath;

            foreach (Asset asset in selectedAssets)
            {
                string assetName = GetAssetName(asset);
                AssetStatus status = GetAssetStatus(
                    asset, 
                    assetStatusCache);

                result.IsCheckedoutEverySelected &= ClassifyAssetStatus.IsCheckedOut(status);
                result.IsDirectoryEverySelected &= asset.isFolder;
                result.IsCheckedinEverySelected &= false; // TODO: not implemented yet
                result.IsChangedEverySelected &= false; // TODO: not implemented yet

                result.IsAnyDirectorySelected |= asset.isFolder;
                result.IsAnyPrivateSelected |= ClassifyAssetStatus.IsPrivate(status);

                result.FilterInfo.IsAnyIgnoredSelected |= ClassifyAssetStatus.IsIgnored(status);
                result.FilterInfo.IsAnyHiddenChangedSelected |= ClassifyAssetStatus.IsHiddenChanged(status);

                if (result.SelectedCount == 1)
                    continue;

                if (result.FilterInfo.CommonName != assetName)
                    result.FilterInfo.CommonName = null;

                if (result.FilterInfo.CommonExtension != Path.GetExtension(assetName))
                    result.FilterInfo.CommonExtension = null;

                if (result.FilterInfo.CommonFullPath != asset.assetPath)
                    result.FilterInfo.CommonFullPath = null;
            }

            return result;
        }

        static AssetStatus GetAssetStatus(Asset asset, IAssetStatusCache assetStatusCache)
        {
            string assetPath = Path.GetFullPath(asset.assetPath);

            if (MetaPath.IsMetaPath(assetPath))
                assetPath = MetaPath.GetPathFromMetaPath(assetPath);

            return assetStatusCache.GetStatusForPath(assetPath);
        }

        static string GetAssetName(Asset asset)
        {
            if (asset.isFolder)
                return Path.GetFileName(Path.GetDirectoryName(asset.path));

            return asset.fullName;
        }
    }
}
