#if ENABLE_ENTITIES

using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;
using UnityEngine.U2D.Common;
using UnityEngine.Scripting;

namespace UnityEngine.U2D.Animation
{
    [Preserve]
    [UnityEngine.ExecuteAlways]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(DeformSpriteSystem))]
    internal class UpdateBoundsSystem : ComponentSystem
    {
        EntityQuery m_ComponentGroup;

        protected override void OnCreateManager()
        {
            m_ComponentGroup = GetEntityQuery(typeof(SpriteSkin));
        }

        struct Bounds
        {
            public float4 center;
            public float4 extents;
        }

        struct CalculateBoundsJob : IJobParallelFor
        {
            [ReadOnly, DeallocateOnJobCompletion]
            public NativeArray<float4x4> worldToLocalArray;
            [ReadOnly, DeallocateOnJobCompletion]
            public NativeArray<float4x4> rootLocalToWorldArray;
            public NativeArray<Bounds> boundsArray;
            public void Execute(int i)
            {
                var matrix = math.mul(worldToLocalArray[i], rootLocalToWorldArray[i]);
                var center = boundsArray[i].center;
                var extents = boundsArray[i].extents;
                var p0 = math.mul(matrix, center + new float4(-extents.x, -extents.y, extents.z, extents.w));
                var p1 = math.mul(matrix, center + new float4(-extents.x, extents.y, extents.z, extents.w));
                var p2 = math.mul(matrix, center + extents);
                var p3 = math.mul(matrix, center + new float4(extents.x, -extents.y, extents.z, extents.w));
                var min = math.min(p0, math.min(p1, math.min(p2, p3)));
                var max = math.max(p0, math.max(p1, math.max(p2, p3)));
                extents = (max - min) * 0.5f;
                center = min + extents;
                boundsArray[i] = new Bounds()
                {
                    center = center,
                    extents = extents
                };
            }
        }

        protected override void OnUpdate()
        {
            var entityLength = m_ComponentGroup.CalculateLength();

            var worldToLocalArray = new NativeArray<float4x4>(entityLength, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var rootLocalToWorldArray = new NativeArray<float4x4>(entityLength, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var boundsArray = new NativeArray<Bounds>(entityLength, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            var counter = 0;
            Entities.ForEach((Entity entity, SpriteSkin spriteSkin) =>
            {
                if (spriteSkin.isValid && spriteSkin.spriteRenderer.enabled)
                { 
                    worldToLocalArray[counter] = spriteSkin.transform.worldToLocalMatrix;
                    rootLocalToWorldArray[counter] = spriteSkin.rootBone.localToWorldMatrix;

                    var unityBounds = spriteSkin.bounds;
                    boundsArray[counter] = new Bounds()
                    {
                        center = new float4(unityBounds.center, 1),
                        extents = new float4(unityBounds.extents, 0),
                    };
                }
                counter++;
            });

            var jobHandle = new CalculateBoundsJob()
            {
                worldToLocalArray = worldToLocalArray,
                rootLocalToWorldArray = rootLocalToWorldArray,
                boundsArray = boundsArray
            }.Schedule(entityLength, 32);
            
            jobHandle.Complete();

            counter = 0;
            Entities.With(m_ComponentGroup).ForEach((Entity entity, SpriteSkin spriteSkin) =>
            {
                if (spriteSkin.isValid && spriteSkin.spriteRenderer.enabled)
                { 
                    var center = boundsArray[counter].center;
                    var extents = boundsArray[counter].extents;
                    var bounds = new UnityEngine.Bounds();
                    bounds.center = new Vector3(center.x, center.y, center.z);
                    bounds.extents = new Vector3(extents.x, extents.y, extents.z);
                    InternalEngineBridge.SetLocalAABB(spriteSkin.spriteRenderer, bounds);
                }
                counter++;
            });

            boundsArray.Dispose();
            return;
        }
    }
}

#endif