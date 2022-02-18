using UnityEngine.UIElements;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.UIElements;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class CreatedOrganizationPanel : VisualElement
    {
        internal CreatedOrganizationPanel(string organizationName, CloudEditionWelcomeWindow parentWindow)
        {
            mOrganizationName = organizationName;
            mParentWindow = parentWindow;

            InitializeLayoutAndStyles();

            BuildComponents();
        }

        void BuildComponents()
        {
            this.SetControlText<Label>("createdTitle",
                PlasticLocalization.Name.CreatedOrganizationTitle);
            this.SetControlText<Label>("createdExplanation",
                PlasticLocalization.Name.CreatedOrganizationExplanation, mOrganizationName);

            this.Q<Button>("continue").clicked += ContinueButton_Clicked;
            this.SetControlText<Button>("continue",
                PlasticLocalization.Name.ContinueButton);
        }

        void InitializeLayoutAndStyles()
        {
            this.LoadLayout(typeof(CreatedOrganizationPanel).Name);

            this.LoadStyle("SignInSignUp");
            this.LoadStyle(typeof(CreatedOrganizationPanel).Name);
        }

        void ContinueButton_Clicked()
        {
            mParentWindow.JoinOrganizationAndWelcomePage(mOrganizationName);
            mParentWindow.Close();
        }

        internal void Dispose()
        {
            this.Q<Button>("continue").clicked -= ContinueButton_Clicked;
        }

        string mOrganizationName;
        CloudEditionWelcomeWindow mParentWindow;
    } 
}