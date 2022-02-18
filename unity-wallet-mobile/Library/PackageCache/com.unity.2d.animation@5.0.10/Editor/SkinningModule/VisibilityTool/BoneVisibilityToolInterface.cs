namespace UnityEditor.U2D.Animation
{
    internal interface IBoneVisibilityToolView
    {
        void OnBoneSelectionChange(SkeletonSelection skeleton);
        void OnBoneExpandedChange(BoneCache[] bones);
        void OnBoneNameChanged(BoneCache bone);
        void OnSelectionChange(SkeletonCache skeleton);
        void Deactivate();
    }
}
