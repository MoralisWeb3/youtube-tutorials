#if ENABLE_ANIMATION_COLLECTION
using System;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.U2D.Common;
using UnityEngine.Profiling;
using Unity.Burst;
using UnityEngine.Assertions;

namespace UnityEngine.U2D.Animation
{
    internal struct PerSkinJobData
    {
        public int deformVerticesStartPos;
        public int2 bindPosesIndex;
        public int2 verticesIndex;
    }

    internal struct SpriteSkinData
    {
        public NativeCustomSlice<Vector3> vertices;
        public NativeCustomSlice<BoneWeight> boneWeights;
        public NativeCustomSlice<Matrix4x4> bindPoses;
        public NativeCustomSlice<Vector4> tangents;
        public bool hasTangents;
        public int spriteVertexStreamSize;
        public int spriteVertexCount;
        public int tangentVertexOffset;
        public int deformVerticesStartPos;
        public int transformId;
        public NativeCustomSlice<int> boneTransformId;
    }

#if ENABLE_ANIMATION_BURST
    [BurstCompile]
#endif 
    internal struct PrepareDeformJob :IJob
    {
        [ReadOnly]
        public NativeArray<PerSkinJobData> perSkinJobData;
        [ReadOnly]
        public int batchDataSize;
        [WriteOnly]
        public NativeArray<int2> boneLookupData;
        [WriteOnly]
        public NativeArray<int2> vertexLookupData;

        public void Execute()
        {
            for (int i = 0; i < batchDataSize; ++i)
            {
                var jobData = perSkinJobData[i];
                for (int k = 0, j = jobData.bindPosesIndex.x; j < jobData.bindPosesIndex.y; ++j, ++k)
                {
                    boneLookupData[j] = new int2(i, k);
                }
                for (int k = 0, j = jobData.verticesIndex.x; j < jobData.verticesIndex.y; ++j, ++k)
                {
                    vertexLookupData[j] = new int2(i, k);
                }
            }
        }
    }

#if ENABLE_ANIMATION_BURST
    [BurstCompile]
#endif 
    internal struct BoneDeformBatchedJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float4x4> boneTransform;
        [ReadOnly]
        public NativeArray<float4x4> rootTransform;
        [ReadOnly]
        public NativeArray<int2> boneLookupData;
        [ReadOnly]
        public NativeArray<SpriteSkinData> spriteSkinData;
        [ReadOnly]
        public NativeHashMap<int, TransformAccessJob.TransformData> rootTransformIndex;
        [ReadOnly]
        public NativeHashMap<int, TransformAccessJob.TransformData> boneTransformIndex;
        [WriteOnly]
        public NativeArray<float4x4> finalBoneTransforms;

        public void Execute(int i)
        {
            int x = boneLookupData[i].x;
            int y = boneLookupData[i].y;
            var ssd = spriteSkinData[x];
            var v = ssd.boneTransformId[y];
            var index = boneTransformIndex[v].transformIndex;
            if (index < 0)
                return;
            var aa = boneTransform[index];
            var bb = ssd.bindPoses[y];
            var cc = rootTransformIndex[ssd.transformId].transformIndex;
            finalBoneTransforms[i] = math.mul(rootTransform[cc], math.mul(aa, bb));
        }
    }

#if ENABLE_ANIMATION_BURST
    [BurstCompile]
#endif 

    internal struct SkinDeformBatchedJob : IJobParallelFor
    {
        public NativeSlice<byte> vertices;

        [ReadOnly]
        public NativeArray<float4x4> finalBoneTransforms;
        [ReadOnly]
        public NativeArray<PerSkinJobData> perSkinJobData;
        [ReadOnly]
        public NativeArray<SpriteSkinData> spriteSkinData;
        [ReadOnly]
        public NativeArray<int2> vertexLookupData;

        public unsafe void Execute(int i)
        {
            int j = vertexLookupData[i].x;
            int k = vertexLookupData[i].y;

            PerSkinJobData perSkinData = perSkinJobData[j];
            var spriteSkin = spriteSkinData[j];
            float3 srcVertex = spriteSkin.vertices[k];
            float4 tangents = spriteSkin.tangents[k];
            var influence = spriteSkin.boneWeights[k];

            int bone0 = influence.boneIndex0 + perSkinData.bindPosesIndex.x;
            int bone1 = influence.boneIndex1 + perSkinData.bindPosesIndex.x;
            int bone2 = influence.boneIndex2 + perSkinData.bindPosesIndex.x;
            int bone3 = influence.boneIndex3 + perSkinData.bindPosesIndex.x;

            byte* deformedPosOffset = (byte*)vertices.GetUnsafePtr();
            byte* deformedPosStart = deformedPosOffset + spriteSkin.deformVerticesStartPos;
            NativeSlice<float3> deformableVerticesFloat3 = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float3>(deformedPosStart, spriteSkin.spriteVertexStreamSize, spriteSkin.spriteVertexCount);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref deformableVerticesFloat3, NativeSliceUnsafeUtility.GetAtomicSafetyHandle(vertices));
#endif
            if (spriteSkin.hasTangents)
            {
                byte* deformedTanOffset = deformedPosStart + spriteSkin.tangentVertexOffset;
                var deformableTangentsFloat4 = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float4>(deformedTanOffset, spriteSkin.spriteVertexStreamSize, spriteSkin.spriteVertexCount);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref deformableTangentsFloat4, NativeSliceUnsafeUtility.GetAtomicSafetyHandle(vertices));
#endif
                var tangent = new float4(tangents.xyz, 0.0f);
                tangent =
                    math.mul(finalBoneTransforms[bone0], tangent) * influence.weight0 +
                    math.mul(finalBoneTransforms[bone1], tangent) * influence.weight1 +
                    math.mul(finalBoneTransforms[bone2], tangent) * influence.weight2 +
                    math.mul(finalBoneTransforms[bone3], tangent) * influence.weight3;
                deformableTangentsFloat4[k] = new float4(math.normalize(tangent.xyz), tangents.w);
            }
            
            deformableVerticesFloat3[k] =
                math.transform(finalBoneTransforms[bone0], srcVertex) * influence.weight0 +
                math.transform(finalBoneTransforms[bone1], srcVertex) * influence.weight1 +
                math.transform(finalBoneTransforms[bone2], srcVertex) * influence.weight2 +
                math.transform(finalBoneTransforms[bone3], srcVertex) * influence.weight3;
        }
    }

#if ENABLE_ANIMATION_BURST
    [BurstCompile]
#endif 
    internal struct CalculateSpriteSkinAABBJob : IJobParallelFor
    {
        public NativeSlice<byte> vertices;
        [ReadOnly] 
        public NativeArray<bool> isSpriteSkinValidForDeformArray;
        [ReadOnly]
        public NativeArray<SpriteSkinData> spriteSkinData;

        [WriteOnly]
        public NativeArray<Bounds> bounds;

        public unsafe void Execute(int i)
        {
            if (!isSpriteSkinValidForDeformArray[i])
                return;

            var spriteSkin = spriteSkinData[i];
            byte* deformedPosOffset = (byte*)vertices.GetUnsafePtr();
            NativeSlice<float3> deformableVerticesFloat3 = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float3>(deformedPosOffset + spriteSkin.deformVerticesStartPos, spriteSkin.spriteVertexStreamSize, spriteSkin.spriteVertexCount);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref deformableVerticesFloat3, NativeSliceUnsafeUtility.GetAtomicSafetyHandle(vertices));
#endif

            bounds[i] = SpriteSkinUtility.CalculateSpriteSkinBounds(deformableVerticesFloat3);
        }
    }

#if ENABLE_ANIMATION_BURST
    [BurstCompile]
#endif
    internal struct FillPerSkinJobSingleThread : IJob
    {
        public PerSkinJobData combinedSkinBatch;

        [ReadOnly]
        public NativeArray<bool> isSpriteSkinValidForDeformArray;

        public NativeArray<SpriteSkinData> spriteSkinDataArray;
        public NativeArray<PerSkinJobData> perSkinJobDataArray;

        public NativeArray<PerSkinJobData> combinedSkinBatchArray;

        public void Execute()
        {
            var startIndex = 0;
            var endIndex = spriteSkinDataArray.Length;
            for (int index = startIndex; index < endIndex; ++index)
            {
                var spriteSkinData = spriteSkinDataArray[index];
                spriteSkinData.deformVerticesStartPos = -1;
                var vertexBufferSize = 0;
                var vertexCount = 0;
                var bindPoseCount = 0;
                if (isSpriteSkinValidForDeformArray[index])
                {
                    spriteSkinData.deformVerticesStartPos = combinedSkinBatch.deformVerticesStartPos;
                    vertexBufferSize = spriteSkinData.spriteVertexCount * spriteSkinData.spriteVertexStreamSize;
                    vertexCount = spriteSkinData.spriteVertexCount;
                    bindPoseCount = spriteSkinData.bindPoses.Length;
                }
                combinedSkinBatch.verticesIndex.x = combinedSkinBatch.verticesIndex.y;
                combinedSkinBatch.verticesIndex.y = combinedSkinBatch.verticesIndex.x + vertexCount;
                combinedSkinBatch.bindPosesIndex.x = combinedSkinBatch.bindPosesIndex.y;
                combinedSkinBatch.bindPosesIndex.y = combinedSkinBatch.bindPosesIndex.x + bindPoseCount;
                spriteSkinDataArray[index] = spriteSkinData;
                perSkinJobDataArray[index] = combinedSkinBatch;
                combinedSkinBatch.deformVerticesStartPos += vertexBufferSize;
            }
            combinedSkinBatchArray[0] = combinedSkinBatch;
        }
    }

#if ENABLE_ANIMATION_BURST
    [BurstCompile]
#endif 
    internal struct CopySpriteRendererBuffersJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<bool> isSpriteSkinValidForDeformArray;
        [ReadOnly]
        public NativeArray<SpriteSkinData> spriteSkinData;
        [ReadOnly, NativeDisableUnsafePtrRestriction]
        public IntPtr ptrVertices;

        [WriteOnly]
        public NativeArray<IntPtr> buffers;
        [WriteOnly]
        public NativeArray<int> bufferSizes;
        
        public void Execute(int i)
        {
            var skinData = spriteSkinData[i];
            IntPtr startVertices = default(IntPtr);
            var vertexBufferLength = 0;
            if (isSpriteSkinValidForDeformArray[i])
            {
                startVertices = ptrVertices + skinData.deformVerticesStartPos;
                vertexBufferLength = skinData.spriteVertexCount * skinData.spriteVertexStreamSize;
            }
            buffers[i] = startVertices;
            bufferSizes[i] = vertexBufferLength;
        }
    }    

    internal class SpriteSkinComposite : ScriptableObject
    {
        static SpriteSkinComposite m_Instance;

        public static SpriteSkinComposite instance
        {
            get
            {
                if (m_Instance == null)
                {
                    var composite = Resources.FindObjectsOfTypeAll<SpriteSkinComposite>();
                    if (composite.Length > 0)
                        m_Instance = composite[0];
                    else
                        m_Instance = ScriptableObject.CreateInstance<SpriteSkinComposite>();
                    m_Instance.hideFlags = HideFlags.HideAndDontSave;
                    m_Instance.Init();
                }
                return m_Instance;
            }
        }

        List<SpriteSkin> m_SpriteSkins = new List<SpriteSkin>();
        List<SpriteSkin> m_SpriteSkinLateUpdate = new List<SpriteSkin>();
        SpriteRenderer[] m_SpriteRenderers = new SpriteRenderer[0];

        NativeByteArray m_DeformedVerticesBuffer;
        NativeArray<float4x4> m_FinalBoneTransforms;

        NativeArray<bool> m_IsSpriteSkinActiveForDeform;
        NativeArray<SpriteSkinData> m_SpriteSkinData;
        NativeArray<PerSkinJobData> m_PerSkinJobData;
        NativeArray<Bounds> m_BoundsData;
        NativeArray<IntPtr> m_Buffers;
        NativeArray<int> m_BufferSizes;

        NativeArray<int2> m_BoneLookupData;
        NativeArray<int2> m_VertexLookupData;
        NativeArray<PerSkinJobData> m_SkinBatchArray;
        TransformAccessJob m_LocalToWorldTransformAccessJob;
        TransformAccessJob m_WorldToLocalTransformAccessJob;
        JobHandle m_BoundJobHandle;
        JobHandle m_DeformJobHandle;
        JobHandle m_CopyJobHandle;

        [SerializeField]
        GameObject m_Helper;

        internal GameObject helperGameObject
        {
            get => m_Helper;
        }

        internal void RemoveTransformById(int transformId)
        {
            m_LocalToWorldTransformAccessJob.RemoveTransformById(transformId);
        }

        internal void AddSpriteSkinBoneTransform(SpriteSkin spriteSkin)
        {
            if (spriteSkin == null)
                return;
            if (spriteSkin.boneTransforms != null)
            {
                foreach (var t in spriteSkin.boneTransforms)
                {
                    if(t != null)
                        m_LocalToWorldTransformAccessJob.AddTransform(t);
                }
            }
        }

        internal void AddSpriteSkinRootBoneTransform(SpriteSkin spriteSkin)
        {
            if (spriteSkin == null || spriteSkin.rootBone == null)
                return;
            m_LocalToWorldTransformAccessJob.AddTransform(spriteSkin.rootBone);
        }

        internal void AddSpriteSkin(SpriteSkin spriteSkin)
        {
            if (spriteSkin == null)
                return;

            bool added = m_SpriteSkins.Contains(spriteSkin);
            Debug.Assert(!added, string.Format("SpriteSkin {0} is already added", spriteSkin.gameObject.name));
            if (!added)
            {
                m_SpriteSkins.Add(spriteSkin);
                var count = m_SpriteSkins.Count;
                Array.Resize(ref m_SpriteRenderers, count);
                m_SpriteRenderers[count - 1] = spriteSkin.spriteRenderer;
                m_WorldToLocalTransformAccessJob.AddTransform(spriteSkin.transform);
                if (m_IsSpriteSkinActiveForDeform.IsCreated)
                {
                    NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_IsSpriteSkinActiveForDeform, count);
                    NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_PerSkinJobData, count);
                    NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_SpriteSkinData, count);
                    NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_BoundsData, count);
                    NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_Buffers, count);
                    NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_BufferSizes, count);
                    CopyToSpriteSkinData(count - 1);
                }
            }
        }

        internal void CopyToSpriteSkinData(SpriteSkin spriteSkin)
        {
            if (spriteSkin == null)
                return;
            int index = m_SpriteSkins.IndexOf(spriteSkin);
            if (index < 0)
                return;
            CopyToSpriteSkinData(index);
        }

        private void CopyToSpriteSkinData(int index)
        {
            if (index < 0 || index >= m_SpriteSkins.Count || !m_SpriteSkinData.IsCreated)
                return;

            SpriteSkinData spriteSkinData = default(SpriteSkinData);
            var spriteSkin = m_SpriteSkins[index];
            spriteSkin.CopyToSpriteSkinData(ref spriteSkinData, index);
            m_SpriteSkinData[index] = spriteSkinData;
            m_SpriteRenderers[index] = spriteSkin.spriteRenderer;
        }

        internal void RemoveSpriteSkin(SpriteSkin spriteSkin)
        {
            int index = m_SpriteSkins.IndexOf(spriteSkin);
            if (index < 0)
                return;

            // Check if it is not the last SpriteSkin
            if (index < m_SpriteSkins.Count - 1)
            {
                m_SpriteSkins.RemoveAtSwapBack(index);
                CopyToSpriteSkinData(index);
            }
            else
            {
                m_SpriteSkins.RemoveAt(index);
            }

            var count = m_SpriteSkins.Count;
            Array.Resize(ref m_SpriteRenderers, count);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_IsSpriteSkinActiveForDeform, count);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_PerSkinJobData, count);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_SpriteSkinData, count);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_BoundsData, count);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_Buffers, count);
            NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_BufferSizes, count);

            m_WorldToLocalTransformAccessJob.RemoveTransform(spriteSkin.transform);
        }

        internal void AddSpriteSkinForLateUpdate(SpriteSkin spriteSkin)
        {
            if (spriteSkin == null)
                return;

            bool added = m_SpriteSkinLateUpdate.Contains(spriteSkin);
            Debug.Assert( !added, string.Format("SpriteSkin {0} is already added for LateUpdate", spriteSkin.gameObject.name));
            if (!added)
            {
                m_SpriteSkinLateUpdate.Add(spriteSkin);
            }
        }

        internal void RemoveSpriteSkinForLateUpdate(SpriteSkin spriteSkin)
        {
            m_SpriteSkinLateUpdate.Remove(spriteSkin);
        }

        void Init()
        {
            if(m_LocalToWorldTransformAccessJob == null)
                m_LocalToWorldTransformAccessJob = new TransformAccessJob();
            if(m_WorldToLocalTransformAccessJob == null)
                m_WorldToLocalTransformAccessJob = new TransformAccessJob();
        }

        internal void ResetComposite()
        {
            foreach (var spriteSkin in m_SpriteSkins)
                spriteSkin.batchSkinning = false;
            m_SpriteSkins.Clear();
            m_LocalToWorldTransformAccessJob.Destroy();
            m_WorldToLocalTransformAccessJob.Destroy();
            m_LocalToWorldTransformAccessJob = new TransformAccessJob();
            m_WorldToLocalTransformAccessJob = new TransformAccessJob();
        }

        public void OnEnable()
        {
            m_Instance = this;
            if (m_Helper == null)
            {
                m_Helper = new GameObject("SpriteSkinManager");
                m_Helper.hideFlags = HideFlags.HideAndDontSave;
                m_Helper.AddComponent<SpriteSkinManager.SpriteSkinManagerInternal>();
#if !UNITY_EDITOR
                GameObject.DontDestroyOnLoad(m_Helper);
#endif
            }
            
            m_FinalBoneTransforms = new NativeArray<float4x4>(1, Allocator.Persistent);
            m_BoneLookupData = new NativeArray<int2>(1, Allocator.Persistent);
            m_VertexLookupData = new NativeArray<int2>(1, Allocator.Persistent);
            m_SkinBatchArray = new NativeArray<PerSkinJobData>(1, Allocator.Persistent);

            Init();

            // Initialise all existing SpriteSkins as execution order is indeterminate
            int count = m_SpriteSkins.Count; 
            if (count > 0)
            {
                m_IsSpriteSkinActiveForDeform = new NativeArray<bool>(count, Allocator.Persistent);
                m_PerSkinJobData = new NativeArray<PerSkinJobData>(count, Allocator.Persistent);
                m_SpriteSkinData = new NativeArray<SpriteSkinData>(count, Allocator.Persistent);
                m_BoundsData = new NativeArray<Bounds>(count, Allocator.Persistent);
                m_Buffers = new NativeArray<IntPtr>(count, Allocator.Persistent);
                m_BufferSizes = new NativeArray<int>(count, Allocator.Persistent);
                for (int i = 0; i < count; ++i)
                {
                    var spriteSkin = m_SpriteSkins[i];
                    spriteSkin.batchSkinning = true;
                    CopyToSpriteSkinData(i);
                }
            }
            else
            {
                m_IsSpriteSkinActiveForDeform = new NativeArray<bool>(1, Allocator.Persistent);
                m_PerSkinJobData = new NativeArray<PerSkinJobData>(1, Allocator.Persistent);
                m_SpriteSkinData = new NativeArray<SpriteSkinData>(1, Allocator.Persistent);
                m_BoundsData = new NativeArray<Bounds>(1, Allocator.Persistent);
                m_Buffers = new NativeArray<IntPtr>(1, Allocator.Persistent);
                m_BufferSizes = new NativeArray<int>(1, Allocator.Persistent);
            }
        }

        private void OnDisable()
        {
            m_DeformJobHandle.Complete();
            m_BoundJobHandle.Complete();
            m_CopyJobHandle.Complete();
            foreach (var spriteSkin in m_SpriteSkins)
                spriteSkin.batchSkinning = false;
            m_SpriteSkins.Clear();
            m_SpriteRenderers = new SpriteRenderer[0];
            BufferManager.instance.ReturnBuffer(GetInstanceID());
            m_IsSpriteSkinActiveForDeform.DisposeIfCreated();
            m_PerSkinJobData.DisposeIfCreated();
            m_SpriteSkinData.DisposeIfCreated();
            m_Buffers.DisposeIfCreated();
            m_BufferSizes.DisposeIfCreated();
            m_BoneLookupData.DisposeIfCreated();
            m_VertexLookupData.DisposeIfCreated();
            m_SkinBatchArray.DisposeIfCreated();
            m_FinalBoneTransforms.DisposeIfCreated();
            m_BoundsData.DisposeIfCreated();
            if (m_Helper != null)
                GameObject.DestroyImmediate(m_Helper);
            m_LocalToWorldTransformAccessJob.Destroy();
            m_WorldToLocalTransformAccessJob.Destroy();
        }

        internal unsafe void LateUpdate()
        {
            foreach (var ss in m_SpriteSkinLateUpdate)
            {
                if(ss != null)
                    ss.OnLateUpdate();
            }

            var count = m_SpriteSkins.Count;
            if (count == 0)
                return;

            Profiler.BeginSample("SpriteSkinComposite.PrepareData");
            Assert.AreEqual(m_IsSpriteSkinActiveForDeform.Length, count);
            Assert.AreEqual(m_PerSkinJobData.Length, count);
            Assert.AreEqual(m_SpriteSkinData.Length, count);
            Assert.AreEqual(m_BoundsData.Length, count);
            Assert.AreEqual(m_Buffers.Length, count);
            Assert.AreEqual(m_BufferSizes.Length, count);
            Assert.AreEqual(m_SpriteRenderers.Length, count);

            Profiler.BeginSample("SpriteSkinComposite.ValidateSpriteSkinData");
            for (int i = 0; i < m_SpriteSkins.Count; ++i)
            {
                var spriteSkin = m_SpriteSkins[i];
                m_IsSpriteSkinActiveForDeform[i] = spriteSkin.BatchValidate();
                if (m_IsSpriteSkinActiveForDeform[i] && spriteSkin.NeedUpdateCompositeCache())
                {
                    CopyToSpriteSkinData(i);
                }
            }
            Profiler.EndSample();

            Profiler.BeginSample("SpriteSkinComposite.TransformAccessJob");
            var localToWorldJobHandle = m_LocalToWorldTransformAccessJob.StartLocalToWorldJob();
            var worldToLocalJobHandle = m_WorldToLocalTransformAccessJob.StartWorldToLocalJob();
            Profiler.EndSample();

            Profiler.BeginSample("SpriteSkinComposite.GetSpriteSkinBatchData");
            NativeArrayHelpers.ResizeIfNeeded(ref m_SkinBatchArray, 1);
            FillPerSkinJobSingleThread fillPerSkinJobSingleThread = new FillPerSkinJobSingleThread()
            {
                isSpriteSkinValidForDeformArray = m_IsSpriteSkinActiveForDeform,
                combinedSkinBatchArray = m_SkinBatchArray,
                spriteSkinDataArray = m_SpriteSkinData,
                perSkinJobDataArray = m_PerSkinJobData,
            };
            fillPerSkinJobSingleThread.Run();
            Profiler.EndSample();
            Profiler.EndSample();

            var skinBatch = m_SkinBatchArray[0];
            int batchCount = m_SpriteSkinData.Length;
            int vertexBufferSize = skinBatch.deformVerticesStartPos;
            if (vertexBufferSize <= 0)
            {
                localToWorldJobHandle.Complete();
                worldToLocalJobHandle.Complete();
                return;
            }

            Profiler.BeginSample("SpriteSkinComposite.ResizeBuffers");
            
            m_DeformedVerticesBuffer = BufferManager.instance.GetBuffer(GetInstanceID(), vertexBufferSize);
            NativeArrayHelpers.ResizeIfNeeded(ref m_FinalBoneTransforms, skinBatch.bindPosesIndex.y);
            NativeArrayHelpers.ResizeIfNeeded(ref m_BoneLookupData, skinBatch.bindPosesIndex.y);
            NativeArrayHelpers.ResizeIfNeeded(ref m_VertexLookupData, skinBatch.verticesIndex.y);
            Profiler.EndSample();

            Profiler.BeginSample("SpriteSkinComposite.Prepare");
            PrepareDeformJob prepareJob = new PrepareDeformJob
            {
                batchDataSize = batchCount,
                perSkinJobData = m_PerSkinJobData,
                boneLookupData = m_BoneLookupData,
                vertexLookupData = m_VertexLookupData
            };
            var jobHandle = prepareJob.Schedule();
            Profiler.EndSample();

            Profiler.BeginSample("SpriteSkinComposite.ScheduleJobs");
            BoneDeformBatchedJob boneJobBatched = new BoneDeformBatchedJob()
            {
                boneTransform = m_LocalToWorldTransformAccessJob.transformMatrix,
                rootTransform = m_WorldToLocalTransformAccessJob.transformMatrix,
                spriteSkinData = m_SpriteSkinData,
                boneLookupData = m_BoneLookupData,
                finalBoneTransforms = m_FinalBoneTransforms,
                rootTransformIndex = m_WorldToLocalTransformAccessJob.transformData,
                boneTransformIndex = m_LocalToWorldTransformAccessJob.transformData
            };
            jobHandle = JobHandle.CombineDependencies(localToWorldJobHandle, worldToLocalJobHandle, jobHandle);
            jobHandle = boneJobBatched.Schedule(skinBatch.bindPosesIndex.y, 8, jobHandle);

            SkinDeformBatchedJob skinJobBatched = new SkinDeformBatchedJob()
            {
                vertices = m_DeformedVerticesBuffer.array,
                vertexLookupData = m_VertexLookupData,
                spriteSkinData = m_SpriteSkinData,
                perSkinJobData = m_PerSkinJobData,
                finalBoneTransforms = m_FinalBoneTransforms,
            };
            m_DeformJobHandle = skinJobBatched.Schedule(skinBatch.verticesIndex.y, 16, jobHandle);

            CopySpriteRendererBuffersJob copySpriteRendererBuffersJob = new CopySpriteRendererBuffersJob()
            {
                isSpriteSkinValidForDeformArray = m_IsSpriteSkinActiveForDeform,
                spriteSkinData = m_SpriteSkinData,
                ptrVertices = (IntPtr) NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(m_DeformedVerticesBuffer.array),
                buffers = m_Buffers,
                bufferSizes = m_BufferSizes,
            };
            m_CopyJobHandle = copySpriteRendererBuffersJob.Schedule(batchCount, 16, jobHandle);

            CalculateSpriteSkinAABBJob updateBoundJob = new CalculateSpriteSkinAABBJob
            {
                vertices = m_DeformedVerticesBuffer.array,
                isSpriteSkinValidForDeformArray = m_IsSpriteSkinActiveForDeform,
                spriteSkinData = m_SpriteSkinData,
                bounds = m_BoundsData,
            };
            m_BoundJobHandle = updateBoundJob.Schedule(batchCount, 4, m_DeformJobHandle);
            Profiler.EndSample();

            JobHandle.ScheduleBatchedJobs();
            jobHandle = JobHandle.CombineDependencies(m_BoundJobHandle, m_CopyJobHandle);
            jobHandle.Complete();

            Profiler.BeginSample("SpriteSkinComposite.SetBatchDeformableBufferAndLocalAABB");
            InternalEngineBridge.SetBatchDeformableBufferAndLocalAABBArray(m_SpriteRenderers, m_Buffers, m_BufferSizes, m_BoundsData);
            Profiler.EndSample();
        }

        internal bool HasDeformableBufferForSprite(int dataIndex)
        {
            if (dataIndex < 0 && m_IsSpriteSkinActiveForDeform.Length >= dataIndex)
                throw new InvalidOperationException("Invalid index for deformable buffer");
            return m_IsSpriteSkinActiveForDeform[dataIndex];
        }

        internal unsafe NativeArray<byte> GetDeformableBufferForSprite(int dataIndex)
        {
            if (dataIndex < 0 && m_SpriteSkinData.Length >= dataIndex)
                throw new InvalidOperationException("Invalid index for deformable buffer");
            
            if (!m_DeformJobHandle.IsCompleted)
                m_DeformJobHandle.Complete();

            var skinData = m_SpriteSkinData[dataIndex];
            if (skinData.deformVerticesStartPos < 0)
                throw new InvalidOperationException("There are no currently deformed vertices.");

            var vertexBufferLength = skinData.spriteVertexCount * skinData.spriteVertexStreamSize;
            byte* ptrVertices = (byte*)m_DeformedVerticesBuffer.array.GetUnsafeReadOnlyPtr();
            ptrVertices += skinData.deformVerticesStartPos; 
            var buffer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(ptrVertices, vertexBufferLength, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref buffer, NativeArrayUnsafeUtility.GetAtomicSafetyHandle(m_DeformedVerticesBuffer.array));
#endif
            return buffer;
        }

        // Code for tests
        #region Code For Tests
        internal string GetDebugLog()
        {
            var log = "";
            log = "====SpriteSkinLateUpdate===\n";
            log += "Count: " + m_SpriteSkinLateUpdate.Count +"\n";
            foreach (var ss in m_SpriteSkinLateUpdate)
            {
                log += ss == null ? "null" : ss.name;
                log += "\n";
            }
            log += "\n";

            log += "===SpriteSkinBatch===\n";
            log += "Count: " + m_SpriteSkins.Count +"\n";
            foreach (var ss in m_SpriteSkins)
            {
                log += ss == null ? "null" : ss.name;
                log += "\n";
            }

            log += "===LocalToWorldTransformAccessJob===\n";
            log += m_LocalToWorldTransformAccessJob.GetDebugLog();
            log += "\n";
            log += "===WorldToLocalTransformAccessJob===\n";
            log += "\n";
            log += m_WorldToLocalTransformAccessJob.GetDebugLog();
            return log;
        }

        internal SpriteSkin[] GetSpriteSkins()
        {
            return m_SpriteSkins.ToArray();
        }

        internal TransformAccessJob GetWorldToLocalTransformAccessJob()
        {
            return m_WorldToLocalTransformAccessJob;
        }

        internal TransformAccessJob GetLocalToWorldTransformAccessJob()
        {
            return m_LocalToWorldTransformAccessJob;
        }
        #endregion
    }
}
#endif