#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Connect;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class ProjectBindVisual : IActivationPopupVisual
    {
        const string k_ExceptionMessage = "There was an error in the Project Binding process. Make sure you are logged in and online: ";

        VisualElement m_ProjectBindContainer;
        VisualElement m_ExceptionContainer;
        ProjectBindDrawer m_ProjectBindDrawer;
        ExceptionVisual m_ExceptionVisual;

        public event Action Done;
        public bool HadException { get; private set; }

        public void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer)
        {
            m_ProjectBindDrawer = new ProjectBindDrawer();
            m_ProjectBindDrawer.stateChangeButtonFired += EndVisual;
            m_ProjectBindDrawer.exceptionCallback += ShowExceptionVisual;

            var visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ServiceActivationPopupVisual.UxmlPath.ProjectBind);
            if (visualAsset != null)
            {
                visualAsset.CloneTree(parentVisual);
            }

            SetupProjectBindVisual(parentVisual);
            SetupExceptionVisual(parentVisual, services, buttonsContainer);
        }

        void EndVisual()
        {
            Done?.Invoke();
        }

        void SetupProjectBindVisual(VisualElement parentVisual)
        {
            m_ProjectBindContainer = parentVisual?.Q(className: ServiceActivationPopupVisual.UxmlClassNames.ProjectBindVisual);
            m_ProjectBindContainer?.Add(m_ProjectBindDrawer.GetVisualElement());
        }

        void SetupExceptionVisual(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer)
        {
            m_ExceptionContainer = parentVisual?.Q(className: ServiceActivationPopupVisual.UxmlClassNames.Exception);

            if (m_ExceptionContainer != null)
            {
                m_ExceptionVisual = new ExceptionVisual();
                m_ExceptionVisual.Init(m_ExceptionContainer, services, buttonsContainer);
                m_ExceptionVisual.Done += EndVisual;
            }
        }

        void ShowExceptionVisual(Exception exception)
        {
            HadException = true;
            VisualElementHelper.SetDisplayStyle(m_ProjectBindContainer, DisplayStyle.None);
            m_ExceptionVisual?.Show(k_ExceptionMessage, exception);
        }

        public void Dispose()
        {
            Done = null;

            m_ProjectBindDrawer.stateChangeButtonFired -= EndVisual;
            m_ProjectBindDrawer.Dispose();
            m_ProjectBindDrawer = null;

            m_ProjectBindContainer = null;
            m_ExceptionContainer = null;
            m_ExceptionVisual.Dispose();
        }
    }
}
#endif
