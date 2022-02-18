# Sprite Swap overview (Experimental Feature)

__Sprite Swap__ is an experimental feature that enables you to change a GameObject’s rendered Sprite within the same character Prefab. This enables you to quickly and easily create  multiple characters that look different while [reusing existing bone and Mesh data](CopyPasteSkele.md). You can also use Sprite Swap to switch the displayed Sprite on each frame at run time to create [frame-by-frame animation](FFanimation.md).

The workflow for implementing Sprite Swap differs if you are using the workflow that is [integrated with 2D Animation](#sprite-swap-and-2d-animation-integration), or if you are [manually setting up](SSManual.md) the Sprite Swap components.

## Sprite Swap Assets and components

The following Assets and components are part of the Sprite Swap process:

1. The [Sprite Library Asset](SLAsset.md) contains the [Categories and Labels](SpriteVis.html#sprite-tab) of the Sprites.
1. The [Sprite Library component](SLAsset.html#sprite-library-component) shows the currently referred to __Sprite Library Asset__.
1. The [Sprite Resolver component](SLAsset.html#sprite-resolver-component) is used to request a Sprite registered to the __Sprite Library Asset__ by referring to the __Category__ and __Label__ value of the desired Sprite.

## How Unity generates Sprite Swap Assets and components

Unity generates the various [Sprite Swap](SpriteSwapIntro.md) components in the following steps:

1. When you import a .psb file with the [PSD Importer](https://docs.unity3d.com/Packages/com.unity.2d.psdimporter@latest/index.html), Unity generates a Prefab containing a Sprite for each Layer in the source file.
   
2. If you create any [Categories or Labels](SpriteVis.html#sprite-tab) while editing the character Prefab, Unity automatically generates a [Sprite Library Asset](SLAsset.md) as a sub-Asset of the Prefab.
   
3. When the Prefab is brought into the Scene view, Unity generates a GameObject for each Sprite in the Prefab that does _not_ belong to a [Category](SpriteVis.html#sprite-tab). However if the Sprite is the first [Label](SpriteVis.html#sprite-tab) in a __Category__, then Unity will generate a GameObject for the Sprite as well.
   
4. If the Prefab has a __Sprite Library Asset__ sub-Asset, then Unity attaches the [Sprite Library component](SLAsset.html#sprite-library-component) to the root GameObject which is set to reference the __Sprite Library Asset__ created in step 1 by default.
   
5. Unity attaches the [Sprite Resolver component](SLAsset.html#sprite-resolver-component) to all __Sprite__ GameObjects that belong to a Category.

Refer to the respective component pages for more information on their functions and properties.

## Sprite Swap and 2D Animation integration

__Sprite Swap__ is integrated with the 2D Animation workflow. You must install the following packages or newer to use the Sprite Swap feature:

- [2D Animation version 2.2.0-preview.1](https://docs.unity3d.com/Packages/com.unity.2d.animation@latest/index.html)
- [PSDImporter version 1.2.0-preview.1](https://docs.unity3d.com/Packages/com.unity.2d.psdimporter@latest/index.html)

## Skeletal animation limitations

To ensure Sprite Swap works correctly with skeletal animation, the skeleton must be identical between the Sprites being swapped. Use the [Copy and Paste](CopyPasteSkele.md) tools to duplicate the skeleton rig from one Sprite to the other Sprite(s) to ensure they can be swapped smoothly.