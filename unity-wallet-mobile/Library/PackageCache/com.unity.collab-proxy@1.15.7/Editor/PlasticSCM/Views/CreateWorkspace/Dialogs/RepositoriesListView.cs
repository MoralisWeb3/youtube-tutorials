using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.CM.Common;
using PlasticGui;
using PlasticGui.SwitcherWindow.Repositories;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace.Dialogs
{
    internal class RepositoriesListView :
        TreeView,
        IPlasticTable<RepositoryInfo>
    {
        internal RepositoriesListView(
            RepositoriesListHeaderState headerState,
            List<string> columnNames,
            Action doubleClickAction)
            : base(new TreeViewState())
        {
            mColumnNames = columnNames;
            mDoubleClickAction = doubleClickAction;

            multiColumnHeader = new MultiColumnHeader(headerState);
            multiColumnHeader.canSort = true;
            multiColumnHeader.sortingChanged += SortingChanged;

            mColumnComparers = RepositoriesTableDefinition.BuildColumnComparers();

            rowHeight = UnityConstants.TREEVIEW_ROW_HEIGHT;
            showAlternatingRowBackgrounds = true;
        }

        public override IList<TreeViewItem> GetRows()
        {
            return mRows;
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1, string.Empty);
        }

        protected override IList<TreeViewItem> BuildRows(
            TreeViewItem rootItem)
        {
            RegenerateRows(
                this, mRepositories, rootItem, mRows);

            return mRows;
        }

        protected override void SearchChanged(string newSearch)
        {
            Refilter();

            Sort();

            Reload();

            TableViewOperations.ScrollToSelection(this);
        }

        protected override void DoubleClickedItem(int id)
        {
            mDoubleClickAction();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            if (args.item is RepositoryListViewItem)
            {
                RepositoryListViewItemGUI(
                    (RepositoryListViewItem)args.item,
                    args,
                    rowHeight);
                return;
            }

            base.RowGUI(args);
        }

        internal string GetSelectedRepository()
        {
            IList<TreeViewItem> selectedItems = FindRows(GetSelection());

            if (selectedItems.Count == 0)
                return null;

            return ((RepositoryListViewItem)selectedItems[0])
                .Repository.GetRepSpec().ToString();
        }

        void IPlasticTable<RepositoryInfo>.Fill(
            List<RepositoryInfo> entries,
            List<RepositoryInfo> entriesToSelect,
            string currentFilter)
        {
            mUnfilteredRepositories = entries;

            Refilter();

            Sort();

            Reload();
        }

        void Refilter()
        {
            mRepositories = RepositoriesTableDefinition.TableFilter.Filter(
                searchString,
                mUnfilteredRepositories);
        }

        void Sort()
        {
            int sortedColumnIdx = multiColumnHeader.state.sortedColumnIndex;
            bool sortAscending = multiColumnHeader.IsSortedAscending(sortedColumnIdx);

            IComparer<RepositoryInfo> comparer = mColumnComparers[
                mColumnNames[sortedColumnIdx]];

            mRepositories.Sort(new SortOrderComparer<RepositoryInfo>(
                comparer, sortAscending));
        }

        void SortingChanged(MultiColumnHeader multiColumnHeader)
        {
            Sort();

            Reload();
        }

        static void RegenerateRows(
            RepositoriesListView listView,
            IList repositories,
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            ClearRows(rootItem, rows);

            if (repositories.Count == 0)
                return;

            for (int i = 0; i < repositories.Count; i++)
            {
                RepositoryListViewItem repositoryListViewItem =
                    new RepositoryListViewItem(i + 1, (RepositoryInfo)repositories[i]);

                rootItem.AddChild(repositoryListViewItem);
                rows.Add(repositoryListViewItem);
            }

            listView.SetSelection(new List<int> { 1 });
        }

        static void ClearRows(
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            if (rootItem.hasChildren)
                rootItem.children.Clear();

            rows.Clear();
        }

        static void RepositoryListViewItemGUI(
            RepositoryListViewItem item,
            RowGUIArgs args,
            float rowHeight)
        {
            for (int visibleColumnIdx = 0; visibleColumnIdx < args.GetNumVisibleColumns(); visibleColumnIdx++)
            {
                Rect cellRect = args.GetCellRect(visibleColumnIdx);

                RepositoriesListColumn column =
                    (RepositoriesListColumn)args.GetColumn(visibleColumnIdx);

                RepositoryListViewItemCellGUI(
                    cellRect,
                    item,
                    column,
                    rowHeight,
                    args.selected,
                    args.focused);
            }
        }

        static void RepositoryListViewItemCellGUI(
            Rect rect,
            RepositoryListViewItem item,
            RepositoriesListColumn column,
            float rowHeight,
            bool isSelected,
            bool isFocused)
        {
            if (column == RepositoriesListColumn.Name)
            {
                DrawTreeViewItem.ForItemCell(
                    rect,
                    rowHeight,
                    0,
                    Images.GetImage(Images.Name.IconRepository),
                    null,
                    item.Repository.Name,
                    isSelected,
                    isFocused,
                    false,
                    false);

                return;
            }

            DrawTreeViewItem.ForSecondaryLabel(
                rect,
                item.Repository.Server,
                isSelected,
                isFocused,
                false);
        }

        List<TreeViewItem> mRows = new List<TreeViewItem>();

        List<RepositoryInfo> mUnfilteredRepositories = new List<RepositoryInfo>();
        List<RepositoryInfo> mRepositories = new List<RepositoryInfo>();

        readonly Dictionary<string, IComparer<RepositoryInfo>> mColumnComparers;
        readonly List<string> mColumnNames;
        readonly Action mDoubleClickAction;
    }
}