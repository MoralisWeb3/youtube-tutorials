/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Math helpers for interpolation and curve evaluation.
/// 
/// </summary>

using UnityEngine;
using Pixelplacement;

namespace Pixelplacement.TweenSystem
{
    public class TweenUtilities : MonoBehaviour
    {
        //Public Methods:
        /// <summary>
        /// Generates the code to contain an Animation Curve in a property for use in storing ease curves within the Tween engine class.
        /// </summary>
        public static void GenerateAnimationCurvePropertyCode (AnimationCurve curve)
        {
            string code = "get { return new AnimationCurve (";
            for (int i = 0; i < curve.keys.Length; i++) 
            {
                Keyframe currentFrame = curve.keys [i];
                code += "new Keyframe (" + currentFrame.time + "f, " + currentFrame.value + "f, " + currentFrame.inTangent + "f, " + currentFrame.outTangent + "f)";
                if (i < curve.keys.Length - 1) code += ", ";
            }
            code += "); }";

            Debug.Log (code);
        }

        /// <summary>
        /// Linear interpolation for float.
        /// </summary>
        public static float LinearInterpolate (float from, float to, float percentage)
        {
            return (to - from) * percentage + from; //this approach is needed instead of a simple Mathf.Lerp to accommodate ease cuves that overshoot
        }

        /// <summary>
        /// Linear interpolation for Vector2.
        /// </summary>
        public static Vector2 LinearInterpolate (Vector2 from, Vector2 to, float percentage)
        {
            return new Vector2 (LinearInterpolate (from.x, to.x, percentage), LinearInterpolate (from.y, to.y, percentage));
        }

        /// <summary>
        /// Linear interpolation for Vector3.
        /// </summary>
        public static Vector3 LinearInterpolate (Vector3 from, Vector3 to, float percentage)
        {
            return new Vector3 (LinearInterpolate (from.x, to.x, percentage), LinearInterpolate (from.y, to.y, percentage), LinearInterpolate (from.z, to.z, percentage));
        }

        /// <summary>
        /// Linear interpolation for Rotations.
        /// </summary>
        public static Vector3 LinearInterpolateRotational (Vector3 from, Vector3 to, float percentage)
        {
            return new Vector3 (CylindricalLerp (from.x, to.x, percentage), CylindricalLerp (from.y, to.y, percentage), CylindricalLerp (from.z, to.z, percentage));
        }

        /// <summary>
        /// Linear interpolation for Vector4.
        /// </summary>
        public static Vector4 LinearInterpolate (Vector4 from, Vector4 to, float percentage)
        {
            return new Vector4 (LinearInterpolate (from.x, to.x, percentage), LinearInterpolate (from.y, to.y, percentage), LinearInterpolate (from.z, to.z, percentage), LinearInterpolate (from.w, to.w, percentage));
        }

        /// <summary>
        /// Linear interpolation for Rect.
        /// </summary>
        public static Rect LinearInterpolate (Rect from, Rect to, float percentage)
        {
            return new Rect (LinearInterpolate (from.x, to.x, percentage), LinearInterpolate (from.y, to.y, percentage), LinearInterpolate (from.width, to.width, percentage), LinearInterpolate (from.height, to.height, percentage));
        }

        /// <summary>
        /// Linear interpolation for Color.
        /// </summary>
        public static Color LinearInterpolate (Color from, Color to, float percentage)
        {
            return new Color (LinearInterpolate (from.r, to.r, percentage), LinearInterpolate (from.g, to.g, percentage), LinearInterpolate (from.b, to.b, percentage), LinearInterpolate (from.a, to.a, percentage));
        }

        /// <summary>
        /// Evaluates a curve at a percentage.
        /// </summary>
        public static float EvaluateCurve (AnimationCurve curve, float percentage)
        {
            return curve.Evaluate ((curve [curve.length - 1].time) * percentage);
        }

        //Private Methods:
        //use to handle rotational lerping so values that wrap around 360 don't spin in the wrong direction:
        static float CylindricalLerp (float from, float to, float percentage)
        {
            float min = 0f;
            float max = 360f;
            float half = Mathf.Abs ((max - min) * .5f);
            float calculation = 0f;
            float difference = 0f;

            if ((to - from) < -half)
            {
                difference = ((max - from) + to) * percentage;
                calculation = from + difference;
            }else if ((to - from) > half){
                difference = -((max - to) + from) * percentage;
                calculation = from + difference;
            }else{ 
                calculation = from + (to - from) * percentage;
            }

            return calculation;
        }
    }
}