/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
///
/// An elegant solution for allowing stronger code-controlled object visibility that doesn't depend on how an object was last left in the editor.
///
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Pixelplacement
{
    [RequireComponent (typeof (Initialization))]
    public class DisplayObject : MonoBehaviour 
    {
        //Private Variables:
        private bool _activated;

        //Public Properties:
        /// <summary>
        /// Wrapper for GameObject's ActiveSelf property for ease of use.
        /// </summary>
        public bool ActiveSelf
        {
            get
            {
                return gameObject.activeSelf;
            }

            set
            {
                SetActive(value);
            }
        }

        //Public Methods:
        /// <summary>
        /// Registers this DisplayObject - should only be called by Initialization.
        /// </summary>
        public void Register ()
        {
            if (!_activated) 
            {
                _activated = true;	
                gameObject.SetActive (false);
            }
        }
        
        /// <summary>
        /// Wrapper for GameObject's SetActive method for ease of use.
        /// </summary>
        public void SetActive (bool value)
        {
            _activated = true;	
            gameObject.SetActive (value);
        }

        /// <summary>
        /// Solo this DisplayObject within other DisplayObjects at the same level in the hierarchy.
        /// </summary>
        public void Solo ()
        {
            Register();
            
            if (transform.parent != null)
            {
                foreach (Transform item in transform.parent) 
                {
                    if (item == transform) continue;
                    DisplayObject displayObject = item.GetComponent<DisplayObject> ();
                    if (displayObject != null) displayObject.SetActive (false);
                }
                gameObject.SetActive (true);
            }else{
                foreach (var item in Resources.FindObjectsOfTypeAll<DisplayObject> ()) 
                {
                    if (item.transform.parent == null)
                    {
                        if (item == this)
                        {
                            item.SetActive (true);
                        }else{
                            item.SetActive (false);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Hides all DisplayObjects at the same level in the hierarchy as this DisplayObject.
        /// </summary>
        public void HideAll ()
        {
            if (transform.parent != null)
            {
                foreach (Transform item in transform.parent) 
                {
                    if (item.GetComponent<DisplayObject> () != null) item.gameObject.SetActive (false);
                }
            }else{
                foreach (var item in Resources.FindObjectsOfTypeAll<DisplayObject> ()) 
                {
                    if (item.transform.parent == null) item.gameObject.SetActive (false);
                }
            }
        }
    }
}