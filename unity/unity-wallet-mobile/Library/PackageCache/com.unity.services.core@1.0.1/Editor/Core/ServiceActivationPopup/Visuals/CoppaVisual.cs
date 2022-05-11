#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Connect;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class CoppaVisual : IActivationPopupVisual
    {
        const string k_ExceptionMessage = "There was an error when setting the COPPA Compliance. Make sure you are logged in and online: ";

        VisualElement m_CoppaContainer;
        VisualElement m_ExceptionContainer;
        CoppaDrawer m_CoppaDrawer;
        ExceptionVisual m_ExceptionVisual;

        public event Action Done;

        public bool HadException { get; private set; }

        public void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer)
        {
            m_CoppaDrawer = new CoppaDrawer();
            m_CoppaDrawer.stateChangeButtonFired += EndVisual;
            m_CoppaDrawer.exceptionCallback += ShowExceptionVisual;

            var visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ServiceActivationPopupVisual.UxmlPath.ProjectBind);
            if (visualAsset != null)
            {
                visualAsset.CloneTree(parentVisual);
            }

            SetupCoppaVisual(parentVisual);
            SetupExceptionVisual(parentVisual, services, buttonsContainer);
        }

        void EndVisual()
        {
            Done?.Invoke();
        }

        void SetupCoppaVisual(VisualElement parentVisual)
        {
            m_CoppaContainer = parentVisual?.Q(className: ServiceActivationPopupVisual.UxmlClassNames.CoppaVisual);
            m_CoppaContainer?.Add(m_CoppaDrawer.GetVisualElement());
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

        void ShowExceptionVisual(CoppaCompliance coppaCompliance, Exception exception)
        {
            HadException = true;
            VisualElementHelper.SetDisplayStyle(m_CoppaContainer, DisplayStyle.None);
            m_ExceptionVisual?.Show(k_ExceptionMessage, exception);
        }

        public void Dispose()
        {
            Done = null;

            m_CoppaDrawer.stateChangeButtonFired -= EndVisual;
            m_CoppaDrawer.Dispose();
            m_CoppaDrawer = null;

            m_CoppaContainer = null;
            m_ExceptionContainer = null;
            m_ExceptionVisual.Dispose();
        }
    }
}
#endif
