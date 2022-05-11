/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Allows for easy selection toggle of Collider Buttons.
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class ColliderButtonSelector : MonoBehaviour
{
    //Public Variables:
    public Chooser chooser;
    public bool loopAround;
    public KeyCode previousKey = KeyCode.LeftArrow;
    public KeyCode nextKey = KeyCode.RightArrow;
    public ColliderButton[] colliderButtons;

    //Private Variables
    private int _index;

    //Init:
    private void OnEnable()
    {
        _index = -1;
        Next();
    }

    private void Reset()
    {
        chooser = GetComponent<Chooser>();
    }

    //Loops:
    private void Update()
    {
        if (Input.GetKeyDown(previousKey)) Previous();
        if (Input.GetKeyDown(nextKey)) Next();
    }

    //Public Methods:
    public void Next()
    {
        _index++;

        if (_index > colliderButtons.Length-1)
        {
            if (loopAround)
            {
                _index = 0;
            }
            else
            {
                _index = colliderButtons.Length - 1;
            }
        }
    
        chooser.transform.LookAt(colliderButtons[_index].transform);
    }

    public void Previous()
    {
        _index--;

        if (_index < 0)
        {
            if (loopAround)
            {
                _index = colliderButtons.Length - 1;
            }
            else
            {
                _index = 0;
            }
        }

        chooser.transform.LookAt(colliderButtons[_index].transform);
    }
}