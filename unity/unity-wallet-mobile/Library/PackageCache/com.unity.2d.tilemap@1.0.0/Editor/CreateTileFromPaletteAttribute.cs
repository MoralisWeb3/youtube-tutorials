using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Use this attribute to add an option to customize how Tiles are created when dragging and dropping assets to the Tile Palette.
    /// </summary>
    /// <remarks>
    /// Append this attribute to a method that has a signature of "static TileBase CreateTile(Sprite sprite)".
    /// </remarks>
    /// <example>
    /// <code lang="cs"><![CDATA[
    /// using UnityEditor.Tilemaps;
    /// using UnityEngine;
    /// using UnityEngine.Tilemaps;
    ///
    /// public class CreateBlueTile
    /// {
    ///     [CreateTileFromPalette]
    ///     public static TileBase BlueTile(Sprite sprite)
    ///     {
    ///         var blueTile = ScriptableObject.CreateInstance<Tile>();
    ///         blueTile.sprite = sprite;
    ///         blueTile.name = sprite.name;
    ///         blueTile.color = Color.blue;
    ///         return blueTile;
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method)]
    public class CreateTileFromPaletteAttribute : Attribute
    {
        private static List<MethodInfo> m_CreateTileFromPaletteMethods;
        internal static List<MethodInfo> createTileFromPaletteMethods
        {
            get
            {
                if (m_CreateTileFromPaletteMethods == null)
                    GetCreateTileFromPaletteAttributeMethods();
                return m_CreateTileFromPaletteMethods;
            }
        }

        [RequiredSignature]
        private static TileBase CreateTile(Sprite sprite)
        {
            return null;
        }

        private static void GetCreateTileFromPaletteAttributeMethods()
        {
            m_CreateTileFromPaletteMethods = new List<MethodInfo>();
            foreach (var sortingMethod in EditorAssemblies.GetAllMethodsWithAttribute<CreateTileFromPaletteAttribute>())
            {
                m_CreateTileFromPaletteMethods.Add(sortingMethod);
            }
        }
    }
}
