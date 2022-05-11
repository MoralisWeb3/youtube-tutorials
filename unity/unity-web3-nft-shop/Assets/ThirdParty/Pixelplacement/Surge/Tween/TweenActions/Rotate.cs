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
    class Rotate : TweenBase
    {
        //Public Properties:
        public Vector3 EndValue {get; private set;}

        //Private Variables:
        Transform _target;
        Vector3 _start;
        Space _space;
        Vector3 _previous;

        //Constructor:
        public Rotate (Transform target, Vector3 endValue, Space space, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //set essential properties:
            SetEssentials (Tween.TweenType.Rotation, target.GetInstanceID (), duration, delay, obeyTimescale, curve, loop, startCallback, completeCallback);

            //catalog custom properties:
            _target = target;
            EndValue = endValue;
            _space = space;
        }

        //Processes:
        protected override bool SetStartValue ()
        {
            if (_target == null) return false;
            _start = _target.localEulerAngles;
            return true;
        }

        protected override void Operation (float percentage)
        {
            if (percentage == 0)
            {
                _target.localEulerAngles = _start;
            }
            Vector3 spinAmount = TweenUtilities.LinearInterpolate (Vector3.zero, EndValue, percentage);
            Vector3 spinDifference = spinAmount - _previous;
            _previous += spinDifference;
            _target.Rotate (spinDifference, _space);
        }

        //Loops:
        public override void Loop ()
        {
            _previous = Vector3.zero;
            ResetStartTime ();
        }

        public override void PingPong ()
        {
            _previous = Vector3.zero;
            EndValue *= -1;
            ResetStartTime ();
        }
    }
}