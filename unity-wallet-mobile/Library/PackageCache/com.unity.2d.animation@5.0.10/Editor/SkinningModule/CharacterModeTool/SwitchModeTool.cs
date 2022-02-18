using System;

namespace UnityEditor.U2D.Animation
{
    internal class SwitchModeTool : BaseTool
    {
        protected override void OnActivate()
        {
            if (skinningCache.mode != SkinningMode.SpriteSheet)
            {
                skinningCache.mode = SkinningMode.SpriteSheet;
                skinningCache.events.skinningModeChanged.Invoke(SkinningMode.SpriteSheet);
            }
        }

        protected override void OnDeactivate()
        {
            if (skinningCache.mode != SkinningMode.Character)
            {
                skinningCache.mode = SkinningMode.Character;
                skinningCache.events.skinningModeChanged.Invoke(SkinningMode.Character);
            }
        }
    }
}
