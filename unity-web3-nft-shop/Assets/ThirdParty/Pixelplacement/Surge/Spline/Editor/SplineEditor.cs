/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Draws all controls for a spline.
/// 
/// </summary>

using UnityEditor;
using UnityEngine;

namespace Pixelplacement
{
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : Editor
    {
        //Private Variables
        Spline _target;

        //Init:
        void OnEnable ()
        {
            _target = target as Spline;
        }

        //Inspector:
        public override void OnInspectorGUI ()
        {
            //draw:
            DrawDefaultInspector ();
            DrawAddButton ();
        }

        //Gizmo Overload:
        [DrawGizmo(GizmoType.Selected)]
        public static void RenderCustomGizmo (Transform objectTransform, GizmoType gizmoType)
        {
            DrawTools (objectTransform);
        }

        //Scene GUI:
        void OnSceneGUI ()
        {
            DrawTools ((target as Spline).transform);	
        }

        //Draw Methods:
        void DrawAddButton ()
        {
            if (GUILayout.Button ("Extend"))
            {
                Undo.RegisterCreatedObjectUndo (_target.AddAnchors (1) [0], "Extend Spline");
            }
        }

        static void DrawTools (Transform target)
        {
            Spline spline = target.GetComponent<Spline> ();
            if (spline == null) return;
            if (target.transform.childCount == 0) return;

            //set primary draw color:
            Handles.color = spline.SecondaryColor;

            for (int i = 0; i < spline.Anchors.Length; i++)
            {
                //refs:
                Quaternion lookRotation = Quaternion.identity;
                SplineAnchor currentAnchor = spline.Anchors[i];

                //scale geometry:
                currentAnchor.InTangent.localScale = Vector3.one * (spline.toolScale * 1.3f);
                currentAnchor.OutTangent.localScale = Vector3.one * (spline.toolScale * 1.3f);
                currentAnchor.Anchor.localScale = Vector3.one * (spline.toolScale * 2.1f);

                if (spline.toolScale > 0)
                {
                    //draw persistent identifiers that face the scene view camera and only draw if the corrosponding tangent is active:
                    if (currentAnchor.OutTangent.gameObject.activeSelf)
                    {
                        //connection:
                        Handles.DrawDottedLine (currentAnchor.Anchor.position, currentAnchor.OutTangent.position, 3);
                        
                        //indicators:
                        if (SceneView.currentDrawingSceneView != null)
                        {
                            lookRotation = Quaternion.LookRotation ((SceneView.currentDrawingSceneView.camera.transform.position - currentAnchor.OutTangent.position).normalized);
                            Handles.CircleHandleCap (0, currentAnchor.OutTangent.position, lookRotation, spline.toolScale * .65f, EventType.Repaint);
                            Handles.CircleHandleCap (0, currentAnchor.OutTangent.position, lookRotation, spline.toolScale * .25f, EventType.Repaint);
                        }
                    }

                    if (currentAnchor.InTangent.gameObject.activeSelf)
                    {
                        //connection:
                        Handles.DrawDottedLine (currentAnchor.Anchor.position, currentAnchor.InTangent.position, 3);

                        //indicators:
                        if (SceneView.currentDrawingSceneView != null)
                        {
                            lookRotation = Quaternion.LookRotation ((SceneView.currentDrawingSceneView.camera.transform.position - currentAnchor.InTangent.position).normalized);
                            Handles.CircleHandleCap (0, currentAnchor.InTangent.position, lookRotation, spline.toolScale * .65f, EventType.Repaint);
                            Handles.CircleHandleCap (0, currentAnchor.InTangent.position, lookRotation, spline.toolScale * .25f, EventType.Repaint);
                        }
                    }
                    
                    //anchor tools:
                    if (SceneView.currentDrawingSceneView != null)
                    {
                        lookRotation = Quaternion.LookRotation ((SceneView.currentDrawingSceneView.camera.transform.position - currentAnchor.Anchor.position).normalized);
                        Handles.CircleHandleCap (0, currentAnchor.Anchor.position, lookRotation, spline.toolScale, EventType.Repaint);
                    }

                    //identify path origin:
                    if (spline.direction == SplineDirection.Forward && i == 0 || spline.direction == SplineDirection.Backwards && i == spline.Anchors.Length - 1) 
                    {
                        Handles.CircleHandleCap (0, currentAnchor.Anchor.position, lookRotation, spline.toolScale * .8f, EventType.Repaint);
                        Handles.CircleHandleCap (0, currentAnchor.Anchor.position, lookRotation, spline.toolScale * .75f, EventType.Repaint);
                        Handles.CircleHandleCap (0, currentAnchor.Anchor.position, lookRotation, spline.toolScale * .5f, EventType.Repaint);
                        Handles.CircleHandleCap (0, currentAnchor.Anchor.position, lookRotation, spline.toolScale * .45f, EventType.Repaint);
                        Handles.CircleHandleCap (0, currentAnchor.Anchor.position, lookRotation, spline.toolScale * .25f, EventType.Repaint);
                    }
                }
            }

            //draw spline:
            for (int i = 0; i < spline.Anchors.Length - 1; i++)
            {
                SplineAnchor startAnchor = spline.Anchors[i];
                SplineAnchor endAnchor = spline.Anchors[i+1];
                Handles.DrawBezier (startAnchor.Anchor.position, endAnchor.Anchor.position, startAnchor.OutTangent.position, endAnchor.InTangent.position, spline.color, null, 2);
            }
        }
    }
}