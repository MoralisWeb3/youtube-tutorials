using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.EditorGameService.SettingsProviderVisuals
{
    class DisabledVisuals : IVisuals
    {
        public VisualElement GetVisuals()
        {
            VisualElement containerUI = null;

            var containerAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Uxml.Path.Disabled);
            if (containerAsset != null)
            {
                containerUI = containerAsset.CloneTree().contentContainer;
                LearnMoreVisualHelper.SetupLearnMore(containerUI);
            }

            return containerUI;
        }
    }
}
