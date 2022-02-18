using System;
using System.Collections.Generic;

namespace UnityEngine.U2D.IK
{
    public partial class IKManager2D : MonoBehaviour
    {
#if UNITY_EDITOR
        [Serializable]
        internal struct SolverEditorData
        {
            public Color color;
            public bool showGizmo;
            public static SolverEditorData defaultValue
            {
                get
                {
                    return new SolverEditorData(){ color = Color.green, showGizmo = true};
                }
            }
        }

        [SerializeField]
        private List<SolverEditorData> m_SolverEditorData = new List<SolverEditorData>();

        void OnEditorDataValidate()
        {
            var solverDataLength = m_SolverEditorData.Count;
            for (int i = solverDataLength; i < m_Solvers.Count; ++i)
            {
                AddSolverEditorData();
            }
        }

        internal SolverEditorData GetSolverEditorData(Solver2D solver)
        {
            var index = m_Solvers.FindIndex(x => x == solver);
            if (index >= 0)
            {
                if(index >= m_SolverEditorData.Count)
                    OnEditorDataValidate();
                return m_SolverEditorData[index];
            }
                
            return SolverEditorData.defaultValue;
        }

        void AddSolverEditorData()
        {
            m_SolverEditorData.Add(new SolverEditorData()
            {
                color = Color.green,
                showGizmo = true
            });
        }

        void RemoveSolverEditorData(Solver2D solver)
        {
            var index = m_Solvers.FindIndex(x => x == solver);
            if(index >= 0)
                m_SolverEditorData.RemoveAt(index);
        }
#else
        void OnEditorDataValidate(){}
        void AddSolverEditorData(){}
        void RemoveSolverEditorData(Solver2D solver){}
#endif
    }
}

