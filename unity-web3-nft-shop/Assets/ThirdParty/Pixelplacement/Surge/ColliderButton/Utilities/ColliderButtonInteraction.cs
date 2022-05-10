/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// An optional helper class that sets up a GameObject so that it can "physically" collide with a ColliderButton for input events... 
/// all this means is that is has a collider that has 'isTrigger' set to true.
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderButtonInteraction : MonoBehaviour
{
    //Init
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Awake()
    {
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }
}