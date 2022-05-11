# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

## [1.0.0] - 2019-09-18
### Added
- Allow child GameObjects in the Palette Asset to be shown in the Tile Palette
- Add toggle to allow rendering of Gizmos in the Tile Palette
- Add OnSceneGUI API to GridBrushEditorBase for GUI calls for the active Brush (OnPaintSceneGUI is called only when the appropriate EditorTool is active)
- Add GridPaletteUtility to create Palette Assets through scripting

## [1.0.0] - 2019-06-06
### Added
- Rename package to 2D Tilemap Editor
- Switch to a Paintable tool after picking from a Picking Is Default Paintable Grid if the previous tool was a tool that did not allow painting
- Store last used brush per session to persist when going into and out of PlayMode

## [1.0.0] - 2019-03-22
### Added
- Allow users to convert Prefabs to Tile Palettes by dragging and dropping a valid Prefab onto the Tile Palette Window
- Add toggle to allow changing of Z Position with GridBrush
- Expose GridPaintingState.scenePaintTarget to allow users to change currently active target for Tile Palette Window
- Expose GridPaintingState.validTargets to allow users to get currently valid targets for Tile Palette Window
- Expose GridPaintingState.gridBrush to allow users to change currently active GridBrush for Tile Palette Window
- Expose GridPaintingState.palette to allow users to change currently active Palette for Tile Palette Window
- Expose TileUtility to allow users to create default Tiles through scripts
- Add CreateTileFromPaletteAttribute to allow users to specify how Tiles are created when dragging and dropping assets to the Tile Palette Window

### Changed
- Convert TilePalette to use EditorTools API

## [1.0.0] - 2019-01-02
### This is the first release of Tilemap Editor, as a Package
