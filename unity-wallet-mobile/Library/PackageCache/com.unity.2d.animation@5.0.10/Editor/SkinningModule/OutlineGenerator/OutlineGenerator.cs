using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.U2D.Common;
using UnityEditor.U2D.Animation.ClipperLib;
using UnityEditor.U2D.Sprites;

namespace UnityEditor.U2D.Animation
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    internal class OutlineGenerator : IOutlineGenerator
    {
        const double kClipperScale = 1000.0;

        private const float kEpsilon = 1.2e-12f;
        private const float kMinLinearizeDistance = 5f;
        private Texture2D m_CurrentTexture;
        private Rect m_CurrentRect;
        private byte m_CurrentAlphaTolerance;

        public void GenerateOutline(ITextureDataProvider textureDataProvider, Rect rect, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths)
        {
            if (alphaTolerance >= 255)
                throw new ArgumentException("Alpha tolerance should be lower than 255");

            m_CurrentTexture = textureDataProvider.GetReadableTexture2D();
            m_CurrentRect = rect;
            m_CurrentAlphaTolerance = alphaTolerance;

            InternalEditorBridge.GenerateOutline(textureDataProvider.texture, rect, 1f, alphaTolerance, holeDetection, out paths);

            if (paths.Length > 0)
            {
                ClipPaths(ref paths);

                Debug.Assert(paths.Length > 0);

                var rectSizeMagnitude = rect.size.magnitude;
                var minDistance = Mathf.Max(rectSizeMagnitude / 10f, kMinLinearizeDistance);
                var maxDistance = Mathf.Max(rectSizeMagnitude / 100f, kMinLinearizeDistance);
                var distance = Mathf.Lerp(minDistance, maxDistance, detail);

                for (var pathIndex = 0; pathIndex < paths.Length; ++pathIndex)
                {
                    var pathLength = CalculatePathLength(paths[pathIndex]);
                    if (pathLength > distance)
                    {
                        var newPath = Linearize(new List<Vector2>(paths[pathIndex]), distance);

                        if (newPath.Count > 3)
                            paths[pathIndex] = newPath.ToArray();

                        SmoothPath(paths[pathIndex], 5, 0.1f, 135f);
                    }
                }

                ClipPaths(ref paths);
            }
        }

        private void ClipPaths(ref Vector2[][] paths)
        {
            Debug.Assert(paths.Length > 0);

            var subj = ToClipper(paths);
            var solution = new Paths();
            var clipper = new Clipper(Clipper.ioPreserveCollinear);

            clipper.AddPaths(subj, PolyType.ptSubject, true);
            clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);
            FilterNestedPaths(solution);
            paths = ToVector2(solution);
        }

        private void FilterNestedPaths(Paths paths)
        {
            var filtered = new List<Path>(paths);

            for (var i = 0; i < paths.Count; ++i)
            {
                var path = paths[i];

                if (!filtered.Contains(path))
                    continue;

                for (var j = i + 1; j < paths.Count; ++j)
                {
                    if (!filtered.Contains(path))
                        continue;

                    var other = paths[j];

                    if (IsPathContainedInOtherPath(path, other))
                    {
                        filtered.Remove(path);
                        break;
                    }
                    else if (IsPathContainedInOtherPath(other, path))
                        filtered.Remove(other);
                }
            }

            paths.Clear();
            paths.AddRange(filtered);
        }

        private bool IsPathContainedInOtherPath(Path path, Path other)
        {
            foreach (var p in path)
            {
                if (Clipper.PointInPolygon(p, other) < 1)
                    return false;
            }

            return true;
        }

        private Paths ToClipper(Vector2[][] paths)
        {
            return new Paths(Array.ConvertAll(paths, p => ToClipper(p)));
        }

        private Path ToClipper(Vector2[] path)
        {
            return new Path(Array.ConvertAll(path, p => new IntPoint(p.x * kClipperScale, p.y * kClipperScale)));
        }

        private Vector2[][] ToVector2(Paths paths)
        {
            return paths.ConvertAll(p => ToVector2(p)).ToArray();
        }

        private Vector2[] ToVector2(Path path)
        {
            return path.ConvertAll(p => new Vector2((float)(p.X / kClipperScale), (float)(p.Y / kClipperScale))).ToArray();
        }

        private float CalculatePathLength(Vector2[] path)
        {
            var sum = 0f;
            for (var i = 0; i < path.Length; i++)
            {
                var nextIndex = NextIndex(i, path.Length);
                var p0 = path[i];
                var p1 = path[nextIndex];
                sum += Vector2.Distance(p0, p1);
            }
            return sum;
        }

        //Adapted from https://github.com/burningmime/curves
        private List<Vector2> Linearize(List<Vector2> src, float pointDistance)
        {
            if (src == null) throw new ArgumentNullException("src");
            if (pointDistance <= kEpsilon) throw new InvalidOperationException("pointDistance " + pointDistance + " is less than epislon " + kEpsilon);

            var dst = new List<Vector2>();

            if (src.Count > 0)
            {
                var accDistance = 0f;
                var lastIndex = 0;
                var lastPoint = src[0];

                dst.Add(lastPoint);

                for (var i = 0; i < src.Count; i++)
                {
                    var nextIndex = NextIndex(i, src.Count);
                    var p0 = src[i];
                    var p1 = src[nextIndex];
                    var edgeDistance = Vector2.Distance(p0, p1);

                    if (accDistance + edgeDistance > pointDistance || nextIndex == 0)
                    {
                        var partialDistance = pointDistance - accDistance;
                        var newPoint = Vector2.Lerp(p0, p1, partialDistance / edgeDistance);
                        var remainingDistance = edgeDistance - partialDistance;

                        //Roll back until we do not intersect any pixel
                        var step = 1f;
                        bool finish = false;
                        while (!finish && IsLineOverImage(newPoint, lastPoint))
                        {
                            partialDistance = Vector2.Distance(p0, newPoint) - step;

                            while (partialDistance < 0f)
                            {
                                if (i > lastIndex + 1)
                                {
                                    accDistance -= edgeDistance;
                                    --i;
                                    p1 = p0;
                                    p0 = src[i];
                                    edgeDistance = Vector2.Distance(p0, p1);
                                    partialDistance += edgeDistance;
                                }
                                else
                                {
                                    partialDistance = 0f;
                                    finish = true;
                                }

                                remainingDistance = edgeDistance - partialDistance;
                            }

                            newPoint = Vector2.Lerp(p0, p1, partialDistance / edgeDistance);
                        }

                        Debug.Assert(lastIndex <= i, "Generate Outline failed");

                        nextIndex = NextIndex(i, src.Count);
                        if (nextIndex != 0 || !EqualsOrClose(newPoint, p1))
                        {
                            dst.Add(newPoint);
                            lastPoint = newPoint;
                            lastIndex = i;
                        }

                        while (remainingDistance > pointDistance)
                        {
                            remainingDistance -= pointDistance;
                            newPoint = Vector2.Lerp(p0, p1, (edgeDistance - remainingDistance) / edgeDistance);
                            if (!EqualsOrClose(newPoint, lastPoint))
                            {
                                dst.Add(newPoint);
                                lastPoint = newPoint;
                            }
                        }
                        accDistance = remainingDistance;
                    }
                    else
                    {
                        accDistance += edgeDistance;
                    }
                }
            }
            return dst;
        }

        private bool EqualsOrClose(Vector2 v1, Vector2 v2)
        {
            return (v1 - v2).sqrMagnitude < kEpsilon;
        }

        private void SmoothPath(Vector2[] path, int iterations, float velocity, float minAngle)
        {
            Debug.Assert(iterations > 0f);
            Debug.Assert(minAngle >= 0f);
            Debug.Assert(minAngle < 180f);

            var cosTolerance = Mathf.Cos(minAngle * Mathf.Deg2Rad);

            for (int iteration = 0; iteration < iterations; ++iteration)
                for (int i = 0; i < path.Length; ++i)
                {
                    var prevPoint = path[PreviousIndex(i, path.Length)];
                    var point = path[i];
                    var nextPoint = path[NextIndex(i, path.Length)];

                    var t1 = prevPoint - point;
                    var t2 = nextPoint - point;

                    var dot = Vector2.Dot(t1.normalized, t2.normalized);

                    if (dot > cosTolerance)
                        continue;

                    var w1 = 1f / (point - prevPoint).magnitude;
                    var w2 = 1f / (point - nextPoint).magnitude;
                    var laplacian = (w1 * prevPoint + w2 * nextPoint) / (w1 + w2) - point;
                    point += laplacian * velocity;

                    if (!IsLineOverImage(point, nextPoint) && !IsLineOverImage(point, prevPoint))
                        path[i] = point;
                }
        }

        private Vector2Int ToVector2Int(Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        private bool IsLineOverImage(Vector2 pointA, Vector2 pointB)
        {
            var pointAInt = ToVector2Int(pointA);
            var pointBInt = ToVector2Int(pointB);

            if (IsPointInRectEdge(pointA) && IsPointInRectEdge(pointB) && (pointAInt.x == pointBInt.x || pointAInt.y == pointBInt.y))
                return false;

            foreach (var point in GetPointsOnLine(pointAInt.x, pointAInt.y, pointBInt.x, pointBInt.y))
            {
                if (IsPointOverImage(point))
                    return true;
            }

            return false;
        }

        private bool IsPointOverImage(Vector2 point)
        {
            Debug.Assert(m_CurrentTexture != null);
            point += m_CurrentRect.center;
            return m_CurrentTexture.GetPixel((int)point.x, (int)point.y).a * 255 > m_CurrentAlphaTolerance;
        }

        private bool IsPointInRectEdge(Vector2 point)
        {
            point += m_CurrentRect.center;
            var pointInt = ToVector2Int(point);
            var minInt = ToVector2Int(m_CurrentRect.min);
            var maxInt = ToVector2Int(m_CurrentRect.max);
            return minInt.x >= pointInt.x || maxInt.x <= pointInt.x || minInt.y >= pointInt.y || maxInt.y <= pointInt.y;
        }

        //From http://ericw.ca/notes/bresenhams-line-algorithm-in-csharp.html
        private IEnumerable<Vector2Int> GetPointsOnLine(int x0, int y0, int x1, int y1)
        {
            bool steep = Mathf.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Mathf.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new Vector2Int((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            yield break;
        }

        private int NextIndex(int index, int pointCount)
        {
            return Mod(index + 1, pointCount);
        }

        private int PreviousIndex(int index, int pointCount)
        {
            return Mod(index - 1, pointCount);
        }

        private int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}
