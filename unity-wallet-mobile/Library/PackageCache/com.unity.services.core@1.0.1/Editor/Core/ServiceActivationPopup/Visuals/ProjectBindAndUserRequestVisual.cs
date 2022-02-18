#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class ProjectBindAndUserRequestVisual : IActivationPopupVisual
    {
        IEnumerable<IEditorGameService> m_Services;
        VisualElement m_ParentVisual;
        VisualElement m_ProjectBindContainer;
        VisualElement m_UserRequestContainer;
        VisualElement m_Separator;
        ProjectBindVisual m_ProjectBindVisual;
        UserRequestVisual m_UserRequestVisual;

        public event Action Done;
        public UserRole UserRole { get; private set; }

        public void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer)
        {
            m_Services = services;
            m_ParentVisual = parentVisual;

            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ServiceActivationPopupVisual.UxmlPath.ProjectBindAndUserRequest);
            if (visualTreeAsset != null)
            {
                visualTreeAsset.CloneTree(m_ParentVisual);

                var projectState = new ProjectStateRequest().GetProjectState();
                SetupProjectBind(buttonsContainer, projectState);
                SetupSeparator();
                SetupUserRequest(buttonsContainer, projectState);
            }
        }

        void SetupProjectBind(VisualElement buttonsContainer, ProjectState projectState)
        {
            CreateVisual(out m_ProjectBindVisual, out m_ProjectBindContainer, buttonsContainer,
                ServiceActivationPopupVisual.UxmlClassNames.ProjectBindContainer, OnProjectBindDone);

            VisualElementHelper.SetDisplayStyle(m_ProjectBindContainer,
                EditorGameServiceSettingsProvider.IsProjectBound(projectState)
                ? DisplayStyle.None
                : DisplayStyle.Flex);
        }

        void CreateVisual<T>(out T visual, out VisualElement container, VisualElement buttonsContainer, string containerClassName, Action onDone)
            where T : IActivationPopupVisual, new()
        {
            visual = default(T);
            container = m_ParentVisual?.Q(className: containerClassName);

            if (container != null)
            {
                visual = new T();
                visual.Init(container, m_Services, buttonsContainer);
                visual.Done += onDone;
            }
        }

        void SetupSeparator()
        {
            m_Separator = m_ParentVisual?.Q(className: ServiceActivationPopupVisual.UxmlClassNames.Separator);
            VisualElementHelper.SetDisplayStyle(m_Separator, DisplayStyle.None);
        }

        void OnProjectBindDone()
        {
            if (m_ProjectBindVisual.HadException)
            {
                EndVisual();
            }
            else
            {
                ShowUserRequest();
            }
        }

        void ShowUserRequest()
        {
            VisualElementHelper.SetDisplayStyle(m_Separator, DisplayStyle.Flex);
            m_UserRequestVisual.Show();
        }

        void SetupUserRequest(VisualElement buttonsContainer, ProjectState projectState)
        {
            CreateVisual(out m_UserRequestVisual, out m_UserRequestContainer, buttonsContainer,
                ServiceActivationPopupVisual.UxmlClassNames.UserRequestContainer, EndVisual);

            if (EditorGameServiceSettingsProvider.IsProjectBound(projectState))
            {
                ShowUserRequest();
            }
        }

        void EndVisual()
        {
            if (m_UserRequestVisual != null)
            {
                UserRole = m_UserRequestVisual.UserRole;
            }

            Done?.Invoke();
        }

        public void Dispose()
        {
            Done = null;
            UserRole = UserRole.Unknown;

            m_ParentVisual = null;
            m_ProjectBindContainer = null;
            m_UserRequestContainer = null;
            m_Separator = null;

            m_ProjectBindVisual?.Dispose();
            m_UserRequestVisual?.Dispose();

            m_Services = null;
        }
    }
}
#endif
