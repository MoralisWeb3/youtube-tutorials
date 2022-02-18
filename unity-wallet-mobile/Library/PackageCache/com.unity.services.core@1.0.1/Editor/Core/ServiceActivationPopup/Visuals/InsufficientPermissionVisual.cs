#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class InsufficientPermissionVisual : IActivationPopupVisual
    {
        public event Action Done;

        public void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer)
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ServiceActivationPopupVisual.UxmlPath.InsufficientPermission);
            if (visualTreeAsset != null)
            {
                visualTreeAsset.CloneTree(parentVisual);
                SetupConfirmationButton(buttonsContainer);
            }
        }

        void SetupConfirmationButton(VisualElement buttonsContainer)
        {
            var confirmationButton = buttonsContainer?.Q<Button>(className: ServiceActivationPopupVisual.UxmlClassNames.ConfirmationButton);
            if (confirmationButton != null)
            {
                confirmationButton.clickable.clicked += OnConfirmationButtonClicked;
                VisualElementHelper.SetDisplayStyle(confirmationButton, DisplayStyle.Flex);
            }
        }

        void OnConfirmationButtonClicked()
        {
            Done?.Invoke();
        }

        public void Dispose()
        {
            Done = null;
        }
    }
}
#endif
