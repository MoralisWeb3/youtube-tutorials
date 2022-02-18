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
    internal class ZipPredict16Image : ImageData
    {
        private ImageData zipImage;

        protected override bool AltersWrittenData
        {
            get { return true; }
        }

        public ZipPredict16Image(byte[] zipData, Size size)
            : base(size, 16)
        {
            // 16-bitdepth images are delta-encoded word-by-word.  The deltas
            // are thus big-endian and must be reversed for further processing.
            var zipRawImage = new ZipImage(zipData, size, 16);
            zipImage = new EndianReverser(zipRawImage);
        }

        internal override void Read(byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                return;
            }

            zipImage.Read(buffer);

            {
                {
                    Unpredict(buffer);
                }
            }
        }

        public override byte[] ReadCompressed()
        {
            return zipImage.ReadCompressed();
        }

        private void Predict(/*UInt16**/ byte[] ptrData)
        {
            int size = sizeof(UInt16);
            // Delta-encode each row
            for (int i = 0; i < Size.Height; i++)
            {
                int rowOffset = Size.Width * i * size;
                //UInt16* ptrDataRow = ptrData;
                var ptrDataRowEnd = Size.Width - 1;

                // Start with the last column in the row
                while (ptrDataRowEnd > 0)
                {
                    var v = BitConverter.ToUInt16(ptrData, ptrDataRowEnd * size + rowOffset);
                    var v1 = BitConverter.ToUInt16(ptrData, (ptrDataRowEnd - 1) * size + rowOffset);
                    v -= v1;
                    var b = BitConverter.GetBytes(v);
                    for (int c = 0; c < b.Length; ++c)
                    {
                        ptrData[ptrDataRowEnd * size + rowOffset + c] = b[c];
                    }
                    ptrDataRowEnd--;
                }
            }
        }

        /// <summary>
        /// Unpredicts the decompressed, native-endian image data.
        /// </summary>
        private void Unpredict(byte[] ptrData)
        {
            int size = sizeof(UInt16);
            // Delta-decode each row
            for (int i = 0; i < Size.Height; i++)
            {
                //UInt16* ptrDataRowEnd = ptrData + Size.Width;
                int rowOffset = Size.Width * i * size;
                // Start with column index 1 on each row
                int start = 1;
                while (start < Size.Width)
                {
                    var v = BitConverter.ToUInt16(ptrData, start * size + rowOffset);
                    var v1 = BitConverter.ToUInt16(ptrData, (start - 1) * size + rowOffset);
                    v += v1;
                    var b = BitConverter.GetBytes(v);
                    for (int c = 0; c < b.Length; ++c)
                    {
                        ptrData[start * size + rowOffset + c] = b[c];
                    }
                    start++;
                }
            }
        }
    }
}
