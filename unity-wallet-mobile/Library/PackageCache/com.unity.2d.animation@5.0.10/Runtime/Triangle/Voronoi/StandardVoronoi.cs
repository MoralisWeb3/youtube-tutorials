// -----------------------------------------------------------------------
// <copyright file="StandardVoronoi.cs">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace UnityEngine.U2D.Animation.TriangleNet
    .Voronoi
{
    using System.Collections.Generic;
    using Animation.TriangleNet.Geometry;
    using Animation.TriangleNet.Tools;
    using Animation.TriangleNet.Topology.DCEL;

    internal class StandardVoronoi : VoronoiBase
    {
        public StandardVoronoi(Mesh mesh)
            : this(mesh, mesh.bounds, new DefaultVoronoiFactory(), RobustPredicates.Default)
        {
        }

        public StandardVoronoi(Mesh mesh, Rectangle box)
            : this(mesh, box, new DefaultVoronoiFactory(), RobustPredicates.Default)
        {
        }

        public StandardVoronoi(Mesh mesh, Rectangle box, IVoronoiFactory factory, IPredicates predicates)
            : base(mesh, factory, predicates, true)
        {
            // We assume the box to be at least as large as the mesh.
            box.Expand(mesh.bounds);

            // We explicitly told the base constructor to call the Generate method, so
            // at this point the basic Voronoi diagram is already created.
            PostProcess(box);
        }

        /// <summary>
        /// Compute edge intersections with bounding box.
        /// </summary>
        private void PostProcess(Rectangle box)
        {
            foreach (var edge in rays)
            {
                // The vertices of the infinite edge.
                var v1 = (Point)edge.origin;
                var v2 = (Point)edge.twin.origin;

                if (box.Contains(v1) || box.Contains(v2))
                {
                    // Move infinite vertex v2 onto the box boundary.
                    IntersectionHelper.BoxRayIntersection(box, v1, v2, ref v2);
                }
                else
                {
                    // There is actually no easy way to handle the second case. The two edges
                    // leaving v1, pointing towards the mesh, don't have to intersect the box
                    // (the could join with edges of other cells outside the box).

                    // A general intersection algorithm (DCEL <-> Rectangle) is needed, which
                    // computes intersections with all edges and discards objects outside the
                    // box.
                }
            }
        }
    }
}
