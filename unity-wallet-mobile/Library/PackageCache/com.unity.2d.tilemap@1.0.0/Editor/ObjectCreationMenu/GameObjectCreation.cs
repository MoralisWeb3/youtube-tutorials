using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    static class GameObjectCreation
    {
        private static class Styles
        {
            public static readonly string pointTopHexagonCreateUndo = L10n.Tr("Hexagonal Point Top Tilemap");
            public static readonly string flatTopHexagonCreateUndo = L10n.Tr("Hexagonal Flat Top Tilemap");
            public static readonly string isometricCreateUndo = L10n.Tr("Isometric Tilemap");
            public static readonly string isometricZAsYCreateUndo = L10n.Tr("Isometric Z As Y Tilemap");
        }

        const int k_MenuPriority = 3;

        [MenuItem("GameObject/2D Object/Tilemap/Rectangular", priority = k_MenuPriority)]
        internal static void CreateRectangularTilemap()
        {
            var root = FindOrCreateRootGrid();
            var uniqueName = GameObjectUtility.GetUniqueNameForSibling(root.transform, "Tilemap");
            var tilemapGO = ObjectFactory.CreateGameObject(uniqueName, typeof(Tilemap), typeof(TilemapRenderer));
            Undo.SetTransformParent(tilemapGO.transform, root.transform, "");
            tilemapGO.transform.position = Vector3.zero;

            Selection.activeGameObject = tilemapGO;
            Undo.SetCurrentGroupName("Create Tilemap");
        }

        [MenuItem("GameObject/2D Object/Tilemap/Hexagonal - Pointed-Top", priority = k_MenuPriority)]
        internal static void CreateHexagonalPointTopTilemap()
        {
            CreateHexagonalTilemap(GridLayout.CellSwizzle.XYZ, Styles.pointTopHexagonCreateUndo, new Vector3(0.8659766f, 1, 1));
        }

        [MenuItem("GameObject/2D Object/Tilemap/Hexagonal - Flat-Top", priority = k_MenuPriority)]
        internal static void CreateHexagonalFlatTopTilemap()
        {
            CreateHexagonalTilemap(GridLayout.CellSwizzle.YXZ, Styles.flatTopHexagonCreateUndo, new Vector3(0.8659766f, 1, 1));
        }

        [MenuItem("GameObject/2D Object/Tilemap/Isometric", priority = k_MenuPriority)]
        internal static void CreateIsometricTilemap()
        {
            CreateIsometricTilemap(GridLayout.CellLayout.Isometric, Styles.isometricCreateUndo);
        }

        [MenuItem("GameObject/2D Object/Tilemap/Isometric Z as Y", priority = k_MenuPriority)]
        internal static void CreateIsometricZAsYTilemap()
        {
            CreateIsometricTilemap(GridLayout.CellLayout.IsometricZAsY, Styles.isometricZAsYCreateUndo);
        }

        private static void CreateIsometricTilemap(GridLayout.CellLayout isometricLayout, string undoMessage)
        {
            var root = FindOrCreateRootGrid();
            var uniqueName = GameObjectUtility.GetUniqueNameForSibling(root.transform, "Tilemap");
            var tilemapGO = ObjectFactory.CreateGameObject(uniqueName, typeof(Tilemap), typeof(TilemapRenderer));
            tilemapGO.transform.SetParent(root.transform);
            tilemapGO.transform.position = Vector3.zero;

            var grid = root.GetComponent<Grid>();
            // Case 1071703: Do not reset cell size if adding a new Tilemap to an existing Grid of the same layout
            if (isometricLayout != grid.cellLayout)
            {
                grid.cellLayout = isometricLayout;
                grid.cellSize = new Vector3(1.0f, 0.5f, 1.0f);
            }

            var tilemapRenderer = tilemapGO.GetComponent<TilemapRenderer>();
            tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopRight;

            Selection.activeGameObject = tilemapGO;
            Undo.RegisterCreatedObjectUndo(tilemapGO, undoMessage);
        }

        private static void CreateHexagonalTilemap(GridLayout.CellSwizzle swizzle, string undoMessage, Vector3 cellSize)
        {
            var root = FindOrCreateRootGrid();
            var uniqueName = GameObjectUtility.GetUniqueNameForSibling(root.transform, "Tilemap");
            var tilemapGO = ObjectFactory.CreateGameObject(uniqueName, typeof(Tilemap), typeof(TilemapRenderer));
            tilemapGO.transform.SetParent(root.transform);
            tilemapGO.transform.position = Vector3.zero;
            var grid = root.GetComponent<Grid>();
            grid.cellLayout = Grid.CellLayout.Hexagon;
            grid.cellSwizzle = swizzle;
            grid.cellSize = cellSize;
            var tilemap = tilemapGO.GetComponent<Tilemap>();
            tilemap.tileAnchor = Vector3.zero;
            Selection.activeGameObject = tilemapGO;
            Undo.RegisterCreatedObjectUndo(tilemapGO, undoMessage);
        }

        private static GameObject FindOrCreateRootGrid()
        {
            GameObject gridGO = null;

            // Check if active object has a Grid and can be a parent for the Tile Map
            if (Selection.activeObject is GameObject)
            {
                var go = (GameObject)Selection.activeObject;
                var parentGrid = go.GetComponentInParent<Grid>();
                if (parentGrid != null)
                {
                    gridGO = parentGrid.gameObject;
                }
            }

            // If neither the active object nor its parent has a grid, create a grid for the tilemap
            if (gridGO == null)
            {
                gridGO = ObjectFactory.CreateGameObject("Grid", typeof(Grid));
                gridGO.transform.position = Vector3.zero;

                var grid = gridGO.GetComponent<Grid>();
                grid.cellSize = new Vector3(1.0f, 1.0f, 0.0f);
                Undo.SetCurrentGroupName("Create Grid");
            }

            return gridGO;
        }
    }
}
