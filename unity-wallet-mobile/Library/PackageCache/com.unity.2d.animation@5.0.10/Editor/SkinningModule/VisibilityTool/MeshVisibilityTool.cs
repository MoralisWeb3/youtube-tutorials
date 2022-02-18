using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;
using System.Linq;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class MeshVisibilityTool : IVisibilityTool
    {
        private MeshVisibilityToolView m_View;
        private MeshVisibilityToolModel m_Model;
        private SkinningCache m_SkinningCache;
        public SkinningCache skinningCache { get { return m_SkinningCache; } }

        public MeshVisibilityTool(SkinningCache s)
        {
            m_SkinningCache = s;
        }

        public void Setup()
        {
            m_Model = skinningCache.CreateCache<MeshVisibilityToolModel>();
            m_View = new MeshVisibilityToolView(skinningCache)
            {
                GetModel = () => m_Model,
                SetAllVisibility = SetAllVisibility,
                GetAllVisibility = GetAllVisibility
            };
        }

        public void Dispose()
        {

        }

        public VisualElement view { get { return m_View; } }
        public string name { get { return "Mesh"; } }

        public void Activate()
        {
            skinningCache.events.selectedSpriteChanged.AddListener(OnSelectionChange);
            skinningCache.events.skinningModeChanged.AddListener(OnViewModeChanged);
            OnViewModeChanged(skinningCache.mode);
            if (m_Model.previousVisiblity != m_Model.allVisibility)
            {
                SetAllMeshVisibility();
                m_Model.previousVisiblity = m_Model.allVisibility;
            }
        }

        public void Deactivate()
        {
            skinningCache.events.selectedSpriteChanged.RemoveListener(OnSelectionChange);
            skinningCache.events.skinningModeChanged.RemoveListener(OnViewModeChanged);
        }

        public bool isAvailable
        {
            get { return false; }
        }

        public void SetAvailabilityChangeCallback(Action callback)
        {}

        private void OnViewModeChanged(SkinningMode characterMode)
        {
            if (characterMode == SkinningMode.Character)
            {
                m_View.Setup(skinningCache.GetSprites());
            }
            else
            {
                m_View.Setup(new[] { skinningCache.selectedSprite });
            }
        }

        private void OnSelectionChange(SpriteCache sprite)
        {
            OnViewModeChanged(skinningCache.mode);
            SetAllMeshVisibility();
        }

        void SetAllVisibility(bool visibility)
        {
            using (skinningCache.UndoScope(TextContent.meshVisibility))
            {
                m_Model.allVisibility = visibility;
                SetAllMeshVisibility();
            }
        }

        void SetAllMeshVisibility()
        {
            SpriteCache[] sprites;
            if (skinningCache.mode == SkinningMode.Character)
                sprites = skinningCache.GetSprites();
            else
                sprites = new[] { skinningCache.selectedSprite };

            foreach (var spr in sprites)
            {
                if (spr != null)
                    m_Model.SetMeshVisibility(spr, m_Model.allVisibility);
            }
        }

        bool GetAllVisibility()
        {
            return m_Model.allVisibility;
        }
    }

    internal class MeshVisibilityToolModel : SkinningObject
    {
        [SerializeField]
        bool m_AllVisibility = true;
        bool m_PreviousVisibility = true;

        public bool allVisibility
        {
            get {return m_AllVisibility; }
            set { m_AllVisibility = value; }
        }

        public void SetMeshVisibility(SpriteCache sprite, bool visibility)
        {

        }

        public bool GetMeshVisibility(SpriteCache sprite)
        {
            return false;
        }

        public bool ShouldDisable(SpriteCache sprite)
        {
            var mesh = sprite.GetMesh();
            return mesh == null || mesh.vertices.Count == 0;
        }

        public bool previousVisiblity
        {
            get { return m_PreviousVisibility; }
            set { m_PreviousVisibility = value; }
        }
    }

    internal class MeshVisibilityToolView : VisibilityToolViewBase
    {
        public Func<MeshVisibilityToolModel> GetModel = () => null;
        public Action<bool> SetAllVisibility = (b) => {};
        public Func<bool> GetAllVisibility = () => true;
        public SkinningCache skinningCache { get; set; }

        public MeshVisibilityToolView(SkinningCache s)
        {
            skinningCache = s;
            var columns = new MultiColumnHeaderState.Column[2];
            columns[0] = new MultiColumnHeaderState.Column
            {
                headerContent = EditorGUIUtility.TrTextContent(TextContent.name),
                headerTextAlignment = TextAlignment.Center,
                width = 200,
                minWidth = 130,
                autoResize = true,
                allowToggleVisibility = false
            };
            columns[1] = new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent(EditorGUIUtility.FindTexture("visibilityOn")),
                headerTextAlignment = TextAlignment.Center,
                width = 32,
                minWidth = 32,
                maxWidth = 32,
                autoResize = false,
                allowToggleVisibility = true
            };
            var multiColumnHeaderState = new MultiColumnHeaderState(columns);
            var multiColumnHeader = new VisibilityToolColumnHeader(multiColumnHeaderState)
            {
                GetAllVisibility = InternalGetAllVisibility,
                SetAllVisibility = InternalSetAllVisibility,
                canSort = false,
                height = 20,
                visibilityColumn = 1
            };
            m_TreeView = new MeshTreeView(m_TreeViewState, multiColumnHeader)
            {
                GetModel = InternalGetModel
            };
            SetupSearchField();
        }

        MeshVisibilityToolModel InternalGetModel()
        {
            return GetModel();
        }

        public void Setup(SpriteCache[] sprites)
        {
            ((MeshTreeView)m_TreeView).Setup(sprites);
            ((MeshTreeView)m_TreeView).SetSelection(skinningCache.selectedSprite);
        }

        bool InternalGetAllVisibility()
        {
            return GetAllVisibility();
        }

        void InternalSetAllVisibility(bool visibility)
        {
            SetAllVisibility(visibility);
        }
    }

    class MeshTreeView : VisibilityTreeViewBase
    {
        private List<SpriteCache> m_Sprites = new List<SpriteCache>();

        public MeshTreeView(TreeViewState treeViewState, MultiColumnHeader header)
            : base(treeViewState, header)
        {
            this.showAlternatingRowBackgrounds = true;
            this.useScrollView = true;
            Reload();
        }

        public Func<MeshVisibilityToolModel> GetModel = () => null;

        public void Setup(SpriteCache[] sprites)
        {
            m_Sprites.Clear();
            m_Sprites.AddRange(sprites);
            Reload();
        }

        private static TreeViewItem CreateTreeViewItem(SpriteCache part)
        {
            return new TreeViewItemBase<SpriteCache>(part.GetInstanceID(), -1, part.name, part);
        }

        private void AddTreeViewItem(IList<TreeViewItem> rows, SpriteCache part)
        {
            if (string.IsNullOrEmpty(searchString) || part.name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var item = CreateTreeViewItem(part);
                rows.Add(item);
                rootItem.AddChild(item);
            }
        }

        private void CellGUI(Rect cellRect, TreeViewItem item, int column, ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (column)
            {
                case 0:
                    DrawNameCell(cellRect, item, ref args);
                    break;
                case 1:
                    DrawVisibilityCell(cellRect, item);
                    break;
            }
        }

        private void DrawVisibilityCell(Rect cellRect, TreeViewItem item)
        {
            GUIStyle style = MultiColumnHeader.DefaultStyles.columnHeaderCenterAligned;
            var itemView = item as TreeViewItemBase<SpriteCache>;
            var shouldDisable = GetModel().ShouldDisable(itemView.customData);
            using (new EditorGUI.DisabledScope(shouldDisable))
            {
                EditorGUI.BeginChangeCheck();
                bool visible = GetModel().GetMeshVisibility(itemView.customData);
                visible = GUI.Toggle(cellRect, visible, visible ? VisibilityIconStyle.visibilityOnIcon : VisibilityIconStyle.visibilityOffIcon, style);
                if (EditorGUI.EndChangeCheck())
                    GetModel().SetMeshVisibility(itemView.customData, visible);
            }
        }

        private void DrawNameCell(Rect cellRect, TreeViewItem item, ref RowGUIArgs args)
        {
            args.rowRect = cellRect;
            base.RowGUI(args);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            SpriteCache newSelected = null;
            if (selectedIds.Count > 0)
            {
                var selected = GetRows().FirstOrDefault(x => ((TreeViewItemBase<SpriteCache>)x).customData.GetInstanceID() == selectedIds[0]) as TreeViewItemBase<SpriteCache>;
                if (selected != null)
                    newSelected = selected.customData;
            }

            var skinningCache = newSelected.skinningCache;

            using (skinningCache.UndoScope(TextContent.selectionChange))
            {
                skinningCache.events.selectedSpriteChanged.Invoke(newSelected);
            }
        }

        public void SetSelection(SpriteCache sprite)
        {
            var rows = GetRows();
            for (int i = 0; rows != null && i < rows.Count; ++i)
            {
                var r = (TreeViewItemBase<SpriteCache>)rows[i];
                if (r.customData == sprite)
                {
                    SetSelection(new[] { r.customData.GetInstanceID() }, TreeViewSelectionOptions.RevealAndFrame);
                    break;
                }
            }
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows() ?? new List<TreeViewItem>(200);
            rows.Clear();

            m_Sprites.RemoveAll(s => s == null);

            foreach (var sprite in m_Sprites)
                AddTreeViewItem(rows, sprite);

            SetupDepthsFromParentsAndChildren(root);
            return rows;
        }
    }
}
