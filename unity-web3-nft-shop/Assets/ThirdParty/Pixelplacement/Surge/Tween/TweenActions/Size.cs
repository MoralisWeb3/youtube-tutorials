/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com 
/// </summary>

using UnityEngine;
using System;
using UnityEngine.UI;
using Pixelplacement;

namespace Pixelplacement.TweenSystem
{
    class Size : TweenBase
    {
        //Public Properties:
        public Vector2 EndValue {get; private set;}

        //Private Variables:
        RectTransform _target;
        Vector2 _start;

        //Constructor:
        public Size (RectTransform target, Vector2 endValue, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //set essential properties:
            SetEssentials (Tween.TweenType.Size, target.GetInstanceID (), duration, delay, obeyTimescale, curve, loop, startCallback, completeCallback);

            //catalog custom properties:
            _target = target;
            EndValue = endValue;
        }

        //Processes:
        protected override bool SetStartValue ()
        {
            if (_target == null) return false;
            _start = _target.sizeDelta;
            return true;
        }

        protected override void Operation (float percentage)
        {
            Vector2 calculatedValue = TweenUtilities.LinearInterpolate (_start, EndValue, percentage);
            _target.sizeDelta = calculatedValue;
        }

        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
            _target.sizeDelta = _start;
        }

        public override void PingPong ()
        {
            ResetStartTime ();
            _target.sizeDelta = EndValue;
            EndValue = _start;
            _start = _target.sizeDelta;
        }
    }
}