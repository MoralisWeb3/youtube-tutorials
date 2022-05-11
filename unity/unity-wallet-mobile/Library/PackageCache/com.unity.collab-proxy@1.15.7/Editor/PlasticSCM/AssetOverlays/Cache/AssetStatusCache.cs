using System;
using System.IO;

using UnityEditor;

using Codice.CM.Common;

namespace Unity.PlasticSCM.Editor.AssetsOverlays.Cache
{
    internal interface IAssetStatusCache
    {
        AssetStatus GetStatusForPath(string fullPath);
        AssetStatus GetStatusForGuid(string guid);
        LockStatusData GetLockStatusData(string guid);
        LockStatusData GetLockStatusDataForPath(string path);
        void Clear();
    }

    internal class AssetStatusCache : IAssetStatusCache
    {
        internal AssetStatusCache(
            WorkspaceInfo wkInfo,
            bool isGluonMode,
            Action repaintProjectWindow)
        {
            mLocalStatusCache = new LocalStatusCache(wkInfo);

            mRemoteStatusCache = new RemoteStatusCache(
                wkInfo,
                isGluonMode,
                repaintProjectWindow);

            mLockStatusCache = new LockStatusCache(
                wkInfo,
                repaintProjectWindow);
        }

        AssetStatus IAssetStatusCache.GetStatusForPath(string fullPath)
        {
            AssetStatus localStatus = mLocalStatusCache.GetStatus(fullPath);

            if (!ClassifyAssetStatus.IsControlled(localStatus))
                return localStatus;

            AssetStatus remoteStatus = mRemoteStatusCache.GetStatus(fullPath);

            AssetStatus lockStatus = mLockStatusCache.GetStatus(fullPath);

            return localStatus | remoteStatus | lockStatus;
        }

        AssetStatus IAssetStatusCache.GetStatusForGuid(string guid)
        {
            string fullPath = GetAssetPath(guid);

            if (string.IsNullOrEmpty(fullPath))
                return AssetStatus.None;

            return ((IAssetStatusCache)this).GetStatusForPath(fullPath);
        }

        LockStatusData IAssetStatusCache.GetLockStatusDataForPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            return mLockStatusCache.GetLockStatusData(path);
        }

        LockStatusData IAssetStatusCache.GetLockStatusData(string guid)
        {
            string fullPath = GetAssetPath(guid);

            return ((IAssetStatusCache)this).GetLockStatusDataForPath(fullPath);
        }

        void IAssetStatusCache.Clear()
        {
            mLocalStatusCache.Clear();
            mRemoteStatusCache.Clear();
            mLockStatusCache.Clear();
        }

        static string GetAssetPath(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(assetPath))
                return null;

            return Path.GetFullPath(assetPath);
        }

        readonly LocalStatusCache mLocalStatusCache;
        readonly RemoteStatusCache mRemoteStatusCache;
        readonly LockStatusCache mLockStatusCache;
    }
}
