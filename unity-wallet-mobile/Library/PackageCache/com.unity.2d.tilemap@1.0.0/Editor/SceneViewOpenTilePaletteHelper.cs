using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace UnityEditor.Tilemaps
{
    internal class SceneViewOpenTilePaletteHelper : ScriptableSingleton<SceneViewOpenTilePaletteHelper>
    {
        private class Styles
        {
            public static readonly GUIContent overlayTitleLabel = EditorGUIUtility.TrTextContent("Open Tile Palette");
            public static readonly GUIContent openContent = EditorGUIUtility.IconContent("Tilemap Icon", "Open Tile Palette|Opens Tile Palette Window");
        }

        private bool m_RegisteredEventHandlers;
        private bool m_IsSelectionValid;

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

            SceneView.duringSceneGui += DuringSceneGUI;
            Selection.selectionChanged += SelectionChanged;
            EditorApplication.hierarchyChanged += SelectionChanged;

            m_IsSelectionValid = IsSelectionValid();

            m_RegisteredEventHandlers = true;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
            Selection.selectionChanged -= SelectionChanged;
            EditorApplication.hierarchyChanged -= SelectionChanged;
            m_RegisteredEventHandlers = false;
        }

        internal static void OpenTilePalette()
        {
            GridPaintPaletteWindow.OpenTilemapPalette();

            var target = Selection.activeGameObject;
            if (target != null)
            {
                if (PrefabUtility.IsPartOfPrefabAsset(target))
                {
                    var path = AssetDatabase.GetAssetPath(target);
                    if (AssetDatabase.LoadAssetAtPath<GridPalette>(path))
                    {
                        GridPaintingState.palette = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    }
                }
                else if (GridPaintingState.validTargets != null)
                {
                    var grid = target.GetComponent<GridLayout>();
                    if (grid != null)
                    {
                        foreach (var validTarget in GridPaintingState.validTargets)
                        {
                            if (validTarget == target)
                            {
                                GridPaintingState.scenePaintTarget = target;
                                break;
                            }
                        }
                    }
                }
            }
        }

        internal static bool IsActive()
        {
            if (GridPaintPaletteWindow.isActive)
                return false;
            return instance.m_IsSelectionValid;
        }

        internal static bool IsSelectionValid()
        {
            if (Selection.activeObject == null)
                return false;
            if (Selection.activeObject is TileBase)
                return true;
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<GridLayout>() != null)
                return true;
            return false;
        }

        private void DuringSceneGUI(SceneView sceneView)
        {
            if (!showInSceneViewActive || !IsActive())
                return;

            SceneViewOverlay.Window(Styles.overlayTitleLabel, OnSceneViewDisplayGUI, (int)SceneViewOverlay.Ordering.TilemapRenderer + 1, SceneViewOverlay.WindowDisplayOption.OneWindowPerTitle);
        }

        private void SelectionChanged()
        {
            m_IsSelectionValid = IsSelectionValid();
        }

        private void OnSceneViewDisplayGUI(Object displayTarget, SceneView sceneView)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Styles.openContent, GUILayout.Height(32), GUILayout.Width(32)))
            {
                OpenTilePalette();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        internal class SceneViewOpenTilePaletteProperties
        {
            public static readonly string showInSceneViewEditorPref = "OpenTilePalette.ShowInSceneView";
            public static readonly string showInSceneViewLookup = "Show Open Tile Palette in Scene View";

            public static readonly GUIContent showInSceneViewLabel = EditorGUIUtility.TrTextContent(showInSceneViewLookup, "Shows an overlay in the SceneView for opening the Tile Palette when selecting an object that interacts with the Tile Palette.");
        }

        internal static bool showInSceneViewActive
        {
            get { return EditorPrefs.GetBool(SceneViewOpenTilePaletteProperties.showInSceneViewEditorPref, true); }
            set { EditorPrefs.SetBool(SceneViewOpenTilePaletteProperties.showInSceneViewEditorPref, value); }
        }

        internal static void PreferencesGUI()
        {
            using (new SettingsWindow.GUIScope())
            {
                EditorGUI.BeginChangeCheck();
                var val = EditorGUILayout.Toggle(SceneViewOpenTilePaletteProperties.showInSceneViewLabel, showInSceneViewActive);
                if (EditorGUI.EndChangeCheck())
                {
                    showInSceneViewActive = val;
                    SceneView.RepaintAll();
                }
            }
        }
    }
}
