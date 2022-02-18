#if ENABLE_ANIMATION_COLLECTION && ENABLE_ANIMATION_BURST
#define ENABLE_ANIMATION_PERFORMANCE
#endif
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Linq;
using UnityEditor.U2D.Sprites;

using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

namespace UnityEditor.U2D.Animation
{
    internal class SpritePostProcess : AssetPostprocessor
    {
        private static List<object> m_AssetList;

        void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            var dataProviderFactories = new SpriteDataProviderFactories();
            dataProviderFactories.Init();
            ISpriteEditorDataProvider ai = dataProviderFactories.GetSpriteEditorDataProviderFromObject(AssetImporter.GetAtPath(assetPath));
            if (ai != null)
            {
                float definitionScale = CalculateDefinitionScale(texture, ai.GetDataProvider<ITextureDataProvider>());
                ai.InitSpriteEditorDataProvider();
                PostProcessBoneData(ai, definitionScale, sprites);
                PostProcessSpriteMeshData(ai, definitionScale, sprites);
                BoneGizmo.instance.ClearSpriteBoneCache();
            }

            // Get all SpriteSkin in scene and inform them to refresh their cache
            RefreshSpriteSkinCache();
        }

        static void RefreshSpriteSkinCache()
        {
#if ENABLE_ANIMATION_PERFORMANCE
            var spriteSkins = GameObject.FindObjectsOfType<SpriteSkin>();
            foreach (var ss in spriteSkins)
            {
                ss.ResetSprite();
            }
#endif
        }

        static void CalculateLocaltoWorldMatrix(int i, SpriteRect spriteRect, float definitionScale, float pixelsPerUnit, List<UnityEngine.U2D.SpriteBone> spriteBone, ref UnityEngine.U2D.SpriteBone?[] outpriteBone, ref NativeArray<Matrix4x4> bindPose)
        {
            if (outpriteBone[i] != null)
                return;
            UnityEngine.U2D.SpriteBone sp = spriteBone[i];
            var isRoot = sp.parentId == -1;
            var position = isRoot ? (spriteBone[i].position - Vector3.Scale(spriteRect.rect.size, spriteRect.pivot)) : spriteBone[i].position;
            position.z = 0f;
            sp.position = position * definitionScale / pixelsPerUnit;
            sp.length = spriteBone[i].length * definitionScale / pixelsPerUnit;
            outpriteBone[i] = sp;

            // Calculate bind poses
            var worldPosition = Vector3.zero;
            var worldRotation = Quaternion.identity;

            if (sp.parentId == -1)
            {
                worldPosition = sp.position;
                worldRotation = sp.rotation;
            }
            else
            {
                if (outpriteBone[sp.parentId] == null)
                {
                    CalculateLocaltoWorldMatrix(sp.parentId, spriteRect, definitionScale, pixelsPerUnit, spriteBone, ref outpriteBone, ref bindPose);
                }
                var parentBindPose = bindPose[sp.parentId];
                var invParentBindPose = Matrix4x4.Inverse(parentBindPose);

                worldPosition = invParentBindPose.MultiplyPoint(sp.position);
                worldRotation = sp.rotation * invParentBindPose.rotation;
            }

            // Practically Matrix4x4.SetTRInverse
            var rot = Quaternion.Inverse(worldRotation);
            Matrix4x4 mat = Matrix4x4.identity;
            mat = Matrix4x4.Rotate(rot);
            mat = mat * Matrix4x4.Translate(-worldPosition);


            bindPose[i] = mat;
        }

        static bool PostProcessBoneData(ISpriteEditorDataProvider spriteDataProvider, float definitionScale, Sprite[] sprites)
        {
            var boneDataProvider = spriteDataProvider.GetDataProvider<ISpriteBoneDataProvider>();
            var textureDataProvider = spriteDataProvider.GetDataProvider<ITextureDataProvider>();

            if (sprites == null || sprites.Length == 0 || boneDataProvider == null || textureDataProvider == null)
                return false;

            bool dataChanged = false;
            var spriteRects = spriteDataProvider.GetSpriteRects();
            foreach (var sprite in sprites)
            {
                var guid = sprite.GetSpriteID();
                {
                    var spriteBone = boneDataProvider.GetBones(guid);
                    if (spriteBone == null)
                        continue;

                    var spriteBoneCount = spriteBone.Count;
                    if (spriteBoneCount == 0)
                        continue;

                    var spriteRect = spriteRects.First(s => { return s.spriteID == guid; });

                    var bindPose = new NativeArray<Matrix4x4>(spriteBoneCount, Allocator.Temp);
                    var outputSpriteBones = new UnityEngine.U2D.SpriteBone ? [spriteBoneCount];
                    for (int i = 0; i < spriteBoneCount; ++i)
                    {
                        CalculateLocaltoWorldMatrix(i, spriteRect, definitionScale, sprite.pixelsPerUnit, spriteBone, ref outputSpriteBones, ref bindPose);
                    }
                    sprite.SetBindPoses(bindPose);
                    sprite.SetBones(outputSpriteBones.Select(x => x.Value).ToArray());
                    bindPose.Dispose();

                    dataChanged = true;
                }
            }

            return dataChanged;
        }

        static bool PostProcessSpriteMeshData(ISpriteEditorDataProvider spriteDataProvider, float definitionScale, Sprite[] sprites)
        {
            var spriteMeshDataProvider = spriteDataProvider.GetDataProvider<ISpriteMeshDataProvider>();
            var boneDataProvider = spriteDataProvider.GetDataProvider<ISpriteBoneDataProvider>();
            var textureDataProvider = spriteDataProvider.GetDataProvider<ITextureDataProvider>();
            if (sprites == null || sprites.Length == 0 || spriteMeshDataProvider == null || textureDataProvider == null)
                return false;

            bool dataChanged = false;
            var spriteRects = spriteDataProvider.GetSpriteRects();
            foreach (var sprite in sprites)
            {
                var guid = sprite.GetSpriteID();
                var vertices = spriteMeshDataProvider.GetVertices(guid);
                int[] indices = null;
                if (vertices.Length > 2)
                    indices = spriteMeshDataProvider.GetIndices(guid);

                if (indices != null && indices.Length > 2 && vertices.Length > 2 )
                {
                    var spriteRect = spriteRects.First(s => { return s.spriteID == guid; });
                    var spriteBone = boneDataProvider.GetBones(guid);
                    var hasBones = spriteBone != null && spriteBone.Count > 0;
                    var hasInvalidWeights = false;

                    var vertexArray = new NativeArray<Vector3>(vertices.Length, Allocator.Temp);
                    var boneWeightArray = new NativeArray<BoneWeight>(vertices.Length, Allocator.Temp);

                    for (int i = 0; i < vertices.Length; ++i)
                    {
                        var boneWeight = vertices[i].boneWeight;

                        vertexArray[i] = (Vector3)(vertices[i].position - Vector2.Scale(spriteRect.rect.size, spriteRect.pivot)) * definitionScale / sprite.pixelsPerUnit;
                        boneWeightArray[i] = boneWeight;

                        if (hasBones && !hasInvalidWeights)
                        {
                            var sum = boneWeight.weight0 + boneWeight.weight1 + boneWeight.weight2 + boneWeight.weight3;
                            hasInvalidWeights = sum < 0.999f;
                        }
                    }

                    var indicesArray = new NativeArray<ushort>(indices.Length, Allocator.Temp);
                    for (int i = 0; i < indices.Length; ++i)
                        indicesArray[i] = (ushort)indices[i];

                    sprite.SetVertexCount(vertices.Length);
                    sprite.SetVertexAttribute<Vector3>(VertexAttribute.Position, vertexArray);
                    sprite.SetIndices(indicesArray);
                    sprite.SetVertexAttribute<BoneWeight>(VertexAttribute.BlendWeight, boneWeightArray);
                    vertexArray.Dispose();
                    boneWeightArray.Dispose();
                    indicesArray.Dispose();

                    // Deformed Sprites require proper Tangent Channels if Lit. Enable Tangent channels.
                    if (hasBones)
                    {
                        var tangentArray = new NativeArray<Vector4>(vertices.Length, Allocator.Temp);
                        for (int i = 0; i < vertices.Length; ++i)
                            tangentArray[i] = new Vector4(1.0f, 0.0f, 0, -1.0f);
                        sprite.SetVertexAttribute<Vector4>(VertexAttribute.Tangent, tangentArray);
                        tangentArray.Dispose();
                    }

                    dataChanged = true;

                    if (hasBones && hasInvalidWeights)
                        Debug.LogWarning("Sprite \"" + spriteRect.name + "\" contains bone weights which sum zero or are not normalized. To avoid visual artifacts please consider fixing them.");
                }
                else
                {
                    var boneWeightArray = new NativeArray<BoneWeight>(sprite.GetVertexCount(), Allocator.Temp);
                    var defaultBoneWeight = new BoneWeight() { weight0 = 1f };

                    for (var i = 0; i < boneWeightArray.Length; ++i)
                        boneWeightArray[i] = defaultBoneWeight;

                    sprite.SetVertexAttribute<BoneWeight>(VertexAttribute.BlendWeight, boneWeightArray);
                }
            }

            return dataChanged;
        }

        static float CalculateDefinitionScale(Texture2D texture, ITextureDataProvider dataProvider)
        {
            float definitionScale = 1;
            if (texture != null && dataProvider != null)
            {
                int actualWidth = 0, actualHeight = 0;
                dataProvider.GetTextureActualWidthAndHeight(out actualWidth, out actualHeight);
                float definitionScaleW = texture.width / (float)actualWidth;
                float definitionScaleH = texture.height / (float)actualHeight;
                definitionScale = Mathf.Min(definitionScaleW, definitionScaleH);
            }
            return definitionScale;
        }
    }
}
