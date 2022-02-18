using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.U2D
{

    namespace UTess
    {

        [StructLayout(LayoutKind.Sequential)]
        [DebuggerDisplay("Length = {Length}")]
        internal unsafe struct ArraySlice<T> : System.IEquatable<ArraySlice<T>> where T : struct
        {
            [NativeDisableUnsafePtrRestriction] internal byte* m_Buffer;
            internal int m_Stride;
            internal int m_Length;
            
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            internal int m_MinIndex;
            internal int m_MaxIndex;
            internal AtomicSafetyHandle m_Safety;
#endif

            public ArraySlice(NativeArray<T> array, int start, int length)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (start < 0)
                    throw new ArgumentOutOfRangeException(nameof(start), $"Slice start {start} < 0.");
                if (length < 0)
                    throw new ArgumentOutOfRangeException(nameof(length), $"Slice length {length} < 0.");
                if (start + length > array.Length)
                    throw new ArgumentException(
                        $"Slice start + length ({start + length}) range must be <= array.Length ({array.Length})");
                m_MinIndex = 0;
                m_MaxIndex = length - 1;
                m_Safety = Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetAtomicSafetyHandle(array);
#endif

                m_Stride = UnsafeUtility.SizeOf<T>();
                var ptr = (byte*) array.GetUnsafePtr() + m_Stride * start;
                m_Buffer = ptr;
                m_Length = length;
            }

            public bool Equals(ArraySlice<T> other)
            {
                return m_Buffer == other.m_Buffer && m_Stride == other.m_Stride && m_Length == other.m_Length;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is ArraySlice<T> && Equals((ArraySlice<T>) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (int) m_Buffer;
                    hashCode = (hashCode * 397) ^ m_Stride;
                    hashCode = (hashCode * 397) ^ m_Length;
                    return hashCode;
                }
            }

            public static bool operator ==(ArraySlice<T> left, ArraySlice<T> right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ArraySlice<T> left, ArraySlice<T> right)
            {
                return !left.Equals(right);
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            // These are double-whammy excluded to we can elide bounds checks in the Burst disassembly view
            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            void CheckReadIndex(int index)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (index < m_MinIndex || index > m_MaxIndex)
                    FailOutOfRangeError(index);
#endif
            }

            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            void CheckWriteIndex(int index)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (index < m_MinIndex || index > m_MaxIndex)
                    FailOutOfRangeError(index);
#endif
            }

            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            private void FailOutOfRangeError(int index)
            {
                if (index < Length && (m_MinIndex != 0 || m_MaxIndex != Length - 1))
                    throw new System.IndexOutOfRangeException(
                        $"Index {index} is out of restricted IJobParallelFor range [{m_MinIndex}...{m_MaxIndex}] in ReadWriteBuffer.\n" +
                        "ReadWriteBuffers are restricted to only read & write the element at the job index. " +
                        "You can use double buffering strategies to avoid race conditions due to " +
                        "reading & writing in parallel to the same elements from a job.");

                throw new System.IndexOutOfRangeException($"Index {index} is out of range of '{Length}' Length.");
            }

#endif

            public static unsafe ArraySlice<T> ConvertExistingDataToArraySlice(void* dataPointer, int stride, int length)
            {
                if (length < 0)
                    throw new System.ArgumentException($"Invalid length of '{length}'. It must be greater than 0.",
                        nameof(length));
                if (stride < 0)
                    throw new System.ArgumentException($"Invalid stride '{stride}'. It must be greater than 0.",
                        nameof(stride));

                var newSlice = new ArraySlice<T>
                {
                    m_Stride = stride,
                    m_Buffer = (byte*) dataPointer,
                    m_Length = length,
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    m_MinIndex = 0,
                    m_MaxIndex = length - 1,
#endif
                };

                return newSlice;
            }

            public T this[int index]
            {
                get
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    CheckReadIndex(index);
#endif
                    return UnsafeUtility.ReadArrayElementWithStride<T>(m_Buffer, index, m_Stride);
                }

                [WriteAccessRequired]
                set
                {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    CheckWriteIndex(index);
#endif
                    UnsafeUtility.WriteArrayElementWithStride(m_Buffer, index, m_Stride, value);
                }
            }

            public int Stride => m_Stride;
            public int Length => m_Length;

        }
        
    }

}