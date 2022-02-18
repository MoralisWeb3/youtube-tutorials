using UnityEditor;
using UnityEngine;

using Codice.CM.Common;
using Codice.Client.BaseCommands.EventTracking;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Items;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.Inspector;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.AssetMenu
{
    [InitializeOnLoad]
    internal class AssetMenuItems
    {
        static AssetMenuItems()
        {
            CooldownWindowDelayer cooldownInitializeAction = new CooldownWindowDelayer(
                DelayedInitialization, UnityConstants.ASSET_MENU_DELAYED_INITIALIZE_INTERVAL);

            cooldownInitializeAction.Ping();
        }

        static void DelayedInitialization()
        { 
            PlasticApp.InitializeIfNeeded();
            Enable();
        }

        internal static void Dispose()
        {
            if (sAssetSelection != null)
                sAssetSelection.Dispose();
        }

        // TODO: do this after calling plastic workspace
        internal static void Enable()
        {
            if (sPlasticAPI == null)
                sPlasticAPI = new PlasticAPI();

            WorkspaceInfo wkInfo = FindWorkspace.InfoForApplicationPath(
                Application.dataPath,
                sPlasticAPI);

            if (wkInfo == null)
                return;

            sOperations = new AssetMenuRoutingOperations();

            sAssetStatusCache = new AssetStatusCache(
                wkInfo,
                sPlasticAPI.IsGluonWorkspace(wkInfo),
                RepaintProjectWindow);
            sAssetSelection = new InspectorAssetSelection(UpdateFilterMenuItems);
            sFilterMenuBuilder = new AssetFilesFilterPatternsMenuBuilder(
                sOperations,
                IGNORE_MENU_ITEMS_PRIORITY,
                HIDDEN_MENU_ITEMS_PRIORITY);

            AddMenuItems();
        }

        // TODO: Call this from the delete plastic workspace window
        internal static void Disable()
        {
            RemoveMenuItems();
        }

        static void AddMenuItems()
        {
            // TODO: Try removing this
            // Somehow first item always disappears. So this is a filler item
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.PendingChangesPlasticMenu),
                PENDING_CHANGES_MENU_ITEM_PRIORITY,
                PendingChanges, ValidatePendingChanges);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.PendingChangesPlasticMenu),
                PENDING_CHANGES_MENU_ITEM_PRIORITY,
                PendingChanges, ValidatePendingChanges);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.AddPlasticMenu),
                ADD_MENU_ITEM_PRIORITY,
                Add, ValidateAdd);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.CheckoutPlasticMenu),
                CHECKOUT_MENU_ITEM_PRIORITY,
                Checkout, ValidateCheckout);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.CheckinPlasticMenu),
                CHECKIN_MENU_ITEM_PRIORITY,
                Checkin, ValidateCheckin);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.UndoPlasticMenu),
                UNDO_MENU_ITEM_PRIORITY,
                Undo, ValidateUndo);

            UpdateFilterMenuItems();

            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.DiffPlasticMenu),
                GetPlasticShortcut.ForAssetDiff(),
                DIFF_MENU_ITEM_PRIORITY,
                Diff, ValidateDiff);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.HistoryPlasticMenu),
                GetPlasticShortcut.ForHistory(),
                HISTORY_MENU_ITEM_PRIORITY,
                History, ValidateHistory);

            HandleMenuItem.UpdateAllMenus();
        }

        static void UpdateFilterMenuItems()
        {
            SelectedPathsGroupInfo info = AssetsSelection.GetSelectedPathsGroupInfo(
                ((AssetOperations.IAssetSelection)sAssetSelection).GetSelectedAssets(),
                sAssetStatusCache);
            sFilterMenuBuilder.UpdateMenuItems(FilterMenuUpdater.GetMenuActions(info));
        }

        static string GetPlasticMenuItemName(PlasticLocalization.Name name)
        {
            return string.Format("{0}/{1}",
                PlasticLocalization.GetString(PlasticLocalization.Name.PrefixPlasticMenu),
                PlasticLocalization.GetString(name));
        }

        static void PendingChanges()
        {
            ShowWindow.Plastic();

            ((IAssetMenuOperations)sOperations).ShowPendingChanges();
        }

        static bool ValidatePendingChanges()
        {
            return true;
        }

        static void Add()
        {
            ((IAssetMenuOperations)sOperations).Add();
        }

        static bool ValidateAdd()
        {
            return ShouldMenuItemBeEnabled(AssetMenuOperations.Add);
        }

        static void Checkout()
        {
            ((IAssetMenuOperations)sOperations).Checkout();
        }

        static bool ValidateCheckout()
        {
            return ShouldMenuItemBeEnabled(AssetMenuOperations.Checkout);
        }

        static void Checkin()
        {
            WorkspaceInfo wkInfo = FindWorkspace.InfoForApplicationPath(
                Application.dataPath,
                sPlasticAPI);
            
            if (wkInfo != null)
            {
                TrackFeatureUseEvent.For(
                    PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
                    TrackFeatureUseEvent.Features.ContextMenuCheckinOption);
            }

            ((IAssetMenuOperations)sOperations).Checkin();
        }

        static bool ValidateCheckin()
        {
            return ShouldMenuItemBeEnabled(AssetMenuOperations.Checkin);
        }

        static void Undo()
        {
            ((IAssetMenuOperations)sOperations).Undo();
        }

        static bool ValidateUndo()
        {
            return ShouldMenuItemBeEnabled(AssetMenuOperations.Undo);
        }

        static void Diff()
        {
            ((IAssetMenuOperations)sOperations).ShowDiff();
        }

        static bool ValidateDiff()
        {
            return ShouldMenuItemBeEnabled(AssetMenuOperations.Diff);
        }

        static void History()
        {
            ShowWindow.Plastic();

            ((IAssetMenuOperations)sOperations).ShowHistory();
        }

        static bool ValidateHistory()
        {
            return ShouldMenuItemBeEnabled(AssetMenuOperations.History);
        }

        static bool ShouldMenuItemBeEnabled(AssetMenuOperations operation)
        {
            if (sOperations == null)
                return false;

            SelectedAssetGroupInfo selectedGroupInfo = SelectedAssetGroupInfo.
                BuildFromAssetList(
                    ((AssetOperations.IAssetSelection)sAssetSelection).GetSelectedAssets(),
                    sAssetStatusCache);

            AssetMenuOperations operations = AssetMenuUpdater.
                GetAvailableMenuOperations(selectedGroupInfo);

            return operations.HasFlag(operation);
        }

        static void RepaintProjectWindow()
        {
            EditorWindow projectWindow = FindEditorWindow.ProjectWindow();

            if (projectWindow == null)
                return;

            projectWindow.Repaint();
        }

        static void RemoveMenuItems()
        {
            sFilterMenuBuilder.RemoveMenuItems();

            HandleMenuItem.RemoveMenuItem(
                PlasticLocalization.GetString(PlasticLocalization.Name.PrefixPlasticMenu));

            HandleMenuItem.UpdateAllMenus();
        }

        static PlasticAPI sPlasticAPI;
        static AssetMenuRoutingOperations sOperations;
        static IAssetStatusCache sAssetStatusCache;
        static InspectorAssetSelection sAssetSelection;
        static AssetFilesFilterPatternsMenuBuilder sFilterMenuBuilder;

        const int BASE_MENU_ITEM_PRIORITY = 19; // Puts Plastic SCM right below Create menu

        // incrementing the "order" param by 11 causes the menu system to add a separator
        const int PENDING_CHANGES_MENU_ITEM_PRIORITY = BASE_MENU_ITEM_PRIORITY;
        const int ADD_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 11;
        const int CHECKOUT_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 12;
        const int CHECKIN_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 13;
        const int UNDO_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 14;
        const int IGNORE_MENU_ITEMS_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 25;
        const int HIDDEN_MENU_ITEMS_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 26;
        const int DIFF_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 37;
        const int HISTORY_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 38;
    }
}