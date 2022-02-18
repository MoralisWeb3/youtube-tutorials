using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// Use this attribute to add an option to customize the sorting of Active Targets in the Active Tilemap list of the Tile Palette window.
    /// </summary>
    /// <remarks>
    /// Append this attribute to a class which inherits from IComparer&lt;GameObject&gt; or to a method which creates an IComparer&lt;GameObject&gt;. The instance of IComparer generated with the attribute is used for comparing and sorting Active Target GameObjects in the Active Tilemaps list.
    /// </remarks>
    /// <example>
    /// <code lang="cs"><![CDATA[
    /// using System;
    /// using System.Collections.Generic;
    /// using UnityEngine;
    /// using UnityEditor;
    ///
    /// [GridPaintSorting]
    /// class Alphabetical : IComparer<GameObject>
    /// {
    ///     public int Compare(GameObject go1, GameObject go2)
    ///     {
    ///         return String.Compare(go1.name, go2.name);
    ///     }
    /// }
    ///
    /// class ReverseAlphabeticalComparer : IComparer<GameObject>
    /// {
    ///     public int Compare(GameObject go1, GameObject go2)
    ///     {
    ///         return -String.Compare(go1.name, go2.name);
    ///     }
    ///
    ///     [GridPaintSorting]
    ///     public static IComparer<GameObject> ReverseAlphabetical()
    ///     {
    ///         return new ReverseAlphabeticalComparer();
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class GridPaintSortingAttribute : Attribute
    {
        private static List<MethodInfo> m_SortingMethods;
        private static List<Type> m_SortingTypes;

        internal static List<MethodInfo> sortingMethods
        {
            get
            {
                if (m_SortingMethods == null)
                    GetUserSortingComparers();
                return m_SortingMethods;
            }
        }

        internal static List<Type> sortingTypes
        {
            get
            {
                if (m_SortingTypes == null)
                    GetUserSortingComparers();
                return m_SortingTypes;
            }
        }

        private static void GetUserSortingComparers()
        {
            m_SortingMethods = new List<MethodInfo>();
            foreach (var sortingMethod in EditorAssemblies.GetAllMethodsWithAttribute<GridPaintSortingAttribute>())
            {
                if (!sortingMethod.ReturnType.IsAssignableFrom(typeof(IComparer<GameObject>)))
                    continue;
                if (sortingMethod.GetGenericArguments().Length > 0)
                    continue;
                m_SortingMethods.Add(sortingMethod);
            }

            m_SortingTypes = new List<Type>();
            foreach (var sortingType in TypeCache.GetTypesWithAttribute<GridPaintSortingAttribute>())
            {
                if (sortingType.IsAbstract)
                    continue;
                m_SortingTypes.Add(sortingType);
            }
        }

        [GridPaintSorting]
        internal class Alphabetical : IComparer<GameObject>
        {
            public int Compare(GameObject go1, GameObject go2)
            {
                return String.Compare(go1.name, go2.name);
            }
        }

        [GridPaintSorting]
        internal class ReverseAlphabetical : IComparer<GameObject>
        {
            public int Compare(GameObject go1, GameObject go2)
            {
                return -String.Compare(go1.name, go2.name);
            }
        }
    }
}
