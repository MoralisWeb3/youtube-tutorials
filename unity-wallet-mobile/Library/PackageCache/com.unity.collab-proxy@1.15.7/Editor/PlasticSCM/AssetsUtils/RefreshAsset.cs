using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.AssetUtils
{
    internal static class RefreshAsset
    {
        internal static void UnityAssetDatabase()
        {
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            VersionControlCache();
        }

        internal static void VersionControlCache()
        {
            UnityEditor.VersionControl.Provider.ClearCache();
            RepaintInspectors();
        }

        internal static void RepaintInspectors()
        {
            UnityEditor.Editor[] editors =
                Resources.FindObjectsOfTypeAll<UnityEditor.Editor>();

            foreach (UnityEditor.Editor editor in editors)
                editor.Repaint();
         }
    }
}