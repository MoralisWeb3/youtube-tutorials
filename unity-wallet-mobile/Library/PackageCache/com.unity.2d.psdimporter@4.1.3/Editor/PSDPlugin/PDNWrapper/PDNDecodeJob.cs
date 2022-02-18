using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using System;
using Unity.Collections.LowLevel.Unsafe;

namespace PaintDotNet.Data.PhotoshopFileType
{

    #region PDNDecodeJob

    internal struct PDNDecoderData
    {
        // Inputs.
        public PDNWrapper.Rectangle Rect;
        public PDNWrapper.Rectangle LayerRect;
        public PDNWrapper.Rectangle ClippedRect;
        public int SurfaceWidth;
        public int SurfaceHeight;
        public int SurfaceByteDepth;
        public DecodeType DecoderType;

        [NativeDisableParallelForRestriction]
        [ReadOnly]
        public NativeArray<byte> ColorChannel0;

        [NativeDisableParallelForRestriction]
        [ReadOnly]
        public NativeArray<byte> ColorChannel1;

        [NativeDisableParallelForRestriction]
        [ReadOnly]
        public NativeArray<byte> ColorChannel2;

        [NativeDisableParallelForRestriction]
        [ReadOnly]
        public NativeArray<byte> ColorChannel3;

        [NativeDisableParallelForRestriction]
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<byte> ColorModeData;

        // Outputs
        [NativeDisableParallelForRestriction]
        public NativeArray<Color32> DecodedImage;

    }

    internal struct PDNDecoderJob : IJobParallelFor
    {

        public PDNDecoderData Data;

        public void Execute(int index)
        {

            int idx = Data.Rect.Top + index;
            {
                // Calculate index into ImageData source from row and column.
                int idxSrcPixel = (idx - Data.LayerRect.Top) * Data.LayerRect.Width + (Data.Rect.Left - Data.LayerRect.Left);
                int idxSrcBytes = idxSrcPixel * Data.SurfaceByteDepth;

                // Calculate pointers to destination Surface.
                var idxDstStart = idx * Data.SurfaceWidth + Data.ClippedRect.Left;
                var idxDstStops = idx * Data.SurfaceWidth + Data.ClippedRect.Right;

                // For 16-bit images, take the higher-order byte from the image data, which is now in little-endian order.
                if (Data.SurfaceByteDepth == 2)
                {
                    idxSrcBytes++;
                }

                switch (Data.DecoderType)
                {
                    case DecodeType.RGB32:
                        {
                            SetPDNRowRgb32(idxDstStart, idxDstStops, idxSrcBytes);
                        }
                        break;
                    case DecodeType.Grayscale32:
                        {
                            SetPDNRowGrayscale32(idxDstStart, idxDstStops, idxSrcBytes);
                        }
                        break;
                    case DecodeType.RGB:
                        {
                            SetPDNRowRgb(idxDstStart, idxDstStops, idxSrcBytes);
                        }
                        break;
                    case DecodeType.CMYK:
                        {
                            SetPDNRowCmyk(idxDstStart, idxDstStops, idxSrcBytes);
                        }
                        break;
                    case DecodeType.Bitmap:
                        {
                            SetPDNRowBitmap(idxDstStart, idxDstStops, idxSrcBytes);
                        }
                        break;
                    case DecodeType.Grayscale:
                        {
                            SetPDNRowGrayscale(idxDstStart, idxDstStops, idxSrcBytes);
                        }
                        break;
                    case DecodeType.Indexed:
                        {
                            SetPDNRowIndexed(idxDstStart, idxDstStops, idxSrcBytes);
                        }
                        break;
                    case DecodeType.Lab:
                        {
                            SetPDNRowLab(idxDstStart, idxDstStops, idxSrcBytes);
                        }
                        break;
                }

            }

        }

        // Case 0:
        private void SetPDNRowRgb32(int dstStart, int dstStops, int idxSrc)
        {
            NativeArray<float> cR = Data.ColorChannel0.Reinterpret<float>(1);
            NativeArray<float> cG = Data.ColorChannel1.Reinterpret<float>(1);
            NativeArray<float> cB = Data.ColorChannel2.Reinterpret<float>(1);
            var c = Data.DecodedImage[dstStart];
            while (dstStart < dstStops)
            {
                c.r = ImageDecoderPdn.RGBByteFromHDRFloat(cR[idxSrc / 4]);
                c.g = ImageDecoderPdn.RGBByteFromHDRFloat(cG[idxSrc / 4]);
                c.b = ImageDecoderPdn.RGBByteFromHDRFloat(cB[idxSrc / 4]);
                Data.DecodedImage[dstStart] = c;

                dstStart++;
                idxSrc += 4;
            }
        }

        // Case 1:
        private void SetPDNRowGrayscale32(int dstStart, int dstStops, int idxSrc)
        {
            NativeArray<float> channel = Data.ColorChannel0.Reinterpret<float>(1);
            var c = Data.DecodedImage[dstStart];
            while (dstStart < dstStops)
            {
                byte rgbValue = ImageDecoderPdn.RGBByteFromHDRFloat(channel[idxSrc / 4]);
                c.r = rgbValue;
                c.g = rgbValue;
                c.b = rgbValue;
                Data.DecodedImage[dstStart] = c;

                dstStart++;
                idxSrc += 4;
            }
        }

        // Case 2:
        private void SetPDNRowRgb(int dstStart, int dstStops, int idxSrc)
        {
            var c = Data.DecodedImage[dstStart];
            while (dstStart < dstStops)
            {
                c.r = Data.ColorChannel0[idxSrc];
                c.g = Data.ColorChannel1[idxSrc];
                c.b = Data.ColorChannel2[idxSrc];
                Data.DecodedImage[dstStart] = c;

                dstStart++;
                idxSrc += Data.SurfaceByteDepth;
            }
        }

        // Case 3:
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
        private void SetPDNRowCmyk(int dstStart, int dstStops, int idxSrc)
        {
            var c = Data.DecodedImage[dstStart];
            while (dstStart < dstStops)
            {
                // CMYK values are stored as complements, presumably to allow for some
                // measure of compatibility with RGB-only applications.
                var C = 255 - Data.ColorChannel0[idxSrc];
                var M = 255 - Data.ColorChannel1[idxSrc];
                var Y = 255 - Data.ColorChannel2[idxSrc];
                var K = 255 - Data.ColorChannel3[idxSrc];

                int R = 255 - Math.Min(255, C * (255 - K) / 255 + K);
                int G = 255 - Math.Min(255, M * (255 - K) / 255 + K);
                int B = 255 - Math.Min(255, Y * (255 - K) / 255 + K);

                c.r = (byte)R;
                c.g = (byte)G;
                c.b = (byte)B;
                Data.DecodedImage[dstStart] = c;

                dstStart++;
                idxSrc += Data.SurfaceByteDepth;
            }
        }

        // Case 4:
        private void SetPDNRowBitmap(int dstStart, int dstStops, int idxSrc)
        {
            var c = Data.DecodedImage[dstStart];
            while (dstStart < dstStops)
            {
                byte mask = (byte)(0x80 >> (idxSrc % 8));
                byte bwValue = (byte)(Data.ColorChannel0[idxSrc / 8] & mask);
                bwValue = (bwValue == 0) ? (byte)255 : (byte)0;

                c.r = bwValue;
                c.g = bwValue;
                c.b = bwValue;
                Data.DecodedImage[dstStart] = c;

                dstStart++;
                idxSrc += Data.SurfaceByteDepth;
            }
        }

        // Case 5:
        private void SetPDNRowGrayscale(int dstStart, int dstStops, int idxSrc)
        {
            var c = Data.DecodedImage[dstStart];
            while (dstStart < dstStops)
            {
        
                c.r = Data.ColorChannel0[idxSrc];
                c.g = Data.ColorChannel0[idxSrc];
                c.b = Data.ColorChannel0[idxSrc];
                Data.DecodedImage[dstStart] = c;

                dstStart++;
                idxSrc += Data.SurfaceByteDepth;
            }
        }

        // Case 6:
        private void SetPDNRowIndexed(int dstStart, int dstStops, int idxSrc)
        {
            var c = Data.DecodedImage[dstStart];
            int index = (int)Data.ColorChannel0[idxSrc];
            while (dstStart < dstStops)
            {
                c.r = Data.ColorModeData[index];
                c.g = Data.ColorModeData[index + 256];
                c.b = Data.ColorModeData[index + 2 * 256];
                Data.DecodedImage[dstStart] = c;

                dstStart++;
                idxSrc += Data.SurfaceByteDepth;
            }
        }

        // Case 7:
        private void SetPDNRowLab(int dstStart, int dstStops, int idxSrc)
        {
            var c = Data.DecodedImage[dstStart];
            while (dstStart < dstStops)
            {
                double exL, exA, exB;
                exL = (double)Data.ColorChannel0[idxSrc];
                exA = (double)Data.ColorChannel1[idxSrc];
                exB = (double)Data.ColorChannel2[idxSrc];

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

                c.r = (byte)nRed;
                c.g = (byte)nGreen;
                c.b = (byte)nBlue;
                Data.DecodedImage[dstStart] = c;

                dstStart++;
                idxSrc += Data.SurfaceByteDepth;
            }
        }
    }

    #endregion

    #region AlphaDecodeJob

    internal struct PDNAlphaMaskData
    {
        // Inputs.
        public PDNWrapper.Rectangle Rect;
        public PDNWrapper.Rectangle LayerRect;
        public PDNWrapper.Rectangle ClippedRect;
        public int SurfaceWidth;
        public int SurfaceHeight;
        public int SurfaceByteDepth;

        public int HasAlphaChannel;
        public int HasUserAlphaMask;
        public int UserMaskInvertOnBlend;
        public PDNWrapper.Rectangle UserMaskRect;
        public PDNWrapper.Rectangle UserMaskContextRect;
        public int HasLayerAlphaMask;
        public int LayerMaskInvertOnBlend;
        public PDNWrapper.Rectangle LayerMaskRect;
        public PDNWrapper.Rectangle LayerMaskContextRect;

        [NativeDisableParallelForRestriction]
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<byte> AlphaChannel0;

        [NativeDisableParallelForRestriction]
        [ReadOnly]
        public NativeArray<byte> UserMask;

        [DeallocateOnJobCompletion]
        [NativeDisableParallelForRestriction]
        public NativeArray<byte> UserAlphaMask;

        [DeallocateOnJobCompletion]
        [NativeDisableParallelForRestriction]
        public NativeArray<byte> UserAlphaMaskEmpty;

        [NativeDisableParallelForRestriction]
        [ReadOnly]
        public NativeArray<byte> LayerMask;

        [DeallocateOnJobCompletion]
        [NativeDisableParallelForRestriction]
        public NativeArray<byte> LayerAlphaMask;

        [DeallocateOnJobCompletion]
        [NativeDisableParallelForRestriction]
        public NativeArray<byte> LayerAlphaMaskEmpty;

        // Outputs
        [NativeDisableParallelForRestriction]
        public NativeArray<Color32> DecodedImage;

        // Colors.
        public byte UserMaskBackgroundColor;
        public byte LayerMaskBackgroundColor;
    }

    internal struct PDNAlphaMaskJob : IJob
    {

        public PDNAlphaMaskData Data;

        public void Execute()
        {

            for (int idx = Data.Rect.Top; idx < Data.Rect.Bottom; idx++)
            {
                // Calculate index into ImageData source from row and column.
                int idxSrcPixel = (idx - Data.LayerRect.Top) * Data.LayerRect.Width + (Data.Rect.Left - Data.LayerRect.Left);
                int idxSrcBytes = idxSrcPixel * Data.SurfaceByteDepth;

                // Calculate pointers to destination Surface.
                var idxDstStart = idx * Data.SurfaceWidth + Data.ClippedRect.Left;
                var idxDstStops = idx * Data.SurfaceWidth + Data.ClippedRect.Right;

                // For 16-bit images, take the higher-order byte from the image data, which is now in little-endian order.
                if (Data.SurfaceByteDepth == 2)
                {
                    idxSrcBytes++;
                }

                SetPDNAlphaRow(idxDstStart, idxDstStops, idxSrcBytes);
                if (0 != Data.HasLayerAlphaMask)
                {
                    GetMaskAlphaRow(idx, Data.LayerAlphaMask, Data.LayerAlphaMaskEmpty, Data.LayerMask, Data.LayerMaskInvertOnBlend, Data.LayerMaskBackgroundColor, Data.LayerMaskContextRect, Data.LayerMaskRect);
                }
                if (0 != Data.HasUserAlphaMask)
                {
                    GetMaskAlphaRow(idx, Data.UserAlphaMask, Data.UserAlphaMaskEmpty, Data.UserMask, Data.UserMaskInvertOnBlend, Data.UserMaskBackgroundColor, Data.UserMaskContextRect, Data.UserMaskRect);
                }
                ApplyPDNMask(idxDstStart, idxDstStops);

            }
        }

        private void SetPDNAlphaRow(int dstStart, int dstStops, int idxSrc)
        {
            // Set alpha to fully-opaque if there is no alpha channel
            if (0 == Data.HasAlphaChannel)
            {
                while (dstStart < dstStops)
                {
                    var c = Data.DecodedImage[dstStart];
                    c.a = 255;
                    Data.DecodedImage[dstStart] = c;
                    dstStart++;
                }
            }
            // Set the alpha channel data
            else
            {
                NativeArray<float> srcAlphaChannel = Data.AlphaChannel0.Reinterpret<float>(1);
                {
                    while (dstStart < dstStops)
                    {
                        var c = Data.DecodedImage[dstStart];
                        c.a = (Data.SurfaceByteDepth < 4) ? Data.AlphaChannel0[idxSrc] : ImageDecoderPdn.RGBByteFromHDRFloat(srcAlphaChannel[idxSrc / 4]);

                        Data.DecodedImage[dstStart] = c;
                        dstStart++;
                        idxSrc += Data.SurfaceByteDepth;
                    }
                }
            }
        }

        private void ApplyPDNMask(int dstStart, int dstStops)
        {
            // Do nothing if there are no masks
            if (0 == Data.HasLayerAlphaMask && 0 == Data.HasUserAlphaMask)
            {
                return;
            }

            // Apply one mask
            else if (0 == Data.HasLayerAlphaMask || 0 == Data.HasUserAlphaMask)
            {
                var maskAlpha = (0 == Data.HasLayerAlphaMask) ? Data.UserAlphaMask : Data.LayerAlphaMask;
                var maskStart = 0;
                {
                    while (dstStart < dstStops)
                    {
                        var c = Data.DecodedImage[dstStart];
                        c.a = (byte)(Data.DecodedImage[dstStart].a * maskAlpha[maskStart] / 255);
                        Data.DecodedImage[dstStart] = c;
                        
                        dstStart++;
                        maskStart++;
                    }
                }                
            }
            // Apply both masks in one pass, to minimize rounding error
            else
            {
                var maskStart = 0;
                {
                    while (dstStart < dstStops)
                    {
                        var c = Data.DecodedImage[dstStart];
                        var alphaFactor = (Data.LayerAlphaMask[maskStart]) * (Data.UserAlphaMask[maskStart]);
                        c.a = (byte)(Data.DecodedImage[dstStart].a * alphaFactor / 65025);
                        Data.DecodedImage[dstStart] = c;

                        dstStart++;
                        maskStart++;
                    }
                }
            }
        }

        private void DecodeMaskAlphaRow32(NativeArray<byte> Alpha, int dstStart, int dstStops, NativeArray<byte> Mask, int maskStart)
        {
            NativeArray<float> floatArray = Mask.Reinterpret<float>(1);

            while (dstStart < dstStops)
            {
                Alpha[dstStart] = ImageDecoderPdn.RGBByteFromHDRFloat(floatArray[maskStart / 4]);

                dstStart++;
                maskStart += 4;
            }
        }

        private void DecodeMaskAlphaRow(NativeArray<byte> Alpha, int dstStart, int dstStops, NativeArray<byte> Mask, int maskStart, int byteDepth)
        {
            while (dstStart < dstStops)
            {
                Alpha[dstStart] = Mask[maskStart];

                dstStart++;
                maskStart += byteDepth;
            }
        }

        private unsafe void GetMaskAlphaRow(int idxSrc, NativeArray<byte> alphaBuffer, NativeArray<byte> alphaBufferEmpty, NativeArray<byte> maskChannel, int MaskInvertOnBlend, byte MaskBackgroundColor, PDNWrapper.Rectangle MaskContextRect, PDNWrapper.Rectangle MaskRect)
        {
            //////////////////////////////////////
            // Transfer mask into the alpha array
            // Background color for areas not covered by the mask
            byte backgroundColor = (0 != MaskInvertOnBlend) ? (byte)(255 - MaskBackgroundColor) : MaskBackgroundColor;
            {
                var alphaBufferPtr = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(alphaBuffer);
                UnsafeUtility.MemSet(alphaBufferPtr, backgroundColor, alphaBuffer.Length);
            }
            // Only process if not Empty.
            if (alphaBufferEmpty[idxSrc] == 0)
            {
                // Get pointers to starting positions
                int alphaColumn = MaskContextRect.X;
                // It's possible that the layer's rect is larger than the clip and it's offset.
                // Since we only copy out the alpha based on the MaskContext size
                // The copy will start from where the MaskContextRect is
                if(Data.LayerRect.X > 0)
                    alphaColumn = MaskContextRect.X - Data.LayerRect.X;
                var pAlpha = alphaColumn;
                var pAlphaEnd = pAlpha + MaskContextRect.Width;

                int maskRow = idxSrc - MaskRect.Y;
                int maskColumn = MaskContextRect.X - MaskRect.X;
                int idxMaskPixel = (maskRow * MaskRect.Width) + maskColumn;
                var pMask = idxMaskPixel * Data.SurfaceByteDepth;

                // Take the high-order byte if values are 16-bit (little-endian)
                if (Data.SurfaceByteDepth == 2)
                {
                    pMask++;
                }

                // Decode mask into the alpha array.
                if (Data.SurfaceByteDepth == 4)
                {
                    DecodeMaskAlphaRow32(alphaBuffer, pAlpha, pAlphaEnd, maskChannel, pMask);
                }
                else
                {
                    DecodeMaskAlphaRow(alphaBuffer, pAlpha, pAlphaEnd, maskChannel, pMask, Data.SurfaceByteDepth);
                }

                // Obsolete since Photoshop CS6, but retained for compatibility with older versions.  Note that the background has already been inverted.
                if (0 != MaskInvertOnBlend)
                {
                    PhotoshopFile.Util.Invert(alphaBuffer, pAlpha, pAlphaEnd);
                }
            }
        }
    }

    #endregion
}