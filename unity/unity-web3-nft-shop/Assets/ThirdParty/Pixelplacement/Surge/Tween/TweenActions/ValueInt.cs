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
    class ValueInt : TweenBase
    {
        //Public Properties:
        public float EndValue {get; private set;}

        //Private Variables:
        Action<int> _valueUpdatedCallback;
        float _start;

        //Constructor:
        public ValueInt (int startValue, int endValue, Action<int> valueUpdatedCallback, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
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
            float calculatedValue = TweenUtilities.LinearInterpolate (_start, EndValue, percentage);
            _valueUpdatedCallback ((int)calculatedValue);
        }

        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
        }

        public override void PingPong ()
        {
            ResetStartTime ();
            float temp = _start;
            _start = EndValue;
            EndValue = temp;
        }
    }
}