using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Connect;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Base class to extend for an external service settings provider to become an editor game service settings provider
    /// </summary>
    public abstract class EditorGameServiceSettingsProvider : SettingsProvider
    {
        const string k_BasePath = "Project/Services/{0}";
        const string k_InsufficientPermissionMsg = "You do not have the required permissions to activate or deactivate a service";
        const string k_UserNameAnonymous = "anonymous";
        const string k_AuthenticationErrorMessage = "An authentication error has occurred while trying to get access" +
            " to the service, if the error persists please try restarting the editor.";

        VisualElement m_ParentVisualElement;
        IProjectStateRequest m_ProjectStateRequest;
        ProjectState m_CurrentProjectState;


#if ENABLE_EDITOR_GAME_SERVICES
        IProjectEditorDrawerFactory m_ProjectBindDrawerFactory;
        IProjectEditorDrawerFactory m_CoppaDrawerFactory;
        IUserRoleHandler m_UserRoleHandler;
#endif

        /// <summary>
        /// Editor game service for these project settings
        /// </summary>
        protected abstract IEditorGameService EditorGameService { get; }

        /// <summary>
        /// Title of the service that will be displayed in the Project Settings
        /// </summary>
        protected abstract string Title { get; }

        /// <summary>
        /// Description of the service that will be displayed in the Project Settings
        /// </summary>
        protected abstract string Description { get; }

        /// <summary>
        /// Builds and return the Services specific UI as a Visual Element
        /// </summary>
        /// <returns>
        /// Return the parent node for this settings' detail UI.
        /// </returns>
        protected abstract VisualElement GenerateServiceDetailUI();

#if ENABLE_EDITOR_GAME_SERVICES
        internal EditorGameServiceSettingsProvider(string path, SettingsScope scopes, IProjectEditorDrawerFactory projectBindDrawer,
                                                   IProjectEditorDrawerFactory projectCoppaDrawer, IProjectStateRequest projectStateRequest = null, IUserRoleHandler userRoleHandler = null, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
            m_ProjectStateRequest = projectStateRequest ?? new ProjectStateRequest();
            m_CurrentProjectState = m_ProjectStateRequest.GetProjectState();
            m_ProjectBindDrawerFactory = projectBindDrawer;
            m_CoppaDrawerFactory = projectCoppaDrawer;
            m_UserRoleHandler = userRoleHandler ?? EditorGameServiceRegistry.Instance.UserRoleHandler;
            activateHandler = ActivateSettingsProvider;
            deactivateHandler = DeactivateSettingsProvider;
        }

        void ActivateSettingsProvider(string searchContext, VisualElement rootElement)
        {
            m_ParentVisualElement = GenerateParentElement();
            rootElement.Add(m_ParentVisualElement);
            RefreshUI();

#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged += OnProjectStateChanged;
            m_UserRoleHandler.UserRoleChanged += OnUserRoleChanged;
            m_UserRoleHandler.TrySendUserRoleRequest();
#endif
        }

        void DeactivateSettingsProvider()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged -= OnProjectStateChanged;
            m_UserRoleHandler.UserRoleChanged -= OnUserRoleChanged;
#endif
        }

        void OnProjectStateChanged()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            if (m_UserRoleHandler.IsBusy())
                return;
#endif

            var projectState = m_ProjectStateRequest.GetProjectState();
            if (m_CurrentProjectState.HasDiff(projectState))
            {
                RefreshUI();
            }
        }

        void OnUserRoleChanged(UserRole userRole)
        {
            RefreshUI();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGameServiceSettingsProvider"/> class.
        /// </summary>
        /// <param name="path">
        /// The path to the settings.
        /// You SHOULD use <see cref="GenerateProjectSettingsPath"/> to provide it.
        /// </param>
        /// <param name="scopes">
        /// The scope of the provided settings.
        /// </param>
        /// <param name="keywords">
        /// Set of keywords for search purposes.
        /// </param>
        protected EditorGameServiceSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : this(path, scopes, null, null, keywords : keywords) {}
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGameServiceSettingsProvider"/> class.
        /// </summary>
        /// <param name="path">
        /// The path to the settings.
        /// You SHOULD use <see cref="GenerateProjectSettingsPath"/> to provide it.
        /// </param>
        /// <param name="scopes">
        /// The scope of the provided settings.
        /// </param>
        /// <param name="keywords">
        /// Set of keywords for search purposes.
        /// </param>
        protected EditorGameServiceSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
            m_ProjectStateRequest = new ProjectStateRequest();
            activateHandler = (searchContext, rootElement) => {
                m_ParentVisualElement = GenerateParentElement();
                rootElement.Add(m_ParentVisualElement);
                RefreshUI();
            };
            deactivateHandler = () => {};
        }

#endif

        /// <summary>
        /// Use this to standardize Service Project Settings path:
        /// usage example:
        /// var provider = new MyCloudServiceSettings(GenerateProjectSettingsPath("My Cloud Service"), SettingsScope.Project);
        /// </summary>
        /// <param name="serviceName">Name of the service to use in path</param>
        /// <returns>The path to pass as argument to SettingsProvider</returns>
        protected static string GenerateProjectSettingsPath(string serviceName)
        {
            return string.Format(k_BasePath, serviceName);
        }

        VisualElement GetSetupOrServiceUI(ProjectState projectState)
        {
#if ENABLE_EDITOR_GAME_SERVICES
            var uiBody = new VisualElement();

            if (!IsUserOnline(projectState))
            {
                DrawOfflineUI(uiBody);
            }
            else if (!IsUserLoggedIn(projectState))
            {
                DrawLoggedOutUI(uiBody);
            }
            else if (!IsProjectBound(projectState))
            {
                DrawProjectBindingUI(uiBody);
            }
            else if (IsUserRoleRequestNotAuthorized())
            {
                DrawAccessTokenErrorUI(uiBody);
            }
            else if (!IsUserRoleSet())
            {
                DrawUserRoleUI(uiBody);
            }
            else if (!IsCoppaComplianceMet(EditorGameService, projectState.CoppaCompliance))
            {
                DrawCoppaComplianceUI(uiBody);
            }
            else
            {
                uiBody.Add(GenerateServiceDetailUI());
            }
            return uiBody;
#else
            return GenerateUnsupportedDetailUI();
#endif
        }

        internal static bool IsUserOnline(ProjectState projectState)
        {
            return projectState.IsOnline;
        }

        void DrawOfflineUI(VisualElement parentVisualElement)
        {
            OfflineUiHelper.AddOfflineUI(parentVisualElement, RefreshUI);
        }

        void DrawAccessTokenErrorUI(VisualElement parentVisualElement)
        {
            AccessTokenErrorUiHelper.AddAccessTokenErrorUI(parentVisualElement);
            Debug.LogWarning(k_AuthenticationErrorMessage);
        }

        internal static bool IsUserLoggedIn(ProjectState projectState)
        {
            return !string.IsNullOrEmpty(projectState.UserId) &&
                !projectState.UserName.Equals(k_UserNameAnonymous, StringComparison.InvariantCultureIgnoreCase);
        }

        static void DrawLoggedOutUI(VisualElement parentVisualElement)
        {
            LoggedOutUiHelper.AddLoggedOutUI(parentVisualElement);
        }

#if ENABLE_EDITOR_GAME_SERVICES
        internal static bool IsProjectBound(ProjectState projectState)
        {
            return projectState.ProjectBound;
        }

        bool IsUserRoleSet()
        {
            return m_UserRoleHandler.CurrentUserRole != UserRole.Unknown;
        }

        bool IsUserRoleRequestNotAuthorized()
        {
            return m_UserRoleHandler.HasAuthorizationError;
        }

        void DrawUserRoleUI(VisualElement parentVisualElement)
        {
            UserRoleRequestUiHelper.AddUserRoleRequestUI(parentVisualElement);
        }

#endif

        void DrawProjectBindingUI(VisualElement parentVisualElement)
        {
#if ENABLE_EDITOR_GAME_SERVICES
            var projectBindDrawer = m_ProjectBindDrawerFactory == null ? new ProjectBindDrawer() : m_ProjectBindDrawerFactory.InstantiateDrawer();
            projectBindDrawer.stateChangeButtonFired += RefreshUI;

            if (projectBindDrawer is ProjectBindDrawer drawer)
            {
                drawer.exceptionCallback += (exception) => ShowExceptionVisual(parentVisualElement, ExceptionMessages.ProjectBinding, exception);
            }

            parentVisualElement.Add(projectBindDrawer.GetVisualElement());
#endif
        }

#if ENABLE_EDITOR_GAME_SERVICES
        static void ShowExceptionVisual(VisualElement exceptionContainer, string contextMessage, Exception exception)
        {
            exceptionContainer.Clear();
            ExceptionHelper.AddExceptionVisual(exceptionContainer, contextMessage, exception.Message);
        }

        internal static bool IsCoppaComplianceMet(IEditorGameService editorGameService, CoppaCompliance currentCoppaStatus)
        {
            return editorGameService == null || !editorGameService.RequiresCoppaCompliance ||
                currentCoppaStatus != CoppaCompliance.CoppaUndefined;
        }

#endif

        void DrawCoppaComplianceUI(VisualElement parentVisualElement)
        {
#if ENABLE_EDITOR_GAME_SERVICES
            var coppaDrawer = m_CoppaDrawerFactory == null ? new CoppaDrawer() : m_CoppaDrawerFactory.InstantiateDrawer();
            coppaDrawer.stateChangeButtonFired += RefreshUI;

            if (coppaDrawer is CoppaDrawer drawer)
            {
                drawer.exceptionCallback += (_, exception) => ShowExceptionVisual(parentVisualElement, ExceptionMessages.CoppaCompliance, exception);
            }

            parentVisualElement.Add(coppaDrawer.GetVisualElement());
            if (!IsUserAllowedToEditCoppaCompliance(m_UserRoleHandler.CurrentUserRole))
            {
                parentVisualElement.Q<VisualElement>(NodeName.CoppaContainer)?.Q<VisualElement>(className: ClassName.EditMode)?.SetEnabled(false);
            }
#endif
        }

        internal static bool IsUserAllowedToEditServiceToggle(IEditorGameService editorGameService, ProjectState projectState, UserRole userRole)
        {
#if ENABLE_EDITOR_GAME_SERVICES
            return IsCoppaComplianceMet(editorGameService, projectState.CoppaCompliance) &&
                (userRole == UserRole.Manager || userRole == UserRole.Owner);
#else
            return false;
#endif
        }

        internal static bool IsUserAllowedToEditCoppaCompliance(UserRole userRole)
        {
            return userRole == UserRole.Manager || userRole == UserRole.Owner;
        }

        /// <summary>
        /// The UI to show when the editor API does not support the Services SDK Core package
        /// </summary>
        /// <returns>Custom UI to show when unsupported</returns>
        protected virtual VisualElement GenerateUnsupportedDetailUI()
        {
            return new VisualElement();
        }

        void RefreshUI()
        {
            m_ParentVisualElement.Clear();
            var projectState = m_ProjectStateRequest.GetProjectState();
            AddStyleSheetsToParentElement(m_ParentVisualElement);
            m_ParentVisualElement.Add(GenerateCommonHeader(projectState));
            m_ParentVisualElement.Add(GetSetupOrServiceUI(projectState));
            TranslateStringsInTree(m_ParentVisualElement);
        }

        static VisualElement GenerateParentElement()
        {
            return new ScrollView();
        }

        static void AddStyleSheetsToParentElement(VisualElement parentElement)
        {
            parentElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath.ServicesProjectSettingsCommon));
            parentElement.styleSheets.Add(
                EditorGUIUtility.isProSkin ?
                AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath.ServicesProjectSettingsDark) :
                AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath.ServicesProjectSettingsLight));
        }

        VisualElement GenerateCommonHeader(ProjectState projectState)
        {
#if ENABLE_EDITOR_GAME_SERVICES
            Action dashboardLinkClickAction = null;
            SettingsCommonHeaderUiHelper.ToggleConfiguration toggleConfiguration = null;

            if (projectState.ProjectBound)
            {
                if (EditorGameService != null && EditorGameService.HasDashboard)
                {
                    dashboardLinkClickAction = EditorGameService.OpenDashboard;
                }

                if (EditorGameService?.Enabler != null)
                {
                    var toggleValue = EditorGameService.Enabler.IsEnabled();
                    var toggleEnabled = IsUserAllowedToEditServiceToggle(EditorGameService, projectState, m_UserRoleHandler.CurrentUserRole);
                    var tooltip = string.Empty;
                    if (!toggleEnabled)
                    {
                        tooltip = L10n.Tr(k_InsufficientPermissionMsg);
                    }

                    toggleConfiguration = new SettingsCommonHeaderUiHelper.ToggleConfiguration(toggleValue,
                        true, toggleEnabled, ToggleValueChangedActionAndRefreshUI, tooltip);
                }
            }

            return SettingsCommonHeaderUiHelper.GenerateCommonHeader(Title, Description, toggleConfiguration, dashboardLinkClickAction);
#else
            return SettingsCommonHeaderUiHelper.GenerateCommonHeader(Title, Description);
#endif
        }

#if ENABLE_EDITOR_GAME_SERVICES
        void ToggleValueChangedActionAndRefreshUI(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                EditorGameService.Enabler.Enable();
            }
            else
            {
                EditorGameService.Enabler.Disable();
            }

            RefreshUI();
        }

#endif

        static bool CoppaComplianceMet(IEditorGameService editorGameService, ProjectState projectState)
        {
#if ENABLE_EDITOR_GAME_SERVICES
            return editorGameService == null || !editorGameService.RequiresCoppaCompliance || projectState.CoppaCompliance != CoppaCompliance.CoppaUndefined;
#else
            return false;
#endif
        }

        internal static void TranslateStringsInTree(VisualElement rootElement)
        {
            rootElement.Query<TextElement>().ForEach((label) => label.text = L10n.Tr(label.text));
        }

        static class UssPath
        {
            internal const string ServicesProjectSettingsCommon = "Packages/com.unity.services.core/Editor/Core/EditorGameService/USS/ServicesProjectSettingsCommon.uss";
            internal const string ServicesProjectSettingsDark = "Packages/com.unity.services.core/Editor/Core/EditorGameService/USS/ServicesProjectSettingsDark.uss";
            internal const string ServicesProjectSettingsLight = "Packages/com.unity.services.core/Editor/Core/EditorGameService/USS/ServicesProjectSettingsLight.uss";
        }

        static class NodeName
        {
            internal const string CoppaContainer = "CoppaContainer";
        }

        static class ClassName
        {
            internal const string EditMode = "edit-mode";
        }

        static class ExceptionMessages
        {
            internal const string ProjectBinding = "There was an error during the Project Binding process. Please make sure you are online and logged in:";
            internal const string CoppaCompliance = "There was an error during the COPPA Compliance process. Please make sure you are online and logged in:";
        }
    }
}
