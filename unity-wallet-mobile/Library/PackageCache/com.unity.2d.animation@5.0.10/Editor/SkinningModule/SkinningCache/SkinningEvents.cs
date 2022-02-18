using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor.U2D.Animation
{
    internal class SkinningEvents
    {
        public class SpriteEvent : UnityEvent<SpriteCache> {}
        public class SkeletonEvent : UnityEvent<SkeletonCache> {}
        public class MeshEvent : UnityEvent<MeshCache> {}
        public class MeshPreviewEvent : UnityEvent<MeshPreviewCache> {}
        public class SkinningModuleModeEvent : UnityEvent<SkinningMode> {}
        public class BoneSelectionEvent : UnityEvent {}
        public class BoneEvent : UnityEvent<BoneCache> {}
        public class CharacterPartEvent : UnityEvent<CharacterPartCache> {}
        public class ToolChangeEvent : UnityEvent<ITool> {}
        public class RestoreBindPoseEvent : UnityEvent {}
        public class CopyEvent : UnityEvent {}
        public class PasteEvent : UnityEvent<bool, bool, bool, bool> {}
        public class ShortcutEvent : UnityEvent<string> {}
        public class BoneVisibilityEvent : UnityEvent<string> {}
        public class SpriteLibraryEvent : UnityEvent {}
        public class MeshPreviewBehaviourChangeEvent : UnityEvent<IMeshPreviewBehaviour> {}

        private SpriteEvent m_SelectedSpriteChanged = new SpriteEvent();
        private SkeletonEvent m_SkeletonPreviewPoseChanged = new SkeletonEvent();
        private SkeletonEvent m_SkeletonBindPoseChanged = new SkeletonEvent();
        private SkeletonEvent m_SkeletonTopologyChanged = new SkeletonEvent();
        private MeshEvent m_MeshChanged = new MeshEvent();
        private MeshPreviewEvent m_MeshPreviewChanged = new MeshPreviewEvent();
        private SkinningModuleModeEvent m_SkinningModuleModeChanged = new SkinningModuleModeEvent();
        private BoneSelectionEvent m_BoneSelectionChangedEvent = new BoneSelectionEvent();
        private BoneEvent m_BoneNameChangedEvent = new BoneEvent();
        private BoneEvent m_BoneDepthChangedEvent = new BoneEvent();
        private CharacterPartEvent m_CharacterPartChanged = new CharacterPartEvent();
        private ToolChangeEvent m_ToolChanged = new ToolChangeEvent();
        private RestoreBindPoseEvent m_RestoreBindPose = new RestoreBindPoseEvent();
        private CopyEvent m_CopyEvent = new CopyEvent();
        private PasteEvent m_PasteEvent = new PasteEvent();
        private ShortcutEvent m_ShortcutEvent = new ShortcutEvent();
        private BoneVisibilityEvent m_BoneVisibilityEvent = new BoneVisibilityEvent();
        private SpriteLibraryEvent m_SpriteLibraryEvent = new SpriteLibraryEvent();
        private MeshPreviewBehaviourChangeEvent m_MeshPreviewBehaviourChange = new MeshPreviewBehaviourChangeEvent();

        //Setting them as virtual so that we can create mock them
        public virtual SpriteEvent selectedSpriteChanged { get { return m_SelectedSpriteChanged; } }
        public virtual SkeletonEvent skeletonPreviewPoseChanged { get { return m_SkeletonPreviewPoseChanged; } }
        public virtual SkeletonEvent skeletonBindPoseChanged { get { return m_SkeletonBindPoseChanged; } }
        public virtual SkeletonEvent skeletonTopologyChanged { get { return m_SkeletonTopologyChanged; } }
        public virtual MeshEvent meshChanged { get { return m_MeshChanged; } }
        public virtual MeshPreviewEvent meshPreviewChanged { get { return m_MeshPreviewChanged; } }
        public virtual SkinningModuleModeEvent skinningModeChanged { get { return m_SkinningModuleModeChanged; } }
        public virtual BoneSelectionEvent boneSelectionChanged { get { return m_BoneSelectionChangedEvent; } }
        public virtual BoneEvent boneNameChanged { get { return m_BoneNameChangedEvent; } }
        public virtual BoneEvent boneDepthChanged { get { return m_BoneDepthChangedEvent; } }
        public virtual CharacterPartEvent characterPartChanged { get { return m_CharacterPartChanged; } }
        public virtual ToolChangeEvent toolChanged { get { return m_ToolChanged; } }
        public virtual RestoreBindPoseEvent restoreBindPose { get { return m_RestoreBindPose; } }
        public virtual CopyEvent copy { get { return m_CopyEvent; } }
        public virtual PasteEvent paste { get { return m_PasteEvent; } }
        public virtual ShortcutEvent shortcut { get { return m_ShortcutEvent; } }
        public virtual BoneVisibilityEvent boneVisibility { get { return m_BoneVisibilityEvent; } }
        public virtual SpriteLibraryEvent spriteLibraryChanged { get { return m_SpriteLibraryEvent; } }
        public virtual MeshPreviewBehaviourChangeEvent meshPreviewBehaviourChange { get { return m_MeshPreviewBehaviourChange; } }
    }
}
