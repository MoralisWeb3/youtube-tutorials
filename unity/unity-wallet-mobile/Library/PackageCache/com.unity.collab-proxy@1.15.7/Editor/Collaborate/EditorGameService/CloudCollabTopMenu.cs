#if ENABLE_EDITOR_GAME_SERVICES
using UnityEditor;

namespace Unity.Cloud.Collaborate.EditorGameService
{
    static class CloudCollabTopMenu
    {
        const string k_ServiceMenuRoot = "Services/Version Control/";
        const int k_ConfigureMenuPriority = 100;

        [MenuItem(k_ServiceMenuRoot + "Configure", priority = k_ConfigureMenuPriority)]
        static void ShowProjectSettings()
        {
            EditorGameServiceAnalyticsSender.SendTopMenuConfigureEvent();
            SettingsService.OpenProjectSettings("Project/Services/Version Control");
        }
    }
}
#endif
