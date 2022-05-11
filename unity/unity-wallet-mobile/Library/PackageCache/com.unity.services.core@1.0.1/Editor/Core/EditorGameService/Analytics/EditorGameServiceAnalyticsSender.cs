using System;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Core.Editor
{
    static class EditorGameServiceAnalyticsSender
    {
        static class AnalyticsComponent
        {
            public const string ProjectSettings = "Project Settings";
        }

        static class AnalyticsAction
        {
            public const string GoToDashboard = "Go to Dashboard";
        }

        const int k_Version = 1;
        const string k_EventName = "editorgameserviceeditor";

        internal static void SendProjectSettingsGoToDashboardEvent(string package)
        {
            SendEvent(AnalyticsComponent.ProjectSettings, AnalyticsAction.GoToDashboard, package);
        }

        static void SendEvent(string component, string action, string package)
        {
            EditorAnalytics.SendEventWithLimit(k_EventName, new EditorGameServiceEvent
            {
                action = action,
                component = component,
                package = package
            }, k_Version);
        }

        /// <remarks>Lowercase is used here for compatibility with analytics.</remarks>
        [Serializable]
        public struct EditorGameServiceEvent
        {
            public string action;
            public string component;
            public string package;
        }
    }
}
