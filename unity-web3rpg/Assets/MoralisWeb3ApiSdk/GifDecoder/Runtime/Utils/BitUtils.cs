using System;
using System.Collections.Generic;
using System.IO;

namespace ThreeDISevenZeroR.UnityGifDecoder.Utils
{
    public static class BitUtils
    {
        public static bool CheckString(byte[] array, string s)
        {
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] != s[i])
                    return false;
            }

            return true;
        }

        public static int ReadInt16LittleEndian(this Stream reader)
        {
            var b1 = reader.ReadByte8();
            var b2 = reader.ReadByte8();
            return (b2 << 8) + b1;
        }

        public static int ReadInt32LittleEndian(this Stream reader)
        {
            var b1 = reader.ReadByte8();
            var b2 = reader.ReadByte8();
            var b3 = reader.ReadByte8();
            var b4 = reader.ReadByte8();
            return (b4 << 24) + (b3 << 16) + (b2 << 8) + b1;
        }

        public static byte ReadByte8(this Stream reader)
        {
            var b = reader.ReadByte();

            if (b == -1)
                throw new EndOfStreamException();

            return (byte) b;
        }

        public static void AssertByte(this Stream reader, int expectedValue)
        {
            var readByte = reader.ReadByte8();
            if(readByte != expectedValue)
                throw new ArgumentException($"Invalid byte, expected {expectedValue}, got {readByte}");
        }
    
        public static int GetColorTableSize(int data)
        {
            return 1 << (data + 1);
        }
    
        public static int GetBitsFromByte(this byte b, int offset, int count)
        {
            var result = 0;
        
            for (var i = 0; i < count; i++)
            {
                result += (GetBitFromByte(b, offset + i) ? 1 : 0) << i;
            }

            return result;
        }
    
        public static bool GetBitFromByte(this byte b, int offset)
        {
            return (b & (1 << offset)) != 0;
        }

        public static byte[] ReadGifBlocks(Stream reader)
        {
            var blocks = new List<byte>();

            while (true)
            {
                var blockSize = reader.ReadByte8();
                
                if(blockSize == 0)
                    break;
                
                var bytes = new byte[blockSize];
                reader.Read(bytes, 0, bytes.Length);
                blocks.AddRange(bytes);
            }

            return blocks.ToArray();
        }
        
        public static void SkipGifBlocks(Stream reader)
        {
            while (true)
            {
                var blockSize = reader.ReadByte8();

                if (blockSize == 0)
                    return;

                reader.Seek(blockSize, SeekOrigin.Current);
            }
        }
    }
}
