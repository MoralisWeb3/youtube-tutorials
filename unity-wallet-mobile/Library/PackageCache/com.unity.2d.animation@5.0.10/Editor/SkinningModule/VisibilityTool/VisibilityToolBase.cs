using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal interface IVisibilityTool
    {
        VisualElement view { get; }
        string name { get; }
        void Activate();
        void Deactivate();
        bool isAvailable { get; }
        void SetAvailabilityChangeCallback(Action callback);
        void Setup();
        void Dispose();
    }

    internal class VisibilityToolViewBase : VisualElement
    {
        IMGUIContainer m_Container;
        SearchField m_SearchField;
        protected TreeView m_TreeView;
        protected TreeViewState m_TreeViewState = new TreeViewState();

        public Action<float> SetOpacityValue = null;
        public Func<float> GetOpacityValue = null;

        protected static class Styles
        {
            public static readonly GUIStyle preLabel = "preLabel";
            public static readonly GUIStyle preSlider = "preSlider";
            public static readonly GUIStyle preSliderThumb = "preSliderThumb";
        }

        public VisibilityToolViewBase()
        {
            m_Container = new IMGUIContainer(OnGUI);
            this.Add(m_Container);
            m_TreeViewState.searchString = "";
        }

        protected void SetupSearchField()
        {
            m_SearchField = new SearchField();
            m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
        }

        void DoSearchField()
        {
            m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            m_TreeView.searchString = m_SearchField.OnToolbarGUI(m_TreeView.searchString);
            GUILayout.EndHorizontal();
        }

        void DoOpacitySlider()
        {
            if (GetOpacityValue != null && SetOpacityValue != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                EditorGUI.BeginChangeCheck();
                var opacity = GUILayout.HorizontalSlider(GetOpacityValue(), 0, 1, Styles.preSlider, Styles.preSliderThumb);
                if (EditorGUI.EndChangeCheck())
                    SetOpacityValue(opacity);
                GUILayout.EndHorizontal();
            }
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();
            DoSearchField();
            GUILayout.EndVertical();
            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
            m_TreeView.OnGUI(rect);
            DoOpacitySlider();
        }
    }

    internal class TreeViewItemBase<T> : TreeViewItem
    {
        public T customData;

        public TreeViewItemBase(int id, int depth, string name, T data) : base(id, depth, name)
        {
            customData = data;
        }
    }

    internal class VisibilityTreeViewBase : TreeView
    {
        static internal class VisibilityIconStyle
        {
            public static readonly GUIContent visibilityOnIcon  = new GUIContent(IconUtility.LoadIconResource("Visibility_Tool", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr("On"));
            public static readonly GUIContent visibilityOffIcon = new GUIContent(IconUtility.LoadIconResource("Visibility_Hidded", IconUtility.k_LightIconResourcePath, IconUtility.k_DarkIconResourcePath), L10n.Tr("Off"));
        }


        public VisibilityTreeViewBase(TreeViewState treeViewState, MultiColumnHeader multiColumn)
            : base(treeViewState, multiColumn)
        {
            Init();
        }

        public VisibilityTreeViewBase(TreeViewState treeViewState)
            : base(treeViewState)
        {
            Init();
        }

        void Init()
        {
            this.showAlternatingRowBackgrounds = true;
            this.useScrollView = true;
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem { id = 0, depth = -1 };
        }
    }
}
