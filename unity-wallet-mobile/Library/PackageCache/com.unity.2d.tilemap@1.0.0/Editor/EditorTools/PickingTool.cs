using UnityEditor.EditorTools;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Tool for doing a picking action with the Tile Palette
    /// </summary>
    public sealed class PickingTool : TilemapEditorTool
    {
        private static class Styles
        {
            public static string tooltipStringFormat = "|Pick or marquee select new brush ({0})";
            public static string shortcutId = "Grid Painting/Picker";
            public static GUIContent toolContent = EditorGUIUtility.IconContent("Grid.PickingTool", GetTooltipText(tooltipStringFormat, shortcutId));
        }

        /// <summary>
        /// Tooltip String Format for the PickingTool
        /// </summary>
        protected override string tooltipStringFormat
        {
            get { return Styles.tooltipStringFormat; }
        }

        /// <summary>
        /// Shortcut Id for the PickingTool
        /// </summary>
        protected override string shortcutId
        {
            get { return Styles.shortcutId; }
        }

        /// <summary>
        /// Toolbar Icon for the PickingTool
        /// </summary>
        public override GUIContent toolbarIcon
        {
            get { return Styles.toolContent; }
        }
    }
}
