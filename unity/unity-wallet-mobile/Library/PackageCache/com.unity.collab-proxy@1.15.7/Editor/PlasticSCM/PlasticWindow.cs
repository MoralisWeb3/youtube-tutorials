using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Common;
using Codice.Client.Common.Connection;
using Codice.Client.Common.Encryption;
using Codice.Client.Common.EventTracking;
using Codice.Client.Common.FsNodeReaders;
using Codice.Client.Common.FsNodeReaders.Watcher;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.LogWrapper;
using CodiceApp.EventTracking;
using GluonGui;
using PlasticGui;
using PlasticGui.WebApi;
using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetUtils.Processor;
using Unity.PlasticSCM.Editor.Configuration;
using Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome;
using Unity.PlasticSCM.Editor.Inspector;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Avatar;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.Views.CreateWorkspace;
using Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs;
using Unity.PlasticSCM.Editor.Views.Welcome;

using GluonCheckIncomingChanges = PlasticGui.Gluon.WorkspaceWindow.CheckIncomingChanges;
using GluonNewIncomingChangesUpdater = PlasticGui.Gluon.WorkspaceWindow.NewIncomingChangesUpdater;
using EventTracking = PlasticGui.EventTracking.EventTracking;
using processor = Unity.PlasticSCM.Editor.AssetUtils.Processor;
using Unity.PlasticSCM.Editor.Views.PendingChanges;

namespace Unity.PlasticSCM.Editor
{
    /// <summary>
    /// The Plastic SCM window.
    /// </summary>
    public class PlasticWindow : EditorWindow,
        PlasticGui.WorkspaceWindow.CheckIncomingChanges.IAutoRefreshIncomingChangesView,
        GluonCheckIncomingChanges.IAutoRefreshIncomingChangesView,
        CreateWorkspaceView.ICreateWorkspaceListener
    {
        internal WorkspaceWindow WorkspaceWindowForTesting { get { return mWorkspaceWindow; } }
        internal ViewSwitcher ViewSwitcherForTesting { get { return mViewSwitcher; } }
        internal static IPlasticAPI PlasticApi 
        {
            get
            {
                if (mPlasticAPI == null)
                    mPlasticAPI = new PlasticAPI();
                return mPlasticAPI;
            }
        }
        internal IPlasticWebRestApi PlasticWebRestApiForTesting { get { return mPlasticWebRestApi; } }
        internal CmConnection CmConnectionForTesting { get { return CmConnection.Get(); } }

        /// <summary>
        /// Invoked when notification status changed.
        /// </summary>
        public static event Action OnNotificationUpdated = delegate { };

        /// <summary>
        /// Open the Plastic SCM window.
        /// </summary>
        public static PlasticWindow Open()
        {
            return ShowWindow.Plastic();
        }

        /// <summary>
        /// Get the Plastic window current icon.
        /// </summary>
        public static Texture GetWindowIcon()
        {
            return HasOpenInstances<PlasticWindow>() ?
                mPlasticWindowIcon :
                PlasticNotification.GetIcon(PlasticNotification.Status.None);
        }

        internal void SetupWindowTitle()
        {
            mPlasticWindowIcon = PlasticNotification.GetIcon(mNotificationStatus);

            // The titleContent icon does not update unless we also update the title text
            // Temporarily doing it by adding space characters
            string title = UnityConstants.PLASTIC_WINDOW_TITLE;
            title += String.Concat(Enumerable.Repeat(" ", (int)mNotificationStatus));

            titleContent = new GUIContent(title, mPlasticWindowIcon);
        }

        internal void SetNotificationStatus(
            PlasticNotification.Status status,
            string infoText,
            string actionText)
        {
            if (status == mNotificationStatus) return;

            mNotificationStatus = status;
            SetupWindowTitle();
            OnNotificationUpdated.Invoke();
        }

        internal void DisableCollabIfEnabledWhenLoaded()
        {
            mDisableCollabIfEnabledWhenLoaded = true;
        }

        internal void ShowPendingChanges()
        {
            ((IAssetMenuOperations)mAssetOperations).ShowPendingChanges();
        }

        internal void Add()
        {
            ((IAssetMenuOperations)mAssetOperations).Add();
        }

        internal void Checkout()
        {
            ((IAssetMenuOperations)mAssetOperations).Checkout();
        }

        internal void Checkin()
        {
            ((IAssetMenuOperations)mAssetOperations).Checkin();
        }

        internal void Undo()
        {
            ((IAssetMenuOperations)mAssetOperations).Undo();
        }

        internal void ShowDiff()
        {
            ((IAssetMenuOperations)mAssetOperations).ShowDiff();
        }

        internal void ShowHistory()
        {
            ((IAssetMenuOperations)mAssetOperations).ShowHistory();
        }

        internal void AddFilesFilterPatterns(
            FilterTypes type, 
            FilterActions action, 
            FilterOperationType operation)
        {
            ((IAssetFilesFilterPatternsMenuOperations)mAssetOperations)
                .AddFilesFilterPatterns(type, action, operation);
        }
 
        void PlasticGui.WorkspaceWindow.CheckIncomingChanges.IAutoRefreshIncomingChangesView.IfVisible()
        {
            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void GluonCheckIncomingChanges.IAutoRefreshIncomingChangesView.IfVisible()
        {
            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void CreateWorkspaceView.ICreateWorkspaceListener.OnWorkspaceCreated(
            WorkspaceInfo wkInfo, bool isGluonMode)
        {
            mWkInfo = wkInfo;
            mIsGluonMode = isGluonMode;
            mWelcomeView = null;

            if (mIsGluonMode)
                ConfigurePartialWorkspace.AsFullyChecked(mWkInfo);

            InitializePlastic();
            Repaint();
        }

        internal void RefreshWorkspaceUI()
        {
            InitializePlastic();
            Repaint();

            OnFocus();
        }

        void OnEnable()
        {
            processor.AssetModificationProcessor.ForceCheckout = EditorPrefs.GetBool("forceCheckoutPlasticSCM"); 
            wantsMouseMove = true;

            if (mException != null)
                return;

            minSize = new Vector2(
                UnityConstants.PLASTIC_WINDOW_MIN_SIZE_WIDTH,
                UnityConstants.PLASTIC_WINDOW_MIN_SIZE_HEIGHT);

            SetupWindowTitle();

            GuiMessage.Initialize(new UnityPlasticGuiMessage(this));

            PlasticApp.InitializeIfNeeded();

            RegisterApplicationFocusHandlers(this);

            PlasticMethodExceptionHandling.InitializeAskCredentialsUi(
                new CredentialsUiImpl(this));
            ClientEncryptionServiceProvider.SetEncryptionPasswordProvider(
                new MissingEncryptionPasswordPromptHandler(this));

            if (mPlasticAPI == null)
                mPlasticAPI = new PlasticAPI();
            mPlasticWebRestApi = new PlasticWebRestApi();

            mEventSenderScheduler = EventTracking.Configure(
                mPlasticWebRestApi,
                AssetsPath.IsRunningAsUPMPackage() ?
                 ApplicationIdentifier.UnityPackage : ApplicationIdentifier.UnityAssetStorePlugin,
                IdentifyEventPlatform.Get());

            if (mEventSenderScheduler != null)
            {
                mPingEventLoop = new PingEventLoop();
                mPingEventLoop.Start();
                mPingEventLoop.SetUnityVersion(Application.unityVersion);

                CollabPlugin.GetVersion(pluginVersion => mPingEventLoop.SetPluginVersion(pluginVersion));
            }

            InitializePlastic();
        }

        void OnDisable()
        {
            EditorPrefs.SetBool("forceCheckoutPlasticSCM",
                processor.AssetModificationProcessor.ForceCheckout);

            AssetsProcessors.Disable();

            if (mWkInfo != null)
            {
                MonoFileSystemWatcher.IsEnabled = false;
                WorkspaceFsNodeReaderCachesCleaner.CleanWorkspaceFsNodeReader(mWkInfo);
            }

            if (mException != null)
                return;

            if (mWkInfo == null)
            {
                ClosePlasticWindow(this);
                return;
            }

            mViewSwitcher.OnDisable();

            ClosePlasticWindow(this);
        }

        void OnDestroy()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            if (!mWorkspaceWindow.IsOperationInProgress())
                return;

            bool bCloseWindow = GuiMessage.ShowQuestion(
                PlasticLocalization.GetString(PlasticLocalization.Name.OperationRunning),
                PlasticLocalization.GetString(PlasticLocalization.Name.ConfirmClosingRunningOperation),
                PlasticLocalization.GetString(PlasticLocalization.Name.YesButton));

            if (bCloseWindow)
                return;

            mForceToOpen = true;
            ShowPlasticWindow(this);
        }

        void OnFocus()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            mViewSwitcher.AutoRefreshPendingChangesView();
            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void OnGUI()
        {
            if (mException != null)
            {
                DoExceptionErrorArea();
                return;
            }

            try
            {
                // IMPORTANT: disable collab (if needed)
                // must be executed before the next if statement
                // where we check if collab is enabled
                if (mDisableCollabIfEnabledWhenLoaded)
                {
                    mDisableCollabIfEnabledWhenLoaded = false;
                    DisableCollabIfEnabled(ProjectPath.FromApplicationDataPath(
                        Application.dataPath));
                }

                if (CollabPlugin.IsEnabled())
                {
                    // execute Close() once after all inspectors update
                    // to avoid our window to be drawn in back color
                    EditorApplication.delayCall = Close;
                    return;
                }

                bool isPlasticExeAvailable = IsExeAvailable.ForMode(mIsGluonMode);
                bool clientNeedsConfiguration = UnityConfigurationChecker.NeedsConfiguration();

                var welcomeView = GetWelcomeView();

                if (clientNeedsConfiguration && welcomeView.autoLoginState == AutoLogin.State.Off)
                {
                    welcomeView.autoLoginState = AutoLogin.State.Started;
                }

                if (welcomeView.autoLoginState == AutoLogin.State.OrganizationChoosed)
                {
                    OnEnable();
                    welcomeView.autoLoginState = AutoLogin.State.InitializingPlastic;
                }

                if (NeedsToDisplayWelcomeView(clientNeedsConfiguration, mWkInfo))
                {
                    welcomeView.OnGUI(clientNeedsConfiguration);
                    return;
                }

                //TODO: Codice - beta: hide the switcher until the update dialog is implemented
                //DrawGuiModeSwitcher.ForMode(
                //    isGluonMode, plasticClient, changesTreeView, editorWindow);

                DoTabToolbar(
                    isPlasticExeAvailable,
                    mWkInfo,
                    mViewSwitcher,
                    mIsGluonMode);

                mViewSwitcher.TabViewGUI();

                if (mWorkspaceWindow.IsOperationInProgress())
                    DrawProgressForOperations.For(
                        mWorkspaceWindow, mWorkspaceWindow.Progress,
                        position.width);

                mStatusBar.OnGUI(
                    mWkInfo,
                    mWorkspaceWindow,
                    mViewSwitcher,
                    mViewSwitcher,
                    mIncomingChangesNotifier,
                    mIsGluonMode);
            }
            catch (Exception ex)
            {
                if (IsExitGUIException(ex))
                    throw;

                GUI.enabled = true;

                if (IsIMGUIPaintException(ex))
                {
                    ExceptionsHandler.LogException("PlasticWindow", ex);
                    return;
                }

                mException = ex;

                DoExceptionErrorArea();

                ExceptionsHandler.HandleException("OnGUI", ex);
            }
        }

        void Update()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            try
            {
                double currentUpdateTime = EditorApplication.timeSinceStartup;
                double elapsedSeconds = currentUpdateTime - mLastUpdateTime;

                mViewSwitcher.Update();
                mWorkspaceWindow.OnParentUpdated(elapsedSeconds);

                if (mWelcomeView != null)
                    mWelcomeView.Update();

                mLastUpdateTime = currentUpdateTime;
            }
            catch (Exception ex)
            {
                mException = ex;

                ExceptionsHandler.HandleException("Update", ex);
            }
        }

        internal void InitializePlastic()
        {
            if (mForceToOpen)
            {
                mForceToOpen = false;
                return;
            }

            try
            {
                if (UnityConfigurationChecker.NeedsConfiguration())
                    return;

                mWkInfo = FindWorkspace.InfoForApplicationPath(
                    Application.dataPath, mPlasticAPI);

                if (mWkInfo == null)
                {
                    return;
                }

                MonoFileSystemWatcher.IsEnabled = true;

                SetupCloudProjectIdIfNeeded(mWkInfo, mPlasticAPI);

                DisableVCSIfEnabled(mWkInfo.ClientPath);

                mIsGluonMode = mPlasticAPI.IsGluonWorkspace(mWkInfo);

                IAssetStatusCache assetStatusCache =
                    new AssetStatusCache(
                        mWkInfo,
                        mIsGluonMode,
                        RepaintProjectWindow);

                AssetsProcessors.Enable(
                    mPlasticAPI,
                    assetStatusCache);

                if (mEventSenderScheduler != null)
                {
                    mPingEventLoop.SetWorkspace(mWkInfo);
                    ((IPlasticWebRestApi)mPlasticWebRestApi).SetToken(
                        CmConnection.Get().BuildWebApiTokenForCloudEditionDefaultUser());
                }

                InitializeNewIncomingChanges(mWkInfo, mIsGluonMode);

                ViewHost viewHost = new ViewHost();

                PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                    new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(mWkInfo);

                mViewSwitcher = new ViewSwitcher(
                    mWkInfo,
                    viewHost,
                    mIsGluonMode,
                    pendingChanges,
                    mDeveloperNewIncomingChangesUpdater,
                    mGluonNewIncomingChangesUpdater,
                    mIncomingChangesNotifier,
                    assetStatusCache,
                    mStatusBar,
                    this);

                mCooldownAutoRefreshPendingChangesAction = new CooldownWindowDelayer(
                    mViewSwitcher.AutoRefreshPendingChangesView,
                    UnityConstants.AUTO_REFRESH_PENDING_CHANGES_DELAYED_INTERVAL);

                mWorkspaceWindow = new WorkspaceWindow(
                    mWkInfo,
                    viewHost,
                    mViewSwitcher,
                    mViewSwitcher,
                    mDeveloperNewIncomingChangesUpdater,
                    this);

                mViewSwitcher.SetWorkspaceWindow(mWorkspaceWindow);
                mViewSwitcher.ShowInitialView();

                UnityStyles.Initialize(Repaint);

                AssetOperations.IAssetSelection inspectorAssetSelection =
                    new InspectorAssetSelection();

                AssetOperations.IAssetSelection projectViewAssetSelection =
                    new ProjectViewAssetSelection();

                AssetOperations inspectorAssetOperations =
                    new AssetOperations(
                        mWkInfo,
                        mWorkspaceWindow,
                        mViewSwitcher,
                        mViewSwitcher,
                        viewHost,
                        mDeveloperNewIncomingChangesUpdater,
                        assetStatusCache,
                        mViewSwitcher,
                        mViewSwitcher,
                        this,
                        inspectorAssetSelection,
                        mIsGluonMode);

                mAssetOperations =
                    new AssetOperations(
                        mWkInfo,
                        mWorkspaceWindow,
                        mViewSwitcher,
                        mViewSwitcher,
                        viewHost,
                        mDeveloperNewIncomingChangesUpdater,
                        assetStatusCache,
                        mViewSwitcher,
                        mViewSwitcher,
                        this,
                        projectViewAssetSelection,
                        mIsGluonMode);

                DrawInspectorOperations.Enable(
                    inspectorAssetOperations,
                    assetStatusCache,
                    inspectorAssetSelection);

                DrawAssetOverlay.Initialize(
                    assetStatusCache,
                    RepaintProjectWindow);

                mLastUpdateTime = EditorApplication.timeSinceStartup;

                mViewSwitcher.ShowBranchesViewIfNeeded();
            }
            catch (Exception ex)
            {
                mException = ex;

                ExceptionsHandler.HandleException("InitializePlastic", ex);
            }
        }

        void InitializeNewIncomingChanges(
            WorkspaceInfo wkInfo,
            bool bIsGluonMode)
        {
            if (bIsGluonMode)
            {
                Gluon.IncomingChangesNotifier gluonNotifier =
                    new Gluon.IncomingChangesNotifier(this);
                mGluonNewIncomingChangesUpdater =
                    NewIncomingChanges.BuildUpdaterForGluon(
                        wkInfo,
                        this,
                        gluonNotifier,
                        new GluonCheckIncomingChanges.CalculateIncomingChanges());
                mIncomingChangesNotifier = gluonNotifier;
                return;
            }

            Developer.IncomingChangesNotifier developerNotifier =
                new Developer.IncomingChangesNotifier(this);
            mDeveloperNewIncomingChangesUpdater =
                NewIncomingChanges.BuildUpdaterForDeveloper(
                    wkInfo, this, developerNotifier);
            mIncomingChangesNotifier = developerNotifier;
        }

        void OnApplicationActivated()
        {
            if (mException != null)
                return;

            Reload.IfWorkspaceConfigChanged(
                mPlasticAPI, mWkInfo, mIsGluonMode,
                ExecuteFullReload);

            if (mWkInfo == null)
                return;

            ((IWorkspaceWindow)mWorkspaceWindow).UpdateTitle();

            NewIncomingChanges.LaunchUpdater(
                mDeveloperNewIncomingChangesUpdater,
                mGluonNewIncomingChangesUpdater);

            // When Unity Editor window is activated it writes some files to its Temp folder.
            // This causes the fswatcher to process those events.
            // We need to wait until the fswatcher finishes processing the events,
            // otherwise the NewChangesInWk method will return TRUE, causing
            // the pending changes view to unwanted auto-refresh.
            // So, we need to delay the auto-refresh call in order
            // to give the fswatcher enough time to process the events.
            // Note that the OnFocus event is not affected by this issue.
            mCooldownAutoRefreshPendingChangesAction.Ping();

            mViewSwitcher.AutoRefreshIncomingChangesView();
        }

        void OnApplicationDeactivated()
        {
            if (mException != null)
                return;

            if (mWkInfo == null)
                return;

            NewIncomingChanges.StopUpdater(
                mDeveloperNewIncomingChangesUpdater,
                mGluonNewIncomingChangesUpdater);
        }

        void ExecuteFullReload()
        {
            mException = null;

            DisposeNewIncomingChanges(this);

            InitializePlastic();
        }

        void DoExceptionErrorArea()
        {
            string labelText = PlasticLocalization.GetString(
                PlasticLocalization.Name.UnexpectedError);

            string buttonText = PlasticLocalization.GetString(
                PlasticLocalization.Name.ReloadButton);

            DrawActionHelpBox.For(
                Images.GetErrorDialogIcon(), labelText, buttonText,
                ExecuteFullReload);
        }

        internal WelcomeView GetWelcomeView()
        {
            if (mWelcomeView != null)
                return mWelcomeView;

            mWelcomeView = new WelcomeView(
                this,
                this,
                mPlasticAPI,
                CmConnection.Get(),
                mPlasticWebRestApi);

            return mWelcomeView;
        }

        static void DoSearchField(ViewSwitcher viewSwitcher)
        {
            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.PendingChanges))
            {
                viewSwitcher.PendingChangesTab.DrawSearchFieldForPendingChangesTab();
                return;
            }
            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.Changesets))
            {
                viewSwitcher.ChangesetsTab.DrawSearchFieldForChangesetsTab();
                return;
            }
            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.History))
            {
                viewSwitcher.HistoryTab.DrawSearchFieldForHistoryTab();
                return;
            }
            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.Branches))
            {
                viewSwitcher.BranchesTab.DrawSearchFieldForBranchesTab();
                return;
            }
        }

        static void DoTabToolbar(
            bool isPlasticExeAvailable,
            WorkspaceInfo workspaceInfo,
            ViewSwitcher viewSwitcher,
            bool isGluonMode)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            viewSwitcher.TabButtonsGUI();

            GUILayout.FlexibleSpace();

            DoSearchField(viewSwitcher);

            DoLaunchButtons(
                isPlasticExeAvailable,
                workspaceInfo,
                viewSwitcher,
                isGluonMode);

            EditorGUILayout.EndHorizontal();
        }

        static void OpenBranchListViewAndSendEvent(
            WorkspaceInfo wkInfo,
            ViewSwitcher viewSwitcher)
        {
            viewSwitcher.ShowBranchesView();
            TrackFeatureUseEvent.For(
               PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
               TrackFeatureUseEvent.Features.OpenBranchesView);
        }

        static void ShowBranchesContextMenu(
             WorkspaceInfo wkInfo,
            ViewSwitcher viewSwitcher,
            bool isGluonMode)
        {
            GenericMenu menu = new GenericMenu();

            string branchesListView = PlasticLocalization.GetString(
                PlasticLocalization.Name.Branches);

            menu.AddItem(
               new GUIContent(branchesListView),
               false,
               () => OpenBranchListViewAndSendEvent(wkInfo, viewSwitcher));

            string branchExplorer = PlasticLocalization.GetString(
            PlasticLocalization.Name.BranchExplorerMenu);

            menu.AddItem(
              new GUIContent(branchExplorer),
              false,
              () => LaunchTool.OpenBranchExplorer(wkInfo, isGluonMode));

            menu.ShowAsContext();
        }

        static void ShowSettingsContextMenu(
            WorkspaceInfo wkInfo,
            bool isGluonMode)
        {
            GenericMenu menu = new GenericMenu();

            string openToolText = isGluonMode ?
                PlasticLocalization.GetString(PlasticLocalization.Name.LaunchGluonButton) :
                PlasticLocalization.GetString(PlasticLocalization.Name.LaunchPlasticButton);

            menu.AddItem(
               new GUIContent(openToolText),
               false,
               () => LaunchTool.OpenGUIForMode(wkInfo, isGluonMode));

            PlasticWindow plasticWindow = EditorWindow.GetWindow<PlasticWindow>();

            menu.AddItem(
                new GUIContent(
                    PlasticLocalization.GetString(
                       PlasticLocalization.Name.InviteMembers)),
                false,
               InviteMemberButton_clicked,
               null);

            menu.AddSeparator("");

            menu.AddItem(
                new GUIContent(
                    PlasticLocalization.GetString(
                PlasticLocalization.Name.Options)),
                false,
                () => SettingsService.OpenProjectSettings(UnityConstants.PROJECT_SETTINGS_MENU_TITLE));

            // If the user has the simplified UI key of type .txt in the Assets folder
            // TODO: Remove when Simplified UI is complete
            if (AssetDatabase.FindAssets("simplifieduikey t:textasset", new[] { "Assets" }).Any())
                menu.AddItem(new GUIContent("Try Simplified UI"),
                    false,
                    TrySimplifiedUIButton_Clicked,
                    null);

            //TODO: Localization
            menu.AddItem(
                new GUIContent(processor.AssetModificationProcessor.ForceCheckout ?
                PlasticLocalization.GetString(PlasticLocalization.Name.DisableForcedCheckout) :
                PlasticLocalization.GetString(PlasticLocalization.Name.EnableForcedCheckout)),
                false,
                ForceCheckout_Clicked,
                null);

            menu.AddSeparator("");

            menu.AddItem(
                new GUIContent(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.TurnOffPlasticSCM)),
                false,
                TurnOffPlasticButton_Clicked,
                null);

            menu.ShowAsContext();
        }

        static void DoLaunchButtons(
            bool isPlasticExeAvailable,
            WorkspaceInfo wkInfo,
            ViewSwitcher viewSwitcher,
            bool isGluonMode)
        {
            //TODO: Codice - beta: hide the diff button until the behavior is implemented
            /*GUILayout.Button(PlasticLocalization.GetString(
                PlasticLocalization.Name.DiffWindowMenuItemDiff),
                EditorStyles.toolbarButton,
                GUILayout.Width(UnityConstants.REGULAR_BUTTON_WIDTH));*/

            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.Changesets))
            {
                viewSwitcher.ChangesetsTab.DrawDateFilter();
            }
            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.Branches))
            {
                viewSwitcher.BranchesTab.DrawDateFilter();
            }

            Texture refreshIcon = Images.GetRefreshIcon();
            string refreshIconTooltip = PlasticLocalization.GetString(
                PlasticLocalization.Name.RefreshButton);

            if (DrawLaunchButton(refreshIcon, refreshIconTooltip))
            {
                viewSwitcher.RefreshSelectedView();
            }

            if (viewSwitcher.IsViewSelected(ViewSwitcher.SelectedTab.PendingChanges))
            {
                Texture2D icon = Images.GetImage(Images.Name.IconUndo);
                string tooltip = PlasticLocalization.GetString(
                    PlasticLocalization.Name.UndoSelectedChanges);

                if (DrawLaunchButton(icon, tooltip))
                {
                    TrackFeatureUseEvent.For(
                        PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
                        TrackFeatureUseEvent.Features.UndoIconButton);

                    viewSwitcher.PendingChangesTab.UndoForMode(wkInfo, isGluonMode);
                }
            }

            if (isGluonMode)
            {
                string label = PlasticLocalization.GetString(PlasticLocalization.Name.ConfigureGluon);
                if (DrawActionButton.For(label))
                    LaunchTool.OpenWorkspaceConfiguration(wkInfo, isGluonMode);
            }
            else
            {
                Texture2D icon = Images.GetImage(Images.Name.IconBranch);
                string tooltip = PlasticLocalization.GetString(PlasticLocalization.Name.Branches);
                if (DrawLaunchButton(icon, tooltip))
                {
                    ShowBranchesContextMenu(
                        wkInfo,
                        viewSwitcher,
                        isGluonMode);
                }
            }

            //TODO: Add settings button tooltip localization
            if (DrawLaunchButton(Images.GetSettingsIcon(), string.Empty))
            {
                ShowSettingsContextMenu(
                    wkInfo,
                    isGluonMode);
            }
        }

        static bool DrawLaunchButton(Texture icon, string tooltip)
        {
            return GUILayout.Button(
                new GUIContent(icon, tooltip),
                EditorStyles.toolbarButton, 
                GUILayout.Width(26));
        }

        static void InviteMemberButton_clicked(object obj)
        {
            Application.OpenURL("https://www.plasticscm.com/dashboard/cloud/unity_cloud/users-and-groups");
        }

        static void TrySimplifiedUIButton_Clicked(object obj)
        {
            PlasticSCMWindow.ShowWindow();
        }

        static void TurnOffPlasticButton_Clicked(object obj)
        {
            ShowWindow.Plastic();
            TurnOffPlasticWindow.ShowWindow();
        }
        static void ForceCheckout_Clicked(object obj)
        {
            processor.AssetModificationProcessor.ForceCheckout = 
                !processor.AssetModificationProcessor.ForceCheckout;
            EditorPrefs.SetBool("forceCheckoutPlasticSCM",
                processor.AssetModificationProcessor.ForceCheckout);
        }

        static void SetupCloudProjectIdIfNeeded(
            WorkspaceInfo wkInfo,
            IPlasticAPI plasticApi)
        {
            if (SetupCloudProjectId.HasCloudProjectId())
                return;

            SetupCloudProjectId.ForWorkspace(wkInfo, plasticApi);

            mLog.DebugFormat("Setup CloudProjectId on Project: {0}",
                wkInfo.ClientPath);
        }

        static void DisableVCSIfEnabled(string projectPath)
        {
            if (!VCSPlugin.IsEnabled())
                return;

            VCSPlugin.Disable();

            mLog.DebugFormat("Disabled VCS Plugin on Project: {0}",
                projectPath);
        }

        static void DisposeNewIncomingChanges(PlasticWindow window)
        {
            NewIncomingChanges.DisposeUpdater(
                window.mDeveloperNewIncomingChangesUpdater,
                window.mGluonNewIncomingChangesUpdater);

            window.mDeveloperNewIncomingChangesUpdater = null;
            window.mGluonNewIncomingChangesUpdater = null;
        }

        static void RegisterApplicationFocusHandlers(PlasticWindow window)
        {
            EditorWindowFocus.OnApplicationActivated += window.OnApplicationActivated;
            EditorWindowFocus.OnApplicationDeactivated += window.OnApplicationDeactivated;
        }

        static void UnRegisterApplicationFocusHandlers(PlasticWindow window)
        {
            EditorWindowFocus.OnApplicationActivated -= window.OnApplicationActivated;
            EditorWindowFocus.OnApplicationDeactivated -= window.OnApplicationDeactivated;
        }

        static bool IsExitGUIException(Exception ex)
        {
            return ex is ExitGUIException;
        }

        static bool IsIMGUIPaintException(Exception ex)
        {
            if (!(ex is ArgumentException))
                return false;

            return ex.Message.StartsWith("Getting control") &&
                   ex.Message.Contains("controls when doing repaint");
        }

        static void ClosePlasticWindow(PlasticWindow window)
        {
            UnRegisterApplicationFocusHandlers(window);

            PlasticApp.Dispose();

            DrawInspectorOperations.Disable();

            DrawAssetOverlay.Dispose();

            AssetMenuItems.Dispose();

            if (window.mEventSenderScheduler != null)
            {
                window.mPingEventLoop.Stop();
                window.mEventSenderScheduler.End();
            }

            DisposeNewIncomingChanges(window);

            AvatarImages.Dispose();
        }

        static void ShowPlasticWindow(PlasticWindow window)
        {
            EditorWindow dockWindow = FindEditorWindow.ToDock<PlasticWindow>();

            PlasticWindow newPlasticWindow = InstantiateFrom(window);

            if (DockEditorWindow.IsAvailable())
                DockEditorWindow.To(dockWindow, newPlasticWindow);

            newPlasticWindow.Show();

            newPlasticWindow.Focus();
        }

        static bool NeedsToDisplayWelcomeView(
            bool clientNeedsConfiguration,
            WorkspaceInfo wkInfo)
        {
            if (clientNeedsConfiguration)
                return true;

            if (wkInfo == null)
                return true;

            return false;
        }

        static void RepaintProjectWindow()
        {
            EditorWindow projectWindow = FindEditorWindow.ProjectWindow();

            if (projectWindow == null)
                return;

            projectWindow.Repaint();
        }

        static void DisableCollabIfEnabled(string projectPath)
        {
            if (!CollabPlugin.IsEnabled())
                return;

            CollabPlugin.Disable();

            mLog.DebugFormat("Disabled Collab Plugin on Project: {0}",
                projectPath);
        }

        static PlasticWindow InstantiateFrom(PlasticWindow window)
        {
            PlasticWindow result = Instantiate(window);
            result.mWkInfo = window.mWkInfo;
            result.mWorkspaceWindow = window.mWorkspaceWindow;
            result.mViewSwitcher = window.mViewSwitcher;
            result.mCooldownAutoRefreshPendingChangesAction = window.mCooldownAutoRefreshPendingChangesAction;
            result.mDeveloperNewIncomingChangesUpdater = window.mDeveloperNewIncomingChangesUpdater;
            result.mGluonNewIncomingChangesUpdater = window.mGluonNewIncomingChangesUpdater;
            result.mException = window.mException;
            result.mLastUpdateTime = window.mLastUpdateTime;
            result.mIsGluonMode = window.mIsGluonMode;
            result.mIncomingChangesNotifier = window.mIncomingChangesNotifier;
            result.mStatusBar = window.mStatusBar;
            result.mWelcomeView = window.mWelcomeView;
            result.mEventSenderScheduler = window.mEventSenderScheduler;
            result.mPingEventLoop = window.mPingEventLoop;
            return result;
        }

        internal PlasticProjectSettingsProvider.IAutoRefreshView GetPendingChangesView()
        {
            return mViewSwitcher != null ? mViewSwitcher.PendingChangesTab : null;
        }

        static class Reload
        {
            internal static void IfWorkspaceConfigChanged(
                IPlasticAPI plasticApi,
                WorkspaceInfo lastWkInfo,
                bool lastIsGluonMode,
                Action reloadAction)
            {
                string applicationPath = Application.dataPath;

                bool isGluonMode = false;
                WorkspaceInfo wkInfo = null;

                IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
                waiter.Execute(
                    /*threadOperationDelegate*/ delegate
                    {
                        wkInfo = FindWorkspace.
                            InfoForApplicationPath(applicationPath, plasticApi);

                        if (wkInfo != null)
                            isGluonMode = plasticApi.IsGluonWorkspace(wkInfo);
                    },
                    /*afterOperationDelegate*/ delegate
                    {
                        if (waiter.Exception != null)
                            return;

                        if (!IsWorkspaceConfigChanged(
                                lastWkInfo, wkInfo,
                                lastIsGluonMode, isGluonMode))
                            return;

                        reloadAction();
                    });
            }

            static bool IsWorkspaceConfigChanged(
                WorkspaceInfo lastWkInfo,
                WorkspaceInfo currentWkInfo,
                bool lastIsGluonMode,
                bool currentIsGluonMode)
            {
                if (lastIsGluonMode != currentIsGluonMode)
                    return true;

                if (lastWkInfo == null || currentWkInfo == null)
                    return true;

                return !lastWkInfo.Equals(currentWkInfo);
            }
        }

        [SerializeField]
        bool mForceToOpen;

        [NonSerialized]
        WorkspaceInfo mWkInfo;

        Exception mException;

        internal IIncomingChangesNotifier mIncomingChangesNotifier;

        double mLastUpdateTime = 0f;

        CooldownWindowDelayer mCooldownAutoRefreshPendingChangesAction;
        internal ViewSwitcher mViewSwitcher;
        WelcomeView mWelcomeView;
        StatusBar mStatusBar = new StatusBar();

        PlasticGui.WorkspaceWindow.NewIncomingChangesUpdater mDeveloperNewIncomingChangesUpdater;
        GluonNewIncomingChangesUpdater mGluonNewIncomingChangesUpdater;

        WorkspaceWindow mWorkspaceWindow;

        bool mIsGluonMode;
        bool mDisableCollabIfEnabledWhenLoaded;

        static PlasticAPI mPlasticAPI;
        static PlasticWebRestApi mPlasticWebRestApi;
        EventSenderScheduler mEventSenderScheduler;
        PingEventLoop mPingEventLoop;
        AssetOperations mAssetOperations;

        PlasticNotification.Status mNotificationStatus = PlasticNotification.Status.None;
        static Texture mPlasticWindowIcon;
        static readonly ILog mLog = LogManager.GetLogger("PlasticWindow");
    }
}