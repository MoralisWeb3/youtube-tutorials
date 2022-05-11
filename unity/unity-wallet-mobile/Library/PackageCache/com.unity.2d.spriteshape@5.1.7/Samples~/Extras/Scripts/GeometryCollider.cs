using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.U2D;
using UnityEngine.U2D;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpriteShapeExtras
{

    [ExecuteAlways]
    public class GeometryCollider : MonoBehaviour
    {
        [SerializeField]
        bool m_UpdateCollider = false;
    
        int m_HashCode = 0;
    
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void Update()
        {
            if (m_UpdateCollider)
                Bake(gameObject, false);
        }
    
        static public void Bake(GameObject go, bool forced)
        {
            var spriteShapeController = go.GetComponent<SpriteShapeController>();
            var spriteShapeRenderer = go.GetComponent<SpriteShapeRenderer>();
            var polyCollider = go.GetComponent<PolygonCollider2D>();
            var geometryCollider = go.GetComponent<GeometryCollider>();
    
            if (spriteShapeController != null && polyCollider != null)
            {
                var spline = spriteShapeController.spline;
                if (geometryCollider != null)
                { 
                    int splineHashCode = spline.GetHashCode();
                    if (splineHashCode == geometryCollider.m_HashCode && !forced)
                        return;
                    geometryCollider.m_HashCode = splineHashCode;
                }
                NativeArray<ushort> indexArray;
                NativeSlice<Vector3> posArray;
                NativeSlice<Vector2> uv0Array;
                NativeArray<SpriteShapeSegment> geomArray;
                spriteShapeRenderer.GetChannels(65536, out indexArray, out posArray, out uv0Array);
                geomArray = spriteShapeRenderer.GetSegments(spline.GetPointCount() * 8);
    
                NativeArray<ushort> indexArrayLocal = new NativeArray<ushort>(indexArray.Length, Allocator.Temp);
    
                List<Vector2> points = new List<Vector2>();
                int indexCount = 0, vertexCount = 0, counter = 0;
                for (int u = 0; u < geomArray.Length; ++u)
                {
                    if (geomArray[u].indexCount > 0)
                    {
                        for (int i = 0; i < geomArray[u].indexCount; ++i)
                        {
                            indexArrayLocal[counter] = (ushort)(indexArray[counter] + vertexCount);
                            counter++;
                        }
                        vertexCount += geomArray[u].vertexCount;
                        indexCount += geomArray[u].indexCount;
                    }
                }
                Debug.Log(go.name + " : " + counter);
                OuterEdges(polyCollider, indexArrayLocal, posArray, indexCount);
            }
        }
    
        // Generate the outer edges from the Renderer mesh. Based on code from www.h3xed.com
        static void OuterEdges(PolygonCollider2D polygonCollider, NativeArray<ushort> triangles, NativeSlice<Vector3> vertices, int triangleCount)
        {
            // Get just the outer edges from the mesh's triangles (ignore or remove any shared edges)
            Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
            for (int i = 0; i < triangleCount; i += 3)
            {
                for (int e = 0; e < 3; e++)
                {
                    int vert1 = triangles[i + e];
                    int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                    string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
                    if (edges.ContainsKey(edge))
                    {
                        edges.Remove(edge);
                    }
                    else
                    {
                        edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
                    }
                }
            }
    
            // Create edge lookup (Key is first vertex, Value is second vertex, of each edge)
            Dictionary<int, int> lookup = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> edge in edges.Values)
            {
                if (lookup.ContainsKey(edge.Key) == false)
                {
                    lookup.Add(edge.Key, edge.Value);
                }
            }
    
            // Create empty polygon collider
            polygonCollider.pathCount = 0;
    
            // Loop through edge vertices in order
            int startVert = 0;
            int nextVert = startVert;
            int highestVert = startVert;
            List<Vector2> colliderPath = new List<Vector2>();
            while (true)
            {
    
                // Add vertex to collider path
                colliderPath.Add(vertices[nextVert]);
    
                // Get next vertex
                nextVert = lookup[nextVert];
    
                // Store highest vertex (to know what shape to move to next)
                if (nextVert > highestVert)
                {
                    highestVert = nextVert;
                }
    
                // Shape complete
                if (nextVert == startVert)
                {
    
                    // Add path to polygon collider
                    polygonCollider.pathCount++;
                    polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
                    colliderPath.Clear();
    
                    // Go to next shape if one exists
                    if (lookup.ContainsKey(highestVert + 1))
                    {
    
                        // Set starting and next vertices
                        startVert = highestVert + 1;
                        nextVert = startVert;
    
                        // Continue to next loop
                        continue;
                    }
    
                    // No more verts
                    break;
                }
            }
        }
    
    #if UNITY_EDITOR
    
        [MenuItem("SpriteShape/Generate Geometry Collider", false, 358)]
        public static void BakeGeometryCollider()
        {
            if (Selection.activeGameObject != null)
                GeometryCollider.Bake(Selection.activeGameObject, true);
        }
    
    #endif
    }

}