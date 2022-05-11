using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using ExtrasClipperLib;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace SpriteShapeExtras
{

    public enum ColliderCornerType
    {
        Square,
        Round,
        Sharp
    }

    [ExecuteAlways]
    public class LegacyCollider : MonoBehaviour
    {
        [SerializeField]
        ColliderCornerType m_ColliderCornerType = ColliderCornerType.Square;
        [SerializeField]
        float m_ColliderOffset = 1.0f;
        [SerializeField]
        bool m_UpdateCollider = false;
    
        const float s_ClipperScale = 100000.0f;
        int m_HashCode = 0;
    
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        private static int NextIndex(int index, int pointCount)
        {
            return Mod(index + 1, pointCount);
        }
    
        private static int PreviousIndex(int index, int pointCount)
        {
            return Mod(index - 1, pointCount);
        }
    
        private static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }    
    
        // Update is called once per frame
        void Update()
        {
            if (m_UpdateCollider)
                Bake(gameObject, false);
        }
    
        static void SampleCurve(float colliderDetail, Vector3 startPoint, Vector3 startTangent, Vector3 endPoint, Vector3 endTangent, ref List<IntPoint> path)
        {
            
            if (startTangent.sqrMagnitude > 0f || endTangent.sqrMagnitude > 0f)
            {
                for (int j = 0; j <= colliderDetail; ++j)
                {
                    float t = j / (float)colliderDetail;
                    Vector3 newPoint = BezierUtility.BezierPoint(startPoint, startTangent + startPoint, endTangent + endPoint, endPoint, t) * s_ClipperScale;
    
                    path.Add(new IntPoint((System.Int64)newPoint.x, (System.Int64)newPoint.y));
                }
            }
            else
            {
                Vector3 newPoint = startPoint * s_ClipperScale;
                path.Add(new IntPoint((System.Int64)newPoint.x, (System.Int64)newPoint.y));
    
                newPoint = endPoint * s_ClipperScale;
                path.Add(new IntPoint((System.Int64)newPoint.x, (System.Int64)newPoint.y));
            }
        }
    
        public static void Bake(GameObject go, bool forced)
        {
            var sc = go.GetComponent<SpriteShapeController>();
            var lc = go.GetComponent<LegacyCollider>();
    
            if (sc != null)
            {
                List<IntPoint> path = new List<IntPoint>();
                int splinePointCount = sc.spline.GetPointCount();
                int pathPointCount = splinePointCount;
    
                ColliderCornerType cct = ColliderCornerType.Square;
                float co = 1.0f;
    
                if (lc != null)
                { 
                    int hashCode = sc.spline.GetHashCode() + lc.m_ColliderCornerType.GetHashCode() + lc.m_ColliderOffset.GetHashCode();
                    if (lc.m_HashCode == hashCode && !forced)
                        return;
    
                    lc.m_HashCode = hashCode;
                    cct = lc.m_ColliderCornerType;
                    co = lc.m_ColliderOffset;
                }
    
                if (sc.spline.isOpenEnded)
                    pathPointCount--;
    
                for (int i = 0; i < pathPointCount; ++i)
                {
                    int nextIndex = NextIndex(i, splinePointCount);
                    SampleCurve(sc.colliderDetail, sc.spline.GetPosition(i), sc.spline.GetRightTangent(i), sc.spline.GetPosition(nextIndex), sc.spline.GetLeftTangent(nextIndex), ref path);
                }
    
                if (co != 0f)
                {
                    List<List<IntPoint>> solution = new List<List<IntPoint>>();
                    ClipperOffset clipOffset = new ClipperOffset();
    
                    EndType endType = EndType.etClosedPolygon;
    
                    if (sc.spline.isOpenEnded)
                    {
                        endType = EndType.etOpenSquare;
    
                        if (cct == ColliderCornerType.Round)
                            endType = EndType.etOpenRound;
                    }
    
                    clipOffset.ArcTolerance = 200f / sc.colliderDetail;
                    clipOffset.AddPath(path, (ExtrasClipperLib.JoinType)cct, endType);
                    clipOffset.Execute(ref solution, s_ClipperScale * co);
    
                    if (solution.Count > 0)
                        path = solution[0];
                }
    
                List<Vector2> pathPoints = new List<Vector2>(path.Count);
                for (int i = 0; i < path.Count; ++i)
                {
                    IntPoint ip = path[i];
                    pathPoints.Add(new Vector2(ip.X / s_ClipperScale, ip.Y / s_ClipperScale));
                }
    
                var pc = go.GetComponent<PolygonCollider2D>();
                if (pc)
                {
                    pc.pathCount = 0;
                    pc.SetPath(0, pathPoints.ToArray());
                }
    
                var ec = go.GetComponent<EdgeCollider2D>();
                if (ec)
                {
                    if (co > 0f || co < 0f && !sc.spline.isOpenEnded)
                        pathPoints.Add(pathPoints[0]);
                    ec.points = pathPoints.ToArray();
                }
            }
        }
    
    #if UNITY_EDITOR
    
        [MenuItem("SpriteShape/Generate Legacy Collider", false, 358)]
        public static void BakeLegacyCollider()
        {
            if (Selection.activeGameObject != null)
                LegacyCollider.Bake(Selection.activeGameObject, true);
        }
    
    #endif
    }

}
