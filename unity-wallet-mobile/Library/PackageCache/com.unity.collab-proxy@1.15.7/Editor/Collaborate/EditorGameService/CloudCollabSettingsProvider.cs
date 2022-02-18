using System.Collections.Generic;
using Unity.Cloud.Collaborate.EditorGameService.SettingsProviderVisuals;
using Unity.Services.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.EditorGameService
{
    class CloudCollabSettingsProvider : EditorGameServiceSettingsProvider
    {
        const string k_CollaborateName = "Version Control";
        CloudCollabVisuals m_Visuals;

        CloudCollabSettingsProvider(IEnumerable<string> keywords = null)
            : base(GenerateProjectSettingsPath(k_CollaborateName), SettingsScope.Project, keywords) { }

        protected override IEditorGameService EditorGameService { get; } = EditorGameServiceRegistry.Instance.GetEditorGameService<CloudCollabServiceIdentifier>();
        protected override string Title { get; } = k_CollaborateName;
        protected override string Description { get; } = "Create together seamlessly";

        protected override VisualElement GenerateServiceDetailUI()
        {
            m_Visuals = new CloudCollabVisuals();
            m_Visuals.Init(EditorGameService);

            return m_Visuals.GetVisuals();
        }

        [SettingsProvider]
        static SettingsProvider CreateSettingsProvider()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            return new CloudCollabSettingsProvider();
#else
            return null;
#endif
        }
    }
}
