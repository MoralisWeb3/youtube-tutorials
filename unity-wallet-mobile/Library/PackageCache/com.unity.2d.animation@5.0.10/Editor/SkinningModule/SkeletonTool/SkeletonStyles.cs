using UnityEngine;
using System;

namespace UnityEditor.U2D.Animation
{
    internal interface ISkeletonStyle
    {
        Color GetColor(BoneCache bone);
        Color GetPreviewColor(int index);
        Color GetParentLinkColor(BoneCache bone);
        Color GetParentLinkPreviewColor(int index);
        Color GetOutlineColor(BoneCache bone, bool isSelected, bool isHovered);
        Color GetPreviewOutlineColor(int index);
        float GetOutlineScale(bool isSelected);
    }

    internal abstract class SkeletonStyleBase : ISkeletonStyle
    {
        private const float kOutlineScale = 1.35f;
        private const float kSelectedOutlineScale = 1.55f;

        public Color GetColor(BoneCache bone)
        {
            return SetAlpha(GetBoneColorRaw(bone), GetAlpha(bone), VisibilityToolSettings.boneOpacity);
        }

        public Color GetPreviewColor(int index)
        {
            return GetBoneColorRaw(index);
        }

        public Color GetParentLinkColor(BoneCache bone)
        {
            return SetAlpha(GetBoneColorRaw(bone), 0.2f * GetAlpha(bone), VisibilityToolSettings.boneOpacity);
        }

        public Color GetParentLinkPreviewColor(int index)
        {
            return SetAlpha(GetBoneColorRaw(index), 0.2f, 1f);
        }

        public Color GetOutlineColor(BoneCache bone, bool isSelected, bool isHovered)
        {
            var skinningCache = bone.skinningCache;

            if (isSelected)
                return SelectionOutlineSettings.outlineColor;

            if (isHovered)
                return Handles.preselectionColor;

            return SetAlpha(CalculateOutlineColor(GetBoneColorRaw(bone), VisibilityToolSettings.boneOpacity), GetAlpha(bone), VisibilityToolSettings.boneOpacity);
        }

        public Color GetPreviewOutlineColor(int index)
        {
            return CalculateOutlineColor(GetBoneColorRaw(index), 1f);
        }

        public float GetOutlineScale(bool isSelected)
        {
            if (isSelected)
                return 1f + (kSelectedOutlineScale - 1f) * SelectionOutlineSettings.selectedBoneOutlineSize;

            return kOutlineScale;
        }

        private Color CalculateOutlineColor(Color color, float opacity)
        {
            color *= 0.35f;
            return SetAlpha(color, 0.75f, opacity);
        }

        private Color SetAlpha(Color color, float alpha, float opacity)
        {
            color.a = alpha * opacity;
            return color;
        }

        protected virtual float GetAlpha(BoneCache bone)
        {
            return 1f;
        }

        protected abstract Color GetBoneColorRaw(BoneCache bone);
        protected abstract Color GetBoneColorRaw(int index);
    }

    internal class BoneColorSkeletonStyle : SkeletonStyleBase
    {
        protected override Color GetBoneColorRaw(BoneCache bone)
        {
            return bone.bindPoseColor;
        }

        protected override Color GetBoneColorRaw(int index)
        {
            return ModuleUtility.CalculateNiceColor(index, 6);
        }

        protected override float GetAlpha(BoneCache bone)
        {
            return 0.9f;
        }
    }

    internal class WeightmapSkeletonStyle : SkeletonStyleBase
    {
        protected override Color GetBoneColorRaw(BoneCache bone)
        {
            return bone.bindPoseColor;
        }

        protected override Color GetBoneColorRaw(int index)
        {
            return ModuleUtility.CalculateNiceColor(index, 6);
        }

        protected override float GetAlpha(BoneCache bone)
        {
            var skinningCache = bone.skinningCache;
            var selectedSprite = skinningCache.selectedSprite;
            var alpha = 0.9f;

            if (skinningCache.mode == SkinningMode.Character && skinningCache.selectedSprite != null)
            {
                var characterPart = selectedSprite.GetCharacterPart();

                if (characterPart.Contains(bone) == false)
                    alpha = 0.25f;
            }

            return alpha;
        }
    }

    internal static class SkeletonStyles
    {
        public static readonly ISkeletonStyle Default = new BoneColorSkeletonStyle();
        public static readonly ISkeletonStyle WeightMap = new WeightmapSkeletonStyle();
    }
}
