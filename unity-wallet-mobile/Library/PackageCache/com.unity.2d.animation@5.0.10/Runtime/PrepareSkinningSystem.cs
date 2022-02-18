#if ENABLE_ENTITIES

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.U2D.Common;
using UnityEngine.Scripting;

namespace UnityEngine.U2D.Animation
{
    [Preserve]
    [UnityEngine.ExecuteAlways]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    internal class PrepareSkinningSystem : ComponentSystem
    {
        EntityQuery m_ComponentGroup;

        protected override void OnCreateManager()
        {
            m_ComponentGroup = GetEntityQuery(typeof(SpriteSkin), typeof(WorldToLocal), typeof(SpriteComponent), typeof(Vertex), typeof(BoneTransform));
        }

        protected override void OnUpdate()
        {
            var worldToLocalComponents = m_ComponentGroup.ToComponentDataArray<WorldToLocal>(Allocator.TempJob);

            var counter = 0;
            Entities.With(m_ComponentGroup).ForEach((Entity entity, SpriteSkin spriteSkin) =>
            {
                var sr = EntityManager.GetSharedComponentData<SpriteComponent>(entity);
                var vertexBuffer = EntityManager.GetBuffer<Vertex>(entity);
                var boneTransformBuffer = EntityManager.GetBuffer<BoneTransform>(entity);
                var currentSprite = sr.Value;
                var currentWorldToLocal = worldToLocalComponents[counter];
                Sprite sprite = null;               
                if (spriteSkin != null)
                { 

                    var spriteRenderer = spriteSkin.spriteRenderer;
                    var isValid = spriteRenderer.enabled && spriteSkin.isValid;
                    var isVisible = spriteRenderer.isVisible || spriteSkin.ForceSkinning;

                    if (!isValid)
                        spriteSkin.DeactivateSkinning();
                    else if (isVisible)
                    {
                        spriteSkin.ForceSkinning = false;
                        sprite = spriteRenderer.sprite;
                        float4x4 worldToLocal = spriteSkin.transform.worldToLocalMatrix;

                        if (vertexBuffer.Length != sprite.GetVertexCount())
                        {
                            vertexBuffer = PostUpdateCommands.SetBuffer<Vertex>(entity);
                            vertexBuffer.ResizeUninitialized(sprite.GetVertexCount());
                        }

                        InternalEngineBridge.SetDeformableBuffer(spriteRenderer, vertexBuffer.Reinterpret<Vector3>().AsNativeArray());

                        if (boneTransformBuffer.Length != spriteSkin.boneTransforms.Length)
                        {
                            boneTransformBuffer = PostUpdateCommands.SetBuffer<BoneTransform>(entity);
                            boneTransformBuffer.ResizeUninitialized(spriteSkin.boneTransforms.Length);
                        }

                        for (var j = 0; j < boneTransformBuffer.Length; ++j)
                            boneTransformBuffer[j] = new BoneTransform() { Value = spriteSkin.boneTransforms[j].localToWorldMatrix };

                        PostUpdateCommands.SetComponent<WorldToLocal>(entity, new WorldToLocal() { Value = worldToLocal });
                    }

                    if (currentSprite != sprite)
                        PostUpdateCommands.SetSharedComponent<SpriteComponent>(entity, new SpriteComponent() { Value = sprite });

                    if (!spriteRenderer.enabled)
                        spriteSkin.ForceSkinning = true;
                }
            });
            worldToLocalComponents.Dispose();
        }
    }
}

#endif