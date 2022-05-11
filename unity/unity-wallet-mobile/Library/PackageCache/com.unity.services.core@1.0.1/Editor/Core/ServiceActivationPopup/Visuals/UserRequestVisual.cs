#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class UserRequestVisual : IActivationPopupVisual
    {
        VisualElement m_ParentVisual;
        Label m_Label;
        Button m_NoPermissionButton;

        public UserRole UserRole { get; private set; }
        public event Action Done;

        public void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer)
        {
            m_ParentVisual = parentVisual;

            UserRoleRequestUiHelper.AddUserRoleRequestUI(parentVisual);

            SetupLabel(parentVisual);
            SetupButton(buttonsContainer);

            VisualElementHelper.SetDisplayStyle(m_ParentVisual, DisplayStyle.None);
        }

        public void Show()
        {
            VisualElementHelper.SetDisplayStyle(m_ParentVisual, DisplayStyle.Flex);

            EditorGameServiceRegistry.Instance.UserRoleHandler.TrySendUserRoleRequest();
            EditorGameServiceRegistry.Instance.UserRoleHandler.UserRoleRequestCompleted += OnUserRoleRequestCompleted;
        }

        void OnUserRoleRequestCompleted(UserRole userRole)
        {
            EditorGameServiceRegistry.Instance.UserRoleHandler.UserRoleRequestCompleted -= OnUserRoleRequestCompleted;

            UserRole = userRole;
            if (EditorGameServiceSettingsProvider.IsUserAllowedToEditCoppaCompliance(UserRole))
            {
                Done?.Invoke();
            }
            else
            {
                SetupLabelText(Messages.InsufficientPermission);

                if (m_NoPermissionButton != null)
                {
                    m_NoPermissionButton.style.display = DisplayStyle.Flex;
                    m_NoPermissionButton.clickable.clicked += OnNoPermissionButtonClicked;
                }
            }
        }

        void OnNoPermissionButtonClicked()
        {
            Done?.Invoke();
        }

        void SetupLabel(VisualElement parentVisual)
        {
            m_Label = parentVisual.Q<Label>(className: ServiceActivationPopupVisual.UxmlClassNames.UserRequestLabel);
            SetupLabelText(Messages.PleaseWait);
        }

        void SetupLabelText(string message)
        {
            if (m_Label != null)
            {
                m_Label.text = L10n.Tr(message);
            }
        }

        void SetupButton(VisualElement buttonsContainer)
        {
            m_NoPermissionButton = buttonsContainer.Q<Button>(className: ServiceActivationPopupVisual.UxmlClassNames.ConfirmationButton);
            VisualElementHelper.SetDisplayStyle(m_NoPermissionButton, DisplayStyle.None);
        }

        public void Dispose()
        {
            Done = null;
            m_ParentVisual = null;
            m_Label = null;
            m_NoPermissionButton = null;
            UserRole = UserRole.Unknown;
        }

        static class Messages
        {
            public const string PleaseWait = "Please Wait...";
            public const string InsufficientPermission = "You do not have the correct permissions to activate the installed services.";
        }
    }
}
#endif
