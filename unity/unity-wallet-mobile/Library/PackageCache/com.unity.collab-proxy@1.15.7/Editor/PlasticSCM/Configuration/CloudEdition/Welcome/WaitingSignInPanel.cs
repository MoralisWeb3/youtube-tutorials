using Codice.Client.Common;

using PlasticGui;
using PlasticGui.Configuration.CloudEdition.Welcome;
using PlasticGui.WebApi;
using Unity.PlasticSCM.Editor.UI.UIElements;
using UnityEngine.UIElements;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class WaitingSignInPanel : VisualElement
    {
        internal WaitingSignInPanel(
            IWelcomeWindowNotify parentNotify,
            OAuthSignIn.INotify notify,
            IPlasticWebRestApi restApi,
            CmConnection cmConnection)
        {
            mParentNotify = parentNotify;

            mNotify = notify;
            mRestApi = restApi;
            mCmConnection = cmConnection;

            InitializeLayoutAndStyles();

            BuildComponents();
        }

        internal void OAuthSignInForConfigure(string ssoProviderName)
        {
            mSignIn = new OAuthSignIn();

            mSignIn.ForConfigure(
                mRestApi,
                ssoProviderName,
                mProgressControls,
                mNotify,
                mCmConnection);

            ShowWaitingSpinner();
        }

        internal void Dispose()
        {
            mCancelButton.clicked -= CancelButton_Clicked;
        }

        void CancelButton_Clicked()
        {
            mSignIn.Cancel();
            mParentNotify.Back();
        }

        void BuildComponents()
        {
            this.SetControlText<Label>("signInToPlasticSCM",
                PlasticLocalization.Name.SignInToPlasticSCM);

            this.SetControlText<Label>("completeSignInOnBrowser",
                PlasticLocalization.Name.CompleteSignInOnBrowser);

            mProgressContainer = this.Q<VisualElement>("progressContainer");

            mProgressControls = new UI.Progress.ProgressControlsForDialogs();

            mCancelButton = this.Query<Button>("cancelButton").First();
            mCancelButton.text = PlasticLocalization.GetString(
                PlasticLocalization.Name.CancelButton);
            mCancelButton.clicked += CancelButton_Clicked;
        }

        void InitializeLayoutAndStyles()
        {
            this.LoadLayout(typeof(WaitingSignInPanel).Name);

            this.LoadStyle("SignInSignUp");
            this.LoadStyle(typeof(WaitingSignInPanel).Name);
        }

        void ShowWaitingSpinner()
        {
            var spinner = new LoadingSpinner();
            mProgressContainer.Add(spinner);
            spinner.Start();

            var checkinMessageLabel = new Label(mProgressControls.ProgressData.ProgressMessage);
            checkinMessageLabel.style.paddingLeft = 10;
            mProgressContainer.Add(checkinMessageLabel);
        }

        Button mCancelButton;
        VisualElement mProgressContainer;

        OAuthSignIn mSignIn;

        UI.Progress.ProgressControlsForDialogs mProgressControls;

        readonly IPlasticWebRestApi mRestApi;
        readonly CmConnection mCmConnection;
        readonly OAuthSignIn.INotify mNotify;
        readonly IWelcomeWindowNotify mParentNotify;
    }
}