#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class ServiceActivationPopupVisual : IActivationPopupVisual
    {
        VisualElement m_ContentContainer;
        VisualElement m_ButtonsContainer;
        IActivationPopupVisual m_CurrentVisual;
        IEnumerable<IEditorGameService> m_Services;

        public event Action Done;

        public void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer = null)
        {
            m_Services = services;

            var visualsContainer = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath.Common);
            if (visualsContainer != null)
            {
                visualsContainer.CloneTree(parentVisual);
                m_ContentContainer = parentVisual?.Q(className: UxmlClassNames.Content);
                SetupButtons(parentVisual);
                SetupStyleSheets(parentVisual);
            }

            CloudProjectSettingsEventManager.instance.projectStateChanged -= RefreshUI;
            CloudProjectSettingsEventManager.instance.projectStateChanged += RefreshUI;

            RefreshUI();
        }

        void SetupButtons(VisualElement parentVisual)
        {
            m_ButtonsContainer = parentVisual?.Q(className: UxmlClassNames.Buttons);
            var button = m_ButtonsContainer?.Q(UxmlClassNames.ConfirmationButton);
            VisualElementHelper.SetDisplayStyle(m_ButtonsContainer?.Q(className: UxmlClassNames.ConfirmationButton), DisplayStyle.None);
            VisualElementHelper.SetDisplayStyle(m_ButtonsContainer?.Q(className: UxmlClassNames.CancelButton), DisplayStyle.None);
        }

        static void SetupStyleSheets(VisualElement parentVisual)
        {
            VisualElementHelper.AddStyleSheetFromPath(parentVisual, UssPath.Window);
            VisualElementHelper.AddStyleSheetFromPath(parentVisual, EditorGUIUtility.isProSkin ? UssPath.DarkTheme : UssPath.LightTheme);
        }

        void RefreshUI()
        {
            if (m_ContentContainer == null)
                return;

            m_ContentContainer.Clear();
            m_CurrentVisual?.Dispose();

            var projectState = new ProjectStateRequest().GetProjectState();

            if (!EditorGameServiceSettingsProvider.IsUserOnline(projectState))
            {
                m_CurrentVisual = CreateVisual<OfflineVisual>(RefreshUI);
            }
            else if (!EditorGameServiceSettingsProvider.IsUserLoggedIn(projectState))
            {
                m_CurrentVisual = CreateVisual<LoggedOutVisual>(RefreshUI);
            }
            else if (!EditorGameServiceSettingsProvider.IsProjectBound(projectState) ||
                     EditorGameServiceRegistry.Instance.UserRoleHandler.CurrentUserRole == UserRole.Unknown)
            {
                m_CurrentVisual = CreateVisual<ProjectBindAndUserRequestVisual>(OnUserRoleRequestVisualDone);
            }
            else
            {
                ShowPostUserRequestUi(projectState);
            }

            EditorGameServiceSettingsProvider.TranslateStringsInTree(m_ContentContainer);
        }

        IActivationPopupVisual CreateVisual<T>(Action onDoneAction)
            where T : IActivationPopupVisual, new()
        {
            var output = new T();
            output.Init(m_ContentContainer, m_Services, m_ButtonsContainer);
            output.Done += onDoneAction;

            return output;
        }

        void OnUserRoleRequestVisualDone()
        {
            if (EditorGameServiceSettingsProvider.IsUserAllowedToEditCoppaCompliance(
                EditorGameServiceRegistry.Instance.UserRoleHandler.CurrentUserRole))
            {
                RefreshUI();
            }
            else
            {
                Done?.Invoke();
            }
        }

        void ShowPostUserRequestUi(ProjectState projectState)
        {
            if (EditorGameServiceSettingsProvider.IsUserAllowedToEditCoppaCompliance(
                EditorGameServiceRegistry.Instance.UserRoleHandler.CurrentUserRole))
            {
                ShowCoppaComplianceOrActivationConfirmation(projectState);
            }
            else
            {
                m_CurrentVisual = CreateVisual<InsufficientPermissionVisual>(OnEndConfirmationDone);
            }
        }

        void ShowCoppaComplianceOrActivationConfirmation(ProjectState projectState)
        {
            if (ShouldShowCoppaCompliance())
            {
                m_CurrentVisual = CreateVisual<CoppaVisual>(OnCoppaVisualDone);
            }
            else
            {
                if (IsUserAllowedToEnableServices(projectState))
                {
                    m_CurrentVisual = CreateVisual<ServiceActivationConfirmationVisual>(OnEndConfirmationDone);
                    m_CurrentVisual.Done += OnEndConfirmationDone;
                }
                else
                {
                    m_CurrentVisual = CreateVisual<InsufficientPermissionVisual>(OnEndConfirmationDone);
                }
            }
        }

        bool ShouldShowCoppaCompliance()
        {
            bool output = false;
            foreach (var operateService in m_Services)
            {
                if (!EditorGameServiceSettingsProvider.IsCoppaComplianceMet(operateService, CloudProjectSettings.coppaCompliance))
                {
                    output = true;
                    break;
                }
            }

            return output;
        }

        void OnCoppaVisualDone()
        {
            if (m_CurrentVisual is CoppaVisual coppaVisual && coppaVisual.HadException)
            {
                Done?.Invoke();
            }
            else
            {
                RefreshUI();
            }
        }

        bool IsUserAllowedToEnableServices(ProjectState projectState)
        {
            var output = true;

            foreach (var service in m_Services)
            {
                if (!EditorGameServiceSettingsProvider.IsUserAllowedToEditServiceToggle(service,
                    projectState,
                    EditorGameServiceRegistry.Instance.UserRoleHandler.CurrentUserRole))
                {
                    output = false;
                    break;
                }
            }

            return output;
        }

        void OnEndConfirmationDone()
        {
            Done?.Invoke();
        }

        public void Dispose()
        {
            Done = null;

            m_ContentContainer = null;

            m_CurrentVisual.Dispose();
            m_CurrentVisual = null;

            CloudProjectSettingsEventManager.instance.projectStateChanged -= RefreshUI;
        }

        internal static class UxmlPath
        {
            public const string Common = "Packages/com.unity.services.core/Editor/Core/ServiceActivationPopup/Visuals/UXML/General.uxml";
            public const string ProjectBindAndUserRequest = "Packages/com.unity.services.core/Editor/Core/ServiceActivationPopup/Visuals/UXML/ProjectBindingAndUserRequest.uxml";
            public const string ProjectBind = "Packages/com.unity.services.core/Editor/Core/ServiceActivationPopup/Visuals/UXML/ProjectBind.uxml";
            public const string EndConfirmation = "Packages/com.unity.services.core/Editor/Core/ServiceActivationPopup/Visuals/UXML/EndConfirmation.uxml";
            public const string InsufficientPermission = "Packages/com.unity.services.core/Editor/Core/ServiceActivationPopup/Visuals/UXML/InsufficientPermission.uxml";
        }

        internal static class UxmlClassNames
        {
            public const string Content = "content";
            public const string Buttons = "buttons";
            public const string ProjectBindContainer = "project-bind";
            public const string UserRequestContainer = "user-request";
            public const string UserRequestLabel = "user-request-text";
            public const string ProjectBindVisual = "project-bind-visual";
            public const string CoppaVisual = "project-bind-visual";
            public const string Exception = "exception";
            public const string Separator = "separator";
            public const string ConfirmationButton = "confirmation-button";
            public const string CancelButton = "cancel-button";
            public const string ServicesDescriptionBodyInner = "services-description-body-inner";
        }

        static class UssPath
        {
            public const string Window = "Packages/com.unity.services.core/Editor/Core/ServiceActivationPopup/Visuals/USS/ServiceActivationWindow.uss";
            public const string LightTheme = "Packages/com.unity.services.core/Editor/Core/EditorGameService/USS/ServicesProjectSettingsLight.uss";
            public const string DarkTheme = "Packages/com.unity.services.core/Editor/Core/EditorGameService/USS/ServicesProjectSettingsDark.uss";
        }
    }
}
#endif
