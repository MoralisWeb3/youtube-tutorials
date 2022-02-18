using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D.IK
{
    /// <summary>
    /// Component to manager 2D IK Solvers.
    /// </summary>
    [DefaultExecutionOrder(-2)]
    [MovedFrom("UnityEngine.Experimental.U2D.IK")]
    public partial class IKManager2D : MonoBehaviour
    {
        [SerializeField]
        private List<Solver2D> m_Solvers = new List<Solver2D>();
        [SerializeField][Range(0f, 1f)]
        private float m_Weight = 1f;

        /// <summary>
        /// Get and Set the weight for solvers.
        /// </summary>
        public float weight
        {
            get { return m_Weight; }
            set { m_Weight = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// Get the Solvers that are managed by this manager.
        /// </summary>
        public List<Solver2D> solvers
        {
            get { return m_Solvers; }
        }

        private void OnValidate()
        {
            m_Weight = Mathf.Clamp01(m_Weight);
            OnEditorDataValidate();
        }

        private void OnEnable()
        {
        }

        private void Reset()
        {
            FindChildSolvers();
            OnEditorDataValidate();
        }

        private void FindChildSolvers()
        {
            m_Solvers.Clear();

            List<Solver2D> solvers = new List<Solver2D>();
            transform.GetComponentsInChildren<Solver2D>(true, solvers);

            foreach (Solver2D solver in solvers)
            {
                if (solver.GetComponentInParent<IKManager2D>() == this)
                    AddSolver(solver);
            }
        }

        /// <summary>
        /// Add Solver to the manager.
        /// </summary>
        /// <param name="solver">Solver to add.</param>
        public void AddSolver(Solver2D solver)
        {
            if (!m_Solvers.Contains(solver))
            {
                m_Solvers.Add(solver);
                AddSolverEditorData();
            }
        }

        /// <summary>
        /// Remove Solver from the manager.
        /// </summary>
        /// <param name="solver">Solver to remove.</param>
        public void RemoveSolver(Solver2D solver)
        {
            RemoveSolverEditorData(solver);
            m_Solvers.Remove(solver);
        }

        /// <summary>
        /// Updates the Solvers in this manager.
        /// </summary>
        public void UpdateManager()
        {
            foreach (var solver in m_Solvers)
            {
                if (solver == null || !solver.isActiveAndEnabled)
                    continue;

                if (!solver.isValid)
                    solver.Initialize();

                solver.UpdateIK(weight);
            }
        }

        private void LateUpdate()
        {
            UpdateManager();
        }
        
#if UNITY_EDITOR
        internal static Events.UnityEvent onDrawGizmos = new Events.UnityEvent();
        private void OnDrawGizmos() { onDrawGizmos.Invoke(); }
#endif
    }
}
