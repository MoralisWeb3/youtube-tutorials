using UnityEngine;
using UnityEngine.U2D.IK;

namespace UnityEditor.U2D.IK
{
    /// <summary>
    /// Custom Inspector for Solver2D.
    /// </summary>
    [CustomEditor(typeof(Solver2D))]
    [CanEditMultipleObjects]
    public class Solver2DEditor : Editor
    {
        private static class Contents
        {
            public static readonly GUIContent constrainRotationLabel = new GUIContent("Constrain Rotation", "Set Effector's rotation to Target");
            public static readonly GUIContent solveFromDefaultPoseLabel = new GUIContent("Solve from Default Pose", "Restore transform's rotation to default value before solving the IK");
            public static readonly GUIContent weightLabel = new GUIContent("Weight", "Blend between Forward and Inverse Kinematics");
            public static readonly string restoreDefaultPoseString = "Restore Default Pose";
            public static readonly string createTargetString = "Create Target";
        }

        private SerializedProperty m_ConstrainRotationProperty;
        private SerializedProperty m_SolveFromDefaultPoseProperty;
        private SerializedProperty m_WeightProperty;
        private SerializedProperty m_SolverColorProperty;

        private void SetupProperties()
        {
            if(m_ConstrainRotationProperty == null || m_SolveFromDefaultPoseProperty == null || m_WeightProperty == null)
            {
                m_ConstrainRotationProperty = serializedObject.FindProperty("m_ConstrainRotation");
                m_SolveFromDefaultPoseProperty = serializedObject.FindProperty("m_SolveFromDefaultPose");
                m_WeightProperty = serializedObject.FindProperty("m_Weight");
            }
        }

        /// <summary>
        /// Custom Inspector GUI for Solver2D.
        /// </summary>
        protected void DrawCommonSolverInspector()
        {
            SetupProperties();

            EditorGUILayout.PropertyField(m_ConstrainRotationProperty, Contents.constrainRotationLabel);
            EditorGUILayout.PropertyField(m_SolveFromDefaultPoseProperty, Contents.solveFromDefaultPoseLabel);
            EditorGUILayout.PropertyField(m_WeightProperty, Contents.weightLabel);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(!EnableCreateTarget());
            DoCreateTargetButton();
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!EnableRestoreDefaultPose());
            DoRestoreDefaultPoseButton();
            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        private bool EnableRestoreDefaultPose()
        {
            foreach (var l_target in targets)
            {
                var solver = l_target as Solver2D;

                if (!solver.isValid || IKEditorManager.instance.FindManager(solver) == null)
                    continue;

                return true;
            }

            return false;
        }

        private bool EnableCreateTarget()
        {
            foreach (var l_target in targets)
            {
                var solver = l_target as Solver2D;

                if (!solver.isValid)
                    continue;

                for(int i = 0; i < solver.chainCount; ++i)
                {
                    var chain = solver.GetChain(i);

                    if(chain.target == null)
                        return true;
                }
            }

            return false;
        }

        private void DoRestoreDefaultPoseButton()
        {
            if (GUILayout.Button(Contents.restoreDefaultPoseString, GUILayout.MaxWidth(150f)))
            {
                foreach (var l_target in targets)
                {
                    var solver = l_target as Solver2D;

                    if (!solver.isValid)
                        continue;

                    IKEditorManager.instance.Record(solver, Contents.restoreDefaultPoseString);

                    for(int i = 0; i < solver.chainCount; ++i)
                    {
                        var chain = solver.GetChain(i);
                        chain.RestoreDefaultPose(solver.constrainRotation);
                        
                        if(chain.target)
                        {
                            chain.target.position = chain.effector.position;
                            chain.target.rotation = chain.effector.rotation;
                        }
                    }

                    IKEditorManager.instance.UpdateSolverImmediate(solver, true);
                }
            }
        }

        private void DoCreateTargetButton()
        {
            if (GUILayout.Button(Contents.createTargetString, GUILayout.MaxWidth(125f)))
            {
                foreach (var l_target in targets)
                {
                    var solver = l_target as Solver2D;

                    if (!solver.isValid)
                        continue;

                    for(int i = 0; i < solver.chainCount; ++i)
                    {
                        var chain = solver.GetChain(i);
                        
                        if(chain.target == null)
                        {
                            Undo.RegisterCompleteObjectUndo(solver, Contents.createTargetString);

                            chain.target = new GameObject(GameObjectUtility.GetUniqueNameForSibling(solver.transform, solver.name + "_Target")).transform;
                            chain.target.SetParent(solver.transform);
                            chain.target.position = chain.effector.position;
                            chain.target.rotation = chain.effector.rotation;

                            Undo.RegisterCreatedObjectUndo(chain.target.gameObject, Contents.createTargetString);
                        }
                    }
                }
            }
        }
    }
}
