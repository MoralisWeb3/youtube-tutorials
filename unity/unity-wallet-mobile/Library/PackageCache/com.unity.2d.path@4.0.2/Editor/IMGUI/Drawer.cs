using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Path.GUIFramework;

namespace UnityEditor.U2D.Path
{
    public class Drawer : IDrawer
    {
        internal class Styles
        {
            public readonly GUIStyle pointNormalStyle;
            public readonly GUIStyle pointHoveredStyle;
            public readonly GUIStyle pointSelectedStyle;
            public readonly GUIStyle pointPreviewStyle;
            public readonly GUIStyle tangentNormalStyle;
            public readonly GUIStyle tangentHoveredStyle;

            public Styles()
            {
                var pointNormal = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/pointNormal.png");
                var pointHovered = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/pointHovered.png");
                var pointSelected = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/pointSelected.png");
                var pointPreview = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/pointPreview.png");
                var tangentNormal = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/tangentNormal.png");

                pointNormalStyle = CreateStyle(AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/pointNormal.png"), Vector2.one * 12f);
                pointHoveredStyle = CreateStyle(AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/pointHovered.png"), Vector2.one * 12f);
                pointSelectedStyle = CreateStyle(AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/pointSelected.png"), Vector2.one * 12f);
                pointPreviewStyle = CreateStyle(AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/pointPreview.png"), Vector2.one * 12f);
                tangentNormalStyle = CreateStyle(AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/tangentNormal.png"), Vector2.one * 8f);
                tangentHoveredStyle = CreateStyle(AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.2d.path/Editor/Handles/Path/pointHovered.png"), Vector2.one * 10f);
            }

            private GUIStyle CreateStyle(Texture2D texture, Vector2 size)
            {
                var guiStyle = new GUIStyle();
                guiStyle.normal.background = texture;
                guiStyle.fixedWidth = size.x;
                guiStyle.fixedHeight = size.y;

                return guiStyle;
            }
        }

        private IGUIState m_GUIState = new GUIState();
        private Styles m_Styles;
        private Styles styles
        {
            get
            {
                if (m_Styles == null)
                    m_Styles = new Styles();

                return m_Styles;
            }
        }

        public void DrawCreatePointPreview(Vector3 position)
        {
            DrawGUIStyleCap(0, position, Quaternion.identity, m_GUIState.GetHandleSize(position), styles.pointPreviewStyle);
        }

        public void DrawPoint(Vector3 position)
        {
            DrawGUIStyleCap(0, position, Quaternion.identity, m_GUIState.GetHandleSize(position), styles.pointNormalStyle);
        }

        public void DrawPointHovered(Vector3 position)
        {
            DrawGUIStyleCap(0, position, Quaternion.identity, m_GUIState.GetHandleSize(position), styles.pointHoveredStyle);
        }

        public void DrawPointSelected(Vector3 position)
        {
            DrawGUIStyleCap(0, position, Quaternion.identity, m_GUIState.GetHandleSize(position), styles.pointSelectedStyle);
        }

        public void DrawLine(Vector3 p1, Vector3 p2, float width, Color color)
        {
            Handles.color = color;
            Handles.DrawAAPolyLine(width, new Vector3[] { p1, p2 });
        }

        public void DrawBezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float width, Color color)
        {
            Handles.color = color;
            Handles.DrawBezier(p1, p4, p2, p3, color, null, width);
        }

        public void DrawTangent(Vector3 position, Vector3 tangent)
        {
            DrawLine(position, tangent, 3f, Color.yellow);
            DrawGUIStyleCap(0, tangent, Quaternion.identity, m_GUIState.GetHandleSize(tangent), styles.tangentNormalStyle);
        }

        
        private void DrawGUIStyleCap(int controlID, Vector3 position, Quaternion rotation, float size, GUIStyle guiStyle)
        {
            if (Camera.current && Vector3.Dot(position - Camera.current.transform.position, Camera.current.transform.forward) < 0f)
                return;

            Handles.BeginGUI();
            guiStyle.Draw(GetGUIStyleRect(guiStyle, position), GUIContent.none, controlID);
            Handles.EndGUI();
        }

        private Rect GetGUIStyleRect(GUIStyle style, Vector3 position)
        {
            Vector2 vector = HandleUtility.WorldToGUIPoint(position);

            float fixedWidth = style.fixedWidth;
            float fixedHeight = style.fixedHeight;

            return new Rect(vector.x - fixedWidth / 2f, vector.y - fixedHeight / 2f, fixedWidth, fixedHeight);
        }
    }
}
