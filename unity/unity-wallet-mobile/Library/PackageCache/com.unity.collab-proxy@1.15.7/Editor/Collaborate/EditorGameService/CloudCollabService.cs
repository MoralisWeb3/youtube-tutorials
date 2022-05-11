using Unity.Cloud.Collaborate.EditorGameService;
using Unity.Cloud.Collaborate.Models.Providers;
using Unity.Services.Core.Editor;
using UnityEditor;
using UnityEngine;

using Collab = UnityEditor.Collaboration.Collab;
using CollabInfo = UnityEditor.Collaboration.CollabInfo;

namespace Unity.Cloud.Collaborate.EditorGameService
{
    class CloudCollabService : IEditorGameService
    {
        public CloudCollabService()
        {
            Collab.instance.StateChanged += OnCollabStateChanged;
        }

        ~CloudCollabService()
        {
            Collab.instance.StateChanged -= OnCollabStateChanged;
        }

        public string Name { get; } = "Collaborate";
        public IEditorGameServiceIdentifier Identifier { get; } = new CloudCollabServiceIdentifier();
        public bool RequiresCoppaCompliance { get; } = false;
        public bool HasDashboard { get; } = true;
        public string GetFormattedDashboardUrl()
        {
    #if ENABLE_EDITOR_GAME_SERVICES
            return $"https://developer.cloud.unity3d.com/collab/orgs/{CloudProjectSettings.organizationKey}/projects/{CloudProjectSettings.projectId}/assets/";
    #else
            return string.Empty;
    #endif
        }

        public IEditorGameServiceEnabler Enabler { get; } = new CloudCollabEnabler();

        void OnCollabStateChanged(CollabInfo collabInfo)
        {
            if (Collab.instance.IsCollabEnabledForCurrentProject() && !Enabler.IsEnabled())
            {
                Enabler.Enable();
            }
            else if (!Collab.instance.IsCollabEnabledForCurrentProject() && Enabler.IsEnabled())
            {
                Enabler.Disable();
            }
        }
    }
}
