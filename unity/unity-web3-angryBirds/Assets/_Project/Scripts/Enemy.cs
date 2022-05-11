using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngryBirdsWeb3
{
    public class Enemy : WorldObject
    {
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Floor") || col.gameObject.CompareTag("Bird"))
            {
                AutoDestroy();
            }
        }
    }   
}