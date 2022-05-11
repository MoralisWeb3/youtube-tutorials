// -----------------------------------------------------------------------
// <copyright file="ISegment.cs" company="">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace UnityEngine.U2D.Animation.TriangleNet
    .Geometry
{
    /// <summary>
    /// Interface for segment geometry.
    /// </summary>
    internal interface ISegment : IEdge
    {
        /// <summary>
        /// Gets the vertex at given index.
        /// </summary>
        /// <param name="index">The local index (0 or 1).</param>
        Vertex GetVertex(int index);

        /// <summary>
        /// Gets an adjoining triangle.
        /// </summary>
        /// <param name="index">The triangle index (0 or 1).</param>
        ITriangle GetTriangle(int index);
    }
}
