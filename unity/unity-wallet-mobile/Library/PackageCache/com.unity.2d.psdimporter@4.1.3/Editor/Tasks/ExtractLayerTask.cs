using System;
using System.Collections.Generic;
using PDNWrapper;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEditor.U2D.PSD
{
    class ExtractLayerTask
    {
        struct ConvertBufferJob : IJobParallelFor
        {
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<IntPtr> original;
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<int> width;
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<int> height;

            [DeallocateOnJobCompletion]
            public NativeArray<IntPtr> output;
            public unsafe void Execute(int index)
            {
                Color32* originalColor = (Color32*)original[index];
                Color32* otuputColor = (Color32*)output[index];
                for (int i = 0; i < height[index]; ++i)
                {
                    int originalYOffset = i * width[index];
                    int outputYOffset = (height[index] - i - 1) * width[index];
                    for (int j = 0; j < width[index]; ++j)
                    {
                        otuputColor[j + outputYOffset] = originalColor[j + originalYOffset];
                    }
                }
            }
        }

        public static unsafe void Execute(List<PSDLayer> extractedLayer, List<BitmapLayer> layers, bool importHiddenLayer)
        {
            UnityEngine.Profiling.Profiler.BeginSample("ExtractLayer_PrepareJob");
            var tempExtractLayer = new List<PSDLayer>();
            int layerWithBuffer = ExtractLayer(tempExtractLayer, layers, importHiddenLayer);
            if (layerWithBuffer == 0)
                return;
            var job = new ConvertBufferJob();
            job.original = new NativeArray<IntPtr>(layerWithBuffer, Allocator.TempJob);
            job.output = new NativeArray<IntPtr>(layerWithBuffer, Allocator.TempJob);
            job.width = new NativeArray<int>(layerWithBuffer, Allocator.TempJob);
            job.height = new NativeArray<int>(layerWithBuffer, Allocator.TempJob);
            for (int i = 0, jobIndex = 0; i < tempExtractLayer.Count; ++i)
            {
                var el = tempExtractLayer[i];
                if (el.texture.Length == 0 || el.width == 0 || el.height == 0)
                {
                    extractedLayer.Add(el);
                    continue;
                }

                job.original[jobIndex] = new IntPtr(el.texture.GetUnsafeReadOnlyPtr());
                el.texture = new NativeArray<Color32>(el.texture.Length, Allocator.Persistent);
                extractedLayer.Add(el);
                job.output[jobIndex] = new IntPtr(el.texture.GetUnsafePtr());
                job.width[jobIndex] = el.width;
                job.height[jobIndex] = el.height;
                ++jobIndex;
            }

            var jobsPerThread = layerWithBuffer / (SystemInfo.processorCount == 0 ? 8 : SystemInfo.processorCount);
            jobsPerThread = Mathf.Max(jobsPerThread, 1);
            var handle = job.Schedule(layerWithBuffer, jobsPerThread);
            UnityEngine.Profiling.Profiler.EndSample();
            handle.Complete();
        }

        static int ExtractLayer(List<PSDLayer> extractedLayer, List<BitmapLayer> layers, bool importHiddenLayer)
        {
            // parent is the previous element in extracedLayer
            int parentGroupIndex = extractedLayer.Count - 1;
            int actualLayerWithBuffer = 0;
            foreach (var l in layers)
            {
                if (!importHiddenLayer && !l.Visible)
                    continue;
                if (l.IsGroup)
                {
                    extractedLayer.Add(new PSDLayer(new NativeArray<Color32>(0, Allocator.Persistent), parentGroupIndex, l.IsGroup, l.Name, 0, 0, l.LayerID));
                    actualLayerWithBuffer += ExtractLayer(extractedLayer, l.ChildLayer, importHiddenLayer);
                }
                else
                {
                    extractedLayer.Add(new PSDLayer(l.Surface.color, parentGroupIndex, l.IsGroup, l.Name, l.Surface.width, l.Surface.height, l.LayerID));
                    ++actualLayerWithBuffer;
                }
            }
            return actualLayerWithBuffer;
        }
    }
}
