using PlasticGui;

using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;

namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    internal static class AssetsProcessors
    {
        internal static void Enable(
            IPlasticAPI plasticApi,
            IAssetStatusCache assetStatusCache)
        {
            PlasticAssetsProcessor.RegisterPlasticAPI(plasticApi);
            AssetModificationProcessor.RegisterAssetStatusCache(assetStatusCache);

            AssetPostprocessor.IsEnabled = true;
            AssetModificationProcessor.IsEnabled = true;
        }

        internal static void Disable()
        {
            AssetPostprocessor.IsEnabled = false;
            AssetModificationProcessor.IsEnabled = false;
        }
    }
}
