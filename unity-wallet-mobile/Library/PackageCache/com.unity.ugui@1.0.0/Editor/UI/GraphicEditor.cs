using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI Graphics.
    /// Extend this class to write your own graphic editor.
    /// </summary>

    [CustomEditor(typeof(MaskableGraphic), false)]
    [CanEditMultipleObjects]
    public class GraphicEditor : Editor
    {
        protected SerializedProperty m_Script;
        protected SerializedProperty m_Color;
        protected SerializedProperty m_Material;
        protected SerializedProperty m_RaycastTarget;
        protected SerializedProperty m_RaycastPadding;
        protected SerializedProperty m_Maskable;

        private GUIContent m_CorrectButtonContent;
        protected AnimBool m_ShowNativeSize;

        GUIContent m_PaddingContent;
        GUIContent m_LeftContent;
        GUIContent m_RightContent;
        GUIContent m_TopContent;
        GUIContent m_BottomContent;
        static private bool m_ShowPadding = false;

        protected virtual void OnDisable()
        {
            Tools.hidden = false;
            m_ShowNativeSize.valueChanged.RemoveListener(Repaint);
        }

        protected virtual void OnEnable()
        {
            m_CorrectButtonContent = EditorGUIUtility.TrTextContent("Set Native Size", "Sets the size to match the content.");
            m_PaddingContent = EditorGUIUtility.TrTextContent("Raycast Padding");
            m_LeftContent = EditorGUIUtility.TrTextContent("Left");
            m_RightContent = EditorGUIUtility.TrTextContent("Right");
            m_TopContent = EditorGUIUtility.TrTextContent("Top");
            m_BottomContent = EditorGUIUtility.TrTextContent("Bottom");

            m_Script = serializedObject.FindProperty("m_Script");
            m_Color = serializedObject.FindProperty("m_Color");
            m_Material = serializedObject.FindProperty("m_Material");
            m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget");
            m_RaycastPadding = serializedObject.FindProperty("m_RaycastPadding");
            m_Maskable = serializedObject.FindProperty("m_Maskable");

            m_ShowNativeSize = new AnimBool(false);
            m_ShowNativeSize.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Script);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            MaskableControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Set if the 'Set Native Size' button should be visible for this editor.
        /// </summary>
        /// <param name="show">Are we showing or hiding the AnimBool for the size.</param>
        /// <param name="instant">Should the size AnimBool change instantly.</param>
        protected void SetShowNativeSize(bool show, bool instant)
        {
            if (instant)
                m_ShowNativeSize.value = show;
            else
                m_ShowNativeSize.target = show;
        }

        /// <summary>
        /// GUI for showing a button that sets the size of the RectTransform to the native size for this Graphic.
        /// </summary>
        protected void NativeSizeButtonGUI()
        {
            if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(EditorGUIUtility.labelWidth);
                    if (GUILayout.Button(m_CorrectButtonContent, EditorStyles.miniButton))
                    {
                        foreach (Graphic graphic in targets.Select(obj => obj as Graphic))
                        {
                            Undo.RecordObject(graphic.rectTransform, "Set Native Size");
                            graphic.SetNativeSize();
                            EditorUtility.SetDirty(graphic);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
        }

        protected void MaskableControlsGUI()
        {
            EditorGUILayout.PropertyField(m_Maskable);
        }

        /// <summary>
        /// GUI related to the appearance of the Graphic. Color and Material properties appear here.
        /// </summary>
        protected void AppearanceControlsGUI()
        {
            EditorGUILayout.PropertyField(m_Color);
            EditorGUILayout.PropertyField(m_Material);
        }

        /// <summary>
        /// GUI related to the Raycasting settings for the graphic.
        /// </summary>
        protected void RaycastControlsGUI()
        {
            EditorGUILayout.PropertyField(m_RaycastTarget);

            m_ShowPadding = EditorGUILayout.Foldout(m_ShowPadding, m_PaddingContent);

            if (m_ShowPadding)
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.indentLevel++;
                    Vector4 newPadding = m_RaycastPadding.vector4Value;

                    newPadding.x = EditorGUILayout.FloatField(m_LeftContent, newPadding.x);
                    newPadding.z = EditorGUILayout.FloatField(m_RightContent, newPadding.z);
                    newPadding.w = EditorGUILayout.FloatField(m_TopContent, newPadding.w);
                    newPadding.y = EditorGUILayout.FloatField(m_BottomContent, newPadding.y);

                    if (check.changed)
                    {
                        m_RaycastPadding.vector4Value = newPadding;
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}
