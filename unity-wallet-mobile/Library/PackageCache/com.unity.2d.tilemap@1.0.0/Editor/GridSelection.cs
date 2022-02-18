using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Object = UnityEngine.Object;

namespace UnityEditor.Tilemaps
{
    /// <summary>Stores the selection made on a GridLayout.</summary>
    [MovedFrom(true, "UnityEditor", "UnityEditor")]
    public class GridSelection : ScriptableObject
    {
        /// <summary>Callback for when the active GridSelection has changed.</summary>
        public static event Action gridSelectionChanged;
        private BoundsInt m_Position;
        private GameObject m_Target;
        [SerializeField] private Object m_PreviousSelection;

        /// <summary>Whether there is an active GridSelection made on a GridLayout.</summary>
        public static bool active { get { return Selection.activeObject is GridSelection && selection.m_Target != null; } }

        private static GridSelection selection { get { return Selection.activeObject as GridSelection; } }

        /// <summary>The cell coordinates of the active GridSelection made on the GridLayout.</summary>
        public static BoundsInt position
        {
            get { return selection != null ? selection.m_Position : new BoundsInt(); }
            set
            {
                if (selection != null && selection.m_Position != value)
                {
                    selection.m_Position = value;
                    if (gridSelectionChanged != null)
                        gridSelectionChanged();
                }
            }
        }

        /// <summary>The GameObject of the GridLayout where the active GridSelection was made.</summary>
        public static GameObject target { get { return selection != null ? selection.m_Target : null; } }

        /// <summary>The Grid of the target of the active GridSelection.</summary>
        public static Grid grid { get { return selection != null && selection.m_Target != null ? selection.m_Target.GetComponentInParent<Grid>() : null; } }

        /// <summary>Creates a new GridSelection and sets it as the active GridSelection.</summary>
        /// <param name="target">The target GameObject for the GridSelection.</param>
        /// <param name="bounds">The cell coordinates of selection made.</param>
        public static void Select(Object target, BoundsInt bounds)
        {
            GridSelection newSelection = CreateInstance<GridSelection>();
            newSelection.m_PreviousSelection = Selection.activeObject;
            newSelection.m_Target = target as GameObject;
            newSelection.m_Position = bounds;
            Selection.activeObject = newSelection;
            if (gridSelectionChanged != null)
                gridSelectionChanged();
        }

        /// <summary>Clears the active GridSelection.</summary>
        public static void Clear()
        {
            if (active)
            {
                selection.m_Position = new BoundsInt();
                Selection.activeObject = selection.m_PreviousSelection;
                if (gridSelectionChanged != null)
                    gridSelectionChanged();
            }
        }
    }
}
