using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class BoneVisibilityToolData : CacheObject
    {
        [SerializeField]
        bool m_AllVisibility = true;
        bool m_PreviousVisibility = true;

        public bool allVisibility
        {
            get { return m_AllVisibility; }
            set { m_AllVisibility = value; }
        }
        public bool previousVisiblity
        {
            get { return m_PreviousVisibility; }
            set { m_PreviousVisibility = value; }
        }
    }

    internal class BoneTreeWidgetModel : IBoneTreeViewModel
    {
        protected SkinningCache m_SkinningCache;
        protected IBoneVisibilityToolView m_View;
        protected BoneVisibilityToolData m_Data;

        public SkinningCache skinningCache
        {
            get { return m_SkinningCache; }
        }

        public IBoneVisibilityToolView view
        {
            get { return m_View; }
        }

        public virtual bool GetAllVisibility()
        {
            return m_Data.allVisibility;
        }

        public SkeletonSelection GetBoneSelection()
        {
            return skinningCache.skeletonSelection;
        }

        public BoneCache[] GetExpandedBones()
        {
            return skinningCache.GetExpandedBones();
        }

        public int GetDepth(BoneCache bone)
        {
            return (int)bone.depth;
        }

        public SkeletonCache GetSelectedSkeleton()
        {
            return skinningCache.GetEffectiveSkeleton(skinningCache.selectedSprite);
        }

        public bool GetVisibility(BoneCache bone)
        {
            return bone.isVisible;
        }

        public void SelectBones(BoneCache[] bones)
        {
            skinningCache.skeletonSelection.elements = bones.ToCharacterIfNeeded();
        }

        public void SetExpandedBones(BoneCache[] bones)
        {
            skinningCache.BoneExpansionChanged(bones);
        }

        public virtual void SetAllVisibility(SkeletonCache skeleton, bool visibility)
        {
            m_Data.allVisibility = visibility;
            SetAllBoneVisibility(skeleton, visibility);
            UpdateVisibilityFromPersistentState();
        }

        public static void SetAllBoneVisibility(SkeletonCache skeleton, bool visibility)
        {
            if (skeleton != null)
            {
                foreach (var bone in skeleton.bones)
                    bone.isVisible = visibility;
            }
        }

        public void SetBoneParent(BoneCache newParent, BoneCache bone, int insertAtIndex)
        {
            TransformCache parent = newParent;

            if (newParent == null)
                parent = bone.skeleton;

            skinningCache.RestoreBindPose();
            bone.SetParent(parent, true);

            if (insertAtIndex == -1)
                insertAtIndex = parent.ChildCount;

            bone.siblingIndex = insertAtIndex;
            bone.SetDefaultPose();
        }

        public void SetDepth(BoneCache bone, int depth)
        {
            var characterBone = bone.ToCharacterIfNeeded();
            characterBone.depth = depth;
            
            if (characterBone != bone || skinningCache.mode == SkinningMode.Character)
                skinningCache.SyncSpriteSheetSkeletons();

            skinningCache.events.boneDepthChanged.Invoke(bone);
        }

        public void SetName(BoneCache bone, string name)
        {
            var characterBone = bone.ToCharacterIfNeeded();
            characterBone.name = name;
            if (characterBone != bone || skinningCache.mode == SkinningMode.Character)
            {
                skinningCache.SyncSpriteSheetSkeletons();
            }
        }

        public void SetVisibility(BoneCache bone, bool visibility)
        {
            bone.isVisible = visibility;
            UpdateVisibilityFromPersistentState();
        }

        public UndoScope UndoScope(string value)
        {
            return skinningCache.UndoScope(value);
        }

        private void UpdateVisibilityFromPersistentState()
        {
            skinningCache.BoneVisibilityChanged(GetSelectedSkeleton());
        }

        public bool hasCharacter {get { return skinningCache.hasCharacter; } }
        public SkinningMode mode {get { return skinningCache.mode; } }
    }


    internal interface IBoneTreeViewModel
    {
        void SetVisibility(BoneCache bone, bool visibility);
        bool GetVisibility(BoneCache bone);
        void SetName(BoneCache bone, string name);
        void SetBoneParent(BoneCache newParent, BoneCache bone, int insertAtIndex);
        int GetDepth(BoneCache bone);
        void SetDepth(BoneCache bone, int depth);
        void SetAllVisibility(SkeletonCache skeleton, bool visibility);
        bool GetAllVisibility();
        void SelectBones(BoneCache[] bones);
        void SetExpandedBones(BoneCache[] bones);
        IBoneVisibilityToolView view { get; }
        SkeletonSelection GetBoneSelection();
        BoneCache[] GetExpandedBones();
        SkeletonCache GetSelectedSkeleton();
        bool hasCharacter { get; }
        SkinningMode mode { get; }
        UndoScope UndoScope(string value);
    }
}
