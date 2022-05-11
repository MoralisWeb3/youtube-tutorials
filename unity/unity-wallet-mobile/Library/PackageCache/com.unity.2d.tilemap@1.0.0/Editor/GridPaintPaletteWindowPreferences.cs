using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    internal class GridPaintPaletteWindowPreferences
    {
        [SettingsProvider]
        internal static SettingsProvider CreateSettingsProvider()
        {
            var settingsProvider = new SettingsProvider("Preferences/2D/Tile Palette", SettingsScope.User, SettingsProvider.GetSearchKeywordsFromGUIContentProperties<GridPaintPaletteWindowPreferences>())
            {
                guiHandler = searchContext =>
                {
                    GridPaintPaletteWindow.PreferencesGUI();
                    GridPaintActiveTargetsPreferences.PreferencesGUI();
                    SceneViewOpenTilePaletteHelper.PreferencesGUI();
                    TilemapPrefabStageHelper.PreferencesGUI();
                }
            };
            return settingsProvider;
        }
    }

    internal class GridPaintActiveTargetsPreferences
    {
        public static readonly string targetSortingModeEditorPref = "TilePalette.ActiveTargetsSortingMode";
        public static readonly string targetSortingModeLookup = "Active Targets Sorting Mode";
        public static readonly string targetRestoreEditModeSelectionEditorPref = "TilePalette.RestoreEditModeSelection";
        public static readonly string targetRestoreEditModeSelectionLookup = "Restore Edit Mode Active Target";
        public static readonly string createTileFromPaletteEditorPref = "TilePalette.CreateTileFromPalette";
        public static readonly string createTileFromPaletteLookup = "Create Tile Method";

        public static readonly string defaultSortingMode = L10n.Tr("None");

        public static readonly GUIContent targetSortingModeLabel =
            EditorGUIUtility.TrTextContent(targetSortingModeLookup,
                "Controls the sorting of the Active Targets in the Tile Palette");

        public static readonly GUIContent targetRestoreEditModeSelectionLabel = EditorGUIUtility.TrTextContent(
            targetRestoreEditModeSelectionLookup
            , "When exiting Play Mode, restores the Active Target in the Tile Palette to the last selected target from Edit Mode");

        public static readonly GUIContent createTileFromPaletteLabel = EditorGUIUtility.TrTextContent(
            createTileFromPaletteLookup
            , "Method used to create Tiles when drag and dropping assets to the Tile Palette");

        public static bool restoreEditModeSelection
        {
            get { return EditorPrefs.GetBool(targetRestoreEditModeSelectionEditorPref, true); }
            set { EditorPrefs.SetBool(targetRestoreEditModeSelectionEditorPref, value); }
        }

        private static string[] s_SortingNames;
        private static int s_SortingSelectionIndex;
        private static string[] s_CreateTileNames;
        private static int s_CreateTileIndex;

        private static bool CompareMethodName(string[] methodNames, MethodInfo method)
        {
            return methodNames.Length == 2 && methodNames[0] == method.ReflectedType.Name &&
                methodNames[1] == method.Name;
        }

        private static bool CompareTypeName(string typeFullName, Type type)
        {
            return typeFullName == type.FullName;
        }

        internal static void PreferencesGUI()
        {
            using (new SettingsWindow.GUIScope())
            {
                if (s_SortingNames == null)
                {
                    var sortingTypeFullName = EditorPrefs.GetString(targetSortingModeEditorPref, defaultSortingMode);
                    var sortingMethodNames = sortingTypeFullName.Split('.');
                    s_SortingNames = new string[1 + GridPaintSortingAttribute.sortingMethods.Count +
                                                GridPaintSortingAttribute.sortingTypes.Count];
                    int count = 0;
                    s_SortingNames[count++] = defaultSortingMode;
                    foreach (var sortingMethod in GridPaintSortingAttribute.sortingMethods)
                    {
                        if (CompareMethodName(sortingMethodNames, sortingMethod))
                            s_SortingSelectionIndex = count;
                        s_SortingNames[count++] = sortingMethod.Name;
                    }

                    foreach (var sortingType in GridPaintSortingAttribute.sortingTypes)
                    {
                        if (CompareTypeName(sortingTypeFullName, sortingType))
                            s_SortingSelectionIndex = count;
                        s_SortingNames[count++] = sortingType.Name;
                    }
                }

                if (s_CreateTileNames == null)
                {
                    var createTileFullName = EditorPrefs.GetString(createTileFromPaletteEditorPref, defaultSortingMode);
                    var createTileMethodNames = createTileFullName.Split('.');
                    int count = 0;
                    s_CreateTileNames = new string[CreateTileFromPaletteAttribute.createTileFromPaletteMethods.Count];
                    foreach (var createTileMethod in CreateTileFromPaletteAttribute.createTileFromPaletteMethods)
                    {
                        if (CompareMethodName(createTileMethodNames, createTileMethod))
                            s_CreateTileIndex = count;
                        s_CreateTileNames[count++] = createTileMethod.Name;
                    }
                }

                EditorGUI.BeginChangeCheck();
                var sortingSelection =
                    EditorGUILayout.Popup(targetSortingModeLabel, s_SortingSelectionIndex, s_SortingNames);
                if (EditorGUI.EndChangeCheck())
                {
                    s_SortingSelectionIndex = sortingSelection;
                    var sortingTypeFullName = defaultSortingMode;
                    if (s_SortingSelectionIndex > 0 &&
                        s_SortingSelectionIndex <= GridPaintSortingAttribute.sortingMethods.Count)
                    {
                        var sortingMethod = GridPaintSortingAttribute.sortingMethods[s_SortingSelectionIndex - 1];
                        sortingTypeFullName =
                            String.Format("{0}.{1}", sortingMethod.ReflectedType.Name, sortingMethod.Name);
                    }
                    else
                    {
                        var idx = s_SortingSelectionIndex - GridPaintSortingAttribute.sortingMethods.Count - 1;
                        if (idx >= 0 && idx < GridPaintSortingAttribute.sortingTypes.Count)
                        {
                            var sortingType = GridPaintSortingAttribute.sortingTypes[idx];
                            sortingTypeFullName = sortingType.FullName;
                        }
                    }

                    EditorPrefs.SetString(targetSortingModeEditorPref, sortingTypeFullName);
                    GridPaintingState.FlushCache();
                }

                EditorGUI.BeginChangeCheck();
                var editModeSelection = EditorGUILayout.Toggle(targetRestoreEditModeSelectionLabel, restoreEditModeSelection);
                if (EditorGUI.EndChangeCheck())
                {
                    restoreEditModeSelection = editModeSelection;
                }

                EditorGUI.BeginChangeCheck();
                var createTileSelection = EditorGUILayout.Popup(createTileFromPaletteLabel, s_CreateTileIndex, s_CreateTileNames);
                if (EditorGUI.EndChangeCheck())
                {
                    var createTileFullName = defaultSortingMode;
                    s_CreateTileIndex = createTileSelection;
                    if (s_CreateTileIndex < CreateTileFromPaletteAttribute.createTileFromPaletteMethods.Count)
                    {
                        var createTileMethod = CreateTileFromPaletteAttribute.createTileFromPaletteMethods[s_CreateTileIndex];
                        createTileFullName = String.Format("{0}.{1}", createTileMethod.ReflectedType.Name, createTileMethod.Name);
                    }

                    EditorPrefs.SetString(createTileFromPaletteEditorPref, createTileFullName);
                }
            }
        }

        public static IComparer<GameObject> GetTargetComparer()
        {
            var sortingTypeFullName = EditorPrefs.GetString(targetSortingModeEditorPref, defaultSortingMode);
            if (!sortingTypeFullName.Equals(defaultSortingMode))
            {
                var sortingMethodNames = sortingTypeFullName.Split('.');
                foreach (var sortingMethod in GridPaintSortingAttribute.sortingMethods)
                {
                    if (CompareMethodName(sortingMethodNames, sortingMethod))
                        return sortingMethod.Invoke(null, null) as IComparer<GameObject>;
                }

                foreach (var sortingType in GridPaintSortingAttribute.sortingTypes)
                {
                    if (CompareTypeName(sortingTypeFullName, sortingType))
                        return Activator.CreateInstance(sortingType) as IComparer<GameObject>;
                }
            }

            return null;
        }

        public static MethodInfo GetCreateTileFromPaletteUsingPreferences()
        {
            var createTileFullName = EditorPrefs.GetString(createTileFromPaletteEditorPref, defaultSortingMode);
            if (!createTileFullName.Equals(defaultSortingMode))
            {
                var methodNames = createTileFullName.Split('.');
                foreach (var createTileMethod in CreateTileFromPaletteAttribute.createTileFromPaletteMethods)
                {
                    if (CompareMethodName(methodNames, createTileMethod))
                        return createTileMethod;
                }
            }
            return typeof(TileUtility).GetMethod("DefaultTile", BindingFlags.Static | BindingFlags.Public);
        }
    }
}
