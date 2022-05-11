using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class OverlayRect
    {
        internal static Rect GetRightBottonRect(
            Rect selectionRect)
        {
            if (selectionRect.width > selectionRect.height)
                return GetOverlayRectForSmallestSize(
                    selectionRect);

            return GetOverlayRectForOtherSizes(selectionRect);
        }

        internal static Rect GetCenteredRect(
                Rect selectionRect)
        {
            return new Rect(
                selectionRect.x + 3f,
                selectionRect.y + 1f,
                UnityConstants.OVERLAY_STATUS_ICON_SIZE,
                UnityConstants.OVERLAY_STATUS_ICON_SIZE);
        }

        static Rect GetOverlayRectForSmallestSize(
                    Rect selectionRect)
        {
            return new Rect(
                selectionRect.x + 5f,
                selectionRect.y + 4f,
                UnityConstants.OVERLAY_STATUS_ICON_SIZE,
                UnityConstants.OVERLAY_STATUS_ICON_SIZE);
        }

        static Rect GetOverlayRectForOtherSizes(
            Rect selectionRect)
        {
            float xOffset =  selectionRect.width - UnityConstants.OVERLAY_STATUS_ICON_SIZE;
            float yOffset =  selectionRect.height - UnityConstants.OVERLAY_STATUS_ICON_SIZE;

            return new Rect(
               selectionRect.x + xOffset,
               selectionRect.y + yOffset,
               UnityConstants.OVERLAY_STATUS_ICON_SIZE,
               UnityConstants.OVERLAY_STATUS_ICON_SIZE);
        }
    }
}