using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class RectSlider
    {
        private static readonly int kRectSliderHashCode = "RectSlider".GetHashCode();
        private Vector2 m_StartPosition = Vector2.zero;
        private Vector2 m_Position = Vector2.zero;

        internal Rect Do()
        {
            return Do(GUIUtility.GetControlID(kRectSliderHashCode, FocusType.Passive));
        }

        internal Rect Do(int controlID)
        {
            var eventType = Event.current.GetTypeForControl(controlID);

            if (eventType == EventType.MouseDown)
            {
                m_StartPosition = ModuleUtility.GUIToWorld(Event.current.mousePosition);
                m_Position = m_StartPosition;
            }

            if (eventType == EventType.Layout)
                HandleUtility.AddDefaultControl(controlID);

            m_Position = Slider2D.Do(controlID, m_Position);

            var rect = new Rect();
            rect.min = m_StartPosition;
            rect.max = m_Position;

            return rect;
        }
    }
}
