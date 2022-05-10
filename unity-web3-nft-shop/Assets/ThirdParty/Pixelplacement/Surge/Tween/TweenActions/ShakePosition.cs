/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// </summary>

using UnityEngine;
using System;
using Pixelplacement;

namespace Pixelplacement.TweenSystem
{
    class ShakePosition : TweenBase
    {
        //Public Properties:
        public Vector3 EndValue {get; private set;}
        
        //Private Variables:
        Transform _target;
        Vector3 _initialPosition;
        Vector3 _intensity;
        
        //Constructor:
        public ShakePosition (Transform target, Vector3 initialPosition, Vector3 intensity, float duration, float delay, AnimationCurve curve, Action startCallback, Action completeCallback, Tween.LoopType loop, bool obeyTimescale)
        {
            //set essential properties:
            SetEssentials (Tween.TweenType.Position, target.GetInstanceID (), duration, delay, obeyTimescale, curve, loop, startCallback, completeCallback);
            
            //catalog custom properties:
            _target = target;
            _initialPosition = initialPosition;
            _intensity = intensity;
        }
        
        //Processes:
        protected override bool SetStartValue ()
        {
            if (_target == null) return false;
            return true;
        }
        
        protected override void Operation (float percentage)
        {
            if (percentage == 0)
            {
                _target.localPosition = _initialPosition;
            }

            percentage = 1 - percentage;

            //create diminishing shake offset:
            Vector3 amount = _intensity * percentage;
            amount.x = UnityEngine.Random.Range (-amount.x, amount.x);
            amount.y = UnityEngine.Random.Range (-amount.y, amount.y);
            amount.z = UnityEngine.Random.Range (-amount.z, amount.z);

            //apply:
            _target.localPosition = _initialPosition + amount;
        }
        
        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
            _target.localPosition = _initialPosition;
        }
        
        public override void PingPong ()
        {
        }
    }
}