using System;
using UnityEngine;

namespace UnityEditor.U2D.Sprites
{
    [RequireSpriteDataProvider(typeof(ISpritePhysicsOutlineDataProvider), typeof(ITextureDataProvider))]
    internal class SpritePhysicsShapeModule : SpriteOutlineModule
    {
        private readonly float kDefaultPhysicsTessellationDetail = 0.25f;
        private readonly byte kDefaultPhysicsAlphaTolerance = 200;

        public SpritePhysicsShapeModule(ISpriteEditor sem, IEventSystem ege, IUndoSystem us, IAssetDatabase ad, IGUIUtility gu, IShapeEditorFactory sef, ITexture2D outlineTexture)
            : base(sem, ege, us, ad, gu, sef, outlineTexture)
        {
            spriteEditorWindow = sem;
        }

        public override string moduleName
        {
            get { return "Custom Physics Shape"; }
        }

        private ISpriteEditor spriteEditorWindow
        {
            get; set;
        }

        public override bool ApplyRevert(bool apply)
        {
            if (m_Outline != null)
            {
                if (apply)
                {
                    var dp = spriteEditorWindow.GetDataProvider<ISpritePhysicsOutlineDataProvider>();
                    for (int i = 0; i < m_Outline.Count; ++i)
                    {
                        dp.SetOutlines(m_Outline[i].spriteID, m_Outline[i].ToListVector());
                        dp.SetTessellationDetail(m_Outline[i].spriteID, m_Outline[i].tessellationDetail);
                    }
                }

                ScriptableObject.DestroyImmediate(m_Outline);
                m_Outline = null;
            }

            return true;
        }

        protected override void LoadOutline()
        {
            m_Outline = ScriptableObject.CreateInstance<SpriteOutlineModel>();
            m_Outline.hideFlags = HideFlags.HideAndDontSave;
            var spriteDataProvider = spriteEditorWindow.GetDataProvider<ISpriteEditorDataProvider>();
            var outlineDataProvider = spriteEditorWindow.GetDataProvider<ISpritePhysicsOutlineDataProvider>();
            foreach (var rect in spriteDataProvider.GetSpriteRects())
            {
                var outlines = outlineDataProvider.GetOutlines(rect.spriteID);
                m_Outline.AddListVector2(rect.spriteID, outlines);
                m_Outline[m_Outline.Count - 1].tessellationDetail = outlineDataProvider.GetTessellationDetail(rect.spriteID);
            }
        }

        protected override void SetupShapeEditorOutline(SpriteRect spriteRect)
        {
            var physicsShape = m_Outline[spriteRect.spriteID];
            var physicsShapes = GenerateSpriteRectOutline(spriteRect.rect,
                Math.Abs(physicsShape.tessellationDetail - (-1f)) < Mathf.Epsilon ? kDefaultPhysicsTessellationDetail : physicsShape.tessellationDetail,
                kDefaultPhysicsAlphaTolerance, m_TextureDataProvider);
            m_Outline[spriteRect.spriteID].spriteOutlines = physicsShapes;
            spriteEditorWindow.SetDataModified();
        }
    }
}
