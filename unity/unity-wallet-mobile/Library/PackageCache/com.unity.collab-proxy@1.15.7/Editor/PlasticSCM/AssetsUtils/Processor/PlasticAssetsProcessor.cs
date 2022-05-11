using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.Config;
using Codice.Client.Commands;
using Codice.Client.Commands.WkTree;
using Codice.LogWrapper;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Views.IncomingChanges;
using Unity.PlasticSCM.Editor.Views.PendingChanges;

namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    internal class PlasticAssetsProcessor
    {
        internal static void RegisterPlasticAPI(IPlasticAPI plasticAPI)
        {
            mPlasticAPI = plasticAPI;
        }

        internal static void RegisterPendingChangesView(
            PendingChangesTab pendingChangesTab)
        {
            mPendingChangesTab = pendingChangesTab;
        }

        internal static void RegisterIncomingChangesView(
            IIncomingChangesTab incomingChangesTab)
        {
            mIncomingChangesTab = incomingChangesTab;
        }

        internal static void UnRegisterViews()
        {
            mPendingChangesTab = null;
            mIncomingChangesTab = null;
        }

        internal static void AddToSourceControl(string[] paths)
        {
            foreach (string path in paths)
                mLog.DebugFormat("AddToSourceControl: {0}", path);

            try
            {
                AddIfNotControlled(
                    paths,
                    mPlasticAPI);
            }
            catch (Exception ex)
            {
                LogAddException(ex);
            }
            finally
            {
                mCooldownAutorefreshAction.Ping();
            }
        }

        internal static void DeleteFromSourceControl(string path)
        {
            mLog.DebugFormat("DeleteFromSourceControl: {0}", path);

            try
            {
                string fullPath = Path.GetFullPath(path);

                DeleteIfControlled(
                    fullPath,
                    mPlasticAPI);

                DeleteIfControlled(
                    MetaPath.GetMetaPath(fullPath),
                    mPlasticAPI);
            }
            catch (Exception ex)
            {
                LogDeleteException(path, ex);
            }
            finally
            {
                mCooldownAutorefreshAction.Ping();
            }
        }

        internal static void MoveOnSourceControl(string srcPath, string dstPath)
        {
            mLog.DebugFormat("MoveOnSourceControl: {0} to {1}", srcPath, dstPath);

            try
            {
                string srcFullPath = Path.GetFullPath(srcPath);
                string dstFullPath = Path.GetFullPath(dstPath);

                MoveIfControlled(
                    srcFullPath,
                    dstFullPath,
                    mPlasticAPI);

                MoveIfControlled(
                    MetaPath.GetMetaPath(srcFullPath),
                    MetaPath.GetMetaPath(dstFullPath),
                    mPlasticAPI);
            }
            catch (Exception ex)
            {
                LogMoveException(srcPath, dstPath, ex);
            }
            finally
            {
                mCooldownAutorefreshAction.Ping();
            }
        }

        internal static void CheckoutOnSourceControl(string[] paths)
        {
            foreach (string path in paths)
                mLog.DebugFormat("CheckoutOnSourceControl: {0}", path);

            try
            {
                CheckoutIfControlledAndChanged(paths, mPlasticAPI);
            }
            catch (Exception ex)
            {
                LogCheckoutException(ex);
            }
            finally
            {
                mCooldownAutorefreshAction.Ping();
            }
        }

        static void AddIfNotControlled(
            string[] paths,
            IPlasticAPI api)
        {
            List<string> fullPaths = new List<string>();

            IgnoredFilesFilter ignoredFilter = new IgnoredFilesFilter(
                GlobalConfig.Instance);

            foreach (string path in paths)
            {
                string fullPath = Path.GetFullPath(path);
                string fullPathMeta = MetaPath.GetMetaPath(fullPath);

                if (api.GetWorkspaceFromPath(fullPath) == null)
                    return;

                if (api.GetWorkspaceTreeNode(fullPath) == null &&
                    !ignoredFilter.IsIgnored(fullPath))
                    fullPaths.Add(fullPath);

                if (File.Exists(fullPathMeta) &&
                    api.GetWorkspaceTreeNode(fullPathMeta) == null &&
                    !ignoredFilter.IsIgnored(fullPath))
                    fullPaths.Add(fullPathMeta);
            }

            if (fullPaths.Count == 0)
                return;

            IList checkouts;
            api.Add(
                fullPaths.ToArray(),
                GetDefaultAddOptions(),
                out checkouts);
        }

        static void DeleteIfControlled(
            string fullPath,
            IPlasticAPI api)
        {
            if (api.GetWorkspaceTreeNode(fullPath) == null)
                return;

            api.DeleteControlled(
                fullPath,
                DeleteModifiers.None);
        }

        static void MoveIfControlled(
            string srcFullPath,
            string dstFullPath,
            IPlasticAPI api)
        {
            if (api.GetWorkspaceTreeNode(srcFullPath) == null)
                return;

            api.Move(
                srcFullPath,
                dstFullPath,
                MoveModifiers.None);
        }

        static void CheckoutIfControlledAndChanged(string[] paths, IPlasticAPI api)
        {
            List<string> fullPaths = new List<string>();

            foreach (string path in paths)
            {
                string fullPath = Path.GetFullPath(path);
                string fullPathMeta = MetaPath.GetMetaPath(fullPath);

                WorkspaceTreeNode node =
                    api.GetWorkspaceTreeNode(fullPath);
                WorkspaceTreeNode nodeMeta =
                    api.GetWorkspaceTreeNode(fullPathMeta);

                if (node != null && ChangedFileChecker.IsChanged(
                    node.LocalInfo, fullPath, false))
                    fullPaths.Add(fullPath);

                if (nodeMeta != null && ChangedFileChecker.IsChanged(
                    nodeMeta.LocalInfo, fullPathMeta, false))
                    fullPaths.Add(fullPathMeta);
            }

            if (fullPaths.Count == 0)
                return;

            api.Checkout(
                fullPaths.ToArray(),
                CheckoutModifiers.None);
        }

        static void PerformAutoRefresh()
        {
            AutoRefresh.PendingChangesView(
                mPendingChangesTab);

            AutoRefresh.IncomingChangesView(
                mIncomingChangesTab);
        }

        static void LogAddException(Exception ex)
        {
            UnityEngine.Debug.LogWarning(
                string.Format(PlasticLocalization.GetString(PlasticLocalization.Name.CannotAddVersionControl),
                ex.Message));

            LogException(ex);
        }

        static void LogDeleteException(string path, Exception ex)
        {
            UnityEngine.Debug.LogWarning(
                string.Format(PlasticLocalization.GetString(PlasticLocalization.Name.CannotDeleteVersionControl),
                path, ex.Message));

            LogException(ex);
        }

        static void LogMoveException(string srcPath, string dstPath, Exception ex)
        {
            UnityEngine.Debug.LogWarning(
                string.Format(PlasticLocalization.GetString(PlasticLocalization.Name.CannotMoveVersionControl),
                srcPath, dstPath, ex.Message));

            LogException(ex);
        }

        static void LogCheckoutException(Exception ex)
        {
            UnityEngine.Debug.LogWarning(
                string.Format(PlasticLocalization.GetString(PlasticLocalization.Name.CannotCheckoutVersionControl),
                ex.Message));

            LogException(ex);
        }

        static void LogException(Exception ex)
        {
            mLog.WarnFormat("Message: {0}", ex.Message);

            mLog.DebugFormat(
                "StackTrace:{0}{1}",
                Environment.NewLine, ex.StackTrace);
        }

        static AddOptions GetDefaultAddOptions()
        {
            AddOptions options = new AddOptions();
            options.AddPrivateParents = true;
            options.NeedCheckPlatformPath = true;
            return options;
        }

        static volatile IPlasticAPI mPlasticAPI;

        static internal PendingChangesTab mPendingChangesTab;
        static IIncomingChangesTab mIncomingChangesTab;

        static CooldownWindowDelayer mCooldownAutorefreshAction = new CooldownWindowDelayer(
            PerformAutoRefresh, UnityConstants.AUTO_REFRESH_DELAYED_INTERVAL);

        static readonly ILog mLog = LogManager.GetLogger("PlasticAssetsProcessor");
    }
}