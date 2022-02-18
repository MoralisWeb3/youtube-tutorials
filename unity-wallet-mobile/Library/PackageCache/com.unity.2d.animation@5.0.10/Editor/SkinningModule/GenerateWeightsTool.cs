using System;
using UnityEditor.U2D.Layout;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class GenerateWeightsTool : MeshToolWrapper
    {
        private const float kWeightTolerance = 0.01f;
        private SpriteMeshDataController m_SpriteMeshDataController = new SpriteMeshDataController();
        private IWeightsGenerator m_WeightGenerator;
        private GenerateWeightsPanel m_GenerateWeightsPanel;

        internal override void OnCreate()
        {
            m_WeightGenerator = new BoundedBiharmonicWeightsGenerator();
        }

        public override void Initialize(LayoutOverlay layout)
        {
            base.Initialize(layout);

            m_GenerateWeightsPanel = GenerateWeightsPanel.GenerateFromUXML();

            layout.rightOverlay.Add(m_GenerateWeightsPanel);

            BindElements();
            m_GenerateWeightsPanel.SetHiddenFromLayout(true);
        }

        private void BindElements()
        {
            Debug.Assert(m_GenerateWeightsPanel != null);

            m_GenerateWeightsPanel.onGenerateWeights += () =>
            {
                HandleWeights(GenerateWeights, TextContent.generateWeights);
            };

            m_GenerateWeightsPanel.onNormalizeWeights += () =>
            {
                HandleWeights(NormalizeWeights, TextContent.normalizeWeights);
            };

            m_GenerateWeightsPanel.onClearWeights += () =>
            {
                HandleWeights(ClearWeights, TextContent.clearWeights);
            };
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            m_GenerateWeightsPanel.SetHiddenFromLayout(false);
            skinningCache.events.skinningModeChanged.AddListener(OnModeChanged);
            skinningCache.events.selectedSpriteChanged.AddListener(OnSpriteSelectionChanged);
            m_GenerateWeightsPanel.Update(skinningCache.mode == SkinningMode.Character);
            OnSpriteSelectionChanged(skinningCache.selectedSprite);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            skinningCache.events.skinningModeChanged.RemoveListener(OnModeChanged);
            skinningCache.events.selectedSpriteChanged.RemoveListener(OnSpriteSelectionChanged);
            m_GenerateWeightsPanel.SetHiddenFromLayout(true);
        }

        void OnModeChanged(SkinningMode mode)
        {
            m_GenerateWeightsPanel.Update(mode == SkinningMode.Character);
        }

        void OnSpriteSelectionChanged(SpriteCache sprite)
        {
            m_GenerateWeightsPanel.generateButtonText = sprite != null ? TextContent.generate : TextContent.generateAll;
        }

        private void HandleWeights(Action<SpriteCache> action, string undoName)
        {
            using (skinningCache.UndoScope(undoName))
            {
                var selectedSprite = skinningCache.selectedSprite;
                if (selectedSprite != null)
                    HandleWeightsForSprite(selectedSprite, action);
                else
                {
                    var sprites = skinningCache.GetSprites();

                    foreach (var sprite in sprites)
                    {
                        if (sprite.IsVisible())
                            HandleWeightsForSprite(sprite, action);
                    }
                }
            }
        }

        private void HandleWeightsForSprite(SpriteCache sprite, Action<SpriteCache> action)
        {
            Debug.Assert(sprite != null);

            using (new DefaultPoseScope(skinningCache.GetEffectiveSkeleton(sprite)))
            {
                action(sprite);
            }

            skinningCache.events.meshChanged.Invoke(sprite.GetMesh());
        }

        private void GenerateWeights(SpriteCache sprite)
        {
            using (m_GenerateWeightsPanel.associateBones ? new AssociateBonesScope(sprite) : null)
            {
                m_SpriteMeshDataController.spriteMeshData = sprite.GetMesh();
                m_SpriteMeshDataController.CalculateWeights(m_WeightGenerator, skinningCache.vertexSelection, kWeightTolerance);
                m_SpriteMeshDataController.SortTrianglesByDepth();
            }
        }

        private void NormalizeWeights(SpriteCache sprite)
        {
            m_SpriteMeshDataController.spriteMeshData = sprite.GetMesh();
            m_SpriteMeshDataController.NormalizeWeights(skinningCache.vertexSelection);
            m_SpriteMeshDataController.SortTrianglesByDepth();
        }

        private void ClearWeights(SpriteCache sprite)
        {
            m_SpriteMeshDataController.spriteMeshData = sprite.GetMesh();
            m_SpriteMeshDataController.ClearWeights(skinningCache.vertexSelection);
        }

        protected override void OnGUI()
        {
            m_MeshPreviewBehaviour.showWeightMap = true;
            m_MeshPreviewBehaviour.overlaySelected = true;
            skeletonMode = SkeletonMode.EditPose;
            meshMode = SpriteMeshViewMode.EditGeometry;
            disableMeshEditor = true;
            skeletonTool.skeletonStyle = SkeletonStyles.WeightMap;

            DoSkeletonGUI();
            DoMeshGUI();
        }
    }
}
