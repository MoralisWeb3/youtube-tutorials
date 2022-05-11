using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor.U2D.Layout;
using UnityEditor.U2D.Sprites;

using UnityEngine.U2D.Common;
using Debug = UnityEngine.Debug;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class SkinningObject : CacheObject
    {
        public SkinningCache skinningCache
        {
            get { return owner as SkinningCache; }
        }
    }

    internal class SkinningCache : Cache
    {
        [Serializable]
        private class SpriteMap : SerializableDictionary<string, SpriteCache> {}
        [Serializable]
        private class MeshMap : SerializableDictionary<SpriteCache, MeshCache> {}
        [Serializable]
        private class SkeletonMap : SerializableDictionary<SpriteCache, SkeletonCache> {}
        [Serializable]
        private class ToolMap : SerializableDictionary<Tools, BaseTool> {}
        [Serializable]
        private class MeshPreviewMap : SerializableDictionary<SpriteCache, MeshPreviewCache> {}
        [Serializable]
        private class CharacterPartMap : SerializableDictionary<SpriteCache, CharacterPartCache> {}

        [SerializeField]
        private SkinningEvents m_Events = new SkinningEvents();
        [SerializeField]
        private List<BaseTool> m_Tools = new List<BaseTool>();
        [SerializeField]
        private SpriteMap m_SpriteMap = new SpriteMap();
        [SerializeField]
        private MeshMap m_MeshMap = new MeshMap();
        [SerializeField]
        private MeshPreviewMap m_MeshPreviewMap = new MeshPreviewMap();
        [SerializeField]
        private SkeletonMap m_SkeletonMap = new SkeletonMap();
        [SerializeField]
        private CharacterPartMap m_CharacterPartMap = new CharacterPartMap();
        [SerializeField]
        private ToolMap m_ToolMap = new ToolMap();
        [SerializeField]
        private SelectionTool m_SelectionTool;
        [SerializeField]
        private CharacterCache m_Character;
        [SerializeField]
        private SkinningMode m_Mode = SkinningMode.SpriteSheet;
        [SerializeField]
        private BaseTool m_SelectedTool;
        [SerializeField]
        private SpriteCache m_SelectedSprite;
        [SerializeField]
        private SkeletonSelection m_SkeletonSelection = new SkeletonSelection();
        [SerializeField]
        private SpriteCategoryListCacheObject m_SpriteCategoryList;
        [SerializeField] 
        private ISkinningCachePersistentState m_State;

        private StringBuilder m_StringBuilder = new StringBuilder();

        public BaseTool selectedTool
        {
            get { return m_SelectedTool; }
            set
            {
                m_SelectedTool = value;
                try
                {
                    m_State.lastUsedTool = m_ToolMap[value];
                }
                catch (KeyNotFoundException)
                {
                    m_State.lastUsedTool = Tools.EditPose;
                }
            }
        }

        public virtual SkinningMode mode
        {
            get { return m_Mode; }
            set
            {
                m_Mode = CheckModeConsistency(value);
                m_State.lastMode = m_Mode;
            }
        }

        public SpriteCache selectedSprite
        {
            get { return m_SelectedSprite; }
            set
            {
                m_SelectedSprite = value;
                m_State.lastSpriteId = m_SelectedSprite ? m_SelectedSprite.id : String.Empty;
            }
        }

        public float brushSize
        {
            get { return m_State.lastBrushSize; }
            set { m_State.lastBrushSize = value; }
        }

        public float brushHardness
        {
            get { return m_State.lastBrushHardness; }
            set { m_State.lastBrushHardness = value; }
        }

        public float brushStep
        {
            get { return m_State.lastBrushStep; }
            set { m_State.lastBrushStep = value; }
        }

        public int visibililtyToolIndex
        {
            get { return m_State.lastVisibilityToolIndex; }
            set { m_State.lastVisibilityToolIndex = value; }
        }
        
        public SkeletonSelection skeletonSelection
        {
            get { return m_SkeletonSelection; }
        }

        public IndexedSelection vertexSelection
        {
            get { return m_State.lastVertexSelection; }
        }

        public SkinningEvents events
        {
            get { return m_Events; }
        }

        public SelectionTool selectionTool
        {
            get { return m_SelectionTool; }
        }

        public SpriteCache[] GetSprites()
        {
            return m_SpriteMap.Values.ToArray();
        }

        public virtual CharacterCache character
        {
            get { return m_Character; }
        }

        public bool hasCharacter
        {
            get { return character != null; }
        }

        public bool applyingChanges
        {
            get;
            set;
        }

        private SkinningMode CheckModeConsistency(SkinningMode mode)
        {
            if (mode == SkinningMode.Character && hasCharacter == false)
                mode = SkinningMode.SpriteSheet;

            return mode;
        }

        public void Create(ISpriteEditor spriteEditor, ISkinningCachePersistentState state)
        {
            Clear();

            var dataProvider = spriteEditor.GetDataProvider<ISpriteEditorDataProvider>();
            var boneProvider = spriteEditor.GetDataProvider<ISpriteBoneDataProvider>();
            var meshProvider = spriteEditor.GetDataProvider<ISpriteMeshDataProvider>();
            var spriteRects = dataProvider.GetSpriteRects();
            var textureProvider = spriteEditor.GetDataProvider<ITextureDataProvider>();

            m_State = state;
            m_State.lastTexture = textureProvider.texture;

            for (var i = 0; i < spriteRects.Length; i++)
            {
                var spriteRect = spriteRects[i];
                var sprite = CreateSpriteCache(spriteRect);
                CreateSkeletonCache(sprite, boneProvider);
                CreateMeshCache(sprite, meshProvider, textureProvider);
                CreateMeshPreviewCache(sprite);
            }

            CreateCharacter(spriteEditor);
            CreateSpriteLibrary(spriteEditor);
        }

        public void CreateToolCache(ISpriteEditor spriteEditor, LayoutOverlay layoutOverlay)
        {
            var spriteEditorDataProvider = spriteEditor.GetDataProvider<ISpriteEditorDataProvider>();
            var skeletonTool = CreateCache<SkeletonTool>();
            var meshTool = CreateCache<MeshTool>();

            skeletonTool.Initialize(layoutOverlay);
            meshTool.Initialize(layoutOverlay);

            m_ToolMap.Add(Tools.EditPose, CreateSkeletonTool<SkeletonToolWrapper>(skeletonTool, SkeletonMode.EditPose, false, layoutOverlay));
            m_ToolMap.Add(Tools.EditJoints, CreateSkeletonTool<SkeletonToolWrapper>(skeletonTool, SkeletonMode.EditJoints, true, layoutOverlay));
            m_ToolMap.Add(Tools.CreateBone, CreateSkeletonTool<SkeletonToolWrapper>(skeletonTool, SkeletonMode.CreateBone, true, layoutOverlay));
            m_ToolMap.Add(Tools.SplitBone, CreateSkeletonTool<SkeletonToolWrapper>(skeletonTool, SkeletonMode.SplitBone, true, layoutOverlay));
            m_ToolMap.Add(Tools.ReparentBone, CreateSkeletonTool<BoneReparentTool>(skeletonTool, SkeletonMode.EditPose, false, layoutOverlay));

            m_ToolMap.Add(Tools.EditGeometry, CreateMeshTool<MeshToolWrapper>(skeletonTool, meshTool, SpriteMeshViewMode.EditGeometry, SkeletonMode.Disabled, layoutOverlay));
            m_ToolMap.Add(Tools.CreateVertex, CreateMeshTool<MeshToolWrapper>(skeletonTool, meshTool, SpriteMeshViewMode.CreateVertex, SkeletonMode.Disabled, layoutOverlay));
            m_ToolMap.Add(Tools.CreateEdge, CreateMeshTool<MeshToolWrapper>(skeletonTool, meshTool, SpriteMeshViewMode.CreateEdge, SkeletonMode.Disabled, layoutOverlay));
            m_ToolMap.Add(Tools.SplitEdge, CreateMeshTool<MeshToolWrapper>(skeletonTool, meshTool, SpriteMeshViewMode.SplitEdge, SkeletonMode.Disabled, layoutOverlay));
            m_ToolMap.Add(Tools.GenerateGeometry, CreateMeshTool<GenerateGeometryTool>(skeletonTool, meshTool, SpriteMeshViewMode.EditGeometry, SkeletonMode.EditPose, layoutOverlay));
            var copyTool = CreateTool<CopyTool>();
            copyTool.Initialize(layoutOverlay);
            copyTool.pixelsPerUnit = spriteEditorDataProvider.pixelsPerUnit;
            copyTool.skeletonTool = skeletonTool;
            copyTool.meshTool = meshTool;
            m_ToolMap.Add(Tools.CopyPaste, copyTool);

            CreateWeightTools(skeletonTool, meshTool, layoutOverlay);

            m_SelectionTool = CreateTool<SelectionTool>();
            m_SelectionTool.spriteEditor = spriteEditor;
            m_SelectionTool.Initialize(layoutOverlay);
            m_SelectionTool.Activate();

            var visibilityTool = CreateTool<VisibilityTool>();
            visibilityTool.Initialize(layoutOverlay);
            visibilityTool.skeletonTool = skeletonTool;
            m_ToolMap.Add(Tools.Visibility, visibilityTool);

            var switchModeTool = CreateTool<SwitchModeTool>();
            m_ToolMap.Add(Tools.SwitchMode, switchModeTool);
        }

        public void RestoreFromPersistentState()
        {
            mode = m_State.lastMode;
            events.skinningModeChanged.Invoke(mode);

            SpriteCache lastSprite;
            var hasLastSprite = m_SpriteMap.TryGetValue(m_State.lastSpriteId, out lastSprite);
            if (hasLastSprite)
            {
                selectedSprite = lastSprite;
            }
            else
            {
                vertexSelection.Clear();
            }

            BaseTool baseTool;
            if (m_ToolMap.TryGetValue(m_State.lastUsedTool, out baseTool))
            {
                selectedTool = baseTool;
            }
            else if (m_ToolMap.TryGetValue(Tools.EditPose, out baseTool))
            {
                selectedTool = baseTool;
            }

            var visibilityTool = m_ToolMap[Tools.Visibility];
            if (m_State.lastVisibilityToolActive)
            {
                visibilityTool.Activate();
            }
        }

        public void RestoreToolStateFromPersistentState()
        {
            events.boneSelectionChanged.RemoveListener(BoneSelectionChanged);
            events.skeletonPreviewPoseChanged.RemoveListener(SkeletonPreviewPoseChanged);
            events.toolChanged.RemoveListener(ToolChanged);

            SkeletonCache skeleton = null;
            if (hasCharacter)
                skeleton = character.skeleton;
            else if (selectedSprite != null)
                skeleton = selectedSprite.GetSkeleton();

            skeletonSelection.Clear();
            if (skeleton != null && m_State.lastBoneSelectionIds.Count > 0)
            {
                bool selectionChanged = false;
                foreach (var bone in skeleton.bones)
                {
                    var id = GetBoneNameHash(m_StringBuilder, bone);
                    if (m_State.lastBoneSelectionIds.Contains(id))
                    {
                        skeletonSelection.Select(bone, true);
                        selectionChanged = true;
                    }
                }
                if (selectionChanged)
                    events.boneSelectionChanged.Invoke();
            }

            if (m_State.lastPreviewPose.Count > 0)
            {
                if (hasCharacter)
                {
                    UpdatePoseFromPersistentState(character.skeleton, null);
                }
                foreach (var sprite in m_SkeletonMap.Keys)
                {
                    UpdatePoseFromPersistentState(m_SkeletonMap[sprite], sprite);
                }
            }

            if (m_State.lastBoneVisibility.Count > 0)
            {
                if (hasCharacter)
                {
                    UpdateVisibilityFromPersistentState(character.skeleton, null);
                }

                foreach (var sprite in m_SkeletonMap.Keys)
                {
                    UpdateVisibilityFromPersistentState(m_SkeletonMap[sprite], sprite);
                }
            }

            if (m_State.lastSpriteVisibility.Count > 0 && hasCharacter)
            {
                foreach (var characterPart in character.parts)
                {
                    if (m_State.lastSpriteVisibility.TryGetValue(characterPart.sprite.id, out var visibility))
                    {
                        characterPart.isVisible = visibility;
                    }
                }

                foreach (var characterGroup in character.groups)
                {
                    var groupHash = GetCharacterGroupHash(m_StringBuilder, characterGroup, character);
                    if (m_State.lastGroupVisibility.TryGetValue(groupHash, out var visibility))
                    {
                        characterGroup.isVisible = visibility;
                    }
                }
            }

            events.boneSelectionChanged.AddListener(BoneSelectionChanged);
            events.skeletonPreviewPoseChanged.AddListener(SkeletonPreviewPoseChanged);
            events.toolChanged.AddListener(ToolChanged);
        }

        private void UpdatePoseFromPersistentState(SkeletonCache skeleton, SpriteCache sprite)
        {
            bool poseChanged = false;
            foreach (var bone in skeleton.bones)
            {
                var id = GetBoneNameHash(m_StringBuilder, bone, sprite);
                BonePose pose;
                if (m_State.lastPreviewPose.TryGetValue(id, out pose))
                {
                    bone.localPose = pose;
                    poseChanged = true;
                }
            }
            if (poseChanged)
            {
                skeleton.SetPosePreview();
                events.skeletonPreviewPoseChanged.Invoke(skeleton);    
            }
        }

        private void UpdateVisibilityFromPersistentState(SkeletonCache skeleton, SpriteCache sprite)
        {
            foreach (var bone in skeleton.bones)
            {
                var id = GetBoneNameHash(m_StringBuilder, bone, sprite);
                if (m_State.lastBoneVisibility.TryGetValue(id, out var visibility))
                {
                    bone.isVisible = visibility;
                }
            }
        }

        private const string kNameSeparator = "/";

        private int GetBoneNameHash(StringBuilder sb, BoneCache bone, SpriteCache sprite = null)
        {
            sb.Clear();
            BuildBoneName(sb, bone);
            sb.Append(kNameSeparator);
            if (sprite != null)
            {
                sb.Append(sprite.id);
            }
            else
            {
                sb.Append(0);
            }
            return Animator.StringToHash(sb.ToString());
        }
        
        private void BuildBoneName(StringBuilder sb, BoneCache bone)
        {
            if (bone.parentBone != null)
            {
                BuildBoneName(sb, bone.parentBone);
                sb.Append(kNameSeparator);
            }
            sb.Append(bone.name);
        }

        private int GetCharacterGroupHash(StringBuilder sb, CharacterGroupCache characterGroup, CharacterCache characterCache)
        {
            sb.Clear();
            BuildGroupName(sb, characterGroup, characterCache);
            return Animator.StringToHash(sb.ToString());
        }

        private void BuildGroupName(StringBuilder sb, CharacterGroupCache group, CharacterCache characterCache)
        {
            if (group.parentGroup >= 0 && group.parentGroup < characterCache.groups.Length)
            {
                BuildGroupName(sb, characterCache.groups[group.parentGroup], characterCache);
                sb.Append(kNameSeparator);
            }
            sb.Append(group.order);
        }

        private void BoneSelectionChanged()
        {
            m_State.lastBoneSelectionIds.Clear();
            m_State.lastBoneSelectionIds.Capacity = skeletonSelection.elements.Length;
            for (var i = 0; i < skeletonSelection.elements.Length; ++i)
            {
                var bone = skeletonSelection.elements[i];
                m_State.lastBoneSelectionIds.Add(GetBoneNameHash(m_StringBuilder, bone));
            }
        }

        private void SkeletonPreviewPoseChanged(SkeletonCache sc)
        {
            if (applyingChanges)
                return;

            m_State.lastPreviewPose.Clear();
            if (hasCharacter)
            {
                StorePersistentStatePoseForSkeleton(character.skeleton, null);
            }
            foreach (var sprite in m_SkeletonMap.Keys)
            {
                StorePersistentStatePoseForSkeleton(m_SkeletonMap[sprite], sprite);
            }
        }

        private void StorePersistentStatePoseForSkeleton(SkeletonCache skeleton, SpriteCache sprite)
        {
            foreach (var bone in skeleton.bones)
            {
                var id = GetBoneNameHash(m_StringBuilder, bone, sprite);
                if (bone.NotInDefaultPose())
                {
                    m_State.lastPreviewPose[id] = bone.localPose;
                }
            }
        }

        internal void Revert()
        {
            m_State.lastVertexSelection.Clear();
        }

        internal void BoneVisibilityChanged(SkeletonCache sc)
        {
            if (applyingChanges)
                return;

            m_State.lastBoneVisibility.Clear();
            if (hasCharacter)
            {
                StorePersistentStateVisibilityForSkeleton(character.skeleton, null);
            }
            foreach (var sprite in m_SkeletonMap.Keys)
            {
                StorePersistentStateVisibilityForSkeleton(m_SkeletonMap[sprite], sprite);
            }
        }

        private void StorePersistentStateVisibilityForSkeleton(SkeletonCache skeleton, SpriteCache sprite)
        {
            foreach (var bone in skeleton.bones)
            {
                var id = GetBoneNameHash(m_StringBuilder, bone, sprite);
                m_State.lastBoneVisibility[id] = bone.isVisible;
            }
        }

        internal void BoneExpansionChanged(BoneCache[] boneCaches)
        {
            if (applyingChanges)
                return;

            m_State.lastBoneExpansion.Clear();
            if (hasCharacter)
            {
                foreach (var bone in boneCaches)
                {
                    if (character.skeleton.bones.Contains(bone))
                    {
                        var id = GetBoneNameHash(m_StringBuilder, bone, null);
                        m_State.lastBoneExpansion[id] = true;    
                    }
                }
            }

            foreach (var sprite in m_SkeletonMap.Keys)
            {
                var skeleton = m_SkeletonMap[sprite];
                foreach (var bone in boneCaches)
                {
                    if (skeleton.bones.Contains(bone))
                    {
                        var id = GetBoneNameHash(m_StringBuilder, bone, sprite);
                        m_State.lastBoneExpansion[id] = true;    
                    }
                }
            }
        }

        internal BoneCache[] GetExpandedBones()
        {
            HashSet<BoneCache> expandedBones = new HashSet<BoneCache>();
            if (m_State.lastBoneExpansion.Count > 0)
            {
                if (hasCharacter)
                {
                    foreach (var bone in character.skeleton.bones)
                    {
                        var id = GetBoneNameHash(m_StringBuilder, bone, null);
                        if (m_State.lastBoneExpansion.TryGetValue(id, out var expanded))
                        {
                            expandedBones.Add(bone);
                        }    
                    }
                }
                foreach (var sprite in m_SkeletonMap.Keys)
                {
                    var skeleton = m_SkeletonMap[sprite];
                    foreach (var bone in skeleton.bones)
                    {
                        var id = GetBoneNameHash(m_StringBuilder, bone, sprite);
                        if (m_State.lastBoneExpansion.TryGetValue(id, out var expanded))
                        {
                            expandedBones.Add(bone);
                        }    
                    }
                }
            }
            return expandedBones.ToArray();
        }

        internal void SpriteVisibilityChanged(CharacterPartCache cc)
        {
            m_State.lastSpriteVisibility[cc.sprite.id] = cc.isVisible;
        }

        internal void GroupVisibilityChanged(CharacterGroupCache gc)
        {
            if (!hasCharacter)
                return;

            var groupHash = GetCharacterGroupHash(m_StringBuilder, gc, character);
            m_State.lastGroupVisibility[groupHash] = gc.isVisible;
        }
        
        public void Clear()
        {
            Destroy();
            m_Tools.Clear();
            m_SpriteMap.Clear();
            m_MeshMap.Clear();
            m_MeshPreviewMap.Clear();
            m_SkeletonMap.Clear();
            m_ToolMap.Clear();
            m_CharacterPartMap.Clear();
        }

        public SpriteCache GetSprite(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            SpriteCache sprite;
            m_SpriteMap.TryGetValue(id, out sprite);
            return sprite;
        }

        public virtual MeshCache GetMesh(SpriteCache sprite)
        {
            if (sprite == null)
                return null;

            MeshCache mesh;
            m_MeshMap.TryGetValue(sprite, out mesh);
            return mesh;
        }

        public virtual MeshPreviewCache GetMeshPreview(SpriteCache sprite)
        {
            if (sprite == null)
                return null;

            MeshPreviewCache meshPreview;
            m_MeshPreviewMap.TryGetValue(sprite, out meshPreview);
            return meshPreview;
        }

        public SkeletonCache GetSkeleton(SpriteCache sprite)
        {
            if (sprite == null)
                return null;

            SkeletonCache skeleton;
            m_SkeletonMap.TryGetValue(sprite, out skeleton);
            return skeleton;
        }

        public virtual CharacterPartCache GetCharacterPart(SpriteCache sprite)
        {
            if (sprite == null)
                return null;

            CharacterPartCache part;
            m_CharacterPartMap.TryGetValue(sprite, out part);
            return part;
        }

        public SkeletonCache GetEffectiveSkeleton(SpriteCache sprite)
        {
            if (mode == SkinningMode.SpriteSheet)
                return GetSkeleton(sprite);

            if (hasCharacter)
                return character.skeleton;

            return null;
        }

        public BaseTool GetTool(Tools tool)
        {
            BaseTool t;
            m_ToolMap.TryGetValue(tool, out t);
            return t;
        }

        public override void BeginUndoOperation(string operationName)
        {
            if (isUndoOperationSet == false)
            {
                base.BeginUndoOperation(operationName);
                undo.RegisterCompleteObjectUndo(m_State, operationName);
            }
        }

        public UndoScope UndoScope(string operationName)
        {
            return UndoScope(operationName, false);
        }

        public UndoScope UndoScope(string operationName, bool incrementGroup)
        {
            return new UndoScope(this, operationName, incrementGroup);
        }

        public DisableUndoScope DisableUndoScope()
        {
            return new DisableUndoScope(this);
        }

        public T CreateTool<T>() where T : BaseTool
        {
            var tool = CreateCache<T>();
            m_Tools.Add(tool);
            return tool;
        }

        private void UpdateCharacterPart(CharacterPartCache characterPart)
        {
            var sprite = characterPart.sprite;
            var characterPartBones = characterPart.bones;
            var newBones = new List<BoneCache>(characterPartBones);
            newBones.RemoveAll(b =>
            {
                return b == null || IsRemoved(b) || b.skeleton != character.skeleton;
            });
            var removedBonesCount = characterPartBones.Length - newBones.Count;

            characterPartBones = newBones.ToArray();
            characterPart.bones = characterPartBones;
            sprite.UpdateMesh(characterPartBones);

            if (removedBonesCount > 0)
                sprite.SmoothFill();
        }

        public void CreateSpriteSheetSkeletons()
        {
            Debug.Assert(character != null);

            using (new DefaultPoseScope(character.skeleton))
            {
                var characterParts = character.parts;

                foreach (var characterPart in characterParts)
                    CreateSpriteSheetSkeleton(characterPart);
            }

            SyncSpriteSheetSkeletons();
        }

        public void SyncSpriteSheetSkeletons()
        {
            Debug.Assert(character != null);

            var characterParts = character.parts;

            foreach (var characterPart in characterParts)
                characterPart.SyncSpriteSheetSkeleton();
        }

        public void CreateSpriteSheetSkeleton(CharacterPartCache characterPart)
        {
            UpdateCharacterPart(characterPart);

            Debug.Assert(character != null);
            Debug.Assert(character.skeleton != null);
            Debug.Assert(character.skeleton.isPosePreview == false);

            var sprite = characterPart.sprite;
            var characterPartBones = characterPart.bones;
            var skeleton = sprite.GetSkeleton();

            Debug.Assert(skeleton != null);

            var spriteBones = characterPartBones.ToSpriteBone(characterPart.localToWorldMatrix);
            skeleton.SetBones(CreateBoneCacheFromSpriteBones(spriteBones, 1.0f), false);

            events.skeletonTopologyChanged.Invoke(skeleton);
        }

        private SpriteCache CreateSpriteCache(SpriteRect spriteRect)
        {
            var sprite = CreateCache<SpriteCache>();
            sprite.name = spriteRect.name;
            sprite.id = spriteRect.spriteID.ToString();
            sprite.textureRect = spriteRect.rect;
            sprite.position = spriteRect.rect.position;
            m_SpriteMap[sprite.id] = sprite;
            return sprite;
        }

        private void CreateSkeletonCache(SpriteCache sprite, ISpriteBoneDataProvider boneProvider)
        {
            var guid = new GUID(sprite.id);
            var skeleton = CreateCache<SkeletonCache>();

            skeleton.position = sprite.textureRect.position;
            skeleton.SetBones(CreateBoneCacheFromSpriteBones(boneProvider.GetBones(guid).ToArray(), 1.0f), false);

            m_SkeletonMap[sprite] = skeleton;
        }

        private void CreateMeshCache(SpriteCache sprite, ISpriteMeshDataProvider meshProvider, ITextureDataProvider textureDataProvider)
        {
            Debug.Assert(m_SkeletonMap.ContainsKey(sprite));

            var guid = new GUID(sprite.id);
            var mesh = CreateCache<MeshCache>();
            var skeleton = m_SkeletonMap[sprite] as SkeletonCache;
            var metaVertices = meshProvider.GetVertices(guid);

            mesh.sprite = sprite;
            mesh.SetCompatibleBoneSet(skeleton.bones);

            foreach (var mv in metaVertices)
            {
                var v = new Vertex2D(mv.position, mv.boneWeight);
                mesh.vertices.Add(v);
            }

            mesh.indices = new List<int>(meshProvider.GetIndices(guid));

            var edges = meshProvider.GetEdges(guid);

            foreach (var e in edges)
                mesh.edges.Add(new Edge(e.x, e.y));

            mesh.textureDataProvider = textureDataProvider;

            m_MeshMap[sprite] = mesh;
        }

        private void CreateMeshPreviewCache(SpriteCache sprite)
        {
            Debug.Assert(sprite != null);
            Debug.Assert(m_MeshPreviewMap.ContainsKey(sprite) == false);

            var meshPreview = CreateCache<MeshPreviewCache>();

            meshPreview.sprite = sprite;
            meshPreview.SetMeshDirty();

            m_MeshPreviewMap.Add(sprite, meshPreview);
        }

        private void CreateCharacter(ISpriteEditor spriteEditor)
        {
            var characterProvider = spriteEditor.GetDataProvider<ICharacterDataProvider>();

            if (characterProvider != null)
            {
                var characterData = characterProvider.GetCharacterData();
                var characterParts = new List<CharacterPartCache>();

                m_Character = CreateCache<CharacterCache>();

                var skeleton = CreateCache<SkeletonCache>();

                skeleton.SetBones(CreateBoneCacheFromSpriteBones(characterData.bones, 1.0f));
                skeleton.position = Vector3.zero;

                var bones = skeleton.bones;

                foreach (var p in characterData.parts)
                {
                    var spriteBones = p.bones != null ? p.bones.ToList() : new List<int>();
                    var characterPartBones = spriteBones.ConvertAll(i => bones[i]).ToArray();
                    var characterPart = CreateCache<CharacterPartCache>();

                    var positionInt = p.spritePosition.position;
                    characterPart.position = new Vector2(positionInt.x, positionInt.y);
                    characterPart.sprite = GetSprite(p.spriteId);
                    characterPart.bones = characterPartBones;
                    characterPart.parentGroup = p.parentGroup;
                    characterPart.order = p.order;
                    
                    var mesh = characterPart.sprite.GetMesh();
                    if (mesh != null)
                        mesh.SetCompatibleBoneSet(characterPartBones);

                    characterParts.Add(characterPart);

                    m_CharacterPartMap.Add(characterPart.sprite, characterPart);
                }
                if (characterData.characterGroups != null)
                {
                    m_Character.groups = characterData.characterGroups.Select(x =>
                    {
                        var group = CreateCache<CharacterGroupCache>();
                        group.name = x.name;
                        group.parentGroup = x.parentGroup;
                        group.order = x.order;
                        return group;
                    }).ToArray();
                }
                else
                {
                    m_Character.groups = new CharacterGroupCache[0];
                }
                
                m_Character.parts = characterParts.ToArray();
                m_Character.skeleton = skeleton;
                m_Character.dimension = characterData.dimension;
                CreateSpriteSheetSkeletons();
            }
        }

        private T CreateSkeletonTool<T>(SkeletonTool skeletonTool, SkeletonMode mode, bool editBindPose, LayoutOverlay layoutOverlay) where T : SkeletonToolWrapper
        {
            var tool = CreateTool<T>();
            tool.skeletonTool = skeletonTool;
            tool.mode = mode;
            tool.editBindPose = editBindPose;
            tool.Initialize(layoutOverlay);
            return tool;
        }

        private void CreateWeightTools(SkeletonTool skeletonTool, MeshTool meshTool, LayoutOverlay layoutOverlay)
        {
            var weightPainterTool = CreateCache<WeightPainterTool>();
            weightPainterTool.Initialize(layoutOverlay);
            weightPainterTool.skeletonTool = skeletonTool;
            weightPainterTool.meshTool = meshTool;

            {
                var tool = CreateTool<SpriteBoneInfluenceTool>();
                tool.Initialize(layoutOverlay);
                tool.skeletonTool = skeletonTool;
                m_ToolMap.Add(Tools.BoneInfluence, tool);
            }

            {
                var tool = CreateTool<WeightPainterToolWrapper>();

                tool.weightPainterTool = weightPainterTool;
                tool.paintMode = WeightPainterMode.Slider;
                tool.title = TextContent.weightSlider;
                tool.Initialize(layoutOverlay);
                m_ToolMap.Add(Tools.WeightSlider, tool);
            }

            {
                var tool = CreateTool<WeightPainterToolWrapper>();

                tool.weightPainterTool = weightPainterTool;
                tool.paintMode = WeightPainterMode.Brush;
                tool.title = TextContent.weightBrush;
                tool.Initialize(layoutOverlay);
                m_ToolMap.Add(Tools.WeightBrush, tool);
            }

            {
                var tool = CreateTool<GenerateWeightsTool>();
                tool.Initialize(layoutOverlay);
                tool.meshTool = meshTool;
                tool.skeletonTool = skeletonTool;
                m_ToolMap.Add(Tools.GenerateWeights, tool);
            }
        }

        private T CreateMeshTool<T>(SkeletonTool skeletonTool, MeshTool meshTool, SpriteMeshViewMode meshViewMode, SkeletonMode skeletonMode, LayoutOverlay layoutOverlay) where T : MeshToolWrapper
        {
            var tool = CreateTool<T>();
            tool.skeletonTool = skeletonTool;
            tool.meshTool = meshTool;
            tool.meshMode = meshViewMode;
            tool.skeletonMode = skeletonMode;
            tool.Initialize(layoutOverlay);
            return tool;
        }

        public void RestoreBindPose()
        {
            var sprites = GetSprites();

            foreach (var sprite in sprites)
                sprite.RestoreBindPose();

            if (character != null)
                character.skeleton.RestoreDefaultPose();
        }

        public void UndoRedoPerformed()
        {
            foreach (var tool in m_Tools)
            {
                if (tool == null)
                    continue;

                if (!tool.isActive)
                    tool.Deactivate();
            }

            foreach (var tool in m_Tools)
            {
                if (tool == null)
                    continue;

                if (tool.isActive)
                    tool.Activate();
            }
        }

        public BoneCache[] CreateBoneCacheFromSpriteBones(UnityEngine.U2D.SpriteBone[] spriteBones, float scale)
        {
            var bones = Array.ConvertAll(spriteBones, b => CreateCache<BoneCache>());

            for (var i = 0; i < spriteBones.Length; ++i)
            {
                var spriteBone = spriteBones[i];
                var bone = bones[i];

                if (spriteBone.parentId >= 0)
                    bone.SetParent(bones[spriteBone.parentId]);

                bone.name = spriteBone.name;
                bone.localLength = spriteBone.length * scale;
                bone.depth = spriteBone.position.z;
                bone.localPosition = (Vector2)spriteBone.position * scale;
                bone.localRotation = spriteBone.rotation;
                bone.bindPoseColor = ModuleUtility.CalculateNiceColor(i, 6);
            }

            foreach (var bone in bones)
            {
                if (bone.parentBone != null && bone.parentBone.localLength > 0f && (bone.position - bone.parentBone.endPosition).sqrMagnitude < 0.005f)
                    bone.parentBone.chainedChild = bone;
            }

            return bones;
        }

        public bool IsOnVisualElement()
        {
            if (selectedTool == null || selectedTool.layoutOverlay == null)
                return false;

            var overlay = selectedTool.layoutOverlay;
            var point = InternalEngineBridge.GUIUnclip(Event.current.mousePosition);
            point = overlay.parent.parent.LocalToWorld(point);

            var selectedElement = selectedTool.layoutOverlay.panel.Pick(point);
            if (selectedElement != null
                && selectedElement.pickingMode != PickingMode.Ignore
                && selectedElement.FindCommonAncestor(overlay) == overlay)
                return true;

            return false;
        }

        void CreateSpriteLibrary(ISpriteEditor spriteEditor)
        {
            var dataProvider = spriteEditor.GetDataProvider<ISpriteLibDataProvider>();
            if (dataProvider != null && hasCharacter)
            {
                m_SpriteCategoryList = CreateCache<SpriteCategoryListCacheObject>();
                var categoryList = dataProvider.GetSpriteCategoryList();

                var spriteIds = m_Character.parts.Select(p => p.sprite.id);
                foreach (var category in categoryList.categories)
                    category.labels.RemoveAll(label => !spriteIds.Contains(label.spriteId));

                m_SpriteCategoryList.CopyFrom(categoryList);
            }
        }

        public SpriteCategoryListCacheObject spriteCategoryList
        {
            get { return m_SpriteCategoryList; }
        }

        private void ToolChanged(ITool tool)
        {
            var visibilityTool = GetTool(Tools.Visibility);
            if ((ITool) visibilityTool == tool)
            {
                m_State.lastVisibilityToolActive = visibilityTool.isActive;
            }
        }
    }
}
