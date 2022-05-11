using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal static class BoneDrawingUtility
    {
        public static float GetBoneRadius(Vector3 position, float scale = 1.0f)
        {
            if (Camera.current != null)
            {
                return 0.15f * scale * HandleUtility.GetHandleSize(position);
            }

            return 10f * scale / Handles.matrix.GetColumn(0).magnitude;
        }

        public static void DrawBoneNode(Vector3 position, Vector3 forward, Color color, float scale = 1.0f)
        {
            Color c = Handles.color;
            Handles.color = color;

            Handles.DrawSolidDisc(position, -forward, GetBoneRadius(position, scale) * 0.3f);

            Handles.color = c;
        }

        public static void DrawBone(Vector3 position, Vector3 endPosition, Vector3 forward, Color color, float scale = 1.0f)
        {
            Color c = Handles.color;
            Handles.color = color;

            var right = Vector3.right;
            var v = endPosition - position;

            if (v.sqrMagnitude != 0)
                right = v.normalized;

            var up = Vector3.Cross(right, forward).normalized;
            var radius = GetBoneRadius(position, scale) * 0.5f;
            var numSamples = 12;

            if (v.sqrMagnitude <= radius * radius)
                DrawingUtility.DrawSolidArc(position, -forward, up, 360f, radius, numSamples * 2);
            else
            {    
                DrawingUtility.DrawSolidArc(position, -forward, up, 180f, radius, numSamples);
                DrawingUtility.DrawLine(position, endPosition, forward, radius * 2f, 0f);
            }

            Handles.color = c;
        }

        public static void DrawBoneOutline(Vector3 position, Vector3 endPosition, Vector3 forward, Color color, float outlineScale = 1.35f, float scale = 1.0f)
        {
            outlineScale = Mathf.Max(1f, outlineScale);
            
            Color c = Handles.color;
            Handles.color = color;

            var right = Vector3.right;
            var v = endPosition - position;

            if (v.sqrMagnitude != 0)
                right = v.normalized;

            var up = Vector3.Cross(right, forward).normalized;
            var radius = GetBoneRadius(position, scale) * 0.5f;
            var outlineWidth = radius * (outlineScale - 1f);
            var numSamples = 12;

            if (v.sqrMagnitude <= radius * radius)
                DrawingUtility.DrawSolidArc(position, -forward, up, 360f, radius, outlineScale, numSamples * 2);
            else
            {    
                DrawingUtility.DrawSolidArc(position, -forward, up, 180f, radius, outlineScale, numSamples);
                DrawingUtility.DrawSolidArc(endPosition, -forward, -up, 180f, outlineWidth, 0f, numSamples);
                DrawingUtility.DrawLine(position + up * (radius + outlineWidth * 0.5f), endPosition + up * outlineWidth * 0.5f, forward, outlineWidth, outlineWidth);
                DrawingUtility.DrawLine(position - up * (radius + outlineWidth * 0.5f), endPosition - up * outlineWidth * 0.5f, forward, outlineWidth, outlineWidth);
            }

            Handles.color = c;
        }
    }
}
