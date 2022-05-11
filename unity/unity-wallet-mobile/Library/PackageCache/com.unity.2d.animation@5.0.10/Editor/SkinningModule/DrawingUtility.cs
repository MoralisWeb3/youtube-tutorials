using UnityEditor.U2D.Common;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class DrawingUtility
    {
        public static readonly Color kSpriteBorderColor = new Color(0.25f, 0.5f, 1f, 0.75f);

        public static void DrawLine(Vector3 p1, Vector3 p2, Vector3 normal, float width)
        {
            DrawLine(p1, p2, normal, width, width);
        }

        public static void DrawLine(Vector3 p1, Vector3 p2, Vector3 normal, float widthP1, float widthP2)
        {
            DrawLine(p1, p2, normal, widthP1, widthP2, Handles.color);
        }

        public static void DrawLine(Vector3 p1, Vector3 p2, Vector3 normal, float widthP1, float widthP2, Color color)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            Vector3 up = Vector3.Cross(normal, p2 - p1).normalized;

            Shader.SetGlobalFloat("_HandleSize", 1);

            InternalEditorBridge.ApplyWireMaterial();
            GL.PushMatrix();
            GL.MultMatrix(Handles.matrix);
            GL.Begin(4);
            GL.Color(color);
            GL.Vertex(p1 + up * widthP1 * 0.5f);
            GL.Vertex(p1 - up * widthP1 * 0.5f);
            GL.Vertex(p2 - up * widthP2 * 0.5f);
            GL.Vertex(p1 + up * widthP1 * 0.5f);
            GL.Vertex(p2 - up * widthP2 * 0.5f);
            GL.Vertex(p2 + up * widthP2 * 0.5f);
            GL.End();
            GL.PopMatrix();
        }

        public static void BeginLines(Color color)
        {
            InternalEditorBridge.ApplyWireMaterial();
            GL.PushMatrix();
            GL.MultMatrix(Handles.matrix);
            GL.Begin(GL.LINES);
            GL.Color(color);
        }

        public static void BeginSolidLines()
        {
            InternalEditorBridge.ApplyWireMaterial();
            GL.PushMatrix();
            GL.MultMatrix(Handles.matrix);
            GL.Begin(GL.TRIANGLES);
        }

        public static void EndLines()
        {
            GL.End();
            GL.PopMatrix();
        }

        public static void DrawLine(Vector3 p1, Vector3 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }

        public static void DrawSolidLine(float width, Vector3 p1, Vector3 p2)
        {
            DrawSolidLine(p1, p2, Vector3.forward, width, width);
        }

        public static void DrawSolidLine(Vector3 p1, Vector3 p2, Vector3 normal, float widthP1, float widthP2)
        {
            GL.Color(Handles.color);

            Vector3 right = Vector3.Cross(normal, p2 - p1).normalized;

            GL.Vertex(p1 + right * widthP1 * 0.5f);
            GL.Vertex(p1 - right * widthP1 * 0.5f);
            GL.Vertex(p2 - right * widthP2 * 0.5f);
            GL.Vertex(p1 + right * widthP1 * 0.5f);
            GL.Vertex(p2 - right * widthP2 * 0.5f);
            GL.Vertex(p2 + right * widthP2 * 0.5f);
        }

        public static void DrawBox(Rect position)
        {
            Vector3[] points = new Vector3[5];
            int i = 0;
            points[i++] = new Vector3(position.xMin, position.yMin, 0f);
            points[i++] = new Vector3(position.xMax, position.yMin, 0f);
            points[i++] = new Vector3(position.xMax, position.yMax, 0f);
            points[i++] = new Vector3(position.xMin, position.yMax, 0f);

            DrawLine(points[0], points[1]);
            DrawLine(points[1], points[2]);
            DrawLine(points[2], points[3]);
            DrawLine(points[3], points[0]);
        }

        public static void DrawMesh(Mesh mesh, Material material, Matrix4x4 matrix)
        {
            Debug.Assert(mesh != null);
            Debug.Assert(material != null);

            if (Event.current.type != EventType.Repaint)
                return;

            material.SetFloat("_AdjustLinearForGamma", PlayerSettings.colorSpace == ColorSpace.Linear ? 1.0f : 0.0f);
            material.SetPass(0);
            Graphics.DrawMeshNow(mesh, Handles.matrix * matrix);
        }

        public static void DrawGUIStyleCap(int controlID, Vector3 position, Quaternion rotation, float size, GUIStyle guiStyle)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (Camera.current && Vector3.Dot(position - Camera.current.transform.position, Camera.current.transform.forward) < 0f)
                return;

            Handles.BeginGUI();
            guiStyle.Draw(GetGUIStyleRect(guiStyle, position), GUIContent.none, controlID);
            Handles.EndGUI();
        }

        private static Rect GetGUIStyleRect(GUIStyle style, Vector3 position)
        {
            Vector2 vector = HandleUtility.WorldToGUIPoint(position);

            float fixedWidth = style.fixedWidth;
            float fixedHeight = style.fixedHeight;

            return new Rect(vector.x - fixedWidth / 2f, vector.y - fixedHeight / 2f, fixedWidth, fixedHeight);
        }

        public static void DrawRect(Rect rect, Vector3 position, Quaternion rotation, Color color, float rectAlpha, float outlineAlpha)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            Vector3[] corners = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                Vector3 point = GetLocalRectPoint(rect, i);
                corners[i] = rotation * point + position;
            }

            Vector3[] points = new Vector3[]
            {
                corners[0],
                corners[1],
                corners[2],
                corners[3],
                corners[0]
            };

            Color l_color = Handles.color;
            Handles.color = color;

            Vector2 offset = new Vector2(1f, 1f);

            if (!Camera.current)
            {
                offset.y *= -1;
            }

            Handles.DrawSolidRectangleWithOutline(points, new Color(1f, 1f, 1f, rectAlpha), new Color(1f, 1f, 1f, outlineAlpha));

            Handles.color = l_color;
        }

        private static Vector2 GetLocalRectPoint(Rect rect, int index)
        {
            switch (index)
            {
                case (0): return new Vector2(rect.xMin, rect.yMax);
                case (1): return new Vector2(rect.xMax, rect.yMax);
                case (2): return new Vector2(rect.xMax, rect.yMin);
                case (3): return new Vector2(rect.xMin, rect.yMin);
            }
            return Vector3.zero;
        }

        private static void SetDiscSectionPoints(Vector3[] dest, int count, Vector3 normal, Vector3 from, float angle)
        {
            from.Normalize();
            Quaternion rotation = Quaternion.AngleAxis(angle / (float)(count - 1), normal);
            Vector3 vector = from;
            for (int i = 0; i < count; i++)
            {
                dest[i] = vector;
                vector = rotation * vector;
            }
        }

        static Vector3[] s_array;
        public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, int numSamples = 60)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            numSamples = Mathf.Clamp(numSamples, 3, 60);

            if (s_array == null)
                s_array = new Vector3[60];

            Color color = Handles.color;
            SetDiscSectionPoints(s_array, numSamples, normal, from, angle);
            InternalEditorBridge.ApplyWireMaterial();
            GL.PushMatrix();
            GL.MultMatrix(Handles.matrix);
            GL.Begin(GL.TRIANGLES);
            for (int i = 1; i < numSamples; i++)
            {
                GL.Color(color);
                GL.Vertex(center);
                GL.Vertex(center + s_array[i - 1] * radius);
                GL.Vertex(center + s_array[i] * radius);
            }
            GL.End();
            GL.PopMatrix();
        }

        public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, float outlineScale, int numSamples = 60)
		{
			if (Event.current.type != EventType.Repaint)
				return;
			
            numSamples = Mathf.Clamp(numSamples, 3, 60);
			
			if(s_array == null)
				s_array = new Vector3[60];
				
            Color color = Handles.color;
            SetDiscSectionPoints(s_array, numSamples, normal, from, angle);
			InternalEditorBridge.ApplyWireMaterial();
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(4);
			for (int i = 1; i < numSamples; i++)
			{
				GL.Color(color);
				GL.Vertex(center + s_array[i - 1] * radius * outlineScale);
				GL.Vertex(center + s_array[i - 1] * radius);
				GL.Vertex(center + s_array[i] * radius);
				GL.Vertex(center + s_array[i - 1] * radius * outlineScale);
				GL.Vertex(center + s_array[i] * radius);
				GL.Vertex(center + s_array[i] * radius * outlineScale);
			}
			GL.End();
			GL.PopMatrix();
		}
    }
}
