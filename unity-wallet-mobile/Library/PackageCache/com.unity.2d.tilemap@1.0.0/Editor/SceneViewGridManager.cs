using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary> This class is in charge of handling Grid component based grid in the scene view (rendering, snapping).
    /// It will hide global scene view grid when it has something to render</summary>
    internal class SceneViewGridManager : ScriptableSingleton<SceneViewGridManager>
    {
        internal static readonly PrefColor sceneViewGridComponentGizmo = new PrefColor("Scene/Grid Component", 255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 25.5f / 255.0f);

        private static Mesh s_GridProxyMesh;
        private static Material s_GridProxyMaterial;
        private static int s_LastGridProxyHash;

        [SerializeField]
        private GridLayout m_ActiveGridProxy;

        private Dictionary<SceneView, bool> m_SceneViewShowGridMap;

        private bool m_RegisteredEventHandlers;

        private bool active { get { return m_ActiveGridProxy != null; } }
        internal GridLayout activeGridProxy { get { return m_ActiveGridProxy; } }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            instance.RegisterEventHandlers();
        }

        private void OnEnable()
        {
            m_SceneViewShowGridMap = new Dictionary<SceneView, bool>();
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            if (m_RegisteredEventHandlers)
                return;

            SceneView.duringSceneGui += OnSceneGuiDelegate;
            Selection.selectionChanged += UpdateCache;
            EditorApplication.hierarchyChanged += UpdateCache;
            UnityEditor.EditorTools.ToolManager.activeToolChanged += ActiveToolChanged;
            EditorApplication.quitting += EditorQuitting;
            GridPaintingState.brushChanged += OnBrushChanged;
            GridPaintingState.scenePaintTargetChanged += OnScenePaintTargetChanged;
            GridSnapping.snapPosition = OnSnapPosition;
            GridSnapping.activeFunc = GetActive;

            m_RegisteredEventHandlers = true;
        }

        private void OnBrushChanged(GridBrushBase brush)
        {
            UpdateCache();
        }

        private void ActiveToolChanged()
        {
            UpdateCache();
        }

        private void OnScenePaintTargetChanged(GameObject scenePaintTarget)
        {
            UpdateCache();
        }

        private void OnDisable()
        {
            FlushCachedGridProxy();
            RestoreSceneViewShowGrid();
            SceneView.duringSceneGui -= OnSceneGuiDelegate;
            Selection.selectionChanged -= UpdateCache;
            EditorApplication.hierarchyChanged -= UpdateCache;
            EditorApplication.quitting -= EditorQuitting;
            UnityEditor.EditorTools.ToolManager.activeToolChanged -= ActiveToolChanged;
            GridPaintingState.brushChanged -= OnBrushChanged;
            GridPaintingState.scenePaintTargetChanged -= OnScenePaintTargetChanged;
            GridSnapping.snapPosition = null;
            GridSnapping.activeFunc = null;
            m_RegisteredEventHandlers = false;
        }

        private void UpdateCache()
        {
            GridLayout gridProxy;
            if (PaintableGrid.InGridEditMode())
                gridProxy = GridPaintingState.scenePaintTarget != null ? GridPaintingState.scenePaintTarget.GetComponentInParent<GridLayout>() : null;
            else
                gridProxy = Selection.activeGameObject != null ? Selection.activeGameObject.GetComponentInParent<GridLayout>() : null;

            if (gridProxy != m_ActiveGridProxy)
            {
                if (m_ActiveGridProxy == null)
                {
                    // Disable SceneView grid if there is now a GridProxy. Store user settings to be restored.
                    StoreSceneViewShowGrid(false);
                }
                else if (gridProxy == null)
                {
                    RestoreSceneViewShowGrid();
                }
                m_ActiveGridProxy = gridProxy;
                FlushCachedGridProxy();
                SceneView.RepaintAll();
            }
        }

        private void EditorQuitting()
        {
            if (NeedsRestoreSceneViewShowGrid())
            {
                RestoreSceneViewShowGrid();
                // SceneView.showGrid is part of default window preferences
                WindowLayout.SaveDefaultWindowPreferences();
            }
        }

        private void OnSceneGuiDelegate(SceneView sceneView)
        {
            if (active)
                DrawGrid(activeGridProxy);
        }

        private static int GenerateHash(GridLayout layout, Color color)
        {
            int hash = 0x7ed55d16;
            hash ^= layout.cellSize.GetHashCode();
            hash ^= layout.cellLayout.GetHashCode() << 23;
            hash ^= (layout.cellGap.GetHashCode() << 4) + 0x165667b1;
            hash ^= layout.cellSwizzle.GetHashCode() << 7;
            hash ^= color.GetHashCode();
            return hash;
        }

        private static void DrawGrid(GridLayout gridLayout)
        {
            int gridHash = GenerateHash(gridLayout, sceneViewGridComponentGizmo.Color);
            if (s_LastGridProxyHash != gridHash)
            {
                FlushCachedGridProxy();
                s_LastGridProxyHash = gridHash;
            }
            GridEditorUtility.DrawGridGizmo(gridLayout, gridLayout.transform, sceneViewGridComponentGizmo.Color, ref s_GridProxyMesh, ref s_GridProxyMaterial);
        }

        private bool NeedsRestoreSceneViewShowGrid()
        {
            return m_SceneViewShowGridMap.Count > 0;
        }

        private void StoreSceneViewShowGrid(bool value)
        {
            m_SceneViewShowGridMap.Clear();
            foreach (SceneView sceneView in SceneView.sceneViews)
            {
                m_SceneViewShowGridMap.Add(sceneView, sceneView.showGrid);
                sceneView.showGrid = value;
            }
        }

        private void RestoreSceneViewShowGrid()
        {
            foreach (var item in m_SceneViewShowGridMap)
            {
                var sceneView = item.Key;
                if (sceneView != null)
                    sceneView.showGrid = item.Value;
            }
            m_SceneViewShowGridMap.Clear();
        }

        private bool GetActive()
        {
            return active;
        }

        internal Vector3 OnSnapPosition(Vector3 position)
        {
            Vector3 result = position;
            if (active && (EditorSnapSettings.hotkeyActive || EditorSnapSettings.gridSnapActive))
            {
                // This will automatically prefer the Grid
                Vector3 local = activeGridProxy.WorldToLocal(position);
                Vector3 interpolatedCell = activeGridProxy.LocalToCellInterpolated(local);

                Vector3 inverse = Vector3.one;
                inverse.x = Mathf.Approximately(EditorSnapSettings.move.x, 0.0f) ? 1.0f : 1.0f / EditorSnapSettings.move.x;
                inverse.y = Mathf.Approximately(EditorSnapSettings.move.y, 0.0f) ? 1.0f : 1.0f / EditorSnapSettings.move.y;
                inverse.z = Mathf.Approximately(EditorSnapSettings.move.z, 0.0f) ? 1.0f : 1.0f / EditorSnapSettings.move.z;

                Vector3 roundedCell = new Vector3(
                    Mathf.Round(inverse.x * interpolatedCell.x) / inverse.x,
                    Mathf.Round(inverse.y * interpolatedCell.y) / inverse.y,
                    Mathf.Round(inverse.z * interpolatedCell.z) / inverse.z
                );

                local = activeGridProxy.CellToLocalInterpolated(roundedCell);
                result = activeGridProxy.LocalToWorld(local);
            }
            return result;
        }

        internal static void FlushCachedGridProxy()
        {
            if (s_GridProxyMesh == null)
                return;

            DestroyImmediate(s_GridProxyMesh);
            s_GridProxyMesh = null;
            s_GridProxyMaterial = null;
        }
    }
}
