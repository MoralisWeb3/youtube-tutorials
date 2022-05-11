#if ENABLE_ANIMATION_COLLECTION && ENABLE_ANIMATION_BURST
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;
using UnityEngine.Profiling;
using Unity.Burst;

namespace UnityEngine.U2D.Animation
{
    internal class TransformAccessJob
    {
        internal struct TransformData
        {
            public int transformIndex;
            public int refCount;

            public TransformData(int index)
            {
                transformIndex = index;
                refCount = 1;
            }
        }

        Transform[] m_Transform;
        TransformAccessArray m_TransformAccessArray;
        NativeHashMap<int, TransformData> m_TransformData;
        NativeArray<float4x4> m_TransformMatrix;
        bool m_Dirty;
        JobHandle m_JobHandle;

        public TransformAccessJob()
        {
            m_TransformMatrix = new NativeArray<float4x4>(1, Allocator.Persistent);
            m_TransformData  = new NativeHashMap<int, TransformData>(1, Allocator.Persistent);
            m_Transform = new Transform[0];
            m_Dirty = false;
            m_JobHandle = default(JobHandle);
        }

        public void Destroy()
        {
            m_JobHandle.Complete();
            if (m_TransformMatrix.IsCreated)
                m_TransformMatrix.Dispose();
            if (m_TransformAccessArray.isCreated)
                m_TransformAccessArray.Dispose();
            if (m_TransformData.IsCreated)
                m_TransformData.Dispose();
        }

        public NativeHashMap<int, TransformData> transformData
        {
            get { return m_TransformData; }
        }

        public NativeArray<float4x4> transformMatrix
        {
            get { return m_TransformMatrix; }
        }

        public void AddTransform(Transform t)
        {
            if (t == null || !m_TransformData.IsCreated)
                return;
            m_JobHandle.Complete();
            int instanceId = t.GetInstanceID();
            if (m_TransformData.ContainsKey(instanceId))
            {
                var transformData = m_TransformData[instanceId];
                transformData.refCount += 1;
                m_TransformData[instanceId] = transformData;
            }
            else
            {
                m_TransformData.TryAdd(instanceId, new TransformData(-1));
                ArrayAdd(ref m_Transform, t);
                m_Dirty = true;
            }

        }

        static void  ArrayAdd<T>(ref T[] array, T item)
        {
            int arraySize = array.Length;
            Array.Resize(ref array, arraySize + 1);
            array[arraySize] = item;
        }

        static void ArrayRemove<T>(ref T[] array, T item)
        {
            List<T> newList = new List<T>(array);
            newList.Remove(item);
            array = newList.ToArray();
        }

        public static void ArrayRemoveAt<T>(ref T[] array, int index)
        {
            List<T> list = new List<T>(array);
            list.RemoveAt(index);
            array = list.ToArray();
        }

        public void RemoveTransform(Transform t)
        {
            if (t == null || !m_TransformData.IsCreated)
                return;
            m_JobHandle.Complete();
            int instanceId = t.GetInstanceID();
            if (m_TransformData.ContainsKey(instanceId))
            {
                var transformData = m_TransformData[instanceId];
                if (transformData.refCount == 1)
                {
                    m_TransformData.Remove(instanceId);
                    ArrayRemove(ref m_Transform, t);
                    m_Dirty = true;
                }
                else
                {
                    transformData.refCount -= 1;
                    m_TransformData[instanceId] = transformData;
                }
            }
        }

        void UpdateTransformIndex()
        {
            if (!m_Dirty)
                return;
            m_Dirty = false;
            Profiler.BeginSample("UpdateTransformIndex");
            NativeArrayHelpers.ResizeIfNeeded(ref m_TransformMatrix, m_Transform.Length);
            if (!m_TransformAccessArray.isCreated)
                TransformAccessArray.Allocate(m_Transform.Length, -1, out m_TransformAccessArray);
            else if (m_TransformAccessArray.capacity != m_Transform.Length)
                m_TransformAccessArray.capacity = m_Transform.Length;
            m_TransformAccessArray.SetTransforms(m_Transform);

            for (int i = 0; i < m_Transform.Length; ++i)
            {
                if (m_Transform[i] != null)
                {
                    var instanceId = m_Transform[i].GetInstanceID();
                    var transformData = m_TransformData[instanceId];
                    transformData.transformIndex = i;
                    m_TransformData[instanceId] = transformData;
                }
            }
            Profiler.EndSample();
        }

        public JobHandle StartLocalToWorldJob()
        {
            if (m_Transform.Length > 0)
            {
                m_JobHandle.Complete();
                UpdateTransformIndex();
                Profiler.BeginSample("StartLocalToWorldJob");
                var job = new LocalToWorldTransformAccessJob()
                {
                    outMatrix = transformMatrix,
                };
                m_JobHandle = job.Schedule(m_TransformAccessArray);
                Profiler.EndSample();
                return m_JobHandle;
            }
            
            return default(JobHandle);
        }

        public JobHandle StartWorldToLocalJob()
        {
            if (m_Transform.Length > 0)
            {
                m_JobHandle.Complete();
                UpdateTransformIndex();
                Profiler.BeginSample("StartWorldToLocalJob");
                var job = new WorldToLocalTransformAccessJob()
                {
                    outMatrix = transformMatrix,
                };
                m_JobHandle = job.Schedule(m_TransformAccessArray);
                Profiler.EndSample();
                return m_JobHandle;
            }

            return default(JobHandle);
        }

        internal string GetDebugLog()
        {
            var log = "";
            log += "TransformData Count: " + m_TransformData.Count() + "\n";
            log += "Transform Count: " + m_Transform.Length + "\n";
            foreach (var ss in m_Transform)
            {
                log += ss == null ? "null" : ss.name + " " + ss.GetInstanceID();
                log += "\n";
                if (ss != null)
                {
                    log += "RefCount: " + m_TransformData[ss.GetInstanceID()].refCount + "\n";
                }
                
                log += "\n";
            }

            return log;
        }

        internal void RemoveTransformById(int transformId)
        {
            if (!m_TransformData.IsCreated)
                return;
            m_JobHandle.Complete();
            if (m_TransformData.ContainsKey(transformId))
            {
                var transformData = m_TransformData[transformId];
                if (transformData.refCount == 1)
                {
                    m_TransformData.Remove(transformId);
                    var index = Array.FindIndex(m_Transform, t => t.GetInstanceID() == transformId);
                    if (index >= 0)
                    {
                        ArrayRemoveAt(ref m_Transform, index);
                    }
                    m_Dirty = true;
                }
                else
                {
                    transformData.refCount -= 1;
                    m_TransformData[transformId] = transformData;
                }
            }
        }
    }
    
    [BurstCompile]
    internal struct LocalToWorldTransformAccessJob : IJobParallelForTransform
    {
        [WriteOnly]
        public NativeArray<float4x4> outMatrix;
        public void Execute(int index, TransformAccess transform)
        {
            outMatrix[index] = transform.localToWorldMatrix;
        }
    }
    
    [BurstCompile]
    internal struct WorldToLocalTransformAccessJob : IJobParallelForTransform
    {
        [WriteOnly]
        public NativeArray<float4x4> outMatrix;
        
        public void Execute(int index, TransformAccess transform)
        {
            outMatrix[index] = transform.worldToLocalMatrix;
        }
    }
}
#endif