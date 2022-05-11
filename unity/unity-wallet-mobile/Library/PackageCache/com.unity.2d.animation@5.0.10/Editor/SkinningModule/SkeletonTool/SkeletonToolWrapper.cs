using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class SkeletonToolWrapper : BaseTool
    {
        private SkeletonTool m_SkeletonTool;
        private SkeletonMode m_Mode;

        public SkeletonTool skeletonTool
        {
            get { return m_SkeletonTool; }
            set { m_SkeletonTool = value; }
        }

        public SkeletonMode mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        public bool editBindPose { get; set; }

        public override int defaultControlID
        {
            get
            {
                Debug.Assert(skeletonTool != null);

                return skeletonTool.defaultControlID;
            }
        }

        protected override void OnActivate()
        {
            Debug.Assert(skeletonTool != null);
            skeletonTool.enableBoneInspector = true;
            skeletonTool.Activate();
        }

        protected override void OnDeactivate()
        {
            skeletonTool.enableBoneInspector = false;
            skeletonTool.Deactivate();
        }

        private SkeletonMode OverrideMode()
        {
            var modeOverride = mode;

            //Disable SkeletonManipulation if character exists and we are in SpriteSheet mode
            if (skinningCache.mode == SkinningMode.SpriteSheet && skinningCache.hasCharacter && editBindPose)
                modeOverride = SkeletonMode.Selection;

            return modeOverride;
        }

        protected override void OnGUI()
        {
            Debug.Assert(skeletonTool != null);

            skeletonTool.mode = OverrideMode();
            skeletonTool.editBindPose = editBindPose;
            skeletonTool.DoGUI();
        }
    }
}
