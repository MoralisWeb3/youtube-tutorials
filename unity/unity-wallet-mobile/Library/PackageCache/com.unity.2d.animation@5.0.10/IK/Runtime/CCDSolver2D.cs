using System.Collections.Generic;
using UnityEngine.Profiling;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D.IK
{
    /// <summary>
    /// Component for 2D Cyclic Coordinate Descent (CCD) IK.
    /// </summary>
    [MovedFrom("UnityEngine.Experimental.U2D.IK")]
    [Solver2DMenuAttribute("Chain (CCD)")]
    public class CCDSolver2D : Solver2D
    {
        private const float kMinTolerance = 0.001f;
        private const int kMinIterations = 1;
        private const float kMinVelocity = 0.01f;
        private const float kMaxVelocity = 1f;

        [SerializeField]
        private IKChain2D m_Chain = new IKChain2D();

        [SerializeField][Range(kMinIterations, 50)]
        private int m_Iterations = 10;
        [SerializeField][Range(kMinTolerance, 0.1f)]
        private float m_Tolerance = 0.01f;
        [SerializeField][Range(0f, 1f)]
        private float m_Velocity = 0.5f;

        private Vector3[] m_Positions;

        /// <summary>
        /// Get and Set the solver's itegration count.
        /// </summary>
        public int iterations
        {
            get { return m_Iterations; }
            set { m_Iterations = Mathf.Max(value, kMinIterations); }
        }

        /// <summary>
        /// Get and Set target distance tolerance.
        /// </summary>
        public float tolerance
        {
            get { return m_Tolerance; }
            set { m_Tolerance = Mathf.Max(value, kMinTolerance); }
        }

        /// <summary>
        /// Get and Set the solver velocity.
        /// </summary>
        public float velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// Returns the number of chain in the solver.
        /// </summary>
        /// <returns>This always returns 1</returns>
        protected override int GetChainCount()
        {
            return 1;
        }

        /// <summary>
        /// Gets the chain in the solver by index.
        /// </summary>
        /// <param name="index">Chain index.</param>
        /// <returns>Returns IKChain2D at the index.</returns>
        public override IKChain2D GetChain(int index)
        {
            return m_Chain;
        }
        
        /// <summary>
        /// DoPrepare override from base class.
        /// </summary>
        protected override void DoPrepare()
        {
            if (m_Positions == null || m_Positions.Length != m_Chain.transformCount)
                m_Positions = new Vector3[m_Chain.transformCount];

            for (int i = 0; i < m_Chain.transformCount; ++i)
                m_Positions[i] = m_Chain.transforms[i].position;
        }

        /// <summary>
        /// DoUpdateIK override from base class.
        /// </summary>
        /// <param name="effectorPositions">Target position for the chain.</param>
        protected override void DoUpdateIK(List<Vector3> effectorPositions)
        {
            Profiler.BeginSample("CCDSolver2D.DoUpdateIK");

            Vector3 effectorPosition = effectorPositions[0];
            Vector2 effectorLocalPosition2D = m_Chain.transforms[0].InverseTransformPoint(effectorPosition);
            effectorPosition = m_Chain.transforms[0].TransformPoint(effectorLocalPosition2D);

            if (CCD2D.Solve(effectorPosition, GetPlaneRootTransform().forward, iterations, tolerance, Mathf.Lerp(kMinVelocity, kMaxVelocity, m_Velocity), ref m_Positions))
            {
                for (int i = 0; i < m_Chain.transformCount - 1; ++i)
                {
                    Vector3 startLocalPosition = m_Chain.transforms[i + 1].localPosition;
                    Vector3 endLocalPosition = m_Chain.transforms[i].InverseTransformPoint(m_Positions[i + 1]);
                    m_Chain.transforms[i].localRotation *= Quaternion.FromToRotation(startLocalPosition, endLocalPosition);
                }
            }

            Profiler.EndSample();
        }
    }
}
