using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Layout
{
    internal class VisibilityToolResizer : MouseManipulator
    {
        private Vector2 m_Start;
        private VisualElement m_Root;
        protected bool m_Active;
        private Rect m_StartPos;

        public VisibilityToolResizer()
        {
            activators.Add(new ManipulatorActivationFilter {button = MouseButton.LeftMouse});
            m_Active = false;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected void OnMouseDown(MouseDownEvent e)
        {
            if (m_Active)
            {
                e.StopImmediatePropagation();
                return;
            }


            if (CanStartManipulation(e))
            {
                m_Root = target;
                while (m_Root.parent != null)
                    m_Root = m_Root.parent;
                m_Start = target.ChangeCoordinatesTo(m_Root, e.localMousePosition);
                m_StartPos = target.parent.layout;
                m_Active = true;
                target.CaptureMouse();
                e.StopPropagation();
            }
        }

        protected void OnMouseMove(MouseMoveEvent e)
        {
            if (m_Active)
            {
                var ce = target.parent;
                Vector2 diff = target.ChangeCoordinatesTo(m_Root, e.localMousePosition) - m_Start;
                var newSize = new Vector2(m_StartPos.width - diff.x, m_StartPos.height - diff.y);
                float minWidth = ce.resolvedStyle.minWidth == StyleKeyword.Auto ? 0 : ce.resolvedStyle.minWidth.value;
                float minHeight = ce.resolvedStyle.minHeight == StyleKeyword.Auto ? 0 : ce.resolvedStyle.minHeight.value;
                float maxWidth = ce.resolvedStyle.maxWidth == StyleKeyword.None ? float.MaxValue : ce.resolvedStyle.maxWidth.value;
                float maxHeight = ce.resolvedStyle.maxHeight == StyleKeyword.None ? float.MaxValue : ce.resolvedStyle.maxHeight.value;

                newSize.x = (newSize.x < minWidth) ? minWidth : ((newSize.x > maxWidth) ? maxWidth : newSize.x);
                newSize.y = (newSize.y < minHeight) ? minHeight : ((newSize.y > maxHeight) ? maxHeight : newSize.y);
                ce.style.width = newSize.x;
                e.StopPropagation();
            }
        }

        protected void OnMouseUp(MouseUpEvent e)
        {
            if (m_Active)
            {
                if (CanStopManipulation(e))
                {
                    m_Active = false;
                    target.ReleaseMouse();
                    e.StopPropagation();
                }
            }
        }
    }
}
