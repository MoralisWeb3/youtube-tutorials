using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Utility class for platform support UI.
    /// </summary>
    public static class PlatformSupportUiHelper
    {
        /// <summary>
        /// Generate a platform support UI.
        /// </summary>
        /// <param name="platforms">
        /// The set of platforms to support in the UI.
        /// </param>
        /// <returns>
        /// Return the parent node of the platform support UI.
        /// </returns>
        public static VisualElement GeneratePlatformSupport(IEnumerable<string> platforms)
        {
            VisualElement platformSupportVisualElement = null;
            if (platforms != null && platforms.Any())
            {
                var platformSupportVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath.platformSupport);
                platformSupportVisualElement = platformSupportVisualTreeAsset.CloneTree().contentContainer;

                platformSupportVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath.platformSupportCommon));
                platformSupportVisualElement.styleSheets.Add(
                    EditorGUIUtility.isProSkin ?
                    AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath.platformSupportDark) :
                    AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath.platformSupportLight));

                var tagContainer = platformSupportVisualElement.Q(className: UssClassName.tagContainer);
                tagContainer.Clear();

                foreach (var platform in platforms)
                {
                    var tag = new Label(platform);
                    tag.AddToClassList(UssClassName.platformTag);
                    tagContainer.Add(tag);
                }
            }
            return platformSupportVisualElement;
        }

        static class UxmlPath
        {
            internal const string platformSupport = "Packages/com.unity.services.core/Editor/Core/UiHelpers/UXML/PlatformSupportVisual.uxml";
        }

        static class UssPath
        {
            internal const string platformSupportCommon = "Packages/com.unity.services.core/Editor/Core/UiHelpers/USS/PlatformSupportVisualCommon.uss";
            internal const string platformSupportDark = "Packages/com.unity.services.core/Editor/Core/UiHelpers/USS/PlatformSupportVisualDark.uss";
            internal const string platformSupportLight = "Packages/com.unity.services.core/Editor/Core/UiHelpers/USS/PlatformSupportVisualLight.uss";
        }

        static class UssClassName
        {
            internal const string tagContainer = "tag-container";
            internal const string platformTag = "platform-tag";
        }
    }
}
