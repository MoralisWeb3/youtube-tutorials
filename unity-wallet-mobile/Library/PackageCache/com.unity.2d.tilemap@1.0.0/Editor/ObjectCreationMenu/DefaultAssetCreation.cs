using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    internal static class DefaultAssetCreation
    {
        const int k_TilePaletteAssetMenuPriority = 4;

        static internal Action<int, ProjectWindowCallback.EndNameEditAction, string, Texture2D, string> StartNewAssetNameEditingDelegate = ProjectWindowUtil.StartNameEditingIfProjectWindowExists;

        [MenuItem("Assets/Create/2D/Tile Palette/Rectangular", priority = k_TilePaletteAssetMenuPriority)]
        static void MenuItem_AssetsCreate2DTilePaletteRectangular(MenuCommand menuCommand)
        {
            CreateAssetObject("Rectangular Palette", GridLayout.CellLayout.Rectangle, GridLayout.CellSwizzle.XYZ, GridPalette.CellSizing.Automatic, new Vector3(1, 1, 0));
        }

        [MenuItem("Assets/Create/2D/Tile Palette/Hexagonal Flat-Top", priority = k_TilePaletteAssetMenuPriority)]
        static void MenuItem_AssetsCreate2DTilePaletteHexagonalFlatTop(MenuCommand menuCommand)
        {
            CreateAssetObject("Hexagon Flat-Top Palette", GridLayout.CellLayout.Hexagon, GridLayout.CellSwizzle.YXZ, GridPalette.CellSizing.Manual, new Vector3(0.8659766f, 1, 0));
        }

        [MenuItem("Assets/Create/2D/Tile Palette/Hexagonal Pointed-Top", priority = k_TilePaletteAssetMenuPriority)]
        static void MenuItem_AssetsCreate2DTilePaletteHexagonalPointedTop(MenuCommand menuCommand)
        {
            CreateAssetObject("Hexagon Pointed-Top Palette", GridLayout.CellLayout.Hexagon, GridLayout.CellSwizzle.XYZ, GridPalette.CellSizing.Manual, new Vector3(0.8659766f, 1, 0));
        }

        [MenuItem("Assets/Create/2D/Tile Palette/Isometric", priority = k_TilePaletteAssetMenuPriority)]
        static void MenuItem_AssetsCreate2DTilePaletteIsometric(MenuCommand menuCommand)
        {
            CreateAssetObject("Isometric Palette", GridLayout.CellLayout.Isometric, GridLayout.CellSwizzle.XYZ, GridPalette.CellSizing.Manual, new Vector3(1, 0.5f, 0));
        }

        static void CreateAssetObject(string defaultAssetName, GridLayout.CellLayout layout, GridLayout.CellSwizzle swizzle, GridPalette.CellSizing cellSizing, Vector3 cellSize)
        {
            var assetSelectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var isFolder = false;
            if (!string.IsNullOrEmpty(assetSelectionPath))
                isFolder = File.GetAttributes(assetSelectionPath).HasFlag(FileAttributes.Directory);
            var path = ProjectWindowUtil.GetActiveFolderPath();
            if (isFolder)
            {
                path = assetSelectionPath;
            }

            var destName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, defaultAssetName));
            var icon = EditorGUIUtility.IconContent<GameObject>().image as Texture2D;
            CreateAssetEndNameEditAction action = ScriptableObject.CreateInstance<CreateAssetEndNameEditAction>();
            action.swizzle = swizzle;
            action.layout = layout;
            action.cellSize = cellSize;
            action.cellSizing = cellSizing;
            StartNewAssetNameEditingDelegate(0, action, destName, icon, "");
        }

        internal class CreateAssetEndNameEditAction : ProjectWindowCallback.EndNameEditAction
        {
            public GridLayout.CellLayout layout { get; set; }
            public GridLayout.CellSwizzle swizzle { get; set; }
            public Vector3 cellSize { get; set; }
            public GridPalette.CellSizing cellSizing { get; set; }

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                GridPaletteUtility.CreateNewPalette(Path.GetDirectoryName(pathName), Path.GetFileName(pathName), layout,
                    cellSizing, cellSize, swizzle);
            }
        }
    }
}
