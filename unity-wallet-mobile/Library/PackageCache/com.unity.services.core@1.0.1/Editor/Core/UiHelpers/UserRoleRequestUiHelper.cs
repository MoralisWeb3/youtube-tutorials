#if ENABLE_EDITOR_GAME_SERVICES
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor
{
    static class UserRoleRequestUiHelper
    {
        const string k_UxmlPath = "Packages/com.unity.services.core/Editor/Core/UiHelpers/UXML/PleaseWait.uxml";

        public static void AddUserRoleRequestUI(VisualElement userRoleRequestContainer)
        {
            if (userRoleRequestContainer == null)
                return;

            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_UxmlPath);
            if (visualTreeAsset != null)
            {
                visualTreeAsset.CloneTree(userRoleRequestContainer);
            }
        }
    }
}
#endif
