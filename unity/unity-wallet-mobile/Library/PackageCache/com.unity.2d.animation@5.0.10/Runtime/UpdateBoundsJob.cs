#if ENABLE_ANIMATION_COLLECTION
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
#if ENABLE_ANIMATION_BURST
using Unity.Burst;
#endif


namespace UnityEngine.U2D.Animation
{
#if ENABLE_ANIMATION_BURST
    [BurstCompile]
#endif
    internal struct UpdateBoundJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<int> rootTransformId;
        [ReadOnly]
        public NativeArray<int> rootBoneTransformId;
        [ReadOnly]
        public NativeArray<float4x4> rootTransform;
        [ReadOnly]
        public NativeArray<float4x4> boneTransform;
        [ReadOnly]
        public NativeHashMap<int, TransformAccessJob.TransformData> rootTransformIndex;
        [ReadOnly]
        public NativeHashMap<int, TransformAccessJob.TransformData> boneTransformIndex;
        [ReadOnly]
        public NativeArray<Bounds> spriteSkinBound;
        public NativeArray<Bounds> bounds;

        public void Execute(int i)
        {
            //for (int i = 0; i < rootTransformId.Length; ++i)
            {
                var unityBounds = spriteSkinBound[i];
                var rootIndex = rootTransformIndex[rootTransformId[i]].transformIndex;
                var rootBoneIndex = boneTransformIndex[rootBoneTransformId[i]].transformIndex;
                if (rootIndex < 0 || rootBoneIndex < 0)
                    return;
                var rootTransformMatrix = rootTransform[rootIndex];
                var rootBoneTransformMatrix = boneTransform[rootBoneIndex];
                var matrix = math.mul(rootTransformMatrix, rootBoneTransformMatrix);
                var center = new float4(unityBounds.center, 1);
                var extents = new float4(unityBounds.extents, 0);
                var p0 = math.mul(matrix, center + new float4(-extents.x, -extents.y, extents.z, extents.w));
                var p1 = math.mul(matrix, center + new float4(-extents.x, extents.y, extents.z, extents.w));
                var p2 = math.mul(matrix, center + extents);
                var p3 = math.mul(matrix, center + new float4(extents.x, -extents.y, extents.z, extents.w));
                var min = math.min(p0, math.min(p1, math.min(p2, p3)));
                var max = math.max(p0, math.max(p1, math.max(p2, p3)));
                extents = (max - min) * 0.5f;
                center = min + extents;
                bounds[i] = new Bounds()
                {
                    center = new Vector3(center.x, center.y, center.z),
                    extents = new Vector3(extents.x, extents.y, extents.z)
                };
            }

        }
    }
}
#endif