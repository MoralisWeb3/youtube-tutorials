using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.EditorGameService.SettingsProviderVisuals
{
    class EnabledVisuals : IVisuals
    {
#if ENABLE_EDITOR_GAME_SERVICES
        const string k_StorageUrl = "https://dashboard.unity3d.com/organizations/{0}/projects/{1}/collaborate/usage";
#else
        const string k_StorageUrl = "https://core.cloud.unity3d.com/orgs/{0}/projects/{1}/usage";
#endif

        public VisualElement GetVisuals()
        {
            VisualElement containerUI = null;

            var containerAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Uxml.Path.Enabled);
            if (containerAsset != null)
            {
                containerUI = containerAsset.CloneTree().contentContainer;

                SetupHistorySection(containerUI);
                SetupPublishSection(containerUI);
                SetupCloudStorage(containerUI);
                LearnMoreVisualHelper.SetupLearnMore(containerUI);
            }

            return containerUI;
        }

        void SetupPublishSection(VisualElement containerUI)
        {
            var publishSection = containerUI.Q(Uxml.Node.PublishSection);
            if (publishSection != null)
            {
                publishSection.style.display = DisplayStyle.Flex;
            }

            var openChangesButton = containerUI.Q<Button>(Uxml.Node.OpenChangesLink);
            if (openChangesButton != null)
            {
                openChangesButton.clicked += ShowChangesWindow;
            }
        }

        static void ShowChangesWindow()
        {
            EditorGameServiceAnalyticsSender.SendProjectSettingsOpenChangesEvent();
            CollaborateWindow.Init(CollaborateWindow.FocusTarget.Changes);
        }

        void SetupHistorySection(VisualElement containerUI)
        {
            var historySection = containerUI.Q(Uxml.Node.HistorySection);
            if (historySection != null)
            {
                historySection.style.display = DisplayStyle.Flex;
            }

            var openHistoryButton = containerUI.Q<Button>(Uxml.Node.OpenHistoryLink);
            if (openHistoryButton != null)
            {
                openHistoryButton.clicked += ShowHistoryWindow;
            }
        }

        static void ShowHistoryWindow()
        {
            EditorGameServiceAnalyticsSender.SendProjectSettingsOpenHistoryEvent();
            CollaborateWindow.Init(CollaborateWindow.FocusTarget.History);
        }

        static void SetupCloudStorage(VisualElement containerUI)
        {
            var goToStorageButton = containerUI.Q(Uxml.Node.GoToStorageLink);
            if (goToStorageButton != null)
            {
                var clickable = new Clickable(GoToCloudStorageDashboard);
                goToStorageButton.AddManipulator(clickable);
            }
        }

        static void GoToCloudStorageDashboard()
        {
            EditorGameServiceAnalyticsSender.SendProjectSettingsCloudStorageDashboardEvent();
#if ENABLE_EDITOR_GAME_SERVICES
            Application.OpenURL(string.Format(k_StorageUrl, CloudProjectSettings.organizationKey, CloudProjectSettings.projectId));
#else
            Application.OpenURL(string.Format(k_StorageUrl, CloudProjectSettings.organizationId, CloudProjectSettings.projectId));
#endif
        }
    }
}
