using System;
using System.Reflection;
using UnityEditor.ShortcutManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor.U2D.Common
{
    internal static class InternalEditorBridge
    {
        public static void RenderSortingLayerFields(SerializedProperty order, SerializedProperty layer)
        {
            SortingLayerEditorUtility.RenderSortingLayerFields(order, layer);
        }

        public static void RepaintImmediately(EditorWindow window)
        {
            window.RepaintImmediately();
        }

        public static ISpriteEditorDataProvider GetISpriteEditorDataProviderFromPath(string importedAsset)
        {
            return AssetImporter.GetAtPath(importedAsset) as ISpriteEditorDataProvider;
        }

        public static void GenerateOutline(Texture2D texture, Rect rect, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths)
        {
            UnityEditor.Sprites.SpriteUtility.GenerateOutline(texture, rect, detail, alphaTolerance, holeDetection, out paths);
        }

        public static bool DoesHardwareSupportsFullNPOT()
        {
            return ShaderUtil.hardwareSupportsFullNPOT;
        }

        public static Texture2D CreateTemporaryDuplicate(Texture2D tex, int width, int height)
        {
            return UnityEditor.SpriteUtility.CreateTemporaryDuplicate(tex, width, height);
        }

        public static void ShowSpriteEditorWindow(UnityEngine.Object obj = null)
        {
            SpriteUtilityWindow.ShowSpriteEditorWindow(obj);
        }

        public static void ApplyWireMaterial()
        {
            HandleUtility.ApplyWireMaterial();
        }

        public static void ResetSpriteEditorView(ISpriteEditor spriteEditor)
        {
            if (spriteEditor != null)
            {
                Type t = spriteEditor.GetType();
                var zoom = t.GetField("m_Zoom", BindingFlags.Instance | BindingFlags.NonPublic);
                if (zoom != null)
                {
                    zoom.SetValue(spriteEditor, -1);
                }

                var scrollPosition = t.GetField("m_ScrollPosition", BindingFlags.Instance | BindingFlags.NonPublic);
                if (scrollPosition != null)
                {
                    scrollPosition.SetValue(spriteEditor, new Vector2());
                }
            }
        }

        public class ShortcutContext : IShortcutToolContext
        {
            public Func<bool> isActive;
            public bool active
            {
                get
                {
                    if (isActive != null)
                        return isActive();
                    return true;
                }
            }
            public object context { get; set; }
        }

        public static void RegisterShortcutContext(ShortcutContext context)
        {
            ShortcutIntegration.instance.contextManager.RegisterToolContext(context);
        }

        public static void UnregisterShortcutContext(ShortcutContext context)
        {
            ShortcutIntegration.instance.contextManager.DeregisterToolContext(context);
        }

        public static void AddEditorApplicationProjectLoadedCallback(UnityAction callback)
        {
            EditorApplication.projectWasLoaded += callback;
        }

        public static void RemoveEditorApplicationProjectLoadedCallback(UnityAction callback)
        {
            EditorApplication.projectWasLoaded -= callback;
        }

        public static string GetProjectWindowActiveFolderPath()
        {
            return ProjectWindowUtil.GetActiveFolderPath();
        }

        public static GUIContent GetIconContent<T>() where T : UnityEngine.Object 
        {
            return EditorGUIUtility.IconContent<T>();
        }

        public static int GetAssetCreationInstanceID_ForNonExistingAssets()
        {
            return ProjectBrowser.kAssetCreationInstanceID_ForNonExistingAssets;
        }
    }
}
