using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor
{
    static class VisualElementHelper
    {
        public static void SetDisplayStyle(VisualElement target, DisplayStyle displayStyle)
        {
            if (target == null)
                return;

            target.style.display = displayStyle;
        }

        public static void AddStyleSheetFromPath(VisualElement target, string styleSheetPath)
        {
            if (target == null)
                return;

            AddStyleSheet(target, AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath));
        }

        static void AddStyleSheet(VisualElement target, StyleSheet styleSheet)
        {
            if (styleSheet == null || target == null)
                return;

            target.styleSheets.Add(styleSheet);
        }
    }
}
