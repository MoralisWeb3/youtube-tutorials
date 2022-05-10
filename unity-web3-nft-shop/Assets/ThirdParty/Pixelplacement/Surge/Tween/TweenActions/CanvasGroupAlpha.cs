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
    class CanvasGroupAlpha : TweenBase
    {
        //Public Properties:
        public float EndValue {get; private set;}

        //Private Variables:
        CanvasGroup _target;
        float _start;

        //Constructor:
        public CanvasGroupAlpha (CanvasGroup target, float endValue, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //set essential properties:
            SetEssentials (Tween.TweenType.CanvasGroupAlpha, target.GetInstanceID (), duration, delay, obeyTimescale, curve, loop, startCallback, completeCallback);

            //catalog custom properties:
            _target = target;
            EndValue = endValue;
        }

        //Processes:
        protected override bool SetStartValue ()
        {
            if (_target == null) return false;
            _start = _target.alpha;
            return true;
        }

        protected override void Operation (float percentage)
        {
            float calculatedValue = TweenUtilities.LinearInterpolate (_start, EndValue, percentage);
            _target.alpha = calculatedValue;
        }

        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
            _target.alpha = _start;
        }

        public override void PingPong ()
        {
            ResetStartTime ();
            _target.alpha = EndValue;
            EndValue = _start;
            _start = _target.alpha;
        }
    }
}