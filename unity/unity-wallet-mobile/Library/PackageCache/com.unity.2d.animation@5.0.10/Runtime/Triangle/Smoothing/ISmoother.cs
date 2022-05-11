// -----------------------------------------------------------------------
// <copyright file="ISmoother.cs">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace UnityEngine.U2D.Animation.TriangleNet
    .Smoothing
{
    using Animation.TriangleNet.Meshing;

    /// <summary>
    /// Interface for mesh smoothers.
    /// </summary>
    internal interface ISmoother
    {
        void Smooth(IMesh mesh);
        void Smooth(IMesh mesh, int limit);
    }
}
