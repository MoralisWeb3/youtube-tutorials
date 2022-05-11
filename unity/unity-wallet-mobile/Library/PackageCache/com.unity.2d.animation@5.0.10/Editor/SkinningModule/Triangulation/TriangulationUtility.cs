using UnityEngine;
using System.Collections.Generic;
using UnityEngine.U2D.Animation.TriangleNet.Geometry;
using UnityEngine.U2D.Animation.TriangleNet.Meshing;
using UnityEngine.U2D.Animation.TriangleNet.Smoothing;
using UnityEngine.U2D.Animation.TriangleNet.Tools;

namespace UnityEditor.U2D.Animation
{
    internal class TriangulationUtility
    {
        public static void Triangulate(IList<Vector2> vertices, IList<Edge> edges, IList<int> indices)
        {
            indices.Clear();

            if (vertices.Count < 3)
                return;

            var polygon = new Polygon(vertices.Count);

            for (int i = 0; i < vertices.Count; ++i)
            {
                Vector2 position = vertices[i];
                polygon.Add(new Vertex(position.x, position.y, 1));
            }

            for (int i = 0; i < edges.Count; ++i)
            {
                Edge edge = edges[i];
                polygon.Add(new Segment(polygon.Points[edge.index1], polygon.Points[edge.index2]));
            }

            var mesh = polygon.Triangulate();

            foreach (ITriangle triangle in mesh.Triangles)
            {
                int id0 = triangle.GetVertexID(0);
                int id1 = triangle.GetVertexID(1);
                int id2 = triangle.GetVertexID(2);

                if (id0 < 0 || id1 < 0 || id2 < 0 ||  id0 >= vertices.Count || id1 >= vertices.Count || id2 >= vertices.Count)
                    continue;

                indices.Add(id0);
                indices.Add(id2);
                indices.Add(id1);
            }
        }

        public static void Tessellate(float minAngle, float maxAngle, float meshAreaFactor, float largestTriangleAreaFactor, int smoothIterations, IList<Vector2> vertices, IList<Edge> edges, IList<int> indices)
        {
            if (vertices.Count < 3)
                return;

            largestTriangleAreaFactor = Mathf.Clamp01(largestTriangleAreaFactor);

            var polygon = new Polygon(vertices.Count);

            for (int i = 0; i < vertices.Count; ++i)
            {
                Vector2 position = vertices[i];
                polygon.Add(new Vertex(position.x, position.y, 1));
            }

            for (int i = 0; i < edges.Count; ++i)
            {
                Edge edge = edges[i];
                polygon.Add(new Segment(polygon.Points[edge.index1], polygon.Points[edge.index2]));
            }

            var mesh = polygon.Triangulate();
            var statistic = new Statistic();

            statistic.Update((UnityEngine.U2D.Animation.TriangleNet.Mesh)mesh, 1);

            if (statistic.LargestArea < 0.01f)
                throw new System.Exception("Invalid Mesh: Largest triangle area too small");

            var maxAreaToApply = (double)Mathf.Max((float)statistic.LargestArea * largestTriangleAreaFactor, (float)(statistic.MeshArea * meshAreaFactor));
            var qualityOptions = new QualityOptions() { SteinerPoints = 0 };

            if (maxAreaToApply > 0f)
                qualityOptions.MaximumArea = maxAreaToApply;

            qualityOptions.MinimumAngle = minAngle;
            qualityOptions.MaximumAngle = maxAngle;

            mesh.Refine(qualityOptions, false);
            mesh.Renumber();

            if (smoothIterations > 0)
            {
                try
                {
                    var smoother = new SimpleSmoother();
                    smoother.Smooth(mesh, smoothIterations);
                }
                catch (System.Exception)
                {
                    Debug.Log(TextContent.smoothMeshError);
                }
            }

            vertices.Clear();
            edges.Clear();
            indices.Clear();

            foreach (Vertex vertex in mesh.Vertices)
            {
                vertices.Add(new Vector2((float)vertex.X, (float)vertex.Y));
            }

            foreach (ISegment segment in mesh.Segments)
            {
                edges.Add(new Edge(segment.P0, segment.P1));
            }

            foreach (ITriangle triangle in mesh.Triangles)
            {
                int id0 = triangle.GetVertexID(0);
                int id1 = triangle.GetVertexID(1);
                int id2 = triangle.GetVertexID(2);

                if (id0 < 0 || id1 < 0 || id2 < 0 ||  id0 >= vertices.Count || id1 >= vertices.Count || id2 >= vertices.Count)
                    continue;

                indices.Add(id0);
                indices.Add(id2);
                indices.Add(id1);
            }
        }
    }
}
