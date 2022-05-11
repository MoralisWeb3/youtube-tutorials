using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor
{
    static class AccessTokenErrorUiHelper
    {
        const string k_UxmlPath = "Packages/com.unity.services.core/Editor/Core/UiHelpers/UXML/AccessTokenError.uxml";

        public static void AddAccessTokenErrorUI(VisualElement accessTokenErrorContainer)
        {
            if (accessTokenErrorContainer == null)
                return;

            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_UxmlPath);
            if (visualTreeAsset != null)
            {
                visualTreeAsset.CloneTree(accessTokenErrorContainer);
            }
        }
    }
}
