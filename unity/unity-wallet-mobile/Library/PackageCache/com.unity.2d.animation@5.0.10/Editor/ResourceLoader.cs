using System.IO;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal static class ResourceLoader
    {
        const string k_ResourcePath = "Packages/com.unity.2d.animation/Editor/Assets";

        internal static T Load<T>(string path) where T : Object
        {
            var assetPath = Path.Combine(k_ResourcePath, path);
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            return asset;
        }
    }
}

