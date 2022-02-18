using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    internal class GridPalettes : ScriptableSingleton<GridPalettes>
    {
        private static bool s_RefreshCache;

        [SerializeField] private List<GameObject> m_PalettesCache;

        public static List<GameObject> palettes
        {
            get
            {
                if (instance.m_PalettesCache == null || s_RefreshCache)
                {
                    instance.RefreshPalettesCache();
                    s_RefreshCache = false;
                }

                return instance.m_PalettesCache;
            }
        }

        private void RefreshPalettesCache()
        {
            if (instance.m_PalettesCache == null)
                instance.m_PalettesCache = new List<GameObject>();

            string[] guids = AssetDatabase.FindAssets("t:GridPalette");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GridPalette paletteAsset = AssetDatabase.LoadAssetAtPath(path, typeof(GridPalette)) as GridPalette;
                if (paletteAsset != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(paletteAsset);
                    GameObject palette = AssetDatabase.LoadMainAssetAtPath(assetPath) as GameObject;
                    if (palette != null)
                    {
                        m_PalettesCache.Add(palette);
                    }
                }
            }
            m_PalettesCache.Sort((x, y) => String.Compare(x.name, y.name, StringComparison.OrdinalIgnoreCase));
        }

        public class AssetProcessor : AssetPostprocessor
        {
            public override int GetPostprocessOrder()
            {
                return 1;
            }

            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
            {
                if (!GridPaintingState.savingPalette)
                    CleanCache();
            }
        }

        internal static void CleanCache()
        {
            instance.m_PalettesCache = null;
        }
    }
}
