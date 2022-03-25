using System.IO;
using ThreeDISevenZeroR.UnityGifDecoder.Utils;

namespace ThreeDISevenZeroR.UnityGifDecoder
{
    /// <summary>
    /// Reader for GIF Blocks
    /// </summary>
    public class GifBitBlockReader
    {
        private Stream stream;
        private int currentByte;
        private int currentBitPosition;
        private int currentBufferPosition;
        private int currentBufferSize;
        private bool endReached;
        private readonly byte[] buffer;

        public GifBitBlockReader()
        {
            buffer = new byte[256];
        }
        
        public GifBitBlockReader(Stream stream) : this()
        {
            SetStream(stream);
        }

        /// <summary>
        /// Set new stream
        /// </summary>
        public void SetStream(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Read first block and initialize reading
        /// </summary>
        public void StartNewReading()
        {
            currentByte = 0;
            currentBitPosition = 8;
            ReadNextBlock();
        }

        /// <summary>
        /// Skips to the last block, if end is not reached
        /// </summary>
        public void FinishReading()
        {
            while (!endReached)
            {
                ReadNextBlock();
            }
        }

        /// <summary>
        /// Read bits from stream and construct value
        /// </summary>
        /// <param name="count">Bit count to read</param>
        /// <returns>Value from readed bits</returns>
        public int ReadBits(int count)
        {
            var result = 0;
            var bitsToRead = count;
            var offset = 0;
            var bitsAvailable = 8 - currentBitPosition;

            while(bitsToRead > 0)
            {
                if (currentBitPosition >= 8)
                {
                    currentBitPosition = 0;
                    bitsAvailable = 8;
                    
                    if (endReached)
                    {
                        // Some gifs can read slightly past end of a stream
                        // (since there is a zero byte afterwards anyway, it is safe to return 0)
                        currentByte = 0;
                    }
                    else
                    {
                        currentByte = buffer[currentBufferPosition++];
                        if (currentBufferPosition == currentBufferSize)
                            ReadNextBlock();
                    }
                }

                var mask = (byte) (((1 << bitsToRead) - 1) << currentBitPosition);
                var readCount = bitsAvailable < bitsToRead ? bitsAvailable : bitsToRead;

                result += ((mask & currentByte) >> currentBitPosition) << offset;

                currentBitPosition += readCount;
                bitsToRead -= readCount;
                offset += readCount;
            }

            return result;
        }
        
        private void ReadNextBlock()
        {
            currentBufferSize = stream.ReadByte8();
            currentBufferPosition = 0;
            endReached = currentBufferSize == 0;
            
            if(!endReached)
                stream.Read(buffer, 0, currentBufferSize);
        }
    }
}