using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Tool for doing a flood fill with the Tile Palette
    /// </summary>
    public sealed class FillTool : TilemapEditorTool
    {
        private static class Styles
        {
            public static string tooltipStringFormat = "|Flood fill with active brush ({0})";
            public static string shortcutId = "Grid Painting/Fill";
            public static GUIContent toolContent = EditorGUIUtility.IconContent("Grid.FillTool", GetTooltipText(tooltipStringFormat, shortcutId));
        }

        /// <summary>
        /// Tooltip String Format for the FillTool
        /// </summary>
        protected override string tooltipStringFormat
        {
            get { return Styles.tooltipStringFormat; }
        }

        /// <summary>
        /// Shortcut Id for the FillTool
        /// </summary>
        protected override string shortcutId
        {
            get { return Styles.shortcutId; }
        }

        /// <summary>
        /// Toolbar Icon for the FillTool
        /// </summary>
        public override GUIContent toolbarIcon
        {
            get { return Styles.toolContent; }
        }
    }
}
