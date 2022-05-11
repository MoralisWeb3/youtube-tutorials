/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Tween singleton and execution engine.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace Pixelplacement
{
    public class Tween
    {
        /// <summary>
        /// Used internally to identify the type of tween to carry out.
        /// </summary>
        public enum TweenType { Position, Rotation, LocalScale, LightColor, CameraBackgroundColor, LightIntensity, LightRange, FieldOfView, SpriteRendererColor, GraphicColor, AnchoredPosition, Size, Volume, Pitch, PanStereo, ShaderFloat, ShaderColor, ShaderInt, ShaderVector, Value, CanvasGroupAlpha, Spline, TextColor, ImageColor, RawImageColor, TextMeshColor };

        /// <summary>
        /// What style of loop, if any, should be applied to this tween.
        /// </summary>
        public enum LoopType { None, Loop, PingPong };

        /// <summary>
        /// Used internally to identify the status of tween.
        /// </summary>
        public enum TweenStatus { Delayed, Running, Canceled, Stopped, Finished }

        //Public Properties:
        public static TweenSystem.TweenEngine Instance
        {
            get
            {
                if (_instance == null) _instance = new GameObject("(Tween Engine)", typeof(TweenSystem.TweenEngine)).GetComponent<TweenSystem.TweenEngine>();
                GameObject.DontDestroyOnLoad(_instance.gameObject);
                //_instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
                return _instance;
            }
        }

        //Public Variables:
        public static List<TweenSystem.TweenBase> activeTweens = new List<TweenSystem.TweenBase>();

        //Private Variables:
        private static TweenSystem.TweenEngine _instance;
        private static AnimationCurve _easeIn;
        private static AnimationCurve _easeInStrong;
        private static AnimationCurve _easeOut;
        private static AnimationCurve _easeOutStrong;
        private static AnimationCurve _easeInOut;
        private static AnimationCurve _easeInOutStrong;
        private static AnimationCurve _easeInBack;
        private static AnimationCurve _easeOutBack;
        private static AnimationCurve _easeInOutBack;
        private static AnimationCurve _easeSpring;
        private static AnimationCurve _easeBounce;
        private static AnimationCurve _easeWobble;

        //Public Methods:
        //tween callers:
        /// <summary>
        /// Shakes a Transform by a diminishing amount.
        /// </summary>
        public static TweenSystem.TweenBase Shake(Transform target, Vector3 initialPosition, Vector3 intensity, float duration, float delay, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ShakePosition tween = new TweenSystem.ShakePosition(target, initialPosition, intensity, duration, delay, EaseLinear, startCallback, completeCallback, loop, obeyTimescale);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Moves a Transform along a spline path from a start percentage to an end percentage.
        /// </summary>
        public static TweenSystem.TweenBase Spline(Spline spline, Transform target, float startPercentage, float endPercentage, bool faceDirection, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.SplinePercentage tween = new TweenSystem.SplinePercentage(spline, target, startPercentage, endPercentage, faceDirection, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the alpha of a Canvas object.
        /// </summary>
        public static TweenSystem.TweenBase CanvasGroupAlpha(CanvasGroup target, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.CanvasGroupAlpha tween = new TweenSystem.CanvasGroupAlpha(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the alpha of a Canvas object.
        /// </summary>
        public static TweenSystem.TweenBase CanvasGroupAlpha(CanvasGroup target, float startValue, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.alpha = startValue;
            return CanvasGroupAlpha(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Sends a Rect to a callback method as it tweens from a start to an end value. Note that Value tweens do not interrupt currently running Value tweens of the same type - catalog a reference by setting your tween to a Tweenbase variable so you can interrupt as needed. 
        /// </summary>
        public static TweenSystem.TweenBase Value(Rect startValue, Rect endValue, Action<Rect> valueUpdatedCallback, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ValueRect tween = new TweenSystem.ValueRect(startValue, endValue, valueUpdatedCallback, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Sends a Vector4 to a callback method as it tweens from a start to an end value. Note that Value tweens do not interrupt currently running Value tweens of the same type - catalog a reference by setting your tween to a Tweenbase variable so you can interrupt as needed. 
        /// </summary>
        public static TweenSystem.TweenBase Value(Vector4 startValue, Vector4 endValue, Action<Vector4> valueUpdatedCallback, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ValueVector4 tween = new TweenSystem.ValueVector4(startValue, endValue, valueUpdatedCallback, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween);
            return tween;
        }

        /// <summary>
        /// Sends a Vector3 to a callback method as it tweens from a start to an end value. Note that Value tweens do not interrupt currently running Value tweens of the same type - catalog a reference by setting your tween to a Tweenbase variable so you can interrupt as needed. 
        /// </summary>
        public static TweenSystem.TweenBase Value(Vector3 startValue, Vector3 endValue, Action<Vector3> valueUpdatedCallback, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ValueVector3 tween = new TweenSystem.ValueVector3(startValue, endValue, valueUpdatedCallback, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween);
            return tween;
        }

        /// <summary>
        /// Sends a Vector2 to a callback method as it tweens from a start to an end value. Note that Value tweens do not interrupt currently running Value tweens of the same type - catalog a reference by setting your tween to a Tweenbase variable so you can interrupt as needed. 
        /// </summary>
        public static TweenSystem.TweenBase Value(Vector2 startValue, Vector2 endValue, Action<Vector2> valueUpdatedCallback, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ValueVector2 tween = new TweenSystem.ValueVector2(startValue, endValue, valueUpdatedCallback, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween);
            return tween;
        }

        /// <summary>
        /// Sends a color to a callback method as it tweens from a start to an end value. Note that Value tweens do not interrupt currently running Value tweens of the same type - catalog a reference by setting your tween to a Tweenbase variable so you can interrupt as needed. 
        /// </summary>
        public static TweenSystem.TweenBase Value(Color startValue, Color endValue, Action<Color> valueUpdatedCallback, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ValueColor tween = new TweenSystem.ValueColor(startValue, endValue, valueUpdatedCallback, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween);
            return tween;
        }

        /// <summary>
        /// Sends an int to a callback method as it tweens from a start to an end value. Note that Value tweens do not interrupt currently running Value tweens of the same type - catalog a reference by setting your tween to a Tweenbase variable so you can interrupt as needed. 
        /// </summary>
        public static TweenSystem.TweenBase Value(int startValue, int endValue, Action<int> valueUpdatedCallback, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ValueInt tween = new TweenSystem.ValueInt(startValue, endValue, valueUpdatedCallback, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween);
            return tween;
        }

        /// <summary>
        /// Sends a float to a callback method as it tweens from a start to an end value. Note that Value tweens do not interrupt currently running Value tweens of the same type - catalog a reference by setting your tween to a Tweenbase variable so you can interrupt as needed. 
        /// </summary>
        public static TweenSystem.TweenBase Value(float startValue, float endValue, Action<float> valueUpdatedCallback, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ValueFloat tween = new TweenSystem.ValueFloat(startValue, endValue, valueUpdatedCallback, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween);
            return tween;
        }

        /// <summary>
        /// Changes the named vector property of a Material's shader.
        /// </summary>
        public static TweenSystem.TweenBase ShaderVector(Material target, string propertyName, Vector4 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ShaderVector tween = new TweenSystem.ShaderVector(target, propertyName, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the named vector property of a Material's shader.
        /// </summary>
        public static TweenSystem.TweenBase ShaderVector(Material target, string propertyName, Vector4 startValue, Vector4 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.SetVector(propertyName, startValue);
            return ShaderVector(target, propertyName, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the named int property of a Material's shader.
        /// </summary>
        public static TweenSystem.TweenBase ShaderInt(Material target, string propertyName, int endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ShaderInt tween = new TweenSystem.ShaderInt(target, propertyName, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the named int property of a Material's shader.
        /// </summary>
        public static TweenSystem.TweenBase ShaderInt(Material target, string propertyName, int startValue, int endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.SetInt(propertyName, startValue);
            return ShaderInt(target, propertyName, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the named color property of a Material's shader.
        /// </summary>
        public static TweenSystem.TweenBase ShaderColor(Material target, string propertyName, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ShaderColor tween = new TweenSystem.ShaderColor(target, propertyName, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the named color property of a Material's shader.
        /// </summary>
        public static TweenSystem.TweenBase ShaderColor(Material target, string propertyName, Color startValue, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.SetColor(propertyName, startValue);
            return ShaderColor(target, propertyName, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the named float property of a Material's shader.
        /// </summary>
        public static TweenSystem.TweenBase ShaderFloat(Material target, string propertyName, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ShaderFloat tween = new TweenSystem.ShaderFloat(target, propertyName, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the named float property of a Material's shader.
        /// </summary>
        public static TweenSystem.TweenBase ShaderFloat(Material target, string propertyName, float startValue, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.SetFloat(propertyName, startValue);
            return ShaderFloat(target, propertyName, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the pitch of an AudioSource.
        /// </summary>
        public static TweenSystem.TweenBase Pitch(AudioSource target, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.Pitch tween = new TweenSystem.Pitch(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the pitch of an AudioSource.
        /// </summary>
        public static TweenSystem.TweenBase Pitch(AudioSource target, float startValue, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.pitch = startValue;
            return Pitch(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the stereo pan of an AudioSource.
        /// </summary>
        public static TweenSystem.TweenBase PanStereo(AudioSource target, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.PanStereo tween = new TweenSystem.PanStereo(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the stereo pan of an AudioSource.
        /// </summary>
        public static TweenSystem.TweenBase PanStereo(AudioSource target, float startValue, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.panStereo = startValue;
            return PanStereo(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the volume of an AudioSource.
        /// </summary>
        public static TweenSystem.TweenBase Volume(AudioSource target, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.Volume tween = new TweenSystem.Volume(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the volume of an AudioSource.
        /// </summary>
        public static TweenSystem.TweenBase Volume(AudioSource target, float startValue, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.volume = startValue;
            return Volume(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the width and height of a RectTransform.
        /// </summary>
        public static TweenSystem.TweenBase Size(RectTransform target, Vector2 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.Size tween = new TweenSystem.Size(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the width and height of a RectTransform.
        /// </summary>
        public static TweenSystem.TweenBase Size(RectTransform target, Vector2 startValue, Vector2 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.sizeDelta = startValue;
            return Size(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the field of view (FOV) of a camera.
        /// </summary>
        public static TweenSystem.TweenBase FieldOfView(Camera target, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.FieldOfView tween = new TweenSystem.FieldOfView(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the field of view (FOV) of a camera.
        /// </summary>
        public static TweenSystem.TweenBase FieldOfView(Camera target, float startValue, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.fieldOfView = startValue;
            return FieldOfView(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the range of a light.
        /// </summary>
        public static TweenSystem.TweenBase LightRange(Light target, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.LightRange tween = new TweenSystem.LightRange(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the range of a light.
        /// </summary>
        public static TweenSystem.TweenBase LightRange(Light target, float startValue, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.range = startValue;
            return LightRange(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the intensity of a light.
        /// </summary>
        public static TweenSystem.TweenBase LightIntensity(Light target, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.LightIntensity tween = new TweenSystem.LightIntensity(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the intensity of a light.
        /// </summary>
        public static TweenSystem.TweenBase LightIntensity(Light target, float startValue, float endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.intensity = startValue;
            return LightIntensity(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Scales a Transform.
        /// </summary>
        public static TweenSystem.TweenBase LocalScale(Transform target, Vector3 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.LocalScale tween = new TweenSystem.LocalScale(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Scales a Transform.
        /// </summary>
        public static TweenSystem.TweenBase LocalScale(Transform target, Vector3 startValue, Vector3 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.localScale = startValue;
            return LocalScale(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }
        
        /// <summary>
        /// Changes the color of graphic.
        /// </summary>
        public static TweenSystem.TweenBase Color(Graphic target, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.GraphicColor tween = new TweenSystem.GraphicColor(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the color of a graphic.
        /// </summary>
        public static TweenSystem.TweenBase Color(Graphic target, Color startValue, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.color = startValue;
            return Color(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }        
        
        /// <summary>
        /// Changes the color of a light.
        /// </summary>
        public static TweenSystem.TweenBase Color(Light target, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.LightColor tween = new TweenSystem.LightColor(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the color of a light.
        /// </summary>
        public static TweenSystem.TweenBase Color(Light target, Color startValue, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.color = startValue;
            return Color(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the color of a Material's "_Color" propery (for property name access see ShaderColor instead).
        /// </summary>
        public static TweenSystem.TweenBase Color(Material target, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.ShaderColor tween = new TweenSystem.ShaderColor(target, "_Color", endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the color of a Material's "_Color" propery (for property name access see ShaderColor instead).
        /// </summary>
        public static TweenSystem.TweenBase Color(Material target, Color startColor, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.color = startColor;
            return Color(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the color of a Renderer's material "_Color" propery (for property name access see ShaderColor instead).
        /// This version is a wrapper for the color tween that targets a material.
        /// </summary>
        public static TweenSystem.TweenBase Color(Renderer target, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            Material material = target.material;
            return Color(material, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the color of a Renderer's material propery (for property name access see ShaderColor instead).
        /// This version is a wrapper for the color tween that targets a material.
        /// </summary>
        public static TweenSystem.TweenBase Color(Renderer target, Color startColor, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            Material material = target.material;
            return Color(material, startColor, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the color of a SpriteRenderer.
        /// </summary>
        public static TweenSystem.TweenBase Color(SpriteRenderer target, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.SpriteRendererColor tween = new TweenSystem.SpriteRendererColor(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the color of a SpriteRenderer.
        /// </summary>
        public static TweenSystem.TweenBase Color(SpriteRenderer target, Color startColor, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.color = startColor;
            return Color(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Changes the color of a Camera's background.
        /// </summary>
        public static TweenSystem.TweenBase Color(Camera target, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.CameraBackgroundColor tween = new TweenSystem.CameraBackgroundColor(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Changes the color of a Camera's background.
        /// </summary>
        public static TweenSystem.TweenBase Color(Camera target, Color startColor, Color endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.backgroundColor = startColor;
            return Color(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Moves the Transform of a GameObject in world coordinates.
        /// </summary>
        public static TweenSystem.TweenBase Position(Transform target, Vector3 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.Position tween = new TweenSystem.Position(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Moves the Transform of a GameObject in world coordinates.
        /// </summary>
        public static TweenSystem.TweenBase Position(Transform target, Vector3 startValue, Vector3 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.position = startValue;
            return Position(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Moves a RectTransform relative to it's reference anchor.
        /// </summary>
        public static TweenSystem.TweenBase AnchoredPosition(RectTransform target, Vector2 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.AnchoredPosition tween = new TweenSystem.AnchoredPosition(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Moves a RectTransform relative to it's reference anchor.
        /// </summary>
        public static TweenSystem.TweenBase AnchoredPosition(RectTransform target, Vector2 startValue, Vector2 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.anchoredPosition = startValue;
            return AnchoredPosition(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Moves the Transform of a GameObject in local coordinates.
        /// </summary>
        public static TweenSystem.TweenBase LocalPosition(Transform target, Vector3 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.LocalPosition tween = new TweenSystem.LocalPosition(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Moves the Transform of a GameObject in local coordinates.
        /// </summary>
        public static TweenSystem.TweenBase LocalPosition(Transform target, Vector3 startValue, Vector3 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.localPosition = startValue;
            return LocalPosition(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Rotates the Transform of a GameObject by a Vector3 amount. This is different from Rotation and LocalRotation in that it does not set the rotation it rotates by the supplied amount.
        /// </summary>
        public static TweenSystem.TweenBase Rotate(Transform target, Vector3 amount, Space space, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.Rotate tween = new TweenSystem.Rotate(target, amount, space, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Rotates the Transform of a GameObject with a Vector3 destination in world coordinates.
        /// </summary>
        public static TweenSystem.TweenBase Rotation(Transform target, Vector3 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            endValue = Quaternion.Euler(endValue).eulerAngles;
            TweenSystem.Rotation tween = new TweenSystem.Rotation(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Rotates the Transform of a GameObject with a Vector3 destination in world coordinates.
        /// </summary>
        public static TweenSystem.TweenBase Rotation(Transform target, Vector3 startValue, Vector3 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            startValue = Quaternion.Euler(startValue).eulerAngles;
            endValue = Quaternion.Euler(endValue).eulerAngles;
            target.rotation = Quaternion.Euler(startValue);
            return Rotation(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Rotates the Transform of a GameObject with a Quaternion destination in world coordinates.
        /// </summary>
        public static TweenSystem.TweenBase Rotation(Transform target, Quaternion endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.Rotation tween = new TweenSystem.Rotation(target, endValue.eulerAngles, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Rotates the Transform of a GameObject with a Quaternion destination in world coordinates.
        /// </summary>
        public static TweenSystem.TweenBase Rotation(Transform target, Quaternion startValue, Quaternion endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.rotation = startValue;
            return Rotation(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Rotates the Transform of a GameObject with a Vector3 destination in local coordinates.
        /// </summary>
        public static TweenSystem.TweenBase LocalRotation(Transform target, Vector3 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            endValue = Quaternion.Euler(endValue).eulerAngles;
            TweenSystem.LocalRotation tween = new TweenSystem.LocalRotation(target, endValue, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Rotates the Transform of a GameObject with a Vector3 destination in local coordinates.
        /// </summary>
        public static TweenSystem.TweenBase LocalRotation(Transform target, Vector3 startValue, Vector3 endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            startValue = Quaternion.Euler(startValue).eulerAngles;
            endValue = Quaternion.Euler(endValue).eulerAngles;
            target.localRotation = Quaternion.Euler(startValue);
            return LocalRotation(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Rotates the Transform of a GameObject with a Quaternion destination in local coordinates.
        /// </summary>
        public static TweenSystem.TweenBase LocalRotation(Transform target, Quaternion endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            TweenSystem.LocalRotation tween = new TweenSystem.LocalRotation(target, endValue.eulerAngles, duration, delay, obeyTimescale, easeCurve, loop, startCallback, completeCallback);
            SendTweenForProcessing(tween, true);
            return tween;
        }

        /// <summary>
        /// Rotates the Transform of a GameObject with a Quaternion destination in local coordinates.
        /// </summary>
        public static TweenSystem.TweenBase LocalRotation(Transform target, Quaternion startValue, Quaternion endValue, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            target.localRotation = startValue;
            return LocalRotation(target, endValue, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Rotates a LookAt operation towards a Transform.
        /// </summary>
        public static TweenSystem.TweenBase LookAt(Transform target, Transform targetToLookAt, Vector3 up, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            Vector3 direction = targetToLookAt.position - target.position;
            Quaternion rotation = Quaternion.LookRotation(direction, up);
            return Rotation(target, rotation, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        /// <summary>
        /// Rotates a LookAt operation towards a Vector3.
        /// </summary>
        public static TweenSystem.TweenBase LookAt(Transform target, Vector3 positionToLookAt, Vector3 up, float duration, float delay, AnimationCurve easeCurve = null, LoopType loop = LoopType.None, Action startCallback = null, Action completeCallback = null, bool obeyTimescale = true)
        {
            Vector3 direction = positionToLookAt - target.position;
            Quaternion rotation = Quaternion.LookRotation(direction, up);
            return Rotation(target, rotation, duration, delay, easeCurve, loop, startCallback, completeCallback, obeyTimescale);
        }

        //utilities:

        /// <summary>
        /// Stop a specific type of tween on a target. Use GetInstanceID() of your target for this method.
        /// </summary>
        public static void Stop(int targetInstanceID, TweenType tweenType)
        {
            if (targetInstanceID == -1) return;
            for (int i = 0; i < activeTweens.Count; i++)
            {
                if (activeTweens[i].targetInstanceID == targetInstanceID && activeTweens[i].tweenType == tweenType && activeTweens[i].Status != TweenStatus.Delayed)
                {
                    activeTweens[i].Stop();
                }
            }
        }

        /// <summary>
        /// Stop all tweens on a target. Use GetInstanceID() of your target for this method.
        /// </summary>
        public static void Stop(int targetInstanceID)
        {
            StopInstanceTarget(targetInstanceID);
        }

        /// <summary>
        /// Stops all tweens.
        /// </summary>
        public static void StopAll()
        {
            foreach (TweenSystem.TweenBase item in activeTweens)
            {
                item.Stop();
            }
        }

        /// <summary>
        /// Finishes all tweens - sets them to their final value and stops them.
        /// </summary>
        public static void FinishAll()
        {
            foreach (TweenSystem.TweenBase item in activeTweens)
            {
                item.Finish();
            }
        }

        /// <summary>
        /// Finishes a tween. Sets it to final value and stops it.
        /// </summary>
        public static void Finish(int targetInstanceID)
        {
            FinishInstanceTarget(targetInstanceID);
        }

        /// <summary>
        /// Cancels a tween - rewinds values to where they started and stops.
        /// </summary>
        public static void Cancel(int targetInstanceID)
        {
            CancelInstanceTarget(targetInstanceID);
        }

        /// <summary>
        /// Cancels all tweens - rewinds their values to where they started and stops them.
        /// </summary>
        public static void CancelAll()
        {
            foreach (TweenSystem.TweenBase item in activeTweens)
            {
                item.Cancel();
            }
        }

        //Public Properties - Ease Curves:
        /// <summary>
        /// A linear curve.
        /// </summary>
        public static AnimationCurve EaseLinear
        {
            get { return null; }
        }

        /// <summary>
        /// A curve that runs slow in the beginning.
        /// </summary>
        public static AnimationCurve EaseIn
        {
            get
            {
                if(_easeIn == null) _easeIn = new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 1, 0));
                return _easeIn;
            }
        }

        /// <summary>
        /// A curve that runs slow in the beginning but with more overall energy.
        /// </summary>
        public static AnimationCurve EaseInStrong
        {
            get
            {
                if (_easeInStrong == null) _easeInStrong = new AnimationCurve(new Keyframe(0, 0, .03f, .03f), new Keyframe(0.45f, 0.03f, 0.2333333f, 0.2333333f), new Keyframe(0.7f, 0.13f, 0.7666667f, 0.7666667f), new Keyframe(0.85f, 0.3f, 2.233334f, 2.233334f), new Keyframe(0.925f, 0.55f, 4.666665f, 4.666665f), new Keyframe(1, 1, 5.999996f, 5.999996f));
                return _easeInStrong;
            }
        }

        /// <summary>
        /// A curve that runs slow at the end.
        /// </summary>
        public static AnimationCurve EaseOut
        {
            get
            {
                if (_easeOut == null) _easeOut = new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 0, 0));
                return _easeOut;
            }
        }

        /// <summary>
        /// A curve that runs slow at the end but with more overall energy.
        /// </summary>
        public static AnimationCurve EaseOutStrong
        {
            get
            {
                if(_easeOutStrong == null) _easeOutStrong = new AnimationCurve(new Keyframe(0, 0, 13.80198f, 13.80198f), new Keyframe(0.04670785f, 0.3973127f, 5.873408f, 5.873408f), new Keyframe(0.1421811f, 0.7066917f, 2.313627f, 2.313627f), new Keyframe(0.2483539f, 0.8539293f, 0.9141542f, 0.9141542f), new Keyframe(0.4751028f, 0.954047f, 0.264541f, 0.264541f), new Keyframe(1, 1, .03f, .03f));
                return _easeOutStrong;
            }
        }

        /// <summary>
        /// A curve that runs slow in the beginning and at the end.
        /// </summary>
        public static AnimationCurve EaseInOut
        {
            get
            {
                if(_easeInOut == null) _easeInOut = AnimationCurve.EaseInOut(0, 0, 1, 1);
                return _easeInOut;
            }
        }

        /// <summary>
        /// A curve that runs slow in the beginning and the end but with more overall energy.
        /// </summary>
        public static AnimationCurve EaseInOutStrong
        {
            get
            {
                if(_easeInOutStrong == null) _easeInOutStrong = new AnimationCurve(new Keyframe(0, 0, 0.03f, 0.03f), new Keyframe(0.5f, 0.5f, 3.257158f, 3.257158f), new Keyframe(1, 1, .03f, .03f));
                return _easeInOutStrong;
            }
        }

        /// <summary>
        /// A curve that appears to "charge up" before it moves.
        /// </summary>
        public static AnimationCurve EaseInBack
        {
            get
            {
                if(_easeInBack == null) _easeInBack = new AnimationCurve(new Keyframe(0, 0, -1.1095f, -1.1095f), new Keyframe(1, 1, 2, 2));
                return _easeInBack;
            }
        }

        /// <summary>
        /// A curve that shoots past its target and springs back into place.
        /// </summary>
        public static AnimationCurve EaseOutBack
        {
            get
            {
                if (_easeOutBack == null) _easeOutBack = new AnimationCurve(new Keyframe(0, 0, 2, 2), new Keyframe(1, 1, -1.1095f, -1.1095f));
                return _easeOutBack;
            }
        }

        /// <summary>
        /// A curve that appears to "charge up" before it moves, shoots past its target and springs back into place.
        /// </summary>
        public static AnimationCurve EaseInOutBack
        {
            get
            {
                if(_easeInOutBack == null) _easeInOutBack = new AnimationCurve(new Keyframe(1, 1, -1.754543f, -1.754543f), new Keyframe(0, 0, -1.754543f, -1.754543f));
                return _easeInOutBack;
            }
        }

        /// <summary>
        /// A curve that snaps to its value with a fun spring motion.
        /// </summary>
        public static AnimationCurve EaseSpring
        {
            get
            {
                if(_easeSpring == null) _easeSpring = new AnimationCurve(new Keyframe(0f, -0.0003805831f, 8.990726f, 8.990726f), new Keyframe(0.35f, 1f, -4.303913f, -4.303913f), new Keyframe(0.55f, 1f, 1.554695f, 1.554695f), new Keyframe(0.7730452f, 1f, -2.007816f, -2.007816f), new Keyframe(1f, 1f, -1.23451f, -1.23451f));
                return _easeSpring;
            }
        }

        /// <summary>
        /// A curve that settles to its value with a fun bounce motion.
        /// </summary>
        public static AnimationCurve EaseBounce
        {
            get
            {
                if(_easeBounce == null) _easeBounce = new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(0.25f, 1, 11.73749f, -5.336508f), new Keyframe(0.4f, 0.6f, -0.1904764f, -0.1904764f), new Keyframe(0.575f, 1, 5.074602f, -3.89f), new Keyframe(0.7f, 0.75f, 0.001192093f, 0.001192093f), new Keyframe(0.825f, 1, 4.18469f, -2.657566f), new Keyframe(0.895f, 0.9f, 0, 0), new Keyframe(0.95f, 1, 3.196362f, -2.028364f), new Keyframe(1, 1, 2.258884f, 0.5f));
                return _easeBounce;
            }
        }

        /// <summary>
        /// A curve that appears to apply a jolt that wobbles back down to where it was.
        /// </summary>
        public static AnimationCurve EaseWobble
        {
            get
            {
                if(_easeWobble == null) _easeWobble = new AnimationCurve(new Keyframe(0f, 0f, 11.01978f, 30.76278f), new Keyframe(0.08054394f, 1f, 0f, 0f), new Keyframe(0.3153235f, -0.75f, 0f, 0f), new Keyframe(0.5614113f, 0.5f, 0f, 0f), new Keyframe(0.75f, -0.25f, 0f, 0f), new Keyframe(0.9086903f, 0.1361611f, 0f, 0f), new Keyframe(1f, 0f, -4.159244f, -1.351373f));
                return _easeWobble;
            }
        }

        //Private Methods:
        static void StopInstanceTarget(int id)
        {
            for (int i = 0; i < activeTweens.Count; i++)
            {
                if (activeTweens[i].targetInstanceID == id)
                {
                    activeTweens[i].Stop();
                }
            }
        }

        static void StopInstanceTargetType(int id, TweenType type)
        {
            for (int i = activeTweens.Count - 1; i >= 0; i--)
            {
                if (activeTweens[i].targetInstanceID == id && activeTweens[i].tweenType == type)
                {
                    activeTweens[i].Stop();
                }
            }
        }

        static void FinishInstanceTarget(int id)
        {
            for (int i = activeTweens.Count - 1; i >= 0; i--)
            {
                if (activeTweens[i].targetInstanceID == id)
                {
                    activeTweens[i].Finish();
                    //activeTweens.RemoveAt(i); needs validation
                }
            }
        }

        static void CancelInstanceTarget(int id)
        {
            for (int i = activeTweens.Count - 1; i >= 0; i--)
            {
                if (activeTweens[i].targetInstanceID == id)
                {
                    activeTweens[i].Cancel();
                    //activeTweens.RemoveAt(i); needs validation
                }
            }
        }

        static void SendTweenForProcessing(TweenSystem.TweenBase tween, bool interrupt = false)
        {
            if (!Application.isPlaying) 
            {
                //Tween can not be called in edit mode!
                return;
            }

            if (interrupt && tween.Delay == 0)
            {
                StopInstanceTargetType(tween.targetInstanceID, tween.tweenType);
            }

            Instance.ExecuteTween(tween);
        }
    }
}