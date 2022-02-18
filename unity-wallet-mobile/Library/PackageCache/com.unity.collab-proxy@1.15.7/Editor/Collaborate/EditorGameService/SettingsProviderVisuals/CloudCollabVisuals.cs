using Unity.Services.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.EditorGameService.SettingsProviderVisuals
{
    class CloudCollabVisuals : IVisuals
    {
        IVisuals m_CurrentVisuals;

        /// <remarks>A standard if else is used here instead of the ternary conditional operator to prevent an implicit conversion error in the 2020 editor.</remarks>
        public void Init(IEditorGameService editorGameService)
        {
            if (editorGameService.Enabler.IsEnabled())
            {
                m_CurrentVisuals = new EnabledVisuals();
            }
            else
            {
                m_CurrentVisuals = new DisabledVisuals();
            }
        }

        public VisualElement GetVisuals()
        {
            var output = m_CurrentVisuals.GetVisuals();
            SetupStyleSheets(output);

            return output;
        }

        static void SetupStyleSheets(VisualElement parentElement)
        {
            if (parentElement == null)
            {
                return;
            }

            parentElement.AddToClassList("collab");

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Uss.Path.Common);
            if (styleSheet != null)
            {
                parentElement.styleSheets.Add(styleSheet);
            }
        }
    }

    static class Uxml
    {
        public static class Path
        {
            public const string Disabled = "Packages/com.unity.collab-proxy/Editor/Collaborate/EditorGameService/SettingsProviderVisuals/UXML/CollabProjectSettingsDisabled.uxml";
            public const string Enabled = "Packages/com.unity.collab-proxy/Editor/Collaborate/EditorGameService/SettingsProviderVisuals/UXML/CollabProjectSettingsEnabled.uxml";
        }

        public static class Node
        {
            public const string PublishSection = "CollabPublishSection";
            public const string HistorySection = "CollabHistorySection";
            public const string OpenHistoryLink = "OpenHistory";
            public const string OpenChangesLink = "OpenChanges";
            public const string GoToStorageLink = "GoToWebDashboard";
            public const string LearnMoreLink = "LearnMore";
        }
    }

    static class Uss
    {
        public static class Path
        {
            public const string Common = "Packages/com.unity.collab-proxy/Editor/Collaborate/EditorGameService/SettingsProviderVisuals/USS/ServicesProjectSettingsCommon.uss";
        }
    }
}
