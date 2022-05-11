/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Base class for States to be used as children of StateMachines.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

namespace Pixelplacement
{
    public class State : MonoBehaviour 
    {
        //Public Properties:
        /// <summary>
        /// Gets a value indicating whether this instance is the first state in this state machine.
        /// </summary>
        public bool IsFirst
        {
            get
            {
                return transform.GetSiblingIndex () == 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is the last state in this state machine.
        /// </summary>
        public bool IsLast
        {
            get
            {
                return transform.GetSiblingIndex () == transform.parent.childCount - 1;
            }
        }

        /// <summary>
        /// Gets or sets the state machine.
        /// </summary>
        public StateMachine StateMachine
        {
            get
            {
                if (_stateMachine == null)
                {
                    _stateMachine = transform.parent.GetComponent<StateMachine>();
                    if (_stateMachine == null)
                    {
                        Debug.LogError("States must be the child of a StateMachine to operate.");
                        return null;
                    }
                }

                return _stateMachine;
            }
        }

        //Private Variables:
        StateMachine _stateMachine;

        //Public Methods
        /// <summary>
        /// Changes the state.
        /// </summary>
        public void ChangeState(int childIndex)
        {
            StateMachine.ChangeState(childIndex);
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        public void ChangeState (GameObject state)
        {
            StateMachine.ChangeState (state.name);
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        public void ChangeState (string state)
        {
            StateMachine.ChangeState (state);
        }

        /// <summary>
        /// Change to the next state if possible.
        /// </summary>
        public GameObject Next (bool exitIfLast = false)
        {
            return StateMachine.Next (exitIfLast);
        }

        /// <summary>
        /// Change to the previous state if possible.
        /// </summary>
        public GameObject Previous (bool exitIfFirst = false)
        {
            return StateMachine.Previous (exitIfFirst);
        }

        /// <summary>
        /// Exit the current state.
        /// </summary>
        public void Exit ()
        {
            StateMachine.Exit ();
        }
        
        protected Coroutine StartCoroutineIfActive(IEnumerator coroutine)
        {
            if (gameObject.activeInHierarchy)
            {
                return StartCoroutine(coroutine);
            }
            else
            {
                return null;
            }
        }
    }
}