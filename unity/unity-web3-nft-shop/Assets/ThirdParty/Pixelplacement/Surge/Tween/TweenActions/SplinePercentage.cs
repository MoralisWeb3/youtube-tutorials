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
    class SplinePercentage : TweenBase
    {
        //Public Properties:
        public float EndValue {get; private set;}

        //Private Variables:
        Transform _target;
        Spline _spline;
        float _startPercentage;
        bool _faceDirection;

        //Constructor:
        public SplinePercentage (Spline spline, Transform target, float startPercentage, float endPercentage, bool faceDirection, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //clamps:
            if (!spline.loop)
            {
                startPercentage = Mathf.Clamp01 (startPercentage);
                endPercentage = Mathf.Clamp01 (endPercentage);
            }

            //set essential properties:
            SetEssentials (Tween.TweenType.Spline, target.GetInstanceID (), duration, delay, obeyTimescale, curve, loop, startCallback, completeCallback);

            //catalog custom properties:
            _spline = spline;
            _target = target;
            EndValue = endPercentage;
            _startPercentage = startPercentage;
            _faceDirection = faceDirection;
        }

        //Operation:
        protected override bool SetStartValue ()
        {
            if (_target == null) return false;
            return true;
        }

        protected override void Operation (float percentage)
        {
            float calculatedValue = TweenUtilities.LinearInterpolate (_startPercentage, EndValue, percentage);
            _target.position = _spline.GetPosition (calculatedValue);
            if (_faceDirection)
            {
                if (_spline.direction == SplineDirection.Forward)
                {
                    _target.LookAt (_target.position + _spline.GetDirection (calculatedValue));
                }else{
                    _target.LookAt (_target.position - _spline.GetDirection (calculatedValue));
                }
            }
        }

        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
            _target.position = _spline.GetPosition (_startPercentage);
        }

        public override void PingPong ()
        {
            ResetStartTime ();
            float temp = EndValue;
            EndValue = _startPercentage;
            _startPercentage = temp;
        }
    }
}