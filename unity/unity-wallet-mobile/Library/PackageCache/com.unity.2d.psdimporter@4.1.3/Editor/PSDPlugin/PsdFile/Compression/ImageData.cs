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
using PDNWrapper;
using System.IO.Compression;

namespace PhotoshopFile.Compression
{
    internal abstract class ImageData
    {
        public int BitDepth { get; private set; }

        public int BytesPerRow { get; private set; }

        public Size Size { get; private set; }

        protected abstract bool AltersWrittenData { get; }

        protected ImageData(Size size, int bitDepth)
        {
            Size = size;
            BitDepth = bitDepth;
            BytesPerRow = Util.BytesPerRow(size, bitDepth);
        }

        /// <summary>
        /// Reads decompressed image data.
        /// </summary>
        public virtual byte[] Read()
        {
            var imageLongLength = (long)BytesPerRow * Size.Height;
            Util.CheckByteArrayLength(imageLongLength);

            var buffer = new byte[imageLongLength];
            Read(buffer);
            return buffer;
        }

        internal abstract void Read(byte[] buffer);

        /// <summary>
        /// Reads compressed image data.
        /// </summary>
        public abstract byte[] ReadCompressed();
    }
}
