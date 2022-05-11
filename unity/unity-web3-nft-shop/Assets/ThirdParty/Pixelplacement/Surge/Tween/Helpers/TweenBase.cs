/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Base class for tweens.
/// 
/// </summary>

using UnityEngine;
using System;
using Pixelplacement;

#pragma warning disable 0168

namespace Pixelplacement.TweenSystem
{
    public abstract class TweenBase
    {
        //Public Variables:
        public int targetInstanceID;
        public Tween.TweenType tweenType;

        //Public Properties:
        public Tween.TweenStatus Status {get; private set;}
        public float Duration {get; private set;}
        public AnimationCurve Curve {get; private set;}
        public Keyframe[] CurveKeys { get; private set; }
        public bool ObeyTimescale {get; private set;}
        public Action StartCallback {get; private set;}
        public Action CompleteCallback {get; private set;}
        public float Delay {get; private set;}
        public Tween.LoopType LoopType {get; private set;}
        public float Percentage {get; private set; }

        //Protected Variables:
        protected float elapsedTime = 0.0f;

        //Public Methods:
        /// <summary>
        /// Stop/pauses the tween.
        /// </summary>
        public void Stop ()
        {
            Status = Tween.TweenStatus.Stopped;
            Tick ();
        }

        /// <summary>
        /// Starts or restarts a tween - interrupts a delay and allows a canceled or finished tween to restart.
        /// </summary>
        public void Start ()
        {
            elapsedTime = 0.0f;

            if (Status == Tween.TweenStatus.Canceled || Status == Tween.TweenStatus.Finished || Status == Tween.TweenStatus.Stopped)
            {
                Status = Tween.TweenStatus.Running;
                Operation (0);
                Tween.Instance.ExecuteTween (this);
            }
        }

        /// <summary>
        /// Resumes a stopped/paused tween.
        /// </summary>
        public void Resume()
        {
            if (Status != Tween.TweenStatus.Stopped) return;
            
            if (Status == Tween.TweenStatus.Stopped)
            {
                Status = Tween.TweenStatus.Running;
                Tween.Instance.ExecuteTween(this);
            }
        }

        /// <summary>
        /// Rewind the tween.
        /// </summary>
        public void Rewind ()
        {
            Cancel ();
            Operation (0);
        }

        /// <summary>
        /// Rewind the tween and stop.
        /// </summary>
        public void Cancel ()
        {
            Status = Tween.TweenStatus.Canceled;
            Tick ();
        }

        /// <summary>
        /// Fast forward the tween and stop.
        /// </summary>
        public void Finish ()
        {
            Status = Tween.TweenStatus.Finished;
            Tick ();
        }

        /// <summary>
        /// Used internally to update the tween and report status to the main system.
        /// </summary>
        public void Tick ()
        {
            //stop where we are:
            if (Status == Tween.TweenStatus.Stopped) 
            {
                CleanUp();
                return;
            }

            //rewind operation and stop:
            if (Status == Tween.TweenStatus.Canceled)
            {
                Operation (0);
                Percentage = 0;
                CleanUp();
                return;
            }

            //fast forward operation and stop:
            if (Status == Tween.TweenStatus.Finished) 
            {
                Operation (1);
                Percentage = 1;
                if (CompleteCallback != null) CompleteCallback ();
                CleanUp();
                return;
            }

            float progress = 0.0f;
            
            //calculate:
            if (ObeyTimescale) 
            {
                elapsedTime += Time.deltaTime;
            }else{
                elapsedTime += Time.unscaledDeltaTime;
            }
            progress = Math.Max(elapsedTime, 0f);

            //percentage:
            float percentage = Mathf.Min(progress / Duration, 1);
  
            //delayed?
            if (percentage == 0 && Status != Tween.TweenStatus.Delayed) Status = Tween.TweenStatus.Delayed;

            //running?
            if (percentage > 0 && Status == Tween.TweenStatus.Delayed) 
            {
                if (SetStartValue ())
                {
                    if (StartCallback != null) StartCallback ();
                    Status = Tween.TweenStatus.Running;	
                }else{
                    CleanUp();
                    return;
                }
            }

            //evaluate:
            float curveValue = percentage;

            //using a curve?
            if (Curve != null && CurveKeys.Length > 0) curveValue = TweenUtilities.EvaluateCurve (Curve, percentage);
        
            //perform operation with minimal overhead of a try/catch to account for anything that has been destroyed while tweening:
            if (Status == Tween.TweenStatus.Running) 
            {
                try {
                    Operation (curveValue);
                    Percentage = curveValue;
                } catch (Exception ex) {
                    CleanUp();
                    return;
                }
            }

            //tween complete:
            if (percentage == 1)
            {
                if (CompleteCallback != null)
                {
                    CompleteCallback();
                }

                switch (LoopType) 
                {
                case Tween.LoopType.Loop:
                    Loop ();
                    break;

                case Tween.LoopType.PingPong:
                    PingPong ();
                    break;

                default:
                    Status = Tween.TweenStatus.Finished;
                    CleanUp();
                    return;
                }
            }
        }
        
        //Private Methods:
        private void CleanUp()
        {
            if (Tween.activeTweens.Contains(this))
            {
                Tween.activeTweens.Remove(this);
            }
        }

        //Protected Methods:
        /// <summary>
        /// Resets the start time.
        /// </summary>
        protected void ResetStartTime ()
        {
            elapsedTime = -Delay;
        }

        /// <summary>
        /// Sets the essential properties that all tweens need and should be called from their constructor. If targetInstanceID is -1 then this tween won't interrupt tweens of the same type on the same target.
        /// </summary>
        protected void SetEssentials (Tween.TweenType tweenType, int targetInstanceID, float duration, float delay, bool obeyTimeScale, AnimationCurve curve, Tween.LoopType loop, Action startCallback, Action completeCallback)
        {
            this.tweenType = tweenType;
            this.targetInstanceID = targetInstanceID;

            if (delay > 0)
                Status = Tween.TweenStatus.Delayed;

            Duration = duration;
            Delay = delay;
            Curve = curve;

            CurveKeys = curve == null ? null : curve.keys;
            StartCallback = startCallback;
            CompleteCallback = completeCallback;
            LoopType = loop;
            ObeyTimescale = obeyTimeScale;

            ResetStartTime ();
        }

        //Abstract Methods:
        /// <summary>
        /// Override this method to carry out the initialization required for the tween.
        /// </summary>
        protected abstract bool SetStartValue ();

        /// <summary>
        /// Override this method to carry out the processing required for the tween.
        /// </summary>
        protected abstract void Operation (float percentage);

        /// <summary>
        /// Override this method to carry out a standard loop.
        /// </summary>
        public abstract void Loop ();

        /// <summary>
        /// Override this method to carry out a ping pong loop.
        /// </summary>
        public abstract void PingPong ();
    }
}