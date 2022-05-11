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
    class ValueVector2 : TweenBase
    {
        //Public Properties:
        public Vector2 EndValue {get; private set;}

        //Private Variables:
        Action<Vector2> _valueUpdatedCallback;
        Vector2 _start;

        //Constructor:
        public ValueVector2 (Vector2 startValue, Vector2 endValue, Action<Vector2> valueUpdatedCallback, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //set essential properties:
            SetEssentials (Tween.TweenType.Value, -1, duration, delay, obeyTimescale, curve, loop, startCallback, completeCallback);

            //catalog custom properties:
            _valueUpdatedCallback = valueUpdatedCallback;
            _start = startValue;
            EndValue = endValue;
        }

        //Processes:
        protected override bool SetStartValue ()
        {
            return true;
        }

        protected override void Operation (float percentage)
        {
            Vector2 calculatedValue = TweenUtilities.LinearInterpolate (_start, EndValue, percentage);
            _valueUpdatedCallback (calculatedValue);
        }

        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
        }

        public override void PingPong ()
        {
            ResetStartTime ();
            Vector2 temp = _start;
            _start = EndValue;
            EndValue = temp;
        }
    }
}