using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace UnityEngine.U2D.IK
{
    /// <summary>
    /// Abstract class for implementing a 2D IK Solver.
    /// </summary>
    [MovedFrom("UnityEngine.Experimental.U2D.IK")]
    public abstract class Solver2D : MonoBehaviour
    {
        [SerializeField]
        private bool m_ConstrainRotation = true;
        [FormerlySerializedAs("m_RestoreDefaultPose")]
        [SerializeField]
        private bool m_SolveFromDefaultPose = true;
        [SerializeField][Range(0f, 1f)]
        private float m_Weight = 1f;

        private Plane m_Plane;
        private List<Vector3> m_TargetPositions = new List<Vector3>();

        /// <summary>
        /// Returns the number of IKChain2D in the solver.
        /// </summary>
        public int chainCount
        {
            get { return GetChainCount(); }
        }

        /// <summary>
        /// Get Set for rotation constrain property.
        /// </summary>
        public bool constrainRotation
        {
            get { return m_ConstrainRotation; }
            set { m_ConstrainRotation = value; }
        }

        /// <summary>
        /// Get Set for restoring default pose.
        /// </summary>
        public bool solveFromDefaultPose
        {
            get { return m_SolveFromDefaultPose; }
            set { m_SolveFromDefaultPose = value; }
        }

        /// <summary>
        /// Returns true if the Solver2D is in a valid state.
        /// </summary>
        public bool isValid
        {
            get { return Validate(); }
        }

        /// <summary>
        /// Returns true if all chains in the Solver has a target.
        /// </summary>
        public bool allChainsHaveTargets
        {
            get { return HasTargets(); }
        }

        /// <summary>
        /// Get and Set Solver weights.
        /// </summary>
        public float weight
        {
            get { return m_Weight; }
            set { m_Weight = Mathf.Clamp01(value); }
        }

        private void OnEnable() {}

        /// <summary>
        /// Validate and initialize the Solver.
        /// </summary>
        protected virtual void OnValidate()
        {
            m_Weight = Mathf.Clamp01(m_Weight);

            if (!isValid)
                Initialize();
        }

        private bool Validate()
        {
            for (int i = 0; i < GetChainCount(); ++i)
            {
                var chain = GetChain(i);
                if (!chain.isValid)
                    return false;
            }
            return DoValidate();
        }

        private bool HasTargets()
        {
            for (int i = 0; i < GetChainCount(); ++i)
            {
                var chain = GetChain(i);
                if (chain.target == null)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Initializes the solver.
        /// </summary>
        public void Initialize()
        {
            DoInitialize();

            for (int i = 0; i < GetChainCount(); ++i)
            {
                var chain = GetChain(i);
                chain.Initialize();
            }
        }

        private void Prepare()
        {
            var rootTransform = GetPlaneRootTransform();
            if (rootTransform != null)
            {
                m_Plane.normal = rootTransform.forward;
                m_Plane.distance = -Vector3.Dot(m_Plane.normal, rootTransform.position);
            }

            for (int i = 0; i < GetChainCount(); ++i)
            {
                var chain = GetChain(i);
                var constrainTargetRotation = constrainRotation && chain.target != null;

                if (m_SolveFromDefaultPose)
                    chain.RestoreDefaultPose(constrainTargetRotation);
            }

            DoPrepare();
        }

        private void PrepareEffectorPositions()
        {
            m_TargetPositions.Clear();

            for (int i = 0; i < GetChainCount(); ++i)
            {
                var chain = GetChain(i);

                if (chain.target)
                    m_TargetPositions.Add(chain.target.position);
            }
        }

        /// <summary>
        /// Perfom Solver IK update.
        /// </summary>
        /// <param name="globalWeight">Weight for position solving.</param>
        public void UpdateIK(float globalWeight)
        {
            if(allChainsHaveTargets)
            {
                PrepareEffectorPositions();
                UpdateIK(m_TargetPositions, globalWeight);
            }
        }

        /// <summary>
        /// Perform Solver IK update.
        /// </summary>
        /// <param name="positions">Positions of chain.</param>
        /// <param name="globalWeight">Weight for position solving.</param>
        public void UpdateIK(List<Vector3> positions, float globalWeight)
        {
            if(positions.Count != chainCount)
                return;
                 
            float finalWeight = globalWeight * weight;
            if (finalWeight == 0f)
                return;

            if (!isValid)
                return;

            Prepare();

            if (finalWeight < 1f)
                StoreLocalRotations();

            DoUpdateIK(positions);

            if (constrainRotation)
            {
                for (int i = 0; i < GetChainCount(); ++i)
                {
                    var chain = GetChain(i);

                    if (chain.target)
                        chain.effector.rotation = chain.target.rotation;
                }
            }

            if (finalWeight < 1f)
                BlendFkToIk(finalWeight);
        }

        private void StoreLocalRotations()
        {
            for (int i = 0; i < GetChainCount(); ++i)
            {
                var chain = GetChain(i);
                chain.StoreLocalRotations();
            }
        }

        private void BlendFkToIk(float finalWeight)
        {
            for (int i = 0; i < GetChainCount(); ++i)
            {
                var chain = GetChain(i);
                var constrainTargetRotation = constrainRotation && chain.target != null;
                chain.BlendFkToIk(finalWeight, constrainTargetRotation);
            }
        }

        /// <summary>
        /// Override to return the IKChain2D at the given index.
        /// </summary>
        /// <param name="index">Index for IKChain2D.</param>
        /// <returns></returns>
        public abstract IKChain2D GetChain(int index);
        
        /// <summary>
        /// OVerride to return the number of chains in the Solver
        /// </summary>
        /// <returns>Integer represents IKChain2D count.</returns>
        protected abstract int GetChainCount();
        
        /// <summary>
        /// Override to perform Solver IK update
        /// </summary>
        /// <param name="effectorPositions">Position of the effectors.</param>
        protected abstract void DoUpdateIK(List<Vector3> effectorPositions);

        /// <summary>
        /// Override to perform custom validation.
        /// </summary>
        /// <returns>Returns true if the Solver is in a valid state. False otherwise.</returns>
        protected virtual bool DoValidate() { return true; }
        
        /// <summary>
        /// Override to perform initialize the solver
        /// </summary>
        protected virtual void DoInitialize() {}
        
        /// <summary>
        /// Override to prepare the solver for update
        /// </summary>
        protected virtual void DoPrepare() {}
        
        /// <summary>
        /// Override to return the root Unity Transform of the Solver. The default implementation returns the root
        /// transform of the first chain.
        /// </summary>
        /// <returns>Unity Transform that represents the root.</returns>
        protected virtual Transform GetPlaneRootTransform()
        {
            if (chainCount > 0)
                return GetChain(0).rootTransform;
            return null;
        }

        /// <summary>
        /// Convert a world position coordinate to the solver's plane space
        /// </summary>
        /// <param name="worldPosition">Vector3 representing world position</param>
        /// <returns>Converted position in solver's plane</returns>
        protected Vector3 GetPointOnSolverPlane(Vector3 worldPosition)
        {
            return GetPlaneRootTransform().InverseTransformPoint(m_Plane.ClosestPointOnPlane(worldPosition));
        }

        /// <summary>
        /// Convert a position from solver's plane to world coordinate
        /// </summary>
        /// <param name="planePoint">Vector3 representing a position in the Solver's plane.</param>
        /// <returns>Converted position to world coordinate.</returns>
        protected Vector3 GetWorldPositionFromSolverPlanePoint(Vector2 planePoint)
        {
            return GetPlaneRootTransform().TransformPoint(planePoint);
        }
    }
}
