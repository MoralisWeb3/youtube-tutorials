using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Sprites;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UnityEditor.U2D.Common
{
    internal class ImagePackerDebugEditor : EditorWindow
    {
        [MenuItem("internal:Window/2D/Common/Image Packer Debug Editor")]
        static void Launch()
        {
            var window = EditorWindow.GetWindow<ImagePackerDebugEditor>();
            var pos = window.position;
            pos.height = pos.width = 400;
            window.position = pos;
            window.Show();
        }

        ReorderableList m_ReorderableList;
        ImagePacker.ImagePackRect[] m_PackingRect = null;
        List<RectInt> m_PackRects = new List<RectInt>();
        RectInt[] m_PackResult = null;
        SpriteRect[] m_SpriteRects = null;
        Texture2D m_Texture;
        int m_TextureActualWidth = 0;
        int m_TextureActualHeight = 0;
        int m_PackWidth = 0;
        int m_PackHeight = 0;
        int m_Padding = 0;
        Vector2 m_ConfigScroll = Vector2.zero;
        float m_Zoom = 1;
        IMGUIContainer m_PackArea;
        int m_PackStep = -1;
        protected const float k_MinZoomPercentage = 0.9f;
        protected const float k_WheelZoomSpeed = 0.03f;
        protected const float k_MouseZoomSpeed = 0.005f;

        void OnEnable()
        {
            var visualContainer = new VisualElement()
            {
                name = "Container",
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row
                }
            };
            this.rootVisualElement.Add(visualContainer);

            var imgui = new IMGUIContainer(OnConfigGUI)
            {
                name = "Config",
                style =
                {
                    width = 300
                }
            };

            visualContainer.Add(imgui);

            m_PackArea = new IMGUIContainer(OnImagePackerGUI)
            {
                name = "ImagePacker",
                style =
                {
                    flexGrow = 1,
                }
            };
            visualContainer.Add(m_PackArea);
            SetupConfigGUI();
        }

        void SetupConfigGUI()
        {
            m_ReorderableList = new ReorderableList(m_PackRects, typeof(RectInt), false, false, true, true);
            m_ReorderableList.elementHeightCallback = (int index) =>
                {
                    return EditorGUIUtility.singleLineHeight * 2 + 6;
                };
            m_ReorderableList.drawElementCallback = DrawListElement;

            m_ReorderableList.onAddCallback = (list) =>
                {
                    m_PackRects.Add(new RectInt());
                };
            m_ReorderableList.onRemoveCallback = (list) =>
                {
                    m_PackRects.RemoveAt(list.index);
                };
        }

        void DrawListElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            var rectInt = m_PackRects[index];
            var name = m_SpriteRects == null || index >= m_SpriteRects.Length ? index.ToString() : m_SpriteRects[index].
                name;
            rectInt.size = EditorGUI.Vector2IntField(rect, name, rectInt.size);
            m_PackRects[index] = rectInt;
        }

        void OnConfigGUI()
        {
            EditorGUILayout.BeginVertical();
            m_ConfigScroll = EditorGUILayout.BeginScrollView(m_ConfigScroll);
            m_ReorderableList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();

            m_PackStep = EditorGUILayout.IntSlider("Step", m_PackStep, 0, m_PackRects.Count);
            EditorGUI.BeginChangeCheck();
            m_Texture = EditorGUILayout.ObjectField(new GUIContent("Texture"), (Object)m_Texture, typeof(Texture2D), false) as Texture2D;
            if (EditorGUI.EndChangeCheck())
                UpdateSpriteRect();
            m_Padding = EditorGUILayout.IntField("Padding", m_Padding);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("<<"))
            {
                m_PackStep = m_PackStep <= 0 ? 0 : m_PackStep - 1;
                Pack();
            }
            if (GUILayout.Button("Pack"))
                Pack();
            if (GUILayout.Button(">>"))
            {
                m_PackStep = m_PackStep > m_PackRects.Count ? m_PackRects.Count : m_PackStep + 1;
                Pack();
            }
            if (GUILayout.Button("Clear"))
            {
                m_PackRects.Clear();
                m_Texture = null;
                m_PackingRect = null;
                m_PackResult = null;
                m_SpriteRects = null;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        void UpdateSpriteRect()
        {
            var dataProvider = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(m_Texture)) as ISpriteEditorDataProvider;
            if (dataProvider == null)
                return;
            dataProvider.InitSpriteEditorDataProvider();
            dataProvider.GetDataProvider<ITextureDataProvider>().GetTextureActualWidthAndHeight(out m_TextureActualWidth, out m_TextureActualHeight);
            m_SpriteRects = dataProvider.GetDataProvider<ISpriteEditorDataProvider>().GetSpriteRects();
            m_PackRects.Clear();
            m_PackRects.AddRange(m_SpriteRects.Select(x => new RectInt((int)x.rect.x, (int)x.rect.y, (int)x.rect.width, (int)x.rect.height)));
            m_PackResult = null;
            m_PackStep = m_PackRects.Count;
        }

        void Pack()
        {
            int count = m_PackStep > 0 && m_PackStep < m_PackRects.Count ? m_PackStep : m_PackRects.Count;
            m_PackingRect = new ImagePacker.ImagePackRect[m_PackRects.Count];
            for (int i = 0; i < m_PackRects.Count; ++i)
            {
                m_PackingRect[i] = new ImagePacker.ImagePackRect()
                {
                    rect = m_PackRects[i],
                    index = i
                };
            }
            Array.Sort(m_PackingRect);
            ImagePacker.Pack(m_PackingRect.Take(count).Select(x => x.rect).ToArray(), m_Padding, out m_PackResult, out m_PackWidth, out m_PackHeight);
        }

        void DrawLabel(Rect rect, string label)
        {
            rect.position = Handles.matrix.MultiplyPoint(rect.position);
            GUI.Label(rect, label);
        }

        void OnImagePackerGUI()
        {
            if (m_PackResult == null)
                return;
            HandleZoom();
            var oldMatrix = Handles.matrix;
            SetupHandlesMatrix();
            Handles.DrawSolidRectangleWithOutline(new Rect(0, 0, m_PackWidth, m_PackHeight), Color.gray, Color.black);
            DrawLabel(new Rect(0, 0, m_PackWidth, m_PackHeight), m_PackWidth + "x" + m_PackHeight);

            int index = 0;

            foreach (var rect in m_PackResult)
            {
                Handles.DrawSolidRectangleWithOutline(new Rect(rect.x, rect.y, rect.width, rect.height), Color.white, Color.black);
                var rect1 = new Rect(rect.x, rect.y + rect.height * 0.5f, rect.width, EditorGUIUtility.singleLineHeight);
                DrawLabel(rect1, m_PackingRect[index].index.ToString());
                ++index;
            }

            index = 0;
            if (m_Texture != null && m_SpriteRects != null)
            {
                var material = new Material(Shader.Find("Sprites/Default"));
                material.mainTexture = m_Texture;
                material.SetPass(0);

                int mouseOverIndex = -1;
                GL.PushMatrix();
                GL.LoadIdentity();
                GL.MultMatrix(GUI.matrix * Handles.matrix);
                GL.Begin(GL.QUADS);
                for (int i = 0; i < m_PackResult.Length; ++i)
                {
                    index = m_PackingRect[i].index;
                    if (index >= m_SpriteRects.Length)
                        continue;
                    var rect = m_PackResult[i];
                    GL.TexCoord(new Vector3(m_SpriteRects[index].rect.x / m_TextureActualWidth, m_SpriteRects[index].rect.y / m_TextureActualHeight, 0));
                    GL.Vertex(new Vector3(rect.x, rect.y, 0));
                    GL.TexCoord(new Vector3(m_SpriteRects[index].rect.xMax / m_TextureActualWidth, m_SpriteRects[index].rect.y / m_TextureActualHeight, 0));
                    GL.Vertex(new Vector3(rect.x + rect.width, rect.y, 0));
                    GL.TexCoord(new Vector3(m_SpriteRects[index].rect.xMax / m_TextureActualWidth, m_SpriteRects[index].rect.yMax / m_TextureActualHeight, 0));
                    GL.Vertex(new Vector3(rect.x + rect.width, rect.y + rect.height, 0));
                    GL.TexCoord(new Vector3(m_SpriteRects[index].rect.x / m_TextureActualWidth, m_SpriteRects[index].rect.yMax / m_TextureActualHeight, 0));
                    GL.Vertex(new Vector3(rect.x, rect.y + rect.height, 0));
                    var m = Handles.matrix.inverse.MultiplyPoint(Event.current.mousePosition);
                    if (rect.Contains(new Vector2Int((int)m.x, (int)m.y)))
                    {
                        mouseOverIndex = index;
                    }
                    ++index;
                }

                GL.End();
                GL.PopMatrix();
                if (mouseOverIndex >= 0)
                {
                    var text = new GUIContent(m_SpriteRects[mouseOverIndex].name + " " + index);
                    var length = EditorStyles.textArea.CalcSize(text);
                    var rect1 = new Rect(m_PackResult[mouseOverIndex].x, m_PackResult[mouseOverIndex].y + m_PackResult[mouseOverIndex].height * 0.5f, length.x, length.y);
                    rect1.position = Handles.matrix.MultiplyPoint(rect1.position);
                    if (Event.current.type == EventType.Repaint)
                        EditorStyles.textArea.Draw(rect1, text, false, false, false, false);
                }
            }

            Handles.matrix = oldMatrix;
        }

        void SetupHandlesMatrix()
        {
            Vector3 handlesPos = new Vector3(0, m_PackHeight * m_Zoom, 0f);
            Vector3 handlesScale = new Vector3(m_Zoom, -m_Zoom, 1f);
            Handles.matrix = Matrix4x4.TRS(handlesPos, Quaternion.identity, handlesScale);
        }

        protected void HandleZoom()
        {
            bool zoomMode = Event.current.alt && Event.current.button == 1;
            if (zoomMode)
            {
                EditorGUIUtility.AddCursorRect(m_PackArea.worldBound, MouseCursor.Zoom);
            }

            if (
                ((Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown) && zoomMode) ||
                ((Event.current.type == EventType.KeyUp || Event.current.type == EventType.KeyDown) && Event.current.keyCode == KeyCode.LeftAlt)
                )
            {
                Repaint();
            }

            if (Event.current.type == EventType.ScrollWheel || (Event.current.type == EventType.MouseDrag && Event.current.alt && Event.current.button == 1))
            {
                float zoomMultiplier = 1f - Event.current.delta.y * (Event.current.type == EventType.ScrollWheel ? k_WheelZoomSpeed : -k_MouseZoomSpeed);

                // Clamp zoom
                float wantedZoom = m_Zoom * zoomMultiplier;

                float currentZoom = Mathf.Clamp(wantedZoom, GetMinZoom(), 1);

                if (currentZoom != m_Zoom)
                {
                    m_Zoom = currentZoom;
                    Event.current.Use();
                }
            }
        }

        protected float GetMinZoom()
        {
            if (m_Texture == null)
                return 1.0f;
            return Mathf.Min(m_PackArea.worldBound.width / m_PackWidth, m_PackArea.worldBound.height / m_PackHeight, 0.05f) * k_MinZoomPercentage;
        }
    }
}
