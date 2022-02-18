using System;
using UnityEngine;
using System.Collections.Generic;

namespace UnityEditor.Experimental.U2D.Animation
{
    internal class SpriteSelectorWidget
    {
        class Styles
        {
            public GUIStyle gridListStyle;

            public Styles()
            {
                gridListStyle = new GUIStyle("GridList");
                gridListStyle.alignment = GUI.skin.button.alignment;
            }
        }

        Sprite[] m_SpriteList = null;
        Texture2D[] m_SpritePreviews = null;
        List<int> m_SpritePreviewNeedFetching = new List<int>();
        Vector2 m_ScrollPos;
        Styles m_Style;
        const int kTargetPreviewSize = 64;
        public SpriteSelectorWidget()
        {}

        public void UpdateContents(Sprite[] sprites)
        {
            m_SpriteList = sprites;
            m_SpritePreviews = new Texture2D[sprites.Length];
            for (int i = 0; i < m_SpritePreviews.Length; ++i)
                m_SpritePreviewNeedFetching.Add(i);
            UpdateSpritePreviews();
        }

        public int ShowGUI(int selectedIndex)
        {
            if (m_Style == null)
                m_Style = new Styles();

            UpdateSpritePreviews();

            if (m_SpriteList == null || m_SpriteList.Length == 0)
                return selectedIndex;

            selectedIndex = (selectedIndex > m_SpriteList.Length) ? 0 : selectedIndex;

            using (var topRect = new EditorGUILayout.HorizontalScope())
            {
                //GUILayout.Label(Styles.spriteList, EditorStyles.label, new [] {GUILayout.Width(EditorGUIUtility.labelWidth - 5)});
                using (var selectionGridRect = new EditorGUILayout.HorizontalScope("box", new[] {GUILayout.ExpandWidth(true)}))
                {
                    {
                        float columnF;
                        int columnCount, rowCount;
                        GetRowColumnCount(EditorGUIUtility.currentViewWidth, kTargetPreviewSize, m_SpriteList.Length, out columnCount, out rowCount, out columnF);
                        if (columnCount > 0 && rowCount > 0)
                        {
                            float contentSize = (columnF * kTargetPreviewSize) / columnCount;

                            if (rowCount >= 2)
                                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, new[] { GUILayout.Height(rowCount > 1 ? contentSize * 2 : contentSize) });

                            m_Style.gridListStyle.fixedWidth = contentSize;
                            m_Style.gridListStyle.fixedHeight = contentSize;
                            selectedIndex = ContentSelectionGrid(selectedIndex, m_SpriteList, m_Style.gridListStyle, columnCount - 1);
                            if (rowCount >= 2)
                                EditorGUILayout.EndScrollView();
                        }
                    }
                }
            }
            return selectedIndex;
        }

        static void GetRowColumnCount(float drawWidth, int size, int contentCount, out int column, out int row, out float columnf)
        {
            columnf = (drawWidth) / size;
            column = (int)columnf;
            if (column == 0)
                row = 0;
            else
                row = (int)Mathf.Ceil((contentCount + column - 1) / column);
        }

        int ContentSelectionGrid(int selected, Sprite[] contents, GUIStyle style,  int columnCount)
        {
            if (contents != null && contents.Length != 0)
            {
                selected = GUILayout.SelectionGrid(selected, m_SpritePreviews, columnCount, style);
            }

            return selected;
        }

        public bool NeedUpdatePreview()
        {
            return m_SpritePreviewNeedFetching.Count > 0;
        }
        
        void UpdateSpritePreviews()
        {
            for (int i = 0; i < m_SpritePreviewNeedFetching.Count; ++i)
            {
                var index = m_SpritePreviewNeedFetching[i];
                if(m_SpriteList[index] == null)
                    m_SpritePreviews[index] = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;
                else
                    m_SpritePreviews[index] = AssetPreview.GetAssetPreview(m_SpriteList[index]);
                if (m_SpritePreviews[index] != null)
                {
                    m_SpritePreviewNeedFetching.RemoveAt(i);
                    --i;
                }
            }
        }
    }
}
