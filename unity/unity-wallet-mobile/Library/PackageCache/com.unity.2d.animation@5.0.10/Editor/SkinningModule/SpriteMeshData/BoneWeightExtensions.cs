using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal static class BoneWeightExtensions
    {
        public static int GetBoneIndex(this BoneWeight boneWeight, int channelIndex)
        {
            if (channelIndex < 0 || channelIndex >= 4)
                throw new ArgumentOutOfRangeException("Channel index out of range");

            if (channelIndex == 0)
                return boneWeight.boneIndex0;
            if (channelIndex == 1)
                return boneWeight.boneIndex1;
            if (channelIndex == 2)
                return boneWeight.boneIndex2;
            if (channelIndex == 3)
                return boneWeight.boneIndex3;
            return -1;
        }

        public static void SetBoneIndex(ref BoneWeight boneWeight, int channelIndex, int boneIndex)
        {
            if (channelIndex < 0 || channelIndex >= 4)
                throw new ArgumentOutOfRangeException("Channel index out of range");

            if (channelIndex == 0)
                boneWeight.boneIndex0 = boneIndex;
            if (channelIndex == 1)
                boneWeight.boneIndex1 = boneIndex;
            if (channelIndex == 2)
                boneWeight.boneIndex2 = boneIndex;
            if (channelIndex == 3)
                boneWeight.boneIndex3 = boneIndex;
        }

        public static float GetWeight(this BoneWeight boneWeight, int channelIndex)
        {
            if (channelIndex < 0 || channelIndex >= 4)
                throw new ArgumentOutOfRangeException("Channel index out of range");

            if (channelIndex == 0)
                return boneWeight.weight0;
            if (channelIndex == 1)
                return boneWeight.weight1;
            if (channelIndex == 2)
                return boneWeight.weight2;
            if (channelIndex == 3)
                return boneWeight.weight3;
            return 0f;
        }

        public static void SetWeight(ref BoneWeight boneWeight, int channelIndex, float weight)
        {
            if (channelIndex < 0 || channelIndex >= 4)
                throw new ArgumentOutOfRangeException("Channel index out of range");

            if (channelIndex == 0)
                boneWeight.weight0 = weight;
            if (channelIndex == 1)
                boneWeight.weight1 = weight;
            if (channelIndex == 2)
                boneWeight.weight2 = weight;
            if (channelIndex == 3)
                boneWeight.weight3 = weight;
        }

        public static float Sum(this BoneWeight boneWeight)
        {
            return boneWeight.weight0 + boneWeight.weight1 + boneWeight.weight2 + boneWeight.weight3;
        }

        public static BoneWeight Normalized(this BoneWeight boneWeight)
        {
            var sum = boneWeight.Sum();
            
            if (sum == 0 || sum == 1f)
                return boneWeight;

            var normalized = boneWeight;
            var sumInv = 1f / sum;

            for (var i = 0; i < 4; ++i)
                SetWeight(ref normalized, i, normalized.GetWeight(i) * sumInv);

            return normalized;
        }
    }
}
