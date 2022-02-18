using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    /// <summary>Base class for Grid Brush Editor.</summary>
    [MovedFrom(true, "UnityEditor", "UnityEditor")]
    [CustomEditor(typeof(GridBrushBase))]
    public class GridBrushEditorBase : Editor
    {
        private static class Styles
        {
            public static readonly Color activeColor = new Color(1f, .5f, 0f);
            public static readonly Color executingColor = new Color(1f, .75f, 0.25f);
        }

        /// <summary>Checks if the Brush allows the changing of Z Position.</summary>
        /// <returns>Whether the Brush can change Z Position.</returns>
        public virtual bool canChangeZPosition
        {
            get { return true; }
            set {}
        }

        /// <summary>Callback for painting the GUI for the GridBrush in the Scene view.</summary>
        /// <param name="gridLayout">Grid that the brush is being used on.</param>
        /// <param name="brushTarget">Target of the GridBrushBase::ref::Tool operation. By default the currently selected GameObject.</param>
        /// <param name="position">Current selected location of the brush.</param>
        /// <param name="tool">Current GridBrushBase::ref::Tool selected.</param>
        /// <param name="executing">Whether is brush is being used.</param>
        /// <remarks>Implement this for any special behaviours when the GridBrush is used on the Scene View.</remarks>
        public virtual void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            OnPaintSceneGUIInternal(gridLayout, brushTarget, position, tool, executing);
        }

        /// <summary>Callback for painting the inspector GUI for the GridBrush in the tilemap palette.</summary>
        /// <remarks>Implement this to have a custom editor in the tilemap palette for the GridBrush.</remarks>
        public virtual void OnPaintInspectorGUI()
        {
            OnInspectorGUI();
        }

        /// <summary>Callback for drawing the Inspector GUI when there is an active GridSelection made in a GridLayout.</summary>
        /// <remarks>Override this to show custom Inspector GUI for the current selection.</remarks>
        public virtual void OnSelectionInspectorGUI() {}

        /// <summary>Callback for painting custom gizmos when there is an active GridSelection made in a GridLayout.</summary>
        /// <param name="gridLayout">Grid that the brush is being used on.</param>
        /// <param name="brushTarget">Target of the GridBrushBase::ref::Tool operation. By default the currently selected GameObject.</param>
        /// <remarks>Override this to show custom gizmos for the current selection.</remarks>
        public virtual void OnSelectionSceneGUI(GridLayout gridLayout, GameObject brushTarget) {}

        /// <summary>
        /// Callback for painting custom gizmos for the GridBrush for the brush target
        /// </summary>
        /// <param name="gridLayout">Grid that the brush is being used on.</param>
        /// <param name="brushTarget">Target of the GridBrushBase::ref::Tool operation. By default the currently selected GameObject.</param>
        /// <remarks>Override this to show custom gizmos for the brush target.</remarks>
        public virtual void OnSceneGUI(GridLayout gridLayout, GameObject brushTarget) {}

        /// <summary>Callback when the mouse cursor leaves a paintable region.</summary>
        /// <remarks>Implement this for any custom behaviour when the mouse cursor leaves a paintable region.</remarks>
        public virtual void OnMouseLeave() {}

        /// <summary>Callback when the mouse cursor enters a paintable region.</summary>
        /// <remarks>Implement this for any custom behaviour when the mouse cursor enters a paintable region.</remarks>
        public virtual void OnMouseEnter() {}

        /// <summary>Callback when a GridBrushBase.Tool is activated.</summary>
        /// <param name="tool">Tool that is activated.</param>
        /// <remarks>Implement this for any special behaviours when a Tool is activated.</remarks>
        public virtual void OnToolActivated(GridBrushBase.Tool tool) {}

        /// <summary>Callback when a GridBrushBase.Tool is deactivated.</summary>
        /// <param name="tool">Tool that is deactivated.</param>
        /// <remarks>Implement this for any special behaviours when a Tool is deactivated.</remarks>
        public virtual void OnToolDeactivated(GridBrushBase.Tool tool) {}

        /// <summary>Callback for registering an Undo action before the GridBrushBase does the current GridBrushBase::ref::Tool action.</summary>
        /// <param name="brushTarget">Target of the GridBrushBase::ref::Tool operation. By default the currently selected GameObject.</param>
        /// <param name="tool">Current GridBrushBase::ref::Tool selected.</param>
        /// <remarks>Implement this for any special Undo behaviours when a brush is used.</remarks>
        public virtual void RegisterUndo(GameObject brushTarget, GridBrushBase.Tool tool) {}

        /// <summary>Returns all valid targets that the brush can edit.</summary>
        public virtual GameObject[] validTargets { get { return null; } }

        internal void OnEditStart(GridLayout gridLayout, GameObject brushTarget)
        {
            SetBufferSyncTile(brushTarget, true);
        }

        internal void OnEditEnd(GridLayout gridLayout, GameObject brushTarget)
        {
            SetBufferSyncTile(brushTarget, false);
        }

        private void SetBufferSyncTile(GameObject brushTarget, bool active)
        {
            if (brushTarget == null || !Tilemap.HasSyncTileCallback())
                return;

            var tilemaps = brushTarget.GetComponentsInChildren<Tilemap>();
            foreach (var tilemap in tilemaps)
            {
                tilemap.bufferSyncTile = active;
            }
        }

        internal static void OnSceneGUIInternal(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (tool == GridBrushBase.Tool.Select ||
                tool == GridBrushBase.Tool.Move)
            {
                if (GridSelection.active && !executing)
                {
                    Color color = Styles.activeColor;
                    GridEditorUtility.DrawGridMarquee(gridLayout, position, color);
                }
            }
        }

        internal static void OnPaintSceneGUIInternal(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            Color color = Color.white;
            if (tool == GridBrushBase.Tool.Pick && executing)
                color = Color.cyan;
            if (tool == GridBrushBase.Tool.Paint && executing)
                color = Color.yellow;

            if (tool == GridBrushBase.Tool.Select ||
                tool == GridBrushBase.Tool.Move)
            {
                if (executing)
                    color = Styles.executingColor;
                else if (GridSelection.active)
                    color = Styles.activeColor;
            }

            if (position.zMin != 0)
            {
                var zeroBounds = position;
                zeroBounds.z = 0;
                GridEditorUtility.DrawGridMarquee(gridLayout, zeroBounds, color);
                color = Color.blue;
            }
            GridEditorUtility.DrawGridMarquee(gridLayout, position, color);
        }
    }
}
