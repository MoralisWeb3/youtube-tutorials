using System;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEditor.U2D.Common
{
    public struct FindTightRectJob : IJobParallelFor
    {
        [ReadOnly]
        [DeallocateOnJobCompletion]
        NativeArray<IntPtr> m_Buffers;
        [ReadOnly]
        int m_Width;
        [ReadOnly]
        int m_Height;
        NativeArray<RectInt> m_Output;

        public unsafe void Execute(int index)
        {
            var rect = new RectInt(m_Width, m_Height, 0, 0);
            var color = (Color32*)m_Buffers[index].ToPointer();
            for (int i = 0; i < m_Height; ++i)
            {
                for (int j = 0; j < m_Width; ++j)
                {
                    if (color->a != 0)
                    {
                        rect.x = Mathf.Min(j, rect.x);
                        rect.y = Mathf.Min(i, rect.y);
                        rect.width = Mathf.Max(j, rect.width);
                        rect.height = Mathf.Max(i, rect.height);
                    }
                    ++color;
                }
            }
            rect.width = Mathf.Max(0, rect.width - rect.x + 1);
            rect.height = Mathf.Max(0, rect.height - rect.y + 1);
            m_Output[index] = rect;
        }

        public static unsafe RectInt[] Execute(NativeArray<Color32>[] buffers, int width, int height)
        {
            var job = new FindTightRectJob();
            job.m_Buffers = new NativeArray<IntPtr>(buffers.Length, Allocator.TempJob);
            for (int i = 0; i < buffers.Length; ++i)
                job.m_Buffers[i] = new IntPtr(buffers[i].GetUnsafeReadOnlyPtr());
            job.m_Output = new NativeArray<RectInt>(buffers.Length, Allocator.TempJob);
            job.m_Width = width;
            job.m_Height = height;
            // Ensure all jobs are completed before we return since we don't own the buffers
            job.Schedule(buffers.Length, 1).Complete();
            var rects = job.m_Output.ToArray();
            job.m_Output.Dispose();
            return rects;
        }
    }
}
