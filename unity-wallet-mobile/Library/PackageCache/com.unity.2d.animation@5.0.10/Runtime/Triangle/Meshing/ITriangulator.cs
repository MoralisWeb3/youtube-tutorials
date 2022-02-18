// -----------------------------------------------------------------------
// <copyright file="ITriangulator.cs" company="">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace UnityEngine.U2D.Animation.TriangleNet
    .Meshing
{
    using System.Collections.Generic;
    using Animation.TriangleNet.Geometry;

    /// <summary>
    /// Interface for point set triangulation.
    /// </summary>
    internal interface ITriangulator
    {
        /// <summary>
        /// Triangulates a point set.
        /// </summary>
        /// <param name="points">Collection of points.</param>
        /// <param name="config"></param>
        /// <returns>Mesh</returns>
        IMesh Triangulate(IList<Vertex> points, Configuration config);
    }
}
