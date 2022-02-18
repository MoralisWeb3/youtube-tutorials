using System;
using UnityEngine;

namespace UnityEditor.Timeline.Actions
{
    [Flags]
    enum MenuFilter
    {
        None = 0,
        Item = 1 << 0,
        Track = 1 << 1,
        MarkerHeader = 1 << 2,
        Default = Item | Track
    }

    /// <summary>
    /// Use this attribute to add a menu item to a context menu.
    /// Used to indicate path and priority that are auto added to the menu
    /// (examples can be found on <see href="https://docs.unity3d.com/ScriptReference/MenuItem.html"/>).
    /// </summary>
    /// <example>
    /// <code source="../../DocCodeExamples/TimelineAttributesExamples.cs" region="declare-menuEntryAttribute" title="menuEntryAttr"/>
    /// </example>
    /// <remarks>
    /// Unlike Menu item, MenuEntryAttribute doesn't handle shortcuts in the menu name. See <see cref="TimelineShortcutAttribute"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuEntryAttribute : Attribute
    {
        internal readonly int priority;
        internal readonly string name;
        internal readonly string subMenuPath;
        internal readonly MenuFilter filter;

        /// <summary>
        /// Constructor for Menu Entry Attribute to define information about the menu item for an action.
        /// </summary>
        /// <param name="path">Path to the menu. If there is a "/" in the path, it will create one (or multiple) submenu items.</param>
        /// <param name="priority">Priority to decide where the menu will be positioned in the menu.
        /// The lower the priority, the higher the menu item will be in the context menu.
        /// </param>
        /// <seealso cref="MenuPriority"/>
        public MenuEntryAttribute(string path = default, int priority = MenuPriority.defaultPriority) : this(path, priority, MenuFilter.Default) {}

        internal MenuEntryAttribute(string path, int priority, MenuFilter filter)
        {
            this.filter = filter;
            path = path ?? string.Empty;
            path = L10n.Tr(path);
            this.priority = priority;

            var index = path.LastIndexOf('/');
            if (index >= 0)
            {
                name = (index == path.Length - 1) ? string.Empty : path.Substring(index + 1);
                subMenuPath = path.Substring(0, index + 1);
            }
            else
            {
                name = path;
                subMenuPath = string.Empty;
            }
        }
    }

    static class MenuFilterExtensions
    {
        public static bool ShouldFilterOut(this MenuFilter filter, MenuEntryAttribute attr)
        {
            return (filter & attr.filter) != filter;
        }
    }
}
