using System;
using System.Collections;
using System.Collections.Generic;

using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.Acl;
using Codice.Client.BaseCommands.BranchExplorer.ExplorerTree;
using Codice.Client.BaseCommands.BranchExplorer.Layout;
using Codice.Client.Commands;
using Codice.Client.Commands.CheckIn;
using Codice.Client.Commands.Tree;
using Codice.Client.Commands.WkTree;
using Codice.Client.Commands.Xlinks;
using Codice.Client.Common;
using Codice.Client.Common.Xlinks;
using Codice.Client.GameUI.Update;
using Codice.Client.IssueTracker;
using Codice.CM.Common;
using Codice.CM.Common.Merge;
using Codice.CM.Common.Replication;
using Codice.CM.Common.Serialization;
using Codice.CM.Common.Tree;
using PlasticGui;
using PlasticGui.SwitcherWindow.Workspaces;
using PlasticGui.WorkspaceWindow.BranchExplorer;
using PlasticGui.WorkspaceWindow.Diff;

namespace Unity.PlasticSCM.Tests.Editor.Mock
{
    internal class PlasticApiMock : IPlasticAPI
    {
        internal void SetupGetWorkspaceTreeNode(string path, WorkspaceTreeNode wkTreeNode)
        {
            mWorkspaceTreeNodes.Add(path, wkTreeNode);
        }

        internal void SetupGetWorkingBranch(BranchInfo workingBranch)
        {
            mWorkingBranch = workingBranch;
        }

        WkAddResult IPlasticAPI.Add(string[] paths, AddOptions options, out IList checkouts)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.AddLockRule(RepositorySpec repSpec, string newRule, bool bDryRun)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.ApplyLabelToWorkspace(WorkspaceInfo wkInfo, RepositorySpec repSpec, MarkerInfo labelInfo)
        {
            throw new NotImplementedException();
        }

        DiffInfo IPlasticAPI.BuildDiffInfoForDiffWithPrevious(string revspec2, string symbolicName2, string defaultPath2, string fileExt, WorkspaceInfo currentWk)
        {
            throw new NotImplementedException();
        }

        OutOfDateItems IPlasticAPI.CalculateOutOfDateItems(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        EnumRevisionType IPlasticAPI.ChangeRevisionType(string path, EnumRevisionType type)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.CheckAttributeExists(RepositorySpec repSpec, string attributeName)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.CheckBranchExists(RepositorySpec repSpec, BranchInfo brInfo)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.CheckCredentials(string server, SEIDWorkingMode mode, string user, string password)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.CheckLabelExists(RepositorySpec repSpec, MarkerInfo mkInfo)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.Checkout(string[] paths)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.Checkout(string[] paths, CheckoutModifiers options)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.CheckRepositoryExists(string repServer, string repName)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.CheckServerConnection(string repServer)
        {
            throw new NotImplementedException();
        }

        AttributeInfo IPlasticAPI.CreateAttribute(RepositorySpec repSpec, string attributeName, string attributeComment)
        {
            throw new NotImplementedException();
        }

        ICheckinOperation IPlasticAPI.CreateCheckInOperation()
        {
            throw new NotImplementedException();
        }

        BranchInfo IPlasticAPI.CreateChildBranch(WorkspaceInfo wkInfo, BranchInfo branchInfo, string title)
        {
            throw new NotImplementedException();
        }

        BranchInfo IPlasticAPI.CreateChildBranch(RepositorySpec repSpec, BranchInfo parentBranchInfo, string name, string comment)
        {
            throw new NotImplementedException();
        }

        BranchInfo IPlasticAPI.CreateChildBranchFromChangeset(RepositorySpec repSpec, BranchInfo parentBranchInfo, long changeset, string name)
        {
            throw new NotImplementedException();
        }

        BranchInfo IPlasticAPI.CreateChildBranchFromChangeset(RepositorySpec repSpec, BranchInfo parentBranchInfo, long changeset, string name, string comment)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.CreateComment(RepositorySpec repSpec, long reviewId, CodeReviewCommentInfo comment)
        {
            throw new NotImplementedException();
        }

        RepositoryInfo IPlasticAPI.CreateRepository(string repServer, string repName)
        {
            throw new NotImplementedException();
        }

        ReviewInfo IPlasticAPI.CreateReview(RepositorySpec repSpec, ReviewInfo reviewInfo)
        {
            throw new NotImplementedException();
        }

        WorkspaceInfo IPlasticAPI.CreateWorkspace(string wkPath, string wkName, string repName)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.DeleteAttributeFromObject(RepositorySpec repSpec, AttributeRealizationInfo attribute)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.DeleteBranch(WorkspaceInfo wkInfo, RepositorySpec repSpec, BranchInfo branchInfo)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.DeleteChangeset(RepositorySpec repSpec, ChangesetInfo changesetInfo)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.DeleteComment(RepositorySpec repSpec, long commentId)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.DeleteControlled(string path, DeleteModifiers options)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.DeleteLabel(RepositorySpec repSpec, MarkerInfo labelInfo)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.DeleteReview(RepositorySpec repSpec, ReviewInfo reviewInfo)
        {
            throw new NotImplementedException();
        }

        IList IPlasticAPI.DoGetAllRepositories(string server, bool bFilterDeleted)
        {
            throw new NotImplementedException();
        }

        IList IPlasticAPI.FindAttributeTypes(RepositorySpec repSpec)
        {
            throw new NotImplementedException();
        }

        HashSet<Guid> IPlasticAPI.FindGuids(RepositorySpec repSpec, WorkspaceInfo wkInfo, string condition, ObjectType objectType)
        {
            throw new NotImplementedException();
        }

        QueryResult IPlasticAPI.FindQuery(WorkspaceInfo wkInfo, string query)
        {
            throw new NotImplementedException();
        }

        QueryResult IPlasticAPI.FindQuery(RepositoryInfo repInfo, string query)
        {
            throw new NotImplementedException();
        }

        QueryResult IPlasticAPI.FindQuery(RepositorySpec repSpec, string query)
        {
            throw new NotImplementedException();
        }

        DiffInfo IPlasticAPI.GetAddedDiffInfo(WorkspaceInfo wkInfo, MountPoint mount, DiffViewEntry entry, WorkspaceInfo currentWk)
        {
            throw new NotImplementedException();
        }

        IList IPlasticAPI.GetAllRepositories(bool bFilterDeleted)
        {
            throw new NotImplementedException();
        }

        IList IPlasticAPI.GetAllRepositories(string server, bool bFilterDeleted)
        {
            throw new NotImplementedException();
        }

        IList IPlasticAPI.GetAllRepositories(string server, bool bFilterDeleted, ServerProfile profile)
        {
            throw new NotImplementedException();
        }

        List<WorkspaceGuiEntry> IPlasticAPI.GetAllWorkspaces()
        {
            throw new NotImplementedException();
        }

        WorkspaceInfo[] IPlasticAPI.GetAllWorkspacesArray()
        {
            throw new NotImplementedException();
        }

        List<AnnotatedLine> IPlasticAPI.GetAnnotations(RepositorySpec repSpec, RevisionInfo revInfo, string localPath, string comparisonMethod, string encoding)
        {
            throw new NotImplementedException();
        }

        AttributeInfo IPlasticAPI.GetAttribute(string server, RepId repId, string attributeName)
        {
            throw new NotImplementedException();
        }

        long IPlasticAPI.GetAttributeIdForRepository(RepositorySpec repSpec, string attributeName)
        {
            throw new NotImplementedException();
        }

        AttributeRealizationInfo[] IPlasticAPI.GetAttributeRealizations(RepositorySpec repSpec, long objId)
        {
            throw new NotImplementedException();
        }

        List<ClientDiff> IPlasticAPI.GetBranchDifferences(RepositorySpec repSpec, BranchInfo brInfo)
        {
            throw new NotImplementedException();
        }

        BrExLayout IPlasticAPI.GetBranchExplorerLayout(WorkspaceInfo wkInfo, RepositorySpec repSpec, FilterCollection filters, DisplayOptions options, out BrExTree explorerTree)
        {
            throw new NotImplementedException();
        }

        BrExLayout IPlasticAPI.GetBranchExplorerLayout(WorkspaceInfo wkInfo, RepositoryExplainMergeData explainMergeData, DisplayOptions displayOptions, out BrExTree explorerTree)
        {
            throw new NotImplementedException();
        }

        BrExLayout IPlasticAPI.GetBranchExplorerLayout(RepositorySpec repSpec, BrExTree explorerTree, DisplayOptions options)
        {
            throw new NotImplementedException();
        }

        BranchInfo IPlasticAPI.GetBranchInfo(RepositorySpec repSpec, string fullBranchName)
        {
            throw new NotImplementedException();
        }

        BranchInfo IPlasticAPI.GetBranchInfo(RepositorySpec repSpec, Guid brId)
        {
            throw new NotImplementedException();
        }

        List<BranchInfo> IPlasticAPI.GetBranchInfos(RepositorySpec repSpec, List<long> ids)
        {
            throw new NotImplementedException();
        }

        Difference IPlasticAPI.GetChangedForMovedDifference(Difference diff)
        {
            throw new NotImplementedException();
        }

        TreeChangedNode IPlasticAPI.GetChangedNodeForPath(WorkspaceInfo wkInfo, string path)
        {
            throw new NotImplementedException();
        }

        IList<ChangeInfo> IPlasticAPI.GetChanges(WorkspaceInfo wkInfo, List<string> basePaths, WorkspaceStatusOptions options)
        {
            throw new NotImplementedException();
        }

        List<ClientDiff> IPlasticAPI.GetChangesetDifferences(RepositorySpec repSpec, ChangesetInfo csetInfo)
        {
            throw new NotImplementedException();
        }

        List<ClientDiff> IPlasticAPI.GetChangesetDifferences(MountPointWithPath mount, ChangesetInfo csetInfo)
        {
            throw new NotImplementedException();
        }

        Guid IPlasticAPI.GetChangesetGuid(RepositorySpec repSpec, long changesetId)
        {
            throw new NotImplementedException();
        }

        ChangesetInfo IPlasticAPI.GetChangesetInfo(RepositorySpec repSpec, Guid csetId)
        {
            throw new NotImplementedException();
        }

        ChangesetInfo IPlasticAPI.GetChangesetInfoFromId(WorkspaceInfo wkInfo, long csId)
        {
            throw new NotImplementedException();
        }

        ChangesetInfo IPlasticAPI.GetChangesetInfoFromId(RepositorySpec repSpec, long csId)
        {
            throw new NotImplementedException();
        }

        ChangesetInfo IPlasticAPI.GetChangesetInfoFromId(RepositoryInfo repInfo, long csId)
        {
            throw new NotImplementedException();
        }

        List<ClientDiff> IPlasticAPI.GetChangesetsDifferences(RepositorySpec repSpec, ChangesetInfo sourceCsetInfo, ChangesetInfo destinationCsetInfo)
        {
            throw new NotImplementedException();
        }

        long IPlasticAPI.GetCurrentChangesetOnWorkspace(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        DiffInfo IPlasticAPI.GetDiffInfo(string revSpec, string symbolicName, string defaultPath, string fileExtension, WorkspaceInfo currrentWk)
        {
            throw new NotImplementedException();
        }

        DiffInfo IPlasticAPI.GetDiffInfo(string revspec1, string revspec2, string symbolicName1, string symbolicName2, string defaultPath1, string defaultPath2, string fileExtension, WorkspaceInfo currrentWk)
        {
            throw new NotImplementedException();
        }

        DiffInfo IPlasticAPI.GetDiffInfo(WorkspaceInfo wkInfo, ChangeInfo changeInfo)
        {
            throw new NotImplementedException();
        }

        DiffInfo IPlasticAPI.GetDiffInfo(WorkspaceInfo wkInfo, ChangeInfo changeInfo, ChangeInfo changedForMoved)
        {
            throw new NotImplementedException();
        }

        DiffInfo IPlasticAPI.GetDiffInfo(WorkspaceInfo wkInfo, MountPoint mount, Difference diff)
        {
            throw new NotImplementedException();
        }

        DiffInfo IPlasticAPI.GetDiffInfo(WorkspaceInfo wkInfo, RepositorySpec repSpec, RevisionInfo leftRev, RevisionInfo rightRev, string leftDefaultPath, string rightDefaultPath)
        {
            throw new NotImplementedException();
        }

        List<RepObjectInfo> IPlasticAPI.GetHistory(RepositorySpec repSpec, long itemId)
        {
            throw new NotImplementedException();
        }

        HumanReadableXlinkDataGenerator IPlasticAPI.GetHumanReadableXlinkDataGenerator()
        {
            throw new NotImplementedException();
        }

        IDictionary<MountPoint, PendingMergeLink> IPlasticAPI.GetInProgressMergeLinks(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        LicenseData IPlasticAPI.GetLicenseData(string server)
        {
            throw new NotImplementedException();
        }

        long IPlasticAPI.GetLoadedChangeset(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        long IPlasticAPI.GetLoadedRevisionId(WorkspaceInfo wkInfo, RepositorySpec repSpec, long itemId)
        {
            throw new NotImplementedException();
        }

        LockRule IPlasticAPI.GetLockRule(RepositorySpec repSpec)
        {
            throw new NotImplementedException();
        }

        BranchInfo IPlasticAPI.GetMainBranch(RepositorySpec repSpec)
        {
            throw new NotImplementedException();
        }

        MarkerInfo IPlasticAPI.GetMarkerInfo(RepositorySpec repSpec, long markerId)
        {
            throw new NotImplementedException();
        }

        Guid IPlasticAPI.GetObjectGuid(RepositorySpec repSpec, long objectId)
        {
            throw new NotImplementedException();
        }

        long IPlasticAPI.GetParentChangeset(RepositorySpec repSpec, long changesetId)
        {
            throw new NotImplementedException();
        }

        RevisionInfo IPlasticAPI.GetParentRevision(RepositorySpec repSpec, RevisionInfo revInfo)
        {
            throw new NotImplementedException();
        }

        IDictionary<MountPoint, IList<PendingMergeLink>> IPlasticAPI.GetPendingMergeLinks(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        IList<ReplicationSourceInfo> IPlasticAPI.GetReplicationSources(RepositorySpec repSpec, RepObjectInfo repObject)
        {
            throw new NotImplementedException();
        }

        RepositoryInfo IPlasticAPI.GetRepositoryInfo(RepositorySpec repSpec)
        {
            throw new NotImplementedException();
        }

        RepositorySpec IPlasticAPI.GetRepositorySpec(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        IList IPlasticAPI.GetReviewComments(RepositorySpec repSpec, long reviewId)
        {
            throw new NotImplementedException();
        }

        IList IPlasticAPI.GetReviewCommentsFromReviewIds(RepositorySpec repSpec, List<long> reviewIds)
        {
            throw new NotImplementedException();
        }

        RevisionInfo IPlasticAPI.GetRevisionAtChangeset(RepositorySpec repSpec, long itemId, long changesetId)
        {
            throw new NotImplementedException();
        }

        ChildItem[] IPlasticAPI.GetRevisionChildren(RepositorySpec repSpec, RevisionInfo revInfo)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.GetRevisionDataToFile(RepositorySpec repSpec, HistoryRevision revision, string tmpFile)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.GetRevisionDataToFile(RepositorySpec repSpec, RevisionInfo revInfo, string tmpFile)
        {
            throw new NotImplementedException();
        }

        RevisionInfo IPlasticAPI.GetRevisionInfo(RepositorySpec repSpec, long revisionId)
        {
            throw new NotImplementedException();
        }

        RepositoryInfo IPlasticAPI.GetRootRepositoryInfo(string clientPath)
        {
            throw new NotImplementedException();
        }

        SEID IPlasticAPI.GetSeidFromName(string server, string name, bool isGroup)
        {
            throw new NotImplementedException();
        }

        SelectorInformation IPlasticAPI.GetSelectorUserInformation(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

		WebadminAddress IPlasticAPI.GetServerWebPortAndProtocol(string server)
        {
            throw new NotImplementedException();
        }
		
        ReplicationLogEntryInfo[] IPlasticAPI.GetSortedReplicationLogsFromBranch(RepositorySpec repSpec, BranchInfo branchInfo)
        {
            throw new NotImplementedException();
        }

        ReplicationLogEntryInfo[] IPlasticAPI.GetSortedReplicationLogsFromRepository(RepositorySpec repSpec)
        {
            throw new NotImplementedException();
        }

        TreeContent IPlasticAPI.GetTreeContent(RepositorySpec repSpec, long changesetId)
        {
            throw new NotImplementedException();
        }

        string IPlasticAPI.GetUserName(string server, SEID owner)
        {
            throw new NotImplementedException();
        }

        IList IPlasticAPI.GetValuesForAttribute(string server, RepId repId, string attributeName)
        {
            throw new NotImplementedException();
        }

        BranchInfo IPlasticAPI.GetWorkingBranch(WorkspaceInfo wkInfo)
        {
            return mWorkingBranch;
        }

        BranchInfo IPlasticAPI.GetCheckoutBranch(WorkspaceInfo wkInfo)
        {
            return mWorkingBranch;
        }

        long IPlasticAPI.GetWorkingChangeset(RepositorySpec repSpec, WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        SEIDWorkingMode IPlasticAPI.GetWorkingMode(string server)
        {
            throw new NotImplementedException();
        }

        WorkspaceInfo IPlasticAPI.GetWorkspaceFromPath(string wkPath)
        {
            throw new NotImplementedException();
        }

        WorkspaceTreeNode IPlasticAPI.GetWorkspaceTree(WorkspaceInfo wkInfo, string path)
        {
            throw new NotImplementedException();
        }

        WorkspaceTreeNode IPlasticAPI.GetWorkspaceTreeNode(string path)
        {
            if (!mWorkspaceTreeNodes.ContainsKey(path))
                return null;

            return mWorkspaceTreeNodes[path];
        }

        List<ClientXlink> IPlasticAPI.GetXLinksInChangesetTree(RepositorySpec repSpec, long changesetId)
        {
            throw new NotImplementedException();
        }

        string IPlasticAPI.GetXlinkTypeString(Xlink xlink)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.HasWorkspaceMergeChanges(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.IsBranchEmpty(RepositorySpec repSpec, long branchId)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.IsCopied(string path, out bool bReplaced)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.IsFsReaderWatchLimitReached(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.IsGluonWorkspace(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.IsIncomingChangesInProgress(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.IsMovedChangedDifference(Difference diff)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.IsOnChangedTree(WorkspaceInfo wkInfo, string path)
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.IsReadOnlyFilesPreferenceEnabled()
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.IsStackTraceEnabled()
        {
            throw new NotImplementedException();
        }

        bool IPlasticAPI.IsWorkingChangesetDeleted(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.LaunchDifferences(DiffInfo diffInfo, IToolLauncher xDiffLauncher)
        {
            throw new NotImplementedException();
        }

        List<PlasticTask> IPlasticAPI.LoadTasks(IPlasticIssueTrackerExtension extension, List<string> taskIds)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.MarkTaskAsOpen(IPlasticIssueTrackerExtension extension, List<string> taskIds, string assignee)
        {
            throw new NotImplementedException();
        }

        BranchInfo IPlasticAPI.MkBranch(RepositorySpec repSpec, string fullBranchName, long changeset, string comment)
        {
            throw new NotImplementedException();
        }

        MarkerInfo IPlasticAPI.MkLabel(RepositorySpec repSpec, string labelName, long changeset, string comment)
        {
            throw new NotImplementedException();
        }

        IList IPlasticAPI.MkLabelInAllXlinkedRepositories(IList<ClientXlink> targetXlinks, string labelName, string comment)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.Move(string source, string destination, MoveModifiers options)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.MoveChangeset(RepositorySpec repSpec, ChangesetInfo csetInfo, BranchInfo dstBrInfo)
        {
            throw new NotImplementedException();
        }

        List<ErrorMessage> IPlasticAPI.PartialUpdate(WorkspaceInfo wkInfo, List<string> paths)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.PerformUpdateMerge(WorkspaceInfo wkInfo, MergeSource mergeSource, MergeResult mergeResult, ICmdNotifier notifier)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.RemoveRepository(RepositoryInfo repInfo)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.RemoveWorkspace(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        BranchInfo IPlasticAPI.RenameBranch(WorkspaceInfo wkInfo, RepositorySpec repSpec, BranchInfo branchInfo, string newName)
        {
            throw new NotImplementedException();
        }

        MarkerInfo IPlasticAPI.RenameLabel(WorkspaceInfo wkInfo, RepositorySpec repSpec, MarkerInfo labelInfo, string newName)
        {
            throw new NotImplementedException();
        }

        RepositoryInfo IPlasticAPI.RenameRepository(RepositoryInfo repInfo, string newName)
        {
            throw new NotImplementedException();
        }

        WorkspaceInfo IPlasticAPI.RenameWorkspace(WorkspaceInfo wkInfo, string newName)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.RevertToClientDiffRevisions(WorkspaceInfo wkInfo, ClientDiffInfo clientDiff, WorkspaceTreeNode wkNode, string workspacePath)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.RevertToThisRevision(WorkspaceInfo wkInfo, RepositorySpec repSpec, HistoryRevision revision, string path)
        {
            throw new NotImplementedException();
        }

        List<ChangeInfo> IPlasticAPI.SearchMatches(WorkspaceInfo wkInfo, string path, double allowedChangesPerUnit)
        {
            throw new NotImplementedException();
        }

        IList IPlasticAPI.SelectiveUpdate(WorkspaceInfo wkInfo, UpdateFlags flags, IList dirs, IList files)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.SetAsGluonWorkspace(WorkspaceInfo wkInfo)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.SetAttribute(RepositorySpec repSpec, long attId, long objId, string attributeValue)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.SetReadonlyFilesPreference(bool value)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.SetSelector(WorkspaceInfo wkInfo, string selector, SetSelectorFlags flags)
        {
            throw new NotImplementedException();
        }

        CheckinResult IPlasticAPI.ShelveChanges(string[] paths, string comment)
        {
            throw new NotImplementedException();
        }

        IUpdateResult IPlasticAPI.SwitchToBranch(WorkspaceInfo wkInfo, RepositorySpec repSpec, BranchInfo brInfo, IContinueWithPendingChangesQuestioner questioner, ICmdNotifier notifier)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.SwitchToBranch(WorkspaceInfo wkInfo, BranchInfo branchInfo)
        {
            throw new NotImplementedException();
        }

        IUpdateResult IPlasticAPI.SwitchToChangeset(WorkspaceInfo wkInfo, RepositorySpec repSpec, BranchInfo brInfo, ChangesetInfo csInfo, IContinueWithPendingChangesQuestioner questioner, ICmdNotifier notifier)
        {
            throw new NotImplementedException();
        }

        IUpdateResult IPlasticAPI.SwitchToLabel(WorkspaceInfo wkInfo, RepositorySpec repSpec, MarkerInfo labelInfo, IContinueWithPendingChangesQuestioner questioner, ICmdNotifier notifier)
        {
            throw new NotImplementedException();
        }

        string IPlasticAPI.SwitchWorkspaceSelector(WorkspaceInfo wkInfo, SwitchToSelectorEntry[] entries)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.UndeleteClientDiff(ClientDiffInfo diff, string restorePath)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.UndeleteRepository(RepositoryInfo repInfo)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.UndeleteRevision(RepositorySpec repSpec, RemovedRealizationInfo removed, string restorePath)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.UndoCheckout(WorkspaceInfo wkInfo, string[] paths, ICmdNotifier notifier)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.UndoCheckout(WorkspaceInfo wkInfo, IList<string> paths, IList<string> skippedLocks, bool bHandleDeletedChangeset)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.UndoShelvedChanges(WorkspaceInfo wkInfo, string[] paths)
        {
            throw new NotImplementedException();
        }

        List<string> IPlasticAPI.UndoUnchanged(WorkspaceInfo wkInfo, IList paths)
        {
            throw new NotImplementedException();
        }

        IUpdateResult IPlasticAPI.Update(string path, UpdateFlags flags, IRunMergeDuringUpdate mergeController, ICmdNotifier notifier)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.UpdateBranchHead(RepositorySpec repSpec, long branchId, long changesetId)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.UpdateCheckoutBranch(WorkspaceInfo wkInfo, BranchInfo branchInfo)
        {
            throw new NotImplementedException();
        }

        ReviewCommentInfo IPlasticAPI.UpdateComment(RepositorySpec repSpec, ReviewCommentInfo editedComment)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.UpdateObjectComment(RepositorySpec repSpec, RepObjectInfo repObject, string newComment)
        {
            throw new NotImplementedException();
        }

        ReviewInfo IPlasticAPI.UpdateReview(RepositorySpec repSpec, ReviewInfo reviewInfo)
        {
            throw new NotImplementedException();
        }

        IUpdateResult IPlasticAPI.UpdateToLatest(WorkspaceInfo wkInfo, UpdateFlags flags, IRunMergeDuringUpdate mergeController, ICmdNotifier notifier)
        {
            throw new NotImplementedException();
        }

        SecurityMember[] IPlasticAPI.GetUsers(string server, string serverSideFilter)
        {
            throw new NotImplementedException();
        }

        SecurityMember[] IPlasticAPI.GetGroups(string server, string serverSideFilter)
        {
            throw new NotImplementedException();
        }

        SecurityMember[] IPlasticAPI.GetSecurityMembers(string server, string serverSideFilter)
        {
            throw new NotImplementedException();
        }

        SecurityMember IPlasticAPI.GetSecurityMemberFromName(string server, string name)
        {
            throw new NotImplementedException();
        }

        WorkspaceInfo IPlasticAPI.CreateDynamicWorkspace(string wkPath, string wkName, string repName)
        {
            throw new NotImplementedException();
        }

        RepositoryServerInfo IPlasticAPI.GetRepositoryServerInfo(string server)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.UpdateSecuredPath(RepositorySpec repSpec, SecuredPathInfo secPathInfo, long[] newBrIds, AclParams aclParams)
        {
            throw new NotImplementedException();
        }

        SecuredPathInfo IPlasticAPI.CreateSecuredPath(RepositorySpec repSpec, string path, string tag, long[] brIds, AclParams aclParams)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.DeleteSecuredPath(RepositorySpec repSpec, SecuredPathInfo secPathInfo)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.CalculateAcl(string server, ObjectInfo obj, out AclInfo aclInfo, out AclEntry[] calculatedPermissions, out bool bIsOwner)
        {
            throw new NotImplementedException();
        }

        SEID IPlasticAPI.GetOwner(string server, ObjectInfo obj)
        {
            throw new NotImplementedException();
        }

        IList<SecuredPathInfo> IPlasticAPI.GetSecuredPaths(string server, RepId repId, string path, string tag, LongArray brIds)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.SetPermissions(string server, ObjectInfo obj, SEID seid, Permissions granted, Permissions denied, Permissions overrideGranted, Permissions overrideDenied)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.RemovePermissions(string server, ObjectInfo obj, SEID seid)
        {
            throw new NotImplementedException();
        }

        IList<SecuredPathInfo> IPlasticAPI.GetAllSecuredPaths(RepositorySpec repSpec)
        {
            throw new NotImplementedException();
        }

        IList<SecuredPathInfo> IPlasticAPI.GetSecuredPaths(RepositorySpec repSpec, string path)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.DownloadFileFromPath(List<ConfiguredPath> filePaths, WorkspaceInfo wkInfo, UpdateProgress updateProgress)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.SaveToken(string server, string user, string token)
        {
            throw new NotImplementedException();
        }

        void IPlasticAPI.SaveProfile(
            string server, SEIDWorkingMode workingMode, string user, string password)
        {
            throw new NotImplementedException();
        }

        ReviewInfo IPlasticAPI.GetReview(RepositorySpec repSpec, long reviewId)
        {
            throw new NotImplementedException();
        }

        BranchInfo mWorkingBranch;
        Dictionary<string, WorkspaceTreeNode> mWorkspaceTreeNodes =
            new Dictionary<string, WorkspaceTreeNode>();
    }
}
