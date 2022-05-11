#if ENABLE_ANIMATION_COLLECTION && ENABLE_ANIMATION_BURST
#define ENABLE_SPRITESKIN_COMPOSITE
#endif

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.U2D.Animation
{
    public sealed  partial class SpriteSkin : MonoBehaviour
#if ENABLE_SPRITESKIN_COMPOSITE
    {
        int m_TransformId;
        NativeArray<int> m_BoneTransformId;
        int m_RootBoneTransformId;
        NativeCustomSlice<Vector2> m_SpriteUVs;
        NativeCustomSlice<Vector3> m_SpriteVertices;
        NativeCustomSlice<Vector4> m_SpriteTangents;
        NativeCustomSlice<BoneWeight> m_SpriteBoneWeights;
        NativeCustomSlice<Matrix4x4> m_SpriteBindPoses;
        NativeCustomSlice<int> m_BoneTransformIdNativeSlice;
        bool m_SpriteHasTangents;
        int m_SpriteVertexStreamSize;
        int m_SpriteVertexCount;
        int m_SpriteTangentVertexOffset;
        int m_DataIndex = -1;
        bool m_BoneCacheUpdateToDate = false;

        void OnEnableBatch()
        {
            m_TransformId = gameObject.transform.GetInstanceID();
            UpdateSpriteDeform();

            if (m_UseBatching && m_BatchSkinning == false)
            {
                CacheBoneTransformIds(true);
                SpriteSkinComposite.instance.AddSpriteSkin(this);
                m_BatchSkinning = true;
            }
            else
                SpriteSkinComposite.instance.AddSpriteSkinForLateUpdate(this);
        }

        void OnResetBatch()
        {
            if (m_UseBatching)
            {
                CacheBoneTransformIds(true);
                SpriteSkinComposite.instance.CopyToSpriteSkinData(this);
            }
        }

        void OnDisableBatch()
        {
            RemoveTransformFromSpriteSkinComposite();
            SpriteSkinComposite.instance.RemoveSpriteSkin(this);
            SpriteSkinComposite.instance.RemoveSpriteSkinForLateUpdate(this);

            m_BatchSkinning = false;
        }

        internal void UpdateSpriteDeform()
        {
            if (sprite == null)
            {
                m_SpriteUVs = NativeCustomSlice<Vector2>.Default();
                m_SpriteVertices = NativeCustomSlice<Vector3>.Default();
                m_SpriteTangents = NativeCustomSlice<Vector4>.Default();
                m_SpriteBoneWeights = NativeCustomSlice<BoneWeight>.Default();
                m_SpriteBindPoses = NativeCustomSlice<Matrix4x4>.Default();
                m_SpriteHasTangents = false;
                m_SpriteVertexStreamSize = 0;
                m_SpriteVertexCount = 0;
                m_SpriteTangentVertexOffset = 0;
            }
            else
            {
                m_SpriteUVs = new NativeCustomSlice<Vector2>(sprite.GetVertexAttribute<Vector2>(UnityEngine.Rendering.VertexAttribute.TexCoord0));
                m_SpriteVertices = new NativeCustomSlice<Vector3>(sprite.GetVertexAttribute<Vector3>(UnityEngine.Rendering.VertexAttribute.Position));
                m_SpriteTangents = new NativeCustomSlice<Vector4>(sprite.GetVertexAttribute<Vector4>(UnityEngine.Rendering.VertexAttribute.Tangent));
                m_SpriteBoneWeights = new NativeCustomSlice<BoneWeight>(sprite.GetVertexAttribute<BoneWeight>(UnityEngine.Rendering.VertexAttribute.BlendWeight));
                m_SpriteBindPoses = new NativeCustomSlice<Matrix4x4>(sprite.GetBindPoses());
                m_SpriteHasTangents = sprite.HasVertexAttribute(Rendering.VertexAttribute.Tangent);
                m_SpriteVertexStreamSize = sprite.GetVertexStreamSize();
                m_SpriteVertexCount = sprite.GetVertexCount();
                m_SpriteTangentVertexOffset = sprite.GetVertexStreamOffset(Rendering.VertexAttribute.Tangent);
            }
            SpriteSkinComposite.instance.CopyToSpriteSkinData(this);
        }

        void CacheBoneTransformIds(bool forceUpdate = false)
        {
            if (!m_BoneCacheUpdateToDate || forceUpdate)
            {
                SpriteSkinComposite.instance.RemoveTransformById(m_RootBoneTransformId);
                if (rootBone != null)
                {
                    m_RootBoneTransformId = rootBone.GetInstanceID();
                    if (this.enabled)
                        SpriteSkinComposite.instance.AddSpriteSkinRootBoneTransform(this);
                }
                else
                    m_RootBoneTransformId = 0;

                if (boneTransforms != null)
                {
                    int boneCount = 0;
                    for (int i = 0; i < boneTransforms.Length; ++i)
                    {
                        if (boneTransforms[i] != null)
                            ++boneCount;
                    }

                    if (m_BoneTransformId.IsCreated)
                    {
                        for (int i = 0; i < m_BoneTransformId.Length; ++i)
                            SpriteSkinComposite.instance.RemoveTransformById(m_BoneTransformId[i]);
                        NativeArrayHelpers.ResizeIfNeeded(ref m_BoneTransformId, boneCount);
                    }
                    else
                    {
                        m_BoneTransformId = new NativeArray<int>(boneCount, Allocator.Persistent);
                    }

                    m_BoneTransformIdNativeSlice = new NativeCustomSlice<int>(m_BoneTransformId);
                    for (int i = 0, j = 0; i < boneTransforms.Length; ++i)
                    {
                        if (boneTransforms[i] != null)
                        {
                            m_BoneTransformId[j] = boneTransforms[i].GetInstanceID();
                            ++j;
                        }
                    }
                    if (this.enabled)
                    {
                        SpriteSkinComposite.instance.AddSpriteSkinBoneTransform(this);
                    }
                }
                else
                {
                    if (m_BoneTransformId.IsCreated)
                        NativeArrayHelpers.ResizeIfNeeded(ref m_BoneTransformId, 0);
                    else
                        m_BoneTransformId = new NativeArray<int>(0, Allocator.Persistent);
                }
                CacheValidFlag();
                m_BoneCacheUpdateToDate = true;
                SpriteSkinComposite.instance.CopyToSpriteSkinData(this);
            }
        }

        void UseBatchingBatch()
        {
            if (!this.enabled)
                return;

            if (m_UseBatching)
            {
                m_BatchSkinning = true;
                CacheBoneTransformIds();
                SpriteSkinComposite.instance.AddSpriteSkin(this);
                SpriteSkinComposite.instance.RemoveSpriteSkinForLateUpdate(this);
            }
            else
            {
                SpriteSkinComposite.instance.RemoveSpriteSkin(this);
                SpriteSkinComposite.instance.AddSpriteSkinForLateUpdate(this);
                RemoveTransformFromSpriteSkinComposite();
                m_BatchSkinning = false;
            }
        }

        void RemoveTransformFromSpriteSkinComposite()
        {
            if (m_BoneTransformId.IsCreated)
            {
                for (int i = 0; i < m_BoneTransformId.Length; ++i)
                    SpriteSkinComposite.instance.RemoveTransformById(m_BoneTransformId[i]);
                m_BoneTransformId.Dispose();
            }
            SpriteSkinComposite.instance.RemoveTransformById(m_RootBoneTransformId);
            m_RootBoneTransformId = -1;
            m_BoneCacheUpdateToDate = false;
        }

        internal void CopyToSpriteSkinData(ref SpriteSkinData data, int spriteSkinIndex)
        {
            CacheBoneTransformIds();
            CacheCurrentSprite();

            data.vertices = m_SpriteVertices;
            data.boneWeights = m_SpriteBoneWeights;
            data.bindPoses = m_SpriteBindPoses;
            data.tangents = m_SpriteTangents;
            data.hasTangents = m_SpriteHasTangents;
            data.spriteVertexStreamSize = m_SpriteVertexStreamSize;
            data.spriteVertexCount = m_SpriteVertexCount;
            data.tangentVertexOffset = m_SpriteTangentVertexOffset;
            data.transformId = m_TransformId;
            data.boneTransformId = m_BoneTransformIdNativeSlice;
            m_DataIndex = spriteSkinIndex;
        }

        internal bool NeedUpdateCompositeCache()
        {
            unsafe
            {
                var iptr = new IntPtr(sprite.GetVertexAttribute<Vector2>(UnityEngine.Rendering.VertexAttribute.TexCoord0).GetUnsafeReadOnlyPtr());
                var rs = m_SpriteUVs.data != iptr;
                if (rs)
                {
                    UpdateSpriteDeform();
                }
                return rs;
            }
        }

        internal bool BatchValidate()
        {
            CacheBoneTransformIds();
            CacheCurrentSprite();
            return (m_IsValid && spriteRenderer.enabled && (alwaysUpdate || spriteRenderer.isVisible));
        }

        void OnBoneTransformChanged()
        {
            if (this.enabled)
            {
                CacheBoneTransformIds(true);
            }
        }

        void OnRootBoneTransformChanged()
        {
            if (this.enabled)
            {
                CacheBoneTransformIds(true);
            }
        }

        void OnBeforeSerializeBatch()
        {}

        void OnAfterSerializeBatch()
        {
#if UNITY_EDITOR
            m_BoneCacheUpdateToDate = false;
#endif
        }
    }
#else
    {
        void OnEnableBatch(){}
        internal void UpdateSpriteDeform(){}
        void OnResetBatch(){}
        void UseBatchingBatch(){}
        void OnDisableBatch(){}
        void OnBoneTransformChanged(){}
        void OnRootBoneTransformChanged(){}
        void OnBeforeSerializeBatch(){}
        void OnAfterSerializeBatch(){}
    }
#endif

}
