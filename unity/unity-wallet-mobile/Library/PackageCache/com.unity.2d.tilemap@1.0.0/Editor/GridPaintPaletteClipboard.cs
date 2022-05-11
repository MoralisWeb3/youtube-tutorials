using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Event = UnityEngine.Event;
using Object = UnityEngine.Object;

namespace UnityEditor.Tilemaps
{
    internal class GridPaintPaletteClipboard : PaintableGrid
    {
        static class Styles
        {
            public static readonly GUIStyle background = "CurveEditorBackground";
        }

        private static readonly string paletteSavedOutsideClipboard = L10n.Tr("Palette Asset {0} was changed outside of the Tile Palette. All changes in the Tile Palette made will be reverted.");

        private bool m_PaletteNeedsSave;
        private const float k_ZoomSpeed = 7f;
        private const float k_MinZoom = 10f; // How many pixels per cell at minimum
        private const float k_MaxZoom = 100f; // How many pixels per cell at maximum
        private const float k_Padding = 0.75f; // How many percentages of window size is the empty padding around the palette content

        private int m_KeyboardPanningID;
        private int m_MousePanningID;

        private float k_KeyboardPanningSpeed = 3.0f;

        private Vector3 m_KeyboardPanning;

        private Rect m_GUIRect = new Rect(0, 0, 200, 200);

        private bool m_OldFog;

        public Rect guiRect
        {
            get { return m_GUIRect; }
            set
            {
                if (m_GUIRect != value)
                {
                    Rect oldValue = m_GUIRect;
                    m_GUIRect = value;
                    OnViewSizeChanged(oldValue, m_GUIRect);
                }
            }
        }

        [SerializeField] private GridPaintPaletteWindow m_Owner;

        public bool activeDragAndDrop { get { return DragAndDrop.objectReferences.Length > 0 && guiRect.Contains(Event.current.mousePosition); } }

        [SerializeField] private bool m_CameraInitializedToBounds;
        [SerializeField] public bool m_CameraPositionSaved;
        [SerializeField] public Vector3 m_CameraPosition;
        [SerializeField] public float m_CameraOrthographicSize;

        private RectInt? m_ActivePick;
        private Dictionary<Vector2Int, Object> m_HoverData;
        private bool m_Unlocked;
        private bool m_PingTileAsset;

        public GameObject palette { get { return m_Owner.palette; } }
        public GameObject paletteInstance { get { return m_Owner.paletteInstance; } }
        public Tilemap tilemap { get { return paletteInstance != null ? paletteInstance.GetComponentInChildren<Tilemap>() : null; } }
        private Grid grid { get { return paletteInstance != null ? paletteInstance.GetComponent<Grid>() : null; } }
        private Grid prefabGrid { get { return palette != null ? palette.GetComponent<Grid>() : null; } }
        public PreviewRenderUtility previewUtility { get { return m_Owner.previewUtility; } }

        private GridBrushBase gridBrush { get { return GridPaintingState.gridBrush; } }

        private Mesh m_GridMesh;
        private int m_LastGridHash;
        private Material m_GridMaterial;
        private static readonly Color k_GridColor = Color.white.AlphaMultiplied(0.1f);

        private bool m_PaletteUsed; // We mark palette used, when it has been changed in any way during being actively open.
        private Vector2? m_PreviousMousePosition;

        public TileBase activeTile
        {
            get
            {
                if (m_ActivePick.HasValue && m_ActivePick.Value.size == Vector2Int.one && GridPaintingState.defaultBrush != null && GridPaintingState.defaultBrush.cellCount > 0)
                    return GridPaintingState.defaultBrush.cells[0].tile;
                return null;
            }
        }

        // TODO: Faster codepath for this
        private RectInt bounds
        {
            get
            {
                if (tilemap == null)
                    return new RectInt();

                var origin = tilemap.origin;
                var size = tilemap.size;

                RectInt r = new RectInt(origin.x, origin.y, size.x, size.y);
                if (TilemapIsEmpty(tilemap))
                    return r;

                int minX = origin.x + size.x;
                int minY = origin.y + size.y;
                int maxX = origin.x;
                int maxY = origin.y;

                foreach (Vector2Int pos in r.allPositionsWithin)
                {
                    if (tilemap.GetTile(new Vector3Int(pos.x, pos.y, 0)) != null)
                    {
                        minX = Math.Min(minX, pos.x);
                        minY = Math.Min(minY, pos.y);
                        maxX = Math.Max(maxX, pos.x);
                        maxY = Math.Max(maxY, pos.y);
                    }
                }
                return new RectInt(minX, minY, maxX - minX + 1, maxY - minY + 1);
            }
        }

        // Max area we are ever showing. Depends on the zoom level and content of palette.
        private Rect paddedBounds
        {
            get
            {
                var GUIAspect = m_GUIRect.width / m_GUIRect.height;
                var orthographicSize = previewUtility.camera.orthographicSize;
                var paddingW = orthographicSize * GUIAspect * k_Padding * 2f;
                var paddingH = orthographicSize * k_Padding * 2f;

                Bounds localBounds = grid.GetBoundsLocal(
                    new Vector3(bounds.xMin, bounds.yMin, 0.0f),
                    new Vector3(bounds.size.x, bounds.size.y, 0.0f));
                Rect result = new Rect(
                    new Vector2(localBounds.min.x - paddingW, localBounds.min.y - paddingH),
                    new Vector2(localBounds.size.x + paddingW * 2f, localBounds.size.y + paddingH * 2f));

                return result;
            }
        }

        private RectInt paddedBoundsInt
        {
            get
            {
                Vector3Int min = grid.LocalToCell(paddedBounds.min);
                Vector3Int max = grid.LocalToCell(paddedBounds.max) + Vector3Int.one;
                return new RectInt(min.x, min.y, max.x - min.x, max.y - min.y);
            }
        }

        private GameObject brushTarget
        {
            get
            {
                return (tilemap != null) ? tilemap.gameObject : (grid != null) ? grid.gameObject : null;
            }
        }

        public bool unlocked
        {
            get { return m_Unlocked; }
            set
            {
                if (value == false && m_Unlocked)
                {
                    if (tilemap != null)
                        tilemap.ClearAllEditorPreviewTiles();
                    SavePaletteIfNecessary();
                }
                m_Unlocked = value;
            }
        }

        public bool pingTileAsset
        {
            get { return m_PingTileAsset; }
            set
            {
                if (value && !m_PingTileAsset && m_ActivePick.HasValue) { PingTileAsset(m_ActivePick.Value); }
                m_PingTileAsset = value;
            }
        }

        public bool invalidClipboard { get { return m_Owner.palette == null; } }
        public bool isReceivingDragAndDrop { get { return m_HoverData != null && m_HoverData.Count > 0; } }

        public bool showNewEmptyClipboardInfo
        {
            get
            {
                if (paletteInstance == null)
                    return false;

                if (tilemap == null)
                    return false;

                if (unlocked && inEditMode)
                    return false;

                if (!TilemapIsEmpty(tilemap))
                    return false;

                if (tilemap.transform.childCount > 0)
                    return false;

                if (isReceivingDragAndDrop)
                    return false;

                // If user happens to erase the last content of used palette, we don't want to show the new palette info anymore
                if (m_PaletteUsed)
                    return false;

                return true;
            }
        }

        public bool isModified { get { return m_PaletteNeedsSave; } }

        public GridPaintPaletteWindow owner
        {
            set { m_Owner = value; }
        }

        public void OnBeforePaletteSelectionChanged()
        {
            SavePaletteIfNecessary();
            DestroyPreviewInstance();
            FlushHoverData();
        }

        private void FlushHoverData()
        {
            if (m_HoverData != null)
            {
                m_HoverData.Clear();
                m_HoverData = null;
            }
        }

        public void OnAfterPaletteSelectionChanged()
        {
            m_PaletteUsed = false;
            ResetPreviewInstance();

            if (palette != null)
                ResetPreviewCamera();
        }

        public void SetupPreviewCameraOnInit()
        {
            if (m_CameraPositionSaved)
                LoadSavedCameraPosition();
            else
                ResetPreviewCamera();
        }

        private void LoadSavedCameraPosition()
        {
            previewUtility.camera.transform.position = m_CameraPosition;
            previewUtility.camera.orthographicSize = m_CameraOrthographicSize;
            previewUtility.camera.nearClipPlane = 0.01f;
            previewUtility.camera.farClipPlane = 100f;
        }

        private void ResetPreviewCamera()
        {
            var transform = previewUtility.camera.transform;
            transform.position = new Vector3(0, 0, -10f);
            transform.rotation = Quaternion.identity;
            previewUtility.camera.nearClipPlane = 0.01f;
            previewUtility.camera.farClipPlane = 100f;
            FrameEntirePalette();
        }

        private void DestroyPreviewInstance()
        {
            if (m_Owner != null)
                m_Owner.DestroyPreviewInstance();
        }

        private void ResetPreviewInstance()
        {
            m_Owner.ResetPreviewInstance();
        }

        public void ResetPreviewMesh()
        {
            if (m_GridMesh != null)
            {
                DestroyImmediate(m_GridMesh);
                m_GridMesh = null;
            }
            m_GridMaterial = null;
        }

        public void FrameEntirePalette()
        {
            Frame(bounds);
        }

        void Frame(RectInt rect)
        {
            if (grid == null)
                return;

            var position = grid.CellToLocalInterpolated(new Vector3(rect.center.x, rect.center.y, 0));
            position.z = -10f;
            previewUtility.camera.transform.position = position;

            var height = (grid.CellToLocal(new Vector3Int(0, rect.yMax, 0)) - grid.CellToLocal(new Vector3Int(0, rect.yMin, 0))).magnitude;
            var width = (grid.CellToLocal(new Vector3Int(rect.xMax, 0, 0)) - grid.CellToLocal(new Vector3Int(rect.xMin, 0, 0))).magnitude;

            var cellSize = grid.cellSize;
            width += cellSize.x;
            height += cellSize.y;

            var GUIAspect = m_GUIRect.width / m_GUIRect.height;
            var contentAspect = width / height;
            previewUtility.camera.orthographicSize = (GUIAspect > contentAspect ? height : width / GUIAspect) / 2f;

            ClampZoomAndPan();
        }

        private void RefreshAllTiles()
        {
            if (tilemap != null)
                tilemap.RefreshAllTiles();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EditorApplication.editorApplicationQuit += EditorApplicationQuit;
            Undo.undoRedoPerformed += UndoRedoPerformed;
            m_KeyboardPanningID = GUIUtility.GetPermanentControlID();
            m_MousePanningID = GUIUtility.GetPermanentControlID();
        }

        protected override void OnDisable()
        {
            if (m_Owner && previewUtility != null && previewUtility.camera != null)
            {
                // Save Preview camera coordinates
                m_CameraPosition = previewUtility.camera.transform.position;
                m_CameraOrthographicSize = previewUtility.camera.orthographicSize;
                m_CameraPositionSaved = true;
            }

            SavePaletteIfNecessary();
            DestroyPreviewInstance();
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            EditorApplication.editorApplicationQuit -= EditorApplicationQuit;
            base.OnDisable();
        }

        public override void OnGUI()
        {
            if (Mathf.Approximately(guiRect.width, 0f) || Mathf.Approximately(guiRect.height, 0f))
                return;

            UpdateMouseGridPosition();

            HandleDragAndDrop();

            if (palette == null)
                return;

            HandlePanAndZoom();

            if (showNewEmptyClipboardInfo)
                return;

            if (Event.current.type == EventType.Repaint && !m_CameraInitializedToBounds)
            {
                Frame(bounds);
                m_CameraInitializedToBounds = true;
            }

            HandleMouseEnterLeave();

            if (guiRect.Contains(Event.current.mousePosition) || Event.current.type != EventType.MouseDown)
                base.OnGUI();

            if (Event.current.type == EventType.Repaint)
                Render();
            else
                DoBrush();

            m_PreviousMousePosition = Event.current.mousePosition;
        }

        public void OnViewSizeChanged(Rect oldSize, Rect newSize)
        {
            if (Mathf.Approximately(oldSize.height * oldSize.width * newSize.height * newSize.width, 0f))
                return;

            Camera cam = previewUtility.camera;

            Vector2 sizeDelta = new Vector2(
                newSize.width / LocalToScreenRatio(newSize.height) - oldSize.width / LocalToScreenRatio(oldSize.height),
                newSize.height / LocalToScreenRatio(newSize.height) - oldSize.height / LocalToScreenRatio(oldSize.height));

            cam.transform.Translate(sizeDelta / 2f);

            ClampZoomAndPan();
        }

        private void EditorApplicationQuit()
        {
            SavePaletteIfNecessary();
        }

        private void UndoRedoPerformed()
        {
            if (unlocked)
            {
                m_PaletteNeedsSave = true;
                RefreshAllTiles();
                Repaint();
            }
        }

        private void HandlePanAndZoom()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (MousePanningEvent() && guiRect.Contains(Event.current.mousePosition) && GUIUtility.hotControl == 0)
                    {
                        GUIUtility.hotControl = m_MousePanningID;
                        Event.current.Use();
                    }
                    break;
                case EventType.ValidateCommand:
                    if (Event.current.commandName == EventCommandNames.FrameSelected)
                    {
                        Event.current.Use();
                    }
                    break;
                case EventType.ExecuteCommand:
                    if (Event.current.commandName == EventCommandNames.FrameSelected)
                    {
                        if (m_ActivePick.HasValue)
                            Frame(m_ActivePick.Value);
                        else
                            FrameEntirePalette();
                        Event.current.Use();
                    }
                    break;
                case EventType.ScrollWheel:
                    if (guiRect.Contains(Event.current.mousePosition))
                    {
                        float zoomDelta = HandleUtility.niceMouseDeltaZoom * (Event.current.shift ? -9 : -3) * k_ZoomSpeed;
                        Camera camera = previewUtility.camera;
                        Vector3 oldLocalPos = ScreenToLocal(Event.current.mousePosition);
                        camera.orthographicSize = Mathf.Max(.0001f, camera.orthographicSize * (1 + zoomDelta * .001f));
                        ClampZoomAndPan();
                        Vector3 newLocalPos = ScreenToLocal(Event.current.mousePosition);
                        Vector3 localDelta = newLocalPos - oldLocalPos;
                        camera.transform.position -= localDelta;
                        ClampZoomAndPan();
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == m_MousePanningID)
                    {
                        Vector3 delta = new Vector3(-Event.current.delta.x, Event.current.delta.y, 0f) / LocalToScreenRatio();
                        previewUtility.camera.transform.Translate(delta);
                        ClampZoomAndPan();
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseMove: // Fix mouse cursor being stuck when panning ended outside our window
                    if (GUIUtility.hotControl == m_MousePanningID && !MousePanningEvent())
                        GUIUtility.hotControl = 0;
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == m_MousePanningID)
                    {
                        ClampZoomAndPan();
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;
                case EventType.KeyDown:
                    if (GUIUtility.hotControl == 0)
                    {
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.LeftArrow:
                                m_KeyboardPanning = new Vector3(-k_KeyboardPanningSpeed, 0f) / LocalToScreenRatio();
                                GUIUtility.hotControl = m_KeyboardPanningID;
                                Event.current.Use();
                                break;
                            case KeyCode.RightArrow:
                                m_KeyboardPanning = new Vector3(k_KeyboardPanningSpeed, 0f) / LocalToScreenRatio();
                                GUIUtility.hotControl = m_KeyboardPanningID;
                                Event.current.Use();
                                break;
                            case KeyCode.UpArrow:
                                m_KeyboardPanning = new Vector3(0f, k_KeyboardPanningSpeed) / LocalToScreenRatio();
                                GUIUtility.hotControl = m_KeyboardPanningID;
                                Event.current.Use();
                                break;
                            case KeyCode.DownArrow:
                                m_KeyboardPanning = new Vector3(0f, -k_KeyboardPanningSpeed) / LocalToScreenRatio();
                                GUIUtility.hotControl = m_KeyboardPanningID;
                                Event.current.Use();
                                break;
                        }
                    }
                    break;
                case EventType.KeyUp:
                    if (GUIUtility.hotControl == m_KeyboardPanningID)
                    {
                        m_KeyboardPanning = Vector3.zero;
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;
                case EventType.Repaint:
                    if (GUIUtility.hotControl == m_KeyboardPanningID)
                    {
                        previewUtility.camera.transform.Translate(m_KeyboardPanning);
                        ClampZoomAndPan();
                        Repaint();
                    }

                    if (GUIUtility.hotControl == m_MousePanningID)
                        EditorGUIUtility.AddCursorRect(guiRect, MouseCursor.Pan);

                    break;
            }
        }

        private static bool MousePanningEvent()
        {
            return (Event.current.button == 0 && Event.current.alt || Event.current.button > 0);
        }

        public void ClampZoomAndPan()
        {
            float pixelsPerCell = grid.cellSize.y * LocalToScreenRatio();
            if (pixelsPerCell < k_MinZoom)
                previewUtility.camera.orthographicSize = (grid.cellSize.y * guiRect.height) / (k_MinZoom * 2f);
            else if (pixelsPerCell > k_MaxZoom)
                previewUtility.camera.orthographicSize = (grid.cellSize.y * guiRect.height) / (k_MaxZoom * 2f);

            Camera cam = previewUtility.camera;
            float cameraOrthographicSize = cam.orthographicSize;
            Rect r = paddedBounds;

            Vector3 camPos = cam.transform.position;
            Vector2 camMin = camPos - new Vector3(cameraOrthographicSize * (guiRect.width / guiRect.height), cameraOrthographicSize);
            Vector2 camMax = camPos + new Vector3(cameraOrthographicSize * (guiRect.width / guiRect.height), cameraOrthographicSize);

            if (camMin.x < r.min.x)
            {
                camPos += new Vector3(r.min.x - camMin.x, 0f, 0f);
            }
            if (camMin.y < r.min.y)
            {
                camPos += new Vector3(0f, r.min.y - camMin.y, 0f);
            }
            if (camMax.x > r.max.x)
            {
                camPos += new Vector3(r.max.x - camMax.x, 0f, 0f);
            }
            if (camMax.y > r.max.y)
            {
                camPos += new Vector3(0f, r.max.y - camMax.y, 0f);
            }

            camPos.Set(camPos.x, camPos.y, -10f);

            cam.transform.position = camPos;

            DestroyImmediate(m_GridMesh);
            m_GridMesh = null;
        }

        private void Render()
        {
            if (m_GridMesh != null && GetGridHash() != m_LastGridHash)
            {
                ResetPreviewInstance();
                ResetPreviewMesh();
            }

            using (new PreviewInstanceScope(guiRect, previewUtility, paletteInstance, m_Owner.drawGizmos))
            {
                RenderGrid();
                previewUtility.Render();
                if (m_Owner.drawGizmos)
                    Handles.Internal_DoDrawGizmos(previewUtility.camera);
            }

            RenderDragAndDropPreview();
            CallOnSceneGUI();
            DoBrush();

            previewUtility.EndAndDrawPreview(guiRect);
            m_LastGridHash = GetGridHash();
        }

        private int GetGridHash()
        {
            if (prefabGrid == null)
                return 0;

            int hash = prefabGrid.GetHashCode();
            unchecked
            {
                hash = hash * 33 + prefabGrid.cellGap.GetHashCode();
                hash = hash * 33 + prefabGrid.cellLayout.GetHashCode();
                hash = hash * 33 + prefabGrid.cellSize.GetHashCode();
                hash = hash * 33 + prefabGrid.cellSwizzle.GetHashCode();
                hash = hash * 33 + SceneViewGridManager.sceneViewGridComponentGizmo.Color.GetHashCode();
            }
            return hash;
        }

        private void RenderDragAndDropPreview()
        {
            if (!activeDragAndDrop || m_HoverData == null || m_HoverData.Count == 0)
                return;

            RectInt rect = TileDragAndDrop.GetMinMaxRect(m_HoverData.Keys.ToList());
            rect.position += mouseGridPosition;
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            GridEditorUtility.DrawGridMarquee(grid, new BoundsInt(new Vector3Int(rect.xMin, rect.yMin, zPosition), new Vector3Int(rect.width, rect.height, 1)), Color.white);
        }

        private void RenderGrid()
        {
            // MeshTopology.Lines doesn't give nice pixel perfect grid so we have to have separate codepath with MeshTopology.Quads specially for palette window here
            if (m_GridMesh == null && grid.cellLayout == GridLayout.CellLayout.Rectangle)
                m_GridMesh = GridEditorUtility.GenerateCachedGridMesh(grid, k_GridColor, 1f / LocalToScreenRatio(), paddedBoundsInt, MeshTopology.Quads);

            GridEditorUtility.DrawGridGizmo(grid, grid.transform, k_GridColor, ref m_GridMesh, ref m_GridMaterial);
        }

        private void DoBrush()
        {
            if (activeDragAndDrop)
                return;

            RenderSelectedBrushMarquee();
            CallOnPaintSceneGUI(mouseGridPosition);
        }

        private class PreviewInstanceScope : IDisposable
        {
            private readonly PreviewRenderUtility m_PreviewRenderUtility;
            private readonly bool m_OldFog;
            private readonly bool m_DrawGizmos;
            private readonly GameObject m_PaletteInstance;
            private readonly Transform[] m_PaletteTransforms;
            private readonly Renderer[] m_Renderers;

            public PreviewInstanceScope(Rect guiRect, PreviewRenderUtility previewRenderUtility, GameObject paletteInstance, bool drawGizmos)
            {
                m_PreviewRenderUtility = previewRenderUtility;
                m_PaletteInstance = paletteInstance;
                m_DrawGizmos = drawGizmos;
                m_OldFog = RenderSettings.fog;

                m_PreviewRenderUtility.BeginPreview(guiRect, Styles.background);
                Unsupported.SetRenderSettingsUseFogNoDirty(false);
                if (m_DrawGizmos)
                {
                    m_PaletteTransforms = m_PaletteInstance.GetComponentsInChildren<Transform>();
                    foreach (var transform in m_PaletteTransforms)
                        transform.gameObject.hideFlags = HideFlags.None;
                    // Case 1199516: Set Dirty on palette instance to force a refresh on gizmo drawing
                    EditorUtility.SetDirty(m_PaletteInstance);
                    Unsupported.SceneTrackerFlushDirty();
                }
                m_Renderers = m_PaletteInstance.GetComponentsInChildren<Renderer>();
                foreach (var renderer in m_Renderers)
                {
                    renderer.gameObject.layer = Camera.PreviewCullingLayer;
                    renderer.allowOcclusionWhenDynamic = false;
                }
                m_PreviewRenderUtility.AddManagedGO(m_PaletteInstance);
                Handles.DrawCameraImpl(guiRect, m_PreviewRenderUtility.camera, DrawCameraMode.Textured, false, new DrawGridParameters(), true, false);
            }

            public void Dispose()
            {
                if (m_Renderers != null)
                {
                    foreach (var renderer in m_Renderers)
                        renderer.gameObject.layer = 0;
                }
                if (m_DrawGizmos && m_PaletteTransforms != null)
                {
                    foreach (var transform in m_PaletteTransforms)
                        transform.gameObject.hideFlags = HideFlags.HideAndDontSave;
                }
                Unsupported.SetRenderSettingsUseFogNoDirty(m_OldFog);
            }
        }

        public void HandleDragAndDrop()
        {
            if (DragAndDrop.objectReferences.Length == 0 || !guiRect.Contains(Event.current.mousePosition))
                return;

            switch (Event.current.type)
            {
                //TODO: Cache this
                case EventType.DragUpdated:
                {
                    List<Texture2D> sheets = TileDragAndDrop.GetValidSpritesheets(DragAndDrop.objectReferences);
                    List<Sprite> sprites = TileDragAndDrop.GetValidSingleSprites(DragAndDrop.objectReferences);
                    List<TileBase> tiles = TileDragAndDrop.GetValidTiles(DragAndDrop.objectReferences);
                    m_HoverData = TileDragAndDrop.CreateHoverData(sheets, sprites, tiles);

                    if (m_HoverData != null && m_HoverData.Count > 0)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Event.current.Use();
                        GUI.changed = true;
                    }
                }
                break;
                case EventType.DragPerform:
                {
                    if (m_HoverData == null || m_HoverData.Count == 0)
                        return;

                    RegisterUndo();

                    bool wasEmpty = TilemapIsEmpty(tilemap);

                    Vector2Int targetPosition = mouseGridPosition;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Dictionary<Vector2Int, TileBase> tileSheet = TileDragAndDrop.ConvertToTileSheet(m_HoverData);
                    foreach (KeyValuePair<Vector2Int, TileBase> item in tileSheet)
                        SetTile(tilemap, targetPosition + item.Key, item.Value, Color.white, Matrix4x4.identity);

                    OnPaletteChanged();

                    m_PaletteNeedsSave = true;
                    FlushHoverData();
                    GUI.changed = true;
                    SavePaletteIfNecessary();

                    if (wasEmpty)
                    {
                        ResetPreviewInstance();
                        FrameEntirePalette();
                    }

                    Event.current.Use();
                    GUIUtility.ExitGUI();
                }
                break;
                case EventType.Repaint:
                    // Handled in Render()
                    break;
            }

            if (m_HoverData != null && (
                Event.current.type == EventType.DragExited ||
                Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
                FlushHoverData();
                Event.current.Use();
            }
        }

        public void SetTile(Tilemap tilemapTarget, Vector2Int position, TileBase tile, Color color, Matrix4x4 matrix)
        {
            Vector3Int pos3 = new Vector3Int(position.x, position.y, zPosition);
            tilemapTarget.SetTile(pos3, tile);
            tilemapTarget.SetColor(pos3, color);
            tilemapTarget.SetTransformMatrix(pos3, matrix);
        }

        protected override void Paint(Vector3Int position)
        {
            if (gridBrush == null)
                return;

            gridBrush.Paint(grid, brushTarget, position);
            OnPaletteChanged();
        }

        protected override void Erase(Vector3Int position)
        {
            if (gridBrush == null)
                return;

            gridBrush.Erase(grid, brushTarget, position);
            OnPaletteChanged();
        }

        protected override void BoxFill(BoundsInt position)
        {
            if (gridBrush == null)
                return;

            gridBrush.BoxFill(grid, brushTarget, position);
            OnPaletteChanged();
        }

        protected override void BoxErase(BoundsInt position)
        {
            if (gridBrush == null)
                return;

            gridBrush.BoxErase(grid, brushTarget, position);
            OnPaletteChanged();
        }

        protected override void FloodFill(Vector3Int position)
        {
            if (gridBrush == null)
                return;

            gridBrush.FloodFill(grid, brushTarget, position);
            OnPaletteChanged();
        }

        protected override void PickBrush(BoundsInt position, Vector3Int pickingStart)
        {
            if (grid == null || gridBrush == null)
                return;

            gridBrush.Pick(grid, brushTarget, position, pickingStart);

            if (!InGridEditMode())
                TilemapEditorTool.SetActiveEditorTool(typeof(PaintTool));

            m_ActivePick = new RectInt(position.min.x, position.min.y, position.size.x, position.size.y);
        }

        protected override void Select(BoundsInt position)
        {
            if (grid)
            {
                GridSelection.Select(brushTarget, position);
                gridBrush.Select(grid, brushTarget, position);
            }
        }

        protected override void Move(BoundsInt from, BoundsInt to)
        {
            if (grid)
                gridBrush.Move(grid, brushTarget, from, to);
        }

        protected override void MoveStart(BoundsInt position)
        {
            if (grid)
                gridBrush.MoveStart(grid, brushTarget, position);
        }

        protected override void MoveEnd(BoundsInt position)
        {
            if (grid)
            {
                gridBrush.MoveEnd(grid, brushTarget, position);
                OnPaletteChanged();
            }
        }

        public override void Repaint()
        {
            m_Owner.Repaint();
        }

        protected override void ClearGridSelection()
        {
            GridSelection.Clear();
        }

        protected override void OnBrushPickStarted()
        {
        }

        protected override void OnBrushPickDragged(BoundsInt position)
        {
            m_ActivePick = new RectInt(position.min.x, position.min.y, position.size.x, position.size.y);
        }

        protected override void OnBrushPickCancelled()
        {
            m_ActivePick = null;
        }

        private void PingTileAsset(RectInt rect)
        {
            // Only able to ping asset if only one asset is selected
            if (rect.size == Vector2Int.zero && tilemap != null)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(rect.xMin, rect.yMin, zPosition));
                EditorGUIUtility.PingObject(tile);
                Selection.activeObject = tile;
            }
        }

        protected override bool ValidateFloodFillPosition(Vector3Int position)
        {
            return true;
        }

        protected override bool PickingIsDefaultTool()
        {
            return !m_Unlocked;
        }

        protected override bool CanPickOutsideEditMode()
        {
            return true;
        }

        protected override GridLayout.CellLayout CellLayout()
        {
            if (grid != null)
                return grid.cellLayout;
            return GridLayout.CellLayout.Rectangle;
        }

        protected override Vector2Int ScreenToGrid(Vector2 screenPosition)
        {
            Vector2 local = ScreenToLocal(screenPosition);
            Vector3Int result3 = grid.LocalToCell(local);
            Vector2Int result = new Vector2Int(result3.x, result3.y);
            return result;
        }

        private void RenderSelectedBrushMarquee()
        {
            if (!unlocked && m_ActivePick.HasValue)
            {
                DrawSelectionGizmo(m_ActivePick.Value);
            }
        }

        protected void DrawSelectionGizmo(RectInt rect)
        {
            if (Event.current.type != EventType.Repaint || !GUI.enabled)
                return;

            Color color = Color.white;
            if (isPicking)
                color = Color.cyan;

            GridEditorUtility.DrawGridMarquee(grid, new BoundsInt(new Vector3Int(rect.xMin, rect.yMin, 0), new Vector3Int(rect.width, rect.height, 1)), color);
        }

        private void HandleMouseEnterLeave()
        {
            if (guiRect.Contains(Event.current.mousePosition))
            {
                if (m_PreviousMousePosition.HasValue && !guiRect.Contains(m_PreviousMousePosition.Value) || !m_PreviousMousePosition.HasValue)
                {
                    if (GridPaintingState.activeBrushEditor != null)
                    {
                        GridPaintingState.activeBrushEditor.OnMouseEnter();
                    }
                }
            }
            else
            {
                if (m_PreviousMousePosition.HasValue && guiRect.Contains(m_PreviousMousePosition.Value) && !guiRect.Contains(Event.current.mousePosition))
                {
                    if (GridPaintingState.activeBrushEditor != null)
                    {
                        GridPaintingState.activeBrushEditor.OnMouseLeave();
                        Repaint();
                    }
                }
            }
        }

        private void CallOnSceneGUI()
        {
            var gridLayout = tilemap != null ? tilemap : grid as GridLayout;
            bool hasSelection = GridSelection.active  && GridSelection.target == brushTarget;
            if (hasSelection)
            {
                var rect = new RectInt(GridSelection.position.xMin, GridSelection.position.yMin, GridSelection.position.size.x, GridSelection.position.size.y);
                BoundsInt brushBounds = new BoundsInt(new Vector3Int(rect.x, rect.y, zPosition), new Vector3Int(rect.width, rect.height, 1));
                GridBrushEditorBase.OnSceneGUIInternal(gridLayout, brushTarget, brushBounds, EditTypeToBrushTool(UnityEditor.EditorTools.ToolManager.activeToolType), m_MarqueeStart.HasValue || executing);
            }
            if (GridPaintingState.activeBrushEditor != null)
            {
                GridPaintingState.activeBrushEditor.OnSceneGUI(gridLayout, brushTarget);
            }
        }

        private void CallOnPaintSceneGUI(Vector2Int position)
        {
            if (!unlocked && !TilemapEditorTool.IsActive(typeof(SelectTool)) && !TilemapEditorTool.IsActive(typeof(PickingTool)))
                return;

            bool hasSelection = GridSelection.active && GridSelection.target == brushTarget;
            if (!hasSelection && GridPaintingState.activeGrid != this)
                return;

            GridBrushBase brush = GridPaintingState.gridBrush;
            if (brush == null)
                return;

            var rect = new RectInt(position, new Vector2Int(1, 1));

            if (m_MarqueeStart.HasValue)
                rect = GridEditorUtility.GetMarqueeRect(position, m_MarqueeStart.Value);
            else if (hasSelection)
                rect = new RectInt(GridSelection.position.xMin, GridSelection.position.yMin, GridSelection.position.size.x, GridSelection.position.size.y);

            var gridLayout = tilemap != null ? tilemap.layoutGrid : grid as GridLayout;
            BoundsInt brushBounds = new BoundsInt(new Vector3Int(rect.x, rect.y, zPosition), new Vector3Int(rect.width, rect.height, 1));

            if (GridPaintingState.activeBrushEditor != null)
                GridPaintingState.activeBrushEditor.OnPaintSceneGUI(gridLayout, brushTarget, brushBounds,
                    EditTypeToBrushTool(UnityEditor.EditorTools.ToolManager.activeToolType),
                    m_MarqueeStart.HasValue || executing);
            else // Fallback when user hasn't defined custom editor
                GridBrushEditorBase.OnPaintSceneGUIInternal(gridLayout, Selection.activeGameObject, brushBounds,
                    EditTypeToBrushTool(UnityEditor.EditorTools.ToolManager.activeToolType),
                    m_MarqueeStart.HasValue || executing);
        }

        protected override void RegisterUndo()
        {
            if (!invalidClipboard)
            {
                Undo.RegisterFullObjectHierarchyUndo(paletteInstance, "Edit Palette");
            }
        }

        private void OnPaletteChanged()
        {
            m_PaletteUsed = true;
            m_PaletteNeedsSave = true;
            Undo.FlushUndoRecordObjects();
        }

        public void CheckRevertIfChanged(string[] paths)
        {
            if (paths != null && m_PaletteNeedsSave && palette != null)
            {
                var currentPalettePath = AssetDatabase.GetAssetPath(palette);
                foreach (var path in paths)
                {
                    if (currentPalettePath == path)
                    {
                        m_PaletteNeedsSave = false;
                        ResetPreviewInstance();
                        Debug.LogWarningFormat(palette, paletteSavedOutsideClipboard, palette.name);
                        break;
                    }
                }
            }
        }

        public void SavePaletteIfNecessary()
        {
            if (m_PaletteNeedsSave)
            {
                m_Owner.SavePalette();
                m_PaletteNeedsSave = false;
            }
        }

        public Vector2 GridToScreen(Vector2 gridPosition)
        {
            Vector3 gridPosition3 = new Vector3(gridPosition.x, gridPosition.y, 0);
            return LocalToScreen(grid.CellToLocalInterpolated(gridPosition3));
        }

        public Vector2 ScreenToLocal(Vector2 screenPosition)
        {
            Vector2 viewPosition = previewUtility.camera.transform.position;
            screenPosition -= new Vector2(guiRect.xMin, guiRect.yMin);
            Vector2 offsetFromCenter = new Vector2(screenPosition.x - guiRect.width * .5f, guiRect.height * .5f - screenPosition.y);
            return viewPosition + offsetFromCenter / LocalToScreenRatio();
        }

        protected Vector2 LocalToScreen(Vector2 localPosition)
        {
            Vector2 viewPosition = previewUtility.camera.transform.position;
            Vector2 offsetFromCenter = new Vector2(localPosition.x - viewPosition.x, viewPosition.y - localPosition.y);
            return offsetFromCenter * LocalToScreenRatio() + new Vector2(guiRect.width * .5f + guiRect.xMin, guiRect.height * .5f + guiRect.yMin);
        }

        private float LocalToScreenRatio()
        {
            return guiRect.height / (previewUtility.camera.orthographicSize * 2f);
        }

        private float LocalToScreenRatio(float viewHeight)
        {
            return viewHeight / (previewUtility.camera.orthographicSize * 2f);
        }

        private static bool TilemapIsEmpty(Tilemap tilemap)
        {
            return tilemap.GetUsedTilesCount() == 0;
        }

        public void UnlockAndEdit()
        {
            unlocked = true;
            m_PaletteNeedsSave = true;
        }
    }
}
