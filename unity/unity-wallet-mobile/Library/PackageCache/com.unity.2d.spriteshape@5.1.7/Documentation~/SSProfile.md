# Sprite Shape Profile

The __Sprite Shape Profile__ contains the settings that determine which Sprites that appear on a Sprite Shape at specific Angle Ranges, as well as other display settings. You can use the same Profile for multiple __Sprite Shapes__ in a Scene.

Create a __Sprite Shape Profile__ from the Editor main menu (menu: __Assets > Create > Sprite Shape Profile__), and select from the two available options: __Open Shape__ and __Closed Shape__.

![](images/v1.1-SSProfile.png)

| **Property**                                                 | **Function**                                                 |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| **Control Points**                                           | -                                                            |
| **Use Sprite Borders**                                       | Enable to draw the **Sprite Borders** of a Sprite at the Control Point. Define the **Sprite Borders** in the **Sprite Editor**. |
| **Fill**                                                     | -                                                            |
| **Texture**                                                  | Set the Texture to be used as a Fill to this field. Has no effect if the **Open Ended** property in the **Sprite Shape Controller** settings is enabled. |
| **Offset**                                                   | Determines the border offset at the edges of the Fill texture. |
| ![Angle Range tool](images/v1.1-AngleRange.png)**Angle Ranges (tool)** | Use this tool to create Angle Ranges and assign Sprites to those ranges. |
| **Start (degrees)**                                          | Enter the starting angle for the selected Angle Range in degrees. |
| **End (degrees)**                                            | Enter the ending angle for the selected Angle Range in degrees. |
| **Order**                                                    | Determines the display priority when Sprites intersect. Sprites with higher values are rendered above lower ones. |
| **Sprites**                                                  | List of Sprites assigned to the selected Angle Range. Displays a list of all Sprites assigned to the selected Angle Range. The order of Sprites in the list determines their **Sprite Variant** number, starting from zero at the top of the list. The first Sprite at the top of the list is the Sprite displayed by default at a Control Point. |
| **Corners**                                                  | -                                                            |
| *** All Corner options**                                     | Assign specific Sprites to be displayed on the Sprite Shape at the respective corners. Refer to the documentation on [Corner Sprites]() for more information. |

## Open Shape

![Open Shape Profile preset](C:\Users\Sam\Documents\GitHub\com.unity.2d.spriteshape\Documentation~\images\OpenShapeProfile.png)

Use the __Open Shape__ preset Profile to create Shapes made from a single edge outline with tiled Sprites along its edge. This preset is ideal for creating level elements such as platforms. 

![Example of an Open Shape](images/2D_SpriteShape_024.png)

Drag the Open Shape Profile into the Scene view to automatically generate a Sprite Shape with __Open Ended__ enabled in its __Sprite Shape Controller__ settings.

## Closed Shape

![Closed Shape Profile preset](images/ClosedShapeProfile.png)

Use the __Closed Shape__ preset Profile to create Shapes that encompass an enclosed area. The Closed Sprite Shape can display and tile a Fill texture in the enclosed area, if a Fill texture is set in its Profile settings. Use this preset to create large solid filled Shapes that are ideal for backgrounds or large platforms.

![Closed Shape square](images/v1.1-ClosedShapeSquare.png)

Drag the Closed Shape Profile into the Scene view to automatically generate a Sprite Shape with __Open Ended__ disabled in its __Sprite Shape Controller__ settings. The Closed Shape Profile's preset Angle Ranges create a square Sprite Shape by default.

A key feature of the __Sprite Shape Profile__ is the Angle Ranges tool. Assigning an Angle Range determines what Sprite is displayed at specific angles, as the Sprite Shape is deformed in the Scene.

## Creating Angle Ranges 

### Method 1:

To create an Angle Range, click the __Create Range__ button at the bottom of the Angle Ranges tool:

![The 'Create Range'button](images/2D_SpriteShape_014.png)

The __Create Range__ button is only visible if the __Preview Handle__ is over an area without an Angle Range (see the example image below).

![Selecting the preview handle](images/2D_SpriteShape_015.png)

### Method 2:

Another way is to hover your cursor over an empty area of the Angle Range circle. An outline appears to show the possible default angle range. Click to create this Angle Range.

![Angle Range outline](images/2D_SpriteShape_017.png)

### Editing the Angle Range degrees

The range covered by the currently selected Angle Range is displayed at the bottom of the tool.

![Enter Angle Range values](images/2D_SpriteShape_018.png)

You can edit a range by entering new values into __Start__ and __End__, or drag either endpoint of the tool to the desired angles. A range cannot be extended into an existing neighboring range. To delete an Angle Range, select the range and then press the __Del/Delete__ key.

## Assigning Sprites

After creating the Angle Ranges, the next step is to assign Sprites to those ranges. The __Sprites__ list is found beneath the __Angle Ranges__ tool. It lists all the Sprites assigned to the selected range.

![Sprite List](images/2D_SpriteShape_019.png)

To add Sprites to the list, click the __+__ icon to insert a new row to the list.  Click the circle icon next to the empty row to open the __Object Picker__ window, which displays all available Sprites in the project.

![Adding a row to the list](images/2D_SpriteShape_020.png)

You can also drag a Sprite directly onto a row to add it to the list. The Sprite at the top of the list is the default Sprite displayed on the Sprite Shape.  Refer to the other Sprites in the list by their [Sprite Variant](SSController.md) number. See the [Sprite Shape Controller](SSController.md) page for more details.

![Drag and drop Sprites to the row to add to the list](images/2D_SpriteShape_021.png)

Drag the leftmost ends of the rows up or down to reorder the list, which changes the __Sprite Variant__ numbers of the Sprites accordingly.

## Previewing Sprites of multiple Angle Ranges

After assigning Sprites to multiple __Angle Ranges__, rotate the Preview Handle around the Angle Range tool to preview the Sprites assigned those ranges.

![Previewing one range](images/2D_SpriteShape_022.png)

![Previewing the other range](images/2D_SpriteShape_023.png)

