/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2016 Tao Yue
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PDNWrapper;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Collections;

namespace PhotoshopFile
{
    internal static class Util
    {
        [DebuggerDisplay("Top = {Top}, Bottom = {Bottom}, Left = {Left}, Right = {Right}")]
        internal struct RectanglePosition
        {
            public int Top { get; set; }
            public int Bottom { get; set; }
            public int Left { get; set; }
            public int Right { get; set; }
        }

        public static Rectangle IntersectWith(
            this Rectangle thisRect, Rectangle rect)
        {
            thisRect.Intersect(rect);
            return thisRect;
        }

        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Fills a buffer with a byte value.
        /// </summary>
        static public void Fill(byte[] ptr, int start, int end, byte value)
        {
            while (start < end)
            {
                ptr[start] = value;
                start++;
            }
        }

        static public void Invert(byte[] ptr, int ptrStart, int ptrEnd)
        {
            while (ptrStart < ptrEnd)
            {
                ptr[ptrStart] = (byte)(ptr[ptrStart] ^ 0xff);
                ptrStart++;
            }
        }

        /// <summary>
        /// Fills a buffer with a byte value.
        /// </summary>
        static public void Fill(NativeArray<byte> ptr, int start, int end, byte value)
        {
            while (start < end)
            {
                ptr[start] = value;
                start++;
            }
        }

        static public void Invert(NativeArray<byte> ptr, int ptrStart, int ptrEnd)
        {
            while (ptrStart < ptrEnd)
            {
                ptr[ptrStart] = (byte)(ptr[ptrStart] ^ 0xff);
                ptrStart++;
            }
        }        
        
        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Reverses the endianness of a 2-byte word.
        /// </summary>
        static public void SwapBytes2(byte[] ptr, int start)
        {
            byte byte0 = ptr[start];
            ptr[start] = ptr[start + 1];
            ptr[start + 1] = byte0;
        }

        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Reverses the endianness of a 4-byte word.
        /// </summary>
        static public void SwapBytes4(byte[] ptr, int start)
        {
            byte byte0 = ptr[start];
            byte byte1 = ptr[start + 1];

            ptr[start] = ptr[start + 3];
            ptr[start + 1] = ptr[start + 2];
            ptr[start + 2] = byte1;
            ptr[start + 3] = byte0;
        }

        /// <summary>
        /// Reverses the endianness of a word of arbitrary length.
        /// </summary>
        static public void SwapBytes(byte[] ptr, int start, int nLength)
        {
            for (long i = 0; i < nLength / 2; ++i)
            {
                byte t = ptr[start + i];
                ptr[start + i] = ptr[start + nLength - i - 1];
                ptr[start + nLength - i - 1] = t;
            }
        }

        ///////////////////////////////////////////////////////////////////////////

        public static void SwapByteArray(int bitDepth,
            byte[] byteArray, int startIdx, int count)
        {
            switch (bitDepth)
            {
                case 1:
                case 8:
                    break;

                case 16:
                    SwapByteArray2(byteArray, startIdx, count);
                    break;

                case 32:
                    SwapByteArray4(byteArray, startIdx, count);
                    break;

                default:
                    throw new Exception(
                    "Byte-swapping implemented only for 16-bit and 32-bit depths.");
            }
        }

        /// <summary>
        /// Reverses the endianness of 2-byte words in a byte array.
        /// </summary>
        /// <param name="byteArray">Byte array containing the sequence on which to swap endianness</param>
        /// <param name="startIdx">Byte index of the first word to swap</param>
        /// <param name="count">Number of words to swap</param>
        public static void SwapByteArray2(byte[] byteArray, int startIdx, int count)
        {
            int endIdx = startIdx + count * 2;
            if (byteArray.Length < endIdx)
                throw new IndexOutOfRangeException();


            {
                //fixed (byte* arrayPtr = &byteArray[0])
                {
                    //byte* ptr = arrayPtr + startIdx;
                    //byte* endPtr = arrayPtr + endIdx;
                    while (startIdx < endIdx)
                    {
                        SwapBytes2(byteArray, startIdx);
                        startIdx += 2;
                    }
                }
            }
        }

        /// <summary>
        /// Reverses the endianness of 4-byte words in a byte array.
        /// </summary>
        /// <param name="byteArray">Byte array containing the sequence on which to swap endianness</param>
        /// <param name="startIdx">Byte index of the first word to swap</param>
        /// <param name="count">Number of words to swap</param>
        public static void SwapByteArray4(byte[] byteArray, int startIdx, int count)
        {
            int endIdx = startIdx + count * 4;
            if (byteArray.Length < endIdx)
                throw new IndexOutOfRangeException();


            {
                //fixed (byte* arrayPtr = &byteArray[0])
                {
                    //byte* ptr = arrayPtr + startIdx;
                    //byte* endPtr = arrayPtr + endIdx;
                    while (startIdx < endIdx)
                    {
                        SwapBytes4(byteArray, startIdx);
                        startIdx += 4;
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Calculates the number of bytes required to store a row of an image
        /// with the specified bit depth.
        /// </summary>
        /// <param name="size">The size of the image in pixels.</param>
        /// <param name="bitDepth">The bit depth of the image.</param>
        /// <returns>The number of bytes needed to store a row of the image.</returns>
        public static int BytesPerRow(Size size, int bitDepth)
        {
            switch (bitDepth)
            {
                case 1:
                    return (size.Width + 7) / 8;
                default:
                    return size.Width * BytesFromBitDepth(bitDepth);
            }
        }

        /// <summary>
        /// Round the integer to a multiple.
        /// </summary>
        public static int RoundUp(int value, int multiple)
        {
            if (value == 0)
                return 0;

            if (Math.Sign(value) != Math.Sign(multiple))
            {
                throw new ArgumentException(
                    "value and multiple cannot have opposite signs.");
            }

            var remainder = value % multiple;
            if (remainder > 0)
            {
                value += (multiple - remainder);
            }
            return value;
        }

        /// <summary>
        /// Get number of bytes required to pad to the specified multiple.
        /// </summary>
        public static int GetPadding(int length, int padMultiple)
        {
            if ((length < 0) || (padMultiple < 0))
                throw new ArgumentException();

            var remainder = length % padMultiple;
            if (remainder == 0)
                return 0;

            var padding = padMultiple - remainder;
            return padding;
        }

        /// <summary>
        /// Returns the number of bytes needed to store a single pixel of the
        /// specified bit depth.
        /// </summary>
        public static int BytesFromBitDepth(int depth)
        {
            switch (depth)
            {
                case 1:
                case 8:
                    return 1;
                case 16:
                    return 2;
                case 32:
                    return 4;
                default:
                    throw new ArgumentException("Invalid bit depth.");
            }
        }

        public static short MinChannelCount(this PsdColorMode colorMode)
        {
            switch (colorMode)
            {
                case PsdColorMode.Bitmap:
                case PsdColorMode.Duotone:
                case PsdColorMode.Grayscale:
                case PsdColorMode.Indexed:
                case PsdColorMode.Multichannel:
                    return 1;
                case PsdColorMode.Lab:
                case PsdColorMode.RGB:
                    return 3;
                case PsdColorMode.CMYK:
                    return 4;
            }

            throw new ArgumentException("Unknown color mode.");
        }

        /// <summary>
        /// Verify that the offset and count will remain within the bounds of the
        /// buffer.
        /// </summary>
        /// <returns>True if in bounds, false if out of bounds.</returns>
        public static bool CheckBufferBounds(byte[] data, int offset, int count)
        {
            if (offset < 0)
                return false;
            if (count < 0)
                return false;
            if (offset + count > data.Length)
                return false;

            return true;
        }

        public static void CheckByteArrayLength(long length)
        {
            if (length < 0)
            {
                throw new Exception("Byte array cannot have a negative length.");
            }
            if (length > 0x7fffffc7)
            {
                throw new OutOfMemoryException(
                    "Byte array cannot exceed 2,147,483,591 in length.");
            }
        }

        /// <summary>
        /// Writes a message to the debug console, indicating the current position
        /// in the stream in both decimal and hexadecimal formats.
        /// </summary>
        [Conditional("DEBUG")]
        public static void DebugMessage(Stream stream, string message,
            params object[] args)
        {
            //var formattedMessage = String.Format(message, args);
            //Debug.WriteLine("0x{0:x}, {0}, {1}",
            //stream.Position, formattedMessage);
        }
    }

    /// <summary>
    /// Fixed-point decimal, with 16-bit integer and 16-bit fraction.
    /// </summary>
    internal class UFixed16_16
    {
        public UInt16 Integer { get; set; }
        public UInt16 Fraction { get; set; }

        public UFixed16_16(UInt16 integer, UInt16 fraction)
        {
            Integer = integer;
            Fraction = fraction;
        }

        /// <summary>
        /// Split the high and low words of a 32-bit unsigned integer into a
        /// fixed-point number.
        /// </summary>
        public UFixed16_16(UInt32 value)
        {
            Integer = (UInt16)(value >> 16);
            Fraction = (UInt16)(value & 0x0000ffff);
        }

        public UFixed16_16(double value)
        {
            if (value >= 65536.0) throw new OverflowException();
            if (value < 0) throw new OverflowException();

            Integer = (UInt16)value;

            // Round instead of truncate, because doubles may not represent the
            // fraction exactly.
            Fraction = (UInt16)((value - Integer) * 65536 + 0.5);
        }

        public static implicit operator double(UFixed16_16 value)
        {
            return (double)value.Integer + value.Fraction / 65536.0;
        }
    }
}
