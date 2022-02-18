using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Sprites;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor._2D.Sprite.Editor
{
    [RequireSpriteDataProvider(typeof(ISecondaryTextureDataProvider), typeof(ITextureDataProvider))]
    internal class SpriteSecondaryTexturesModule : SpriteEditorModuleBase
    {
        private static class Styles
        {
            public static readonly string invalidEntriesWarning = L10n.Tr("Invalid secondary Texture entries (without names or Textures) have been removed.");
            public static readonly string nameUniquenessWarning = L10n.Tr("Every secondary Texture attached to the Sprite must have a unique name.");
            public static readonly string builtInNameCollisionWarning = L10n.Tr("The names _MainTex and _AlphaTex are reserved for internal use.");
            public static readonly GUIContent panelTitle = EditorGUIUtility.TrTextContent("Secondary Textures");
            public static readonly GUIContent name = EditorGUIUtility.TrTextContent("Name");
            public static readonly GUIContent texture = EditorGUIUtility.TrTextContent("Texture");
            public const float textFieldDropDownWidth = 18.0f;
        }


        ReorderableList m_ReorderableList;
        Vector2 m_ReorderableListScrollPosition;
        string[] m_SuggestedNames;
        private IMGUIContainer m_SecondaryTextureInspectorContainer;
        internal List<SecondarySpriteTexture> secondaryTextureList { get; private set; }

        public override string moduleName
        {
            get { return Styles.panelTitle.text; }
        }

        public override bool ApplyRevert(bool apply)
        {
            if (apply)
            {
                var secondaryTextureDataProvider = spriteEditor.GetDataProvider<ISecondaryTextureDataProvider>();


                // Remove invalid entries.
                var validEntries = secondaryTextureList.FindAll(x => (x.name != null && x.name != "" && x.texture != null));
                if (validEntries.Count < secondaryTextureList.Count)
                    Debug.Log(Styles.invalidEntriesWarning);

                secondaryTextureDataProvider.textures = validEntries.ToArray();
            }

            return true;
        }

        public override bool CanBeActivated()
        {
            var dataProvider = spriteEditor.GetDataProvider<ISpriteEditorDataProvider>();
            return dataProvider != null && dataProvider.spriteImportMode != SpriteImportMode.None;
        }

        public override void DoPostGUI()
        {
        }

        public void SecondaryTextureReorderableListUI()
        {
            using (new EditorGUI.DisabledScope(spriteEditor.editingDisabled))
            {
                var windowDimension = spriteEditor.windowDimension;

                GUILayout.BeginArea(new Rect(0, 0, 290, 290), Styles.panelTitle, GUI.skin.window);
                m_ReorderableListScrollPosition = GUILayout.BeginScrollView(m_ReorderableListScrollPosition);
                m_ReorderableList.DoLayoutList();
                GUILayout.EndScrollView();
                GUILayout.EndArea();

                // Deselect the list item if left click outside the panel area.
                UnityEngine.Event e = UnityEngine.Event.current;
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    m_ReorderableList.index = -1;
                    OnSelectCallback(m_ReorderableList);
                    spriteEditor.RequestRepaint();
                }
            }
        }

        public override void DoMainGUI()
        {
        }

        public override void DoToolbarGUI(Rect drawArea)
        {
        }

        public override void OnModuleActivate()
        {
            var secondaryTextureDataProvider = spriteEditor.GetDataProvider<ISecondaryTextureDataProvider>();
            secondaryTextureList = secondaryTextureDataProvider.textures == null ? new List<SecondarySpriteTexture>() : secondaryTextureDataProvider.textures.ToList();

            m_ReorderableListScrollPosition = Vector2.zero;
            m_ReorderableList = new ReorderableList(secondaryTextureList, typeof(SecondarySpriteTexture), false, false, true, true);
            m_ReorderableList.drawElementCallback = DrawSpriteSecondaryTextureElement;
            m_ReorderableList.onAddCallback = AddSpriteSecondaryTextureElement;
            m_ReorderableList.onRemoveCallback = RemoveSpriteSecondaryTextureElement;
            m_ReorderableList.onCanAddCallback = CanAddSpriteSecondaryTextureElement;
            m_ReorderableList.elementHeightCallback = (int index) => (EditorGUIUtility.singleLineHeight * 3) + 5;
            m_ReorderableList.onSelectCallback = OnSelectCallback;

            spriteEditor.selectedSpriteRect = null;

            string suggestedNamesPrefs = EditorPrefs.GetString("SecondarySpriteTexturePropertyNames");
            if (!string.IsNullOrEmpty(suggestedNamesPrefs))
            {
                m_SuggestedNames = suggestedNamesPrefs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < m_SuggestedNames.Length; ++i)
                    m_SuggestedNames[i] = m_SuggestedNames[i].Trim();

                Array.Sort(m_SuggestedNames);
            }
            else
                m_SuggestedNames = null;

            m_SecondaryTextureInspectorContainer = new IMGUIContainer(SecondaryTextureReorderableListUI)
            {
                style =
                {
                    flexGrow = 0,
                    flexBasis = 1,
                    flexShrink = 0,
                    minWidth = 290,
                    minHeight = 290,
                    bottom = 24,
                    right = 24,
                    position = new StyleEnum<Position>(Position.Absolute)
                },
                name = "SecondaryTextureInspector"
            };
            spriteEditor.GetMainVisualContainer().Add(m_SecondaryTextureInspectorContainer);
        }

        void OnSelectCallback(ReorderableList list)
        {
            // Preview the current selected secondary texture.
            Texture2D previewTexture = null;
            int width = 0, height = 0;

            var textureDataProvider = spriteEditor.GetDataProvider<ITextureDataProvider>();
            if (textureDataProvider != null)
            {
                previewTexture = textureDataProvider.previewTexture;
                textureDataProvider.GetTextureActualWidthAndHeight(out width, out height);
            }

            if (m_ReorderableList.index >= 0 && m_ReorderableList.index < secondaryTextureList.Count)
                previewTexture = secondaryTextureList[m_ReorderableList.index].texture;

            if (previewTexture != null)
                spriteEditor.SetPreviewTexture(previewTexture, width, height);
        }

        public override void OnModuleDeactivate()
        {
            DisplayMainTexture();
            if (spriteEditor.GetMainVisualContainer().Contains(m_SecondaryTextureInspectorContainer))
                spriteEditor.GetMainVisualContainer().Remove(m_SecondaryTextureInspectorContainer);
        }

        void DrawSpriteSecondaryTextureElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            bool dataModified = false;
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 70.0f;
            SecondarySpriteTexture secondaryTexture = secondaryTextureList[index];

            // "Name" text field
            EditorGUI.BeginChangeCheck();
            var r = new Rect(rect.x, rect.y + 5, rect.width - Styles.textFieldDropDownWidth, EditorGUIUtility.singleLineHeight);
            string newName = EditorGUI.TextField(r, Styles.name, secondaryTexture.name);
            dataModified = EditorGUI.EndChangeCheck();

            // Suggested names
            if (m_SuggestedNames != null)
            {
                var popupRect = new Rect(r.x + r.width, r.y, Styles.textFieldDropDownWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.BeginChangeCheck();
                int selected = EditorGUI.Popup(popupRect, -1, m_SuggestedNames, EditorStyles.textFieldDropDown);
                if (EditorGUI.EndChangeCheck())
                {
                    newName = m_SuggestedNames[selected];
                    dataModified = true;
                }
            }

            if (dataModified)
            {
                if (!string.IsNullOrEmpty(newName) && newName != secondaryTexture.name && secondaryTextureList.Exists(x => x.name == newName))
                    Debug.LogWarning(Styles.nameUniquenessWarning);
                else if (newName == "_MainTex" || newName == "_AlphaTex")
                    Debug.LogWarning(Styles.builtInNameCollisionWarning);
                else
                    secondaryTexture.name = newName;
            }

            // "Texture" object field
            EditorGUI.BeginChangeCheck();
            r.width = rect.width;
            r.y += EditorGUIUtility.singleLineHeight;
            secondaryTexture.texture = EditorGUI.ObjectField(r, Styles.texture, secondaryTexture.texture, typeof(Texture2D), false) as Texture2D;
            dataModified = dataModified || EditorGUI.EndChangeCheck();

            if (dataModified)
            {
                secondaryTextureList[index] = secondaryTexture;
                spriteEditor.SetDataModified();
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        void AddSpriteSecondaryTextureElement(ReorderableList list)
        {
            m_ReorderableListScrollPosition += new Vector2(0.0f, list.elementHeightCallback(0));
            secondaryTextureList.Add(new SecondarySpriteTexture());
            spriteEditor.SetDataModified();
        }

        void RemoveSpriteSecondaryTextureElement(ReorderableList list)
        {
            DisplayMainTexture();
            secondaryTextureList.RemoveAt(list.index);
            spriteEditor.SetDataModified();
        }

        bool CanAddSpriteSecondaryTextureElement(ReorderableList list)
        {
            return list.count < 8;
        }

        void DisplayMainTexture()
        {
            ITextureDataProvider textureDataProvider = spriteEditor.GetDataProvider<ITextureDataProvider>();
            if (textureDataProvider != null && textureDataProvider.previewTexture != null)
            {
                Texture2D mainTexture = textureDataProvider.previewTexture;
                int width = 0, height = 0;
                textureDataProvider.GetTextureActualWidthAndHeight(out width, out height);
                spriteEditor.SetPreviewTexture(mainTexture, width, height);
            }
        }
    }
}
