/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Forces pivot mode to center so an anchor's pivot is always correct while adjusting a spline.
/// 
/// </summary>

using UnityEditor;
using UnityEngine;

namespace Pixelplacement
{
    [CustomEditor(typeof(SplineTangent))]
    public class SplineTangentEditor : Editor
    {
        //Scene GUI:
        void OnSceneGUI ()
        {
            //ensure pivot is used so anchor selection has a proper transform origin:
            if (Tools.pivotMode == PivotMode.Center)
            {
                Tools.pivotMode = PivotMode.Pivot;
            }
        }

        //Gizmos:
        [DrawGizmo(GizmoType.Selected)]
        static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
        {
            if (objectTransform.parent != null && objectTransform.parent.parent != null)
            {
                SplineEditor.RenderCustomGizmo(objectTransform.parent.parent, gizmoType);
            }
        }
    }
}