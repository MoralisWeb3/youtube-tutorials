using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    internal class TilemapPrefabStageHelper : ScriptableSingleton<TilemapPrefabStageHelper>
    {
        private bool m_RegisteredEventHandlers;

        private static Grid m_Grid;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            instance.RegisterEventHandlers();
        }

        private void OnEnable()
        {
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            if (m_RegisteredEventHandlers)
                return;

            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
            PrefabStage.prefabStageClosing += OnPrefabStageClosing;
            m_RegisteredEventHandlers = true;
        }

        private GameObject CreateGridGameObject(GameObject prefabInstanceRoot)
        {
            // Create Grid root for the Tilemap
            var gridGameObject = EditorUtility.CreateGameObjectWithHideFlags("Grid (Environment)", HideFlags.DontSave);
            SceneManager.MoveGameObjectToScene(gridGameObject, prefabInstanceRoot.scene);
            prefabInstanceRoot.transform.SetParent(gridGameObject.transform, false);
            return gridGameObject;
        }

        private void OnPrefabStageOpened(PrefabStage prefabStage)
        {
            var prefabInstanceRoot = prefabStage.prefabContentsRoot;
            if (prefabStage.mode == PrefabStage.Mode.InIsolation && prefabInstanceRoot.transform.parent != null)
                return;

            var tilemap = prefabInstanceRoot.GetComponentInChildren<Tilemap>();
            if (tilemap == null)
                return;

            if (tilemap.layoutGrid == null)
            {
                GameObject gridGameObject;
                if (prefabStage.mode == PrefabStage.Mode.InIsolation)
                {
                    gridGameObject = CreateGridGameObject(prefabInstanceRoot);
                }
                else
                {
                    gridGameObject = prefabInstanceRoot.transform.parent != null
                        ? prefabInstanceRoot.transform.parent.gameObject
                        : CreateGridGameObject(prefabInstanceRoot);
                }
                if (gridGameObject != null)
                {
                    m_Grid = gridGameObject.AddComponent<Grid>();
                }

                if (m_Grid != null)
                {
                    m_Grid.cellSize = cellSize;
                    m_Grid.cellGap = cellGap;
                    m_Grid.cellLayout = cellLayout;
                    m_Grid.cellSwizzle = cellSwizzle;
                }
            }
        }

        private void OnPrefabStageClosing(PrefabStage prefabStage)
        {
            m_Grid = null;
        }

        private void OnDisable()
        {
            PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
            PrefabStage.prefabStageOpened -= OnPrefabStageOpened;
            m_RegisteredEventHandlers = false;
        }

        internal class OpenTilemapInPrefabModeProperties
        {
            public static readonly string cellSizeXEditorPref = "OpenTilemapInPrefabMode.CellSize.X";
            public static readonly string cellSizeYEditorPref = "OpenTilemapInPrefabMode.CellSize.Y";
            public static readonly string cellSizeZEditorPref = "OpenTilemapInPrefabMode.CellSize.Z";
            public static readonly string cellGapXEditorPref = "OpenTilemapInPrefabMode.CellGap.X";
            public static readonly string cellGapYEditorPref = "OpenTilemapInPrefabMode.CellGap.Y";
            public static readonly string cellGapZEditorPref = "OpenTilemapInPrefabMode.CellGap.Z";
            public static readonly string cellLayoutEditorPref = "OpenTilemapInPrefabMode.CellLayout";
            public static readonly string cellSwizzleEditorPref = "OpenTilemapInPrefabMode.CellSwizzle";

            public static readonly string cellSizeLookup = "Prefab Mode Grid Cell Size";
            public static readonly string cellGapLookup = "Prefab Mode Grid Cell Gap";
            public static readonly string cellLayoutLookup = "Prefab Mode Grid Cell Layout";
            public static readonly string cellSwizzleLookup = "Prefab Mode Grid Cell Swizzle";

            public static readonly GUIContent cellSizeLabel = EditorGUIUtility.TrTextContent(cellSizeLookup, "Cell Size for Grid when opening a Tilemap in Prefab mode without a Grid.");
            public static readonly GUIContent cellGapLabel = EditorGUIUtility.TrTextContent(cellGapLookup, "Cell Gap for Grid when opening a Tilemap in Prefab mode without a Grid.");
            public static readonly GUIContent cellLayoutLabel = EditorGUIUtility.TrTextContent(cellLayoutLookup, "Cell Layout for Grid when opening a Tilemap in Prefab mode without a Grid.");
            public static readonly GUIContent cellSwizzleLabel = EditorGUIUtility.TrTextContent(cellSwizzleLookup, "Cell Swizzle for Grid when opening a Tilemap in Prefab mode without a Grid.");
        }

        internal static Vector3 cellSize
        {
            get
            {
                return new Vector3(
                    EditorPrefs.GetFloat(OpenTilemapInPrefabModeProperties.cellSizeXEditorPref, 1.0f)
                    , EditorPrefs.GetFloat(OpenTilemapInPrefabModeProperties.cellSizeYEditorPref, 1.0f)
                    , EditorPrefs.GetFloat(OpenTilemapInPrefabModeProperties.cellSizeZEditorPref, 0.0f));
            }
            set
            {
                EditorPrefs.SetFloat(OpenTilemapInPrefabModeProperties.cellSizeXEditorPref, value.x);
                EditorPrefs.SetFloat(OpenTilemapInPrefabModeProperties.cellSizeYEditorPref, value.y);
                EditorPrefs.SetFloat(OpenTilemapInPrefabModeProperties.cellSizeZEditorPref, value.z);
            }
        }

        internal static Vector3 cellGap
        {
            get
            {
                return new Vector3(
                    EditorPrefs.GetFloat(OpenTilemapInPrefabModeProperties.cellGapXEditorPref, 0.0f)
                    , EditorPrefs.GetFloat(OpenTilemapInPrefabModeProperties.cellGapYEditorPref, 0.0f)
                    , EditorPrefs.GetFloat(OpenTilemapInPrefabModeProperties.cellGapZEditorPref, 0.0f));
            }
            set
            {
                EditorPrefs.SetFloat(OpenTilemapInPrefabModeProperties.cellGapXEditorPref, value.x);
                EditorPrefs.SetFloat(OpenTilemapInPrefabModeProperties.cellGapYEditorPref, value.y);
                EditorPrefs.SetFloat(OpenTilemapInPrefabModeProperties.cellGapZEditorPref, value.z);
            }
        }

        internal static GridLayout.CellLayout cellLayout
        {
            get
            {
                return (GridLayout.CellLayout)EditorPrefs.GetInt(
                    OpenTilemapInPrefabModeProperties.cellLayoutEditorPref, (int)GridLayout.CellLayout.Rectangle);
            }
            set
            {
                EditorPrefs.SetInt(OpenTilemapInPrefabModeProperties.cellLayoutEditorPref, (int)value);
            }
        }

        internal static GridLayout.CellSwizzle cellSwizzle
        {
            get
            {
                return (GridLayout.CellSwizzle)EditorPrefs.GetInt(
                    OpenTilemapInPrefabModeProperties.cellSwizzleEditorPref, (int)GridLayout.CellSwizzle.XYZ);
            }
            set
            {
                EditorPrefs.SetInt(OpenTilemapInPrefabModeProperties.cellSwizzleEditorPref, (int)value);
            }
        }

        internal static void PreferencesGUI()
        {
            using (new SettingsWindow.GUIScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginChangeCheck();
                var newCellSize = EditorGUILayout.Vector3Field(OpenTilemapInPrefabModeProperties.cellSizeLabel, cellSize);
                if (EditorGUI.EndChangeCheck())
                    cellSize = newCellSize;
                EditorGUI.BeginChangeCheck();
                var newCellGap = EditorGUILayout.Vector3Field(OpenTilemapInPrefabModeProperties.cellGapLabel, cellGap);
                if (EditorGUI.EndChangeCheck())
                    cellGap = newCellGap;
                EditorGUI.BeginChangeCheck();
                var newCellLayout = (GridLayout.CellLayout)EditorGUILayout.EnumPopup(OpenTilemapInPrefabModeProperties.cellLayoutLabel, cellLayout);
                if (EditorGUI.EndChangeCheck())
                    cellLayout = newCellLayout;
                EditorGUI.BeginChangeCheck();
                var newCellSwizzle = (GridLayout.CellSwizzle)EditorGUILayout.EnumPopup(OpenTilemapInPrefabModeProperties.cellSwizzleLabel, cellSwizzle);
                if (EditorGUI.EndChangeCheck())
                    cellSwizzle = newCellSwizzle;
                if (EditorGUI.EndChangeCheck() && m_Grid != null)
                {
                    m_Grid.cellSize = newCellSize;
                    m_Grid.cellGap = newCellGap;
                    m_Grid.cellLayout = newCellLayout;
                    m_Grid.cellSwizzle = newCellSwizzle;
                    SceneView.RepaintAll();
                }
            }
        }
    }
}
