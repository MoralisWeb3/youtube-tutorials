using System;

using UnityEditor;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.AssetsOverlays
{
    internal static class DrawAssetOverlay
    {
        internal static void Initialize(
            IAssetStatusCache cache,
            Action repaintProjectWindow)
        {
            mAssetStatusCache = cache;
            mRepaintProjectWindow = repaintProjectWindow;

            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;

            mRepaintProjectWindow();
        }

        internal static void Dispose()
        {
            EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemGUI;

            if (mRepaintProjectWindow != null)
                mRepaintProjectWindow();
        }

        internal static void ClearCache()
        {
            mAssetStatusCache.Clear();
            mRepaintProjectWindow();
        }

        internal static string GetStatusString(AssetStatus assetStatus)
        {
            if (ClassifyAssetStatus.IsPrivate(assetStatus))
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.Private);

            if (ClassifyAssetStatus.IsIgnored(assetStatus))
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.StatusIgnored);

            if (ClassifyAssetStatus.IsAdded(assetStatus))
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.StatusAdded);

            if (ClassifyAssetStatus.IsConflicted(assetStatus))
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.StatusConflicted);

            if (ClassifyAssetStatus.IsDeletedOnServer(assetStatus))
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.StatusDeletedOnServer);

            if (ClassifyAssetStatus.IsLockedRemote(assetStatus))
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.StatusLockedRemote);

            if (ClassifyAssetStatus.IsOutOfDate(assetStatus))
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.StatusOutOfDate);

            if (ClassifyAssetStatus.IsLocked(assetStatus))
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.StatusLockedMe);

            if (ClassifyAssetStatus.IsCheckedOut(assetStatus))
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.StatusCheckout);

            return string.Empty;
        }

        internal static string GetTooltipText(
            AssetStatus statusValue,
            LockStatusData lockStatusData)
        {
            string statusText = GetStatusString(statusValue);

            if (lockStatusData == null)
                return statusText;

            // example:
            // Changed by:
            // * dani_pen@hotmail.com
            // * workspace wkLocal"

            char bulletCharacter = '\u25cf';

            string line1 = PlasticLocalization.GetString(
                PlasticLocalization.Name.AssetOverlayTooltipStatus, statusText);

            string line2 = string.Format("{0} {1}",
                bulletCharacter,
                lockStatusData.LockedBy);

            string line3 = string.Format("{0} {1}",
                bulletCharacter,
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.AssetOverlayTooltipWorkspace,
                    lockStatusData.WorkspaceName));

            return string.Format(
                "{0}" + Environment.NewLine +
                "{1}" + Environment.NewLine +
                "{2}",
                line1,
                line2,
                line3);
        }

        static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            if (string.IsNullOrEmpty(guid))
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            AssetStatus assetStatus = mAssetStatusCache.GetStatusForGuid(guid);

            LockStatusData lockStatusData =
                ClassifyAssetStatus.IsLockedRemote(assetStatus) ?
                mAssetStatusCache.GetLockStatusData(guid) :
                null;

            string tooltipText = GetTooltipText(
                assetStatus,
                lockStatusData);

            DrawOverlayIcon.ForStatus(
                selectionRect,
                assetStatus,
                tooltipText);
        }

        internal static class DrawOverlayIcon
        {
            internal static void ForStatus(
                Rect selectionRect,
                AssetStatus status,
                string tooltipText)
            {
                Texture overlayIcon = GetOverlayIcon(status);

                if (overlayIcon == null)
                    return;

                Rect overlayRect = OverlayRect.GetRightBottonRect(
                    selectionRect);

                GUI.DrawTexture(
                    overlayRect, overlayIcon, ScaleMode.ScaleToFit);

                Rect tooltipRect = GetTooltipRect(selectionRect, overlayRect);

                GUI.Label(tooltipRect, new GUIContent(string.Empty, tooltipText));
            }

            internal static Texture GetOverlayIcon(AssetStatus assetStatus)
            {
                if (ClassifyAssetStatus.IsPrivate(assetStatus))
                    return Images.GetPrivatedOverlayIcon();

                if (ClassifyAssetStatus.IsIgnored(assetStatus))
                    return Images.GetIgnoredOverlayIcon();

                if (ClassifyAssetStatus.IsAdded(assetStatus))
                    return Images.GetAddedOverlayIcon();

                if (ClassifyAssetStatus.IsConflicted(assetStatus))
                    return Images.GetConflictedOverlayIcon();

                if (ClassifyAssetStatus.IsDeletedOnServer(assetStatus))
                    return Images.GetDeletedRemoteOverlayIcon();

                if (ClassifyAssetStatus.IsLockedRemote(assetStatus))
                    return Images.GetLockedRemoteOverlayIcon();

                if (ClassifyAssetStatus.IsOutOfDate(assetStatus))
                    return Images.GetOutOfSyncOverlayIcon();

                if (ClassifyAssetStatus.IsLocked(assetStatus))
                    return Images.GetLockedLocalOverlayIcon();

                if (ClassifyAssetStatus.IsCheckedOut(assetStatus))
                    return Images.GetCheckedOutOverlayIcon();

                return null;
            }

            static Rect Inflate(Rect rect, float width, float height)
            {
                return new Rect(
                    rect.x - width,
                    rect.y - height,
                    rect.width + 2f * width,
                    rect.height + 2f * height);
            }

            static Rect GetTooltipRect(
                Rect selectionRect,
                Rect overlayRect)
            {
                if (selectionRect.width > selectionRect.height)
                {
                    return overlayRect;
                }

                return Inflate(overlayRect, 3f, 3f);
            }
        }

        static IAssetStatusCache mAssetStatusCache;
        static Action mRepaintProjectWindow;
    }
}

