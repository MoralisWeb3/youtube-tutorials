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
    class ShaderColor : TweenBase
    {
        //Public Properties:
        public Color EndValue {get; private set;}

        //Private Variables:
        Material _target;
        Color _start;
        string _propertyName;
        
        //Constructor:
        public ShaderColor (Material target, string propertyName, Color endValue, float duration, float delay, bool obeyTimescale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            //set essential properties:
            SetEssentials (Tween.TweenType.ShaderColor, target.GetInstanceID (), duration, delay, obeyTimescale, curve, loop, startCallback, completeCallback);
            
            //catalog custom properties:
            _target = target;
            _propertyName = propertyName;
            EndValue = endValue;
        }
        
        //Processes:
        protected override bool SetStartValue ()
        {
            _start = _target.GetColor (_propertyName);
            if (_target == null) return false;
            return true;
        }

        protected override void Operation (float percentage)
        {
            Color calculatedValue = TweenUtilities.LinearInterpolate (_start, EndValue, percentage);
            _target.SetColor (_propertyName, calculatedValue);
        }
        
        //Loops:
        public override void Loop ()
        {
            ResetStartTime ();
            _target.SetColor (_propertyName, _start);
        }
        
        public override void PingPong ()
        {
            ResetStartTime ();
            _target.SetColor (_propertyName, EndValue);
            EndValue = _start;
            _start = _target.GetColor (_propertyName);
        }
    }
}