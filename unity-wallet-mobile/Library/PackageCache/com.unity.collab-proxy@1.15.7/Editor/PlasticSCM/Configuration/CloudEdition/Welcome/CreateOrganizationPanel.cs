using System.Collections.Generic;
using System.Linq;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.UIElements;
using PlasticGui.WebApi;
using PlasticGui.Configuration.CloudEdition.Welcome;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class CreateOrganizationPanel :
        VisualElement,
        IsValidOrganization.INotify,
        GetDatacenters.INotify,
        CreateOrganization.INotify
    {
        internal CreateOrganizationPanel(
            CloudEditionWelcomeWindow parentWindow,
            VisualElement parentPanel,
            IPlasticWebRestApi restApi)
        {
            mParentWindow = parentWindow;
            mParentPanel = parentPanel;
            mRestApi = restApi;

            InitializeLayoutAndStyles();

            BuildComponents();

            var progressControls = new ProgressControlsForDialogs(new VisualElement[] { mCreateButton, mBackButton });
            mProgressControls = progressControls;
            mGettingDatacentersProgressContainer.Add(progressControls);

            GetDatacenters.Run(
                mRestApi, mProgressControls, this);
        }

        internal void Dispose()
        {
            mLoadingSpinner.Dispose();

            mCreateButton.clicked -= CreateButton_Clicked;

            mOrganizationNameTextField.UnregisterValueChangedCallback(
                OnOrganizationNameTextFieldChanged);
        }

        void OnOrganizationNameTextFieldChanged(ChangeEvent<string> evt)
        {
            mOrganizationNameNotification.text = "";
        }

        void DataCenterClicked(DropdownMenuAction action)
        {
            mSelectedDatacenter = action.name;
            mDatacenter.text = action.name;
        }

        void DisplayAlert(Label label, string alert, Color color, bool display)
        {
            label.text = alert;
            label.style.color = color;
            if (alert.Length > 130)
                label.style.marginBottom = 30;
            else
                label.style.marginBottom = 5;

            if (display)
                label.RemoveFromClassList("hidden");
            else
                label.AddToClassList("hidden");
        }

        void GetDatacenters.INotify.Success(
            List<string> datacenters, int defaultDatacenter)
        {
            mSelectedDatacenter = datacenters.FirstOrDefault();
            mDatacenter = new ToolbarMenu { text = mSelectedDatacenter };
            foreach (string datacenter in datacenters)
                mDatacenter.menu.AppendAction(datacenter, DataCenterClicked, DataCenterActive);
            VisualElement datacenterContainer = this.Q<VisualElement>("datacenter");
            datacenterContainer.Add(mDatacenter);
        }

        void GetDatacenters.INotify.Error(string message)
        {
            DisplayAlert(mDataCenterRetryAlert, message, Color.red, true);
            mDataCenterRetryButton.RemoveFromClassList("hidden");
        }

        void IsValidOrganization.INotify.OrganizationAvailable(string organizationName)
        {
            if (organizationName != mOrganizationNameTextField.text)
                return;

            DisplayAlert(
                mOrganizationNameNotification,
                PlasticLocalization.GetString(PlasticLocalization.Name.Available),
                Color.green,
                true);
        }

        void IsValidOrganization.INotify.OrganizationUnavailable(string organizationName)
        {
            if (organizationName != mOrganizationNameTextField.text)
                return;

            DisplayAlert(
                mOrganizationNameNotification,
                PlasticLocalization.GetString(PlasticLocalization.Name.Unavailable),
                Color.red,
                true);
        }

        void IsValidOrganization.INotify.ValidationFailed(string validationResult)
        {
            DisplayAlert(
                mOrganizationNameNotification,
                validationResult,
                Color.red,
                true);
        }

        void IsValidOrganization.INotify.Error(string organizationName, string message)
        {
            if (organizationName != mOrganizationNameTextField.text)
                return;

            DisplayAlert(
                mOrganizationNameNotification,
                message,
                Color.red,
                true);
        }

        void CreateOrganization.INotify.Success(string organizationName)
        {
            mParentWindow.ReplaceRootPanel(new CreatedOrganizationPanel(
                organizationName,
                mParentWindow));
        }

        void CreateOrganization.INotify.ValidationFailed(
            CreateOrganization.ValidationResult validationResult)
        {
            if (validationResult.OrganizationError != null)
                DisplayAlert(
                    mOrganizationNameNotification,
                    validationResult.OrganizationError,
                    Color.red,
                    true);

            if (validationResult.DatacenterError != null)
                DisplayAlert(
                    mOrganizationNameNotification,
                    validationResult.DatacenterError,
                    Color.red,
                    true);
        }

        void CreateOrganization.INotify.OrganizationUnavailable()
        {
            DisplayAlert(
                mOrganizationNameNotification,
                PlasticLocalization.GetString(PlasticLocalization.Name.Unavailable),
                Color.red,
                true);
        }

        void CreateOrganization.INotify.Error(string message)
        {
            mProgressControls.ShowError(message);
        }

        void OrganizationNameTextBox_DelayedTextChanged()
        {
            IsValidOrganization.Run(
                mRestApi, mOrganizationNameTextField.text, this);
        }

        void GetDatacentersRetryButton_Click()
        {
            mDataCenterRetryContainer.AddToClassList("hidden");
            GetDatacenters.Run(mRestApi, mProgressControls, this);
        }

        void EncryptionLinkLabel_LinkClicked()
        {
            Application.OpenURL(CreateOrganization.ENCRYPTION_LEARN_MORE_URL);
        }

        void BackButton_Clicked()
        {
            mParentWindow.ReplaceRootPanel(mParentPanel);
        }

        void CreateButton_Clicked()
        {
            CreateOrganization.Run(
                mRestApi,
                new CreateOrganization.Data(
                    mOrganizationNameTextField.text,
                    mDatacenter.text,
                    mEncryptData.value),
                mProgressControls,
                this);
        }

        DropdownMenuAction.Status DataCenterActive(DropdownMenuAction action)
        {
            if (action.name == mSelectedDatacenter)
                return DropdownMenuAction.Status.Checked;

            return DropdownMenuAction.Status.Normal;
        }

        void BuildComponents()
        {
            mOrganizationNameTextField = this.Q<TextField>("orgName");
            mOrganizationNameTextField.RegisterValueChangedCallback(
                x => OrganizationNameTextBox_DelayedTextChanged());
            mOrganizationNameNotification = this.Q<Label>("orgNameNotification");

            mDataCenterRetryContainer = this.Q<VisualElement>("dataCenterRetryContainer");
            mDataCenterRetryAlert = this.Q<Label>("dataCenterRetryAlert");
            mDataCenterRetryButton = this.Q<Button>("dataCenterRetryButton");
            mDataCenterRetryButton.clicked += GetDatacentersRetryButton_Click;

            mBackButton = this.Q<Button>("back");
            mBackButton.clicked += BackButton_Clicked;

            mCreateButton = this.Q<Button>("create");
            mCreateButton.clicked += CreateButton_Clicked;

            mEncryptData = this.Q<Toggle>("encryptData");
            mEncryptLearnMoreButton = this.Q<Button>("encryptLearnMore");
            mEncryptLearnMoreButton.clicked += EncryptionLinkLabel_LinkClicked;

            mGettingDatacentersProgressContainer = this.Q<VisualElement>("gettingDatacenters");

            mOrganizationNameTextField.RegisterValueChangedCallback(
                OnOrganizationNameTextFieldChanged);
            mOrganizationNameTextField.FocusOnceLoaded();

            this.SetControlText<Label>("createLabel",
                PlasticLocalization.Name.CreateOrganizationTitle);
            this.SetControlLabel<TextField>("orgName",
                PlasticLocalization.Name.OrganizationName);
            this.SetControlText<Label>("datacenterLabel",
                PlasticLocalization.Name.Datacenter);
            this.SetControlText<Toggle>("encryptData",
                PlasticLocalization.Name.EncryptionCheckButton);
            this.SetControlText<Label>("encryptExplanation",
                PlasticLocalization.Name.EncryptionCheckButtonExplanation, "");
            this.SetControlText<Button>("encryptLearnMore",
                PlasticLocalization.Name.LearnMore);
            this.SetControlText<Button>("back",
                PlasticLocalization.Name.BackButton);
            this.SetControlText<Button>("create",
                PlasticLocalization.Name.CreateButton);
        }

        void InitializeLayoutAndStyles()
        {
            this.LoadLayout(typeof(CreateOrganizationPanel).Name);

            this.LoadStyle("SignInSignUp");
            this.LoadStyle(typeof(CreateOrganizationPanel).Name);
        }

        CloudEditionWelcomeWindow mParentWindow;
        VisualElement mParentPanel;

        TextField mOrganizationNameTextField;
        Label mOrganizationNameNotification;

        VisualElement mDataCenterRetryContainer;
        Label mDataCenterRetryAlert;
        Button mDataCenterRetryButton;

        ToolbarMenu mDatacenter;
        Button mBackButton;
        Button mCreateButton;
        Button mEncryptLearnMoreButton;
        VisualElement mGettingDatacentersProgressContainer;
        LoadingSpinner mLoadingSpinner;
        string mSelectedDatacenter;
        Toggle mEncryptData;

        IProgressControls mProgressControls;
        readonly IPlasticWebRestApi mRestApi;
    }
}