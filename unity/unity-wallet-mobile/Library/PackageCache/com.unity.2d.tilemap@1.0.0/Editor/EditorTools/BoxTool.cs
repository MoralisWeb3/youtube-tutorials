using UnityEditor.EditorTools;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Tool for doing a box fill with the Tile Palette
    /// </summary>
    //[EditorTool("Box Tool", typeof(GridLayout))]
    public sealed class BoxTool : TilemapEditorTool
    {
        private static class Styles
        {
            public static string tooltipStringFormat = "|Paint a filled box with active brush ({0})";
            public static string shortcutId = "Grid Painting/Rectangle";
            public static GUIContent toolContent = EditorGUIUtility.IconContent("Grid.BoxTool", GetTooltipText(tooltipStringFormat, shortcutId));
        }

        /// <summary>
        /// Tooltip String Format for the BoxTool
        /// </summary>
        protected override string tooltipStringFormat
        {
            get { return Styles.tooltipStringFormat; }
        }

        /// <summary>
        /// Shortcut Id for the BoxTool
        /// </summary>
        protected override string shortcutId
        {
            get { return Styles.shortcutId; }
        }

        /// <summary>
        /// Toolbar Icon for the BoxTool
        /// </summary>
        public override GUIContent toolbarIcon
        {
            get { return Styles.toolContent; }
        }
    }
}
