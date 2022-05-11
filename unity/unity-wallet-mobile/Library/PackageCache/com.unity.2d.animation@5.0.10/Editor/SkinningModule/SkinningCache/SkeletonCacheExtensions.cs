using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal static class SkeletonCacheExtensions
    {
        public static void RotateBones(this SkeletonCache skeleton, BoneCache[] bones, float deltaAngle)
        {
            Debug.Assert(skeleton != null);

            foreach (var bone in bones)
            {
                Debug.Assert(bone != null);
                Debug.Assert(skeleton.Contains(bone));
                
                bone.localRotation *= Quaternion.AngleAxis(deltaAngle, Vector3.forward);
            }
        }

        public static void MoveBones(this SkeletonCache skeleton, BoneCache[] bones, Vector3 deltaPosition)
        {
            Debug.Assert(skeleton != null);

            foreach (var bone in bones)
            {
                Debug.Assert(bone != null);
                Debug.Assert(skeleton.Contains(bone));

                bone.position += deltaPosition;

                if (bone.parentBone != null && bone.parentBone.chainedChild == bone)
                    bone.parentBone.OrientToChainedChild(false);
            }
        }

        public static void FreeMoveBones(this SkeletonCache skeleton, BoneCache[] bones, Vector3 deltaPosition)
        {
            Debug.Assert(skeleton != null);

            foreach (var bone in bones)
            {
                Debug.Assert(bone != null);
                Debug.Assert(skeleton.Contains(bone));

                var childrenWorldPose = bone.GetChildrenWoldPose();

                if (bone.chainedChild != null && ArrayUtility.Contains(bones, bone.chainedChild) == false)
                    bone.chainedChild = null;

                if (bone.parentBone != null && bone.parentBone.chainedChild == bone && ArrayUtility.Contains(bones, bone.parentBone) == false)
                    bone.parentBone.chainedChild = null;

                bone.position += deltaPosition;

                bone.SetChildrenWorldPose(childrenWorldPose);
            }
        }

        public static void MoveJoints(this SkeletonCache skeleton, BoneCache[] bones, Vector3 deltaPosition)
        {
            Debug.Assert(skeleton != null);

            foreach (var bone in bones)
            {
                Debug.Assert(bone != null);
                Debug.Assert(skeleton.Contains(bone));

                var childrenWorldPose = bone.GetChildrenWoldPose();
                var endPosition = bone.endPosition;

                bone.position += deltaPosition;

                if (bone.localLength > 0f)
                    bone.endPosition = endPosition;

                if (bone.parentBone != null && bone.parentBone.chainedChild == bone)
                    bone.parentBone.OrientToChainedChild(true);

                bone.SetChildrenWorldPose(childrenWorldPose);

                if (bone.chainedChild != null)
                    bone.OrientToChainedChild(true);
            }
        }

        public static void SetEndPosition(this SkeletonCache skeleton, BoneCache bone, Vector3 endPosition)
        {
            Debug.Assert(skeleton != null);
            Debug.Assert(bone != null);
            Debug.Assert(skeleton.Contains(bone));

            var childrenStorage = bone.GetChildrenWoldPose();
            bone.endPosition = endPosition;
            bone.SetChildrenWorldPose(childrenStorage);
        }

        public static BoneCache SplitBone(this SkeletonCache skeleton, BoneCache boneToSplit, float splitLength, string name)
        {
            Debug.Assert(skeleton.Contains(boneToSplit));
            Debug.Assert(boneToSplit.length > splitLength);
            
            var endPosition = boneToSplit.endPosition;
            var chainedChild = boneToSplit.chainedChild;
            var splitPosition = boneToSplit.position + boneToSplit.right * splitLength;

            boneToSplit.length = splitLength;

            var bone = skeleton.CreateBone(boneToSplit, splitPosition, endPosition, true, name);

            if (chainedChild != null)
            {
                chainedChild.SetParent(bone);
                bone.chainedChild = chainedChild;
            }

            return bone;
        }

        public static BoneCache CreateBone(this SkeletonCache skeleton, BoneCache parentBone, Vector3 position, Vector3 endPosition, bool isChained, string name)
        {
            Debug.Assert(skeleton != null);

            if (parentBone != null)
                Debug.Assert(skeleton.Contains(parentBone));

            var bone = skeleton.skinningCache.CreateCache<BoneCache>();

            bone.SetParent(parentBone);
            bone.name = name;
            bone.bindPoseColor = ModuleUtility.CalculateNiceColor(skeleton.BoneCount, 6);
            bone.position = position;
            bone.endPosition = endPosition;

            if (isChained && parentBone != null)
                parentBone.chainedChild = bone;

            skeleton.AddBone(bone);


            return bone;
        }

        public static void SetBones(this SkeletonCache skeleton, BoneCache[] bones)
        {
            skeleton.SetBones(bones, true);
        }

        public static void SetBones(this SkeletonCache skeleton, BoneCache[] bones, bool worldPositionStays)
        {
            skeleton.Clear();
            skeleton.AddBones(bones, worldPositionStays);
            skeleton.SetDefaultPose();
        }

        public static void AddBones(this SkeletonCache skeleton, BoneCache[] bones)
        {
            skeleton.AddBones(bones, true);
        }

        public static void AddBones(this SkeletonCache skeleton, BoneCache[] bones, bool worldPositionStays)
        {
            foreach (var bone in bones)
                skeleton.AddBone(bone, worldPositionStays);
        }

        public static void DestroyBones(this SkeletonCache skeleton, BoneCache[] bones)
        {
            Debug.Assert(skeleton != null);

            foreach (var bone in bones)
            {
                Debug.Assert(bone != null);
                Debug.Assert(skeleton.Contains(bone));

                skeleton.DestroyBone(bone);
            }
        }
    }
}
