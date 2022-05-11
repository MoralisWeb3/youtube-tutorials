using UnityEditor;
using UnityEngine;

using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.PendingChanges;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs
{
    internal class CheckinMergeNeededDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 650, 390);
            }
        }
        internal static CheckinMergeNeededData Merge(
            WorkspaceInfo wkInfo,
            EditorWindow parentWindow)
        {
            RepositorySpec repSpec = PlasticGui.Plastic.API.GetRepositorySpec(wkInfo);
            BranchInfo parentBranchInfo = PlasticGui.Plastic.API.GetWorkingBranch(wkInfo);

            CheckinMergeNeededDialog dialog = Create(
                repSpec, parentBranchInfo,
                new ProgressControlsForDialogs());

            ResponseType dialogResult = dialog.RunModal(parentWindow);

            CheckinMergeNeededData result = new CheckinMergeNeededData(
                repSpec, parentBranchInfo,
                dialog.mMergeNow, dialog.mChildBranchName);

            result.Result = dialogResult == ResponseType.Ok;
            return result;
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(
                PlasticLocalization.Name.CheckinMergeRequest));

            Paragraph(PlasticLocalization.GetString(
                PlasticLocalization.Name.CheckinMergeRequestMessage));

            Paragraph(PlasticLocalization.GetString(
                PlasticLocalization.Name.CheckinMergeRequestQuestion));

            DoMergeNowArea();

            DoMergeLaterArea();

            GUILayout.Space(10);

            DrawProgressForDialogs.For(
                mProgressControls.ProgressData);

            GUILayout.Space(10);

            DoButtonsArea();

            mProgressControls.ForcedUpdateProgress(this);
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.CheckinMergeTitle);
        }

        void DoMergeNowArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(20);
                EditorGUI.BeginChangeCheck();
                bool mergeNowChecked = RadioToggle(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CheckinMergeNow), mMergeNow);
                if (EditorGUI.EndChangeCheck() && mergeNowChecked)
                {
                    mMergeNow = mergeNowChecked; // Just check
                }
                GUILayout.FlexibleSpace();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(45);
                Paragraph(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CheckinMergeNowMessage));
            }
        }

        void DoMergeLaterArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(20);
                EditorGUI.BeginChangeCheck();
                bool mergeLaterChecked = RadioToggle(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CheckinMergeLater), !mMergeNow);
                if (EditorGUI.EndChangeCheck() && mergeLaterChecked)
                {
                    mMergeNow = !mergeLaterChecked; // Just uncheck
                }
                GUILayout.FlexibleSpace();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(45);
                Paragraph(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CheckinMergeLaterMessage,
                    mCurrentBranchInfo.BranchName));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(60);
                GUILayout.Label(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CheckinMergeBranch), UnityStyles.Paragraph);
                GUILayout.Space(-10);
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Space(6);

                    GUI.enabled = !mMergeNow;

                    mChildBranchName = EditorGUILayout.TextField(
                        string.Empty, mChildBranchName);

                    GUI.enabled = true;
                }
            }
        }

        void DoButtonsArea()
        {
            using (new EditorGUI.DisabledScope(
                mProgressControls.ProgressData.IsWaitingAsyncResult))
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    DoMergeButton();
                    DoCancelButton();
                    return;
                }

                DoCancelButton();
                DoMergeButton();
            }
        }

        void DoMergeButton()
        {
            if (!AcceptButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.MergeButton)))
                return;

            MergeButtonAction();
        }

        void DoCancelButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CancelButton)))
                return;

            CancelButtonAction();
        }

        void MergeButtonAction()
        {
            CheckinMergeNeededData data = new CheckinMergeNeededData(
                mRepSpec, mCurrentBranchInfo, mMergeNow, mChildBranchName);

            CheckinMergeNeededValidation.AsyncValidation(
                data, this, mProgressControls);
        }

        bool RadioToggle(string text, bool isOn)
        {
            var rect = GUILayoutUtility.GetRect(
                new GUIContent(text), UnityStyles.Dialog.RadioToggle);

            bool isOnAfter = EditorGUI.Toggle(
                rect, isOn, UnityStyles.Dialog.RadioToggle);

            GUI.Label(
                new Rect(
                    rect.x + rect.height, rect.y,
                    rect.xMax - rect.height, rect.height),
                text, UnityStyles.Paragraph);

            return isOnAfter;
        }

        static CheckinMergeNeededDialog Create(
            RepositorySpec repSpec,
            BranchInfo currentBranchInfo,
            ProgressControlsForDialogs progressControls)
        {
            var instance = CreateInstance<CheckinMergeNeededDialog>();
            instance.mRepSpec = repSpec;
            instance.mCurrentBranchInfo = currentBranchInfo;
            instance.mProgressControls = progressControls;
            instance.mEnterKeyAction = instance.MergeButtonAction;
            instance.mEscapeKeyAction = instance.CancelButtonAction;
            return instance;
        }

        [SerializeField]
        bool mMergeNow = true;
        [SerializeField]
        string mChildBranchName = DEFAULT_BRANCH_NAME;

        ProgressControlsForDialogs mProgressControls;
        BranchInfo mCurrentBranchInfo;
        RepositorySpec mRepSpec;

        const string DEFAULT_BRANCH_NAME = "task000";
    }
}
