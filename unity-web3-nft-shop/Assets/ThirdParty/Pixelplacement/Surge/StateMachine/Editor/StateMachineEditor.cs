/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Custom inspector for the StateMachine class.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pixelplacement
{
    [CustomEditor (typeof (StateMachine), true)]
    public class StateMachineEditor : Editor 
    {
        //Private Variables:
        StateMachine _target;

        //Init:
        void OnEnable()
        {
            _target = target as StateMachine;
        }

        //Inspector GUI:
        public override void OnInspectorGUI()
        {
            //if no states are found:
            if (_target.transform.childCount == 0)
            {
                DrawNotification("Add child Gameobjects for this State Machine to control.", Color.yellow);
                return;
            }

            //change buttons:
            if (EditorApplication.isPlaying)
            {
                DrawStateChangeButtons();
            }

            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, new string[] {
                "currentState",
                "_unityEventsFolded",
                "defaultState",
                "verbose",
                "allowReentry",
                "returnToDefaultOnDisable",
                "Unity Events",
                "OnStateExited",
                "OnStateEntered",
                "OnFirstStateEntered",
                "OnFirstStateExited",
                "OnLastStateEntered",
                "OnLastStateExited"
            });

            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultState"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("verbose"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowReentry"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("returnToDefaultOnDisable"));

            //fold events:
            _target._unityEventsFolded = EditorGUILayout.Foldout(_target._unityEventsFolded, "Unity Events", true);
            if (_target._unityEventsFolded)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStateExited"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStateEntered"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFirstStateEntered"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFirstStateExited"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnLastStateEntered"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnLastStateExited"));
            }

            serializedObject.ApplyModifiedProperties();

            if (!EditorApplication.isPlaying)
            {
                DrawHideAllButton();
            }
        }

        //GUI Draw Methods:
        void DrawStateChangeButtons()
        {
            if (_target.transform.childCount == 0) return;
            Color currentColor = GUI.color;
            for (int i = 0; i < _target.transform.childCount; i++)
            {
                GameObject current = _target.transform.GetChild(i).gameObject;

                if (_target.currentState != null && current == _target.currentState)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button(current.name)) _target.ChangeState(current);
            }
            GUI.color = currentColor;
            if (GUILayout.Button("Exit")) _target.Exit();
        }

        void DrawHideAllButton()
        {
            GUI.color = Color.red;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Hide All"))
            {
                Undo.RegisterCompleteObjectUndo(_target.transform, "Hide All");
                foreach (Transform item in _target.transform)
                {
                    item.gameObject.SetActive(false);
                }
            }
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
        }

        void DrawNotification(string message, Color color)
        {
            Color currentColor = GUI.color;
            GUI.color = color;
            EditorGUILayout.HelpBox(message, MessageType.Warning);
            GUI.color = currentColor;
        }
    }
}