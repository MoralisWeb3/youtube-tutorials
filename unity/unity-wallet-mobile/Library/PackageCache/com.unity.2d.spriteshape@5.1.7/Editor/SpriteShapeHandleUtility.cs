using UnityEditor;
using UnityEngine;
using UnityEditor.Sprites;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.U2D
{
    public class SpriteShapeHandleUtility
    {
        private class Styles
        {
            public Texture playheadTex;
            public Texture handRightTex;
            public Texture handLeftTex;
        }

        private static Styles s_Styles;
        private static Styles styles
        {
            get
            {
                if (s_Styles == null)
                    s_Styles = new Styles();

                return s_Styles;
            }
        }

        static private Material s_HandleWireMaterial;
        private static Material handleWireMaterial
        {
            get
            {
                if (!s_HandleWireMaterial)
                    s_HandleWireMaterial = (Material)EditorGUIUtility.LoadRequired("SceneView/2DHandleLines.mat");

                return s_HandleWireMaterial;
            }
        }

        private static Material s_FillTextureMaterial;
        private static Material fillTextureMaterial
        {
            get
            {
                if (s_FillTextureMaterial == null)
                {
                    s_FillTextureMaterial = new Material(Shader.Find("Hidden/InternalSpritesInspector"));
                    s_FillTextureMaterial.hideFlags = HideFlags.DontSave;
                }
                s_FillTextureMaterial.SetFloat("_AdjustLinearForGamma", PlayerSettings.colorSpace == ColorSpace.Linear ? 1.0f : 0.0f);
                return s_FillTextureMaterial;
            }
        }

        private static Mesh s_TextureCapMesh;
        private static Mesh textureCapMesh
        {
            get
            {
                if (s_TextureCapMesh == null)
                {
                    s_TextureCapMesh = new Mesh();
                    s_TextureCapMesh.hideFlags = HideFlags.DontSave;
                    s_TextureCapMesh.vertices = new Vector3[] {
                        new Vector2(-0.5f, -0.5f),
                        new Vector2(-0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(-0.5f, -0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, -0.5f)
                    };
                    s_TextureCapMesh.uv = new Vector2[] {
                        Vector3.zero,
                        Vector3.up,
                        Vector3.up + Vector3.right,
                        Vector3.zero,
                        Vector3.up + Vector3.right,
                        Vector3.right
                    };
                    s_TextureCapMesh.SetTriangles(new int[] { 0, 1, 2, 3, 4, 5 }, 0);
                }

                return s_TextureCapMesh;
            }
        }

        private static readonly Vector3[] s_WireArcPoints = new Vector3[60];

        public static float PosToAngle(Vector2 position, Vector2 center, float angleOffset)
        {
            Vector2 dir = (Quaternion.AngleAxis(angleOffset, Vector3.forward) * (position - center)).normalized;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        public static Vector2 Slider2D(int id, Vector2 position, Vector3 capOffset, Quaternion rotation, float size, Handles.CapFunction drawCapFunction)
        {
            return Handles.Slider2D(id, position, capOffset, Vector3.forward, rotation * Vector3.up, rotation * Vector3.right, size, drawCapFunction, Vector2.zero);
        }

        public static void DrawRangeOutline(float start, float end, float angleOffset, Vector2 center, float radius, float width)
        {
            Vector3 startVec = Quaternion.AngleAxis(start + angleOffset, Vector3.forward) * Vector3.right;
            Vector3 endVec = Quaternion.AngleAxis(end + angleOffset, Vector3.forward) * Vector3.right;

            Handles.DrawWireArc(center, Vector3.forward, startVec, end - start, radius - width);
            Handles.DrawWireArc(center, Vector3.forward, startVec, end - start, radius);
            Handles.DrawLine(startVec * (radius - width) + (Vector3)center, startVec * radius + (Vector3)center);
            Handles.DrawLine(endVec * (radius - width) + (Vector3)center, endVec * radius + (Vector3)center);
        }

        private static void ApplyWireMaterial()
        {
            UnityEngine.Rendering.CompareFunction zTest = UnityEngine.Rendering.CompareFunction.Always;
            ApplyWireMaterial(zTest);
        }

        private static void ApplyWireMaterial(UnityEngine.Rendering.CompareFunction zTest)
        {
            Material mat = handleWireMaterial;
            mat.SetInt("_HandleZTest", (int)zTest);
            mat.SetPass(0);
        }

        static void SetDiscSectionPoints(Vector3[] dest, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            Vector3 fromn = from.normalized;
            Quaternion r = Quaternion.AngleAxis(angle / (float)(dest.Length - 1), normal);
            Vector3 tangent = fromn * radius;
            for (int i = 0; i < dest.Length; i++)
            {
                dest[i] = center + tangent;
                tangent = r * tangent;
            }
        }

        public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, float width)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            SetDiscSectionPoints(s_WireArcPoints, center, normal, from, angle, radius);

            Shader.SetGlobalColor("_HandleColor", Handles.color);
            Shader.SetGlobalFloat("_HandleSize", 1);

            ApplyWireMaterial(Handles.zTest);

            float widthPercentage = 1f - Mathf.Clamp01(width / radius);
            // Draw it twice to ensure backface culling doesn't hide any of the faces
            GL.PushMatrix();
            GL.MultMatrix(Handles.matrix);
            GL.Begin(GL.TRIANGLES);
            for (int i = 1, count = s_WireArcPoints.Length; i < count; ++i)
            {
                Vector3 d1 = s_WireArcPoints[i - 1] - center;
                Vector3 d2 = s_WireArcPoints[i] - center;
                GL.Color(Handles.color);
                GL.Vertex(d1 * widthPercentage + center);
                GL.Vertex(s_WireArcPoints[i - 1]);
                GL.Vertex(s_WireArcPoints[i]);
                GL.Vertex(d1 * widthPercentage + center);
                GL.Vertex(s_WireArcPoints[i]);
                GL.Vertex(d2 * widthPercentage + center);

                GL.Vertex(d1 * widthPercentage + center);
                GL.Vertex(s_WireArcPoints[i]);
                GL.Vertex(s_WireArcPoints[i - 1]);
                GL.Vertex(d1 * widthPercentage + center);
                GL.Vertex(d2 * widthPercentage + center);
                GL.Vertex(s_WireArcPoints[i]);
            }
            GL.End();
            GL.PopMatrix();
        }

        public static void DrawTextureArc(Texture texture, float pixelsPerRadius, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            if (Event.current.type != EventType.Repaint || !texture)
                return;

            SetDiscSectionPoints(s_WireArcPoints, Vector3.zero, normal, from, angle, 0.5f);

            fillTextureMaterial.mainTexture = texture;
            fillTextureMaterial.mainTextureScale = new Vector2(1f, -1f);
            fillTextureMaterial.mainTextureOffset = Vector2.zero;
            fillTextureMaterial.SetPass(0);

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(center, Quaternion.identity, new Vector3(radius, radius, 1) * 2f);
            Vector3 texOffset = Vector2.one * 0.5f;
            float scale = pixelsPerRadius / radius;

            GL.PushMatrix();
            GL.LoadPixelMatrix();
            GL.MultMatrix(matrix);
            GL.Begin(GL.TRIANGLES);
            for (int i = 1, count = s_WireArcPoints.Length; i < count; ++i)
            {
                GL.Color(Handles.color);
                GL.TexCoord(texOffset);
                GL.Vertex(Vector3.zero);
                GL.TexCoord(s_WireArcPoints[i - 1] * scale + texOffset);
                GL.Vertex(s_WireArcPoints[i - 1]);
                GL.TexCoord(s_WireArcPoints[i] * scale + texOffset);
                GL.Vertex(s_WireArcPoints[i]);
                GL.TexCoord(texOffset);
                GL.Vertex(Vector3.zero);
                GL.TexCoord(s_WireArcPoints[i] * scale + texOffset);
                GL.Vertex(s_WireArcPoints[i]);
                GL.TexCoord(s_WireArcPoints[i - 1] * scale + texOffset);
                GL.Vertex(s_WireArcPoints[i - 1]);
            }
            GL.End();
            GL.PopMatrix();
        }

        public static void PlayHeadCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (styles.playheadTex == null)
                styles.playheadTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.spriteshape/Editor/Handles/ss_playhead.png");
            GUITextureCap(controlID, styles.playheadTex, position, rotation, size, eventType);
        }

        public static void RangeLeftCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (styles.handLeftTex == null)
                styles.handLeftTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.spriteshape/Editor/Handles/ss_leftrange.png");
            GUITextureCap(controlID, styles.handLeftTex, position, rotation, size, eventType);
        }

        public static void RangeRightCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            if (styles.handRightTex == null)
                styles.handRightTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.spriteshape/Editor/Handles/ss_rightrange.png");
            GUITextureCap(controlID, styles.handRightTex, position, rotation, size, eventType);
        }

        public static void GUITextureCap(int controlID, Texture texture, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            switch (eventType)
            {
                case (EventType.Layout):
                    HandleUtility.AddControl(controlID, DistanceToRectangle(position, rotation, Vector2.one * size * 0.5f));
                    break;
                case (EventType.Repaint):

                    FilterMode filterMode = texture.filterMode;
                    texture.filterMode = FilterMode.Bilinear;

                    EditorSpriteGUIUtility.spriteMaterial.mainTexture = texture;

                    float w = (float)texture.width;
                    float h = (float)texture.height;
                    float max = Mathf.Max(w, h);

                    Vector3 scale = new Vector2(w / max, h / max) * size;

                    if (Camera.current == null)
                        scale.y *= -1f;

                    EditorSpriteGUIUtility.DrawMesh(textureCapMesh, EditorSpriteGUIUtility.spriteMaterial, position, rotation, scale);

                    texture.filterMode = filterMode;
                    break;
            }
        }

        public static float DistanceToArcWidth(Vector2 position, Vector2 center, float start, float end, float radius, float width, float angleOffet)
        {
            float innerRadius = radius - width;
            float angle = PosToAngle(position, center, -angleOffet);

            angle = Mathf.Repeat(angle - start, 360f);
            float range = end - start;

            if (angle >= 0f && angle <= range)
            {
                float distanceToCenter = (position - center).magnitude;

                if (distanceToCenter <= radius && distanceToCenter >= innerRadius)
                    return 0f;
                else if (distanceToCenter > radius)
                    return distanceToCenter - radius;
                else if (distanceToCenter < innerRadius)
                    return innerRadius - distanceToCenter;
            }
            else if (angle < 0f)
            {
                Vector2 pos1 = (Vector2)(Quaternion.AngleAxis(start + angleOffet, Vector3.forward) * Vector3.right * radius) + center;
                Vector2 pos2 = (Vector2)(Quaternion.AngleAxis(start + angleOffet, Vector3.forward) * Vector3.right * innerRadius) + center;

                return Mathf.Min((position - pos1).magnitude, (position - pos2).magnitude);
            }
            else if (angle > range)
            {
                Vector2 pos1 = (Vector2)(Quaternion.AngleAxis(end + angleOffet, Vector3.forward) * Vector3.right * radius) + center;
                Vector2 pos2 = (Vector2)(Quaternion.AngleAxis(end + angleOffet, Vector3.forward) * Vector3.right * innerRadius) + center;

                return Mathf.Min((position - pos1).magnitude, (position - pos2).magnitude);
            }

            return float.MaxValue;
        }

        public static float DistanceToRectangle(Vector3 position, Quaternion rotation, Vector2 size)
        {
            Vector3[] points = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
            Vector3 sideways = rotation * new Vector3(size.x, 0, 0);
            Vector3 up = rotation * new Vector3(0, size.y, 0);
            points[0] = HandleUtility.WorldToGUIPoint(position + sideways + up);
            points[1] = HandleUtility.WorldToGUIPoint(position + sideways - up);
            points[2] = HandleUtility.WorldToGUIPoint(position - sideways - up);
            points[3] = HandleUtility.WorldToGUIPoint(position - sideways + up);
            points[4] = points[0];

            Vector2 pos = Event.current.mousePosition;
            bool oddNodes = false;
            int j = 4;
            for (int i = 0; i < 5; i++)
            {
                if ((points[i].y > pos.y) != (points[j].y > pos.y))
                {
                    if (pos.x < (points[j].x - points[i].x) * (pos.y - points[i].y) / (points[j].y - points[i].y) + points[i].x)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }
            if (!oddNodes)
            {
                // Distance to closest edge (not so fast)
                float dist, closestDist = -1f;
                j = 1;
                for (int i = 0; i < 4; i++)
                {
                    dist = HandleUtility.DistancePointToLineSegment(pos, points[i], points[j++]);
                    if (dist < closestDist || closestDist < 0)
                        closestDist = dist;
                }
                return closestDist;
            }
            else
                return 0;
        }
    }
}
