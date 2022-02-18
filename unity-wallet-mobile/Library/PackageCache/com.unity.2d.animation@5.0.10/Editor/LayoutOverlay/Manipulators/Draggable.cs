using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Layout
{
    internal interface IDraggable
    {
        bool IsMovableNow();
        void UpdatePresenterPosition();
    }

    internal class Draggable : MouseManipulator
    {
        private Vector2 m_Start;
        protected bool m_Active;

        public Vector2 panSpeed { get; set; }
        public bool clampToParentEdges { get; set; }

        public Draggable(bool clampToParentEdges = false)
        {
            activators.Add(new ManipulatorActivationFilter {button = MouseButton.LeftMouse});
            panSpeed = Vector2.one;
            this.clampToParentEdges = clampToParentEdges;
            m_Active = false;
        }

        protected Rect CalculatePosition(float x, float y, float width, float height)
        {
            var rect = new Rect(x, y, width, height);

            if (clampToParentEdges)
            {
                if (rect.x < 0f)
                    rect.x = 0f;
                else if (rect.xMax > target.parent.layout.width)
                    rect.x = target.parent.layout.width - rect.width;

                if (rect.y < 0f)
                    rect.y = 0f;
                else if (rect.yMax > target.parent.layout.height)
                    rect.y = target.parent.layout.height - rect.height;

                // Reset size, we never intended to change them in the first place
                rect.width = width;
                rect.height = height;
            }

            return rect;
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

            /*
            IDraggable ce = e.target as IDraggable;
            if (ce == null || !ce.IsMovableNow())
            {
                return;
            }
            */

            if (CanStartManipulation(e))
            {
                m_Start = e.localMousePosition;

                m_Active = true;
                target.CaptureMouse();
                e.StopPropagation();
            }
        }

        protected void OnMouseMove(MouseMoveEvent e)
        {
            /*
            IDraggable ce = e.target as IDraggable;
            if (ce == null || !ce.IsMovableNow())
            {
                return;
            }
            */

            if (m_Active)
            {
                Vector2 diff = e.localMousePosition - m_Start;
                Rect rect = CalculatePosition(target.layout.x + diff.x, target.layout.y + diff.y, target.layout.width, target.layout.height);

                if (target.style.position == Position.Relative)
                {
                    target.style.left = rect.xMin;
                    target.style.top = rect.yMin;
                    target.style.right = rect.xMax;
                    target.style.bottom = rect.yMax;
                }
                else if (target.style.position == Position.Absolute)
                {
                    target.style.left = rect.x;
                    target.style.top = rect.y;
                }

                e.StopPropagation();
            }
        }

        protected void OnMouseUp(MouseUpEvent e)
        {
            /*
            IDraggable ce = e.target as IDraggable;
            if (ce == null || !ce.IsMovableNow())
            {
                return;
            }
            */

            if (m_Active)
            {
                if (CanStopManipulation(e))
                {
                    //ce.UpdatePresenterPosition();
                    m_Active = false;
                    target.ReleaseMouse();
                    e.StopPropagation();
                }
            }
        }
    }
}
