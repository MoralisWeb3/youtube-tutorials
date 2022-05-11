using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor
{
    static class ExceptionHelper
    {
        const string k_UxmlPath = "Packages/com.unity.services.core/Editor/Core/UiHelpers/UXML/ExceptionVisual.uxml";
        const string k_ExceptionContext = "exception-context";
        const string k_ExceptionMessage = "exception-message";

        public static void AddExceptionVisual(VisualElement exceptionContainer, string exceptionContext, string exceptionMessage)
        {
            if (exceptionContainer == null)
                return;

            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_UxmlPath);
            if (visualTreeAsset != null)
            {
                visualTreeAsset.CloneTree(exceptionContainer);

                SetLabelText(exceptionContainer, k_ExceptionContext, exceptionContext);
                SetLabelText(exceptionContainer, k_ExceptionMessage, exceptionMessage);
            }
        }

        static void SetLabelText(VisualElement labelContainer, string className, string message)
        {
            var label = labelContainer.Q<Label>(className: className);
            if (label != null)
            {
                label.text = L10n.Tr(message);
            }
        }
    }
}
