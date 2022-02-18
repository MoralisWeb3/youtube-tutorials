using UnityEditor;
using UnityEngine;
using UnityEditor.Sprites;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.U2D
{
    public class SpriteShapeEditorGUI
    {
        private const float kSpacingSubLabel = 2.0f;
        private const float kMiniLabelW = 13;
        private const int kVerticalSpacingMultiField = 0;
        private const float kIndentPerLevel = 15;
        public static int s_FoldoutHash = "Foldout".GetHashCode();

        public static void MultiDelayedIntField(Rect position, GUIContent[] subLabels, int[] values, float labelWidth)
        {
            int eCount = values.Length;
            float w = (position.width - (eCount - 1) * kSpacingSubLabel) / eCount;
            Rect nr = new Rect(position);
            nr.width = w;
            float t = EditorGUIUtility.labelWidth;
            int l = EditorGUI.indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = 0;
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = EditorGUI.DelayedIntField(nr, subLabels[i], values[i]);
                nr.x += w + kSpacingSubLabel;
            }
            EditorGUIUtility.labelWidth = t;
            EditorGUI.indentLevel = l;
        }
    }
}
