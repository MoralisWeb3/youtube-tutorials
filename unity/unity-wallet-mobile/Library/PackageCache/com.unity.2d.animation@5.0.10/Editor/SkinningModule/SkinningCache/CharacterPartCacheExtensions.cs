using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal static class CharacterPartCacheExtensions
    {
        public static void SyncSpriteSheetSkeleton(this CharacterPartCache characterPart)
        {
            var skinningCache = characterPart.skinningCache;
            var character = skinningCache.character;
            var characterSkeleton = character.skeleton;
            var spriteSkeleton = characterPart.sprite.GetSkeleton();
            var spriteSkeletonBones = spriteSkeleton.bones;
            var characterPartBones = characterPart.bones;

            if (spriteSkeletonBones.Length != characterPartBones.Length)
                return;

            for (var i = 0; i < characterPartBones.Length; ++i)
            {
                var spriteBone = spriteSkeletonBones[i];
                var characterBone = characterPartBones[i];
                var childWorldPose = spriteBone.GetChildrenWoldPose();

                spriteBone.position = spriteSkeleton.localToWorldMatrix.MultiplyPoint3x4(
                    characterPart.worldToLocalMatrix.MultiplyPoint3x4(characterBone.position));
                spriteBone.rotation = characterBone.rotation;
                spriteBone.length = characterBone.length;
                spriteBone.name = characterBone.name;
                spriteBone.depth = characterBone.depth;
                spriteBone.bindPoseColor = characterBone.bindPoseColor;

                spriteBone.SetChildrenWorldPose(childWorldPose);
            }

            if (characterSkeleton.isPosePreview)
                spriteSkeleton.SetPosePreview();
            else
                spriteSkeleton.SetDefaultPose();
        }

        public static void DeassociateUnusedBones(this CharacterPartCache characterPart)
        {
            var skinningCache = characterPart.skinningCache;
            var bones = characterPart.bones;

            if (bones.Length == 0)
                return;

            Debug.Assert(characterPart.sprite != null);

            var mesh = characterPart.sprite.GetMesh();

            Debug.Assert(mesh != null);

            var vertices = mesh.vertices;
            var newBonesSet = new HashSet<BoneCache>();

            foreach (var vertex in vertices)
            {
                var boneWeight = vertex.editableBoneWeight;

                foreach (BoneWeightChannel channel in boneWeight)
                    if (channel.enabled)
                        newBonesSet.Add(bones[channel.boneIndex]);
            }

            bones = new List<BoneCache>(newBonesSet).ToArray();

            characterPart.bones = bones;

            characterPart.sprite.UpdateMesh(bones);

            skinningCache.events.characterPartChanged.Invoke(characterPart);
        }
    }
}
