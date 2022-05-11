using System.Collections.Generic;
using System.Linq;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.UIElements;
using PlasticGui.WebApi;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class OrganizationPanel : VisualElement
    {
        internal OrganizationPanel(
            CloudEditionWelcomeWindow parentWindow,
            IPlasticWebRestApi restApi,
            string title,
            List<string> organizations, 
            bool canCreateAnOrganization)
        {
            mParentWindow = parentWindow;
            mRestApi = restApi;
            mOrganizations = organizations;

            InitializeLayoutAndStyles();

            BuildComponents(title, canCreateAnOrganization);
        }

        void BuildComponents(string title, bool canCreateAnOrganization)
        {
            mParentWindow.titleContent = new UnityEngine.GUIContent(title);

            this.SetControlText<Label>("confirmationMessage",
                PlasticLocalization.Name.SignedUpTitle);

            if (mOrganizations.Count == 1)
            {
                BuildSingleOrganizationSection(mOrganizations.First());
                mJoinSingleOrganizationButton = this.Q<Button>("joinSingleOrganizationButton");
                mJoinSingleOrganizationButton.clicked += JoinOrganizationButton_clicked;
            }
            else if (mOrganizations.Count > 1)
            {
                BuildMultipleOrganizationsSection(mOrganizations);
                mJoinMultipleOrganizationsButton = this.Q<Button>("joinMultipleOrganizationsButton");
                mJoinMultipleOrganizationsButton.clicked += JoinOrganizationButton_clicked;
                mOrganizationToJoin = mOrganizations.First();
            }

            if (canCreateAnOrganization)
            {
                BuildCreateOrganizationSection(!mOrganizations.Any());

                mCreateOrganizationButton = this.Q<Button>("createOrganizationButton");
                mCreateOrganizationButton.clicked += CreateOrganizationButton_Clicked;
            }
        }

        internal void Dispose()
        {
            mParentWindow.CancelJoinOrganization();

            if (mJoinSingleOrganizationButton != null)
                mJoinSingleOrganizationButton.clicked -= JoinOrganizationButton_clicked;

            if (mJoinMultipleOrganizationsButton != null)
                mJoinMultipleOrganizationsButton.clicked -= JoinOrganizationButton_clicked;

            if (mCreateOrganizationButton != null)
                mCreateOrganizationButton.clicked -= CreateOrganizationButton_Clicked;
        }

        private void JoinOrganizationButton_clicked()
        {
            mParentWindow.JoinOrganizationAndWelcomePage(mOrganizationToJoin);

            // TODO: Closing the window for now. Need to connect this event to the main on boarding
            //       workflow.
            mParentWindow.Close();
        }

        private void CreateOrganizationButton_Clicked()
        {
            mParentWindow.ReplaceRootPanel(new CreateOrganizationPanel(mParentWindow, this, mRestApi));
        }

        void BuildSingleOrganizationSection(string organizationName)
        {
            mOrganizationToJoin = organizationName;

            this.Query<VisualElement>("joinSingleOrganization").First().RemoveFromClassList("display-none");

            this.SetControlText<Label>("joinSingleOrganizationLabel",
                PlasticLocalization.Name.YouBelongToOrganization, organizationName);

            this.SetControlText<Button>("joinSingleOrganizationButton",
                PlasticLocalization.Name.JoinButton);
        }

        void BuildMultipleOrganizationsSection(List<string> organizationNames)
        {
            this.Query<VisualElement>("joinMultipleOrganizations").First().RemoveFromClassList("display-none");

            this.SetControlText<Label>("joinMultipleOrganizationsLabel",
                PlasticLocalization.Name.YouBelongToSeveralOrganizations);

            VisualElement organizationDropdown = this.Query<VisualElement>("organizationDropdown").First();
            ToolbarMenu toolbarMenu = new ToolbarMenu
            {
                text = organizationNames.FirstOrDefault()
            };
            foreach (string name in organizationNames)
            {
                toolbarMenu.menu.AppendAction(name, x => 
                {
                    toolbarMenu.text = name;
                    mOrganizationToJoin = name;
                }, DropdownMenuAction.AlwaysEnabled);
                organizationDropdown.Add(toolbarMenu);
            }

            this.SetControlText<Button>("joinMultipleOrganizationsButton",
                PlasticLocalization.Name.JoinButton);
        }

        void BuildCreateOrganizationSection(bool firstOrganization)
        {
            this.Query<VisualElement>("createOrganization").First().RemoveFromClassList("display-none");

            PlasticLocalization.Name createOrganizationLabelName = firstOrganization ?
                PlasticLocalization.Name.CreateFirstOrganizationLabel :
                PlasticLocalization.Name.CreateOtherOrganizationLabel;

            this.SetControlText<Label>("createOrganizationLabel",
                createOrganizationLabelName);

            this.SetControlText<Button>("createOrganizationButton",
                PlasticLocalization.Name.CreateButton);
        }

        void InitializeLayoutAndStyles()
        {
            this.LoadLayout(typeof(OrganizationPanel).Name);

            this.LoadStyle("SignInSignUp");
            this.LoadStyle(typeof(OrganizationPanel).Name);
        }

        List<string> mOrganizations;

        Button mJoinSingleOrganizationButton = null;
        Button mJoinMultipleOrganizationsButton = null;
        Button mCreateOrganizationButton = null;
        public string mOrganizationToJoin = "";

        readonly CloudEditionWelcomeWindow mParentWindow;
        readonly IPlasticWebRestApi mRestApi;
    }
}
