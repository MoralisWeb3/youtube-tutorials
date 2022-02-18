#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class ExceptionVisual : IActivationPopupVisual
    {
        VisualElement m_ParentVisual;
        Button m_CloseButton;

        public event Action Done;

        public void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer = null)
        {
            m_ParentVisual = parentVisual;
            SetupButton(buttonsContainer);
        }

        void SetupButton(VisualElement buttonsContainer)
        {
            m_CloseButton = buttonsContainer?.Q<Button>(className: ServiceActivationPopupVisual.UxmlClassNames.ConfirmationButton);
            if (m_CloseButton != null)
            {
                m_CloseButton.clickable.clicked += OnCloseButtonClicked;
                m_CloseButton.style.display = DisplayStyle.None;
            }
        }

        void OnCloseButtonClicked()
        {
            Done?.Invoke();
        }

        public void Show(string text, Exception exception)
        {
            m_ParentVisual.Clear();
            ExceptionHelper.AddExceptionVisual(m_ParentVisual, text, exception.Message);
            VisualElementHelper.SetDisplayStyle(m_CloseButton, DisplayStyle.Flex);
        }

        public void Dispose()
        {
            Done = null;

            m_CloseButton = null;
        }
    }
}
#endif
