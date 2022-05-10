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
    class AnchoredPosition : TweenBase
    {
        //Public Properties:
        public Vector2 EndValue {get; private set;}

        //Private Variables:
        RectTransform _target;
        Vector2 _start;

        //Constructor:
        public AnchoredPosition (RectTransform target, Vector2 endValue, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //set essential properties:
            SetEssentials (Tween.TweenType.AnchoredPosition, target.GetInstanceID (), duration, delay, obeyTimescale, curve, loop, startCallback, completeCallback);

            //catalog custom properties:
            _target = target;
            EndValue = endValue;
        }

        //Processes:
        protected override bool SetStartValue ()
        {
            if (_target == null) return false;
            _start = _target.anchoredPosition;
            return true;
        }

        protected override void Operation (float percentage)
        {
            Vector3 calculatedValue = TweenUtilities.LinearInterpolate (_start, EndValue, percentage);
            _target.anchoredPosition = calculatedValue;
        }

        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
            _target.anchoredPosition = _start;
        }

        public override void PingPong ()
        {
            ResetStartTime ();
            _target.anchoredPosition = EndValue;
            EndValue = _start;
            _start = _target.anchoredPosition;
        }
    }
}