/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// This looks over all pieces of the framework to ensure they are properly set up - this is normally needed if a script that was already added to a GameObject suddenly extends something with a RequireComponent which is a decent bug in my opinion but this approach seems to work as a decent safety net.
/// 
/// </summary>

using UnityEngine;
using UnityEditor;
using System;

namespace Pixelplacement
{
    [InitializeOnLoad]
    public class InitializationRequirements
    {
        static InitializationRequirements ()
        {
            //state machines:
            StateMachine[] stateMachines = Resources.FindObjectsOfTypeAll<StateMachine> ();
            foreach (StateMachine item in stateMachines) 
            {
                if (item.GetComponent<Initialization> () == null) item.gameObject.AddComponent<Initialization> ();	
            }

            //display object:
            DisplayObject[] displayObjects = Resources.FindObjectsOfTypeAll<DisplayObject> ();
            foreach (DisplayObject item in displayObjects) 
            {
                if (item.GetComponent<Initialization> () == null) item.gameObject.AddComponent<Initialization> ();	
            }

            //singleton (generics require some hackery to find what we need):
            foreach (GameObject item in Resources.FindObjectsOfTypeAll<GameObject> ()) 
            {
                foreach (Component subItem in item.GetComponents<Component> ())
                {
                    //bypass this component if its currently unavailable due to a broken or missing script:
                    if (subItem == null) continue;

                    string baseType;

                    #if NETFX_CORE
                    baseType = subItem.GetType ().GetTypeInfo ().BaseType.ToString ();
                    #else
                    baseType = subItem.GetType ().BaseType.ToString ();
                    #endif

                    if (baseType.Contains ("Singleton") && baseType.Contains ("Pixelplacement")) 
                    {
                        if (item.GetComponent<Initialization> () == null) item.gameObject.AddComponent<Initialization> ();
                        continue;
                    }
                }
            }
        }
    }
}