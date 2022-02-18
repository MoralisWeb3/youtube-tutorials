using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.U2D.Animation.Sample
{
    public class LoadSwapDLC : MonoBehaviour
    {
        const string k_AssetBundleName = "2DAnimationSampleAssetBundles";
        public SwapFullSkin[] swapFullSkin;
        
        public void LoadAssetBundle()
        {
            var assetBundlePath = Path.Combine(Application.streamingAssetsPath, k_AssetBundleName);
            var bundle = AssetBundle.LoadFromFile(Path.Combine(assetBundlePath, k_AssetBundleName));
            if (bundle == null)
            {
                Debug.LogWarning("AssetBundle not found");
                return;
            }
            var manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (manifest == null)
            {
                Debug.LogWarning("Unable to load manifest");
                return;
            }
            foreach (var assetBundleName in manifest.GetAllAssetBundles())
            {
                var subBundle = AssetBundle.LoadFromFile(Path.Combine(assetBundlePath, assetBundleName));
                var assets = subBundle.LoadAllAssets();
                foreach (var asset in assets)
                {
                    if (asset is SpriteLibraryAsset)
                    {
                        var sla = (SpriteLibraryAsset)asset;
                        foreach (var sfs in swapFullSkin)
                        {
                            var list = sfs.spriteLibraries.ToList();
                            list.Add(sla);
                            sfs.spriteLibraries = list.ToArray();
                        }
                        
                    }
                }
            }
            foreach (var sfs in swapFullSkin)
            {
                sfs.UpdateSelectionChoice();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Build Asset Bundles")]
        void BuildBundles()
        {
            BuildAssetBundles();
        }

        public static void BuildAssetBundles()
        {
            string assetBundleDirectory = Path.Combine(Application.streamingAssetsPath, "2DAnimationSampleAssetBundles");
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
#endif
    }
}