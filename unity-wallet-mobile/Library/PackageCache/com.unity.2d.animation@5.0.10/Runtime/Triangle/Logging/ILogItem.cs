// -----------------------------------------------------------------------
// <copyright file="ILogItem.cs" company="">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace UnityEngine.U2D.Animation.TriangleNet
    .Logging
{
    using System;

    /// <summary>
    /// A basic log item interface.
    /// </summary>
    internal interface ILogItem
    {
        DateTime Time { get; }
        LogLevel Level { get; }
        string Message { get; }
        string Info { get; }
    }
}
