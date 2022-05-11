using System;
using System.Linq;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Unity.PlasticSCM.Editor.UI.Tree
{
    internal static class TreeHeaderSettings
    {
        internal static void Load(
            MultiColumnHeaderState headerState,
            string treeSettingsName,
            int defaultSortColumnIdx,
            bool defaultSortedAscending = true)
        {
            var visibleColumns = EditorPrefs.GetString(
                GetSettingKey(treeSettingsName, VISIBLE_COLUMNS_KEY), string.Empty)
                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(idx => int.Parse(idx))
                    .Where(idx => idx >= 0 && idx < headerState.columns.Length)
                    .ToArray();

            var columnWidths = EditorPrefs.GetString(
                GetSettingKey(treeSettingsName, COLUMNS_WIDTHS_KEY), string.Empty)
                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => float.Parse(w))
                    .ToArray();

            if (visibleColumns.Length > 0)
                headerState.visibleColumns = visibleColumns;

            if (headerState.columns.Length == columnWidths.Length)
                TreeHeaderColumns.SetWidths(headerState.columns, columnWidths);

            if (defaultSortColumnIdx == UnityConstants.UNSORT_COLUMN_ID)
                return;

            var sortColumnIdx = EditorPrefs.GetInt(
                GetSettingKey(treeSettingsName, SORT_COLUMN_INDEX_KEY),
                defaultSortColumnIdx);

            if (sortColumnIdx < 0 || sortColumnIdx >= headerState.columns.Length)
                sortColumnIdx = defaultSortColumnIdx;

            var sortColumnAscending = EditorPrefs.GetBool(
                GetSettingKey(treeSettingsName, SORT_ASCENDING_KEY),
                defaultSortedAscending);

            headerState.sortedColumnIndex = sortColumnIdx;
            headerState.columns[sortColumnIdx].sortedAscending = sortColumnAscending;
        }

        internal static void Save(
            MultiColumnHeaderState headerState,
            string treeSettingsName)
        {
            int[] visibleColumns = headerState.visibleColumns;
            float[] columnWidths = TreeHeaderColumns.GetWidths(headerState.columns);

            EditorPrefs.SetString(
                GetSettingKey(treeSettingsName, VISIBLE_COLUMNS_KEY),
                string.Join(",", visibleColumns.Select(idx => idx.ToString()).ToArray()));
            EditorPrefs.SetString(
                GetSettingKey(treeSettingsName, COLUMNS_WIDTHS_KEY),
                string.Join(",", columnWidths.Select(w => w.ToString()).ToArray()));

            int sortColumnIdx = headerState.sortedColumnIndex;

            if (sortColumnIdx == UnityConstants.UNSORT_COLUMN_ID)
                return;

            bool sortColumnAscending = headerState.
                columns[headerState.sortedColumnIndex].sortedAscending;

            EditorPrefs.SetInt(
                GetSettingKey(treeSettingsName, SORT_COLUMN_INDEX_KEY),
                sortColumnIdx);
            EditorPrefs.SetBool(
                GetSettingKey(treeSettingsName, SORT_ASCENDING_KEY),
                sortColumnAscending);
        }

        internal static void Clear(string treeSettingsName)
        {
            EditorPrefs.DeleteKey(
                GetSettingKey(treeSettingsName, VISIBLE_COLUMNS_KEY));
            EditorPrefs.DeleteKey(
                GetSettingKey(treeSettingsName, COLUMNS_WIDTHS_KEY));
            EditorPrefs.DeleteKey(
                GetSettingKey(treeSettingsName, SORT_COLUMN_INDEX_KEY));
            EditorPrefs.DeleteKey(
                GetSettingKey(treeSettingsName, SORT_ASCENDING_KEY));
        }

        static string GetSettingKey(string treeSettingsName, string key)
        {
            return string.Format(treeSettingsName, PlayerSettings.productGUID, key);
        }

        static string VISIBLE_COLUMNS_KEY = "VisibleColumns";
        static string COLUMNS_WIDTHS_KEY = "ColumnWidths";
        static string SORT_COLUMN_INDEX_KEY = "SortColumnIdx";
        static string SORT_ASCENDING_KEY = "SortAscending";
    }
}