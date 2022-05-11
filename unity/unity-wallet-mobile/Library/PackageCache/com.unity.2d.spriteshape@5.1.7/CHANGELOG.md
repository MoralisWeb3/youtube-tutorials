# Changelog

## [5.1.7] - 2021-11-24
### Fixed
- 1381502 Fixed cae where SpriteShape Corner is not set when setting multiple corners

## [5.1.6] - 2021-11-03
### Fixed
- 1371084 Fix case where it crashes when disable Optimizing the geometry of SpriteShape.

## [5.1.5] - 2021-09-11
### Fixed
- Update manual to reflect reorganization of menu item.
- 1364350 Fix case where colliders do not extend to the end of the sprite texture when Sprite Borders are enabled.
- 1364349 Fix case where Edge and Polygon colliders have missing edges on certain open-ended shapes.
- 1364348 Fix case where enabling Fill Tessellation and setting profile's fill offset to positive causes errors.

## [5.1.4] - 2021-07-23
### Fixed
- 1350020 Fix case where Triangular spriteshape with 0 offset collider does not generate collision shape.
- 1349675 Fix case where all open Scenes were dirtied when editing a SpriteShape Spline.

## [5.1.3] - 2021-05-31
### Fixed
- 1331740 Fixed an issue where Edge collider 2D and polygon collider 2D create different collision shapes during playmode.
- 1331195 Fixed case where SpriteShape generates Edge Colliders with deformed corners.
- 1339776 Fixed case where vertex data allocation may not be enough and overflows.

## [5.1.2] - 2021-04-12
### Fixed
- 1321973 When SpriteShapeController has "Update Collider" set to true, it will dirty the scene every time it
- 1320361 Disabling Edit Spline on Sprite Shape Controller while Cache Geometry is enabled throws an ArgumentException error

## [5.1.1] - 2020-11-25
### Fixed
- 1293736 Fixed InvalidOperationException thrown continuously on adding SpriteShapeController component to a GameObject with SpriteRenderer.
- 1293737 Fixed ArgumentException when tangents and cache geometry are enabled on SpriteShapeController component.
- 1291842 Fixed error when adding AngleRange to SpriteShapeProfile Preset that was reset before.

## [5.1.0] - 2020-09-24
### Added
- Added C# Job Tessellation support for Fill Area of SpriteShape.

### Fixed
- 1274400 SpriteShape Bounding Box does not take into account certain vertices
- 1273705 Assertion failed exception is thrown on undoing after clicking on Create Range button
- 1273635 Errors occurs when adding range on Reset-ed Preset of the SpriteShapeProfile
- 1271817 Icon is missing on creating SpriteShapeProfile at the time of creating
- 1280016 Unable to create Sprite Shape Profile along with ArgumentNullException thrown in the Project window
- 1274776 NullReferenceException thrown on performing Redo operation after creating Range property in the SpriteShape profiler preset

## [5.0.2] - 2020-08-31
### Fixed
- 1267542 Sprite Variant Window does not appear in Sprite Shape Controller Component when selecting a Spline pivot point.
- 1265846 Dragging Sprite Shape Profile to Hierarchy creates a Game Object in main Scene when being in Prefab Mode

## [5.0.1] - 2020-07-17
### Changed
- If Geometry is baked using SpriteShapeGeometryCache, do not check for dirty once data is updated to prevent GC.
- Updated Help Section to point to the correct URLs.

### Fixed
- 1242910 Unable to add item on Resetting the Preset of the SpriteShapeProfile
- 1256914 Exception thrown continuously when Undo operation is performed with sprites are assigned earlier
- 1263266 BakeCollider requires GC every frame even when there are no changes in SpriteShape

## [5.0.0] - 2020-05-28
### Added
- Sample script GenerateSpriteShapes.cs to demonstrate force generating invisible SpriteShapes on runtime scene load.

### Changed
- Version bump for Unity 2020.2

### Fixed
- 1246133 Error occurs when unselecting Cache Geometry for Sprite Shape prefab
- 1240380 OnGUI in SpriteShapeController creates GC allocs.
- 1235972 "A Native Collection has not been disposed, resulting in a memory leak" is thrown when 2D Sprite Shape Controller is disabled
- 1240514 InvalidOperationException thrown continuously on adding "Sprite Shape Controller" Component to a Sprite object
- 1241841 Disabled corner option does not work on existing spriteshape upgraded from a previous version

## [4.1.1] - 2020-04-20
### Added
- Added BakeMesh to save generated geometry data.
- Added warning when a valid SpriteShape profile is not set.

## [4.1.0] - 2020-03-16
### Added
- Stretched Corners.

### Fixed
- 1226841 Fix when Collider generation allocation.
- 1226856 SpriteShape Edge Collider does not extend to End-point even if Edges dont overlap.
- 1226847 SpriteShape Corner Threshold does not work.


## [4.0.3] - 2020-03-09
### Fixed
- 1220091 SpriteShapeController leaks memory when zero control points are used
- 1216990 Colliders should also respect Pivot property of Edge Sprites.
- 1225366 Ensure SpriteShape are generated when not in view on Runtime.

## [4.0.2] - 2020-02-11
### Changed
- Improved Memory Allocations.

### Fixed
- Fixed OnDrawGizmos to Get/Release RenderTexture through CommandBuffer.

## [4.0.1] - 2019-11-26
### Changed
- Updated License file
- Updated Third Party Notices file
- Changed how Samples are installed into user's project

### Fixed
- Fixed where the last point of the Sprite Shape does not behave correctly when using Continuous Points in a closed shape (case 1184721)

## [4.0.0] - 2019-11-06
### Changed
- Update version number for Unity 2020.1

## [3.0.7] - 2019-10-27
### Fixed
- Added missing meta file

### Changed
- Update com.unity.2d.path package dependency

## [3.0.6] - 2019-09-27
### Added
- Added support to set CornerAngleThreshold.
- Burst is now enabled for performance boost.
### Fixed
- Fix (Case 1041062) Inputting Point Position manually causes mesh to not conform to the spline
- Fix GC in confirming Spline Extras sample.
- Fix hash Validation errors.
- Removed resources from Packages.

## [3.0.5] - 2019-09-05
### Fixed
- Fix (Case 1159767) Error generated when using a default sprite for Corner sprite or Angle Range sprite in Sprite Shape Profile
- Fix (Case 1178579) "ArgumentOutofRangeException" is thrown and SpriteShapeProfile freezes on reset

## [3.0.4] - 2019-08-09
### Added
- Added tangent channel support for proper 2D lighting in URP.

## [3.0.3] - 2019-07-24
### Added
- Add related test packages

## [3.0.2] - 2019-07-13
### Changed
- Update to latest Mathematics package version

## [3.0.1] - 2019-07-13
### Changed
- Mark package to support Unity 2019.3.0a10 onwards.

## [3.0.0] - 2019-06-19
### Changed
- Stable Version.
- Remove experimental namespace.

## [2.1.0-preview.8] - 2019-06-12
### Changed
- Fix (Case 1152342) The first point of the Sprite Shape does not behave correctly when using Continuous Points
- Fix (Case 1160009) Edge and Polygon Collider does not seem to follow the spriteshape for some broken mirrored tangent points
- Fix (Case 1157201) Edge Sprite Material changed when using a fill texture that is already an edge sprite on spriteshape
- Fix (Case 1162134) Open ended Spriteshape renders the fill texture instead of the range sprite

## [2.1.0-preview.7] - 2019-06-02
### Changed
- Fix Variant Selection.

## [2.1.0-preview.6] - 2019-06-02
### Changed
- Fix Null reference exception caused by SplineEditorCache changes.
- Fill Inspector changes due to Path integration.

## [2.1.0-preview.4] - 2019-05-28
### Changed
- Upgrade Mathematics package.
- Use path editor.

## [2.1.0-preview.2] - 2019-05-13
### Changed
- Initial version for 2019.2
- Update for common package.

## [2.0.0-preview.8] - 2019-05-16
### Fixed
- Fixed issue when sprites are re-ordered in Angle Range.
- Updated Samples.

## [2.0.0-preview.7] - 2019-05-10
### Fixed
- Version Update and fixes.

## [2.0.0-preview.6] - 2019-05-08
### Fixed
- Added Sprite Variant Selector.
- Fix Variant Bug (https://forum.unity.com/threads/spriteshape-preview-package.522575/page-6#post-4480936)
- Fix (Case 1146747) SpriteShape generating significant GC allocations every frame (OnWillRenderObject)

## [2.0.0-preview.5] - 2019-04-18
### Fixed
- Shape angle does not show the accurate sprite on certain parts of the shape.
- SpriteShape - Unable to use the Depth buffer (https://forum.unity.com/threads/spriteshape-preview-package.522575/page-6#post-4413142)

## [2.0.0-preview.4] - 2019-03-28
### Changed
- Disable burst for now until we have a final release.

## [2.0.0-preview.3] - 2019-03-25
### Fixed
- Update Common version.

## [2.0.0-preview.2] - 2019-03-08
### Fixed
- Fix Edge Case Scenario where Vertices along Continuous segment could be duplicated..
- Ensure that Collider uses a valid Sprite on Generation.

## [2.0.0-preview.1] - 2019-02-27
### Changed
- Updated version.

## [1.1.0-preview.1] - 2019-02-10
### Added
- Spriteshape tessellation code is re-implemented in C# Jobs and utilizes Burst for Performance.
- Added Mirrored and Non-Mirrored continous Tangent mode.
- Simplified Collider Generation support and is part of C# Job/Burst for performance.
- Added Shortcut Keys (for setting Tangentmode, Sprite Variant and Mirror Tangent).
- Ability to drag Spriteshape Profile form Project view to Hierarchy to create Sprite Shape in Scene.
- Simplified Corner mode for Points and is now enabled by default.
- Added Stretch UV support for Fill Area.
- Added Color property to SpriteShapeRenderer.

### Fixed
- SpriteShapeController shows wrong Sprites after deleting a sprite from the top angle range.
- Empty SpriteShapeController still seem to show the previous Spriteshape drawcalls
- Streched Sprites are generated in between non Linked Points
- Corners sprites are no longer usable if user only sets the corners for the bottom
- Sprites in SpriteShape still shows even after user deletes the SpriteShape Profile
- SpriteShape doesn't update Point Positions visually at runtime for Builds
- Spriteshape Colliders does not update in scene immediately
- Fixed constant Mesh baking (https://forum.unity.com/threads/spriteshape-preview-package.522575/page-4#post-3925789)
- Fixed Bounds generation issue (https://forum.unity.com/threads/spriteshape-preview-package.522575/page-5#post-4079857)
- Sprite Shape Profile component breaks when creating range
- Fixed when sprite is updated in the sprite editor, the spriteshape is not updated.
- Fixed cases where Spline Edit is disabled even when points are selected. (https://forum.unity.com/threads/spriteshape-preview-package.522575/#post-3436940)
- Sprite with SpriteShapeBody Shader gets graphical artifacts when rotating the camera.
- When multiple SpriteShapes are selected, Edit Spline button is now disabled. (https://forum.unity.com/threads/spriteshape-preview-package.522575/page-3#post-3764413)
- Fixed texelSize property (https://forum.unity.com/threads/spriteshape-preview-package.522575/page-4#post-3877081)
- Fixed Collider generation for different quality levels. (https://forum.unity.com/threads/spriteshape-preview-package.522575/page-4#post-3956062)
- Fixed Framing Issues (https://forum.unity.com/threads/spriteshape-preview-package.522575/page-5#post-4137214)
- Fixed Collider generation for Offsets (https://forum.unity.com/threads/spriteshape-preview-package.522575/page-5#post-4149841)
- Fixed Collider generation for different Heights (https://forum.unity.com/threads/spriteshape-preview-package.522575/page-5#post-4190116)

### Changed
- SpriteShape Asset parameters WorldSpace UV, PixelPerUnit have been moved to SpriteShapeController properties.
- Collider generation has been simplified and aligns well with the generated geometry (different height, corners etc.)

### Removed
- Remove redundant parameters BevelCutoff and BevelSize that can be done by simply modifying source spline.

## [1.0.12-preview.1] - 2018-08-03
### Added
- Fix issue where Point Positions do not update visually at runtime for Builds

## [1.0.11-preview] - 2018-06-20
### Added
- Fix Spriteshape does not update when Sprites are reimported.
- Fix SpriteShapeController in Scene view shows a different sprite when user reapplies a Sprite import settings
- Fix Editor Crashed when user adjusts the "Bevel Cutoff" value
- Fix Crash when changing Spline Control Points for a Sprite Shape Controller in debug Inspector
- Fix SpriteShape generation when End-points are Broken.
- Fix cases where the UV continuity is broken even when the Control point is continous.

## [1.0.10-preview] - 2018-04-12
### Added
- Version number format changed to -preview

## [0.1.0] - 2017-11-20
### Added
-  Bezier Spline Shape
-  Corner Sprites
-  Edge variations
-  Point scale
-  SpriteShapeRenderer with support for masking
-  Auto update collision shape
