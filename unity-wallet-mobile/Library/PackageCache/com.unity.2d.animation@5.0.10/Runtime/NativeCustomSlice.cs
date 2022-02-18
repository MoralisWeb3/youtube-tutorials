using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.U2D.Animation
{
    internal struct NativeCustomSlice<T> where T : struct
    {
        [NativeDisableUnsafePtrRestriction] public IntPtr data;
        public int length;
        public int stride;

        public static NativeCustomSlice<T> Default()
        {
            return new NativeCustomSlice<T>
            {
                data = IntPtr.Zero,
                length = 0,
                stride = 0
            };
        }

        public unsafe NativeCustomSlice(NativeSlice<T> nativeSlice)
        {
            data = new IntPtr(nativeSlice.GetUnsafeReadOnlyPtr());
            length = nativeSlice.Length;
            stride = nativeSlice.Stride;
        }
        
        public unsafe NativeCustomSlice(NativeSlice<byte> slice, int length, int stride)
        {
            this.data = new IntPtr(slice.GetUnsafeReadOnlyPtr());
            this.length = length;
            this.stride = stride;
        }

        public unsafe T this[int index]
        {
            get { return UnsafeUtility.ReadArrayElementWithStride<T>(data.ToPointer(), index, stride); }
        }

        public int Length
        {
            get { return length; }
        }
    }

    internal struct NativeCustomSliceEnumerator<T> : IEnumerable<T>, IEnumerator<T> where T : struct
    {
        private NativeCustomSlice<T> nativeCustomSlice;
        private int index;

        internal NativeCustomSliceEnumerator(NativeSlice<byte> slice, int length, int stride)
        {
            nativeCustomSlice = new NativeCustomSlice<T>(slice, length, stride);
            index = -1;
            Reset();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public bool MoveNext()
        {
            if (++index < nativeCustomSlice.length)
            {
                return true;
            }
            return false;
        }

        public void Reset()
        {
            index = -1;
        }

        public T Current => nativeCustomSlice[index];

        object IEnumerator.Current => Current;

        void IDisposable.Dispose() {}
    }
}