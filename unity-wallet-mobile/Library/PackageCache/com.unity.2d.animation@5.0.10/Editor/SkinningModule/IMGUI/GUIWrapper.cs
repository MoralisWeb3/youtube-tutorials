using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal struct SliderData
    {
        public Vector3 position;
        public Vector3 forward;
        public Vector3 up;
        public Vector3 right;

        public static readonly SliderData zero = new SliderData() { position = Vector3.zero, forward = Vector3.forward, up = Vector3.up, right = Vector3.right };
    }

    internal interface IGUIWrapper
    {
        Vector2 mousePosition { get; }
        int mouseButton { get; }
        int clickCount { get; }
        bool isShiftDown { get; }
        bool isAltDown { get; }
        bool isActionKeyDown { get; }
        EventType eventType { get; }
        string commandName { get; }
        bool IsMouseDown(int button);
        bool IsMouseUp(int button);
        bool IsKeyDown(KeyCode keyCode);
        int GetControlID(int hint, FocusType focusType);
        void LayoutControl(int controlID, float distance);
        bool IsControlNearest(int controlID);
        bool IsControlHot(int controlID);
        bool IsMultiStepControlHot(int controlID);
        void SetControlHot(int controlID);
        void SetMultiStepControlHot(int controlID);
        bool DoSlider(int id, SliderData sliderData, out Vector3 newPosition);
        void UseCurrentEvent();
        float DistanceToSegment(Vector3 p1, Vector3 p2);
        float DistanceToSegmentClamp(Vector3 p1, Vector3 p2);
        float DistanceToCircle(Vector3 center, float radius);
        Vector3 GUIToWorld(Vector2 guiPosition);
        Vector3 GUIToWorld(Vector2 guiPosition, Vector3 planeNormal, Vector3 planePosition);
        void Repaint();
        bool IsRepainting();
        bool IsEventOutsideWindow();
        void SetGuiChanged(bool changed);
        float GetHandleSize(Vector3 position);
        bool IsViewToolActive();
        bool HasCurrentCamera();
    }

    internal class GUIWrapper : IGUIWrapper
    {
        private Handles.CapFunction nullCap = (int c, Vector3 p , Quaternion r, float s, EventType ev) => {};
        private int m_MultiStepHotControl = 0;

        public Vector2 mousePosition
        {
            get { return Event.current.mousePosition; }
        }

        public int mouseButton
        {
            get { return Event.current.button; }
        }

        public int clickCount
        {
            get { return Event.current.clickCount; }
        }

        public bool isShiftDown
        {
            get { return Event.current.shift; }
        }

        public bool isAltDown
        {
            get { return Event.current.alt; }
        }

        public bool isActionKeyDown
        {
            get { return EditorGUI.actionKey; }
        }

        public EventType eventType
        {
            get { return Event.current.type; }
        }

        public string commandName
        {
            get { return Event.current.commandName; }
        }

        public bool IsMouseDown(int button)
        {
            return Event.current.type == EventType.MouseDown && Event.current.button == button;
        }

        public bool IsMouseUp(int button)
        {
            return Event.current.type == EventType.MouseUp && Event.current.button == button;
        }

        public bool IsKeyDown(KeyCode keyCode)
        {
            return Event.current.type == EventType.KeyDown && Event.current.keyCode == keyCode;
        }

        public int GetControlID(int hint, FocusType focusType)
        {
            return GUIUtility.GetControlID(hint, focusType);
        }

        public void LayoutControl(int controlID, float distance)
        {
            if (Event.current.type == EventType.Layout)
                HandleUtility.AddControl(controlID, distance);
        }

        public bool IsControlNearest(int controlID)
        {
            return HandleUtility.nearestControl == controlID;
        }

        public bool IsControlHot(int controlID)
        {
            return GUIUtility.hotControl == controlID;
        }

        public bool IsMultiStepControlHot(int controlID)
        {
            return m_MultiStepHotControl == controlID;
        }

        public void SetControlHot(int controlID)
        {
            GUIUtility.hotControl = controlID;
        }

        public void SetMultiStepControlHot(int controlID)
        {
            m_MultiStepHotControl = controlID;
        }

        public bool DoSlider(int id, SliderData sliderData, out Vector3 newPosition)
        {
            EditorGUI.BeginChangeCheck();

            if (HasCurrentCamera())
                newPosition = Handles.Slider2D(id, sliderData.position, sliderData.forward, sliderData.right, sliderData.up, 1f, nullCap, Vector2.zero);
            else
                newPosition = Slider2D.Do(id, sliderData.position, null);

            return EditorGUI.EndChangeCheck();
        }

        public void UseCurrentEvent()
        {
            Event.current.Use();
        }

        public float DistanceToSegment(Vector3 p1, Vector3 p2)
        {
            p1 = HandleUtility.WorldToGUIPoint(p1);
            p2 = HandleUtility.WorldToGUIPoint(p2);

            return HandleUtility.DistancePointToLineSegment(mousePosition, p1, p2);
        }

        public float DistanceToSegmentClamp(Vector3 p1, Vector3 p2)
        {
            p1 = HandleUtility.WorldToGUIPoint(p1);
            p2 = HandleUtility.WorldToGUIPoint(p2);

            return MathUtility.DistanceToSegmentClamp(mousePosition, p1, p2);
        }

        public float DistanceToCircle(Vector3 center, float radius)
        {
            return HandleUtility.DistanceToCircle(center, radius);
        }

        public Vector3 GUIToWorld(Vector2 guiPosition)
        {
            return ModuleUtility.GUIToWorld(guiPosition);
        }

        public Vector3 GUIToWorld(Vector2 guiPosition, Vector3 planeNormal, Vector3 planePosition)
        {
            return ModuleUtility.GUIToWorld(guiPosition, planeNormal, planePosition);
        }

        public void Repaint()
        {
            HandleUtility.Repaint();
        }

        public bool IsRepainting()
        {
            return eventType == EventType.Repaint;
        }

        public void SetGuiChanged(bool changed)
        {
            GUI.changed = true;
        }

        public bool IsEventOutsideWindow()
        {
            return Event.current.type == EventType.Ignore;
        }

        public float GetHandleSize(Vector3 position)
        {
            return HandleUtility.GetHandleSize(position);
        }

        public bool IsViewToolActive()
        {
            return UnityEditor.Tools.current == Tool.View || isAltDown || mouseButton == 1 || mouseButton == 2;
        }

        public bool HasCurrentCamera()
        {
            return Camera.current != null;
        }
    }
}
