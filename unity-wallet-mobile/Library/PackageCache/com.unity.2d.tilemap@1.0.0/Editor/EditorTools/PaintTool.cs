using UnityEditor.EditorTools;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Tool for doing a paint with the Tile Palette
    /// </summary>
    public sealed class PaintTool : TilemapEditorTool
    {
        private static class Styles
        {
            public static string tooltipStringFormat = "|Paint with active brush ({0})";
            public static string shortcutId = "Grid Painting/Brush";
            public static GUIContent toolContent = EditorGUIUtility.IconContent("Grid.PaintTool", GetTooltipText(tooltipStringFormat, shortcutId));
        }

        /// <summary>
        /// Tooltip String Format for the PaintTool
        /// </summary>
        protected override string tooltipStringFormat
        {
            get { return Styles.tooltipStringFormat; }
        }

        /// <summary>
        /// Shortcut Id for the PaintTool
        /// </summary>
        protected override string shortcutId
        {
            get { return Styles.shortcutId; }
        }

        /// <summary>
        /// Toolbar Icon for the PaintTool
        /// </summary>
        public override GUIContent toolbarIcon
        {
            get { return Styles.toolContent; }
        }
    }
}
