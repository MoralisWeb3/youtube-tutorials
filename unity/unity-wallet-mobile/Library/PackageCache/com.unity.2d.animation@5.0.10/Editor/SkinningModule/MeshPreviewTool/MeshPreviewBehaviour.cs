using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class DefaultPreviewBehaviour : IMeshPreviewBehaviour
    {
        public float GetWeightMapOpacity(SpriteCache sprite)
        {
            return 0f;
        }

        public bool DrawWireframe(SpriteCache sprite)
        {
            return false;
        }

        public bool Overlay(SpriteCache sprite)
        {
            return false;
        }

        public bool OverlayWireframe(SpriteCache sprite)
        {
            return sprite.IsVisible() && sprite.skinningCache.selectedSprite == sprite;
        }
    }

    internal class MeshPreviewBehaviour : IMeshPreviewBehaviour
    {
        public bool showWeightMap { get; set; }
        public bool drawWireframe { get; set; }
        public bool overlaySelected { get; set; }

        public float GetWeightMapOpacity(SpriteCache sprite)
        {
            var skinningCache = sprite.skinningCache;

            if (showWeightMap)
            {
                if (skinningCache.selectedSprite == sprite || skinningCache.selectedSprite == null)
                    return VisibilityToolSettings.meshOpacity;
            }

            return 0f;
        }

        public bool DrawWireframe(SpriteCache sprite)
        {
            var skinningCache = sprite.skinningCache;

            if (drawWireframe)
                return skinningCache.selectedSprite == null;

            return false;
        }

        public bool Overlay(SpriteCache sprite)
        {
            var skinningCache = sprite.skinningCache;

            if (overlaySelected && skinningCache.selectedSprite == sprite)
                return true;

            return false;
        }

        public bool OverlayWireframe(SpriteCache sprite)
        {
            return sprite.IsVisible() && sprite.skinningCache.selectedSprite == sprite;
        }
    }
}
