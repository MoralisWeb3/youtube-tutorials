using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal static class MathUtility
    {
        public static float DistanceToSegmentClamp(Vector3 p, Vector3 p1, Vector3 p2)
        {
            float l2 = (p2 - p1).sqrMagnitude;    // i.e. |b-a|^2 -  avoid a sqrt
            if (l2 == 0.0)
                return float.MaxValue;       // a == b case
            float t = Vector3.Dot(p - p1, p2 - p1) / l2;
            if (t < 0.0)
                return float.MaxValue;       // Beyond the 'a' end of the segment
            if (t > 1.0)
                return float.MaxValue;         // Beyond the 'b' end of the segment
            Vector3 projection = p1 + t * (p2 - p1); // Projection falls on the segment
            return (p - projection).magnitude;
        }

        public static Vector2 ClampPositionToRect(Vector2 position, Rect rect)
        {
            return new Vector2(Mathf.Clamp(position.x, rect.xMin, rect.xMax), Mathf.Clamp(position.y, rect.yMin, rect.yMax));
        }

        public static Vector2 MoveRectInsideFrame(Rect rect, Rect frame, Vector2 delta)
        {
            if (frame.size.x <= rect.size.x)
                delta.x = 0f;

            if (frame.size.y <= rect.size.y)
                delta.y = 0f;

            Vector2 min = rect.min + delta;
            Vector2 max = rect.max + delta;
            Vector2 size = rect.size;
            Vector2 position = rect.position;

            max.x = Mathf.Clamp(max.x, frame.min.x, frame.max.x);
            max.y = Mathf.Clamp(max.y, frame.min.y, frame.max.y);

            min = max - size;

            min.x = Mathf.Clamp(min.x, frame.min.x, frame.max.x);
            min.y = Mathf.Clamp(min.y, frame.min.y, frame.max.y);

            max = min + size;

            rect.min = min;
            rect.max = max;

            delta = rect.position - position;

            return delta;
        }

        public static bool SegmentIntersection(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, ref Vector2 point)
        {
            Vector2 s1 = p1 - p0;
            Vector2 s2 = p3 - p2;

            float s, t, determinant;
            determinant = (s1.x * s2.y - s2.x * s1.y);

            if (Mathf.Approximately(determinant, 0f))
                return false;

            s = (-s1.y * (p0.x - p2.x) + s1.x * (p0.y - p2.y)) / determinant;
            t = (s2.x * (p0.y - p2.y) - s2.y * (p0.x - p2.x)) / determinant;

            if (s >= 0f && s <= 1f && t >= 0f && t <= 1f)
            {
                point = p0 + (t * s1);
                return true;
            }

            return false;
        }

        //https://gamedev.stackexchange.com/a/49370
        public static void Barycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c, out Vector3 coords)
        {
            Vector2 v0 = b - a, v1 = c - a, v2 = p - a;
            float d00 = Vector2.Dot(v0, v0);
            float d01 = Vector2.Dot(v0, v1);
            float d11 = Vector2.Dot(v1, v1);
            float d20 = Vector2.Dot(v2, v0);
            float d21 = Vector2.Dot(v2, v1);
            float invDenom = 1f / (d00 * d11 - d01 * d01);
            coords.y = (d11 * d20 - d01 * d21) * invDenom;
            coords.z = (d00 * d21 - d01 * d20) * invDenom;
            coords.x = 1f - coords.y - coords.z;
        }

        public static Quaternion NormalizeQuaternion(Quaternion q)
        {
            Vector4 v = new Vector4(q.x, q.y, q.z, q.w).normalized;
            return new Quaternion(v.x, v.y, v.z, v.w);
        }

        //From: https://answers.unity.com/questions/861719/a-fast-triangle-triangle-intersection-algorithm-fo.html
        public static bool Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray)
        {
            Vector3 e1, e2;  
            Vector3 p, q, t;
            float det, invDet, u, v;
            e1 = p2 - p1;
            e2 = p3 - p1;
            p = Vector3.Cross(ray.direction, e2);
            det = Vector3.Dot(e1, p);

            if (Mathf.Approximately(det, 0f))
                return false;

            invDet = 1.0f / det;

            t = ray.origin - p1;
            u = Vector3.Dot(t, p) * invDet;

            if (u < 0 || u > 1)
                return false;

            q = Vector3.Cross(t, e1);
            v = Vector3.Dot(ray.direction, q) * invDet;

            if (v < 0 || u + v > 1)
                return false;

            if ((Vector3.Dot(e2, q) * invDet) > 0f)
                return true;

            return false;
        }
    }
}
