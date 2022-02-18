using System;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// A base class for Editor Tools which work with the Tile Palette
    /// and GridBrushes
    /// </summary>
    public abstract class TilemapEditorTool : EditorTool
    {
        private static EditorTool[] s_TilemapEditorTools = null;
        private static float s_TilemapEditorToolsToolbarSize = 0.0f;

        /// <summary>
        /// All currently active Editor Tools which work with the Tile Palette
        /// </summary>
        public static EditorTool[] tilemapEditorTools
        {
            get
            {
                if (IsCachedEditorToolsInvalid())
                    InstantiateEditorTools();
                return s_TilemapEditorTools;
            }
        }

        /// <summary>
        /// The horizontal size of a Toolbar with all the TilemapEditorTools
        /// </summary>
        public static float tilemapEditorToolsToolbarSize
        {
            get
            {
                if (IsCachedEditorToolsInvalid())
                    InstantiateEditorTools();
                return s_TilemapEditorToolsToolbarSize;
            }
        }

        /// <summary>
        /// Tooltip String format which accepts a shortcut combination as the parameter
        /// </summary>
        protected abstract string tooltipStringFormat { get; }
        /// <summary>
        /// Shortcut Id for this tool
        /// </summary>
        protected abstract string shortcutId { get; }

        /// <summary>
        /// Gets the text for the tooltip given a tooltip string format
        /// and the shortcut combination for a tooltip
        /// </summary>
        /// <param name="tooltipStringFormat">String format which accepts a shortcut combination as the parameter</param>
        /// <param name="shortcutId">Shortcut Id for this tool</param>
        /// <returns>The final text for the tooltip</returns>
        protected static string GetTooltipText(string tooltipStringFormat, string shortcutId)
        {
            return String.Format(tooltipStringFormat, GetKeysFromToolName(shortcutId));
        }

        /// <summary>
        /// Gets the key combination for triggering the shortcut for this tool
        /// </summary>
        /// <param name="id">The shortcut id for this tool</param>
        /// <returns>The key combination for triggering the shortcut for this tool</returns>
        protected static string GetKeysFromToolName(string id)
        {
            return ShortcutIntegration.instance.GetKeyCombinationFor(id);
        }

        /// <summary>
        /// Updates the tooltip whenever there is a change in shortcut combinations
        /// </summary>
        protected void UpdateTooltip()
        {
            toolbarIcon.tooltip = GetTooltipText(tooltipStringFormat, shortcutId);
        }

        /// <summary>
        /// Gets whether the tool is available for use
        /// </summary>
        /// <returns>Whether the tool is available for use</returns>
        public override bool IsAvailable()
        {
            return (GridPaintPaletteWindow.instances.Count > 0) && GridPaintingState.gridBrush;
        }

        internal static void UpdateTooltips()
        {
            if (s_TilemapEditorTools == null)
                InstantiateEditorTools();

            foreach (var editorTool in s_TilemapEditorTools)
            {
                var tilemapEditorTool = editorTool as TilemapEditorTool;
                if (tilemapEditorTool == null)
                    return;

                tilemapEditorTool.UpdateTooltip();
            }
        }

        /// <summary>
        /// Toggles the state of active editor tool with the type passed in.
        /// </summary>
        /// <remarks>
        /// This will change the current active editor tool if the type passed in
        /// is not the same as the current active editor tool. Otherwise, it will
        /// set the View Mode tool as the current active editor tool.
        /// </remarks>
        /// <param name="type">
        /// The type of editor tool. This must be inherited from EditorTool.
        /// </param>
        public static void ToggleActiveEditorTool(Type type)
        {
            if (ToolManager.activeToolType != type)
            {
                SetActiveEditorTool(type);
            }
            else
            {
                // Switch out of TilemapEditorTool if possible
                var lastTool = EditorToolManager.GetLastTool(x => !(x is TilemapEditorTool));
                if (lastTool != null)
                    ToolManager.SetActiveTool(lastTool);
                else
                    ToolManager.SetActiveTool(typeof(ViewModeTool));
            }
        }

        /// <summary>
        /// Sets the current active editor tool to the type passed in
        /// </summary>
        /// <param name="type">The type of editor tool. This must be inherited from TilemapEditorTool</param>
        /// <exception cref="ArgumentException">Throws this if an invalid type parameter is set</exception>
        public static void SetActiveEditorTool(Type type)
        {
            if (type == null || !type.IsSubclassOf(typeof(TilemapEditorTool)))
                throw new ArgumentException("The tool to set must be valid and derive from TilemapEditorTool.");

            EditorTool selectedTool = null;
            foreach (var tool in tilemapEditorTools)
            {
                if (tool.GetType() == type)
                {
                    selectedTool = tool;
                    break;
                }
            }

            if (selectedTool != null)
            {
                ToolManager.SetActiveTool(selectedTool);
            }
        }

        internal static bool IsActive(Type toolType)
        {
            return ToolManager.activeToolType != null && ToolManager.activeToolType == toolType;
        }

        private static bool IsCachedEditorToolsInvalid()
        {
            return s_TilemapEditorTools == null || s_TilemapEditorTools.Length == 0 || s_TilemapEditorTools[0] == null;
        }

        private static void InstantiateEditorTools()
        {
            s_TilemapEditorTools = new EditorTool[]
            {
                CreateInstance<SelectTool>(),
                CreateInstance<MoveTool>(),
                CreateInstance<PaintTool>(),
                CreateInstance<BoxTool>(),
                CreateInstance<PickingTool>(),
                CreateInstance<EraseTool>(),
                CreateInstance<FillTool>()
            };
            GUIStyle toolbarStyle = "Command";
            s_TilemapEditorToolsToolbarSize = s_TilemapEditorTools.Sum(x => toolbarStyle.CalcSize(x.toolbarIcon).x);
        }
    }
}
