using System;
using UnityEngine;

namespace UnityEditor.U2D
{
    internal class SpriteSelector
    {
        int m_SelectedSprite = 0;
        Sprite[] m_SpriteList = null;
        Texture[] m_Thumbnails;
        Vector2 m_ScrollPos;

        public int selectedIndex 
        {
            get { return m_SelectedSprite; } 
            set {  m_SelectedSprite = value; }
        }
        internal static class Styles
        {
            public static GUIStyle gridList = "GridList";
            public static GUIContent spriteList = EditorGUIUtility.TrTextContent("Sprite Variant");
            public static GUIContent missingSprites = EditorGUIUtility.TrTextContent("No brushes defined.");
            public static GUIStyle localGrid = null;
        }
        public SpriteSelector()
        {
            m_SpriteList = null;
        }

        public void UpdateSprites(Sprite[] sprites)
        {
            m_SpriteList = sprites;
            UpdateSelection(0);
        }

        public void UpdateSelection(int newSelectedBrush)
        {
            m_SelectedSprite = newSelectedBrush;
        }

        private static int CalcTotalHorizSpacing(int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle)
        {
            if (xCount < 2)
                return 0;
            if (xCount == 2)
                return Mathf.Max(firstStyle.margin.right, lastStyle.margin.left);

            int internalSpace = Mathf.Max(midStyle.margin.left, midStyle.margin.right);
            return Mathf.Max(firstStyle.margin.right, midStyle.margin.left) + Mathf.Max(midStyle.margin.right, lastStyle.margin.left) + internalSpace * (xCount - 3);
        }

        // Helper function: Get all mouse rects
        private static Rect[] CalcMouseRects(Rect position, GUIContent[] contents, int xCount, float elemWidth, float elemHeight, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, bool addBorders, GUI.ToolbarButtonSize buttonSize)
        {
            int count = contents.Length;
            int x = 0;
            float xPos = position.xMin, yPos = position.yMin;
            GUIStyle currentStyle = style;
            Rect[] retval = new Rect[count];
            if (count > 1)
                currentStyle = firstStyle;
            for (int i = 0; i < count; i++)
            {
                float w = 0;
                switch (buttonSize)
                {
                    case GUI.ToolbarButtonSize.Fixed:
                        w = elemWidth;
                        break;
                    case GUI.ToolbarButtonSize.FitToContents:
                        w = currentStyle.CalcSize(contents[i]).x;
                        break;
                }

                if (!addBorders)
                    retval[i] = new Rect(xPos, yPos, w, elemHeight);
                else
                    retval[i] = currentStyle.margin.Add(new Rect(xPos, yPos, w, elemHeight));

                //we round the values to the dpi-aware pixel grid
                retval[i] = GUIUtility.AlignRectToDevice(retval[i]);

                GUIStyle nextStyle = midStyle;
                if (i == count - 2 || i == xCount - 2)
                    nextStyle = lastStyle;

                xPos = retval[i].xMax + Mathf.Max(currentStyle.margin.right, nextStyle.margin.left);

                x++;
                if (x >= xCount)
                {
                    x = 0;
                    yPos += elemHeight + Mathf.Max(style.margin.top, style.margin.bottom);
                    xPos = position.xMin;
                    nextStyle = firstStyle;
                }

                currentStyle = nextStyle;
            }
            return retval;
        }

        static void DrawRectangleOutline(Rect rect, Color color)
        {
            Color currentColor = Handles.color;
            Handles.color = color;

            // Draw viewport outline
            Vector3[] points = new Vector3[5];
            points[0] = new Vector3(rect.x, rect.y, 0.0f);
            points[1] = new Vector3(rect.x + rect.width, rect.y, 0.0f);
            points[2] = new Vector3(rect.x + rect.width, rect.y + rect.height, 0.0f);
            points[3] = new Vector3(rect.x, rect.y + rect.height, 0.0f);
            points[4] = new Vector3(rect.x, rect.y, 0.0f);
            Handles.DrawPolyLine(points);

            Handles.color = currentColor;
        }

        // Make a button grid
        private static int DoButtonGrid(Rect position, int selected, GUIContent[] contents, string[] controlNames, int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, GUI.ToolbarButtonSize buttonSize, bool[] contentsEnabled = null)
        {
            int count = contents.Length;
            if (count == 0)
                return selected;
            if (xCount <= 0)
            {
                Debug.LogWarning("You are trying to create a SelectionGrid with zero or less elements to be displayed in the horizontal direction. Set xCount to a positive value.");
                return selected;
            }

            if (contentsEnabled != null && contentsEnabled.Length != count)
                throw new ArgumentException("contentsEnabled");

            // Figure out how large each element should be
            int rows = count / xCount;
            if (count % xCount != 0)
                rows++;

            float totalHorizSpacing = CalcTotalHorizSpacing(xCount, style, firstStyle, midStyle, lastStyle);
            float totalVerticalSpacing = Mathf.Max(style.margin.top, style.margin.bottom) * (rows - 1);
            float elemWidth = (position.width - totalHorizSpacing) / xCount;
            float elemHeight = (position.height - totalVerticalSpacing) / rows;

            if (style.fixedWidth != 0)
                elemWidth = style.fixedWidth;
            if (style.fixedHeight != 0)
                elemHeight = style.fixedHeight;

            Rect[] buttonRects = CalcMouseRects(position, contents, xCount, elemWidth, elemHeight, style, firstStyle, midStyle, lastStyle, false, buttonSize);
            GUIStyle selectedButtonStyle = null;
            int selectedButtonID = 0;
            for (int buttonIndex = 0; buttonIndex < count; ++buttonIndex)
            {
                bool wasEnabled = GUI.enabled;
                GUI.enabled &= (contentsEnabled == null || contentsEnabled[buttonIndex]);
                var buttonRect = buttonRects[buttonIndex];
                var content = contents[buttonIndex];

                if (controlNames != null)
                    GUI.SetNextControlName(controlNames[buttonIndex]);
                var id = GUIUtility.GetControlID("ButtonGrid".GetHashCode(), FocusType.Passive, buttonRect);
                if (buttonIndex == selected)
                    selectedButtonID = id;

                switch (Event.current.GetTypeForControl(id))
                {
                    case EventType.MouseDown:
                        if (buttonRect.Contains(Event.current.mousePosition))
                        {
                            GUIUtility.hotControl = id;
                            Event.current.Use();
                        }
                        break;
                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == id)
                            Event.current.Use();
                        break;
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == id)
                        {
                            GUIUtility.hotControl = 0;
                            Event.current.Use();

                            GUI.changed = true;
                            return buttonIndex;
                        }
                        break;
                    case EventType.Repaint:
                        var buttonStyle = count == 1 ? style : (buttonIndex == 0 ? firstStyle : (buttonIndex == count - 1 ? lastStyle : midStyle));
                        var isSelected = selected == buttonIndex;

                        if (!isSelected)
                        {
                            GUI.DrawTexture(buttonRect, content.image, ScaleMode.ScaleToFit, true);
                            GUI.Label(new Rect(buttonRect.x, buttonRect.y, 32, 32), buttonIndex.ToString());
                        }
                        else
                            selectedButtonStyle = buttonStyle;
                        break;
                }

                GUI.enabled = wasEnabled;
            }

            // draw selected button at the end so it overflows nicer
            if (selectedButtonStyle != null)
            {
                var buttonRect = buttonRects[selected];
                var content = contents[selected];
                var wasEnabled = GUI.enabled;
                GUI.enabled &= (contentsEnabled == null || contentsEnabled[selected]);

                GUI.DrawTexture(new Rect(buttonRect.x + 4, buttonRect.y + 4, buttonRect.width - 8, buttonRect.height - 8), content.image, ScaleMode.ScaleToFit, true);
                DrawRectangleOutline(buttonRect, GUI.skin.settings.selectionColor);
                GUI.Label(new Rect(buttonRect.x, buttonRect.y, 32, 32), selected.ToString());
                GUI.enabled = wasEnabled;
            }

            return selected;
        }

        // Get Temp Texture Contents for Sprites.
        private static GUIContent[] Temp(Texture[] images)
        {
            GUIContent[] retval = new GUIContent[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                retval[i] = new GUIContent(images[i]);
            }
            return retval;
        }

        public Sprite GetActiveSprite()
        {
            if (m_SelectedSprite >= m_SpriteList.Length)
                m_SelectedSprite = 0;
            return m_SpriteList[m_SelectedSprite];
        }

        public bool ShowGUI(int selectedIndex)
        {
            bool repaint = false;
            if (m_SpriteList == null || m_SpriteList.Length == 0)
                return false;
            int approxSize = 64;
            int approxHolderSize = 66;

            if (Styles.localGrid == null)
            {
                Styles.localGrid = new GUIStyle(Styles.gridList);
                Styles.localGrid.fixedWidth = approxSize;
                Styles.localGrid.fixedHeight = approxSize;
            }
            m_SelectedSprite = (selectedIndex > m_SpriteList.Length) ? 0 : selectedIndex;

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label(Styles.spriteList, EditorStyles.label, GUILayout.Width(EditorGUIUtility.labelWidth - 5));

                int cviewwidth = (int)(EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - approxSize);
                int columns = (int)(cviewwidth) / approxSize;
                columns = columns == 0 ? 1 : columns;
                int rows = (int)Mathf.Ceil((m_SpriteList.Length + columns - 1) / columns);
                int lyColumns = (rows == 1) ? (approxHolderSize): (approxHolderSize * 2);

                GUILayout.BeginVertical("box", new GUILayoutOption[] { GUILayout.Height(lyColumns) } );
                {
                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, new GUILayoutOption[] { GUILayout.Height(lyColumns) });
                    int newBrush = SpriteSelectionGrid(m_SelectedSprite, m_SpriteList, approxSize, Styles.localGrid, Styles.missingSprites, columns, rows);
                    if (newBrush != m_SelectedSprite)
                    {
                        UpdateSelection(newBrush);
                        repaint = true;
                    }
                    EditorGUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            return repaint;
        }

        int SpriteSelectionGrid(int selected, Sprite[] sprites, int approxSize, GUIStyle style, GUIContent emptyString, int columns, int rows)
        {            
            int retval = 0;
            if (sprites.Length != 0)
            {
                Rect r = GUILayoutUtility.GetRect((float)columns * approxSize, (float)rows * approxSize);
                Event evt = Event.current;
                if (evt.type == EventType.MouseDown && evt.clickCount == 2 && r.Contains(evt.mousePosition))
                    evt.Use();

                m_Thumbnails = PreviewTexturesFromSprites(sprites);
                retval = DoButtonGrid(r, selected, Temp(m_Thumbnails), null, (int)columns, style, style, style, style, GUI.ToolbarButtonSize.FitToContents);
            }
            else
                GUILayout.Label(emptyString);
            return retval;
        }


        internal static Texture[] PreviewTexturesFromSprites(Sprite[] sprites)
        {
            Texture[] retval = new Texture[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
                retval[i] = AssetPreview.GetAssetPreview(sprites[i]) ?? Texture2D.whiteTexture;
            return retval;
        }
    }
} //namespace
