using System;
using UnityEditor.U2D.Layout;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class GenerateGeometryTool : MeshToolWrapper
    {
        private const float kWeightTolerance = 0.1f;
        private SpriteMeshDataController m_SpriteMeshDataController = new SpriteMeshDataController();
        private ITriangulator m_Triangulator;
        private IOutlineGenerator m_OutlineGenerator;
        private IWeightsGenerator m_WeightGenerator;
        private GenerateGeometryPanel m_GenerateGeometryPanel;

        internal override void OnCreate()
        {
            m_Triangulator = new Triangulator();
            m_OutlineGenerator = new OutlineGenerator();
            m_WeightGenerator = new BoundedBiharmonicWeightsGenerator();
        }

        public override void Initialize(LayoutOverlay layout)
        {
            base.Initialize(layout);

            m_GenerateGeometryPanel = GenerateGeometryPanel.GenerateFromUXML();
            m_GenerateGeometryPanel.skinningCache = skinningCache;

            layout.rightOverlay.Add(m_GenerateGeometryPanel);

            BindElements();
            Hide();
        }

        private void BindElements()
        {
            Debug.Assert(m_GenerateGeometryPanel != null);

            m_GenerateGeometryPanel.onAutoGenerateGeometry += (float d, byte a, float s) =>
            {
                var selectedSprite = skinningCache.selectedSprite;

                if (selectedSprite != null)
                {
                    EditorUtility.DisplayProgressBar(TextContent.generatingGeometry, selectedSprite.name, 0f);

                    using (skinningCache.UndoScope(TextContent.generateGeometry))
                    {
                        GenerateGeometry(selectedSprite, d / 100f, a, s);

                        if (m_GenerateGeometryPanel.generateWeights)
                        {
                            EditorUtility.DisplayProgressBar(TextContent.generatingWeights, selectedSprite.name, 1f);
                            GenerateWeights(selectedSprite);
                        }

                        skinningCache.vertexSelection.Clear();
                        skinningCache.events.meshChanged.Invoke(selectedSprite.GetMesh());
                    }

                    EditorUtility.ClearProgressBar();
                }
            };

            m_GenerateGeometryPanel.onAutoGenerateGeometryAll += (float d, byte a, float s) =>
            {
                var sprites = skinningCache.GetSprites();

                using (skinningCache.UndoScope(TextContent.generateGeometry))
                {
                    for (var i = 0; i < sprites.Length; ++i)
                    {
                        var sprite = sprites[i];
                        
                        if (!sprite.IsVisible())
                            continue;

                        EditorUtility.DisplayProgressBar(TextContent.generateGeometry, sprite.name, i * 2f / (sprites.Length * 2f));

                        GenerateGeometry(sprite, d / 100f, a, s);

                        if (m_GenerateGeometryPanel.generateWeights)
                        {
                            EditorUtility.DisplayProgressBar(TextContent.generatingWeights, sprite.name, (i * 2f + 1) / (sprites.Length * 2f));
                            GenerateWeights(sprite);
                        }
                    }

                    foreach(var sprite in sprites)
                        skinningCache.events.meshChanged.Invoke(sprite.GetMesh());

                    EditorUtility.ClearProgressBar();
                }
            };
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            UpdateButton();
            Show();
            skinningCache.events.selectedSpriteChanged.AddListener(OnSelectedSpriteChanged);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            Hide();
            skinningCache.events.selectedSpriteChanged.RemoveListener(OnSelectedSpriteChanged);
        }

        private void Show()
        {
            m_GenerateGeometryPanel.SetHiddenFromLayout(false);
        }

        private void Hide()
        {
            m_GenerateGeometryPanel.SetHiddenFromLayout(true);
        }

        private void UpdateButton()
        {
            var selectedSprite = skinningCache.selectedSprite;

            if (selectedSprite == null)
                m_GenerateGeometryPanel.SetMode(GenerateGeometryPanel.GenerateMode.Multiple);
            else
                m_GenerateGeometryPanel.SetMode(GenerateGeometryPanel.GenerateMode.Single);
        }

        private void OnSelectedSpriteChanged(SpriteCache sprite)
        {
            UpdateButton();
        }

        private void GenerateGeometry(SpriteCache sprite, float outlineDetail, byte alphaTolerance, float subdivide)
        {
            Debug.Assert(sprite != null);

            var mesh = sprite.GetMesh();

            Debug.Assert(mesh != null);

            m_SpriteMeshDataController.spriteMeshData = mesh;
            m_SpriteMeshDataController.OutlineFromAlpha(m_OutlineGenerator, mesh.textureDataProvider, outlineDetail, alphaTolerance);
            m_SpriteMeshDataController.Triangulate(m_Triangulator);

            if (subdivide > 0f)
            {
                var largestAreaFactor = Mathf.Lerp(0.5f, 0.05f, Math.Min(subdivide, 100f) / 100f);
                m_SpriteMeshDataController.Subdivide(m_Triangulator, largestAreaFactor);
            }

            foreach (var vertex in mesh.vertices)
                vertex.position -= sprite.textureRect.position;
        }

        private void GenerateWeights(SpriteCache sprite)
        {
            Debug.Assert(sprite != null);

            var mesh = sprite.GetMesh();

            Debug.Assert(mesh != null);

            using (new DefaultPoseScope(skinningCache.GetEffectiveSkeleton(sprite)))
            {
                if (NeedsAssociateBones(sprite.GetCharacterPart()))
                {
                    using (new AssociateBonesScope(sprite))
                    {
                        GenerateWeights(mesh);
                    }
                }
                else
                    GenerateWeights(mesh);
            }
        }

        private bool NeedsAssociateBones(CharacterPartCache characterPart)
        {
            if (characterPart == null)
                return false;

            var skeleton = characterPart.skinningCache.character.skeleton;

            return characterPart.BoneCount == 0 ||
                    (characterPart.BoneCount == 1 && characterPart.GetBone(0) == skeleton.GetBone(0));
        }

        private void GenerateWeights(MeshCache mesh)
        {
            Debug.Assert(mesh != null);

            m_SpriteMeshDataController.spriteMeshData = mesh;
            m_SpriteMeshDataController.CalculateWeights(m_WeightGenerator, null, kWeightTolerance);
            m_SpriteMeshDataController.SortTrianglesByDepth();
        }

        protected override void OnGUI()
        {
            m_MeshPreviewBehaviour.showWeightMap = m_GenerateGeometryPanel.generateWeights;
            m_MeshPreviewBehaviour.overlaySelected = m_GenerateGeometryPanel.generateWeights;

            skeletonTool.skeletonStyle = SkeletonStyles.Default;

            if (m_GenerateGeometryPanel.generateWeights)
                skeletonTool.skeletonStyle = SkeletonStyles.WeightMap;

            DoSkeletonGUI();
        }
    }
}
