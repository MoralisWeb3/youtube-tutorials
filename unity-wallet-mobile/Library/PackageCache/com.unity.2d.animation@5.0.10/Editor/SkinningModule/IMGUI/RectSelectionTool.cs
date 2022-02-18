using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class RectSelectionTool<T>
    {
        private int m_HashCode = "RectSelectionTool".GetHashCode();
        private int m_ControlID = -1;
        private bool m_Moved = false;
        private RectSlider m_RectSlider = new RectSlider();
        public int controlID { get { return m_ControlID; } }
        public IRectSelector<T> rectSelector { get; set; }
        public ICacheUndo cacheUndo { get; set; }
        public Action onSelectionStart = () => {};
        public Action onSelectionUpdate = () => {};
        public Action onSelectionEnd = () => {};

        public void OnGUI()
        {
            Debug.Assert(rectSelector != null);
            Debug.Assert(cacheUndo != null);

            m_ControlID = GUIUtility.GetControlID(m_HashCode, FocusType.Passive);

            Event ev = Event.current;
            EventType eventType = ev.GetTypeForControl(m_ControlID);

            if (GUIUtility.hotControl == 0 && HandleUtility.nearestControl == m_ControlID &&
                rectSelector.selection.Count > 0 && eventType == EventType.MouseDown && ev.button == 0 && !ev.alt)
            {
                m_Moved = false;
                onSelectionStart();
            }

            if (m_Moved && GUIUtility.hotControl == m_ControlID && eventType == EventType.MouseUp && ev.button == 0)
            {
                cacheUndo.BeginUndoOperation(TextContent.selection);
                rectSelector.selection.EndSelection(true);
                onSelectionEnd();
            }

            EditorGUI.BeginChangeCheck();

            rectSelector.rect = m_RectSlider.Do(m_ControlID);

            if (EditorGUI.EndChangeCheck())
            {
                if(!m_Moved)
                {
                    cacheUndo.BeginUndoOperation(TextContent.selection);

                    if(!ev.shift)
                        rectSelector.selection.Clear();
                        
                    m_Moved = true;
                }
                
                rectSelector.selection.BeginSelection();
                rectSelector.Select();
                onSelectionUpdate();
            }

            if (eventType == EventType.Repaint && GUIUtility.hotControl == m_ControlID)
            {
                DrawingUtility.DrawRect(rectSelector.rect, Vector3.zero, Quaternion.identity, new Color(0f, 1f, 1f, 1f), 0.05f, 0.8f);
            }
        }
    }
}
