/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Used internally by the tween system to run all tween calculations.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using Pixelplacement;

namespace Pixelplacement.TweenSystem
    {
    public class TweenEngine : MonoBehaviour
        {
            public void ExecuteTween(TweenBase tween) 
            {
                Tween.activeTweens.Add(tween);
            }
            
            private void Update()
            {
                foreach (var tween in Tween.activeTweens.ToArray())
                {
                    tween.Tick();
                }
                
                // for (int i = Tween.activeTweens.Count - 1; i >= 0; i--)
                // {
                //     if (!Tween.activeTweens[i].Tick())
                //     {
                //         Tween.activeTweens.RemoveAt(i);
                //     }
                // } 
            }
        }
    }