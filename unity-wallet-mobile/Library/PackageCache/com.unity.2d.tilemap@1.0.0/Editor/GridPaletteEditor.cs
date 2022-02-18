using System;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Editor for GridPalette
    /// </summary>
    [CustomEditor(typeof(GridPalette))]
    public class GridPaletteEditor : Editor
    {
        private static class Styles
        {
            public static readonly GUIContent cellSizingLabel = EditorGUIUtility.TrTextContent("Cell Sizing", "Determines the sizing of cells based on Tiles in the Palette");
            public static readonly GUIContent transparencySortModeLabel = EditorGUIUtility.TrTextContent("Sort Mode", "Determines the transparency sorting mode of renderers in the Palette");
            public static readonly GUIContent transparencySortAxisLabel = EditorGUIUtility.TrTextContent("Sort Axis", "Determines the sorting axis if the transparency sort mode is set to Custom Axis Sort");
        }

        private SerializedProperty m_CellSizing;
        private SerializedProperty m_TransparencySortMode;
        private SerializedProperty m_TransparencySortAxis;

        private int m_CustomAxisIndex;

        private void OnEnable()
        {
            m_CellSizing = serializedObject.FindProperty("cellSizing");
            m_TransparencySortMode = serializedObject.FindProperty("m_TransparencySortMode");
            m_TransparencySortAxis = serializedObject.FindProperty("m_TransparencySortAxis");
            m_CustomAxisIndex = Array.IndexOf(Enum.GetValues(typeof(TransparencySortMode)), TransparencySortMode.CustomAxis);
        }

        /// <summary>
        /// Draws the Inspector GUI for a GridPalette
        /// </summary>
        public override void OnInspectorGUI()
        {
            m_SerializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CellSizing, Styles.cellSizingLabel);
            EditorGUILayout.PropertyField(m_TransparencySortMode, Styles.transparencySortModeLabel);
            using (new EditorGUI.DisabledScope(m_TransparencySortMode.enumValueIndex != m_CustomAxisIndex))
            {
                EditorGUILayout.PropertyField(m_TransparencySortAxis, Styles.transparencySortAxisLabel);
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_SerializedObject.ApplyModifiedProperties();
                if (AssetDatabase.GetAssetPath(GridPaintingState.palette) == AssetDatabase.GetAssetPath(target))
                {
                    GridPaintingState.UpdateActiveGridPalette();
                    GridPaintingState.RepaintGridPaintPaletteWindow();
                }
            }
        }
    }
}
