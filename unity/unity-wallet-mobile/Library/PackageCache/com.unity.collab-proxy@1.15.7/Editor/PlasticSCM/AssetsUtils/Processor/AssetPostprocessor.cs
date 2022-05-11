namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    class AssetPostprocessor : UnityEditor.AssetPostprocessor
    {
        internal static bool IsEnabled { get; set; }

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!IsEnabled)
                return;

            for (int i = 0; i < movedAssets.Length; i++)
            {
                PlasticAssetsProcessor.MoveOnSourceControl(
                    movedFromAssetPaths[i],
                    movedAssets[i]);
            }

            foreach (string deletedAsset in deletedAssets)
            {
                PlasticAssetsProcessor.DeleteFromSourceControl(
                    deletedAsset);
            }

            PlasticAssetsProcessor.AddToSourceControl(importedAssets);

            if (AssetModificationProcessor.ModifiedAssets == null)
                return;

            PlasticAssetsProcessor.CheckoutOnSourceControl(AssetModificationProcessor.ModifiedAssets);
            AssetModificationProcessor.ModifiedAssets = null;
        }
    }
}
