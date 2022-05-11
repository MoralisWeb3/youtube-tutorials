using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

namespace UnityEditor.U2D
{
    public class SpriteShapeAssetPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Length > 0)
            {
                GameObject[] allGOs = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (GameObject go in allGOs)
                {
                    if (!go.activeInHierarchy)
                        continue;
                    SpriteShapeController sc = go.GetComponent<SpriteShapeController>();
                    if (sc != null)
                        sc.RefreshSpriteShape();
                }
            }
        }
    }
}
