using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.U2D.Animation;

namespace UnityEditor.Experimental.U2D.Animation
{
    [CustomEditor(typeof(SpriteResolver))]
    internal class SpriteResolverInspector : Editor
    {
        static class Style
        {
            public static GUIContent noSpriteLibContainer = EditorGUIUtility.TrTextContent("No Sprite Library Container Component found or Sprite Library has no categories.");
            public static GUIContent categoryLabel = EditorGUIUtility.TrTextContent("Category");
            public static GUIContent labelLabel = EditorGUIUtility.TrTextContent("Label");
            public static GUIContent categoryIsEmptyLabel = EditorGUIUtility.TrTextContent("Category is Empty");
        }

        struct SpriteCategorySelectionList
        {
            public string categoryName;
            public int categoryNameHash;
            public string[] names;
            public int[] nameHash;
            public Sprite[] sprites;
        }

        private SerializedProperty m_SpriteCategoryHash;
        private SerializedProperty m_SpritelabelHash;
        private SpriteSkin m_SpriteSkin;
        Dictionary<int, SpriteCategorySelectionList> m_SpriteLibSelection = new Dictionary<int, SpriteCategorySelectionList>();
        string[] m_CategorySelection;
        int[] m_CategorySelectionHash;
        int m_CategorySelectionIndex = 0;
        int m_PreviousCategoryHash = 0;
        int m_labelSelectionIndex = 0;
        int m_PreviouslabelHash = 0;
        SpriteSelectorWidget m_SpriteSelectorWidget = new SpriteSelectorWidget();

        public void OnEnable()
        {
            m_SpriteCategoryHash = serializedObject.FindProperty("m_CategoryHash");
            m_SpritelabelHash = serializedObject.FindProperty("m_labelHash");
            m_SpriteSkin = (target as SpriteResolver).GetComponent<SpriteSkin>();

            m_PreviousCategoryHash = SpriteResolver.ConvertFloatToInt(m_SpriteCategoryHash.floatValue);
            m_PreviouslabelHash = SpriteResolver.ConvertFloatToInt(m_SpritelabelHash.floatValue);
            UpdateSpriteLibrary();
        }

        SpriteResolver spriteResolver { get {return target as SpriteResolver; } }

        void UpdateSpriteLibrary()
        {
            m_SpriteLibSelection.Clear();
            int categoryHash = SpriteResolver.ConvertFloatToInt(m_SpriteCategoryHash.floatValue);
            int labelHash = SpriteResolver.ConvertFloatToInt(m_SpritelabelHash.floatValue);
            var spriteLib = spriteResolver.spriteLibrary;
            if (spriteLib != null)
            {
                foreach (var labels in spriteLib.labels)
                {
                    if (!m_SpriteLibSelection.ContainsKey(labels.hash))
                    {
                        var nameHash = labels.categoryList.Select(x => x.hash).Distinct().ToArray();
                        if (nameHash.Length > 0)
                        {
                            var selectionList = new SpriteCategorySelectionList()
                            {
                                names = nameHash.Select(x =>
                                {
                                    var v = labels.categoryList.FirstOrDefault(y => y.hash == x);
                                    return v.name;
                                }).ToArray(),
                                nameHash = nameHash,
                                sprites = nameHash.Select(x =>
                                {
                                    var v = labels.categoryList.FirstOrDefault(y => y.hash == x);
                                    return v.sprite;
                                }).ToArray(),
                                categoryName = labels.name,
                                categoryNameHash = labels.hash
                            };

                            m_SpriteLibSelection.Add(labels.hash, selectionList);
                        }
                    }
                }
            }
            m_CategorySelection = new string[1 + m_SpriteLibSelection.Keys.Count];
            m_CategorySelection[0] = TextContent.none;
            m_CategorySelectionHash = new int[1 + m_SpriteLibSelection.Keys.Count];
            m_CategorySelectionHash[0] = SpriteLibraryAsset.GetStringHash(TextContent.none);
            for (int i = 0; i < m_SpriteLibSelection.Keys.Count; ++i)
            {
                var selection = m_SpriteLibSelection[m_SpriteLibSelection.Keys.ElementAt(i)];
                m_CategorySelection[i + 1] = selection.categoryName;
                m_CategorySelectionHash[i + 1] = selection.categoryNameHash;
                if (selection.categoryNameHash == categoryHash)
                    m_CategorySelectionIndex = i + 1;
            }
            ValidateCategorySelectionIndexValue();
            if (m_CategorySelectionIndex > 0)
            {
                m_SpriteSelectorWidget.UpdateContents(m_SpriteLibSelection[m_CategorySelectionHash[m_CategorySelectionIndex]].sprites);
                if (m_SpriteLibSelection.ContainsKey(categoryHash))
                {
                    m_labelSelectionIndex = Array.FindIndex(m_SpriteLibSelection[categoryHash].nameHash, x => x == labelHash);
                }
            }
            spriteResolver.spriteLibChanged = false;
        }

        void ValidateCategorySelectionIndexValue()
        {
            if (m_CategorySelectionIndex < 0 || m_CategorySelectionHash.Length <= m_CategorySelectionIndex)
                m_CategorySelectionIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (spriteResolver.spriteLibChanged)
                UpdateSpriteLibrary();

            var currentlabelHashValue = SpriteResolver.ConvertFloatToInt(m_SpritelabelHash.floatValue);
            var currentCategoryHashValue = SpriteResolver.ConvertFloatToInt(m_SpriteCategoryHash.floatValue);

            m_CategorySelectionIndex = Array.FindIndex(m_CategorySelectionHash, x => x == currentCategoryHashValue);
            ValidateCategorySelectionIndexValue();

            if (m_CategorySelection.Length == 1)
            {
                EditorGUILayout.LabelField(Style.noSpriteLibContainer);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                m_CategorySelectionIndex = EditorGUILayout.Popup(Style.categoryLabel, m_CategorySelectionIndex, m_CategorySelection);
                if (m_CategorySelectionIndex != 0)
                {
                    var selection = m_SpriteLibSelection[m_CategorySelectionHash[m_CategorySelectionIndex]];
                    if (selection.names.Length <= 0)
                    {
                        EditorGUILayout.LabelField(Style.categoryIsEmptyLabel);
                    }
                    else
                    {
                        if (m_labelSelectionIndex < 0 || m_labelSelectionIndex >= selection.names.Length)
                            m_labelSelectionIndex = 0;
                        m_labelSelectionIndex = EditorGUILayout.Popup(Style.labelLabel, m_labelSelectionIndex, selection.names);
                        m_labelSelectionIndex = m_SpriteSelectorWidget.ShowGUI(m_labelSelectionIndex);
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    currentCategoryHashValue = m_CategorySelectionHash[m_CategorySelectionIndex];
                    if (m_SpriteLibSelection.ContainsKey(currentCategoryHashValue))
                    {
                        var hash = m_SpriteLibSelection[currentCategoryHashValue].nameHash;
                        if (hash.Length > 0)
                        {
                            if (m_labelSelectionIndex < 0 || m_labelSelectionIndex >= hash.Length)
                                m_labelSelectionIndex = 0;
                            currentlabelHashValue = m_SpriteLibSelection[currentCategoryHashValue].nameHash[m_labelSelectionIndex];
                        }
                    }

                    m_SpriteCategoryHash.floatValue = SpriteResolver.ConvertIntToFloat(currentCategoryHashValue);
                    m_SpritelabelHash.floatValue = SpriteResolver.ConvertIntToFloat(currentlabelHashValue);
                    serializedObject.ApplyModifiedProperties();

                    var sf = target as SpriteResolver;
                    if (m_SpriteSkin != null)
                        m_SpriteSkin.ignoreNextSpriteChange = true;
                    sf.ResolveSpriteToSpriteRenderer();
                }

                if (m_PreviousCategoryHash != currentCategoryHashValue)
                {
                    if (m_SpriteLibSelection.ContainsKey(currentCategoryHashValue))
                    {
                        m_SpriteSelectorWidget.UpdateContents(m_SpriteLibSelection[currentCategoryHashValue].sprites);
                    }
                    m_PreviousCategoryHash = currentCategoryHashValue;
                }

                if (m_PreviouslabelHash != currentlabelHashValue)
                {
                    if (m_SpriteLibSelection.ContainsKey(currentCategoryHashValue))
                        m_labelSelectionIndex = Array.FindIndex(m_SpriteLibSelection[currentCategoryHashValue].nameHash, x => x == currentlabelHashValue);
                    m_PreviouslabelHash = currentlabelHashValue;
                }

                serializedObject.ApplyModifiedProperties();
                if (m_SpriteSelectorWidget.NeedUpdatePreview())
                    this.Repaint();
            }
        }
    }
}
