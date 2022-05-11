#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class ServiceActivationConfirmationVisual : IActivationPopupVisual
    {
        const string k_ServiceNameFormat = "- {0}";

        public event Action Done;

        IEnumerable<IEditorGameService> m_Services;

        public void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer)
        {
            m_Services = services;
            var visualsContainer = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ServiceActivationPopupVisual.UxmlPath.EndConfirmation);
            if (visualsContainer != null)
            {
                visualsContainer.CloneTree(parentVisual);

                AddServiceNames(parentVisual, services);
                SetupButtons(buttonsContainer);
            }
        }

        static void AddServiceNames(VisualElement parentVisual, IEnumerable<IEditorGameService> services)
        {
            var innerContainer = parentVisual?.Q(className: ServiceActivationPopupVisual.UxmlClassNames.ServicesDescriptionBodyInner);

            if (innerContainer != null)
            {
                foreach (var service in services)
                {
                    innerContainer.Add(new Label(string.Format(k_ServiceNameFormat, service.Name)));
                }
            }
        }

        string GetFormattedServiceNames()
        {
            var serviceNames = m_Services.Select(operateService => operateService.Name).ToList();

            return string.Join(", ", serviceNames);
        }

        void SetupButtons(VisualElement buttonsContainer)
        {
            GetAndSetupButton(buttonsContainer, ServiceActivationPopupVisual.UxmlClassNames.ConfirmationButton, true);
            GetAndSetupButton(buttonsContainer, ServiceActivationPopupVisual.UxmlClassNames.CancelButton, false);
        }

        void GetAndSetupButton(VisualElement buttonsContainer, string buttonName, bool shouldActivateServices)
        {
            var button = buttonsContainer?.Q<Button>(className: buttonName);
            if (button != null)
            {
                button.clickable.clicked += () => OnButtonClicked(shouldActivateServices);
            }
            VisualElementHelper.SetDisplayStyle(button, DisplayStyle.Flex);
        }

        void OnButtonClicked(bool shouldActivateServices)
        {
            if (shouldActivateServices)
            {
                foreach (var operateService in m_Services)
                {
                    operateService.Enabler?.Enable();
                }
            }

            Done?.Invoke();
        }

        public void Dispose()
        {
            Done = null;
            m_Services = null;
        }
    }
}
#endif
