using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Path.GUIFramework;

namespace UnityEditor.U2D.Path
{
    public abstract class RectSelector<T> : ISelector<T>
    {
        public class Styles
        {
            public readonly GUIStyle selectionRectStyle;

            public Styles()
            {
                selectionRectStyle = GUI.skin.FindStyle("selectionRect");
            }
        }

        public Action<ISelector<T>, bool> onSelectionBegin;
        public Action<ISelector<T>> onSelectionChanged;
        public Action<ISelector<T>> onSelectionEnd;

        private GUISystem m_GUISystem;
        private Control m_RectSelectorControl;
        private GUIAction m_RectSelectAction;
        private Rect m_GUIRect;
        private Styles m_Styles;

        public Rect guiRect
        {
            get { return m_GUIRect; }
        }

        private Styles styles
        {
            get
            {
                if (m_Styles == null)
                    m_Styles = new Styles();

                return m_Styles;
            }
        }

        public RectSelector() : this(new GUISystem(new GUIState())) { }
        
        public RectSelector(GUISystem guiSystem)
        {
            m_GUISystem = guiSystem;

            m_RectSelectorControl = new GenericDefaultControl("RectSelector");

            var start = Vector2.zero;
            var rectEnd = Vector2.zero;
            m_RectSelectAction = new SliderAction(m_RectSelectorControl)
            {
                enable = (guiState, action) => !IsAltDown(guiState),
                enableRepaint = (guiState, action) =>
                {
                    var size = start - rectEnd;
                    return size != Vector2.zero && guiState.hotControl == action.ID;
                },
                onSliderBegin = (guiState, control, position) =>
                {
                    start = guiState.mousePosition;
                    rectEnd = guiState.mousePosition;
                    m_GUIRect = FromToRect(start, rectEnd);

                    if (onSelectionBegin != null)
                        onSelectionBegin(this, guiState.isShiftDown);
                },
                onSliderChanged = (guiState, control, position) =>
                {
                    rectEnd = guiState.mousePosition;
                    m_GUIRect = FromToRect(start, rectEnd);

                    if (onSelectionChanged != null)
                        onSelectionChanged(this);
                },
                onSliderEnd = (guiState, control, position) =>
                {
                    if (onSelectionEnd != null)
                        onSelectionEnd(this);
                },
                onRepaint = (guiState, action) =>
                {
                    Handles.BeginGUI();
                    styles.selectionRectStyle.Draw(m_GUIRect, GUIContent.none, false, false, false, false);
                    Handles.EndGUI();
                }
            };

            m_GUISystem.AddControl(m_RectSelectorControl);
            m_GUISystem.AddAction(m_RectSelectAction);
        }

        private bool IsAltDown(IGUIState guiState)
        {
            return guiState.hotControl == 0 && guiState.isAltDown;
        }

        private Rect FromToRect(Vector2 start, Vector2 end)
        {
            Rect r = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
            if (r.width < 0)
            {
                r.x += r.width;
                r.width = -r.width;
            }
            if (r.height < 0)
            {
                r.y += r.height;
                r.height = -r.height;
            }
            return r;
        }

        public void OnGUI()
        {
            m_GUISystem.OnGUI();
        }

        bool ISelector<T>.Select(T element)
        {
            return Select(element);
        }

        protected abstract bool Select(T element);
    }
}
