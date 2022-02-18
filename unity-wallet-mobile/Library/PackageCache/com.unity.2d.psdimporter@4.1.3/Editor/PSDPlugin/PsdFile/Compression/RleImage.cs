/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is ptortorovided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2016 Tao Yue
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using PDNWrapper;
using System.IO;
using System.Linq;

namespace PhotoshopFile.Compression
{
    internal class RleImage : ImageData
    {
        private byte[] rleData;
        private RleRowLengths rleRowLengths;

        protected override bool AltersWrittenData
        {
            get { return false; }
        }

        public RleImage(byte[] rleData, RleRowLengths rleRowLengths,
                        Size size, int bitDepth)
            : base(size, bitDepth)
        {
            this.rleData = rleData;
            this.rleRowLengths = rleRowLengths;
        }

        internal override void Read(byte[] buffer)
        {
            var rleStream = new MemoryStream(rleData);
            var rleReader = new RleReader(rleStream);
            var bufferIndex = 0;
            for (int i = 0; i < Size.Height; i++)
            {
                var bytesRead = rleReader.Read(buffer, bufferIndex, BytesPerRow);
                if (bytesRead != BytesPerRow)
                {
                    throw new Exception("RLE row decompressed to unexpected length.");
                }
                bufferIndex += bytesRead;
            }
        }

        public override byte[] ReadCompressed()
        {
            return rleData;
        }
    }
}
