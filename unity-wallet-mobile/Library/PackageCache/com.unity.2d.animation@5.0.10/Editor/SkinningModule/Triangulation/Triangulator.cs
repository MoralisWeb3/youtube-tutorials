using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class Triangulator : ITriangulator
    {
        public void Triangulate(IList<Vector2> vertices, IList<Edge> edges, IList<int> indices)
        {
            TriangulationUtility.Triangulate(vertices, edges, indices);
        }

        public void Tessellate(float minAngle, float maxAngle, float meshAreaFactor, float largestTriangleAreaFactor, int smoothIterations, IList<Vector2> vertices, IList<Edge> edges, IList<int> indices)
        {
            TriangulationUtility.Tessellate(minAngle, maxAngle, meshAreaFactor, largestTriangleAreaFactor, smoothIterations, vertices, edges, indices);
        }
    }
}
