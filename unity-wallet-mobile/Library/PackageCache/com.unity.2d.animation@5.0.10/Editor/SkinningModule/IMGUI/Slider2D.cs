using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class Slider2D
    {
        private static Vector2 s_CurrentMousePosition;
        private static Vector2 s_DragStartScreenPosition;
        private static Vector2 s_DragScreenOffset;
        private static double s_Time;

        public static Vector2 Do(int controlID, Vector2 position, Handles.CapFunction drawCapFunction = null)
        {
            EventType type = Event.current.GetTypeForControl(controlID);

            switch (type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0 && HandleUtility.nearestControl == controlID && !Event.current.alt)
                    {
                        s_Time = EditorApplication.timeSinceStartup;

                        GUIUtility.keyboardControl = controlID;
                        GUIUtility.hotControl = controlID;
                        s_CurrentMousePosition = Event.current.mousePosition;
                        s_DragStartScreenPosition = Event.current.mousePosition;
                        Vector2 b = HandleUtility.WorldToGUIPoint(position);
                        s_DragScreenOffset = s_CurrentMousePosition - b;

                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID && (Event.current.button == 0 || Event.current.button == 2))
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        s_CurrentMousePosition = Event.current.mousePosition;
                        float screenDisplacement = (s_CurrentMousePosition - s_DragStartScreenPosition).magnitude;
                        Vector2 center = position;
                        Vector2 screenPosition = s_CurrentMousePosition - s_DragScreenOffset;
                        position = Handles.inverseMatrix.MultiplyPoint(screenPosition);
                        float displacement = (center - position).magnitude;

                        if (!Mathf.Approximately(displacement, 0f) && (EditorApplication.timeSinceStartup - s_Time > 0.15 || screenDisplacement >= 10f))
                            GUI.changed = true;

                        Event.current.Use();
                    }
                    break;
                case EventType.KeyDown:
                    if (GUIUtility.hotControl == controlID && Event.current.keyCode == KeyCode.Escape)
                    {
                        position = Handles.inverseMatrix.MultiplyPoint(s_DragStartScreenPosition - s_DragScreenOffset);
                        GUIUtility.hotControl = 0;
                        GUI.changed = true;
                        Event.current.Use();
                    }
                    break;
                case EventType.Layout:
                    if (drawCapFunction != null)
                        drawCapFunction(controlID, position, Quaternion.identity, 1f, EventType.Layout);
                    break;
                case EventType.Repaint:
                    if (drawCapFunction != null)
                        drawCapFunction(controlID, position, Quaternion.identity, 1f, EventType.Repaint);
                    break;
            }

            return position;
        }
    }
}
