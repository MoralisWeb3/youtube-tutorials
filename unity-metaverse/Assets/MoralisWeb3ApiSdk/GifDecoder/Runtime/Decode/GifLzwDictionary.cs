﻿﻿using System;
 
namespace ThreeDISevenZeroR.UnityGifDecoder.Decode
{
    /// <summary>
    /// LZW Dictionary used to decode bit stream
    /// </summary>
    public class GifLzwDictionary
    {
        private readonly int[] dictionaryEntryOffsets;
        private readonly int[] dictionaryEntrySizes;
        private byte[] dictionaryHeap;
        private int dictionarySize;
        private int dictionaryHeapPosition;
        
        private int initialDictionarySize;
        private int initialLzwCodeSize;
        private int initialDictionaryHeapPosition;
        private int nextLzwCodeGrowth;
        private int currentMinLzwCodeSize;

        private int codeSize;
        private int clearCodeId;
        private int stopCodeId;
        private int lastCodeId;

        private bool isFull;

        /// <summary>
        /// Creates new instance and allocates dictionary resources
        /// </summary>
        public GifLzwDictionary()
        {
            dictionaryEntryOffsets = new int[4096];
            dictionaryEntrySizes = new int[4096];
            dictionaryHeap = new byte[32768];
        }

        /// <summary>
        /// Initializes dictionary with minimum code size
        /// </summary>
        /// <param name="minLzwCodeSize">new minimum lzw code size</param>
        public void InitWithWordSize(int minLzwCodeSize)
        {
            if (currentMinLzwCodeSize != minLzwCodeSize)
            {
                currentMinLzwCodeSize = minLzwCodeSize;
                dictionaryHeapPosition = 0;
                dictionarySize = 0;
            
                var colorCount = 1 << minLzwCodeSize;

                for (var i = 0; i < colorCount; i++)
                {
                    dictionaryEntryOffsets[i] = dictionaryHeapPosition;
                    dictionaryEntrySizes[i] = 1;
                    dictionaryHeap[dictionaryHeapPosition++] = (byte) i;
                }
            
                initialDictionarySize = colorCount + 2;
                initialLzwCodeSize = minLzwCodeSize + 1;
                initialDictionaryHeapPosition = dictionaryHeapPosition;

                clearCodeId = colorCount;
                stopCodeId = colorCount + 1;
            }

            Clear();
        }

        /// <summary>
        /// Clear dictionary contents
        /// </summary>
        public void Clear()
        {
            codeSize = initialLzwCodeSize;
            dictionarySize = initialDictionarySize;
            dictionaryHeapPosition = initialDictionaryHeapPosition;
            nextLzwCodeGrowth = 1 << codeSize;
            isFull = false;
            lastCodeId = -1;
        }

        /// <summary>
        /// Decode block reader to canvas
        /// </summary>
        public void DecodeStream(GifBitBlockReader reader, GifCanvas c)
        {
            while (true)
            {
                var entry = reader.ReadBits(codeSize);
  
                if (entry == clearCodeId)
                {
                    Clear();
                    continue;
                }

                if (entry == stopCodeId)
                {
                    return;
                }

                // Decode
                if (entry < dictionarySize)
                {
                    if (lastCodeId >= 0)
                        CreateNewCode(lastCodeId, entry);
                
                    lastCodeId = entry;
                }
                else
                {
                    lastCodeId = CreateNewCode(lastCodeId, lastCodeId);
                }
                
                // Output
                var position = dictionaryEntryOffsets[lastCodeId];
                var size = dictionaryEntrySizes[lastCodeId];
                var heapEnd = position + size;
                    
                for (var i = position; i < heapEnd; i++)
                    c.OutputPixel(dictionaryHeap[i]);
            }
        }

        /// <summary>
        /// Create new dictionary entry from base entry
        /// </summary>
        public int CreateNewCode(int baseEntry, int deriveEntry)
        {
            if (isFull)
                return -1;

            var entryHeapPosition = dictionaryEntryOffsets[baseEntry];
            var entrySize = dictionaryEntrySizes[baseEntry];
            var newEntryOffset = dictionaryHeapPosition;
            var newEntrySize = entrySize + 1;
            var newHeapCapacity = newEntryOffset + newEntrySize;
            
            if (dictionaryHeap.Length < newHeapCapacity)
                Array.Resize(ref dictionaryHeap, Math.Max(dictionaryHeap.Length * 2, newHeapCapacity));
 
            if (entrySize < 12)
            {
                // It is faster to just copy array manually for small values
                var endValue = entryHeapPosition + entrySize;
                for (var i = entryHeapPosition; i < endValue; i++)
                    dictionaryHeap[dictionaryHeapPosition++] = dictionaryHeap[i];
            }
            else
            {
                Buffer.BlockCopy(dictionaryHeap, entryHeapPosition,
                    dictionaryHeap, dictionaryHeapPosition, entrySize);
                dictionaryHeapPosition += entrySize;
            }

            dictionaryHeap[dictionaryHeapPosition++] = deriveEntry < initialDictionarySize
                ? (byte) deriveEntry : dictionaryHeap[dictionaryEntryOffsets[deriveEntry]];

            var insertPosition = dictionarySize++;
            dictionaryEntryOffsets[insertPosition] = newEntryOffset;
            dictionaryEntrySizes[insertPosition] = newEntrySize;

            if (dictionarySize >= nextLzwCodeGrowth)
            {
                codeSize++;
                nextLzwCodeGrowth = codeSize == 12 ? int.MaxValue : 1 << codeSize;
            }

            // Dictionary is capped at 4096 elements
            if (dictionarySize >= 4096)
                isFull = true;

            return insertPosition;
        }
    }
}