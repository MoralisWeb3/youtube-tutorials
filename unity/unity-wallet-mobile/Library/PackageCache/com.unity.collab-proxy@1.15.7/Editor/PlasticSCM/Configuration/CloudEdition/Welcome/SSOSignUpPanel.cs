using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using Codice.Client.Common;
using PlasticGui;
using PlasticGui.WebApi;
using PlasticGui.Configuration.CloudEdition;
using PlasticGui.Configuration.CloudEdition.Welcome;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.UIElements;
using PlasticGui.WebApi.Responses;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class SSOSignUpPanel :
        VisualElement,
        SignUp.INotify
    {
        internal SSOSignUpPanel(
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

        internal void SetSignUpData(
            string user,
            string password)
        {
            mUserNameTextField.value = user;
            mPasswordTextField.value = password;

            CleanNotificationLabels();
        }

        internal void Dispose()
        {
            mSignUpButton.clicked -= SignUpButton_Clicked;
            mTermsOfServiceButton.clicked -= TermsOfServiceButton_Clicked;
            mPrivacyPolicyButton.clicked -= PrivacyPolicyButton_Clicked;
            mPrivacyPolicyStatementButton.clicked -= PrivacyPolicyStatementButton_Clicked;

            mUserNameTextField.UnregisterValueChangedCallback(
                UserNameTextBox_TextChanged);
            mPasswordTextField.UnregisterValueChangedCallback(
                PasswordTextBox_TextChanged);
            mConfirmPasswordTextField.UnregisterValueChangedCallback(
                ConfirmPasswordTextBox_TextChanged);
        }

        void UserNameTextBox_TextChanged(ChangeEvent<string> evt)
        {
            CleanNotification(mUserNotificationLabel);
        }

        void PasswordTextBox_TextChanged(ChangeEvent<string> evt)
        {
            CleanNotification(mPasswordNotificationLabel);
        }

        void ConfirmPasswordTextBox_TextChanged(ChangeEvent<string> evt)
        {
            CleanNotification(mConfirmPasswordNotificationLabel);
        }

        void SignUpButton_Clicked()
        {
            CleanNotificationLabels();

            SignUp.Run(
                mRestApi,
                new SaveCloudEditionCreds(),
                new SignUp.Data(
                    mUserNameTextField.text,
                    mPasswordTextField.text,
                    mConfirmPasswordTextField.text,
                    false),
                mProgressControls,
                this);
        }

        void SignUpWithUnityButton_clicked()
        {
            mWaitingSignInPanel = new WaitingSignInPanel(
                mParentWindow,
                mParentWindow,
                mRestApi,
                mCmConnection);

            mParentWindow.ReplaceRootPanel(mWaitingSignInPanel);
            mWaitingSignInPanel.OAuthSignInForConfigure(SsoProvider.UNITY_URL_ACTION);
        }

        void TermsOfServiceButton_Clicked()
        {
            Application.OpenURL(SignUp.TERMS_OF_SERVICE_URL);
        }

        void PrivacyPolicyButton_Clicked()
        {
            Application.OpenURL(SignUp.PRIVACY_POLICY_URL);
        }

        void PrivacyPolicyStatementButton_Clicked()
        {
            Application.OpenURL(SignUp.PRIVACY_POLICY_URL);
        }

        void BuildComponents()
        {

            mUserNameTextField = this.Q<TextField>("emailField");
            mUserNameTextField.label = PlasticLocalization.GetString(
                PlasticLocalization.Name.Email);
            mUserNameTextField.RegisterValueChangedCallback(
                UserNameTextBox_TextChanged);

            mUserNotificationLabel = this.Q<Label>("emailNotification");

            mPasswordTextField = this.Q<TextField>("passwordField");
            mPasswordTextField.label = PlasticLocalization.GetString(
                PlasticLocalization.Name.Password);
            mPasswordTextField.RegisterValueChangedCallback(
                PasswordTextBox_TextChanged);

            mConfirmPasswordTextField = this.Q<TextField>("confirmPasswordField");
            mConfirmPasswordTextField.label = PlasticLocalization.GetString(
                PlasticLocalization.Name.ConfirmPassword);
            mConfirmPasswordTextField.RegisterValueChangedCallback(
                ConfirmPasswordTextBox_TextChanged);

            mPasswordNotificationLabel = this.Q<Label>("passwordNotificationLabel");
            mConfirmPasswordNotificationLabel = this.Q<Label>("confirmPasswordNotificationLabel");

            mSignUpButton = this.Q<Button>("signUp");
            mSignUpButton.text = PlasticLocalization.GetString(PlasticLocalization.Name.SignUp);
            mSignUpButton.clicked += SignUpButton_Clicked;

            string[] signUpText = PlasticLocalization.GetString(
                PlasticLocalization.Name.SignUpAgreeToShort).Split('{', '}');
            Label signUpAgreePt1 = this.Q<Label>("signUpAgreePt1");
            signUpAgreePt1.text = signUpText[0];

            mTermsOfServiceButton = this.Q<Button>("termsOfService");
            mTermsOfServiceButton.text = PlasticLocalization.GetString(PlasticLocalization.Name.TermsOfService);
            mTermsOfServiceButton.clicked += TermsOfServiceButton_Clicked;

            Label signUpAgreePt2 = this.Q<Label>("signUpAgreePt2");
            signUpAgreePt2.text = signUpText[2];

            mPrivacyPolicyButton = this.Q<Button>("privacyPolicy");
            mPrivacyPolicyButton.text = PlasticLocalization.GetString(PlasticLocalization.Name.PrivacyPolicy);
            mPrivacyPolicyButton.clicked += PrivacyPolicyButton_Clicked;

            this.SetControlImage("unityIcon", Images.Name.ButtonSsoSignInUnity);

            mSignUpWithUnityButton = this.Q<Button>("unityIDButton");
            mSignUpWithUnityButton.text = PlasticLocalization.GetString(PlasticLocalization.Name.SignInWithUnityID);
            mSignUpWithUnityButton.clicked += SignUpWithUnityButton_clicked;

            this.SetControlText<Label>("privacyStatementText",
                PlasticLocalization.Name.PrivacyStatementText,
                PlasticLocalization.GetString(PlasticLocalization.Name.PrivacyStatement));

            mPrivacyPolicyStatementButton = this.Q<Button>("privacyStatement");
            mPrivacyPolicyStatementButton.text = PlasticLocalization.GetString(
                PlasticLocalization.Name.PrivacyStatement);
            mPrivacyPolicyStatementButton.clicked += PrivacyPolicyStatementButton_Clicked;

            // TODO: add controls to disable and disable control logic
            mProgressControls = new ProgressControlsForDialogs(new VisualElement[] { mSignUpButton, mSignUpWithUnityButton });
            mProgressContainer = this.Q<VisualElement>("progressContainer");
            mProgressContainer.Add((VisualElement)mProgressControls);
        }

        void InitializeLayoutAndStyles()
        {
            AddToClassList("grow");

            this.LoadLayout(typeof(SSOSignUpPanel).Name);

            this.LoadStyle("SignInSignUp");
            this.LoadStyle(typeof(SSOSignUpPanel).Name);
        }

        void CleanNotificationLabels()
        {
            CleanNotification(mUserNotificationLabel);
            CleanNotification(mPasswordNotificationLabel);
            CleanNotification(mConfirmPasswordNotificationLabel);
        }

        static void ShowNotification(Label label, string text)
        {
            label.text = text;
            label.RemoveFromClassList("hidden");
        }

        static void CleanNotification(Label label)
        {
            label.text = "";
            label.AddToClassList("hidden");
        }

        void SignUp.INotify.Success(List<string> organizations, bool canCreateAnOrganization)
        {
            mParentWindow.ShowOrganizationPanel(
                PlasticLocalization.GetString(PlasticLocalization.Name.SignUp),
                organizations,
                canCreateAnOrganization);
        }

        void SignUp.INotify.ValidationFailed(SignUp.ValidationResult validationResult)
        {
            if (validationResult.UserError != null)
                ShowNotification(mUserNotificationLabel, validationResult.UserError);

            if (validationResult.ClearPasswordError != null)
                ShowNotification(mPasswordNotificationLabel, validationResult.ClearPasswordError);

            if (validationResult.ClearPasswordConfirmationError != null)
                ShowNotification(mConfirmPasswordNotificationLabel, validationResult.ClearPasswordConfirmationError);
        }

        void SignUp.INotify.LoginNeeded(Login.Data loginData, string message)
        {
            Debug.Log("LoginNeeded");
            throw new NotImplementedException();
            //mWelcomeForm.SwitchToLoginPage(
            //    loginData.User, loginData.ClearPassword, message);
        }

        void SignUp.INotify.Error(string message)
        {
            mProgressControls.ShowError(message);
        }

        Button mTermsOfServiceButton;
        Button mPrivacyPolicyButton;
        Button mPrivacyPolicyStatementButton;
        TextField mUserNameTextField;
        TextField mPasswordTextField;
        TextField mConfirmPasswordTextField;
        Label mUserNotificationLabel;
        Label mPasswordNotificationLabel;
        Label mConfirmPasswordNotificationLabel;
        Button mSignUpButton;
        Button mSignUpWithUnityButton;
        VisualElement mProgressContainer;
        IProgressControls mProgressControls;
        WaitingSignInPanel mWaitingSignInPanel;

        readonly CloudEditionWelcomeWindow mParentWindow;
        readonly IPlasticWebRestApi mRestApi;
        readonly CmConnection mCmConnection;

        //TODO: remove this once Google sign up functionality is added
        const bool hideGoogleSignUpButton = true;
    }
}