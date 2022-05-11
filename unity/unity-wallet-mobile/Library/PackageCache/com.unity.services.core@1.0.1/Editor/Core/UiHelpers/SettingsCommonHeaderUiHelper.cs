using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor
{
    static class SettingsCommonHeaderUiHelper
    {
        internal static VisualElement GenerateCommonHeader(string title, string description,
            ToggleConfiguration toggleConfiguration = null, Action dashboardButtonClick = null)
        {
            var projectSettingsHeaderTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath.servicesProjectSettingsCommonHeader);
            var projectSettingsHeaderVisualElement = projectSettingsHeaderTreeAsset.CloneTree().contentContainer;

            ConfigureTitle(title, projectSettingsHeaderVisualElement);
            ConfigureDescription(description, projectSettingsHeaderVisualElement);
            ConfigureDashboardButton(dashboardButtonClick, projectSettingsHeaderVisualElement);
            ConfigureToggle(toggleConfiguration, projectSettingsHeaderVisualElement);

            return projectSettingsHeaderVisualElement;
        }

        static void ConfigureTitle(string title, VisualElement projectSettingsHeaderVisualElement)
        {
            var serviceTitle = projectSettingsHeaderVisualElement.Q<TextElement>(UxmlNodeName.serviceTitle);
            if (serviceTitle != null)
            {
                serviceTitle.text = title;
            }
        }

        static void ConfigureDescription(string description, VisualElement projectSettingsHeaderVisualElement)
        {
            var serviceDescription = projectSettingsHeaderVisualElement.Q<TextElement>(UxmlNodeName.serviceDescription);
            if (serviceDescription != null)
            {
                serviceDescription.text = description;
            }
        }

        static void ConfigureDashboardButton(Action dashboardButtonClick, VisualElement projectSettingsHeaderVisualElement)
        {
            var dashboardLinkButton = projectSettingsHeaderVisualElement.Q<TextElement>(UxmlNodeName.dashboardLinkButton);
            if (dashboardLinkButton != null)
            {
                if (dashboardButtonClick != null)
                {
                    dashboardLinkButton.AddManipulator(new Clickable(dashboardButtonClick));
                }
                else
                {
                    dashboardLinkButton.visible = false;
                }
            }
        }

        static void ConfigureToggle(ToggleConfiguration configuration, VisualElement projectSettingsHeaderVisualElement)
        {
            var serviceToggle = projectSettingsHeaderVisualElement.Q<Toggle>(UxmlNodeName.serviceToggle);
            if (serviceToggle != null)
            {
                if (configuration == null)
                {
                    serviceToggle.visible = false;
                }
                else
                {
                    serviceToggle.visible = configuration.Visible;
                    serviceToggle.SetEnabled(configuration.Enabled);
                    serviceToggle.SetValueWithoutNotify(configuration.Value);
                    if (!string.IsNullOrWhiteSpace(configuration.Tooltip))
                    {
                        serviceToggle.tooltip = configuration.Tooltip;
                        var toggleContainer = projectSettingsHeaderVisualElement.Q<VisualElement>(UxmlNodeName.toggleContainer);
                        if (toggleContainer != null)
                        {
                            toggleContainer.tooltip = configuration.Tooltip;
                        }
                    }
                    if (configuration.ValueChangedAction != null)
                    {
                        serviceToggle.RegisterValueChangedCallback(evt => {
                            configuration.ValueChangedAction.Invoke(evt);
                        });
                    }
                }
                SliderToggleUiHelper.ConvertToggleToSliderToggle(serviceToggle);
            }
        }

        static class UxmlPath
        {
            internal const string servicesProjectSettingsCommonHeader = "Packages/com.unity.services.core/Editor/Core/UiHelpers/UXML/ProjectSettingsCommonHeader.uxml";
        }

        static class UxmlNodeName
        {
            internal const string dashboardLinkButton = "dashboard-link-button";
            internal const string serviceTitle = "service-title";
            internal const string serviceDescription = "service-description";
            internal const string serviceToggle = "service-toggle";
            internal const string toggleContainer = "toggle-container";
        }

        internal class ToggleConfiguration
        {
            internal bool Value { get; }
            internal bool Visible { get; }
            internal bool Enabled { get; }
            internal string Tooltip { get; }
            internal Action<ChangeEvent<bool>> ValueChangedAction { get; }

            internal ToggleConfiguration(bool value, bool visible, bool enabled, Action<ChangeEvent<bool>> valueChangedAction, string tooltip = null)
            {
                Value = value;
                Visible = visible;
                Enabled = enabled;
                Tooltip = tooltip;
                ValueChangedAction = valueChangedAction;
            }
        }
    }
}
