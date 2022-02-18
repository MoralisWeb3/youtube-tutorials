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
    internal class ZipPredict32Image : ImageData
    {
        private ZipImage zipImage;

        protected override bool AltersWrittenData
        {
            // Prediction will pack the data into a temporary buffer, so the
            // original data will remain unchanged.
            get { return false; }
        }

        public ZipPredict32Image(byte[] zipData, Size size)
            : base(size, 32)
        {
            zipImage = new ZipImage(zipData, size, 32);
        }

        internal override void Read(byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                return;
            }

            var predictedData = new byte[buffer.Length];
            zipImage.Read(predictedData);


            {
                //fixed (byte* ptrData = &predictedData[0])
                //fixed (byte* ptrOutput = &buffer[0])
                {
                    Unpredict(predictedData, buffer);
                }
            }
        }

        public override byte[] ReadCompressed()
        {
            return zipImage.ReadCompressed();
        }

        private void Predict(byte[] ptrData, byte[] ptrOutput /*Int32* ptrData, byte* ptrOutput*/)
        {
            int size = sizeof(Int32);
            int inputIndex = 0;
            int outputIndex = 0;
            for (int i = 0; i < Size.Height; i++)
            {
                // Pack together the individual bytes of the 32-bit words, high-order
                // bytes before low-order bytes.
                int offset1 = Size.Width;
                int offset2 = 2 * offset1;
                int offset3 = 3 * offset1;

                //Int32* ptrDataRow = ptrData;
                //Int32* ptrDataRowEnd = ptrDataRow + Size.Width;
                int start = 0, end = Size.Width;
                //while (ptrData < ptrDataRowEnd)
                while (start < end)
                {
                    Int32 data = BitConverter.ToInt32(ptrData, inputIndex);
                    ptrOutput[start + outputIndex] = (byte)(data >> 24);
                    ptrOutput[start + outputIndex + offset1] = (byte)(data >> 16);
                    ptrOutput[start + outputIndex + offset2] = (byte)(data >> 8);
                    ptrOutput[start + outputIndex + offset3] = (byte)(data);

                    //ptrData++;
                    //ptrOutput++;
                    start++;
                    inputIndex += size;
                }

                // Delta-encode the row
                //byte* ptrOutputRow = ptrOutput;
                //byte* ptrOutputRowEnd = ptrOutputRow + BytesPerRow;

                //ptrOutput = ptrOutputRowEnd - 1;
                start = BytesPerRow - 1;
                while (start > 0)
                {
                    ptrOutput[start + outputIndex] -= ptrOutput[start + outputIndex - 1];
                    start--;
                }
                outputIndex += BytesPerRow;
                // Advance pointer to next row
                //ptrOutput = ptrOutputRowEnd;
                //Debug.Assert(ptrData == ptrDataRowEnd);
            }
        }

        /// <summary>
        /// Unpredicts the raw decompressed image data into a 32-bpp bitmap with
        /// native endianness.
        /// </summary>
        private void Unpredict(byte[] ptrData, byte[] ptrOutput /*byte* ptrData, Int32* ptrOutput*/)
        {
            int inputIndex = 0;
            int outputIndex = 0;
            for (int i = 0; i < Size.Height; i++)
            {
                //byte* ptrDataRow = ptrData;
                //byte* ptrDataRowEnd = ptrDataRow + BytesPerRow;

                // Delta-decode each row
                //ptrData++;
                //while (ptrData < ptrDataRowEnd)
                int startIndex = 1;
                while (startIndex < BytesPerRow)
                {
                    //*ptrData += *(ptrData - 1);
                    //ptrData++;
                    ptrData[inputIndex + startIndex] += ptrData[inputIndex + startIndex - 1];
                    startIndex++;
                }

                // Within each row, the individual bytes of the 32-bit words are
                // packed together, high-order bytes before low-order bytes.
                // We now unpack them into words.
                int offset1 = Size.Width;
                int offset2 = 2 * offset1;
                int offset3 = 3 * offset1;

                //ptrData = ptrDataRow;
                //Int32* ptrOutputRowEnd = ptrOutput + Size.Width;
                //while (ptrOutput < ptrOutputRowEnd)
                startIndex = 0;
                while (startIndex < Size.Width)
                {
                    Int32 pp = (Int32)ptrData[inputIndex + startIndex] << 24;
                    pp |= (Int32)ptrData[inputIndex + startIndex + offset1] << 16;
                    pp |= (Int32)ptrData[inputIndex + startIndex + offset2] << 8;
                    pp |= (Int32)ptrData[inputIndex + startIndex + offset3];
                    byte[] rr = BitConverter.GetBytes(pp);
                    for (int k = 0; k < rr.Length; ++k)
                    {
                        ptrOutput[outputIndex] = rr[k];
                        outputIndex++;
                    }
                    startIndex++;
                    //*ptrOutput = *(ptrData) << 24
                    //  | *(ptrData + offset1) << 16
                    //  | *(ptrData + offset2) << 8
                    //  | *(ptrData + offset3);

                    //ptrData++;
                    //ptrOutput++;
                }

                // Advance pointer to next row
                //ptrData = ptrDataRowEnd;
                //Debug.Assert(ptrOutput == ptrOutputRowEnd);
                inputIndex += BytesPerRow;
            }
        }
    }
}
