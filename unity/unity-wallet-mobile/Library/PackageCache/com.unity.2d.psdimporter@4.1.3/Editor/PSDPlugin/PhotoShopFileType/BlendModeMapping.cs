/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2014 Tao Yue
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using PaintDotNet;
using PhotoshopFile;
using PDNWrapper;

namespace PaintDotNet.Data.PhotoshopFileType
{
    internal static class BlendModeMapping
    {
        /// <summary>
        /// Convert between Paint.NET and Photoshop blend modes.
        /// </summary>
        public static string ToPsdBlendMode(this LayerBlendMode pdnBlendMode)
        {
            switch (pdnBlendMode)
            {
                case LayerBlendMode.Normal:
                    return PsdBlendMode.Normal;

                case LayerBlendMode.Multiply:
                    return PsdBlendMode.Multiply;
                case LayerBlendMode.Additive:
                    return PsdBlendMode.LinearDodge;
                case LayerBlendMode.ColorBurn:
                    return PsdBlendMode.ColorBurn;
                case LayerBlendMode.ColorDodge:
                    return PsdBlendMode.ColorDodge;
                case LayerBlendMode.Overlay:
                    return PsdBlendMode.Overlay;
                case LayerBlendMode.Difference:
                    return PsdBlendMode.Difference;
                case LayerBlendMode.Lighten:
                    return PsdBlendMode.Lighten;
                case LayerBlendMode.Darken:
                    return PsdBlendMode.Darken;
                case LayerBlendMode.Screen:
                    return PsdBlendMode.Screen;

                // Paint.NET blend modes without a Photoshop equivalent are saved
                // as Normal.
                case LayerBlendMode.Glow:
                case LayerBlendMode.Negation:
                case LayerBlendMode.Reflect:
                case LayerBlendMode.Xor:
                    return PsdBlendMode.Normal;

                default:
                    Debug.Fail("Unknown Paint.NET blend mode.");
                    return PsdBlendMode.Normal;
            }
        }

        /// <summary>
        /// Convert a Photoshop blend mode to a Paint.NET BlendOp.
        /// </summary>
        public static LayerBlendMode FromPsdBlendMode(string blendModeKey)
        {
            switch (blendModeKey)
            {
                case PsdBlendMode.Normal:
                    return LayerBlendMode.Normal;

                case PsdBlendMode.Multiply:
                    return LayerBlendMode.Multiply;
                case PsdBlendMode.LinearDodge:
                    return LayerBlendMode.Additive;
                case PsdBlendMode.ColorBurn:
                    return LayerBlendMode.ColorBurn;
                case PsdBlendMode.ColorDodge:
                    return LayerBlendMode.ColorDodge;
                case PsdBlendMode.Overlay:
                    return LayerBlendMode.Overlay;
                case PsdBlendMode.Difference:
                    return LayerBlendMode.Difference;
                case PsdBlendMode.Lighten:
                    return LayerBlendMode.Lighten;
                case PsdBlendMode.Darken:
                    return LayerBlendMode.Darken;
                case PsdBlendMode.Screen:
                    return LayerBlendMode.Screen;

                // Photoshop blend modes without a Paint.NET equivalent are loaded
                // as Normal.
                default:
                    return LayerBlendMode.Normal;
            }
        }
    }
}
