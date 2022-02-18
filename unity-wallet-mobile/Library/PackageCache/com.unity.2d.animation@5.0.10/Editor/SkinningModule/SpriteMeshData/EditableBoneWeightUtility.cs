using UnityEngine;
using System;
using System.Collections.Generic;

namespace UnityEditor.U2D.Animation
{
    internal struct BoneWeightData : IComparable<BoneWeightData>
    {
        public int boneIndex;
        public float weight;

        public int CompareTo(BoneWeightData other)
        {
            return other.weight.CompareTo(weight);
        }
    }
    
    internal static class EditableBoneWeightUtility
    {
        private static List<BoneWeightData> s_BoneWeightDataList = new List<BoneWeightData>();
        private static EditableBoneWeight s_LerpFirst = new EditableBoneWeight();
        private static EditableBoneWeight s_LerpSecond = new EditableBoneWeight();
        private static EditableBoneWeight s_LerpResult = new EditableBoneWeight();

        public static EditableBoneWeight CreateFromBoneWeight(BoneWeight boneWeight)
        {
            EditableBoneWeight editableBoneWeight = new EditableBoneWeight();

            editableBoneWeight.SetFromBoneWeight(boneWeight);
            editableBoneWeight.UnifyChannelsWithSameBoneIndex();

            return editableBoneWeight;
        }

        public static void SetFromBoneWeight(this EditableBoneWeight editableBoneWeight, BoneWeight boneWeight)
        {
            editableBoneWeight.Clamp(4, false);

            while (editableBoneWeight.Count < 4)
                editableBoneWeight.AddChannel(0, 0f, false);

            for (var i = 0; i < 4; ++i)
            {
                var weight = boneWeight.GetWeight(i);
                editableBoneWeight[i].boneIndex = boneWeight.GetBoneIndex(i);
                editableBoneWeight[i].weight = weight;
                editableBoneWeight[i].enabled = weight > 0f;
            }
        }

        public static BoneWeight ToBoneWeight(this EditableBoneWeight editableBoneWeight, bool sortByWeight)
        {
            var boneWeight = new BoneWeight();

            if (editableBoneWeight.Count > 0)
            {
                s_BoneWeightDataList.Clear();
                s_BoneWeightDataList.Capacity = editableBoneWeight.Count;

                for (var i = 0; i < editableBoneWeight.Count; ++i)
                {
                    s_BoneWeightDataList.Add(new BoneWeightData()
                    {
                        boneIndex = editableBoneWeight[i].boneIndex,
                        weight = editableBoneWeight[i].weight
                    });
                }

                if (sortByWeight)
                    s_BoneWeightDataList.Sort();

                var count = Mathf.Min(editableBoneWeight.Count, 4);

                for (var i = 0; i < count; ++i)
                {
                    BoneWeightExtensions.SetBoneIndex(ref boneWeight, i, s_BoneWeightDataList[i].boneIndex);
                    BoneWeightExtensions.SetWeight(ref boneWeight, i, s_BoneWeightDataList[i].weight);
                }
            }

            return boneWeight;
        }

        public static bool ContainsBoneIndex(this EditableBoneWeight editableBoneWeight, int boneIndex)
        {
            return GetChannelFromBoneIndex(editableBoneWeight, boneIndex) > -1;
        }

        public static int GetChannelFromBoneIndex(this EditableBoneWeight editableBoneWeight, int boneIndex)
        {
            for (int i = 0; i < editableBoneWeight.Count; ++i)
                if (editableBoneWeight[i].enabled && editableBoneWeight[i].boneIndex == boneIndex)
                    return i;

            return -1;
        }

        public static void Clamp(this EditableBoneWeight editableBoneWeight, int numChannels, bool sortChannels = true)
        {
            if (sortChannels)
                editableBoneWeight.Sort();

            while (editableBoneWeight.Count > numChannels)
                editableBoneWeight.RemoveChannel(numChannels);
        }

        public static void ValidateChannels(this EditableBoneWeight editableBoneWeight)
        {
            for (int i = 0; i < editableBoneWeight.Count; ++i)
            {
                var weight = editableBoneWeight[i].weight;

                if (!editableBoneWeight[i].enabled)
                    weight = 0f;

                weight = Mathf.Clamp01(weight);
                editableBoneWeight[i].weight = weight;
            }
        }

        public static float Sum(this EditableBoneWeight editableBoneWeight)
        {
            var sum = 0f;

            for (var i = 0; i < editableBoneWeight.Count; ++i)
                if (editableBoneWeight[i].enabled)
                    sum += editableBoneWeight[i].weight;

            return sum;
        }

        public static void Normalize(this EditableBoneWeight editableBoneWeight)
        {
            ValidateChannels(editableBoneWeight);

            var sum = editableBoneWeight.Sum();

            if (sum == 0f || sum == 1f)
                return;

            var sumInv = 1f / sum;

            for (var i = 0; i < editableBoneWeight.Count; ++i)
                if (editableBoneWeight[i].enabled)
                    editableBoneWeight[i].weight *= sumInv;
        }

        public static void CompensateOtherChannels(this EditableBoneWeight editableBoneWeight, int masterChannel)
        {
            ValidateChannels(editableBoneWeight);

            var validChannelCount = 0;
            var sum = 0f;

            for (int i = 0; i < editableBoneWeight.Count; ++i)
            {
                if (i != masterChannel && editableBoneWeight[i].enabled)
                {
                    sum += editableBoneWeight[i].weight;
                    ++validChannelCount;
                }
            }

            if (validChannelCount == 0)
                return;

            var targetSum = 1f - editableBoneWeight[masterChannel].weight;

            for (var i = 0; i < editableBoneWeight.Count; ++i)
            {
                if (i != masterChannel && editableBoneWeight[i].enabled)
                {
                    if (sum == 0f)
                        editableBoneWeight[i].weight = targetSum / validChannelCount;
                    else
                        editableBoneWeight[i].weight *= targetSum / sum;
                }
            }
        }

        public static void UnifyChannelsWithSameBoneIndex(this EditableBoneWeight editableBoneWeight)
        {
            for (var i = 0; i < editableBoneWeight.Count; ++i)
            {
                if (!editableBoneWeight[i].enabled)
                    continue;

                bool weightChanged = false;

                for (var j = i + 1; j < editableBoneWeight.Count; ++j)
                {
                    if (editableBoneWeight[j].boneIndex == editableBoneWeight[i].boneIndex)
                    {
                        weightChanged = true;
                        editableBoneWeight[i].weight += editableBoneWeight[j].weight;
                        editableBoneWeight[j].enabled = false;
                    }
                }

                if (weightChanged)
                    editableBoneWeight.CompensateOtherChannels(i);
            }
        }

        public static void FilterChannels(this EditableBoneWeight editableBoneWeight, float weightTolerance)
        {
            for (var i = 0; i < editableBoneWeight.Count; ++i)
            {
                if (editableBoneWeight[i].weight <= weightTolerance)
                {
                    editableBoneWeight[i].boneIndex = 0;
                    editableBoneWeight[i].weight = 0f;
                    editableBoneWeight[i].enabled = false;
                }
            }
        }

        public static BoneWeight Lerp(BoneWeight first, BoneWeight second, float t)
        {
            s_LerpFirst.SetFromBoneWeight(first);
            s_LerpSecond.SetFromBoneWeight(second);
            Lerp(s_LerpFirst, s_LerpSecond, ref s_LerpResult, t);

            return s_LerpResult.ToBoneWeight(true);
        }

        private static void Lerp(EditableBoneWeight first, EditableBoneWeight second, ref EditableBoneWeight result, float t)
        {
            result.Clear();

            foreach (BoneWeightChannel channel in first)
            {
                if (!channel.enabled)
                    continue;

                var weight = channel.weight * (1f - t);

                if (weight > 0f)
                    result.AddChannel(channel.boneIndex, weight, true);
            }

            foreach (BoneWeightChannel channel in second)
            {
                if (!channel.enabled)
                    continue;

                var weight = channel.weight * t;

                if (weight > 0f)
                    result.AddChannel(channel.boneIndex, weight, true);
            }

            result.UnifyChannelsWithSameBoneIndex();
            result.Clamp(4);

            if (result.Sum() > 1f)
                result.Normalize();

            result.FilterChannels(0f);
        }
    }
}
