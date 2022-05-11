using System.IO;

using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using PlasticGui;

namespace Unity.PlasticSCM.Editor.Inspector
{
    static class DrawInspectorOperations
    {
        internal static void Enable(
            IAssetMenuOperations operations,
            IAssetStatusCache statusCache,
            AssetOperations.IAssetSelection assetsSelection)
        {
            mOperations = operations;
            mStatusCache = statusCache;
            mAssetsSelection = assetsSelection;

            mIsEnabled = true;

            UnityEditor.Editor.finishedDefaultHeaderGUI +=
                Editor_finishedDefaultHeaderGUI;

            RefreshAsset.RepaintInspectors();
        }

        internal static void Disable()
        {
            mIsEnabled = false;

            UnityEditor.Editor.finishedDefaultHeaderGUI -=
                Editor_finishedDefaultHeaderGUI;

            RefreshAsset.RepaintInspectors();
        }

        static void Editor_finishedDefaultHeaderGUI(UnityEditor.Editor obj)
        {
            if (!mIsEnabled)
                return;

            AssetList assetList = mAssetsSelection.GetSelectedAssets();

            if (mOperations == null ||
                assetList.Count == 0 ||
                string.IsNullOrEmpty(assetList[0].path))
                return;

            string selectionFullPath = Path.GetFullPath(assetList[0].path);

            AssetsOverlays.AssetStatus assetStatus = (assetList.Count > 1) ?
                AssetsOverlays.AssetStatus.None :
                mStatusCache.GetStatusForPath(selectionFullPath);

            LockStatusData lockStatusData = mStatusCache.GetLockStatusDataForPath(
                selectionFullPath);

            SelectedAssetGroupInfo selectedGroupInfo = SelectedAssetGroupInfo.
                BuildFromAssetList(assetList, mStatusCache);

            AssetMenuOperations assetOperations =
                AssetMenuUpdater.GetAvailableMenuOperations(selectedGroupInfo);

            bool guiEnabledBck = GUI.enabled;
            GUI.enabled = true;
            try
            {
                DrawBackRectangle(guiEnabledBck);

                GUILayout.BeginHorizontal();
                
                DrawStatusLabel(assetStatus, lockStatusData);
                
                GUILayout.FlexibleSpace();

                DrawButtons(assetList, assetOperations);

                GUILayout.EndHorizontal();
            }
            finally
            {
                GUI.enabled = guiEnabledBck;
            }
        }

        static void DrawBackRectangle(bool isEnabled)
        {
            // when the inspector is disabled, there is a separator line
            // that breaks the visual style. Draw an empty rectangle
            // matching the background color to cover it

            GUILayout.Space(UnityConstants.INSPECTOR_ACTIONS_BACK_RECTANGLE_TOP_MARGIN);

            GUIStyle targetStyle = (isEnabled) ?
                UnityStyles.Inspector.HeaderBackgroundStyle :
                UnityStyles.Inspector.DisabledHeaderBackgroundStyle;

            Rect rect = GUILayoutUtility.GetRect(
                GUIContent.none, targetStyle);

            // extra space to cover the inspector full width
            rect.x -= 20;
            rect.width += 80;

            GUI.Box(rect, GUIContent.none, targetStyle);

            // now reset the space used by the rectangle
            GUILayout.Space(
                -UnityConstants.INSPECTOR_ACTIONS_HEADER_BACK_RECTANGLE_HEIGHT
                - UnityConstants.INSPECTOR_ACTIONS_BACK_RECTANGLE_TOP_MARGIN);
        }

        static void DrawButtons(
            AssetList assetList,
            AssetMenuOperations selectedGroupInfo)
        {
            if (selectedGroupInfo.HasFlag(AssetMenuOperations.Add))
                DoAddButton();

            if (selectedGroupInfo.HasFlag(AssetMenuOperations.Checkout))
                DoCheckoutButton();

            if (selectedGroupInfo.HasFlag(AssetMenuOperations.Checkin))
                DoCheckinButton();

            if (selectedGroupInfo.HasFlag(AssetMenuOperations.Undo))
                DoUndoButton();
        }

        static void DrawStatusLabel(
            AssetsOverlays.AssetStatus assetStatus,
            LockStatusData lockStatusData)
        {
            Texture overlayIcon = DrawAssetOverlay.DrawOverlayIcon.
                GetOverlayIcon(assetStatus);

            if (overlayIcon == null)
                return;

            string statusText = DrawAssetOverlay.
                GetStatusString(assetStatus);

            string tooltipText = DrawAssetOverlay.GetTooltipText(
                assetStatus, lockStatusData);

            Rect selectionRect = GUILayoutUtility.GetRect(
                new GUIContent(statusText + EXTRA_SPACE, overlayIcon),
                GUIStyle.none);

            selectionRect.height = UnityConstants.OVERLAY_STATUS_ICON_SIZE;

            Rect overlayRect = OverlayRect.GetCenteredRect(selectionRect);

            GUI.DrawTexture(
                overlayRect,
                overlayIcon,
                ScaleMode.ScaleToFit);

            selectionRect.x += UnityConstants.OVERLAY_STATUS_ICON_SIZE;
            selectionRect.width -= UnityConstants.OVERLAY_STATUS_ICON_SIZE;

            GUI.Label(
                selectionRect,
                new GUIContent(statusText, tooltipText));
        }

        static void DoAddButton()
        {
            string buttonText = PlasticLocalization.GetString(PlasticLocalization.Name.AddButton);
            if (GUILayout.Button(string.Format("{0}", buttonText), EditorStyles.miniButton))
            {
                mOperations.Add();
            }
        }

        static void DoCheckoutButton()
        {
            string buttonText = PlasticLocalization.GetString(PlasticLocalization.Name.CheckoutButton);
            if (GUILayout.Button(string.Format("{0}", buttonText), EditorStyles.miniButton))
            {
                mOperations.Checkout();
            }
        }

        static void DoCheckinButton()
        {
            string buttonText = PlasticLocalization.GetString(PlasticLocalization.Name.CheckinButton);
            if (GUILayout.Button(string.Format("{0}", buttonText), EditorStyles.miniButton))
            {
                mOperations.Checkin();
                EditorGUIUtility.ExitGUI();
            }
        }

        static void DoUndoButton()
        {
            string buttonText = PlasticLocalization.GetString(PlasticLocalization.Name.UndoButton);
            if (GUILayout.Button(string.Format("{0}", buttonText), EditorStyles.miniButton))
            {
                mOperations.Undo();
                EditorGUIUtility.ExitGUI();
            }
        }

        static IAssetMenuOperations mOperations;
        static IAssetStatusCache mStatusCache;
        static AssetOperations.IAssetSelection mAssetsSelection;
        static bool mIsEnabled;
        const string EXTRA_SPACE = "    ";
    }
}