using System;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEditor.U2D.Animation
{
    internal static class BoneCacheExtensions
    {
        public static BoneCache[] ToCharacterIfNeeded(this BoneCache[] bones)
        {
            return Array.ConvertAll(bones, b => ToCharacterIfNeeded(b));
        }

        public static BoneCache[] ToSpriteSheetIfNeeded(this BoneCache[] bones)
        {
            return Array.ConvertAll(bones, b => ToSpriteSheetIfNeeded(b));
        }

        public static BoneCache ToCharacterIfNeeded(this BoneCache bone)
        {
            if (bone == null)
                return null;

            var skinningCache = bone.skinningCache;

            if (skinningCache.hasCharacter)
            {
                if (bone.skeleton != skinningCache.character.skeleton)
                {
                    var selectedSprite = skinningCache.selectedSprite;

                    if (selectedSprite == null)
                        return null;

                    var skeleton = selectedSprite.GetSkeleton();
                    var characterPart = selectedSprite.GetCharacterPart();

                    Debug.Assert(skeleton != null);
                    Debug.Assert(characterPart != null);
                    Debug.Assert(bone.skeleton == skeleton);
                    Debug.Assert(skeleton.BoneCount == characterPart.BoneCount);

                    var index = skeleton.IndexOf(bone);

                    if (index == -1)
                        bone = null;
                    else
                        bone = characterPart.GetBone(index);
                }
            }

            return bone;
        }

        public static BoneCache ToSpriteSheetIfNeeded(this BoneCache bone)
        {
            if (bone == null)
                return null;

            var skinningCache = bone.skinningCache;

            if (skinningCache.hasCharacter && skinningCache.mode == SkinningMode.SpriteSheet)
            {
                var selectedSprite = skinningCache.selectedSprite;

                if (selectedSprite == null)
                    return null;

                var characterSkeleton = skinningCache.character.skeleton;

                Debug.Assert(bone.skeleton == characterSkeleton);

                var skeleton = selectedSprite.GetSkeleton();
                var characterPart = selectedSprite.GetCharacterPart();

                Debug.Assert(skeleton != null);
                Debug.Assert(characterPart != null);
                Debug.Assert(skeleton.BoneCount == characterPart.BoneCount);

                var index = characterPart.IndexOf(bone);

                if (index == -1)
                    bone = null;
                else
                    bone = skeleton.GetBone(index);
            }

            return bone;
        }

        public static UnityEngine.U2D.SpriteBone ToSpriteBone(this BoneCache bone, Matrix4x4 rootTransform, int parentId)
        {
            var position = bone.localPosition;
            var rotation = bone.localRotation;

            if (parentId == -1)
            {
                rotation = bone.rotation;
                position = rootTransform.inverse.MultiplyPoint3x4(bone.position);
            }

            return new UnityEngine.U2D.SpriteBone()
            {
                name = bone.name,
                position = new Vector3(position.x, position.y, bone.depth),
                rotation = rotation,
                length = bone.localLength,
                parentId = parentId
            };
        }

        public static UnityEngine.U2D.SpriteBone[] ToSpriteBone(this BoneCache[] bones, Matrix4x4 rootTransform)
        {
            var spriteBones = new List<UnityEngine.U2D.SpriteBone>();

            foreach (var bone in bones)
            {
                var parentId = -1;

                if (ArrayUtility.Contains(bones, bone.parentBone))
                    parentId = Array.IndexOf(bones, bone.parentBone);

                spriteBones.Add(bone.ToSpriteBone(rootTransform, parentId));
            }

            return spriteBones.ToArray();
        }
    }
}
