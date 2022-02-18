using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Tool for doing an erase with the Tile Palette
    /// </summary>
    public sealed class EraseTool : TilemapEditorTool
    {
        private static class Styles
        {
            public static string tooltipStringFormat = "|Erase with active brush ({0})";
            public static string shortcutId = "Grid Painting/Erase";
            public static GUIContent toolContent = EditorGUIUtility.IconContent("Grid.EraserTool", GetTooltipText(tooltipStringFormat, shortcutId));
        }

        /// <summary>
        /// Tooltip String Format for the EraseTool
        /// </summary>
        protected override string tooltipStringFormat
        {
            get { return Styles.tooltipStringFormat; }
        }

        /// <summary>
        /// Shortcut Id for the EraseTool
        /// </summary>
        protected override string shortcutId
        {
            get { return Styles.shortcutId; }
        }

        /// <summary>
        /// Toolbar Icon for the EraseTool
        /// </summary>
        public override GUIContent toolbarIcon
        {
            get { return Styles.toolContent; }
        }
    }
}
