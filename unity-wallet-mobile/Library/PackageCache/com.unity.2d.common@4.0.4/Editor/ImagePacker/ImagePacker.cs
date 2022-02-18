//#define PACKING_DEBUG

using System;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEditor.U2D.Common
{
    internal static class ImagePacker
    {
        /// <summary>
        /// Given an array of rects, the method returns an array of rects arranged within outPackedWidth and outPackedHeight
        /// </summary>
        /// <param name="rects">Rects to pack</param>
        /// <param name="padding">Padding between each rect</param>
        /// <param name="outPackedRects">Rects arranged within outPackedWidth and outPackedHeight</param>
        /// <param name="outPackedWidth">Width of the packed rects</param>
        /// <param name="outPackedHeight">Height of the packed rects</param>
        public static void Pack(RectInt[] rects, int padding, out RectInt[] outPackedRects, out int outPackedWidth, out int outPackedHeight)
        {
            var packNode = InternalPack(rects, padding);
            outPackedWidth = packNode.rect.width;
            outPackedHeight = packNode.rect.height;
            var visitor = new CollectPackNodePositionVisitor();
            packNode.AcceptVisitor(visitor);

            outPackedRects = new RectInt[rects.Length];
            for (int i = 0; i < rects.Length; ++i)
                outPackedRects[i] = new RectInt(visitor.positions[i].x + padding, visitor.positions[i].y + padding, rects[i].width, rects[i].height);
#if PACKING_DEBUG
            var emptyNodeCollector = new CollectEmptyNodePositionVisitor();
            packNode.AcceptVisitor(emptyNodeCollector);
            Array.Resize(ref outPackedRects, rects.Length + emptyNodeCollector.emptyAreas.Count);
            for (int i = rects.Length; i < outPackedRects.Length; ++i)
                outPackedRects[i] = emptyNodeCollector.emptyAreas[i - rects.Length];
#endif
        }

        /// <summary>
        /// Packs image buffer into a single buffer. Image buffers are assumed to be 4 bytes per pixel in RGBA format
        /// </summary>
        /// <param name="buffers">Image buffers to pack</param>
        /// <param name="width">Image buffers width</param>
        /// <param name="height">Image buffers height</param>
        /// <param name="padding">Padding between each packed image</param>
        /// <param name="outPackedBuffer">Packed image buffer</param>
        /// <param name="outPackedBufferWidth">Packed image buffer's width</param>
        /// <param name="outPackedBufferHeight">Packed iamge buffer's height</param>
        /// <param name="outPackedRect">Location of each image buffers in the packed buffer</param>
        /// <param name="outUVTransform">Translation data from image original buffer to packed buffer</param>
        public static void Pack(NativeArray<Color32>[] buffers, int width, int height, int padding, out NativeArray<Color32> outPackedBuffer, out int outPackedBufferWidth, out int outPackedBufferHeight, out RectInt[] outPackedRect, out Vector2Int[] outUVTransform)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Pack");
            // Determine the area that contains data in the buffer
            outPackedBuffer = default(NativeArray<Color32>);
            try
            {
                var tightRects = FindTightRectJob.Execute(buffers, width, height);
                Pack(tightRects, padding, out outPackedRect, out outPackedBufferWidth, out outPackedBufferHeight);
                outUVTransform = new Vector2Int[tightRects.Length];
                for (int i = 0; i < outUVTransform.Length; ++i)
                {
                    outUVTransform[i] = new Vector2Int(outPackedRect[i].x - tightRects[i].x, outPackedRect[i].y - tightRects[i].y);
                }
                outPackedBuffer = new NativeArray<Color32>(outPackedBufferWidth * outPackedBufferHeight, Allocator.Persistent);

                Blit(outPackedBuffer, outPackedRect, outPackedBufferWidth, buffers, tightRects, width, padding);
            }
            catch (Exception ex)
            {
                if (outPackedBuffer.IsCreated)
                    outPackedBuffer.Dispose();
                throw ex;
            }
            finally
            {
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        static ImagePackNode InternalPack(RectInt[] rects, int padding)
        {
            if (rects == null || rects.Length == 0)
                return new ImagePackNode() { rect = new RectInt(0, 0, 0, 0)};
            var sortedRects = new ImagePackRect[rects.Length];
            for (int i = 0; i < rects.Length; ++i)
            {
                sortedRects[i] = new ImagePackRect();
                sortedRects[i].rect = rects[i];
                sortedRects[i].index = i;
            }
            Array.Sort<ImagePackRect>(sortedRects);
            var root = new ImagePackNode();
            root.rect = new RectInt(0, 0, (int)NextPowerOfTwo((ulong)rects[0].width), (int)NextPowerOfTwo((ulong)rects[0].height));

            for (int i = 0; i < rects.Length; ++i)
            {
                if (!root.Insert(sortedRects[i], padding)) // we can't fit
                {
                    int newWidth = root.rect.width , newHeight = root.rect.height;
                    if (root.rect.width < root.rect.height)
                        newWidth = (int)NextPowerOfTwo((ulong)root.rect.width + 1);
                    else
                        newHeight = (int)NextPowerOfTwo((ulong)root.rect.height + 1);
                    // Reset all packing and try again
                    root = new ImagePackNode();
                    root.rect = new RectInt(0, 0, newWidth, newHeight);
                    i = -1;
                }
            }
            return root;
        }

        public static unsafe void Blit(NativeArray<Color32> buffer, RectInt[] blitToArea, int bufferbytesPerRow, NativeArray<Color32>[] originalBuffer, RectInt[] blitFromArea, int bytesPerRow, int padding)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Blit");

            var c = (Color32*)buffer.GetUnsafePtr();
            for (int bufferIndex = 0; bufferIndex < blitToArea.Length && bufferIndex < originalBuffer.Length && bufferIndex < blitFromArea.Length; ++bufferIndex)
            {
                var b = (Color32*)originalBuffer[bufferIndex].GetUnsafeReadOnlyPtr();
                var rectFrom = blitFromArea[bufferIndex];
                var rectTo = blitToArea[bufferIndex];
                for (int i = 0; i < rectFrom.height; ++i)
                {
                    for (int j = 0; j < rectFrom.width; ++j)
                    {
                        Color32 cc = b[(rectFrom.y + i) * bytesPerRow + rectFrom.x + j];
                        c[((rectTo.y + i) * bufferbytesPerRow) + rectTo.x + j] = cc;
                    }
                }
            }

#if PACKING_DEBUG
            var emptyColors = new Color32[]
            {
                new Color32((byte)255, (byte)0, (byte)0, (byte)255),
                new Color32((byte)255, (byte)255, (byte)0, (byte)255),
                new Color32((byte)255, (byte)0, (byte)255, (byte)255),
                new Color32((byte)255, (byte)255, (byte)255, (byte)255),
                new Color32((byte)0, (byte)255, (byte)0, (byte)255),
                new Color32((byte)0, (byte)0, (byte)255, (byte)255)
            };

            for (int k = originalBuffer.Length; k < blitToArea.Length; ++k)
            {
                var rectFrom = blitToArea[k];
                for (int i = 0; i < rectFrom.height; ++i)
                {
                    for (int j = 0; j < rectFrom.width; ++j)
                    {
                        c[((rectFrom.y + i) * bufferbytesPerRow) + rectFrom.x + j] =
                            emptyColors[k % emptyColors.Length];
                    }
                }
            }
#endif
            UnityEngine.Profiling.Profiler.EndSample();
        }

        internal static ulong NextPowerOfTwo(ulong v)
        {
            v -= 1;
            v |= v >> 16;
            v |= v >> 8;
            v |= v >> 4;
            v |= v >> 2;
            v |= v >> 1;
            return v + 1;
        }

        internal class ImagePackRect : IComparable<ImagePackRect>
        {
            public RectInt rect;
            public int index;

            public int CompareTo(ImagePackRect obj)
            {
                var lhsArea = rect.width * rect.height;
                var rhsArea = obj.rect.width * obj.rect.height;
                if (lhsArea > rhsArea)
                    return -1;
                if (lhsArea < rhsArea)
                    return 1;
                if (index < obj.index)
                    return -1;

                return 1;
            }
        }
    }
}
