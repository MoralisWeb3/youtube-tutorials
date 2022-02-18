using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class Brush
    {
        private static readonly float kWheelSizeSpeed = 1f;
        private static readonly int kBrushHashCode = "Brush".GetHashCode();
        private IGUIWrapper m_GUIWrapper;
        private float m_DeltaAcc = 0f;
        private int m_ControlID = -1;
        private SliderData m_SliderData = SliderData.zero;

        public event Action<Brush> onMove = (b) => {};
        public event Action<Brush> onSize = (b) => {};
        public event Action<Brush> onRepaint = (b) => {};
        public event Action<Brush> onStrokeBegin = (b) => {};
        public event Action<Brush> onStrokeDelta = (b) => {};
        public event Action<Brush> onStrokeStep = (b) => {};
        public event Action<Brush> onStrokeEnd = (b) => {};

        public bool isHot
        {
            get { return m_GUIWrapper.IsControlHot(m_ControlID); }
        }
        public bool isActivable
        {
            get { return m_GUIWrapper.IsControlHot(0) && m_GUIWrapper.IsControlNearest(m_ControlID); }
        }

        public int controlID
        {
            get { return m_ControlID; }
        }

        public float hardness { get; set; }
        public float step { get; set; }
        public float size { get; set; }
        public Vector3 position
        {
            get { return m_SliderData.position; }
        }

        public Brush(IGUIWrapper guiWrapper)
        {
            m_GUIWrapper = guiWrapper;
            size = 25f;
            step = 20f;
        }

        public void OnGUI()
        {
            m_ControlID = m_GUIWrapper.GetControlID(kBrushHashCode, FocusType.Passive);

            var eventType = m_GUIWrapper.eventType;

            if (!m_GUIWrapper.isAltDown)
                m_GUIWrapper.LayoutControl(controlID, 0f);

            if (isActivable)
            {
                m_SliderData.position = m_GUIWrapper.GUIToWorld(m_GUIWrapper.mousePosition);

                if (m_GUIWrapper.IsMouseDown(0))
                {
                    m_DeltaAcc = 0f;
                    onStrokeBegin(this);
                    onStrokeStep(this);
                    m_GUIWrapper.SetGuiChanged(true);
                }

                if (eventType == EventType.MouseMove)
                {
                    onMove(this);
                    m_GUIWrapper.UseCurrentEvent();
                }

                if (m_GUIWrapper.isShiftDown && eventType == EventType.ScrollWheel)
                {
                    var sizeDelta = HandleUtility.niceMouseDeltaZoom * kWheelSizeSpeed;
                    size = Mathf.Max(1f, size + sizeDelta);
                    onSize(this);
                    m_GUIWrapper.UseCurrentEvent();
                }
            }

            if (isHot && m_GUIWrapper.IsMouseUp(0))
                onStrokeEnd(this);

            if (m_GUIWrapper.IsRepainting() && (isHot || isActivable))
                onRepaint(this);

            Vector3 position;
            if (m_GUIWrapper.DoSlider(m_ControlID, m_SliderData, out position))
            {
                step = Mathf.Max(step, 1f);

                var delta = position - m_SliderData.position;
                var direction = delta.normalized;
                var magnitude = delta.magnitude;

                m_SliderData.position -= direction * m_DeltaAcc;

                m_DeltaAcc += magnitude;

                if (m_DeltaAcc >= step)
                {
                    var stepVector = direction * step;

                    while (m_DeltaAcc >= step)
                    {
                        m_SliderData.position += stepVector;

                        onMove(this);
                        onStrokeStep(this);

                        m_DeltaAcc -= step;
                    }
                }

                m_SliderData.position = position;
                onStrokeDelta(this);
            }
        }
    }
}
