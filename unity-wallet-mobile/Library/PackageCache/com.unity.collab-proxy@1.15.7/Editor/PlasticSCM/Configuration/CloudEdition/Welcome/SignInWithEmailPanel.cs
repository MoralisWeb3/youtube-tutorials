using System.Collections.Generic;
using Unity.PlasticSCM.Editor.UI.UIElements;
using UnityEngine.UIElements;

using PlasticGui;
using PlasticGui.Configuration.CloudEdition.Welcome;
using PlasticGui.Configuration.CloudEdition;
using PlasticGui.WebApi;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class SignInWithEmailPanel :
        VisualElement,
        Login.INotify
    {
        internal SignInWithEmailPanel(
            CloudEditionWelcomeWindow parentWindow,
            IWelcomeWindowNotify notify,
            IPlasticWebRestApi restApi)
        {
            mParentWindow = parentWindow;
            mNotify = notify;
            mRestApi = restApi;

            InitializeLayoutAndStyles();

            BuildComponents();
        }

        internal void Dispose()
        {
            mSignInButton.clicked -= SignInButton_Clicked;
            mBackButton.clicked -= BackButton_Clicked;
        }

        void SignInButton_Clicked()
        {
            CleanNotificationLabels();

            Login.Run(
                mRestApi,
                new SaveCloudEditionCreds(),
                mEmailField.text,
                mPasswordField.text,
                string.Empty,
                Login.Mode.Configure,
                mProgressControls,
                this);
        }

        void BackButton_Clicked()
        {
            mNotify.Back();
        }

        void Login.INotify.SuccessForConfigure(
            List<string> organizations,
            bool canCreateAnOrganization)
        {
            mNotify.SuccessForConfigure(
                organizations,
                canCreateAnOrganization);
        }

        void Login.INotify.SuccessForSSO(
            string organization)
        {
            mNotify.SuccessForSSO(organization);
        }

        void Login.INotify.SuccessForProfile(
            string userName)
        {
            mNotify.SuccessForProfile(userName);
        }

        void Login.INotify.SuccessForCredentials(string userName, string password)
        {
            mNotify.SuccessForCredentials(userName, password);
        }

        void Login.INotify.ValidationFailed(
            Login.ValidationResult validationResult)
        {
            if (validationResult.UserError != null)
            {
                mEmailNotificationLabel.text = validationResult.UserError;
            }

            if (validationResult.PasswordError != null)
            {
                mPasswordNotificationLabel.text = validationResult.PasswordError;
            }
        }

        void Login.INotify.SignUpNeeded(
            Login.Data loginData)
        {
            mNotify.SignUpNeeded(loginData.User, loginData.ClearPassword);
        }

        void Login.INotify.Error(
            string message)
        {
            mProgressControls.ShowError(message);
        }

        void CleanNotificationLabels()
        {
            mEmailNotificationLabel.text = string.Empty;
            mPasswordNotificationLabel.text = string.Empty;
        }

        void BuildComponents()
        {
            mEmailField = this.Q<TextField>("email");
            mPasswordField = this.Q<TextField>("password");
            mEmailNotificationLabel = this.Q<Label>("emailNotification");
            mPasswordNotificationLabel = this.Q<Label>("passwordNotification");
            mSignInButton = this.Q<Button>("signIn");
            mBackButton = this.Q<Button>("back");
            mProgressContainer = this.Q<VisualElement>("progressContainer");

            mSignInButton.clicked += SignInButton_Clicked;
            mBackButton.clicked += BackButton_Clicked;
            mEmailField.FocusOnceLoaded();

            mProgressControls = new ProgressControlsForDialogs(new VisualElement[] { mSignInButton });
            mProgressContainer.Add((VisualElement)mProgressControls);

            this.SetControlText<Label>("signInLabel",
                PlasticLocalization.Name.SignInWithEmail);
            this.SetControlLabel<TextField>("email",
                PlasticLocalization.Name.Email);
            this.SetControlLabel<TextField>("password",
                PlasticLocalization.Name.Password);
            this.SetControlText<Button>("signIn",
                PlasticLocalization.Name.SignIn);
            this.SetControlText<Button>("back",
                PlasticLocalization.Name.BackButton);
        }

        void InitializeLayoutAndStyles()
        {
            this.LoadLayout(typeof(SignInWithEmailPanel).Name);

            this.LoadStyle("SignInSignUp");
            this.LoadStyle(typeof(SignInWithEmailPanel).Name);
        }

        TextField mEmailField;
        TextField mPasswordField;

        Label mEmailNotificationLabel;
        Label mPasswordNotificationLabel;

        Button mSignInButton;
        Button mBackButton;

        VisualElement mProgressContainer;

        IProgressControls mProgressControls;

        readonly CloudEditionWelcomeWindow mParentWindow;
        readonly IWelcomeWindowNotify mNotify;
        readonly IPlasticWebRestApi mRestApi;
    }
}