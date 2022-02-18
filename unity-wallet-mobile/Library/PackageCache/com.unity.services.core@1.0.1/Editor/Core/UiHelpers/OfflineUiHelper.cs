using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor
{
    static class OfflineUiHelper
    {
        const string k_UxmlPath = "Packages/com.unity.services.core/Editor/Core/UiHelpers/UXML/Offline.uxml";
        const string k_ButtonClassName = "submit-button";

        public static void AddOfflineUI(VisualElement offlineContainer, Action buttonClickAction)
        {
            if (offlineContainer == null)
                return;

            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_UxmlPath);
            if (visualTreeAsset != null)
            {
                visualTreeAsset.CloneTree(offlineContainer);

                SetupButton(offlineContainer, buttonClickAction);
            }
        }

        static void SetupButton(VisualElement buttonContainer, Action buttonClickAction)
        {
            var button = buttonContainer.Q<Button>(className: k_ButtonClassName);
            if (button != null)
            {
                button.clickable.clicked += () => buttonClickAction?.Invoke();
            }
        }
    }
}
