using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Layout;

namespace UnityEditor.U2D.Animation
{
    internal class SpriteBoneInflueceToolController
    {
        SkinningEvents m_Events;
        ISpriteBoneInfluenceToolModel m_Model;
        public SpriteBoneInflueceToolController(ISpriteBoneInfluenceToolModel model, SkinningEvents events)
        {
            m_Events = events;
            m_Model = model;
        }

        public void Activate()
        {
            m_Events.selectedSpriteChanged.AddListener(OnSpriteSelectionChanged);
            m_Events.boneSelectionChanged.AddListener(OnBoneSelectionChanged);
            m_Events.boneNameChanged.AddListener(OnBoneNameChanged);
            m_Events.skeletonTopologyChanged.AddListener(OnSkeletonTopologyChanged);
            m_Events.meshChanged.AddListener(OnMeshChanged);
            ShowHideView(true);
            OnBoneSelectionChanged();
        }

        public void Deactivate()
        {
            m_Events.selectedSpriteChanged.RemoveListener(OnSpriteSelectionChanged);
            m_Events.boneSelectionChanged.RemoveListener(OnBoneSelectionChanged);
            m_Events.boneNameChanged.RemoveListener(OnBoneNameChanged);
            m_Events.skeletonTopologyChanged.RemoveListener(OnSkeletonTopologyChanged);
            m_Events.meshChanged.RemoveListener(OnMeshChanged);
            ShowHideView(false);
        }

        private void OnMeshChanged(MeshCache mesh)
        {
            if (m_Model.view.visible)
                m_Model.view.OnMeshChanged();
        }

        private void OnSpriteSelectionChanged(SpriteCache sprite)
        {
            if (m_Model.view.visible)
            {
                m_Model.view.OnSpriteSelectionChanged();
                SetViewHeaderText();
            }
        }

        private void OnBoneSelectionChanged()
        {
            if (m_Model.view.visible)
                m_Model.view.OnBoneSelectionChanged();
        }

        private void OnBoneNameChanged(BoneCache bone)
        {
            if (m_Model.view.visible)
            {
                m_Model.view.OnSkeletonChanged();
            }
        }

        private void OnSkeletonTopologyChanged(SkeletonCache skeleton)
        {
            if (m_Model.view.visible)
                m_Model.view.OnSkeletonChanged();
        }

        public void OnViewCreated()
        {
            m_Model.view.onAddBone += AddSelectedBoneInfluencetoSprite;
            m_Model.view.onRemoveBone += RemoveSelectedBoneInfluenceFromSprite;
            m_Model.view.onReordered += OnReorderBoneInfluenceFromSprite;
            m_Model.view.onSelectionChanged += SelectBones;
            m_Model.view.SetController(this);
            ShowHideView(false);
        }

        private void AddSelectedBoneInfluencetoSprite()
        {
            var character = m_Model.characterSkeleton;

            if (character == null)
                return;

            var characterPart = m_Model.GetSpriteCharacterPart(m_Model.selectedSprite);
            var selectedBones = m_Model.selectedBones;
            var characterBones = characterPart.bones.ToList();

            foreach (var bone in selectedBones)
            {
                if (!characterBones.Contains(bone))
                    characterBones.Add(bone);
            }

            using (m_Model.UndoScope(TextContent.addBoneInfluence))
            {
                characterPart.bones = characterBones.ToArray();
                m_Events.characterPartChanged.Invoke(characterPart);
                m_Model.view.OnSkeletonChanged();
                m_Model.view.OnBoneSelectionChanged();
            }
        }

        private void RemoveSelectedBoneInfluenceFromSprite()
        {
            var character = m_Model.characterSkeleton;

            if (character == null)
                return;

            var characterPart = m_Model.GetSpriteCharacterPart(m_Model.selectedSprite);
            var selectedBones = m_Model.selectedBones;
            var characterBones = characterPart.bones.ToList();

            characterBones.RemoveAll(b => selectedBones.Contains(b));

            using (m_Model.UndoScope(TextContent.removeBoneInfluence))
            {
                characterPart.bones = characterBones.ToArray();
                m_Events.characterPartChanged.Invoke(characterPart);
                
                characterPart.sprite.SmoothFill();
                m_Events.meshChanged.Invoke(characterPart.sprite.GetMesh());

                m_Model.view.OnSkeletonChanged();
                m_Model.view.OnBoneSelectionChanged();
            }
        }

        private void OnReorderBoneInfluenceFromSprite(IEnumerable<BoneCache> boneCache)
        {
            var character = m_Model.characterSkeleton;

            if (character != null)
            {
                var characterPart = m_Model.GetSpriteCharacterPart(m_Model.selectedSprite);
                using (m_Model.UndoScope(TextContent.reorderBoneInfluence))
                {
                    characterPart.bones = boneCache.ToArray();
                    m_Events.characterPartChanged.Invoke(characterPart);
                    m_Model.view.OnSkeletonChanged();
                }
            }
            else
            {
                using (m_Model.UndoScope(TextContent.reorderBoneInfluence))
                {
                    m_Model.selectedSprite.GetSkeleton().ReorderBones(boneCache);
                    m_Events.skeletonTopologyChanged.Invoke(m_Model.selectedSprite.GetSkeleton());
                }
            }
        }

        private void SelectBones(IEnumerable<BoneCache> selectedBones)
        {
            using (m_Model.UndoScope(TextContent.boneSelection))
            {
                m_Model.selectedBones = selectedBones;
                m_Events.boneSelectionChanged.Invoke();
            }
        }

        private void ShowHideView(bool show)
        {
            m_Model.view.SetHiddenFromLayout(!show);
            if (show)
            {
                m_Model.view.OnSpriteSelectionChanged();
                SetViewHeaderText();
            }
        }

        private void SetViewHeaderText()
        {
            var headerText = m_Model.selectedSprite != null ? m_Model.selectedSprite.name : TextContent.noSpriteSelected;
            m_Model.view.headerText = headerText;
        }

        public BoneCache[] GetSelectedSpriteBoneInfluence()
        {
            var selectedSprite = m_Model.selectedSprite;

            if (selectedSprite != null)
            {
                if (m_Model.hasCharacter)
                {
                    var characterPart = m_Model.GetSpriteCharacterPart(selectedSprite);
                    return characterPart.bones.ToArray();
                }
                else
                {
                    return selectedSprite.GetSkeleton().bones;
                }
            }
            return new BoneCache[0];
        }

        public int[] GetSelectedBoneForList(IEnumerable<BoneCache> bones)
        {
            var selectedBones = m_Model.selectedBones;
            var spriteBones = GetSelectedSpriteBoneInfluence();
            var result = new List<int>();
            foreach (var bone in selectedBones)
            {
                var index = Array.IndexOf(spriteBones, bone);
                if (index >= 0)
                    result.Add(index);
            }
            return result.ToArray();
        }

        public bool ShouldEnableAddButton(IEnumerable<BoneCache> bones)
        {
            if (InCharacterMode())
            {
                var hasSelectedSprite = m_Model.selectedSprite != null;
                var selectedBones = m_Model.selectedBones;
                return hasSelectedSprite && selectedBones.FirstOrDefault(x => !bones.Contains(x)) != null;
            }
            return false;
        }

        public bool InCharacterMode()
        {
            return m_Model.hasCharacter && m_Model.skinningMode == SkinningMode.Character;
        }
    }

    internal interface ISpriteBoneInfluenceToolModel
    {
        ISpriteBoneInfluenceWindow view { get; }
        IEnumerable<BoneCache> selectedBones { get; set; }
        SpriteCache selectedSprite { get; }
        bool hasCharacter { get; }
        SkinningMode skinningMode { get; }
        SkeletonCache characterSkeleton { get; }
        UndoScope UndoScope(string description);
        CharacterPartCache GetSpriteCharacterPart(SpriteCache sprite);
    }

    internal class SpriteBoneInfluenceTool : BaseTool, ISpriteBoneInfluenceToolModel
    {
        SpriteBoneInflueceToolController m_Controller;
        private MeshPreviewBehaviour m_MeshPreviewBehaviour = new MeshPreviewBehaviour();
        private SpriteBoneInfluenceWindow m_View;

        public SkeletonTool skeletonTool { set; private get; }
        public override IMeshPreviewBehaviour previewBehaviour
        {
            get { return m_MeshPreviewBehaviour; }
        }

        internal override void OnCreate()
        {
            m_Controller = new SpriteBoneInflueceToolController(this, skinningCache.events);
        }

        ISpriteBoneInfluenceWindow ISpriteBoneInfluenceToolModel.view {get { return m_View; } }

        IEnumerable<BoneCache> ISpriteBoneInfluenceToolModel.selectedBones
        {
            get { return skinningCache.skeletonSelection.elements; }
            set { skinningCache.skeletonSelection.elements = value.ToArray(); }
        }
        SpriteCache ISpriteBoneInfluenceToolModel.selectedSprite { get { return skinningCache.selectedSprite; } }
        bool ISpriteBoneInfluenceToolModel.hasCharacter { get { return skinningCache.hasCharacter; } }
        SkinningMode ISpriteBoneInfluenceToolModel.skinningMode { get { return skinningCache.mode; } }
        SkeletonCache ISpriteBoneInfluenceToolModel.characterSkeleton { get { return skinningCache.character != null ? skinningCache.character.skeleton : null; } }

        UndoScope ISpriteBoneInfluenceToolModel.UndoScope(string description)
        {
            return skinningCache.UndoScope(description);
        }

        protected override void OnActivate()
        {
            m_Controller.Activate();
            if (skeletonTool != null)
                skeletonTool.Activate();
        }

        protected override void OnDeactivate()
        {
            m_Controller.Deactivate();
            if (skeletonTool != null)
                skeletonTool.Deactivate();
        }

        public override void Initialize(LayoutOverlay layout)
        {
            if (m_View == null)
            {
                m_View = SpriteBoneInfluenceWindow.CreateFromUXML();
                m_Controller.OnViewCreated();
            }

            layout.rightOverlay.Add(m_View);
        }

        protected override void OnGUI()
        {
            m_MeshPreviewBehaviour.showWeightMap = true;
            m_MeshPreviewBehaviour.overlaySelected = true;
            m_MeshPreviewBehaviour.drawWireframe = true;

            skeletonTool.skeletonStyle = SkeletonStyles.WeightMap;
            skeletonTool.mode = SkeletonMode.EditPose;
            skeletonTool.editBindPose = false;
            skeletonTool.DoGUI();
        }

        public CharacterPartCache GetSpriteCharacterPart(SpriteCache sprite)
        {
            return sprite.GetCharacterPart();
        }
    }
}
