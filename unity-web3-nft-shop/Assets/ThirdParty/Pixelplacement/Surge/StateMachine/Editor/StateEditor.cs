/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Custom inspector for the State class.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Pixelplacement
{
    [CustomEditor (typeof (State), true)]
    public class StateEditor : Editor 
    {
        //Private Variables:
        State _target;
        
        //Init:
        void OnEnable()
        {
            _target = target as State;
        }

        //Inspector GUI:
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector ();
            if (!Application.isPlaying)
            {
                GUILayout.BeginHorizontal();
                DrawSoloButton();
                DrawHideAllButton();
                GUILayout.EndHorizontal();
            }
            else
            {
                DrawChangeStateButton();
            }
        }

        //GUI Draw Methods:
        void DrawChangeStateButton ()
        {
            GUI.color = Color.green;
            if (GUILayout.Button("Change State"))
            {
                _target.ChangeState(_target.gameObject);
            }
        }

        void DrawHideAllButton ()
        {
            GUI.color = Color.red;
            if (GUILayout.Button ("Hide All"))
            {
                Undo.RegisterCompleteObjectUndo (_target.transform.parent.transform, "Hide All");
                foreach (Transform item in _target.transform.parent.transform) 
                {
                    item.gameObject.SetActive (false);
                }
            }
        }

        void DrawSoloButton ()
        {
            GUI.color = Color.green;
            if (GUILayout.Button ("Solo"))
            {
                foreach (Transform item in _target.transform.parent.transform) 
                {
                    if (item != _target.transform) item.gameObject.SetActive (false);
                    Undo.RegisterCompleteObjectUndo (_target, "Solo");
                    _target.gameObject.SetActive (true);
                }
            }
        }
    }
}