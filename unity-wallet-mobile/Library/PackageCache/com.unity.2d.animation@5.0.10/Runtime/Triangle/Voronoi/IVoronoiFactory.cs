namespace UnityEngine.U2D.Animation.TriangleNet
    .Voronoi
{
    using Animation.TriangleNet.Topology.DCEL;

    internal interface IVoronoiFactory
    {
        void Initialize(int vertexCount, int edgeCount, int faceCount);

        void Reset();

        Vertex CreateVertex(double x, double y);

        HalfEdge CreateHalfEdge(Vertex origin, Face face);

        Face CreateFace(Geometry.Vertex vertex);
    }
}
