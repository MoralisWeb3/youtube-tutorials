using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.U2D.Animation
{
    internal static class NativeArrayHelpers
    {
        public static unsafe void ResizeIfNeeded<T>(ref NativeArray<T> nativeArray, int size, Allocator allocator = Allocator.Persistent) where T : struct
        {
            bool canDispose = nativeArray.IsCreated;
            if (canDispose && nativeArray.Length != size)
            {
                nativeArray.Dispose();
                canDispose = false;
            }

            if(!canDispose)
                nativeArray = new NativeArray<T>(size, allocator);
        }

        public static void ResizeAndCopyIfNeeded<T>(ref NativeArray<T> nativeArray, int size, Allocator allocator = Allocator.Persistent) where T : struct
        {
            bool canDispose = nativeArray.IsCreated;
            if (canDispose && nativeArray.Length == size)
                return;

            var newArray = new NativeArray<T>(size, allocator);
            if (canDispose)
            {
                NativeArray<T>.Copy(nativeArray, newArray, size < nativeArray.Length ? size : nativeArray.Length);
                nativeArray.Dispose();
            }
            nativeArray = newArray;
        }

        public static unsafe void DisposeIfCreated<T>(this NativeArray<T> nativeArray) where T : struct
        {
            if (nativeArray.IsCreated)
                nativeArray.Dispose();
        }

        [WriteAccessRequired]
        public static unsafe void CopyFromNativeSlice<T, S>(this NativeArray<T> nativeArray, int dstStartIndex, int dstEndIndex, NativeSlice<S> slice, int srcStartIndex, int srcEndIndex) where T : struct where S : struct
        {
            if ((dstEndIndex - dstStartIndex) != (srcEndIndex - srcStartIndex))
                throw new System.ArgumentException($"Destination and Source copy counts must match.", nameof(slice));

            var dstSizeOf = UnsafeUtility.SizeOf<T>();
            var srcSizeOf = UnsafeUtility.SizeOf<T>();

            byte* srcPtr = (byte*)slice.GetUnsafeReadOnlyPtr();
            srcPtr = srcPtr + (srcStartIndex * srcSizeOf);
            byte* dstPtr = (byte*)nativeArray.GetUnsafePtr();
            dstPtr = dstPtr + (dstStartIndex * dstSizeOf);
            UnsafeUtility.MemCpyStride(dstPtr, srcSizeOf, srcPtr, slice.Stride, dstSizeOf, srcEndIndex - srcStartIndex);
        }
    }
}