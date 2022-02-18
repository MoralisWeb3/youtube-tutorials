using System;
using System.Collections.Generic;

namespace UnityEngine.U2D
{
    // Spline Internal Meta Data.
    internal struct SplinePointMetaData
    {
        public float height;
        public uint spriteIndex;
        public int cornerMode;
    };    
    
    /// <summary>
    /// Spline contains control points used to define curve/outline for generating SpriteShape geometry.
    /// </summary>
    [Serializable]
    public class Spline
    {
        private static readonly string KErrorMessage = "Internal error: Point too close to neighbor";
        private static readonly float KEpsilon = 0.01f;
        [SerializeField]
        private bool m_IsOpenEnded;
        [SerializeField]
        private List<SplineControlPoint> m_ControlPoints = new List<SplineControlPoint>();

        /// <summary>
        /// Get/Set Spline's shape to open ended or closed.
        /// </summary>
        public bool isOpenEnded
        {
            get
            {
                if (GetPointCount() < 3)
                    return true;

                return m_IsOpenEnded;
            }
            set { m_IsOpenEnded = value; }
        }

        private bool IsPositionValid(int index, int next, Vector3 point)
        {
            int prev = (index == 0) ? (m_ControlPoints.Count - 1) : (index - 1);
            next = (next >= m_ControlPoints.Count) ? 0 : next;
            if (prev >= 0)
            {
                Vector3 diff = m_ControlPoints[prev].position - point;
                if (diff.magnitude < KEpsilon)
                    return false;
            }
            if (next < m_ControlPoints.Count)
            {
                Vector3 diff = m_ControlPoints[next].position - point;
                if (diff.magnitude < KEpsilon)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Clear all control points.
        /// </summary>
        public void Clear()
        {
            m_ControlPoints.Clear();
        }

        /// <summary>
        /// Get Spline's control point count.
        /// </summary>
        /// <returns>Count of control points.</returns>
        public int GetPointCount()
        {
            return m_ControlPoints.Count;
        }

        /// <summary>
        /// Insert control point at index.
        /// </summary>
        /// <param name="index">Index at which a control point will be inserted.</param>
        /// <param name="point">Position of the control point.</param>
        /// <exception cref="ArgumentException"></exception>
        public void InsertPointAt(int index, Vector3 point)
        {
            if (!IsPositionValid(index, index, point))
                throw new ArgumentException(KErrorMessage);
            m_ControlPoints.Insert(index, new SplineControlPoint { position = point, height = 1.0f, cornerMode = Corner.Automatic });
        }

        /// <summary>
        /// Remove a control point from the Spline at index.
        /// </summary>
        /// <param name="index">Index of the control point to be removed.</param>
        public void RemovePointAt(int index)
        {
            if (m_ControlPoints.Count > 2)
                m_ControlPoints.RemoveAt(index);
        }

        /// <summary>
        /// Get position of control point at index.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <returns></returns>
        public Vector3 GetPosition(int index)
        {
            return m_ControlPoints[index].position;
        }

        /// <summary>
        /// Set position of control point at index.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <param name="point">Position of control point.</param>
        /// <exception cref="ArgumentException"></exception>
        public void SetPosition(int index, Vector3 point)
        {
            if (!IsPositionValid(index, index + 1, point))
                throw new ArgumentException(KErrorMessage);
            SplineControlPoint newPoint = m_ControlPoints[index];
            newPoint.position = point;
            m_ControlPoints[index] = newPoint;
        }

        /// <summary>
        /// Get left tangent of control point at index.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <returns>Left tangent of control point.</returns>
        public Vector3 GetLeftTangent(int index)
        {
            ShapeTangentMode mode = GetTangentMode(index);

            if (mode == ShapeTangentMode.Linear)
                return Vector3.zero;

            return m_ControlPoints[index].leftTangent;
        }

        /// <summary>
        /// Set left tangent of control point at index.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <param name="tangent">Left tangent of control point.</param>
        public void SetLeftTangent(int index, Vector3 tangent)
        {
            ShapeTangentMode mode = GetTangentMode(index);

            if (mode == ShapeTangentMode.Linear)
                return;

            SplineControlPoint newPoint = m_ControlPoints[index];
            newPoint.leftTangent = tangent;
            m_ControlPoints[index] = newPoint;
        }

        /// <summary>
        /// Get right tangent of control point at index,
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <returns>Right tangent of control point.</returns>
        public Vector3 GetRightTangent(int index)
        {
            ShapeTangentMode mode = GetTangentMode(index);

            if (mode == ShapeTangentMode.Linear)
                return Vector3.zero;

            return m_ControlPoints[index].rightTangent;
        }

        /// <summary>
        /// Set right tangent of control point at index.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <param name="tangent">Right tangent of control point.</param>
        public void SetRightTangent(int index, Vector3 tangent)
        {
            ShapeTangentMode mode = GetTangentMode(index);

            if (mode == ShapeTangentMode.Linear)
                return;

            SplineControlPoint newPoint = m_ControlPoints[index];
            newPoint.rightTangent = tangent;
            m_ControlPoints[index] = newPoint;
        }

        /// <summary>
        /// Get tangent mode of control point at index.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <returns>Tangent mode of control point</returns>
        public ShapeTangentMode GetTangentMode(int index)
        {
            return m_ControlPoints[index].mode;
        }

        /// <summary>
        /// Set the tangent mode of control point at index.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <param name="mode">Tangent mode.</param>
        public void SetTangentMode(int index, ShapeTangentMode mode)
        {
            SplineControlPoint newPoint = m_ControlPoints[index];
            newPoint.mode = mode;
            m_ControlPoints[index] = newPoint;
        }

        /// <summary>
        /// Get height of control point at index.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <returns>Height.</returns>
        public float GetHeight(int index)
        {
            return m_ControlPoints[index].height;
        }

        /// <summary>
        /// Set height of control point at index.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <param name="value">Height.</param>
        public void SetHeight(int index, float value)
        {
            m_ControlPoints[index].height = value;
        }

        /// <summary>
        /// Get Sprite index to be used for rendering edge starting at control point.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <returns>Sprite index.</returns>
        public int GetSpriteIndex(int index)
        {
            return m_ControlPoints[index].spriteIndex;
        }

        /// <summary>
        /// Set Sprite index to be used for rendering edge starting at control point.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <param name="value">Sprite index.</param>
        public void SetSpriteIndex(int index, int value)
        {
            m_ControlPoints[index].spriteIndex = value;
        }

        /// <summary>
        /// Test if a corner mode is enabled at control point.  
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <returns>True if a valid corner mode is set.</returns>
        public bool GetCorner(int index)
        {
            return GetCornerMode(index) != Corner.Disable;
        }

        /// <summary>
        /// Set corner mode to automatic or disabled.
        /// </summary>
        /// <param name="index">Index of control point.</param>
        /// <param name="value">Enable/disable corner mode</param>
        public void SetCorner(int index, bool value)
        {
            m_ControlPoints[index].corner = value;
            m_ControlPoints[index].cornerMode = value ? Corner.Automatic : Corner.Disable;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)2166136261;

                for (int i = 0; i < GetPointCount(); ++i)
                {
                    hashCode = hashCode * 16777619 ^ m_ControlPoints[i].GetHashCode();
                }

                hashCode = hashCode * 16777619 ^ m_IsOpenEnded.GetHashCode();

                return hashCode;
            }
        }
        
        internal void SetCornerMode(int index, Corner value)
        {
            m_ControlPoints[index].corner = (value != Corner.Disable);
            m_ControlPoints[index].cornerMode = value;
        }
        
        internal Corner GetCornerMode(int index)
        {
            if (m_ControlPoints[index].cornerMode == Corner.Disable)
            {
                // For backward compatibility.
                if (m_ControlPoints[index].corner)
                {
                    m_ControlPoints[index].cornerMode = Corner.Automatic;
                    return Corner.Automatic;
                }
            }
            return m_ControlPoints[index].cornerMode;
        }        
    }
}
