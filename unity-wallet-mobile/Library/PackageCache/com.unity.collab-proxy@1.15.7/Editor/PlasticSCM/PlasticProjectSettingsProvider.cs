using Codice.Client.Commands;
using Codice.Client.Common.FsNodeReaders;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.Utils;
using PlasticGui;
using PlasticGui.WorkspaceWindow.PendingChanges;
using Unity.PlasticSCM.Editor.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.PlasticSCM.Editor
{
    class PlasticProjectSettingsProvider : SettingsProvider
    {
        public PlasticProjectSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        /// <summary>
        /// When initialized
        /// </summary>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            IAutoRefreshView autoRefreshView = GetPendingChangesView();

            if (autoRefreshView != null)
                autoRefreshView.DisableAutoRefresh();

            // Check if FSWatcher should be enabled
            WorkspaceInfo workspaceInfo = FindWorkspace.InfoForApplicationPath(
                        Application.dataPath,
                        PlasticWindow.PlasticApi);
            CheckFsWatcher(workspaceInfo);

            mInitialOptions = new PendingChangesOptions();
            mInitialOptions.LoadPendingChangesOptions();
            SetOptions(mInitialOptions);
        }

        public override void OnDeactivate()
        {
            if (mInitialOptions == null)
                return;

            bool isDialogueDirty = false;
            try
            {
                PendingChangesOptions currentOptions = GetOptions();
                isDialogueDirty = IsDirty(currentOptions);
                if (!isDialogueDirty)
                    return;

                currentOptions.SavePreferences();
            }
            finally
            {
                IAutoRefreshView autoRefreshView = GetPendingChangesView();

                if (autoRefreshView != null)
                {

                    autoRefreshView.EnableAutoRefresh();

                    if (isDialogueDirty)
                        autoRefreshView.ForceRefresh();
                }
            }
        }

        /// <summary>
        /// Called every frame for the Projects Setting window/Plastic SCM category
        /// </summary>
        public override void OnGUI(string searchContext)
        {
            EditorGUIUtility.labelWidth = 395;

            DoGeneralSettings();

            DoFileChangeSettings();

            DoFileVisibililySettings();

            DoFileDetectionSetings();

            DoFsWatcherMessage(mFSWatcherEnabled);
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (CollabPlugin.IsEnabled())
                return null;

            if (!FindWorkspace.HasWorkspace(Application.dataPath))
                return null;

            PlasticProjectSettingsProvider provider = new PlasticProjectSettingsProvider(
                UnityConstants.PROJECT_SETTINGS_MENU_TITLE, SettingsScope.Project)
                    {
                        keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
                    };
            return provider;
        }

        void DoGeneralSettings()
        {
            EditorGUILayout.Space(5);
            GUILayout.Label(
                PlasticLocalization.GetString(
                        PlasticLocalization.Name.ProjectSettingsGeneral),
                EditorStyles.boldLabel);
            EditorGUILayout.Space(1);

            mShowCheckouts = EditorGUILayout.Toggle(Styles.showCheckouts, mShowCheckouts);
            mAutoRefresh = EditorGUILayout.Toggle(Styles.autoRefresh, mAutoRefresh);
        }

        void DoFileChangeSettings()
        {
            EditorGUILayout.Space(5);
            GUILayout.Label(
                PlasticLocalization.GetString(
                        PlasticLocalization.Name.ProjectSettingsFileChange),
                EditorStyles.boldLabel);
            EditorGUILayout.Space(1);

            mShowChangedFiles = EditorGUILayout.Toggle(Styles.showChangedFiles, mShowChangedFiles);
            mCheckFileContent = EditorGUILayout.Toggle(Styles.checkFileContent, mCheckFileContent);
        }

        void DoFileVisibililySettings()
        {
            EditorGUILayout.Space(5);
            GUILayout.Label(
                PlasticLocalization.GetString(
                        PlasticLocalization.Name.ProjectSettingsFileVisibility),
                EditorStyles.boldLabel);
            EditorGUILayout.Space(1);

            mShowPrivateFields = EditorGUILayout.Toggle(Styles.showPrivateFields, mShowPrivateFields);
            mShowIgnoredFiles = EditorGUILayout.Toggle(Styles.showIgnoredFields, mShowIgnoredFiles);
            mShowHiddenFiles = EditorGUILayout.Toggle(Styles.showHiddenFields, mShowHiddenFiles);
            mShowDeletedFiles = EditorGUILayout.Toggle(Styles.showDeletedFilesDirs, mShowDeletedFiles);
        }

        void DoFileDetectionSetings()
        {
            EditorGUILayout.Space(5);
            GUILayout.Label(
                PlasticLocalization.GetString(
                        PlasticLocalization.Name.ProjectSettingsMoveAndRename),
                EditorStyles.boldLabel);
            EditorGUILayout.Space(1);

            mShowMovedFiles = EditorGUILayout.Toggle(Styles.showMovedFiles, mShowMovedFiles);
            mMatchBinarySameExtension = EditorGUILayout.Toggle(Styles.matchBinarySameExtension, mMatchBinarySameExtension);
            mMatchTextSameExtension = EditorGUILayout.Toggle(Styles.matchTextSameExtension, mMatchTextSameExtension);
            mSimilarityPercent = EditorGUILayout.IntSlider(Styles.similarityPercent, mSimilarityPercent, 0, 100);
        }

        /*** FS Watcher Message ***/
        void DoFsWatcherMessage(bool isEnabled)
        {
            EditorGUILayout.Space(45);

            GUIContent message = new GUIContent(
                isEnabled ?
                    GetFsWatcherEnabledMessage() :
                    GetFsWatcherDisabledMessage(),
                isEnabled ?
                    Images.GetInfoIcon() :
                    Images.GetWarnIcon());

            GUILayout.Label(message, UnityStyles.Dialog.Toggle, GUILayout.Height(32));
            GUILayout.Space(-10);

            string formattedExplanation = isEnabled ?
                GetFsWatcherEnabledExplanation() :
                GetFsWatcherDisabledExplanation();

            string helpLink = GetHelpLink();

            DrawTextBlockWithEndLink.For(
                helpLink, formattedExplanation, UnityStyles.Paragraph);
        }

        void CheckFsWatcher(WorkspaceInfo wkInfo)
        {
            bool isFileSystemWatcherEnabled = false;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
                waiter.Execute(
                    /*threadOperationDelegate*/
                    delegate
                    {
                        isFileSystemWatcherEnabled =
                        IsFileSystemWatcherEnabled(wkInfo);
                    },
                    /*afterOperationDelegate*/
                    delegate
                    {
                        if (waiter.Exception != null)
                            return;
                        mFSWatcherEnabled = isFileSystemWatcherEnabled;
                    });
        }

        PendingChangesOptions GetOptions()
        {
            WorkspaceStatusOptions resultWkStatusOptions =
                WorkspaceStatusOptions.None;

            if (mShowCheckouts)
            {
                resultWkStatusOptions |= WorkspaceStatusOptions.FindCheckouts;
                resultWkStatusOptions |= WorkspaceStatusOptions.FindReplaced;
                resultWkStatusOptions |= WorkspaceStatusOptions.FindCopied;
            }

            if (mShowChangedFiles)
                resultWkStatusOptions |= WorkspaceStatusOptions.FindChanged;
            if (mShowPrivateFields)
                resultWkStatusOptions |= WorkspaceStatusOptions.FindPrivates;
            if (mShowIgnoredFiles)
                resultWkStatusOptions |= WorkspaceStatusOptions.ShowIgnored;
            if (mShowHiddenFiles)
                resultWkStatusOptions |= WorkspaceStatusOptions.ShowHiddenChanges;
            if (mShowDeletedFiles)
                resultWkStatusOptions |= WorkspaceStatusOptions.FindLocallyDeleted;
            if (mShowMovedFiles)
                resultWkStatusOptions |= WorkspaceStatusOptions.CalculateLocalMoves;

            MovedMatchingOptions matchingOptions = new MovedMatchingOptions();
            matchingOptions.AllowedChangesPerUnit =
                (100 - mSimilarityPercent) / 100f;
            matchingOptions.bBinMatchingOnlySameExtension =
                mMatchBinarySameExtension;
            matchingOptions.bTxtMatchingOnlySameExtension =
                mMatchTextSameExtension;

            return new PendingChangesOptions(
                resultWkStatusOptions, matchingOptions,
                mAutoRefresh, false,
                mCheckFileContent, false);
        }

        void SetOptions(PendingChangesOptions options)
        {
            mShowCheckouts = IsEnabled(
                WorkspaceStatusOptions.FindCheckouts, options.WorkspaceStatusOptions);
            mAutoRefresh = options.AutoRefresh;

            mShowChangedFiles = IsEnabled(
                WorkspaceStatusOptions.FindChanged, options.WorkspaceStatusOptions);
            mCheckFileContent = options.CheckFileContentForChanged;

            mShowPrivateFields = IsEnabled(
                WorkspaceStatusOptions.FindPrivates, options.WorkspaceStatusOptions);
            mShowIgnoredFiles = IsEnabled(
                WorkspaceStatusOptions.ShowIgnored, options.WorkspaceStatusOptions);
            mShowHiddenFiles = IsEnabled(
                WorkspaceStatusOptions.ShowHiddenChanges, options.WorkspaceStatusOptions);
            mShowDeletedFiles = IsEnabled(
                WorkspaceStatusOptions.FindLocallyDeleted, options.WorkspaceStatusOptions);

            mShowMovedFiles = IsEnabled(
                WorkspaceStatusOptions.CalculateLocalMoves, options.WorkspaceStatusOptions);
            mMatchBinarySameExtension =
                options.MovedMatchingOptions.bBinMatchingOnlySameExtension;
            mMatchTextSameExtension =
                options.MovedMatchingOptions.bTxtMatchingOnlySameExtension;
            mSimilarityPercent = (int)((1 - options.MovedMatchingOptions.AllowedChangesPerUnit) * 100f);
        }

        bool IsDirty(PendingChangesOptions currentOptions)
        {
            return !mInitialOptions.AreSameOptions(currentOptions);
        }

        static string GetFsWatcherEnabledMessage()
        {
            if (PlatformIdentifier.IsWindows() || PlatformIdentifier.IsMac())
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.PendingChangesFilesystemWatcherEnabled);

            return PlasticLocalization.GetString(
                PlasticLocalization.Name.PendingChangesINotifyEnabled);
        }

        static string GetFsWatcherDisabledMessage()
        {
            if (PlatformIdentifier.IsWindows() || PlatformIdentifier.IsMac())
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.PendingChangesFilesystemWatcherDisabled);

            return PlasticLocalization.GetString(
                PlasticLocalization.Name.PendingChangesINotifyDisabled);
        }

        static string GetFsWatcherEnabledExplanation()
        {
            if (PlatformIdentifier.IsWindows() || PlatformIdentifier.IsMac())
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.PendingChangesFilesystemWatcherEnabledExplanation);

            return PlasticLocalization.GetString(
            PlasticLocalization.Name.PendingChangesINotifyEnabledExplanation);
        }

        static string GetFsWatcherDisabledExplanation()
        {
            if (PlatformIdentifier.IsWindows() || PlatformIdentifier.IsMac())
            {
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.PendingChangesFilesystemWatcherDisabledExplanation)
                    .Replace("[[HELP_URL|{0}]]", "{0}");
            }

            return PlasticLocalization.GetString(
                PlasticLocalization.Name.PendingChangesINotifyDisabledExplanation);
        }

        static string GetHelpLink()
        {
            if (PlatformIdentifier.IsWindows() || PlatformIdentifier.IsMac())
                return FS_WATCHER_HELP_URL;

            return INOTIFY_HELP_URL;
        }

        static bool IsFileSystemWatcherEnabled(WorkspaceInfo wkInfo)
        {
            return WorkspaceWatcherFsNodeReadersCache.Get().
                IsWatcherEnabled(wkInfo);
        }

        static bool IsEnabled(WorkspaceStatusOptions option, WorkspaceStatusOptions options)
        {
            return (options & option) == option;
        }

        static IAutoRefreshView GetPendingChangesView()
        {
            if (!EditorWindow.HasOpenInstances<PlasticWindow>())
                return null;

            PlasticWindow window = EditorWindow.
                GetWindow<PlasticWindow>(null, false);

            return window.GetPendingChangesView();
        }

        internal interface IAutoRefreshView
        {
            void DisableAutoRefresh();
            void EnableAutoRefresh();
            void ForceRefresh();
        }

        /// <summary>
        /// UI styles and label content
        /// </summary>
        class Styles
        {
            internal static GUIContent showCheckouts =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowCheckouts),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowCheckoutsExplanation));
            internal static GUIContent autoRefresh =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesAutoRefresh),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesAutoRefreshExplanation));

            internal static GUIContent showChangedFiles =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesFindChanged),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesFindChangedExplanation));
            internal static GUIContent checkFileContent =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesCheckFileContent),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesCheckFileContentExplanation));

            internal static GUIContent showPrivateFields =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowPrivateFiles),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowPrivateFilesExplanation));
            internal static GUIContent showIgnoredFields =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowIgnoredFiles),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowIgnoredFilesExplanation));
            internal static GUIContent showHiddenFields =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowHiddenFiles),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowHiddenFilesExplanation));
            internal static GUIContent showDeletedFilesDirs =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowDeletedFiles),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesShowDeletedFilesExplanation));

            internal static GUIContent showMovedFiles =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesFindMovedFiles),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesFindMovedFilesExplanation));
            internal static GUIContent matchBinarySameExtension =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesMatchBinarySameExtension),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesMatchBinarySameExtensionExplanation));
            internal static GUIContent matchTextSameExtension =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesMatchTextSameExtension),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesMatchTextSameExtensionExplanation));
            internal static GUIContent similarityPercent =
                new GUIContent(PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesSimilarityPercentage),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.PendingChangesSimilarityPercentageExplanation));
        }

        PendingChangesOptions mInitialOptions;

        bool mShowCheckouts;
        bool mAutoRefresh;
        bool mFSWatcherEnabled;

        bool mShowChangedFiles;
        bool mCheckFileContent;

        bool mShowPrivateFields;
        bool mShowIgnoredFiles;
        bool mShowHiddenFiles;
        bool mShowDeletedFiles;

        bool mShowMovedFiles;
        bool mMatchBinarySameExtension;
        bool mMatchTextSameExtension;
        int mSimilarityPercent;

        const string FS_WATCHER_HELP_URL = "https://plasticscm.com/download/help/support";
        const string INOTIFY_HELP_URL = "https://plasticscm.com/download/help/inotify";
    }
}
