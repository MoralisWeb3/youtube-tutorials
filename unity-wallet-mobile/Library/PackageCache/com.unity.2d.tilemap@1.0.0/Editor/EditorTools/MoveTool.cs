using UnityEditor.EditorTools;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Tool for doing a move with the Tile Palette
    /// </summary>
    public sealed class MoveTool : TilemapEditorTool
    {
        private static class Styles
        {
            public static string tooltipStringFormat = "|Move selection with active brush ({0})";
            public static string shortcutId = "Grid Painting/Move";
            public static GUIContent toolContent = EditorGUIUtility.IconContent("Grid.MoveTool", GetTooltipText(tooltipStringFormat, shortcutId));
        }

        /// <summary>
        /// Tooltip String Format for the MoveTool
        /// </summary>
        protected override string tooltipStringFormat
        {
            get { return Styles.tooltipStringFormat; }
        }

        /// <summary>
        /// Shortcut Id for the MoveTool
        /// </summary>
        protected override string shortcutId
        {
            get { return Styles.shortcutId; }
        }

        /// <summary>
        /// Toolbar Icon for the MoveTool
        /// </summary>
        public override GUIContent toolbarIcon
        {
            get { return Styles.toolContent; }
        }
    }
}
