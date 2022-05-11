using UnityEngine;

namespace UnityEditor.Tilemaps
{
    [CustomEditor(typeof(GridSelection))]
    internal class GridSelectionEditor : Editor
    {
        private const float iconSize = 32f;

        static class Styles
        {
            public static readonly GUIContent gridSelectionLabel = EditorGUIUtility.TrTextContent("Grid Selection");
        }

        private void OnEnable()
        {
            // Give focus to Inspector window for keyboard actions
            EditorWindow.FocusWindowIfItsOpen<InspectorWindow>();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            if (GridPaintingState.activeBrushEditor && GridSelection.active)
            {
                GridPaintingState.activeBrushEditor.OnSelectionInspectorGUI();
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (GridPaintingState.IsPartOfActivePalette(GridSelection.target))
                {
                    GridPaintingState.UnlockGridPaintPaletteClipboardForEditing();
                    GridPaintingState.RepaintGridPaintPaletteWindow();
                }
            }
        }

        protected override void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.inspectorBig);
            Texture2D icon = AssetPreview.GetMiniTypeThumbnail(typeof(Grid));
            GUILayout.Label(icon, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
            EditorGUILayout.BeginVertical();
            GUILayout.Label(Styles.gridSelectionLabel);
            GridSelection.position = EditorGUILayout.BoundsIntField(GUIContent.none, GridSelection.position);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            DrawHeaderHelpAndSettingsGUI(GUILayoutUtility.GetLastRect());
        }

        public bool HasFrameBounds()
        {
            return GridSelection.active;
        }

        public Bounds OnGetFrameBounds()
        {
            Bounds bounds = new Bounds();
            if (GridSelection.active)
            {
                Vector3Int gridMin = GridSelection.position.min;
                Vector3Int gridMax = GridSelection.position.max;

                Vector3 min = GridSelection.grid.CellToWorld(gridMin);
                Vector3 max = GridSelection.grid.CellToWorld(gridMax);

                bounds = new Bounds((max + min) * .5f, max - min);
            }

            return bounds;
        }
    }
}
