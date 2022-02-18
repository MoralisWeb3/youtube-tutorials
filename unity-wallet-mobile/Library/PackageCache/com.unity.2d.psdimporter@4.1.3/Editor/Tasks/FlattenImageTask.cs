using System;
using System.Collections.Generic;
using PDNWrapper;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEditor.U2D.PSD
{
    static class FlattenImageTask
    {
        static unsafe public void Execute(List<BitmapLayer> layer, bool importHiddenLayer, int width, int height, NativeArray<Color32> output)
        {
            UnityEngine.Profiling.Profiler.BeginSample("FlattenImage");
            List<IntPtr> buffers = new List<IntPtr>();
            for (int i = layer.Count - 1; i >= 0; --i)
            {
                GetBuffersToMergeFromLayer(layer[i], importHiddenLayer, buffers);
            }

            if (buffers.Count == 0)
                return;

            var layersPerJob = buffers.Count / (SystemInfo.processorCount == 0 ? 8 : SystemInfo.processorCount);
            layersPerJob = Mathf.Max(layersPerJob, 1);

            var job = new FlattenImageInternalJob();
            var combineJob = new FlattenImageInternalJob();

            job.buffers = new NativeArray<IntPtr>(buffers.ToArray(), Allocator.TempJob);
            for (int i = 0; i < buffers.Count; ++i)
                job.buffers[i] = buffers[i];

            combineJob.width = job.width = width;
            combineJob.height = job.height = height;

            job.layersPerJob = layersPerJob;
            job.flipY = false;
            combineJob.flipY = true;

            int jobCount = buffers.Count / layersPerJob + (buffers.Count % layersPerJob > 0 ? 1 : 0);
            combineJob.layersPerJob = jobCount;

            NativeArray<byte>[] premergedBuffer = new NativeArray<byte>[jobCount];
            job.output = new NativeArray<IntPtr>(jobCount, Allocator.TempJob);
            combineJob.buffers = new NativeArray<IntPtr>(jobCount, Allocator.TempJob);

            for (int i = 0; i < jobCount; ++i)
            {
                premergedBuffer[i] = new NativeArray<byte>(width * height * 4, Allocator.TempJob);
                job.output[i] = new IntPtr(premergedBuffer[i].GetUnsafePtr());
                combineJob.buffers[i] = new IntPtr(premergedBuffer[i].GetUnsafeReadOnlyPtr());
            }
            combineJob.output = new NativeArray<IntPtr>(new[] {new IntPtr(output.GetUnsafePtr()), }, Allocator.TempJob);

            var handle = job.Schedule(jobCount, 1);
            combineJob.Schedule(1, 1, handle).Complete();
            foreach (var b in premergedBuffer)
            {
                if (b.IsCreated)
                    b.Dispose();
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        static unsafe void GetBuffersToMergeFromLayer(BitmapLayer layer, bool importHiddenLayer, List<IntPtr> buffers)
        {
            if (!layer.Visible && importHiddenLayer == false)
                return;
            if (layer.IsGroup)
            {
                for (int i = layer.ChildLayer.Count - 1; i >= 0; --i)
                    GetBuffersToMergeFromLayer(layer.ChildLayer[i], importHiddenLayer, buffers);
            }
            if (layer.Surface != null)
                buffers.Add(new IntPtr(layer.Surface.color.GetUnsafeReadOnlyPtr()));
            else
                Debug.LogWarning(string.Format("Layer {0} has no color buffer", layer.Name));
        }

        struct FlattenImageInternalJob : IJobParallelFor
        {
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<IntPtr> buffers;
            [ReadOnly]
            public int layersPerJob;
            [ReadOnly]
            public int width;
            [ReadOnly]
            public int height;
            [ReadOnly]
            public bool flipY;
            [DeallocateOnJobCompletion]
            public NativeArray<IntPtr> output;

            public unsafe void Execute(int index)
            {
                var premerge = (Color32*)output[index].ToPointer();
                for (int layerIndex = index * layersPerJob; layerIndex < (index * layersPerJob) + layersPerJob; ++layerIndex)
                {
                    if (buffers.Length <= layerIndex)
                        break;
                    var buffer = (Color32*)buffers[layerIndex].ToPointer();
                    for (int i = 0; i < height; ++i)
                    {
                        int sourceYIndex = i * width;
                        int destYIndex = flipY ? (height - 1 - i) * width : sourceYIndex;
                        for (int j = 0; j < width; ++j)
                        {
                            int sourceIndex = sourceYIndex + j;
                            int destIndex = destYIndex + j;
                            Color sourceColor = buffer[sourceIndex];
                            Color destColor = premerge[destIndex];
                            Color finalColor = new Color();

                            var destAlpha = destColor.a * (1 - sourceColor.a);
                            finalColor.a = sourceColor.a + destColor.a * (1 - sourceColor.a);
                            var premultiplyAlpha = 1 / finalColor.a;
                            finalColor.r = (sourceColor.r * sourceColor.a + destColor.r * destAlpha) * premultiplyAlpha;
                            finalColor.g = (sourceColor.g * sourceColor.a + destColor.g * destAlpha) * premultiplyAlpha;
                            finalColor.b = (sourceColor.b * sourceColor.a + destColor.b * destAlpha) * premultiplyAlpha;

                            premerge[destIndex] = finalColor;
                        }
                    }
                }
            }
        }
    }
}
