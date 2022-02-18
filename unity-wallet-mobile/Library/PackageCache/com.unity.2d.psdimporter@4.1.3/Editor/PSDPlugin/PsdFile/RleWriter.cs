/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2015 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using UnityEngine.Assertions;

namespace PhotoshopFile
{
    internal class RleWriter
    {
        private int maxPacketLength = 128;

        // Current task
        private object rleLock;
        private Stream stream;
        private byte[] data;
        private int offset;

        // Current packet
        private bool isRepeatPacket;
        private int idxPacketStart;
        private int packetLength;

        private byte runValue;
        private int runLength;

        public RleWriter(Stream stream)
        {
            rleLock = new object();
            this.stream = stream;
        }

        /// <summary>
        /// Encodes byte data using PackBits RLE compression.
        /// </summary>
        /// <param name="data">Raw data to be encoded.</param>
        /// <param name="offset">Offset at which to begin transferring data.</param>
        /// <param name="count">Number of bytes of data to transfer.</param>
        /// <returns>Number of compressed bytes written to the stream.</returns>
        /// <remarks>
        /// There are multiple ways to encode two-byte runs:
        ///   1. Apple PackBits only encodes three-byte runs as repeats.
        ///   2. Adobe Photoshop encodes two-byte runs as repeats, unless preceded
        ///      by literals.
        ///   3. TIFF PackBits recommends that two-byte runs be encoded as repeats,
        ///      unless preceded *and* followed by literals.
        ///
        /// This class adopts the Photoshop behavior, as it has slightly better
        /// compression efficiency than Apple PackBits, and is easier to implement
        /// than TIFF PackBits.
        /// </remarks>
        public int Write(byte[] data, int offset, int count)
        {
            if (!Util.CheckBufferBounds(data, offset, count))
                throw new ArgumentOutOfRangeException();

            // We cannot encode a count of 0, because the PackBits flag-counter byte
            // uses 0 to indicate a length of 1.
            if (count == 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            lock (rleLock)
            {
                var startPosition = stream.Position;

                this.data = data;
                this.offset = offset;
                //fixed (byte* ptrData = &data[0])
                {
                    //byte* ptr = ptrData + offset;
                    //byte* ptrEnd = ptr + count;
                    //var bytesEncoded = EncodeToStream(ptr, ptrEnd);
                    //Debug.Assert(bytesEncoded == count, "Encoded byte count should match the argument.");
                    var bytesEncoded = EncodeToStream(data, offset, offset + count);
                    Assert.AreEqual(bytesEncoded, count, "Encoded byte count should match the argument.");
                }

                return (int)(stream.Position - startPosition);
            }
        }

        private void ClearPacket()
        {
            this.isRepeatPacket = false;
            this.packetLength = 0;
        }

        private void WriteRepeatPacket(int length)
        {
            var header = unchecked((byte)(1 - length));
            stream.WriteByte(header);
            stream.WriteByte(runValue);
        }

        private void WriteLiteralPacket(int length)
        {
            var header = unchecked((byte)(length - 1));
            stream.WriteByte(header);
            stream.Write(data, idxPacketStart, length);
        }

        private void WritePacket()
        {
            if (isRepeatPacket)
                WriteRepeatPacket(packetLength);
            else
                WriteLiteralPacket(packetLength);
        }

        private void StartPacket(int count,
            bool isRepeatPacket, int runLength, byte value)
        {
            this.isRepeatPacket = isRepeatPacket;

            this.packetLength = runLength;
            this.runLength = runLength;
            this.runValue = value;

            this.idxPacketStart = offset + count;
        }

        private void ExtendPacketAndRun(byte value)
        {
            packetLength++;
            runLength++;
        }

        private void ExtendPacketStartNewRun(byte value)
        {
            packetLength++;
            runLength = 1;
            runValue = value;
        }

        private int EncodeToStream(byte[] ptr, int start, int end /*byte* ptr, byte* ptrEnd*/)
        {
            // Begin the first packet.
            StartPacket(0, false, 1, ptr[start]);
            int numBytesEncoded = 1;
            start++;

            // Loop invariant: Packet is never empty.
            while (start < end)
            {
                var value = ptr[start];

                if (packetLength == 1)
                {
                    isRepeatPacket = (value == runValue);
                    if (isRepeatPacket)
                        ExtendPacketAndRun(value);
                    else
                        ExtendPacketStartNewRun(value);
                }
                else if (packetLength == maxPacketLength)
                {
                    // Packet is full, so write it out and start a new one.
                    WritePacket();
                    StartPacket(numBytesEncoded, false, 1, value);
                }
                else if (isRepeatPacket)
                {
                    // Decide whether to continue the repeat packet.
                    if (value == runValue)
                        ExtendPacketAndRun(value);
                    else
                    {
                        // Different color, so terminate the run and start a new packet.
                        WriteRepeatPacket(packetLength);
                        StartPacket(numBytesEncoded, false, 1, value);
                    }
                }
                else
                {
                    // Decide whether to continue the literal packet.
                    if (value == runValue)
                    {
                        ExtendPacketAndRun(value);

                        // A 3-byte run terminates the literal and starts a new repeat
                        // packet.  That's because the 3-byte run can be encoded as a
                        // 2-byte repeat.  So even if the run ends at 3, we've already
                        // paid for the next flag-counter byte.
                        if (runLength == 3)
                        {
                            // The 3-byte run can come in the middle of a literal packet,
                            // but not at the beginning.  The first 2 bytes of the run
                            // should've triggered a repeat packet.
                            Debug.Assert(packetLength > 3);

                            // -2 because numBytesEncoded has not yet been incremented
                            WriteLiteralPacket(packetLength - 3);
                            StartPacket(numBytesEncoded - 2, true, 3, value);
                        }
                    }
                    else
                    {
                        ExtendPacketStartNewRun(value);
                    }
                }

                start++;
                numBytesEncoded++;
            }

            // Loop terminates with a non-empty packet waiting to be written out.
            WritePacket();
            ClearPacket();

            return numBytesEncoded;
        }
    }
}
