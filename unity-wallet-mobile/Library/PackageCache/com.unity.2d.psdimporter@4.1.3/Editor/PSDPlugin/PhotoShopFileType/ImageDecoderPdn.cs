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

using PaintDotNet;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PDNWrapper;
using System.Text;

using PhotoshopFile;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Unity.Jobs;

namespace PaintDotNet.Data.PhotoshopFileType
{

    internal enum DecodeType
    {
        RGB32 = 0,
        Grayscale32 = 1,
        RGB = 2,
        CMYK = 3,
        Bitmap = 4,
        Grayscale = 5,
        Indexed = 6,
        Lab = 7
    };

    internal static class ImageDecoderPdn
    {
        private static double rgbExponent = 1 / 2.19921875;

        private class DecodeContext
        {
            public PhotoshopFile.Layer Layer { get; private set; }
            public int ByteDepth { get; private set; }
            public int HasAlphaChannel { get; private set; }
            public Channel[] Channels { get; private set; }
            public NativeArray<byte> AlphaChannel { get; private set; }
            public PsdColorMode ColorMode { get; private set; }
            public NativeArray<byte> ColorModeData { get; private set; }

            public Rectangle Rectangle { get; private set; }
            public MaskDecodeContext LayerMaskContext { get; private set; }
            public MaskDecodeContext UserMaskContext { get; private set; }

            public DecodeContext(PhotoshopFile.Layer layer, Rectangle bounds)
            {
                Layer = layer;
                ByteDepth = Util.BytesFromBitDepth(layer.PsdFile.BitDepth);
                HasAlphaChannel = 0;
                Channels = layer.Channels.ToIdArray();

                var alphaSize = 4;
                if (layer.AlphaChannel != null && layer.AlphaChannel.ImageData.Length > 0)
                {
                    HasAlphaChannel = 1;
                    alphaSize = layer.AlphaChannel.ImageData.Length;
                    alphaSize = (alphaSize / 4) + (alphaSize % 4 > 0 ? 1 : 0);
                    alphaSize = alphaSize * 4;
                } 
                AlphaChannel = new NativeArray<byte>(alphaSize, Allocator.TempJob);
                if (HasAlphaChannel > 0)
                    NativeArray<byte>.Copy(layer.AlphaChannel.ImageData, AlphaChannel, layer.AlphaChannel.ImageData.Length);
                ColorMode = layer.PsdFile.ColorMode;
                ColorModeData = new NativeArray<byte>(layer.PsdFile.ColorModeData, Allocator.TempJob);

                // Clip the layer to the specified bounds
                Rectangle = Layer.Rect.IntersectWith(bounds);

                if (layer.Masks != null)
                {
                    LayerMaskContext = GetMaskContext(layer.Masks.LayerMask);
                    UserMaskContext = GetMaskContext(layer.Masks.UserMask);
                }
            }

            internal void Cleanup()
            {
                AlphaChannel.Dispose();
                ColorModeData.Dispose();
            }
            
            private MaskDecodeContext GetMaskContext(Mask mask)
            {
                if ((mask == null) || (mask.Disabled))
                {
                    return null;
                }

                return new MaskDecodeContext(mask, this);
            }
        }

        private class MaskDecodeContext
        {
            public Mask Mask { get; private set; }
            public Rectangle Rectangle { get; private set; }
            public MaskDecodeContext(Mask mask, DecodeContext layerContext)
            {
                Mask = mask;

                // The PositionVsLayer flag is documented to indicate a position
                // relative to the layer, but Photoshop treats the position as
                // absolute.  So that's what we do, too.
                Rectangle = mask.Rect.IntersectWith(layerContext.Rectangle);
            }

            public bool IsRowEmpty(int row)
            {
                return (Mask.ImageData == null)
                    || (Mask.ImageData.Length == 0)
                    || (Rectangle.Size.IsEmpty)
                    || (row < Rectangle.Top)
                    || (row >= Rectangle.Bottom);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////

        internal static byte RGBByteFromHDRFloat(float ptr)
        {
            var result = (byte)(255 * Math.Pow(ptr, rgbExponent));
            return result;
        }

        private static DecodeDelegate GetDecodeDelegate(PsdColorMode psdColorMode, ref DecodeType decoderType)
        {
            switch (psdColorMode)
            {
                case PsdColorMode.Bitmap:
                    decoderType = DecodeType.Bitmap;
                    return SetPDNRowBitmap;
                case PsdColorMode.Grayscale:
                case PsdColorMode.Duotone:
                    decoderType = DecodeType.Grayscale;
                    return SetPDNRowGrayscale;
                case PsdColorMode.Indexed:
                    decoderType = DecodeType.Indexed;
                    return SetPDNRowIndexed;
                case PsdColorMode.RGB:
                    decoderType = DecodeType.RGB;
                    return SetPDNRowRgb;
                case PsdColorMode.CMYK:
                    decoderType = DecodeType.CMYK;
                    return SetPDNRowCmyk;
                case PsdColorMode.Lab:
                    decoderType = DecodeType.Lab;
                    return SetPDNRowLab;
                case PsdColorMode.Multichannel:
                    throw new Exception("Cannot decode multichannel.");
                default:
                    throw new Exception("Unknown color mode.");
            }
        }

        private static DecodeDelegate GetDecodeDelegate32(PsdColorMode psdColorMode, ref DecodeType decoderType)
        {
            switch (psdColorMode)
            {
                case PsdColorMode.Grayscale:
                    decoderType = DecodeType.Grayscale32;
                    return SetPDNRowGrayscale32;
                case PsdColorMode.RGB:
                    decoderType = DecodeType.RGB32;
                    return SetPDNRowRgb32;
                default:
                    throw new PsdInvalidException(
                    "32-bit HDR images must be either RGB or grayscale.");
            }
        }

        /// <summary>
        /// Decode image from Photoshop's channel-separated formats to BGRA.
        /// </summary>
        public static JobHandle DecodeImage(BitmapLayer pdnLayer, PhotoshopFile.Layer psdLayer, JobHandle inputDeps)
        {
            UnityEngine.Profiling.Profiler.BeginSample("DecodeImage");
            var decodeContext = new DecodeContext(psdLayer, pdnLayer.Bounds);
            DecodeDelegate decoder = null;
            DecodeType decoderType = 0;

            if (decodeContext.ByteDepth == 4)
                decoder = GetDecodeDelegate32(decodeContext.ColorMode, ref decoderType);
            else
                decoder = GetDecodeDelegate(decodeContext.ColorMode, ref decoderType);

            JobHandle jobHandle = DecodeImage(pdnLayer, decodeContext, decoderType, inputDeps);
            UnityEngine.Profiling.Profiler.EndSample();
            return jobHandle;
        }

        /// <summary>
        /// Decode image from Photoshop's channel-separated formats to BGRA,
        /// using the specified decode delegate on each row.
        /// </summary>
        private static JobHandle DecodeImage(BitmapLayer pdnLayer, DecodeContext decodeContext, DecodeType decoderType, JobHandle inputDeps)
        {

            var psdLayer = decodeContext.Layer;
            var surface = pdnLayer.Surface;
            var rect = decodeContext.Rectangle;

            // Convert rows from the Photoshop representation, writing the
            // resulting ARGB values to to the Paint.NET Surface.

            int jobCount = Unity.Jobs.LowLevel.Unsafe.JobsUtility.JobWorkerMaximumCount;
            int execCount = (rect.Bottom - rect.Top);
            int sliceCount = execCount / jobCount;
            PDNDecoderJob decoderJob = new PDNDecoderJob();

            decoderJob.Data.Rect = rect;
            decoderJob.Data.LayerRect = psdLayer.Rect;
            decoderJob.Data.ClippedRect = rect;
            decoderJob.Data.SurfaceWidth = surface.width;
            decoderJob.Data.SurfaceHeight = surface.height;
            decoderJob.Data.SurfaceByteDepth = decodeContext.ByteDepth;
            decoderJob.Data.DecoderType = decoderType;

            decoderJob.Data.ColorChannel0 = decodeContext.Channels[0].ImageData;
            decoderJob.Data.ColorChannel1 = decodeContext.Channels.Length > 1 ? decodeContext.Channels[1].ImageData : decodeContext.Channels[0].ImageData;
            decoderJob.Data.ColorChannel2 = decodeContext.Channels.Length > 2 ? decodeContext.Channels[2].ImageData : decodeContext.Channels[0].ImageData;
            decoderJob.Data.ColorChannel3 = decodeContext.Channels.Length > 3 ? decodeContext.Channels[3].ImageData : decodeContext.Channels[0].ImageData;
            decoderJob.Data.ColorModeData = decodeContext.ColorModeData;
            decoderJob.Data.DecodedImage  = surface.color;

            // Schedule the job, returns the JobHandle which can be waited upon later on
            JobHandle jobHandle = decoderJob.Schedule(execCount, sliceCount, inputDeps);

            // Mask and Alpha.
            int userMaskContextSize = decodeContext.UserMaskContext != null ? decodeContext.Rectangle.Width : 1;
            int layerMaskContextSize = decodeContext.LayerMaskContext != null ? decodeContext.Rectangle.Width : 1;
            var userAlphaMask = new NativeArray<byte>(userMaskContextSize, Allocator.TempJob);
            var layerAlphaMask = new NativeArray<byte>(layerMaskContextSize, Allocator.TempJob);
            var userAlphaMaskEmpty = new NativeArray<byte>(rect.Bottom, Allocator.TempJob);
            var layerAlphaMaskEmpty = new NativeArray<byte>(rect.Bottom, Allocator.TempJob);

            PDNAlphaMaskJob alphaMaskJob = new PDNAlphaMaskJob();

            for (int y = rect.Top; y < rect.Bottom; ++y)
            {
                if (decodeContext.UserMaskContext != null)
                    userAlphaMaskEmpty[y] = decodeContext.UserMaskContext.IsRowEmpty(y) ? (byte)1 : (byte)0;
                if (decodeContext.LayerMaskContext != null)
                    layerAlphaMaskEmpty[y] = decodeContext.LayerMaskContext.IsRowEmpty(y) ? (byte)1 : (byte)0;
            }

            alphaMaskJob.Data.Rect = rect;
            alphaMaskJob.Data.LayerRect = psdLayer.Rect;
            alphaMaskJob.Data.ClippedRect = rect;
            alphaMaskJob.Data.SurfaceWidth = surface.width;
            alphaMaskJob.Data.SurfaceHeight = surface.height;
            alphaMaskJob.Data.SurfaceByteDepth = decodeContext.ByteDepth;
            alphaMaskJob.Data.HasAlphaChannel = decodeContext.HasAlphaChannel;

            alphaMaskJob.Data.HasUserAlphaMask = decodeContext.UserMaskContext != null ? 1 : 0;
            alphaMaskJob.Data.UserMaskInvertOnBlend = decodeContext.UserMaskContext != null ? (decodeContext.UserMaskContext.Mask.InvertOnBlend ? 1 : 0) : 0;
            alphaMaskJob.Data.UserMaskRect = decodeContext.UserMaskContext != null ? decodeContext.UserMaskContext.Mask.Rect : rect;
            alphaMaskJob.Data.UserMaskContextRect = decodeContext.UserMaskContext != null ? decodeContext.UserMaskContext.Rectangle : rect;
            alphaMaskJob.Data.HasLayerAlphaMask = decodeContext.LayerMaskContext != null ? 1 : 0;
            alphaMaskJob.Data.LayerMaskInvertOnBlend = decodeContext.LayerMaskContext != null ? (decodeContext.LayerMaskContext.Mask.InvertOnBlend ? 1 : 0) : 0;
            alphaMaskJob.Data.LayerMaskRect = decodeContext.LayerMaskContext != null ? decodeContext.LayerMaskContext.Mask.Rect : rect;
            alphaMaskJob.Data.LayerMaskContextRect = decodeContext.LayerMaskContext != null ? decodeContext.LayerMaskContext.Rectangle : rect;

            alphaMaskJob.Data.AlphaChannel0 = decodeContext.AlphaChannel;
            alphaMaskJob.Data.UserMask = decodeContext.UserMaskContext != null ? decodeContext.UserMaskContext.Mask.ImageData : decodeContext.AlphaChannel;
            alphaMaskJob.Data.UserAlphaMask = userAlphaMask;
            alphaMaskJob.Data.UserAlphaMaskEmpty = userAlphaMaskEmpty;
            alphaMaskJob.Data.LayerMask = decodeContext.LayerMaskContext != null ? decodeContext.LayerMaskContext.Mask.ImageData : decodeContext.AlphaChannel;
            alphaMaskJob.Data.LayerAlphaMask = layerAlphaMask;
            alphaMaskJob.Data.LayerAlphaMaskEmpty = layerAlphaMaskEmpty;
            alphaMaskJob.Data.DecodedImage = surface.color;
            alphaMaskJob.Data.UserMaskBackgroundColor = decodeContext.UserMaskContext != null ? decodeContext.UserMaskContext.Mask.BackgroundColor : (byte)0;
            alphaMaskJob.Data.LayerMaskBackgroundColor = decodeContext.LayerMaskContext != null ? decodeContext.LayerMaskContext.Mask.BackgroundColor : (byte)0;

            jobHandle = alphaMaskJob.Schedule(jobHandle);
            return jobHandle;

        }

        ///////////////////////////////////////////////////////////////////////////
        /// SINGLE THREADED - KEPT FOR REFERENCE
        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Decode image from Photoshop's channel-separated formats to BGRA.
        /// </summary>
        public static void DecodeImage(BitmapLayer pdnLayer, PhotoshopFile.Layer psdLayer)
        {
            UnityEngine.Profiling.Profiler.BeginSample("DecodeImage");
            var decodeContext = new DecodeContext(psdLayer, pdnLayer.Bounds);
            DecodeDelegate decoder = null;
            DecodeType decoderType = 0;

            if (decodeContext.ByteDepth == 4)
                decoder = GetDecodeDelegate32(decodeContext.ColorMode, ref decoderType);
            else
                decoder = GetDecodeDelegate(decodeContext.ColorMode, ref decoderType);

            DecodeImage(pdnLayer, decodeContext, decoder);
            decodeContext.Cleanup();
            UnityEngine.Profiling.Profiler.EndSample();
        }

        private delegate void DecodeDelegate(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, int idxSrc, DecodeContext context);

        /// <summary>
        /// Decode image from Photoshop's channel-separated formats to BGRA,
        /// using the specified decode delegate on each row.
        /// </summary>
        private static void DecodeImage(BitmapLayer pdnLayer, DecodeContext decodeContext, DecodeDelegate decoder)
        {

            var psdLayer = decodeContext.Layer;
            var surface = pdnLayer.Surface;
            var rect = decodeContext.Rectangle;

            // Convert rows from the Photoshop representation, writing the
            // resulting ARGB values to to the Paint.NET Surface.
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                // Calculate index into ImageData source from row and column.
                int idxSrcPixel = (y - psdLayer.Rect.Top) * psdLayer.Rect.Width
                    + (rect.Left - psdLayer.Rect.Left);
                int idxSrcByte = idxSrcPixel * decodeContext.ByteDepth;

                // Calculate pointers to destination Surface.
                //var pDestRow = surface.GetRowAddress(y);
                //var pDestStart = pDestRow + decodeContext.Rectangle.Left;
                //var pDestEnd = pDestRow + decodeContext.Rectangle.Right;
                var pDestStart = y * surface.width + decodeContext.Rectangle.Left;
                var pDestEnd = y * surface.width + decodeContext.Rectangle.Right;

                // For 16-bit images, take the higher-order byte from the image
                // data, which is now in little-endian order.
                if (decodeContext.ByteDepth == 2)
                    idxSrcByte++;

                // Decode the color and alpha channels
                decoder(pDestStart, pDestEnd, surface.width, surface.color, idxSrcByte, decodeContext);
            }

            // Mask and Alpha.
            int userMaskContextSize = decodeContext.UserMaskContext != null ? decodeContext.Rectangle.Width : 1;
            int layerMaskContextSize = decodeContext.LayerMaskContext != null ? decodeContext.Rectangle.Width : 1;
            var userAlphaMask = new NativeArray<byte>(userMaskContextSize, Allocator.TempJob);
            var layerAlphaMask = new NativeArray<byte>(layerMaskContextSize, Allocator.TempJob);

            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                // Calculate index into ImageData source from row and column.
                int idxSrcPixel = (y - psdLayer.Rect.Top) * psdLayer.Rect.Width + (rect.Left - psdLayer.Rect.Left);
                int idxSrcByte = idxSrcPixel * decodeContext.ByteDepth;

                // Calculate pointers to destination Surface.
                //var pDestRow = surface.GetRowAddress(y);
                //var pDestStart = pDestRow + decodeContext.Rectangle.Left;
                //var pDestEnd = pDestRow + decodeContext.Rectangle.Right;
                var pDestStart = y * surface.width + decodeContext.Rectangle.Left;
                var pDestEnd = y * surface.width + decodeContext.Rectangle.Right;

                // For 16-bit images, take the higher-order byte from the image
                // data, which is now in little-endian order.
                if (decodeContext.ByteDepth == 2)
                    idxSrcByte++;

                // Decode the color and alpha channels
                SetPDNAlphaRow(pDestStart, pDestEnd, surface.width, surface.color, idxSrcByte, decodeContext.ByteDepth, decodeContext.HasAlphaChannel, decodeContext.AlphaChannel);
                // Apply layer masks(s) to the alpha channel
                GetMaskAlphaRow(y, decodeContext, decodeContext.LayerMaskContext, ref layerAlphaMask);
                GetMaskAlphaRow(y, decodeContext, decodeContext.UserMaskContext, ref userAlphaMask);
                ApplyPDNMask(pDestStart, pDestEnd, surface.width, surface.color, layerAlphaMask, userAlphaMask);
            }
            userAlphaMask.Dispose();
            layerAlphaMask.Dispose();
        }

        private static unsafe void GetMaskAlphaRow(int y, DecodeContext layerContext, MaskDecodeContext maskContext, ref NativeArray<byte> alphaBuffer)
        {
            if (maskContext == null)
                return;
            var mask = maskContext.Mask;

            // Background color for areas not covered by the mask
            byte backgroundColor = mask.InvertOnBlend
                ? (byte)(255 - mask.BackgroundColor)
                : mask.BackgroundColor;
            {
                var alphaBufferPtr = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(alphaBuffer);
                UnsafeUtility.MemSet(alphaBufferPtr, backgroundColor, alphaBuffer.Length);
            }
            if (maskContext.IsRowEmpty(y))
            {
                return;
            }

            //////////////////////////////////////
            // Transfer mask into the alpha array
            var pMaskData = mask.ImageData;
            {
                // Get pointers to starting positions
                int alphaColumn = maskContext.Rectangle.X - layerContext.Rectangle.X;
                var pAlpha = alphaColumn;
                var pAlphaEnd = pAlpha + maskContext.Rectangle.Width;

                int maskRow = y - mask.Rect.Y;
                int maskColumn = maskContext.Rectangle.X - mask.Rect.X;
                int idxMaskPixel = (maskRow * mask.Rect.Width) + maskColumn;
                var pMask = idxMaskPixel * layerContext.ByteDepth;

                // Take the high-order byte if values are 16-bit (little-endian)
                if (layerContext.ByteDepth == 2)
                    pMask++;

                // Decode mask into the alpha array.
                if (layerContext.ByteDepth == 4)
                {
                    DecodeMaskAlphaRow32(alphaBuffer, pAlpha, pAlphaEnd, pMaskData, pMask);
                }
                else
                {
                    DecodeMaskAlphaRow(alphaBuffer, pAlpha, pAlphaEnd, pMaskData, pMask, layerContext.ByteDepth);
                }

                // Obsolete since Photoshop CS6, but retained for compatibility with
                // older versions.  Note that the background has already been inverted.
                if (mask.InvertOnBlend)
                {
                    Util.Invert(alphaBuffer, pAlpha, pAlphaEnd);
                }
            }
        }

        private static void SetPDNAlphaRow(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, int idxSrc, int byteDepth, int hasAlphaChannel, NativeArray<byte> alphaChannel)
        {
            // Set alpha to fully-opaque if there is no alpha channel
            if (0 == hasAlphaChannel)
            {
                var pDest = pDestStart;
                while (pDest < pDestEnd)
                {
                    var c = color[pDest];
                    c.a = 255;
                    color[pDest] = c;
                    pDest++;
                }
            }
            // Set the alpha channel data
            else
            {
                NativeArray<float> srcAlphaChannel = alphaChannel.Reinterpret<float>(1);
                {
                    var pDest = pDestStart;
                    while (pDest < pDestEnd)
                    {
                        var c = color[pDest];
                        c.a = (byteDepth < 4) ? alphaChannel[idxSrc] : RGBByteFromHDRFloat(srcAlphaChannel[idxSrc / 4]);

                        color[pDest] = c;
                        pDest++;
                        idxSrc += byteDepth;
                    }
                }
            }
        }

        private static void DecodeMaskAlphaRow32(NativeArray<byte> pAlpha, int pAlphaStart, int pAlphaEnd, NativeArray<byte> pMask, int pMaskStart)
        {

            NativeArray<float> floatArray = pMask.Reinterpret<float>(1);

            while (pAlphaStart < pAlphaEnd)
            {
                pAlpha[pAlphaStart] = RGBByteFromHDRFloat(floatArray[pMaskStart * 4]);

                pAlphaStart++;
                pMaskStart += 4;
            }
        }

        private static void DecodeMaskAlphaRow(NativeArray<byte> pAlpha, int pAlphaStart, int pAlphaEnd, NativeArray<byte> pMask, int pMaskStart, int byteDepth)
        {
            while (pAlphaStart < pAlphaEnd)
            {
                pAlpha[pAlphaStart] = pMask[pMaskStart];

                pAlphaStart++;
                pMaskStart += byteDepth;
            }
        }
        
        private static void ApplyPDNMask(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, NativeArray<byte> layerMaskAlpha, NativeArray<byte> userMaskAlpha)
        {
            // Do nothing if there are no masks
            if ((layerMaskAlpha.Length <= 1) && (userMaskAlpha.Length <= 1))
            {
                return;
            }
            // Apply one mask
            else if ((layerMaskAlpha.Length <= 1) || (userMaskAlpha.Length <= 1))
            {
                var maskAlpha = (layerMaskAlpha.Length <= 1) ? userMaskAlpha : layerMaskAlpha;
                var maskStart = 0;
                {
                    while (pDestStart < pDestEnd)
                    {
                        var c = color[pDestStart];
                        c.a = (byte)(color[pDestStart].a * maskAlpha[maskStart] / 255);
                        color[pDestStart] = c;
                        pDestStart++;
                        maskStart++;
                    }
                }
            }
            // Apply both masks in one pass, to minimize rounding error
            else
            {
                //fixed (byte* pLayerMaskAlpha = &layerMaskAlpha[0],
                //  pUserMaskAlpha = &userMaskAlpha[0])
                {
                    var maskStart = 0;
                    while (pDestStart < pDestEnd)
                    {
                        var alphaFactor = (layerMaskAlpha[maskStart]) * (userMaskAlpha[maskStart]);
                        var c = color[pDestStart];
                        c.a = (byte)(color[pDestStart].a * alphaFactor / 65025);
                        color[pDestStart] = c;

                        pDestStart++;
                        maskStart++;
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////

        #region Decode 32-bit HDR channels

        private static void SetPDNRowRgb32(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, int idxSrc, DecodeContext context)
        {
            NativeArray<float> redChannel = context.Channels[0].ImageData.Reinterpret<float>(1);
            NativeArray<float> greenChannel = context.Channels[1].ImageData.Reinterpret<float>(1);
            NativeArray<float> blueChannel = context.Channels[2].ImageData.Reinterpret<float>(1);
            
            {
                while (pDestStart < pDestEnd)
                {
                    var c = color[pDestStart];
                    c.r = RGBByteFromHDRFloat(redChannel[idxSrc / 4]);
                    c.g = RGBByteFromHDRFloat(greenChannel[idxSrc / 4]);
                    c.b = RGBByteFromHDRFloat(blueChannel[idxSrc / 4]);
                    color[pDestStart] = c;
                    pDestStart++;
                    idxSrc += 4;
                }
            }
        }

        private static void SetPDNRowGrayscale32(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, int idxSrc, DecodeContext context)
        {
            NativeArray<float> channel = context.Channels[0].ImageData.Reinterpret<float>(1);
            {
                while (pDestStart < pDestEnd)
                {
                    byte rgbValue = RGBByteFromHDRFloat(channel[idxSrc / 4]);
                    var c = color[pDestStart];
                    c.r = rgbValue;
                    c.g = rgbValue;
                    c.b = rgbValue;
                    color[pDestStart] = c;

                    pDestStart++;
                    idxSrc += 4;
                }
            }
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region Decode 8-bit and 16-bit channels

        private static void SetPDNRowRgb(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, int idxSrc, DecodeContext context)
        {
            while (pDestStart < pDestEnd)
            {
                var c = color[pDestStart];
                c.r = context.Channels[0].ImageData[idxSrc];
                c.g = context.Channels[1].ImageData[idxSrc];
                c.b = context.Channels[2].ImageData[idxSrc];
                color[pDestStart] = c;

                pDestStart++;
                idxSrc += context.ByteDepth;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        //
        // The color-conversion formulas come from the Colour Space Conversions FAQ:
        //     http://www.poynton.com/PDFs/coloureq.pdf
        //
        // RGB --> CMYK                              CMYK --> RGB
        // ---------------------------------------   --------------------------------------------
        // Black   = minimum(1-Red,1-Green,1-Blue)   Red   = 1-minimum(1,Cyan*(1-Black)+Black)
        // Cyan    = (1-Red-Black)/(1-Black)         Green = 1-minimum(1,Magenta*(1-Black)+Black)
        // Magenta = (1-Green-Black)/(1-Black)       Blue  = 1-minimum(1,Yellow*(1-Black)+Black)
        // Yellow  = (1-Blue-Black)/(1-Black)
        //
        ///////////////////////////////////////////////////////////////////////////////

        private static void SetPDNRowCmyk(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, int idxSrc, DecodeContext context)
        {
            while (pDestStart < pDestEnd)
            {
                // CMYK values are stored as complements, presumably to allow for some
                // measure of compatibility with RGB-only applications.
                var C = 255 - context.Channels[0].ImageData[idxSrc];
                var M = 255 - context.Channels[1].ImageData[idxSrc];
                var Y = 255 - context.Channels[2].ImageData[idxSrc];
                var K = 255 - context.Channels[3].ImageData[idxSrc];

                int nRed = 255 - Math.Min(255, C * (255 - K) / 255 + K);
                int nGreen = 255 - Math.Min(255, M * (255 - K) / 255 + K);
                int nBlue = 255 - Math.Min(255, Y * (255 - K) / 255 + K);

                var c = color[pDestStart];
                c.r = (byte)nRed;
                c.g = (byte)nGreen;
                c.b = (byte)nBlue;
                color[pDestStart] = c;

                pDestStart++;
                idxSrc += context.ByteDepth;
            }
        }

        private static void SetPDNRowBitmap(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, int idxSrc, DecodeContext context)
        {
            var bitmap = context.Channels[0].ImageData;
            while (pDestStart < pDestEnd)
            {
                byte mask = (byte)(0x80 >> (idxSrc % 8));
                byte bwValue = (byte)(bitmap[idxSrc / 8] & mask);
                bwValue = (bwValue == 0) ? (byte)255 : (byte)0;

                var c = color[pDestStart];
                c.r = bwValue;
                c.g = bwValue;
                c.b = bwValue;
                color[pDestStart] = c;

                pDestStart++;
                idxSrc += context.ByteDepth;
            }
        }

        private static void SetPDNRowGrayscale(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, int idxSrc, DecodeContext context)
        {
            while (pDestStart < pDestEnd)
            {
                var level = context.Channels[0].ImageData[idxSrc];
                var c = color[pDestStart];
                c.r = level;
                c.g  = level;
                c.b  = level;
                color[pDestStart] = c;

                pDestStart++;
                idxSrc += context.ByteDepth;
            }
        }

        private static void SetPDNRowIndexed(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, int idxSrc, DecodeContext context)
        {
            while (pDestStart < pDestEnd)
            {
                int index = (int)context.Channels[0].ImageData[idxSrc];
                var c = color[pDestStart];
                c.r = (byte)context.ColorModeData[index];
                c.g = context.ColorModeData[index + 256];
                c.b = context.ColorModeData[index + 2 * 256];
                color[pDestStart] = c;

                pDestStart++;
                idxSrc += context.ByteDepth;
            }
        }

        private static void SetPDNRowLab(int pDestStart, int pDestEnd, int width, NativeArray<Color32> color, int idxSrc, DecodeContext context)
        {
            while (pDestStart < pDestEnd)
            {
                double exL, exA, exB;
                exL = (double)context.Channels[0].ImageData[idxSrc];
                exA = (double)context.Channels[1].ImageData[idxSrc];
                exB = (double)context.Channels[2].ImageData[idxSrc];

                int L = (int)(exL / 2.55);
                int a = (int)(exA - 127.5);
                int b = (int)(exB - 127.5);

                // First, convert from Lab to XYZ.
                // Standards used Observer = 2, Illuminant = D65

                const double ref_X = 95.047;
                const double ref_Y = 100.000;
                const double ref_Z = 108.883;

                double var_Y = ((double)L + 16.0) / 116.0;
                double var_X = (double)a / 500.0 + var_Y;
                double var_Z = var_Y - (double)b / 200.0;

                double var_X3 = var_X * var_X * var_X;
                double var_Y3 = var_Y * var_Y * var_Y;
                double var_Z3 = var_Z * var_Z * var_Z;

                if (var_Y3 > 0.008856)
                    var_Y = var_Y3;
                else
                    var_Y = (var_Y - 16 / 116) / 7.787;

                if (var_X3 > 0.008856)
                    var_X = var_X3;
                else
                    var_X = (var_X - 16 / 116) / 7.787;

                if (var_Z3 > 0.008856)
                    var_Z = var_Z3;
                else
                    var_Z = (var_Z - 16 / 116) / 7.787;

                double X = ref_X * var_X;
                double Y = ref_Y * var_Y;
                double Z = ref_Z * var_Z;

                // Then, convert from XYZ to RGB.
                // Standards used Observer = 2, Illuminant = D65
                // ref_X = 95.047, ref_Y = 100.000, ref_Z = 108.883

                double var_R = X * 0.032406 + Y * (-0.015372) + Z * (-0.004986);
                double var_G = X * (-0.009689) + Y * 0.018758 + Z * 0.000415;
                double var_B = X * 0.000557 + Y * (-0.002040) + Z * 0.010570;

                if (var_R > 0.0031308)
                    var_R = 1.055 * (Math.Pow(var_R, 1 / 2.4)) - 0.055;
                else
                    var_R = 12.92 * var_R;

                if (var_G > 0.0031308)
                    var_G = 1.055 * (Math.Pow(var_G, 1 / 2.4)) - 0.055;
                else
                    var_G = 12.92 * var_G;

                if (var_B > 0.0031308)
                    var_B = 1.055 * (Math.Pow(var_B, 1 / 2.4)) - 0.055;
                else
                    var_B = 12.92 * var_B;

                int nRed = (int)(var_R * 256.0);
                int nGreen = (int)(var_G * 256.0);
                int nBlue = (int)(var_B * 256.0);

                if (nRed < 0)
                    nRed = 0;
                else if (nRed > 255)
                    nRed = 255;
                if (nGreen < 0)
                    nGreen = 0;
                else if (nGreen > 255)
                    nGreen = 255;
                if (nBlue < 0)
                    nBlue = 0;
                else if (nBlue > 255)
                    nBlue = 255;

                var c = color[pDestStart];
                c.r = (byte)nRed;
                c.g = (byte)nGreen;
                c.b = (byte)nBlue;
                color[pDestStart] = c;

                pDestStart++;
                idxSrc += context.ByteDepth;
            }
        }

        #endregion
    }
}
