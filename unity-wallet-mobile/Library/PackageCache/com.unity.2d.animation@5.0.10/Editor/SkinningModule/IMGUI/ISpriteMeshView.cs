using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal enum SpriteMeshViewMode
    {
        EditGeometry,
        CreateVertex,
        CreateEdge,
        SplitEdge
    }

    internal enum MeshEditorAction
    {
        None,
        CreateVertex,
        MoveVertex,
        CreateEdge,
        SplitEdge,
        MoveEdge,
        SelectVertex,
        SelectEdge,
        Remove
    }

    internal interface ISpriteMeshView
    {
        SpriteMeshViewMode mode { get; set; }
        ISelection<int> selection { get; set; }
        int defaultControlID { get; set; }
        Rect frame { get; set; }
        Vector2 mouseWorldPosition { get; }
        int hoveredVertex { get; }
        int hoveredEdge { get; }
        int closestEdge { get; }

        void CancelMode();
        void BeginLayout();
        void EndLayout();
        void LayoutVertex(Vector2 position, int index);
        void LayoutEdge(Vector2 startPosition, Vector2 endPosition, int index);
        bool DoCreateVertex();
        bool DoSelectVertex(out bool additive);
        bool DoMoveVertex(out Vector2 delta);
        bool DoMoveEdge(out Vector2 delta);
        bool DoCreateEdge();
        bool DoSplitEdge();
        bool DoSelectEdge(out bool additive);
        bool DoRemove();
        void DrawVertex(Vector2 position);
        void DrawVertexHovered(Vector2 position);
        void DrawVertexSelected(Vector2 position);
        void BeginDrawEdges();
        void EndDrawEdges();
        void DrawEdge(Vector2 startPosition, Vector2 endPosition);
        void DrawEdgeHovered(Vector2 startPosition, Vector2 endPosition);
        void DrawEdgeSelected(Vector2 startPosition, Vector2 endPosition);
        bool IsActionTriggered(MeshEditorAction action);
        bool IsActionActive(MeshEditorAction action);
        bool IsActionHot(MeshEditorAction action);
        Vector2 WorldToScreen(Vector2 position);
        void DoRepaint();
        bool CanRepaint();
        bool CanLayout();
    }
}
