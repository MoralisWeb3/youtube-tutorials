/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Math. Math. Math.
/// 
/// </summary>

using UnityEngine;

namespace Pixelplacement
{
    public static class CoreMath  
    {
        //interpolation:
        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static float LinearInterpolate (float from, float to, float percentage)
        {
            return (to - from) * percentage + from;
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Vector2 LinearInterpolate (Vector2 from, Vector2 to, float percentage)
        {
            return new Vector2 (LinearInterpolate (from.x, to.x, percentage), LinearInterpolate (from.y, to.y, percentage));
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Vector3 LinearInterpolate (Vector3 from, Vector3 to, float percentage)
        {
            return new Vector3 (LinearInterpolate (from.x, to.x, percentage), LinearInterpolate (from.y, to.y, percentage), LinearInterpolate (from.z, to.z, percentage));
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Vector4 LinearInterpolate (Vector4 from, Vector4 to, float percentage)
        {
            return new Vector4 (LinearInterpolate (from.x, to.x, percentage), LinearInterpolate (from.y, to.y, percentage), LinearInterpolate (from.z, to.z, percentage), LinearInterpolate (from.w, to.w, percentage));
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Rect LinearInterpolate (Rect from, Rect to, float percentage)
        {
            return new Rect (LinearInterpolate (from.x, to.x, percentage), LinearInterpolate (from.y, to.y, percentage), LinearInterpolate (from.width, to.width, percentage), LinearInterpolate (from.height, to.height, percentage));
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Color LinearInterpolate (Color from, Color to, float percentage)
        {
            return new Color (LinearInterpolate (from.r, to.r, percentage), LinearInterpolate (from.g, to.g, percentage), LinearInterpolate (from.b, to.b, percentage), LinearInterpolate (from.a, to.a, percentage));
        }

        //animation curves:
        /// <summary>
        /// Evaluates the curve.
        /// </summary>
        /// <returns>The value evaluated at the percentage of the clip.</returns>
        /// <param name="curve">Curve.</param>
        /// <param name="percentage">Percentage.</param>
        public static float EvaluateCurve (AnimationCurve curve, float percentage)
        {
            return curve.Evaluate ((curve [curve.length - 1].time) * percentage);
        }
    }
}