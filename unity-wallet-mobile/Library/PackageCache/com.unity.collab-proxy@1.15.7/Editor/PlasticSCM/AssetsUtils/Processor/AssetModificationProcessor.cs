using System.IO;

using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;

namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    class AssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        internal static bool IsEnabled { get; set; }
        internal static bool ForceCheckout { get; set; }
        
        /*We need to do a checkout, verifying that the content/date or size has changed. In order
          to do this checkout we need the changes to have reached the disk. That's why we save the
          changed files in this array, and when they are reloaded in 
        AssetPostprocessor.OnPostprocessAllAssets we process them. */
        internal static string[] ModifiedAssets { get; set; }

        internal static void RegisterAssetStatusCache(
            IAssetStatusCache assetStatusCache)
        {
            mAssetStatusCache = assetStatusCache;
        }

        static string[] OnWillSaveAssets(string[] paths)
        {
            if (!IsEnabled)
                return paths;

            ModifiedAssets = (string[])paths.Clone();

            return paths;
        }

        static bool IsOpenForEdit(string assetPath, out string message)
        {
            message = string.Empty;

            if (!IsEnabled)
                return true;

            if (assetPath.StartsWith("ProjectSettings/"))
                return true;

            if (!ForceCheckout)
                return true;

            if (MetaPath.IsMetaPath(assetPath))
                assetPath = MetaPath.GetPathFromMetaPath(assetPath);

            AssetStatus status = mAssetStatusCache.GetStatusForPath(
                Path.GetFullPath(assetPath));

            if (ClassifyAssetStatus.IsAdded(status) ||
                ClassifyAssetStatus.IsCheckedOut(status))
                return true;

            return !ClassifyAssetStatus.IsControlled(status);
        }

        static IAssetStatusCache mAssetStatusCache;
    }
}
