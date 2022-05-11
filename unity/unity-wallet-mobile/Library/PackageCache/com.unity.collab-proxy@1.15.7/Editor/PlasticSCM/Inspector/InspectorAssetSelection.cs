using System;

using UnityEditor;

using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetUtils;
using UnityEditor.VersionControl;

namespace Unity.PlasticSCM.Editor.Inspector
{
    internal class InspectorAssetSelection : AssetOperations.IAssetSelection
    {
        internal InspectorAssetSelection() { }

        internal InspectorAssetSelection(Action assetSelectionChangedAction)
        {
            mAssetSelectionChangedAction = assetSelectionChangedAction;

            Selection.selectionChanged += SelectionChanged;
        }

        internal void Dispose()
        {
            Selection.selectionChanged -= SelectionChanged;
        }

        void SelectionChanged()
        {
            // Selection.selectionChanged gets triggered on both
            // project view and scene view. We only want to trigger
            // the action if user selects on project view (has assets)
            if (HasSelectedAssets())
                mAssetSelectionChangedAction();
        }

        AssetList AssetOperations.IAssetSelection.GetSelectedAssets()
        {
            AssetList result = new AssetList();

            // We filter for assets because it is possible for user
            // to select things in both project and scene views at the same time
            UnityEngine.Object[] selectedObjects = 
                Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets);

            foreach (UnityEngine.Object obj in selectedObjects)
            {
                result.Add(new Asset(AssetsPath.GetFullPath(obj)));
            }

            return result;
        }

        bool HasSelectedAssets()
        {
            // Objects in project view have GUIDs, objects in scene view don't
            return Selection.assetGUIDs.Length > 0;
        }

        Action mAssetSelectionChangedAction;
    }
}
