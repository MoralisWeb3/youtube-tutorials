using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DrawSplitter
    {
        internal static void ForHorizontalIndicator()
        {
            GUIStyle style = UnityStyles.SplitterIndicator;

            Rect splitterRect = GUILayoutUtility.GetRect(
                EditorGUIUtility.currentViewWidth,
                UnityConstants.SPLITTER_INDICATOR_HEIGHT,
                style);

            GUI.Label(splitterRect, string.Empty, style);
        }
    }
}
