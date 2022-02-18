using UnityEditor.U2D.Animation;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

namespace UnityEditor.Experimental.U2D.Animation
{
    [CustomEditor(typeof(SpriteLibraryAsset))]
    internal class SpriteLibraryAssetInspector : Editor
    {
        static class Style
        {
            public static GUIContent duplicateWarningText = EditorGUIUtility.TrTextContent("Duplicate name found or name hash clashes. Please use a different name");
            public static GUIContent duplicateWarning = EditorGUIUtility.TrIconContent("console.warnicon.sml", duplicateWarningText.text);
            public static GUIContent nameLabel = new GUIContent(TextContent.label);
            public static string categoryListLabel = L10n.Tr("Category List");
            public static int lineSpacing = 3;
        }

        private SerializedProperty m_Labels;
        private ReorderableList m_LabelReorderableList;

        private bool m_UpdateHash = false;

        private readonly float kElementHeight = EditorGUIUtility.singleLineHeight * 3;

        public void OnEnable()
        {
            m_Labels = serializedObject.FindProperty("m_Labels");

            m_LabelReorderableList = new ReorderableList(serializedObject, m_Labels, true, false, true, true);
            SetupOrderList();
        }

        public void OnDisable()
        {
            var sla = target as SpriteLibraryAsset;
            if (sla != null)
                sla.UpdateHashes();
        }

        float GetElementHeight(int index)
        {
            var property = m_Labels.GetArrayElementAtIndex(index);
            var spriteListProp = property.FindPropertyRelative("m_CategoryList");
            if (spriteListProp.isExpanded)
                return (spriteListProp.arraySize + 1) * (EditorGUIUtility.singleLineHeight + Style.lineSpacing) + kElementHeight;

            return kElementHeight;
        }

        void DrawElement(Rect rect, int index, bool selected, bool focused)
        {
            var property = m_Labels.GetArrayElementAtIndex(index);

            var catRect = new Rect(rect.x, rect.y, rect.width - kElementHeight, EditorGUIUtility.singleLineHeight);
            var vaRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width - kElementHeight, EditorGUIUtility.singleLineHeight);

            var categoryProp = property.FindPropertyRelative("m_Name");

            var spriteListProp = property.FindPropertyRelative("m_CategoryList");

            EditorGUI.BeginChangeCheck();
            var newCatName = EditorGUI.DelayedTextField(catRect, categoryProp.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                newCatName = newCatName.Trim();
                m_UpdateHash = true;
                if (categoryProp.stringValue != newCatName)
                {
                    // Check if this nameLabel is already taken
                    if (!IsNameInUsed(newCatName, m_Labels, "m_Name", 0))
                        categoryProp.stringValue = newCatName;
                    else
                        Debug.LogWarning(Style.duplicateWarningText.text);
                }
            }

            spriteListProp.isExpanded = EditorGUI.Foldout(vaRect, spriteListProp.isExpanded, Style.categoryListLabel, true);
            if (spriteListProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                var indentedRect = EditorGUI.IndentedRect(vaRect);
                var labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 40 + indentedRect.x - vaRect.x;
                indentedRect.y += EditorGUIUtility.singleLineHeight + Style.lineSpacing;
                var sizeRect = indentedRect;
                int size = EditorGUI.IntField(sizeRect, TextContent.size, spriteListProp.arraySize);
                if (size != spriteListProp.arraySize && size >= 0)
                    spriteListProp.arraySize = size;
                indentedRect.y += EditorGUIUtility.singleLineHeight + Style.lineSpacing;
                DrawSpriteListProperty(indentedRect, spriteListProp);
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.indentLevel--;
            }
        }

        void DrawSpriteListProperty(Rect rect, SerializedProperty spriteListProp)
        {
            for (int i = 0; i < spriteListProp.arraySize; ++i)
            {
                var element = spriteListProp.GetArrayElementAtIndex(i);
                EditorGUI.BeginChangeCheck();
                var oldName = element.FindPropertyRelative("m_Name").stringValue;
                var nameRect = new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight);
                bool nameDuplicate = IsNameInUsed(oldName, spriteListProp, "m_Name", 1);
                if (nameDuplicate)
                {
                    nameRect.width -= 20;
                }
                var newName = EditorGUI.DelayedTextField(
                    nameRect,
                    Style.nameLabel,
                    oldName);
                if (nameDuplicate)
                {
                    nameRect.x += nameRect.width;
                    nameRect.width = 20;
                    GUI.Label(nameRect, Style.duplicateWarning);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    newName = newName.Trim();
                    element.FindPropertyRelative("m_Name").stringValue = newName;
                }

                EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("m_Sprite"));
                rect.y += EditorGUIUtility.singleLineHeight + Style.lineSpacing;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            if (EditorGUI.EndChangeCheck())
                SetupOrderList();

            m_UpdateHash = false;
            m_LabelReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if (m_UpdateHash)
                (target as SpriteLibraryAsset).UpdateHashes();
        }

        bool IsNameInUsed(string name, SerializedProperty property, string propertyField, int threshold)
        {
            int count = 0;
            var nameHash = SpriteLibraryAsset.GetStringHash(name);
            for (int i = 0; i < property.arraySize; ++i)
            {
                var sp = property.GetArrayElementAtIndex(i);
                var otherName = sp.FindPropertyRelative(propertyField).stringValue;
                var otherNameHash = SpriteLibraryAsset.GetStringHash(otherName);
                if (otherName == name || nameHash == otherNameHash)
                {
                    count++;
                    if (count > threshold)
                        return true;
                }
            }

            return false;
        }

        void OnAddCallback(ReorderableList list)
        {
            var oldSize = m_Labels.arraySize;
            m_Labels.arraySize += 1;
            const string kNewCatName = "New Category";
            string newCatName = kNewCatName;
            int catNameIncrement = 1;
            while (true)
            {
                if (IsNameInUsed(newCatName, m_Labels, "m_Name", 0))
                    newCatName = string.Format("{0} {1}", kNewCatName, catNameIncrement++);
                else
                    break;
            }

            var sp = m_Labels.GetArrayElementAtIndex(oldSize);
            sp.FindPropertyRelative("m_Name").stringValue = newCatName;
            sp.FindPropertyRelative("m_Hash").intValue = SpriteLibraryAsset.GetStringHash(newCatName);
        }

        private void SetupOrderList()
        {
            m_LabelReorderableList.drawElementCallback = DrawElement;
            m_LabelReorderableList.elementHeight = kElementHeight;
            m_LabelReorderableList.elementHeightCallback = GetElementHeight;
            m_LabelReorderableList.onAddCallback = OnAddCallback;
        }
    }
}
