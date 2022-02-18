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

namespace PhotoshopFile.Compression
{
    internal class RawImage : ImageData
    {
        private byte[] data;

        protected override bool AltersWrittenData
        {
            get { return false; }
        }

        public RawImage(byte[] data, Size size, int bitDepth)
            : base(size, bitDepth)
        {
            this.data = data;
        }

        internal override void Read(byte[] buffer)
        {
            Array.Copy(data, buffer, data.Length);
        }

        public override byte[] ReadCompressed()
        {
            return data;
        }
    }
}
