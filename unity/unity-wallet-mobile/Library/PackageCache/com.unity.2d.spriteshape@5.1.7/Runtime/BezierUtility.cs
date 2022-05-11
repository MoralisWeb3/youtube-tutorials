
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.U2D
{
    /// <summary>
    /// Utility functions for Bezier.
    /// </summary>
    public static class BezierUtility
    {
        static Vector3[] s_TempPoints = new Vector3[3];
        
        /// <summary>
        /// Get the BezierPoint position between two control points.
        /// </summary>
        /// <param name="startRightTangent">Right tangent of the first control point.</param>
        /// <param name="startPosition">Position of the first control point.</param>
        /// <param name="endPosition">Position of the second control point</param>
        /// <param name="endLeftTangent">Left tangent of the second control point.</param>
        /// <param name="t">Interval in the range of (0, 1)</param>
        /// <returns>Intermediate position</returns>
        public static Vector3 BezierPoint(Vector3 startRightTangent, Vector3 startPosition, Vector3 endPosition, Vector3 endLeftTangent, float t)
        {
            float s = 1.0f - t;
            return s * s * s * startRightTangent + 3.0f * s * s * t * startPosition + 3.0f * s * t * t * endPosition + t * t * t * endLeftTangent;
        }        
        internal static float GetSpritePixelWidth(Sprite sprite)
        {
            float4 meta = new float4(sprite.pixelsPerUnit, sprite.pivot.y / sprite.textureRect.height, sprite.rect.width, sprite.rect.height);
            float4 border = new float4(sprite.border.x, sprite.border.y, sprite.border.z, sprite.border.w);
            float rpunits = 1.0f / meta.x;
            float2 whsize = new float2(meta.z, meta.w) * rpunits;
                            
            border = border * rpunits;
            float stPixelU = border.x;
            float enPixelU = whsize.x - border.z;
            float pxlWidth = enPixelU - stPixelU;
            return pxlWidth;
        }
        
        internal static float BezierLength(NativeArray<ShapeControlPoint> shapePoints, int splineDetail, ref float smallestSegment)
        {
            // Expand the Bezier.
            int controlPointContour = shapePoints.Length - 1;
            float spd = 0;
            float fmax = (float)(splineDetail - 1);
            for (int i = 0; i < controlPointContour; ++i)
            {
                int j = i + 1;
                ShapeControlPoint cp = shapePoints[i];
                ShapeControlPoint pp = shapePoints[j];

                Vector3 p0 = cp.position;
                Vector3 p1 = pp.position;
                Vector3 sp = p0;
                Vector3 rt = p0 + cp.rightTangent;
                Vector3 lt = p1 + pp.leftTangent;

                for (int n = 1; n < splineDetail; ++n)
                {
                    float t = (float)n / fmax;
                    Vector3 bp = BezierPoint(rt, p0, p1, lt, t);
                    float d = math.distance(bp, sp);
                    smallestSegment = math.min(d, smallestSegment);
                    spd += d;
                    sp = bp;
                }
            }

            return spd;
        }

        public static Vector3 ClosestPointOnCurve(Vector3 point, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, out float t)
        {
            Vector3 startToEnd = endPosition - startPosition;
            Vector3 startToTangent = (startTangent - startPosition);
            Vector3 endToTangent = (endTangent - endPosition);

            float sqrError = 0.001f;

            if (Colinear(startToTangent, startToEnd, sqrError) && Colinear(endToTangent, startToEnd, sqrError))
                return ClosestPointToSegment(point, startPosition, endPosition, out t);

            Vector3 leftStartPosition;
            Vector3 leftEndPosition;
            Vector3 leftStartTangent;
            Vector3 leftEndTangent;

            Vector3 rightStartPosition;
            Vector3 rightEndPosition;
            Vector3 rightStartTangent;
            Vector3 rightEndTangent;

            float leftStartT = 0f;
            float leftEndT = 0.5f;
            float rightStartT = 0.5f;
            float rightEndT = 1f;

            SplitBezier(0.5f, startPosition, endPosition, startTangent, endTangent,
                out leftStartPosition, out leftEndPosition, out leftStartTangent, out leftEndTangent,
                out rightStartPosition, out rightEndPosition, out rightStartTangent, out rightEndTangent);

            Vector3 pointLeft = ClosestPointOnCurveIterative(point, leftStartPosition, leftEndPosition, leftStartTangent, leftEndTangent, sqrError, ref leftStartT, ref leftEndT);
            Vector3 pointRight = ClosestPointOnCurveIterative(point, rightStartPosition, rightEndPosition, rightStartTangent, rightEndTangent, sqrError, ref rightStartT, ref rightEndT);

            if ((point - pointLeft).sqrMagnitude < (point - pointRight).sqrMagnitude)
            {
                t = leftStartT;
                return pointLeft;
            }

            t = rightStartT;
            return pointRight;
        }

        public static Vector3 ClosestPointOnCurveFast(Vector3 point, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, out float t)
        {
            float sqrError = 0.001f;
            float startT = 0f;
            float endT = 1f;

            Vector3 closestPoint = ClosestPointOnCurveIterative(point, startPosition, endPosition, startTangent, endTangent, sqrError, ref startT, ref endT);

            t = startT;

            return closestPoint;
        }

        private static Vector3 ClosestPointOnCurveIterative(Vector3 point, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, float sqrError, ref float startT, ref float endT)
        {
            while ((startPosition - endPosition).sqrMagnitude > sqrError)
            {
                Vector3 startToEnd = endPosition - startPosition;
                Vector3 startToTangent = (startTangent - startPosition);
                Vector3 endToTangent = (endTangent - endPosition);

                if (Colinear(startToTangent, startToEnd, sqrError) && Colinear(endToTangent, startToEnd, sqrError))
                {
                    float t;
                    Vector3 closestPoint = ClosestPointToSegment(point, startPosition, endPosition, out t);
                    t *= (endT - startT);
                    startT += t;
                    endT -= t;
                    return closestPoint;
                }

                Vector3 leftStartPosition;
                Vector3 leftEndPosition;
                Vector3 leftStartTangent;
                Vector3 leftEndTangent;

                Vector3 rightStartPosition;
                Vector3 rightEndPosition;
                Vector3 rightStartTangent;
                Vector3 rightEndTangent;

                SplitBezier(0.5f, startPosition, endPosition, startTangent, endTangent,
                    out leftStartPosition, out leftEndPosition, out leftStartTangent, out leftEndTangent,
                    out rightStartPosition, out rightEndPosition, out rightStartTangent, out rightEndTangent);

                s_TempPoints[0] = leftStartPosition;
                s_TempPoints[1] = leftStartTangent;
                s_TempPoints[2] = leftEndTangent;

                float sqrDistanceLeft = SqrDistanceToPolyLine(point, s_TempPoints);

                s_TempPoints[0] = rightEndPosition;
                s_TempPoints[1] = rightEndTangent;
                s_TempPoints[2] = rightStartTangent;

                float sqrDistanceRight = SqrDistanceToPolyLine(point, s_TempPoints);

                if (sqrDistanceLeft < sqrDistanceRight)
                {
                    startPosition = leftStartPosition;
                    endPosition = leftEndPosition;
                    startTangent = leftStartTangent;
                    endTangent = leftEndTangent;

                    endT -= (endT - startT) * 0.5f;
                }
                else
                {
                    startPosition = rightStartPosition;
                    endPosition = rightEndPosition;
                    startTangent = rightStartTangent;
                    endTangent = rightEndTangent;

                    startT += (endT - startT) * 0.5f;
                }
            }

            return endPosition;
        }

        public static void SplitBezier(float t, Vector3 startPosition, Vector3 endPosition, Vector3 startRightTangent, Vector3 endLeftTangent,
            out Vector3 leftStartPosition, out Vector3 leftEndPosition, out Vector3 leftStartTangent, out Vector3 leftEndTangent,
            out Vector3 rightStartPosition, out Vector3 rightEndPosition, out Vector3 rightStartTangent, out Vector3 rightEndTangent)
        {
            Vector3 tangent0 = (startRightTangent - startPosition);
            Vector3 tangent1 = (endLeftTangent - endPosition);
            Vector3 tangentEdge = (endLeftTangent - startRightTangent);

            Vector3 tangentPoint0 = startPosition + tangent0 * t;
            Vector3 tangentPoint1 = endPosition + tangent1 * (1f - t);
            Vector3 tangentEdgePoint = startRightTangent + tangentEdge * t;

            Vector3 newTangent0 = tangentPoint0 + (tangentEdgePoint - tangentPoint0) * t;
            Vector3 newTangent1 = tangentPoint1 + (tangentEdgePoint - tangentPoint1) * (1f - t);
            Vector3 newTangentEdge = newTangent1 - newTangent0;

            Vector3 bezierPoint = newTangent0 + newTangentEdge * t;

            leftStartPosition = startPosition;
            leftEndPosition = bezierPoint;
            leftStartTangent = tangentPoint0;
            leftEndTangent = newTangent0;

            rightStartPosition = bezierPoint;
            rightEndPosition = endPosition;
            rightStartTangent = newTangent1;
            rightEndTangent = tangentPoint1;
        }

        private static Vector3 ClosestPointToSegment(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd, out float t)
        {
            Vector3 relativePoint = point - segmentStart;
            Vector3 segment = (segmentEnd - segmentStart);
            Vector3 segmentDirection = segment.normalized;
            float length = segment.magnitude;

            float dot = Vector3.Dot(relativePoint, segmentDirection);

            if (dot <= 0f)
                dot = 0f;
            else if (dot >= length)
                dot = length;

            t = dot / length;

            return segmentStart + segment * t;
        }

        private static float SqrDistanceToPolyLine(Vector3 point, Vector3[] points)
        {
            float minDistance = float.MaxValue;

            for (int i = 0; i < points.Length - 1; ++i)
            {
                float distance = SqrDistanceToSegment(point, points[i], points[i + 1]);

                if (distance < minDistance)
                    minDistance = distance;
            }

            return minDistance;
        }

        private static float SqrDistanceToSegment(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            Vector3 relativePoint = point - segmentStart;
            Vector3 segment = (segmentEnd - segmentStart);
            Vector3 segmentDirection = segment.normalized;
            float length = segment.magnitude;

            float dot = Vector3.Dot(relativePoint, segmentDirection);

            if (dot <= 0f)
                return (point - segmentStart).sqrMagnitude;
            else if (dot >= length)
                return (point - segmentEnd).sqrMagnitude;

            return Vector3.Cross(relativePoint, segmentDirection).sqrMagnitude;
        }

        private static bool Colinear(Vector3 v1, Vector3 v2, float error = 0.0001f)
        {
            return Mathf.Abs(v1.x * v2.y - v1.y * v2.x + v1.x * v2.z - v1.z * v2.x + v1.y * v2.z - v1.z * v2.y) < error;
        }
    }
}
