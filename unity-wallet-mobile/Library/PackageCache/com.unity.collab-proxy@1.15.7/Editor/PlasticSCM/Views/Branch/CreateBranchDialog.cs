using UnityEditor;
using UnityEngine;

using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.QueryViews.Branches;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;

namespace Unity.PlasticSCM.Editor.Views.Branches
{
    class CreateBranchDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 710, 380);
            }
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(PlasticLocalization.Name.CreateChildBranchTitle);
        }

        protected override void OnModalGUI()
        {
            DoTitleArea();

            DoFieldsArea();

            DrawProgressForDialogs.For(
               mProgressControls.ProgressData);

            GUILayout.Space(10);

            DoButtonsArea();
        }

        internal static BranchCreationData CreateBranchFromLastParentBranchChangeset(
            EditorWindow parentWindow,
            RepositorySpec repSpec,
            BranchInfo parentBranchInfo )
        {
            string changesetStr = PlasticLocalization.GetString(
                PlasticLocalization.Name.LastChangeset);

            string explanation = BranchCreationUserInfo.GetFromObjectString(
                repSpec, parentBranchInfo, changesetStr);

            CreateBranchDialog dialog = Create(repSpec, parentBranchInfo, explanation);
            ResponseType dialogueResult = dialog.RunModal(parentWindow);

            BranchCreationData result = dialog.BuildCreationData();
            result.Result = dialogueResult == ResponseType.Ok;
            return result;
        }

        void DoTitleArea()
        {
            GUILayout.BeginVertical();

            Title(PlasticLocalization.GetString(PlasticLocalization.Name.CreateChildBranchTitle));

            GUILayout.Space(5);

            Paragraph(string.Format("{0} {1}", PlasticLocalization.GetString(
                PlasticLocalization.Name.CreateChildBranchExplanation), mExplanation));

            GUILayout.Space(20);

            GUILayout.EndVertical();
        }

        void DoFieldsArea()
        {
            GUILayout.BeginVertical();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(
                    PlasticLocalization.GetString(PlasticLocalization.Name.BranchNameEntry),
                    GUILayout.Width(100));
                mNewBranchName = GUILayout.TextField(mNewBranchName);
                GUILayout.Space(5);
            }

            GUILayout.Space(5);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(100)))
                {
                    GUILayout.Space(49);
                    GUILayout.Label(
                        PlasticLocalization.GetString(PlasticLocalization.Name.CommentsEntry),
                        GUILayout.Width(100));
                }
                mComment = GUILayout.TextArea(mComment, GUILayout.Height(100));
                GUILayout.Space(5);
            }

            GUILayout.Space(20);

            mSwitchToBranch = GUILayout.Toggle(mSwitchToBranch, PlasticLocalization.GetString(PlasticLocalization.Name.SwitchToBranchCheckButton));

            GUILayout.Space(10);

            GUILayout.EndVertical();
        }

        void DoButtonsArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                DoCreateButton();
                DoCancelButton();
            }
        }

        void DoCancelButton()
        {
            if (NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CancelButton)))
            {
                CancelButtonAction();
            }
        }

        void DoCreateButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(PlasticLocalization.Name.CreateButton)))
                return;

            BranchCreationValidation.AsyncValidation(
                BuildCreationData(), this, mProgressControls);
        }

        static CreateBranchDialog Create(RepositorySpec repSpec, BranchInfo parentBranchInfo, string explanation)
        {
            var instance = CreateInstance<CreateBranchDialog>();
            instance.IsResizable = false;
            instance.mEscapeKeyAction = instance.CloseButtonAction;
            instance.mRepositorySpec = repSpec;
            instance.mParentBranchInfo = parentBranchInfo;
            instance.mNewBranchName = "";
            instance.mComment = "";
            instance.mSwitchToBranch = true;
            instance.mProgressControls = new ProgressControlsForDialogs();
            instance.mExplanation = explanation;
            return instance;
        }

        BranchCreationData BuildCreationData()
        {
            return new BranchCreationData(
                mRepositorySpec,
                mParentBranchInfo,
                mParentBranchInfo.Changeset,
                mNewBranchName,
                mComment,
                null,
                mSwitchToBranch);
        }

        ProgressControlsForDialogs mProgressControls;

        RepositorySpec mRepositorySpec;
        BranchInfo mParentBranchInfo;

        string mNewBranchName;
        string mComment;
        bool mSwitchToBranch;
        string mExplanation;
    }
}