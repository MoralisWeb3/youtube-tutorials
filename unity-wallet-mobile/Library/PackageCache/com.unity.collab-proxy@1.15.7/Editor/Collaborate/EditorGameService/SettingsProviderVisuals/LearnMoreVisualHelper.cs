using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.EditorGameService.SettingsProviderVisuals
{
    static class LearnMoreVisualHelper
    {
        static readonly string k_LearnMoreAboutTeamsUrl = "https://unity3d.com/teams";

        public static void SetupLearnMore(VisualElement containerUI)
        {
            var learnMoreButton = containerUI.Q(Uxml.Node.LearnMoreLink);
            if (learnMoreButton != null)
            {
                var clickable = new Clickable(GoToLearnMore);
                learnMoreButton.AddManipulator(clickable);
            }
        }

        static void GoToLearnMore()
        {
            EditorGameServiceAnalyticsSender.SendProjectSettingsLearnMoreEvent();
            Application.OpenURL(k_LearnMoreAboutTeamsUrl);
        }
    }
}
