using UnityEditor.VersionControl;

namespace Unity.PlasticSCM.Editor.AssetMenu
{
    internal class ProjectViewAssetSelection : AssetOperations.IAssetSelection
    {
        AssetList AssetOperations.IAssetSelection.GetSelectedAssets()
        {
            return Provider.GetAssetListFromSelection();
        }
    }
}
