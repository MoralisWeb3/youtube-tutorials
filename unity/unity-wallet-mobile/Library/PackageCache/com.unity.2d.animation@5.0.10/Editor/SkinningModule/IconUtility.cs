using System.IO;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal static class IconUtility
    {
        static public readonly string k_LightIconResourcePath = "SkinningModule/Icons/Light";
        static public readonly string k_DarkIconResourcePath = "SkinningModule/Icons/Dark";
        static public readonly string k_SelectedResourceIconPath = "SkinningModule/Icons/Selected";

        public static Texture2D LoadIconResource(string name, string personalPath, string proPath)
        {
            string iconPath = "";

            if (EditorGUIUtility.isProSkin && !string.IsNullOrEmpty(proPath))
                iconPath = Path.Combine(proPath, "d_" + name);
            else
                iconPath = Path.Combine(personalPath, name);
            if (EditorGUIUtility.pixelsPerPoint > 1.0f)
            {
                var icon2x = ResourceLoader.Load<Texture2D>(iconPath + "@2x.png");
                if (icon2x != null)
                    return icon2x;
            }

            return ResourceLoader.Load<Texture2D>(iconPath+".png");
        }
    }
}
