#if ENABLE_ENTITIES

using Unity.Entities;
using Unity.Mathematics;

namespace UnityEngine.U2D.Animation
{
    [InternalBufferCapacity(0)]
    struct Vertex : IBufferElementData
    {
        public float3 Value;
    }

    [InternalBufferCapacity(0)]
    struct BoneTransform : IBufferElementData
    {
        public float4x4 Value;
    }

    struct WorldToLocal : IComponentData
    {
        public float4x4 Value;
    }

    struct SpriteComponent : ISharedComponentData
    {
        public Sprite Value;
    }

    struct BoundsComponent : IComponentData
    {
        public float4 center;
        public float4 extents;
    }
}

#endif