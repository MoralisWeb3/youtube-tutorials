using System;

using UnityEditor;

using Codice.Client.Common.Threading;
using Codice.CM.Common;
using GluonGui;
using PlasticGui;
using PlasticGui.Gluon;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.Merge;
using PlasticGui.WorkspaceWindow.QueryViews;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetUtils.Processor;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Views.Changesets;
using Unity.PlasticSCM.Editor.Views.History;
using Unity.PlasticSCM.Editor.Views.IncomingChanges;
using Unity.PlasticSCM.Editor.Views.PendingChanges;
using Unity.PlasticSCM.Editor.Views.Branches;

using GluonNewIncomingChangesUpdater = PlasticGui.Gluon.WorkspaceWindow.NewIncomingChangesUpdater;
using ObjectInfo = Codice.CM.Common.ObjectInfo;

namespace Unity.PlasticSCM.Editor
{
    internal class ViewSwitcher :
        IViewSwitcher,
        IMergeViewLauncher,
        IGluonViewSwitcher,
        IHistoryViewLauncher
    {
        internal IIncomingChangesTab IncomingChangesTabForTesting { get { return mIncomingChangesTab; } }
        internal PendingChangesTab PendingChangesTab { get; private set; }
        internal ChangesetsTab ChangesetsTab { get; private set; }
        internal BranchesTab BranchesTab { get; private set; }
        internal HistoryTab HistoryTab { get; private set; }

        internal ViewSwitcher(
            WorkspaceInfo wkInfo,
            ViewHost viewHost,
            bool isGluonMode,
            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges,
            NewIncomingChangesUpdater developerNewIncomingChangesUpdater,
            GluonNewIncomingChangesUpdater gluonNewIncomingChangesUpdater,
            IIncomingChangesNotifier incomingChangesNotifier,
            IAssetStatusCache assetStatusCache,
            StatusBar statusBar,
            EditorWindow parentWindow)
        {
            mWkInfo = wkInfo;
            mViewHost = viewHost;
            mIsGluonMode = isGluonMode;
            mPendingChanges = pendingChanges;
            mDeveloperNewIncomingChangesUpdater = developerNewIncomingChangesUpdater;
            mGluonNewIncomingChangesUpdater = gluonNewIncomingChangesUpdater;
            mIncomingChangesNotifier = incomingChangesNotifier;
            mAssetStatusCache = assetStatusCache;
            mStatusBar = statusBar;
            mParentWindow = parentWindow;

            mPendingChangesTabButton = new TabButton();
            mIncomingChangesTabButton = new TabButton();
            mChangesetsTabButton = new TabButton();
            mBranchesTabButton = new TabButton();
            mHistoryTabButton = new TabButton();
        }

        internal void SetWorkspaceWindow(WorkspaceWindow workspaceWindow)
        {
            mWorkspaceWindow = workspaceWindow;
        }

        internal void ShowInitialView()
        {
            ShowPendingChangesView();
        }

        internal void AutoRefreshPendingChangesView()
        {
            AutoRefresh.PendingChangesView(
                PendingChangesTab);
        }

        internal void AutoRefreshIncomingChangesView()
        {
            AutoRefresh.IncomingChangesView(
                mIncomingChangesTab);
        }

        internal void RefreshView(ViewType viewType)
        {
            IRefreshableView view = GetRefreshableView(viewType);

            if (view == null)
                return;

            view.Refresh();
        }

        internal void RefreshSelectedView()
        {
            IRefreshableView view = GetRefreshableViewBasedOnSelectedTab(mSelectedTab);

            if (view == null)
                return;

            view.Refresh();
        }

        internal void OnDisable()
        {
            PlasticAssetsProcessor.UnRegisterViews();

            if (PendingChangesTab != null)
                PendingChangesTab.OnDisable();

            if (mIncomingChangesTab != null)
                mIncomingChangesTab.OnDisable();

            if (ChangesetsTab != null)
                ChangesetsTab.OnDisable();

            if (BranchesTab != null)
                BranchesTab.OnDisable();

            if (HistoryTab != null)
                HistoryTab.OnDisable();
        }

        internal void Update()
        {
            if (IsViewSelected(SelectedTab.PendingChanges))
            {
                PendingChangesTab.Update();
                return;
            }

            if (IsViewSelected(SelectedTab.IncomingChanges))
            {
                mIncomingChangesTab.Update();
                return;
            }

            if (IsViewSelected(SelectedTab.Changesets))
            {
                ChangesetsTab.Update();
                return;
            }

            if (IsViewSelected(SelectedTab.Branches))
            {
                BranchesTab.Update();
                return;
            }

            if (IsViewSelected(SelectedTab.History))
            {
                HistoryTab.Update();
                return;
            }
        }

        internal void TabButtonsGUI()
        {
            InitializeTabButtonWidth();

            PendingChangesTabButtonGUI();

            IncomingChangesTabButtonGUI();

            ChangesetsTabButtonGUI();

            BranchesTabButtonGUI();

            HistoryTabButtonGUI();
        }

        internal void TabViewGUI()
        {
            if (IsViewSelected(SelectedTab.PendingChanges))
            {
                PendingChangesTab.OnGUI();
                return;
            }

            if (IsViewSelected(SelectedTab.IncomingChanges))
            {
                mIncomingChangesTab.OnGUI();
                return;
            }

            if (IsViewSelected(SelectedTab.Changesets))
            {
                ChangesetsTab.OnGUI();
                return;
            }

            if (IsViewSelected(SelectedTab.Branches))
            {
                BranchesTab.OnGUI();
                return;
            }

            if (IsViewSelected(SelectedTab.History))
            {
                HistoryTab.OnGUI();
                return;
            }
        }

        internal void ShowBranchesViewIfNeeded()
        {
            if (!BoolSetting.Load(UnityConstants.SHOW_BRANCHES_VIEW_KEY_NAME, true))
                return;

            string query = QueryConstants.BranchesBeginningQuery;

            ViewQueryResult queryResult = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    queryResult = new ViewQueryResult(
                        PlasticGui.Plastic.API.FindQuery(mWkInfo, query));
                },
                /*afterOperationDelegate*/ delegate
                {
                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.DisplayException(waiter.Exception);
                        return;
                    }

                    if (queryResult == null)
                        return;
                   
                    if (queryResult.Count()>0)
                        OpenBranchesTab();
                });
        }

        internal void ShowBranchesView()
        {
            OpenBranchesTab();

            bool wasBranchesSelected =
                IsViewSelected(SelectedTab.Branches);

            if (!wasBranchesSelected)
                ((IRefreshableView)BranchesTab).Refresh();

            SetSelectedView(SelectedTab.Branches);
        }

        void IViewSwitcher.ShowPendingChanges()
        {
            ShowPendingChangesView();
            mParentWindow.Repaint();
        }

        void IViewSwitcher.ShowSyncView(string syncViewToSelect)
        {
            throw new NotImplementedException();
        }

        void IViewSwitcher.ShowBranchExplorerView()
        {
            //TODO: Codice
            //launch plastic with branch explorer view option
        }

        void IViewSwitcher.DisableMergeView()
        {
        }

        bool IViewSwitcher.IsIncomingChangesView()
        {
            return IsViewSelected(SelectedTab.IncomingChanges);
        }

        void IViewSwitcher.CloseIncomingChangesView()
        {
            ((IViewSwitcher)this).DisableMergeView();
        }

        void IMergeViewLauncher.MergeFrom(ObjectInfo objectInfo, EnumMergeType mergeType)
        {
            ((IMergeViewLauncher)this).MergeFromInterval(objectInfo, null, mergeType);
        }

        void IMergeViewLauncher.MergeFrom(ObjectInfo objectInfo, EnumMergeType mergeType, ShowIncomingChangesFrom from)
        {
            ((IMergeViewLauncher)this).MergeFromInterval(objectInfo, null, mergeType);
        }

        void IMergeViewLauncher.MergeFromInterval(ObjectInfo objectInfo, ObjectInfo ancestorChangesetInfo, EnumMergeType mergeType)
        {
            if (mergeType == EnumMergeType.IncomingMerge)
            {
                ShowIncomingChangesView();
                mParentWindow.Repaint();
                return;
            }

            LaunchTool.OpenMerge(mWkInfo, mIsGluonMode);
        }

        void IGluonViewSwitcher.ShowIncomingChangesView()
        {
            ShowIncomingChangesView();
            mParentWindow.Repaint();
        }

        void IHistoryViewLauncher.ShowHistoryView(
            RepositorySpec repSpec,
            long itemId,
            string path,
            bool isDirectory)
        {
            ShowHistoryView(
                repSpec,
                itemId,
                path,
                isDirectory);

            mParentWindow.Repaint();
        }

        void CloseHistoryTab()
        {
            ShowView(mPreviousSelectedTab);

            mViewHost.RemoveRefreshableView(
                ViewType.HistoryView, HistoryTab);

            HistoryTab.OnDisable();
            HistoryTab = null;

            mParentWindow.Repaint();
        }

        void OpenBranchesTab()
        {
            if (BranchesTab == null)
            {
                BranchesTab = new BranchesTab(
                     mWkInfo,
                     mWorkspaceWindow,
                     this,
                     this,
                     mWorkspaceWindow,
                     mDeveloperNewIncomingChangesUpdater,
                     mParentWindow);

                mViewHost.AddRefreshableView(
                    ViewType.BranchesView, BranchesTab);
            }

            BoolSetting.Save(true, UnityConstants.SHOW_BRANCHES_VIEW_KEY_NAME);
        }

        void CloseBranchesTab()
        {
            BoolSetting.Save(false,UnityConstants.SHOW_BRANCHES_VIEW_KEY_NAME);

            ShowView(mPreviousSelectedTab);

            mViewHost.RemoveRefreshableView(
                ViewType.BranchesView, BranchesTab);

            BranchesTab.OnDisable();
            BranchesTab = null;

            mParentWindow.Repaint();
        }

        void ShowPendingChangesView()
        {
            if (PendingChangesTab == null)
            {
                PendingChangesTab = new PendingChangesTab(
                    mWkInfo,
                    mViewHost,
                    mIsGluonMode,
                    mWorkspaceWindow,
                    this,
                    this,
                    this,
                    mPendingChanges,
                    mDeveloperNewIncomingChangesUpdater,
                    mGluonNewIncomingChangesUpdater,
                    mAssetStatusCache,
                    mStatusBar,
                    mParentWindow);

                mViewHost.AddRefreshableView(
                    ViewType.CheckinView,
                    PendingChangesTab);

                PlasticAssetsProcessor.
                    RegisterPendingChangesView(PendingChangesTab);
            }

            bool wasPendingChangesSelected =
                IsViewSelected(SelectedTab.PendingChanges);

            if (!wasPendingChangesSelected)
            {
                PendingChangesTab.AutoRefresh();
            }

            SetSelectedView(SelectedTab.PendingChanges);
        }

        void ShowIncomingChangesView()
        {
            if (mIncomingChangesTab == null)
            {
                mIncomingChangesTab = mIsGluonMode ?
                    new Views.IncomingChanges.Gluon.IncomingChangesTab(
                        mWkInfo,
                        mViewHost,
                        mWorkspaceWindow,
                        mGluonNewIncomingChangesUpdater,
                        (Gluon.IncomingChangesNotifier)mIncomingChangesNotifier,
                        mStatusBar,
                        mParentWindow) as IIncomingChangesTab :
                    new Views.IncomingChanges.Developer.IncomingChangesTab(
                        mWkInfo,
                        mWorkspaceWindow,
                        this,
                        mDeveloperNewIncomingChangesUpdater,
                        mParentWindow);

                mViewHost.AddRefreshableView(
                    ViewType.IncomingChangesView,
                    (IRefreshableView)mIncomingChangesTab);

                PlasticAssetsProcessor.
                    RegisterIncomingChangesView(mIncomingChangesTab);
            }

            bool wasIncomingChangesSelected =
                IsViewSelected(SelectedTab.IncomingChanges);

            if (!wasIncomingChangesSelected)
                mIncomingChangesTab.AutoRefresh();

            SetSelectedView(SelectedTab.IncomingChanges);
        }

        void ShowChangesetsView()
        {
            if (ChangesetsTab == null)
            {
                ChangesetsTab = new ChangesetsTab(
                    mWkInfo,
                    mWorkspaceWindow,
                    this,
                    this,
                    mParentWindow,
                    mIsGluonMode);

                mViewHost.AddRefreshableView(
                    ViewType.ChangesetsView,
                    ChangesetsTab);
            }

            bool wasChangesetsSelected =
                IsViewSelected(SelectedTab.Changesets);

            if (!wasChangesetsSelected)
                ((IRefreshableView)ChangesetsTab).Refresh();

            SetSelectedView(SelectedTab.Changesets);
        }

        void ShowHistoryView(
            RepositorySpec repSpec,
            long itemId,
            string path,
            bool isDirectory)
        {
            if (HistoryTab == null)
            {
                HistoryTab = new HistoryTab(
                    mWkInfo,
                    mWorkspaceWindow,
                    repSpec,
                    mDeveloperNewIncomingChangesUpdater,
                    mViewHost,
                    mParentWindow,
                    mIsGluonMode);

                mViewHost.AddRefreshableView(
                    ViewType.HistoryView, HistoryTab);
            }

            HistoryTab.RefreshForItem(
                itemId,
                path,
                isDirectory);

            SetSelectedView(SelectedTab.History);
        }

        void InitializeTabButtonWidth()
        {
            if (mTabButtonWidth != -1)
                return;

            mTabButtonWidth = MeasureMaxWidth.ForTexts(
                UnityStyles.PlasticWindow.TabButton,
                PlasticLocalization.GetString(PlasticLocalization.Name.PendingChangesViewTitle),
                PlasticLocalization.GetString(PlasticLocalization.Name.IncomingChangesViewTitle),
                PlasticLocalization.GetString(PlasticLocalization.Name.BranchesViewTitle),
                PlasticLocalization.GetString(PlasticLocalization.Name.ChangesetsViewTitle));
        }

        void ShowView(SelectedTab viewToShow)
        {
            switch (viewToShow)
            {
                case SelectedTab.PendingChanges:
                    ShowPendingChangesView();
                    break;

                case SelectedTab.IncomingChanges:
                    ShowIncomingChangesView();
                    break;

                case SelectedTab.Changesets:
                    ShowChangesetsView();
                    break;
            }
        }

        IRefreshableView GetRefreshableViewBasedOnSelectedTab(SelectedTab selectedTab)
        {
            switch (selectedTab)
            {
                case SelectedTab.PendingChanges:
                    return PendingChangesTab;

                case SelectedTab.IncomingChanges:
                    return (IRefreshableView)mIncomingChangesTab;

                case SelectedTab.Changesets:
                    return ChangesetsTab;

                case SelectedTab.Branches:
                    return BranchesTab;

                case SelectedTab.History:
                    return HistoryTab;

                default:
                    return null;
            }
        }

        IRefreshableView GetRefreshableView(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.PendingChangesView:
                    return PendingChangesTab;

                case ViewType.IncomingChangesView:
                    return (IRefreshableView)mIncomingChangesTab;

                case ViewType.ChangesetsView:
                    return ChangesetsTab;

                case ViewType.BranchesView:
                    return BranchesTab;

                case ViewType.HistoryView:
                    return HistoryTab;
                default:
                    return null;
            }
        }

        internal bool IsViewSelected(SelectedTab tab)
        {
            return mSelectedTab == tab;
        }

        void SetSelectedView(SelectedTab tab)
        {
            if (mSelectedTab != tab)
                mPreviousSelectedTab = mSelectedTab;

            mSelectedTab = tab;

            if (mIncomingChangesTab == null)
                return;

            mIncomingChangesTab.IsVisible =
                tab == SelectedTab.IncomingChanges;
        }

        void PendingChangesTabButtonGUI()
        {
            bool wasPendingChangesSelected =
                IsViewSelected(SelectedTab.PendingChanges);

            bool isPendingChangesSelected = mPendingChangesTabButton.
                DrawTabButton(
                    PlasticLocalization.GetString(PlasticLocalization.Name.PendingChangesViewTitle),
                    wasPendingChangesSelected,
                    mTabButtonWidth);

            if (isPendingChangesSelected)
                ShowPendingChangesView();
        }

        void IncomingChangesTabButtonGUI()
        {
            bool wasIncomingChangesSelected =
                IsViewSelected(SelectedTab.IncomingChanges);

            bool isIncomingChangesSelected = mIncomingChangesTabButton.
                DrawTabButton(
                    PlasticLocalization.GetString(PlasticLocalization.Name.IncomingChangesViewTitle),
                    wasIncomingChangesSelected,
                    mTabButtonWidth);

            if (isIncomingChangesSelected)
                ShowIncomingChangesView();
        }

        void ChangesetsTabButtonGUI()
        {
            bool wasChangesetsSelected =
                IsViewSelected(SelectedTab.Changesets);

            bool isChangesetsSelected = mChangesetsTabButton.
                DrawTabButton(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ChangesetsViewTitle),
                    wasChangesetsSelected,
                    mTabButtonWidth);

            if (isChangesetsSelected)
                ShowChangesetsView();
        }

        void HistoryTabButtonGUI()
        {
            if (HistoryTab == null)
                return;

            bool wasHistorySelected =
                IsViewSelected(SelectedTab.History);

            bool isCloseButtonClicked;

            bool isHistorySelected = mHistoryTabButton.
                DrawClosableTabButton(
                    PlasticLocalization.GetString(PlasticLocalization.Name.FileHistory),
                    wasHistorySelected,
                    true,
                    mTabButtonWidth,
                    mParentWindow.Repaint,
                    out isCloseButtonClicked);

            if (isCloseButtonClicked)
            {
                CloseHistoryTab();
                return;
            }

            if (isHistorySelected)
                SetSelectedView(SelectedTab.History);
        }

        void BranchesTabButtonGUI()
        {
            if (BranchesTab == null)
                return;

            bool wasBranchesSelected =
                 IsViewSelected(SelectedTab.Branches);

            bool isCloseButtonClicked;
            
            bool isBranchesSelected = mBranchesTabButton.
                DrawClosableTabButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.Branches),
                    wasBranchesSelected,
                    true,
                    mTabButtonWidth,
                    mParentWindow.Repaint,
                    out isCloseButtonClicked);
                        
            if (isCloseButtonClicked)
            {
                CloseBranchesTab();
                return;
            }

            if (isBranchesSelected)
                SetSelectedView(SelectedTab.Branches);
        }

        internal enum SelectedTab
        {
            None = 0,
            PendingChanges = 1,
            IncomingChanges = 2,
            Changesets = 3,
            Branches = 4,
            History = 5
        }

        IIncomingChangesTab mIncomingChangesTab;

        SelectedTab mSelectedTab;
        SelectedTab mPreviousSelectedTab;

        float mTabButtonWidth = -1;

        TabButton mPendingChangesTabButton;
        TabButton mChangesetsTabButton;
        TabButton mIncomingChangesTabButton;
        TabButton mHistoryTabButton;
        TabButton mBranchesTabButton;

        WorkspaceWindow mWorkspaceWindow;

        readonly EditorWindow mParentWindow;
        readonly StatusBar mStatusBar;
        readonly IAssetStatusCache mAssetStatusCache;
        readonly IIncomingChangesNotifier mIncomingChangesNotifier;
        readonly GluonNewIncomingChangesUpdater mGluonNewIncomingChangesUpdater;
        readonly NewIncomingChangesUpdater mDeveloperNewIncomingChangesUpdater;
        readonly PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges mPendingChanges;
        readonly bool mIsGluonMode;
        readonly ViewHost mViewHost;
        readonly WorkspaceInfo mWkInfo;
    }
}