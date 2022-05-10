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
    class CameraBackgroundColor : TweenBase
    {
        //Public Properties:
        public Color EndValue { get; private set; }

        //Private Variables:
        Camera _target;
        Color _start;

        //Constructor:
        public CameraBackgroundColor(Camera target, Color endValue, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //set essential properties:
            SetEssentials(Tween.TweenType.CameraBackgroundColor, target.GetInstanceID(), duration, delay, obeyTimescale, curve, loop, startCallback, completeCallback);

            //catalog custom properties:
            _target = target;
            EndValue = endValue;
        }

        //Processes:
        protected override bool SetStartValue()
        {
            if (_target == null) return false;
            _start = _target.backgroundColor;
            return true;
        }

        protected override void Operation(float percentage)
        {
            Color calculatedValue = TweenUtilities.LinearInterpolate(_start, EndValue, percentage);
            _target.backgroundColor = calculatedValue;
        }

        //Loops:
        public override void Loop()
        {
            ResetStartTime();
            _target.backgroundColor = _start;
        }

        public override void PingPong()
        {
            ResetStartTime();
            _target.backgroundColor = EndValue;
            EndValue = _start;
            _start = _target.backgroundColor;
        }
    }
}