using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D.IK
{
    /// <summary>
    /// Attribute to add a menu item in IKManager2D to create the Solver.
    /// </summary>
    [MovedFrom("UnityEngine.Experimental.U2D.IK")]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class Solver2DMenuAttribute : Attribute
    {
        string m_MenuPath;

        /// <summary>
        /// Menu path.
        /// </summary>
        public string menuPath
        {
            get { return m_MenuPath; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_menuPath">Menu item path.</param>
        public Solver2DMenuAttribute(string _menuPath)
        {
            m_MenuPath = _menuPath;
        }
    }
}
