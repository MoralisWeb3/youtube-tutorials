using System.Collections.Generic;
using System.Threading.Tasks;

using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.EventTracking;
using Codice.CM.Common;
using GluonGui.WorkspaceWindow.Views.Checkin.Operations;
using PlasticGui;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal partial class PendingChangesTab
    {
        void CheckinForMode(WorkspaceInfo wkInfo, bool isGluonMode, bool keepItemsLocked)
        {
            TrackFeatureUseEvent.For(
                PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
                isGluonMode ?
                    TrackFeatureUseEvent.Features.PartialCheckin :
                    TrackFeatureUseEvent.Features.Checkin);

            if (isGluonMode)
            {
                PartialCheckin(keepItemsLocked);
                return;
            }

            Checkin();
        }

        internal void UndoForMode(WorkspaceInfo wkInfo, bool isGluonMode)
        {
            TrackFeatureUseEvent.For(
                PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
                    isGluonMode ?
                    TrackFeatureUseEvent.Features.PartialUndo :
                    TrackFeatureUseEvent.Features.Undo);

            if (isGluonMode)
            {
                PartialUndo();
                return;
            }

            Undo();
        }

        void UndoChangesForMode(
            bool isGluonMode,
            List<ChangeInfo> changesToUndo,
            List<ChangeInfo> dependenciesCandidates)
        {
            if (isGluonMode)
            {
                PartialUndoChanges(changesToUndo, dependenciesCandidates);
                return;
            }

            UndoChanges(changesToUndo, dependenciesCandidates);
        }

        void PartialCheckin(bool keepItemsLocked)
        {
            List<ChangeInfo> changesToCheckin;
            List<ChangeInfo> dependenciesCandidates;

            mPendingChangesTreeView.GetCheckedChanges(
                false,
                out changesToCheckin,
                out dependenciesCandidates);

            if (CheckEmptyOperation(changesToCheckin))
            {
                ((IProgressControls)mProgressControls).ShowWarning(
                    PlasticLocalization.GetString(PlasticLocalization.Name.NoItemsAreSelected));
                return;
            }

            bool isCancelled;
            SaveAssets.ForChangesWithConfirmation(changesToCheckin, out isCancelled);

            if (isCancelled)
                return;

            CheckinUIOperation ciOperation = new CheckinUIOperation(
                mWkInfo, mViewHost, mProgressControls, mGuiMessage,
                new LaunchCheckinConflictsDialog(mParentWindow),
                new LaunchDependenciesDialog(
                    PlasticLocalization.GetString(PlasticLocalization.Name.CheckinButton),
                    mParentWindow),
                this, mWorkspaceWindow.GluonProgressOperationHandler);

            ciOperation.Checkin(
                changesToCheckin,
                dependenciesCandidates,
                CommentText,
                keepItemsLocked,
                EndCheckin);
        }

        void Checkin()
        {
            List<ChangeInfo> changesToCheckin;
            List<ChangeInfo> dependenciesCandidates;

            mPendingChangesTreeView.GetCheckedChanges(
                false, out changesToCheckin, out dependenciesCandidates);

            if (CheckEmptyOperation(changesToCheckin, HasPendingMergeLinks()))
            {
                ((IProgressControls)mProgressControls).ShowWarning(
                    PlasticLocalization.GetString(PlasticLocalization.Name.NoItemsAreSelected));
                return;
            }

            bool isCancelled;
            SaveAssets.ForChangesWithConfirmation(changesToCheckin, out isCancelled);

            if (isCancelled)
                return;

            mPendingChangesOperations.Checkin(
                changesToCheckin,
                dependenciesCandidates,
                CommentText,
                null,
                EndCheckin);
        }

        void PartialUndo()
        {
            List<ChangeInfo> changesToUndo;
            List<ChangeInfo> dependenciesCandidates;

            mPendingChangesTreeView.GetCheckedChanges(
                true, out changesToUndo, out dependenciesCandidates);

            PartialUndoChanges(changesToUndo, dependenciesCandidates);
        }

        void Undo()
        {
            List<ChangeInfo> changesToUndo;
            List<ChangeInfo> dependenciesCandidates;

            mPendingChangesTreeView.GetCheckedChanges(
                true, out changesToUndo, out dependenciesCandidates);

            UndoChanges(changesToUndo, dependenciesCandidates);
        }

        void PartialUndoChanges(
            List<ChangeInfo> changesToUndo,
            List<ChangeInfo> dependenciesCandidates)
        {
            if (CheckEmptyOperation(changesToUndo))
            {
                ((IProgressControls)mProgressControls).ShowWarning(
                    PlasticLocalization.GetString(PlasticLocalization.Name.NoItemsToUndo));
                return;
            }

            SaveAssets.ForChangesWithoutConfirmation(changesToUndo);

            UndoUIOperation undoOperation = new UndoUIOperation(
                mWkInfo, mViewHost,
                new LaunchDependenciesDialog(
                    PlasticLocalization.GetString(PlasticLocalization.Name.UndoButton),
                    mParentWindow),
                mProgressControls, mGuiMessage);

            undoOperation.Undo(
                changesToUndo,
                dependenciesCandidates,
                RefreshAsset.UnityAssetDatabase);
        }

        void UndoChanges(
            List<ChangeInfo> changesToUndo,
            List<ChangeInfo> dependenciesCandidates)
        {
            if (CheckEmptyOperation(changesToUndo, HasPendingMergeLinks()))
            {
                ((IProgressControls)mProgressControls).ShowWarning(
                    PlasticLocalization.GetString(PlasticLocalization.Name.NoItemsToUndo));
                return;
            }

            SaveAssets.ForChangesWithoutConfirmation(changesToUndo);

            mPendingChangesOperations.Undo(
                changesToUndo,
                dependenciesCandidates,
                mPendingMergeLinks.Count,
                RefreshAsset.UnityAssetDatabase);
        }

        void EndCheckin()
        {
            ShowCheckinSuccess();

            RefreshAsset.UnityAssetDatabase();
        }

        void ShowCheckinSuccess()
        {
            bool isTreeViewEmpty = mPendingChangesTreeView.GetCheckedItemCount() ==
                mPendingChangesTreeView.GetTotalItemCount();

            if (isTreeViewEmpty)
            {
                mIsCheckedInSuccessful = true;
                mCooldownClearCheckinSuccessAction.Ping();
                return;
            }

            mStatusBar.Notify(
                PlasticLocalization.GetString(PlasticLocalization.Name.CheckinCompleted), 
                UnityEditor.MessageType.None, 
                Images.Name.StepOk);
        }

        void DelayedClearCheckinSuccess()
        {
            mIsCheckedInSuccessful = false;
        }

        static bool CheckEmptyOperation(List<ChangeInfo> elements)
        {
            return elements == null || elements.Count == 0;
        }

        static bool CheckEmptyOperation(List<ChangeInfo> elements, bool bHasPendingMergeLinks)
        {
            if (bHasPendingMergeLinks)
                return false;

            if (elements != null && elements.Count > 0)
                return false;

            return true;
        }
    }
}
