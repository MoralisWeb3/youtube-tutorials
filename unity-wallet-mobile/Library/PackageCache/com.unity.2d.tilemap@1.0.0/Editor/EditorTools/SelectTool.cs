using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Tool for doing a selection with the Tile Palette
    /// </summary>
    public sealed class SelectTool : TilemapEditorTool
    {
        private static class Styles
        {
            public static string tooltipStringFormat = "|Select an area of the grid ({0})";
            public static string shortcutId = "Grid Painting/Select";
            public static GUIContent toolContent = EditorGUIUtility.IconContent("Grid.Default", GetTooltipText(tooltipStringFormat, shortcutId));
        }

        /// <summary>
        /// Tooltip String Format for the SelectTool
        /// </summary>
        protected override string tooltipStringFormat
        {
            get { return Styles.tooltipStringFormat; }
        }

        /// <summary>
        /// Shortcut Id for the SelectTool
        /// </summary>
        protected override string shortcutId
        {
            get { return Styles.shortcutId; }
        }

        /// <summary>
        /// Toolbar Icon for the SelectTool
        /// </summary>
        public override GUIContent toolbarIcon
        {
            get { return Styles.toolContent; }
        }
    }
}
