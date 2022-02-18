using System;
using UnityEditor.U2D.Layout;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class SkeletonTool : BaseTool
    {
        [SerializeField]
        private SkeletonController m_SkeletonController;
        private SkeletonToolView m_SkeletonToolView;
        private RectBoneSelector m_RectBoneSelector = new RectBoneSelector();
        private RectSelectionTool<BoneCache> m_RectSelectionTool = new RectSelectionTool<BoneCache>();
        private UnselectTool<BoneCache> m_UnselectTool = new UnselectTool<BoneCache>();

        public bool enableBoneInspector { get; set; }

        public SkeletonMode mode
        {
            get { return m_SkeletonController.view.mode; }
            set { m_SkeletonController.view.mode = value; }
        }

        public bool editBindPose
        {
            get { return m_SkeletonController.editBindPose; }
            set { m_SkeletonController.editBindPose = value; }
        }

        public ISkeletonStyle skeletonStyle
        {
            get { return m_SkeletonController.styleOverride; }
            set { m_SkeletonController.styleOverride = value; }
        }

        public override int defaultControlID
        {
            get { return 0; }
        }

        public BoneCache hoveredBone
        {
            get { return m_SkeletonController.hoveredBone; }
        }

        public SkeletonCache skeleton
        {
            get { return m_SkeletonController.skeleton; }
            private set { m_SkeletonController.skeleton = value; }
        }

        internal override void OnCreate()
        {
            m_SkeletonController = new SkeletonController();
            m_SkeletonController.view = new SkeletonView(new GUIWrapper());
            m_SkeletonController.view.InvalidID = 0;
            m_SkeletonController.selection = skinningCache.skeletonSelection;
            m_SkeletonToolView = new SkeletonToolView();
            m_SkeletonToolView.onBoneNameChanged += BoneNameChanged;
            m_SkeletonToolView.onBoneDepthChanged += BoneDepthChanged;
            m_RectBoneSelector.selection = skinningCache.skeletonSelection;
            m_RectSelectionTool.rectSelector = m_RectBoneSelector;
            m_RectSelectionTool.cacheUndo = skinningCache;
            m_RectSelectionTool.onSelectionUpdate += () =>
            {
                skinningCache.events.boneSelectionChanged.Invoke();
            };
            m_UnselectTool.cacheUndo = skinningCache;
            m_UnselectTool.selection = skinningCache.skeletonSelection;
            m_UnselectTool.onUnselect += () =>
            {
                skinningCache.events.boneSelectionChanged.Invoke();
            };
        }

        public override void Initialize(LayoutOverlay layout)
        {
            m_SkeletonToolView.Initialize(layout);
        }

        protected override void OnActivate()
        {
            SetupSkeleton(skinningCache.GetEffectiveSkeleton(skinningCache.selectedSprite));
            UpdateBoneInspector();
            skinningCache.events.boneSelectionChanged.AddListener(BoneSelectionChanged);
            skinningCache.events.selectedSpriteChanged.AddListener(SelectedSpriteChanged);
            skinningCache.events.skinningModeChanged.AddListener(SkinningModeChanged);
            skinningCache.events.boneDepthChanged.AddListener(BoneDataChanged);
            skinningCache.events.boneNameChanged.AddListener(BoneDataChanged);
            skeletonStyle = null;
        }

        protected override void OnDeactivate()
        {
            m_SkeletonToolView.Hide();
            m_SkeletonController.Reset();
            skinningCache.events.boneSelectionChanged.RemoveListener(BoneSelectionChanged);
            skinningCache.events.selectedSpriteChanged.RemoveListener(SelectedSpriteChanged);
            skinningCache.events.skinningModeChanged.RemoveListener(SkinningModeChanged);
            skinningCache.events.boneDepthChanged.RemoveListener(BoneDataChanged);
            skinningCache.events.boneNameChanged.RemoveListener(BoneDataChanged);
            skeletonStyle = null;
        }

        void BoneDataChanged(BoneCache bone)
        {
            if(m_SkeletonToolView.target == bone)
                m_SkeletonToolView.Update(bone.name, Mathf.RoundToInt(bone.depth));
        }

        private void SelectedSpriteChanged(SpriteCache sprite)
        {
            SetupSkeleton(skinningCache.GetEffectiveSkeleton(sprite));
        }

        private void BoneSelectionChanged()
        {
            UpdateBoneInspector();
        }

        private void UpdateBoneInspector()
        {
            var selectedBone = skinningCache.skeletonSelection.activeElement;
            var selectionCount = skinningCache.skeletonSelection.Count;

            m_SkeletonToolView.Hide();

            if (enableBoneInspector && selectedBone != null && selectionCount == 1)
            {
                m_SkeletonToolView.Update(selectedBone.name, Mathf.RoundToInt(selectedBone.depth));
                m_SkeletonToolView.Show(selectedBone);
            }
        }

        private void SkinningModeChanged(SkinningMode mode)
        {
            SetupSkeleton(skinningCache.GetEffectiveSkeleton(skinningCache.selectedSprite));
        }

        private void SetupSkeleton(SkeletonCache sk)
        {
            m_RectBoneSelector.bones = null;
            skeleton = sk;

            if (skeleton != null)
                m_RectBoneSelector.bones = skeleton.bones;
        }

        protected override void OnGUI()
        {
            m_SkeletonController.view.defaultControlID = 0;

            if (skeleton != null && mode != SkeletonMode.Disabled)
            {
                m_RectSelectionTool.OnGUI();
                m_SkeletonController.view.defaultControlID = m_RectSelectionTool.controlID;
            }

            m_SkeletonController.OnGUI();
            m_UnselectTool.OnGUI();
        }

        private void BoneNameChanged(BoneCache selectedBone, string name)
        {
            if (selectedBone != null)
            {
                if (string.Compare(selectedBone.name, name) == 0)
                    return;

                if(string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                    m_SkeletonToolView.Update(selectedBone.name, Mathf.RoundToInt(selectedBone.depth));
                else
                {
                    using (skinningCache.UndoScope(TextContent.boneName))
                    {
                        selectedBone.name = name;
                        skinningCache.events.boneNameChanged.Invoke(selectedBone);
                    }
                }
            }
        }

        private void BoneDepthChanged(BoneCache selectedBone, int depth)
        {
            if (selectedBone != null)
            {
                if (Mathf.RoundToInt(selectedBone.depth) == depth)
                    return;

                using (skinningCache.UndoScope(TextContent.boneDepth))
                {
                    selectedBone.depth = depth;
                    skinningCache.events.boneDepthChanged.Invoke(selectedBone);
                }   
            }
        }
    }
}
