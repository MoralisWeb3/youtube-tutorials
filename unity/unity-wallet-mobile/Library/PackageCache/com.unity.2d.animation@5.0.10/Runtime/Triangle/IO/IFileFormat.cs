// -----------------------------------------------------------------------
// <copyright file="IFileFormat.cs" company="">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace UnityEngine.U2D.Animation.TriangleNet
    .IO
{
    internal interface IFileFormat
    {
        bool IsSupported(string file);
    }
}
