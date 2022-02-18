using System;
using System.Reflection;
using Unity.Services.Core.Editor;
using UnityEngine;
using Collab = UnityEditor.Collaboration.Collab;

namespace Unity.Cloud.Collaborate.EditorGameService
{
    class CloudCollabEnabler : EditorGameServiceFlagEnabler
    {
        const string k_ProjectSettingsSettingName = "Collab";

        bool m_IsEnabled = GetEnabledStatusWithReflection();

        protected override string FlagName { get; } = "collab";

        protected override void EnableLocalSettings()
        {
            m_IsEnabled = true;
            Collab.instance.SetCollabEnabledForCurrentProject(true);
            SetEnabledStatusWithReflection(true);
        }

        protected override void DisableLocalSettings()
        {
            m_IsEnabled = false;
            Collab.instance.SetCollabEnabledForCurrentProject(false);
            SetEnabledStatusWithReflection(false);
        }

        public override bool IsEnabled()
        {
            return m_IsEnabled;
        }

        static bool GetEnabledStatusWithReflection()
        {
            var playerSettingsType = Type.GetType("UnityEditor.PlayerSettings,UnityEditor.dll");
            var isEnabled = false;
            if (playerSettingsType != null)
            {
                var getCloudServiceEnabledMethod = playerSettingsType.GetMethod("GetCloudServiceEnabled", BindingFlags.Static | BindingFlags.NonPublic);
                if (getCloudServiceEnabledMethod != null)
                {
                    var enabledStateResult = getCloudServiceEnabledMethod.Invoke(null, new object[] {k_ProjectSettingsSettingName});
                    isEnabled = Convert.ToBoolean(enabledStateResult);
                }
            }

            return isEnabled;
        }

        static void SetEnabledStatusWithReflection(bool value)
        {
            var playerSettingsType = Type.GetType("UnityEditor.PlayerSettings,UnityEditor.dll");
            if (playerSettingsType != null)
            {
                var setCloudServiceEnabledMethod = playerSettingsType.GetMethod("SetCloudServiceEnabled", BindingFlags.Static | BindingFlags.NonPublic);
                if (setCloudServiceEnabledMethod != null)
                {
                    setCloudServiceEnabledMethod.Invoke(null, new object[] {k_ProjectSettingsSettingName, value});
                }
            }
        }
    }
}
