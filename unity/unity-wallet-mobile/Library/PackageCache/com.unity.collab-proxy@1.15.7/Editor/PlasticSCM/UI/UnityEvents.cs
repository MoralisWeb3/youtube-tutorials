using UnityEngine;

using Codice.Utils;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class Keyboard
    {
        internal static bool IsShiftPressed(Event e)
        {
            return e.type == EventType.KeyDown
                && e.shift;
        }

        internal static bool IsReturnOrEnterKeyPressed(Event e)
        {
            return IsKeyPressed(e, KeyCode.Return) ||
                   IsKeyPressed(e, KeyCode.KeypadEnter);
        }

        internal static bool IsKeyPressed(Event e, KeyCode keyCode)
        {
            return e.type == EventType.KeyDown
                && e.keyCode == keyCode;
        }

        internal static bool IsControlOrCommandKeyPressed(Event e)
        {
            if (PlatformIdentifier.IsMac())
                return e.type == EventType.KeyDown && e.command;

            return e.type == EventType.KeyDown && e.control;
        }
    }

    internal class Mouse
    {
        internal static bool IsLeftMouseButtonPressed(Event e)
        {
            if (!e.isMouse)
                return false;

            return e.button == UnityConstants.LEFT_MOUSE_BUTTON
                && e.type == EventType.MouseDown;
        }

        internal static bool IsRightMouseButtonPressed(Event e)
        {
            if (!e.isMouse)
                return false;

            return e.button == UnityConstants.RIGHT_MOUSE_BUTTON
                && e.type == EventType.MouseDown;
        }
    }
}
