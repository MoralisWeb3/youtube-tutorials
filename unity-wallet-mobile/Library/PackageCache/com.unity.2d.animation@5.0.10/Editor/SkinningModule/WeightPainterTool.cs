using UnityEngine;
using UnityEditor.U2D.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.U2D.Animation
{
    internal enum WeightPainterMode
    {
        Brush,
        Slider
    }

    internal class WeightPainterTool : MeshToolWrapper
    {
        private WeightPainterPanel m_WeightPainterPanel;
        private WeightEditor m_WeightEditor = new WeightEditor();
        private Brush m_Brush = new Brush(new GUIWrapper());
        private ISelection<int> m_BrushSelection = new IndexedSelection();
        private CircleVertexSelector m_CircleVertexSelector = new CircleVertexSelector();

        public WeightPainterMode paintMode
        {
            get { return m_WeightPainterPanel.paintMode; }
            set { m_WeightPainterPanel.paintMode = value; }
        }

        public override int defaultControlID
        {
            get { return m_Brush.controlID; }
        }

        internal override void OnCreate()
        {
            m_WeightEditor.cacheUndo = skinningCache;

            m_Brush.onMove += (brush) =>
            {
                UpdateBrushSelection(brush);
            };
            m_Brush.onRepaint += (brush) =>
            {
                DrawBrush(brush);
            };
            m_Brush.onSize += (brush) =>
            {
                UpdateBrushSelection(brush);
                m_WeightPainterPanel.size = Mathf.RoundToInt(brush.size);
            };
            m_Brush.onStrokeBegin += (brush) =>
            {
                UpdateBrushSelection(brush);
                EditStart(m_BrushSelection, true);
            };
            m_Brush.onStrokeDelta += (brush) =>
            {
                if (m_BrushSelection.Count > 0)
                    meshTool.UpdateWeights();
            };
            m_Brush.onStrokeStep += (brush) =>
            {
                UpdateBrushSelection(brush);

                var hardness = brush.hardness / 100f;

                if (EditorGUI.actionKey)
                    hardness *= -1f;

                EditWeights(hardness, false);
            };
            m_Brush.onStrokeEnd += (brush) =>
            {
                EditEnd();
            };
        }

        public string panelTitle
        {
            set { m_WeightPainterPanel.title = value; }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            m_WeightPainterPanel.SetHiddenFromLayout(false);

            skinningCache.events.selectedSpriteChanged.AddListener(OnSelectedSpriteChanged);
            skinningCache.events.skinningModeChanged.AddListener(OnSkinningModeChanged);
            skinningCache.events.boneSelectionChanged.AddListener(OnBoneSelectionChanged);

            m_Brush.size = skinningCache.brushSize;
            m_Brush.hardness = skinningCache.brushHardness;
            m_Brush.step = skinningCache.brushStep;
            m_WeightPainterPanel.size = (int) m_Brush.size;
            m_WeightPainterPanel.hardness = (int) m_Brush.hardness;
            m_WeightPainterPanel.step = (int) m_Brush.step;

            UpdatePanel();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            skinningCache.events.selectedSpriteChanged.RemoveListener(OnSelectedSpriteChanged);
            skinningCache.events.skinningModeChanged.RemoveListener(OnSkinningModeChanged);
            skinningCache.events.boneSelectionChanged.RemoveListener(OnBoneSelectionChanged);

            m_WeightPainterPanel.SetHiddenFromLayout(true);
        }

        private void OnBoneSelectionChanged()
        {
            UpdateSelectedBone();
        }

        private void OnSelectedSpriteChanged(SpriteCache sprite)
        {
            UpdatePanel();
        }

        private void OnSkinningModeChanged(SkinningMode mode)
        {
            UpdatePanel();
        }

        private string[] GetSkeletonBonesNames()
        {
            var names = new List<string>() { WeightPainterPanel.kNone };
            var skeleton = skinningCache.GetEffectiveSkeleton(skinningCache.selectedSprite);

            if (skeleton != null)
                names.AddRange(GetUniqueBoneNames(skeleton.bones, skeleton));

            return names.ToArray();
        }

        private string[] GetMeshBoneNames()
        {
            var mesh = meshTool.mesh;
            var skeleton = skinningCache.GetEffectiveSkeleton(skinningCache.selectedSprite);

            if (mesh != null && skeleton != null)
            {
                var bones = meshTool.mesh.bones.ToSpriteSheetIfNeeded();
                return GetUniqueBoneNames(bones, skeleton);
            }

            return new string[0];
        }

        private string[] GetUniqueBoneNames(BoneCache[] bones, SkeletonCache skeleton)
        {
            return Array.ConvertAll(bones, b => skeleton.GetUniqueName(b));
        }

        private void UpdatePanel()
        {
            m_WeightPainterPanel.SetActive(skinningCache.selectedSprite != null);
            m_WeightPainterPanel.UpdateWeightInspector(meshTool.mesh, GetMeshBoneNames(), skinningCache.vertexSelection, skinningCache);
            m_WeightPainterPanel.UpdatePanel(GetSkeletonBonesNames());
            UpdateSelectedBone();
        }

        private void UpdateSelectedBone()
        {
            var boneName = WeightPainterPanel.kNone;
            var bone = skinningCache.skeletonSelection.activeElement.ToSpriteSheetIfNeeded();
            var skeleton = skinningCache.GetEffectiveSkeleton(skinningCache.selectedSprite);

            if (skeleton != null && skeleton.Contains(bone))
                boneName = skeleton.GetUniqueName(bone);

            m_WeightPainterPanel.SetBoneSelectionByName(boneName);
        }

        public override void Initialize(LayoutOverlay layout)
        {
            base.Initialize(layout);

            m_WeightPainterPanel = WeightPainterPanel.GenerateFromUXML();
            m_WeightPainterPanel.SetHiddenFromLayout(true);
            layout.rightOverlay.Add(m_WeightPainterPanel);

            m_WeightPainterPanel.sliderStarted += () =>
                {
                    EditStart(skinningCache.vertexSelection, false);
                };
            m_WeightPainterPanel.sliderChanged += (value) =>
                {
                    EditWeights(value, true);
                    meshTool.UpdateWeights();
                };
            m_WeightPainterPanel.sliderEnded += () =>
                {
                    EditEnd();
                };
            m_WeightPainterPanel.bonePopupChanged += (i) =>
                {
                    var skeleton = skinningCache.GetEffectiveSkeleton(skinningCache.selectedSprite);

                    if (skeleton != null)
                    {
                        BoneCache bone = null;

                        if (i != -1)
                            bone = skeleton.GetBone(i).ToCharacterIfNeeded();

                        if(bone != skinningCache.skeletonSelection.activeElement)
                        {
                            using (skinningCache.UndoScope(TextContent.boneSelection))
                            {
                                skinningCache.skeletonSelection.activeElement = bone;
                                InvokeBoneSelectionChanged();
                            }
                        }
                    }
                };
            m_WeightPainterPanel.weightsChanged += () => meshTool.UpdateWeights();
        }

        internal void SetWeightPainterPanelTitle(string title)
        {
            m_WeightPainterPanel.title = title;
        }

        private void AssociateSelectedBoneToCharacterPart()
        {
            var mesh = meshTool.mesh;

            if (skinningCache.hasCharacter 
                && skinningCache.mode == SkinningMode.Character
                && m_WeightPainterPanel.boneIndex != -1 
                && mesh != null)
            {
                var skeleton = skinningCache.character.skeleton;

                Debug.Assert(skeleton != null);

                var bone = skeleton.GetBone(m_WeightPainterPanel.boneIndex);

                if (!mesh.ContainsBone(bone))
                {
                    using (skinningCache.UndoScope(TextContent.addBoneInfluence))
                    {
                        var characterPart = mesh.sprite.GetCharacterPart();
                        var characterBones = characterPart.bones.ToList();
                        characterBones.Add(bone);
                        characterPart.bones = characterBones.ToArray();
                        skinningCache.events.characterPartChanged.Invoke(characterPart);
                        m_WeightPainterPanel.UpdateWeightInspector(meshTool.mesh, GetMeshBoneNames(), skinningCache.vertexSelection, skinningCache);
                    }
                }
            }
        }
        
        private void EditStart(ISelection<int> selection, bool relative)
        {
            AssociateSelectedBoneToCharacterPart();

            SetupWeightEditor(selection);

            if (m_WeightEditor.spriteMeshData != null)
                m_WeightEditor.OnEditStart(relative);
        }

        private void EditWeights(float hardness, bool emptySelectionEditsAll)
        {
            m_WeightEditor.emptySelectionEditsAll = emptySelectionEditsAll;

            if (m_WeightEditor.spriteMeshData != null)
                m_WeightEditor.DoEdit(hardness);
        }

        private void EditEnd()
        {
            if (m_WeightEditor.spriteMeshData != null)
            {
                m_WeightEditor.OnEditEnd();
                meshTool.UpdateWeights();
            }
        }

        private void InvokeBoneSelectionChanged()
        {
            skinningCache.events.boneSelectionChanged.RemoveListener(OnBoneSelectionChanged);
            skinningCache.events.boneSelectionChanged.Invoke();
            skinningCache.events.boneSelectionChanged.AddListener(OnBoneSelectionChanged);
        }

        private int ConvertBoneIndex(int index)
        {
            if (index != -1 && meshTool.mesh != null)
            {
                var skeleton = skinningCache.GetEffectiveSkeleton(meshTool.mesh.sprite);

                if (skeleton != null)
                {
                    var bone = skeleton.GetBone(index).ToCharacterIfNeeded();
                    index = Array.IndexOf(meshTool.mesh.bones, bone);
                }
            }

            return index;
        }

        private void SetupWeightEditor(ISelection<int> selection)
        {
            m_WeightEditor.spriteMeshData = meshTool.mesh;
            m_WeightEditor.mode = m_WeightPainterPanel.mode;
            m_WeightEditor.boneIndex = ConvertBoneIndex(m_WeightPainterPanel.boneIndex);
            m_WeightEditor.autoNormalize = m_WeightPainterPanel.normalize;
            m_WeightEditor.selection = selection;
            m_WeightEditor.emptySelectionEditsAll = true;
        }

        private void UpdateBrushSelection(Brush brush)
        {
            m_BrushSelection.Clear();
            m_CircleVertexSelector.spriteMeshData = meshTool.mesh;
            m_CircleVertexSelector.position = brush.position;
            m_CircleVertexSelector.radius = brush.size;
            m_CircleVertexSelector.selection = m_BrushSelection;
            m_CircleVertexSelector.Select();
        }

        private void DrawBrush(Brush brush)
        {
            var oldColor =  Handles.color;
            Handles.color = Color.white;

            if (EditorGUI.actionKey)
                Handles.color = Color.red;
            if (brush.isHot)
                Handles.color = Color.yellow;

            Handles.DrawWireDisc(brush.position, Vector3.forward, brush.size);
            Handles.color = oldColor;
        }

        protected override void OnGUI()
        {
            m_MeshPreviewBehaviour.showWeightMap = true;
            m_MeshPreviewBehaviour.overlaySelected = true;
            skeletonTool.skeletonStyle = SkeletonStyles.WeightMap;

            skeletonMode = SkeletonMode.EditPose;
            meshMode = SpriteMeshViewMode.EditGeometry;
            disableMeshEditor = true;

            var isBoneHovered = skeletonTool.hoveredBone != null && !m_Brush.isHot;
            var useBrush = paintMode == WeightPainterMode.Brush;

            meshTool.selectionOverride = null;

            if (useBrush)
                meshTool.selectionOverride = m_BrushSelection;

            DoSkeletonGUI();
            DoMeshGUI();

            if (useBrush && !isBoneHovered)
            {
                var handlesMatrix = Handles.matrix;
                var selectedSprite = skinningCache.selectedSprite;
                var matrix = Matrix4x4.identity;

                if (selectedSprite != null)
                    matrix = selectedSprite.GetLocalToWorldMatrixFromMode();

                Handles.matrix *= matrix;

                skinningCache.brushSize = m_Brush.size = m_WeightPainterPanel.size;
                skinningCache.brushHardness = m_Brush.hardness = m_WeightPainterPanel.hardness;
                skinningCache.brushStep = m_Brush.step = m_WeightPainterPanel.step;

                if (m_Brush.isHot || !skinningCache.IsOnVisualElement())
                {
                    meshTool.BeginPositionOverride();
                    m_Brush.OnGUI();
                    meshTool.EndPositionOverride();
                }

                Handles.matrix = handlesMatrix;
            }
        }
    }
}
