# Skinning Editor

The __Skinning Editor__ is available as a module in the __Sprite Editor__ after you install the __2D Animation__ package. You can use the [available tools](SkinEdToolsShortcuts.md) in the Skinning Editor to create the [bones](SkinEdToolsShortcuts.html#bone-tools) of the animation skeleton, generate and edit the mesh [geometry](SkinEdToolsShortcuts.html#geometry-tools) of your character, and adjust the [weights](SkinEdToolsShortcuts.html#weight-tools) used to bind the bones to the Sprite meshes.

To open your imported character in the Skinning Editor:

1. Select the character [Prefab](https://docs.unity3d.com/Manual/Prefabs.html) created after importing your character with the PSD Importer.

2. Select the Prefab and go to its Inspector window. Select the __Sprite Editor__ button to open the Prefab in the Sprite Editor.

3. In the Sprite Editor, open the drop-down menu at the upper left of the editor window and select the __Skinning Editor__ module.

   ![](images/SelectSknEditor.png)



See the [Editor tools and shortcuts](SkinEdToolsShortcuts.md) page for more information about the different features and tools available in the Skinning Editor.

## How to select a Sprite in the editor

To select a Sprite in the Skinning Editor window:

1. Double-click a Sprite to select it in the editor window. An orange outline appears around the Sprite that is selected (you can change the outline color in [Tool Preferences](ToolPref.md).

2. If the Sprite you want to select is behind other Sprites, hover over where the Sprite is, and double-click to cycle through all Sprites at the cursor location until you reach the desired Sprite.

3. Double-click on a blank area in the editor window to deselect all Sprites.

## How to select bone or Mesh vertices in the editor

To select a bone or mesh vertices when using the [Bone](SkinEdToolsShortcuts.html#bone-tools) and [Geometry tools](SkinEdToolsShortcuts.html#geometry-tools):

1. Click a bone or mesh vertex to select it specifically.

2. Draw a selection rectangle over multiple bones or vertices to select them all at once.

3. Right click to deselect any selected bone or mesh vertices.