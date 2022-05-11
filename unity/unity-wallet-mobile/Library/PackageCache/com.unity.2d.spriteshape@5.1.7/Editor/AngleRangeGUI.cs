using UnityEditor;
using UnityEngine;
using UnityEditor.Sprites;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.U2D
{
    public class AngleRangeGUI
    {
        public static readonly int kLeftHandleHashCode = "LeftHandle".GetHashCode();
        public static readonly int kRightHandleHashCode = "RightHandle".GetHashCode();
        public const float kRangeWidth = 10f;

        public static void AngleRangeField(Rect rect, ref float start, ref float end, float angleOffset, float radius, bool snap, bool drawLine, bool drawCircle, Color rangeColor)
        {
            var leftId = GUIUtility.GetControlID(kLeftHandleHashCode, FocusType.Passive);
            var rightId = GUIUtility.GetControlID(kRightHandleHashCode, FocusType.Passive);
            AngleRangeField(rect, leftId, rightId, ref start, ref end, angleOffset, radius, snap, drawLine, drawCircle, rangeColor);
        }

        public static void AngleRangeField(Rect rect, int leftHandleID, int rightHandleID, ref float start, ref float end, float angleOffset, float radius, bool snap, bool drawLine, bool drawCircle, Color rangeColor)
        {
            Color activeColor = Handles.color;

            if (Event.current.type == EventType.Repaint)
            {
                float range = end - start;

                Color color = Handles.color;
                Handles.color = rangeColor;
                if (range < 0f)
                    Handles.color = Color.red;

                SpriteShapeHandleUtility.DrawSolidArc(rect.center, Vector3.forward, Quaternion.AngleAxis(start + angleOffset, Vector3.forward) * Vector3.right, range, radius, kRangeWidth);
                Handles.color = color;
            }

            EditorGUI.BeginChangeCheck();

            float handleSize = 15f;

            start = AngleField(rect, leftHandleID, start, angleOffset, new Vector2(-3.5f, -7.5f), start + angleOffset + 90f, handleSize, radius, snap, drawLine, drawCircle, SpriteShapeHandleUtility.RangeLeftCap);

            if (EditorGUI.EndChangeCheck())
                start = Mathf.Clamp(start, end - 360f, end);

            EditorGUI.BeginChangeCheck();

            end = AngleField(rect, rightHandleID, end, angleOffset, new Vector2(3.5f, -7.5f), end + angleOffset + 90f, handleSize, radius, snap, drawLine, false, SpriteShapeHandleUtility.RangeRightCap);

            if (EditorGUI.EndChangeCheck())
                end = Mathf.Clamp(end, end, start + 360f);

            Handles.color = activeColor;
        }

        public static void AngleRangeField(Rect rect, SerializedProperty startProperty, SerializedProperty endProperty, float angleOffset, float radius, bool snap, bool drawLine, bool drawCircle, Color rangeColor)
        {
            var start = startProperty.floatValue;
            var end = endProperty.floatValue;

            EditorGUI.BeginChangeCheck();
            AngleRangeField(rect, ref start, ref end, angleOffset, radius, snap, drawLine, drawCircle, rangeColor);
            if (EditorGUI.EndChangeCheck())
            {
                startProperty.floatValue = start;
                endProperty.floatValue = end;
            }
        }

        public static void AngleField(int id, SerializedProperty property, float angleOffset, Vector2 handleOffset, float handleAngle, float handleSize, float radius, bool snap, bool drawLine, bool drawCircle, Handles.CapFunction drawCapFunction)
        {
            EditorGUI.BeginChangeCheck();
            float value = AngleField(id, property.floatValue, angleOffset, handleOffset, handleAngle, handleSize, radius, snap, drawLine, drawCircle, drawCapFunction);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = value;
            }
        }

        public static void AngleField(Rect r, int id, SerializedProperty property, float angleOffset, Vector2 handleOffset, float handleAngle, float handleSize, float radius, bool snap, bool drawLine, bool drawCircle, Handles.CapFunction drawCapFunction)
        {
            EditorGUI.BeginChangeCheck();
            float value = AngleField(r, id, property.floatValue, angleOffset, handleOffset, handleAngle, handleSize, radius, snap, drawLine, drawCircle, drawCapFunction);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = value;
            }
        }

        public static float AngleField(int id, float angle, float angleOffset, Vector2 handleOffset, float handleAngle, float radius, float handleSize, bool snap, bool drawLine, bool drawCircle, Handles.CapFunction drawCapFunction)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, radius * 2f);
            return AngleField(rect, id, angle, angleOffset, handleOffset, handleAngle, radius, handleSize, snap, drawLine, drawCircle, drawCapFunction);
        }

        public static float AngleField(Rect rect, int id, float angle, float angleOffset, Vector2 handleOffset, float handleAngle, float handleSize, float radius, bool snap, bool drawLine, bool drawCircle, Handles.CapFunction drawCapFunction)
        {
            float offsetedAngle = angle + angleOffset;
            Vector2 pos = new Vector2(Mathf.Cos(offsetedAngle * Mathf.Deg2Rad), Mathf.Sin(offsetedAngle * Mathf.Deg2Rad)) * radius + rect.center;

            if (Event.current.type == EventType.Repaint)
            {
                if (drawCircle)
                    Handles.DrawWireDisc(rect.center, Vector3.forward, radius);

                if (drawLine)
                    Handles.DrawLine(rect.center, pos);
            }

            if (GUI.enabled)
            {
                EditorGUI.BeginChangeCheck();

                Quaternion rotation = Quaternion.AngleAxis(handleAngle, Vector3.forward);
                Vector2 posNew = SpriteShapeHandleUtility.Slider2D(id, pos, rotation * handleOffset, rotation, handleSize, drawCapFunction);

                if (EditorGUI.EndChangeCheck())
                {
                    Vector2 v1 = pos - rect.center;
                    Vector2 v2 = posNew - rect.center;

                    float angleDirection = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(v1, v2)));
                    float angleIncrement = Vector2.Angle(v1, v2);

                    angle += angleIncrement * angleDirection;

                    if (snap)
                        angle = Mathf.RoundToInt(angle);
                }
            }

            return angle;
        }
    }
}
