#if ENABLE_ENTITIES

using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace UnityEngine.U2D.Animation
{
    [Preserve]
    [UnityEngine.ExecuteAlways]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(PrepareSkinningSystem))]
    class DeformSpriteSystem : JobComponentSystem
    {
        List<SpriteComponent> m_UniqueSpriteComponents = new List<SpriteComponent>(16);
        EntityQuery m_ComponentGroup;

        protected override void OnCreateManager()
        {
            m_ComponentGroup = GetEntityQuery(typeof(WorldToLocal), typeof(SpriteComponent), typeof(Vertex), typeof(BoneTransform));
        }

        [BurstCompile]
        public struct SkinJob : IJobParallelFor
        {
            // these are readonly per sprite and shared
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<Entity> entities;
            [ReadOnly]
            public NativeSlice<float3> vertices;
            [ReadOnly]
            public NativeSlice<BoneWeight> boneWeights;
            [ReadOnly]
            public NativeSlice<float4x4> bindPoses;
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<WorldToLocal> localToWorldArray;

            // these are calculated per renderer and per instance
            [NativeDisableParallelForRestriction]
            public BufferFromEntity<BoneTransform> boneTransformArray;
            [NativeDisableParallelForRestriction]
            public BufferFromEntity<Vertex> deformedArray;

            public void Execute(int i)
            {
                var rootTransformId = localToWorldArray[i].Value;
                var boneTransforms = boneTransformArray[entities[i]].Reinterpret<float4x4>().AsNativeArray();
                var deformableVertices = deformedArray[entities[i]].Reinterpret<float3>().AsNativeArray();
                SpriteSkinUtility.Deform(rootTransformId, vertices, boneWeights, boneTransforms, bindPoses, deformableVertices);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_UniqueSpriteComponents.Clear();
            EntityManager.GetAllUniqueSharedComponentData(m_UniqueSpriteComponents);

            var spriteComponentCount = m_UniqueSpriteComponents.Count;
            var returnHandle = inputDeps;
            
            for (var i = 0; i < spriteComponentCount; i++)
            {
                var spriteComponent = m_UniqueSpriteComponents[i];
                var sprite = spriteComponent.Value;
                var entityCount = 0;

                if (sprite != null)
                {
                    m_ComponentGroup.SetFilter(spriteComponent);
                    var filteredEntities = m_ComponentGroup.ToEntityArray(Allocator.TempJob);

                    entityCount = filteredEntities.Length;
                    if (entityCount > 0)
                    {
                        var skinJob = new SkinJob
                        {
                            entities = filteredEntities,
                            vertices = sprite.GetVertexAttribute<Vector3>(UnityEngine.Rendering.VertexAttribute.Position).SliceWithStride<float3>(),
                            boneWeights = sprite.GetVertexAttribute<BoneWeight>(UnityEngine.Rendering.VertexAttribute.BlendWeight),
                            bindPoses = new NativeSlice<Matrix4x4>(sprite.GetBindPoses()).SliceWithStride<float4x4>(),
                            localToWorldArray = m_ComponentGroup.ToComponentDataArray<WorldToLocal>(Allocator.TempJob),
                            boneTransformArray = GetBufferFromEntity<BoneTransform>(),
                            deformedArray = GetBufferFromEntity<Vertex>()
                        };

                        returnHandle = skinJob.Schedule(entityCount, 4, returnHandle);
                    }
                    else
                        filteredEntities.Dispose();
                }
            }

            var system = World.GetOrCreateSystem<EndPresentationEntityCommandBufferSystem>();
            system.AddJobHandleForProducer(returnHandle);

            return returnHandle;
        }
    }

}

#endif