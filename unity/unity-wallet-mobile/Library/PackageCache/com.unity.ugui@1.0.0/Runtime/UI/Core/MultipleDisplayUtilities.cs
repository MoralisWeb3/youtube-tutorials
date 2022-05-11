using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    internal static class MultipleDisplayUtilities
    {
        /// <summary>
        /// Converts the current drag position into a relative position for the display.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="position"></param>
        /// <returns>Returns true except when the drag operation is not on the same display as it originated</returns>
        public static bool GetRelativeMousePositionForDrag(PointerEventData eventData, ref Vector2 position)
        {
            #if UNITY_EDITOR
            position = eventData.position;
            #else
            int pressDisplayIndex = eventData.pointerPressRaycast.displayIndex;
            var relativePosition = Display.RelativeMouseAt(eventData.position);
            int currentDisplayIndex = (int)relativePosition.z;

            // Discard events on a different display.
            if (currentDisplayIndex != pressDisplayIndex)
                return false;

            // If we are not on the main display then we must use the relative position.
            position = pressDisplayIndex != 0 ? (Vector2)relativePosition : eventData.position;
            #endif
            return true;
        }

        /// <summary>
        /// Adjusts the position when the main display has a different rendering resolution to the system resolution.
        /// By default, the mouse position is relative to the main render area, we need to adjust this so it is relative to the system resolution
        /// in order to correctly determine the position on other displays.
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetMousePositionRelativeToMainDisplayResolution()
        {
            var position = Input.mousePosition;
            #if !UNITY_EDITOR
            if (Display.main.renderingHeight != Display.main.systemHeight)
            {
                // The position is relative to the main render area, we need to adjust this so
                // it is relative to the system resolution in order to correctly determine the position on other displays.

                // Correct the y position if we are outside the main display.
                if ((position.y < 0 || position.y > Display.main.renderingHeight ||
                     position.x < 0 || position.x > Display.main.renderingWidth) && (Screen.fullScreenMode != FullScreenMode.Windowed))
                {
                    position.y += Display.main.systemHeight - Display.main.renderingHeight;
                }
            }
            #endif
            return position;
        }
    }
}
