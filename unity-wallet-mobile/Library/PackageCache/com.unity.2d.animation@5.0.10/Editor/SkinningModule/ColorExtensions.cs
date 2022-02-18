using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal static class ColorExtensions
    {
        public static Color AlphaMultiplied(this Color c, float multiplier)
        {
            return new Color(c.r, c.g, c.b, c.a * multiplier);
        }
    }
}
