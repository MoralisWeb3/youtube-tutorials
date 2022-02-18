using UnityEngine;
using UnityEngine.UIElements;

using Codice.Client.Common;
using PlasticGui;
using PlasticGui.WebApi.Responses;
using PlasticGui.WebApi;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.UIElements;
using PlasticGui.Configuration.CloudEdition.Welcome;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class SignInPanel : VisualElement
    {
        internal SignInPanel(
            CloudEditionWelcomeWindow parentWindow,
            IPlasticWebRestApi restApi,
            CmConnection cmConnection)
        {
            mParentWindow = parentWindow;
            mRestApi = restApi;
            mCmConnection = cmConnection;

            InitializeLayoutAndStyles();

            BuildComponents();
        }

        internal void Dispose()
        {
            mSignInWithUnityIdButton.clicked -= SignInWithUnityIdButton_Clicked;
            mSignInWithEmailButton.clicked -= SignInWithEmailButton_Clicked;
            mPrivacyPolicyStatementButton.clicked -= PrivacyPolicyStatementButton_Clicked;

            if (mSignInWithEmailPanel != null)
                mSignInWithEmailPanel.Dispose();

            if (mWaitingSignInPanel != null)
                mWaitingSignInPanel.Dispose();
        }

        void SignInWithEmailButton_Clicked()
        {
            mSignInWithEmailPanel = new SignInWithEmailPanel(
                mParentWindow,
                mParentWindow,
                mRestApi);

            mParentWindow.ReplaceRootPanel(mSignInWithEmailPanel);
        }

        internal void SignInWithUnityIdButton_Clicked()
        {
            mWaitingSignInPanel = new WaitingSignInPanel(
                mParentWindow,
                mParentWindow,
                mRestApi,
                mCmConnection);

            mParentWindow.ReplaceRootPanel(mWaitingSignInPanel);
            mWaitingSignInPanel.OAuthSignInForConfigure(SsoProvider.UNITY_URL_ACTION);
        }
        internal void SignInWithUnityIdButtonAutoLogin()
        {
            mWaitingSignInPanel = new WaitingSignInPanel(
                mParentWindow,
                mParentWindow,
                mRestApi,
                mCmConnection);

            mParentWindow.ReplaceRootPanel(mWaitingSignInPanel);
        }

        void PrivacyPolicyStatementButton_Clicked()
        {
            // TODO: update when dll is avaiable PlasticGui.Configuration.CloudEdition.Welcome
            //       SignUp.PRIVACY_POLICY_URL
            Application.OpenURL(SignUp.PRIVACY_POLICY_URL);
        }

        void BuildComponents()
        {
            this.SetControlImage("iconUnity",
                Images.Name.ButtonSsoSignInUnity);

            mSignInWithUnityIdButton = this.Query<Button>("unityIDButton").First();
            mSignInWithUnityIdButton.text = PlasticLocalization.GetString(
                PlasticLocalization.Name.SignInWithUnityID);
            mSignInWithUnityIdButton.clicked += SignInWithUnityIdButton_Clicked;


            this.SetControlImage("iconEmail",
                Images.Name.ButtonSsoSignInEmail);

            mSignInWithEmailButton = this.Query<Button>("emailButton").First();
            mSignInWithEmailButton.text = PlasticLocalization.GetString(
                PlasticLocalization.Name.SignInWithEmail);
            mSignInWithEmailButton.clicked += SignInWithEmailButton_Clicked;

            this.SetControlText<Label>("privacyStatementText",
                PlasticLocalization.Name.PrivacyStatementText,
                PlasticLocalization.GetString(PlasticLocalization.Name.PrivacyStatement));

            mPrivacyPolicyStatementButton = this.Query<Button>("privacyStatement").First();
            mPrivacyPolicyStatementButton.text = PlasticLocalization.GetString(
                PlasticLocalization.Name.PrivacyStatement);
            mPrivacyPolicyStatementButton.clicked += PrivacyPolicyStatementButton_Clicked;
        }

        void InitializeLayoutAndStyles()
        {
            AddToClassList("grow");
            this.LoadLayout(typeof(SignInPanel).Name);

            this.LoadStyle("SignInSignUp");
            this.LoadStyle(typeof(SignInPanel).Name);
        }

        SignInWithEmailPanel mSignInWithEmailPanel;
        WaitingSignInPanel mWaitingSignInPanel;
        Button mSignInWithUnityIdButton;
        Button mSignInWithEmailButton;
        Button mPrivacyPolicyStatementButton;

        readonly CloudEditionWelcomeWindow mParentWindow;
        readonly IPlasticWebRestApi mRestApi;
        readonly CmConnection mCmConnection;
    }
}