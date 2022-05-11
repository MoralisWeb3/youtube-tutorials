# Changelog
All notable changes to this package will be documented in this file. The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

## [1.4.8] - 2021-04-15

### Fixed

- Fixed a regression where changes made to clip curves would not be processed until another modification caused a graph rebuild.

## [1.4.7] - 2021-02-25

### Fixed

- Fixed rare issue where the time field could not be edited after opening a timeline. ([1312198](https://issuetracker.unity3d.com/product/unity/issues/guid/1312198))

## [1.4.6] - 2021-01-08

### Fixed

- Create button on timeline panel no longer defaults to an invalid path. ([1289923](https://issuetracker.unity3d.com/product/unity/issues/guid/1289923))
- Fixed issue where Timeline's bindings field would loses names and bindings when selecting clips. ([1293941](https://issuetracker.unity3d.com/product/unity/issues/guid/1293941))
- Make Timeline's duration result displayed in the Inspector, when switching from duration mode: Based On Clips to Fixed Length, closer to the actual duration. ([1156920](https://issuetracker.unity3d.com/product/unity/issues/guid/1156920))
- Copy/Paste of clips in the Timeline Window will no longer paste clips at an invalid time in mix-mode. ([1289925](https://issuetracker.unity3d.com/product/unity/issues/guid/1289925))

## [1.4.5] - 2020-11-19

### Fixed

- Fixed issue where changing a clip's extrapolation values would clear the current clip selection. ([936046](https://issuetracker.unity3d.com/product/unity/issues/guid/936046))
- Fixed multiple issues related to the curves view:
  - Fixed curve removal not functioning with `PlayableAsset`s (clips & tracks curves). ([1231002](https://issuetracker.unity3d.com/product/unity/issues/guid/1231002))
  - Fixed inconsistent icon display on curves.
  - Fixed incorrect ordering of properties. Properties now have a object/type/property ordering.
  - Fixed unnecessary grouping of fields.
  - Changed context menu from `Remove Properties` to `Remove Curves` to better reflect the change in functionality between curves for GameObjects and curves for `PlayableAssets`.
  - Fixed behaviour where removing a single field in a `Position`, `Rotation` or `Scale` group would remove the entire group.
- Fixed case where pausing in Playmode and switching the active director in editor could pause the director. ([1263707](https://issuetracker.unity3d.com/product/unity/issues/guid/1263707))
- Material properties are now displayed by their shader name in the curves view when possible. ([1115961](https://issuetracker.unity3d.com/product/unity/issues/guid/1115961))
- Fixed issue where a signal could be pasted on a track that doesn't support notifications. ([1283763](https://issuetracker.unity3d.com/product/unity/issues/guid/1283763))
- Fixed issue where a clip could be paseted on an incompatible track. ([1283763](https://issuetracker.unity3d.com/product/unity/issues/guid/1283763))
- Fixed errors when leaving prefab mode when a timeline is opened. ([1280331](https://issuetracker.unity3d.com/product/unity/issues/guid/1280331))
- No preview will be shown when the PlayableDirector is disabled. ([1286198](https://issuetracker.unity3d.com/product/unity/issues/guid/1286198))
- Fixed issue where an infinite clip's `Foot Ik` property was not visible in the Inspector when selecting its track. ([1279824](https://issuetracker.unity3d.com/product/unity/issues/guid/1279824))
- Fixed issue where child particle systems were not controlled correctly when they are not subemitters. ([1212943](https://issuetracker.unity3d.com/product/unity/issues/guid/1212943))
- Fixed inconsistent recording behaviour on audio tracks and `PlayableAssets`. Default values are changed when a value is not recorded, and the key added/updated when a value is already animated. ([1283453](https://issuetracker.unity3d.com/product/unity/issues/guid/1283453))
- Fixed issue where the curves view for tracks and `PlayableAsset`s would not update when changed externally (such as from the Animation window).
- Fixed `Add Key`/`Remove Key` context menus not being properly enabled in some cases when using tracks and `PlayableAsset`s.
- Fixed simulation of subemitters when scrubbing a timeline. ([1142781](https://issuetracker.unity3d.com/product/unity/issues/guid/1142781))
- Fixed choppy playback of particles with a large fixed time step. ([1262234](https://issuetracker.unity3d.com/product/unity/issues/guid/1262234))

## [1.4.4] - 2020-10-09

### Fixed
- Disable drag and drop of Signal asset on Control Track. ([1222760](https://issuetracker.unity3d.com/product/unity/issues/guid/1222760/))
- Fixed system locale causing issues when keying float values on custom clips. ([1190877](https://issuetracker.unity3d.com/product/unity/issues/guid/1190877/))
- Fixed issue where recording to a clip would place keys on the frame. ([1274892](https://issuetracker.unity3d.com/product/unity/issues/guid/1274892/))
- Fixed keyboard clip selection from locked tracks. ([1233612](https://issuetracker.unity3d.com/product/unity/issues/guid/1233612/))
- Fixed issue where the Timeline window would stay locked even when no timeline asset is shown. ([1278598](https://issuetracker.unity3d.com/product/unity/issues/guid/1278598/))
- Fixed issue where invoking `SelectLeft` or `SelectRight` shortcuts on a group track, the group would not collapse or expand. ([1279379](https://issuetracker.unity3d.com/product/unity/issues/guid/1279379/))
- Fixed  Blend Curve Editor from the clip's inspector that was not responding correctly to undo and redo commands. (978673)
- Fixed issue where the `Frame All` action would not frame keys outside of clips when the curve display is collapsed.  ([1273725](https://issuetracker.unity3d.com/product/unity/issues/guid/1273725/), #295)
- Scrolling the horizontal scrollbar of the timeline to the right edge will no longer prevent the user from dragging left again. ([1127199](https://issuetracker.unity3d.com/product/unity/issues/guid/1127199/), #301)
- Splitting a clip with an ease in or out value now ensures ease duration stays on correct side of split. ([1279350](https://issuetracker.unity3d.com/product/unity/issues/guid/1279350/))
- Fixed delay when zooming in after reaching Timeline window's maximum and then zooming back. ([1214228](https://issuetracker.unity3d.com/product/unity/issues/guid/1214228/))
- Prevent creation of presets with Group Tracks. ([1281056](https://issuetracker.unity3d.com/product/unity/issues/guid/1281056))
- Fixed issue where markers placed on top of clips could not be selected. ([1284807](https://issuetracker.unity3d.com/product/unity/issues/guid/1284807), #314)
- Fixed issue where multiple markers placed on top of each other could not be selected. ([1284801](https://issuetracker.unity3d.com/product/unity/issues/guid/1284801), #314)

## [1.4.3] - 2020-08-26

### Fixed
- Fixed incorrect selection when clicking on a clip's blend. (1178052)
- Fixed issue where an exception was thrown when drawing an Audio clip's waveform when that clip wasn't in the AssetDatabase. ([1268868](https://issuetracker.unity3d.com/product/unity/issues/guid/1268868/))
- When choosing `Add Signal Emitter from Signal Asset`, closing the Object Selector window will not add an empty Signal Emitter. ([1261553](https://issuetracker.unity3d.com/product/unity/issues/guid/1261553/))
- Fixed issue where an error would appear when editing keys in the Animation window if the Timeline window is opened. (1269829)
- Fixed issue where the `Frame All` operation would continually increase the zoom value when only empty tracks are added to the timeline ([1273540](https://issuetracker.unity3d.com/product/unity/issues/guid/1273540/)).

## [1.4.2] - 2020-08-04

### Fixed
- Fixed double-click not opening the AnimationWindow on clips with animated parameters. ([1262950](https://issuetracker.unity3d.com/product/unity/issues/guid/1262950/))
- Fixed issue where the Timeline window would rebuild its Playable Graph every time an AnimationClip would be added, changed or deserialized. (1265314, [1267055](https://issuetracker.unity3d.com/product/unity/issues/guid/1267055/))

## [1.4.1] - 2020-07-15

### Fixed
- Fixed `IndexOutOfRangeException` exception being thrown when editing inspector curves. ([1259902](https://issuetracker.unity3d.com/product/unity/issues/guid/1259902/))
- Fixed `IndexOutOfRangeException` exception being thrown when the `New Signal` dialog replaces an existing signal. ([1241170](https://issuetracker.unity3d.com/product/unity/issues/guid/1241170/))
- Fixed signal state being reset on paused timelines. ([1257208](https://issuetracker.unity3d.com/product/unity/issues/guid/1257208/))
- Fixed nested custom types not updating animation values in the inspector. ([1239893](https://issuetracker.unity3d.com/product/unity/issues/guid/1239893/))
- Fixed `AnimationTrack`s SceneOffset mode incorrectly overriding root transform on tracks without root transform in editor. ([1237704](https://issuetracker.unity3d.com/product/unity/issues/guid/1237704/))
- The `DisplayName` attribute is now supported when used with `TrackAsset`s. ([1253397](https://issuetracker.unity3d.com/product/unity/issues/guid/1253397/))
- Fixed `NullReference` exception being thrown when clicking on the `Scene Preview` checkbox if the Timeline window was closed. (1261543)

## [1.4.0] - 2020-06-26

### Added
- Added `ClipCaps.AutoScale` to automatically change the speed multiplier value when the clip is trimmed in the Timeline window.
- Added a `DeleteClip` method in `TrackAsset`.
- Added dependency on Animation, Audio, Director and Particle System modules. ([1229825](https://issuetracker.unity3d.com/product/unity/issues/guid/1229825/))
- Added an option in `TimelineAsset.EditorSettings` to disable scene preview.
- Added base classes to define custom actions:
  - `TimelineAction`
  - `TrackAction`
  - `ClipAction`
  - `MarkerAction`
- Added the following attributes that can be used with action classes:
  - `ApplyDefaultUndo` to automatically manage undo operations.
  - `ActiveInMode` to control in which Timeline mode the action is valid.
  - `MenuEntry` to add the action to the context menu.
  - `TimelineShortcut` can be added to a static method to invoke the action with a shortcut.
- `Invoker` to invoke actions using Timeline's selection or context.
- `MenuOrder` contains menu priority values, to be used with `MenuEntry`.
- `TimelineModes` to specify in which mode an action is valid, to be used with `MenuEntry`.
- `ActionContext` to provide a context to invoke `TimelineAction`s.
- `ActionValidity` to specify is an action is valid for a given context.
- `UndoExtension` to manage undo operations with common Timeline types.

### Changed
- Improved performance with ControlTracks in preview mode for cases where multiple Control Tracks are assigned to the same PlayableDirector.
- Improved layout and appearance of track header buttons.
- Reduced icons' file size without any quality loss.
- A track's binding will be duplicated when pasting or duplicating a track.
- When creating a new timeline asset, the "Timeline" suffix will not be added to the file name twice.
- `ClipCaps.All` now includes the new `Autoscale` feature. To get the previous `ClipCaps.All` behaviour on clips, use
```
ClipCaps.Looping | ClipCaps.Extrapolation | ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | ClipCaps.Blending
```
- Inline curve selection is now synced with the clip's selection.
- Selecting a curve view property will also select the corresponding curve view.
- Clicking and holding the `Command` or `Control` key on a curve view will deselect it if it was already selected.
- Improved Timeline window UI performance.

### Fixed
- Selecting clips from locked tracks is not allowed anymore when using the playhead's context menu.
- Inserting gaps in locked tracks is not allowed anymore.
- When adding an Activation track, the viewport is adjusted to show the new Activation clip.
- Fixed issue where trimming AnimationClips would also change the speed multiplier.

## [1.3.4] - 2020-06-09

### Fixed
- Fix a Control Track bug that caused the first frame of an animation to evaluated incorrectly when scrubbing forwards and backwards. (1253485)
- Fixed memory leak where the most recently played timeline would not get unloaded. ([1214752](https://issuetracker.unity3d.com/product/unity/issues/guid/1214752/) and 1253974)

## [1.3.3] - 2020-05-29

### Fixed
- Fixed regression where animation tracks were writing root motion when the animation clip did not contain root transform values ([1249355](https://issuetracker.unity3d.com/product/unity/issues/guid/1249355/))

## [1.3.2] - 2020-04-02

### Fixed
- Fixed issue where the clip Inspector's curve preview would close when clicking on the curve. ([1228127](https://issuetracker.unity3d.com/product/unity/issues/guid/1228127/))
- Fixed issue where the curves view was not synced between Animation and Timeline windows. ([1213937](https://issuetracker.unity3d.com/issues/animation-window-curves-are-not-updated-immediately-when-changing-them-in-timeline-window))
- Fixed issue where play range didn't loop when range ends on the final frame. ([1215926](https://issuetracker.unity3d.com/issues/timeline-play-range-doesnt-loop-when-play-range-ends-on-the-final-frame))
- Fixed issue where displaying an array in the curves view generated errors. ([1178251](https://issuetracker.unity3d.com/product/unity/issues/guid/1178251/))

## [1.3.1] - 2020-03-13

### Fixed
- Fixed issue where the curves view would flicker when editing multiple keys. ([1217326](https://issuetracker.unity3d.com/product/unity/issues/guid/1217326/))
- Fixed issue where adding a keyframe in the curves view at the end of a clip would not place the keyframe at the correct position. ([1221337](https://issuetracker.unity3d.com/product/unity/issues/guid/1221337/))

## [1.3.0] - 2020-02-26

### Added
- Inline Curve Properties can be removed.
- Tracks can be individually resized.

### Changed
- Creating a new Timeline will no longer automatically add an Animation Track and an Animator to the target GameObject.
- Ease-in and ease-out values for clips are no longer restricted to 50% of the clip's duration.
- The resize handle for inline curves has been moved to the track header area.
- Reduced the minimum width of the track header area.
- Trimming the left edge of a clip while pressing the Shift key will change the Speed Multiplier value.

### Fixed
- Fixed humanoid characters going to default pose during initial root motion recording. (1174752)
- Fixed Override Tracks not masking RootTransform when an AvatarMask without the Root Node is applied. ([1190600](https://issuetracker.unity3d.com/product/unity/issues/guid/1190600/))
- Fixed preview of Avatar Masks on base level Animation Tracks. ([1190600](https://issuetracker.unity3d.com/product/unity/issues/guid/1190600/))

## [1.2.13] - 2020-02-24

### Fixed
- Fixed Performance issue where Control Tracks would resimulate during the tail of a non-looping particle clip. ([1216702](https://issuetracker.unity3d.com/product/unity/issues/guid/1216702/))
- Fixed adjacent recording clips highlighting the wrong clip. ([1210312](https://issuetracker.unity3d.com/product/unity/issues/guid/1210312/))
- Fixed timescale drawing to only draw visible lines which avoids a hang with very large clips. ([1213189](https://issuetracker.unity3d.com/product/unity/issues/guid/1213189/))
- Fixed `SignalReceiver.ChangeSignalAtIndex` incorrectly throwing exception when multiple entries are set to null. ([1210877](https://issuetracker.unity3d.com/product/unity/issues/guid/1210877/))
- Fixed a memory leak with Animation Clips in Edit mode.
- Fixed issue where changes to a Signal Receiver component in a prefab were reverted. ([1210883](https://issuetracker.unity3d.com/product/unity/issues/guid/1210883/))
- Fixed avatar mask reassignment not causing immediate re-evaluation. ([1219326](https://issuetracker.unity3d.com/product/unity/issues/guid/1219326/))
- Fixed issues related to recursive control tracks. (1178423)
- Fixed issue where using the `HideInMenu` attribute in combination with a class inheriting from `Marker` would not hide the marker from the Timeline context menus. ([1221054](https://issuetracker.unity3d.com/product/unity/issues/guid/1221054/))

## [1.2.12] - 2020-02-21

### Fixed
- Fixed issue where the curves view would change its framing when moving a clip. ([1217353](https://issuetracker.unity3d.com/product/unity/issues/guid/1217353/))

## [1.2.11] - 2020-01-22

### Fixed
- Fixed Control Track inspector dropdown not opening. ([1208943](https://issuetracker.unity3d.com/product/unity/issues/guid/1208943/))
- Fixed issue where applying the Match content command on subtimeline clip with a newly created subtimeline with no duration makes the clip disappear. ([1203662](https://issuetracker.unity3d.com/product/unity/issues/guid/1203662/))
- Fixed issue where the opened timeline is changed to another timeline when switching focus from Unity to a different application. ([1087348](https://issuetracker.unity3d.com/product/unity/issues/guid/1087348/))
- Fixed issue where the keys in the inline curves view were incorrectly positioned ([1205835](https://issuetracker.unity3d.com/product/unity/issues/guid/1205835/))

### Changed
- ControlPlayableAsset.searchHierarchy (a.k.a. Control Children) now defaults to false.

## [1.2.10] - 2019-12-08

### Fixed
- Fixed issue where object selectors on tracks did not show bound objects. (1202853)
- Fixing inspector blend graph display for animation clips. (1201474)
- Fixed Timeline Window lock state when restarting Unity and no timeline are selected. ([1201405](https://issuetracker.unity3d.com/product/unity/issues/guid/1201405/))

## [1.2.9] - 2019-12-06

### Fixed
- Added missing high-resolution icons for Personal Skin.

## [1.2.8] - 2019-11-21

### Fixed
- Fixed issue where recording couldn't be turned on for override tracks. (1199389)
- Fixed overlay bug when panning. (1198348)
- Fixed Foot IK being applied in Editor when option is disabled. ([1197426](https://issuetracker.unity3d.com/product/unity/issues/guid/1197426/))
- Fixed issue where the Animation Track's inline curves were not properly aligned when panning the timeline. (1198364)

## [1.2.7] - 2019-11-15

### Fixed
- Fixed inline curves to display PlayableBehaviour array properties. (1178251)
- Fixed clip selection from playhead. (1187495)
- Fixed recorded clips dirtying the scene on copy/paste. (1181492)

## [1.2.6] - 2019-10-25

### Added
- Added Timeline manual.

## [1.2.5] - 2019-10-16

### Changed
- Added tooltips that were missing for Timeline selector and settings buttons. ([1152790](https://issuetracker.unity3d.com/product/unity/issues/guid/1152790/))
- Removed Undo menu entry that was added when clicking on the Inline curves button. ([1187402](https://issuetracker.unity3d.com/product/unity/issues/guid/1187402/))

### Fixed
- Fixed issue where recording couldn't be turned off when an object is deactivated. (1187174)
- Timelines listed in the Timeline selector will now be sorted alphabetically. (1190514)
- Fixed Insert Frames options from Trackhead context menu not applying to markers. (1187895)
- Fixed incorrect display when a large number of nested group tracks was added to a Timeline. (1157367)

## [1.2.4] - 2019-10-03

### Changed
- Properties in the Inline Curve editor will now be listed in the same order as the Animation window. (1184058)
- Updated the appearance of the Timeline window to conform to the [editor's UX redesign](https://blogs.unity3d.com/2019/08/29/evolving-the-unity-editor-ux/)
- Improved the appearance of clip blends.

### Fixed
- Adding a PlayableDirector with no Playable Asset will no longer trigger a repaint of the Timeline Window on each frame. ([1172707](https://issuetracker.unity3d.com/product/unity/issues/guid/1172707/))
- Fixed issue where a clip's blend selection border was not drawn correctly when there was a previous clip. (1178173)
- Fixed issue where Animation Events were fired twice when the Playable Director Wrap mode is set to Loop. ([1173281](https://issuetracker.unity3d.com/product/unity/issues/guid/1173281/))
- Fixed issue where double-clicking on a Timeline Asset would not open it in the Timeline window. ([1182159](https://issuetracker.unity3d.com/product/unity/issues/guid/1182159))
- Fixed issue where the paste shortcut would not work when copying and pasting between two different timelines. (1184967)
- Fixed audio stutter when going into playmode. ([1167289](https://issuetracker.unity3d.com/product/unity/issues/guid/1167289/))
- Fixed PreviousFrame and NextFrame controls in subtimelines with large offsets. (1175320)
- Fixed issue where exceptions were thrown when resetting a Signal Receiver component. ([1158227](https://issuetracker.unity3d.com/product/unity/issues/guid/1158227/))
- Increased font size of clip labels (1179642)

## [1.2.3] - 2019-10-03

### Fixed
- Removed unnecessary directories from the package.

## [1.2.2] - 2019-08-20

### Fixed
- Fixed issue where fields for custom clips were not responding to Add Key commands. (1174416)
- Fixed issue where a different track's bound GameObject is highlighted when clicking a track's bound GameObject box. (1141836)
- Fixed issue where a clip locks to the playhead's position when moving it. (1157280)

## [1.2.1] - 2019-08-01

### Fixed
- Fixed appearance of a selected clip's border.
- Fixed non-transform properties from AnimationClips not being correctly put into preview mode when the avatar root does not contain the animator component. ([1162334](https://issuetracker.unity3d.com/product/unity/issues/guid/1162334/))
- Fixed an issue where the context menu for inline curves keys would not open on MacOS. ([1158584](https://issuetracker.unity3d.com/product/unity/issues/guid/1158584/))
- Fixed recording state being incorrect after toggling preview mode (1146551)
- Fixed copying clips without ExposedReferences causing the scene to dirty (1144469)

## [1.2.0] - 2019-07-16
*Compatible with Unity 2019.3*

### Added
- Added ILayerable interface. Implementing this interface on a custom track will enable support for multiple layers, similar to the AnimationTracks override tracks.
- Added "Pan" autoscrolling option in the Timeline window.
- Enabled rectangle tool for inline curves.

### Changed
- Scrolling horizontally with the mouse wheel or trackpad now pans the timeline view horizontally, instead of zooming.
- Scrolling vertically with the mouse wheel or trackpad on the track headers or on the vertical scroll bar now pans the timeline view vertically, instead of zooming.

### Fixed
- Fixed an issue causing info text to overlap when displaying multiple lines (1150863).
- Fixed duration mode not reverting from "Fixed Length" to "Based On Clips" properly. (1154034)
- Fixed playrange markers being drawn over horizontal scrollbar (1156023)
- Fixed an issue where a hotkey does not autofit all when Marker is present (1158704)
- Fixed an issue where an exception was thrown when overwriting a Signal Asset through the Signal Emitter inspector. (1152202)
- Fixed Control Tracks not updating instances when source prefab change. (case 1158592)
- An exception will be thrown when calling TrackAsset.CreateMarker() with a marker that implements INotification if the track does not support notifications. (1150248)
- Fixed preview mode being reenabled when warnings change on tracks. (case 1151381)
- Fixed minimum clip duration to be frame aligned. (case 1156602)
- Fixed playhead being moved when applying undo while recording.(case 1154802)
- Fixed warnings about localEulerAnglesRaw when using RectTransform. (case 1151100)
- Fixed precision error on the duration of infinite tracks. (case 1156723)
- Fixed issue where two GatherProperties call were made when switching between two PlayableDirectors. (1159036)
- Fixed issue where inspectors for clips, tracks and markers would get incorrectly displayed when no Timeline Window is opened. (1158242, 1158283)
- Fixed issue with clip connectors that were incorrectly drawn when the timeline was panned or zoomed. (1141960)
- Fixed issue where evaluating a Playable Graph inside a Notification Receiver would cause an infinite recursion. ([1149930](https://issuetracker.unity3d.com/product/unity/issues/guid/1149930/))
- Fixed Trim and Move operations to ensure playable duration is updated upon completion. ([1151894](https://issuetracker.unity3d.com/product/unity/issues/guid/1151894/))
- Fixed options menu icon that was blurry on high-dpi screens. (1154623)
- Track binding field is now larger. (1153446)
- Fixed issue where an empty Timeline window would create new objects on each repaint. (1142894)
- Fixed an issue causing info text to overlap when displaying multiple lines (when trimming + time scaling, for example). (1150863)
- Fixed duration mode not reverting from "Fixed Length" to "Based On Clips" properly. ([1154034](https://issuetracker.unity3d.com/product/unity/issues/guid/1154034/))
- Prevented the PlayableGraph from being created twice when playing a timeline in play mode with the Timeline window opened. (1147247)
- Fixed issue where an exception was thrown when clicking on a SignalEmitter with the Timeline window in asset mode. (1146261)
- A timeline will now be played correctly when building a player with Mono and Managed Stripping Level set higher than Low. ([1133182](https://issuetracker.unity3d.com/product/unity/issues/guid/1133182/))
- The Signal Asset creation dialog will no longer throw exceptions when canceled on macOS. ([1141959](https://issuetracker.unity3d.com/product/unity/issues/guid/1141959/))
- Fixed issue where the Emit Signal property on a Signal Emitter would not get saved correctly. ([1148709](https://issuetracker.unity3d.com/product/unity/issues/guid/1148709/))
- Fixed issue where a Signal Emitter placed at the start of a timeline would be fired twice. ([1149653](https://issuetracker.unity3d.com/product/unity/issues/guid/1149653/))
- Fixed record button state not updating when offset modes are changed. ([1142747](https://issuetracker.unity3d.com/product/unity/issues/guid/1142747/))
- Cleared invalid assets from the Timeline Clipboard when going into or out of PlayMode. (1144473)
- Copying a Control Clip during play mode no longer throws exceptions. (1141581)
- Going to Play Mode while inspecting a Track Asset will no longer throw exceptions. (1141958)
- Resizing Timeline's window no longer affects the zoom value. ([1147150](https://issuetracker.unity3d.com/product/unity/issues/guid/1147150/))
- Snap relaxing now responds to Command on Mac, instead of Control. (1149144)
- Clips will no longer randomly disappear when showing or hiding inline curves. (1141661)
- The global/local time referential button will no longer be shown for a top-level timeline. (1080872)
- Playhead will not be drawn above the bottom scrollbar anymore. (1134016)
- Fixed moving a marker on an Infinite Track will keep the track in infinite mode (1141190)
- Fixed zooming in/out will keep the padding at the beginning of the timeline (1030689)
- Fixed marker UI is the same color and size on infinite track (1139370)
- Fixed Disable the possibility to add Markers to tracks of a Timeline that is ReadOnly (1134463)
- Fixed wrong context menu being shown when right-clicking a marker (1133592)
- Fixed creation of override track to work with multiselection (1133592)

## [1.1.0] - 2019-02-14
*Compatible with Unity 2019.2*
### Added
- ClipEditor, TrackEditor and MarkerEditor classes users can derive from to control visual appearance of custom timeline clips, tracks and markers using the CustomTimelineEditor attribute.
- ClipEditor.GetSubTimelines to allow user created clips that support sub-timelines in editor
- TimelineEditor.selectedClip and TimelineEditor.selectedClips to set and retrieve the currently selected timeline clips
- IPropertyCollector.AddFromName override that takes a component.
- Warning icons to SignalEmitters when they do not reference an asset
- Ability to mute/unmute a Group Track.
- Mute/Unmute only selected track command added for tracks with multiple layers.
- Animate-able Properties on Tracks and Clips can now be edited through inline curves.
- Added loop override on AnimationTrack clips (1140766)
- ReadOnly/Source Control Lock support for Timeline Scene

### Changed
- Control Track display to show a particle system icon when particle systems are being controlled
- Animate-able Properties for clips are no longer edited using by "recording"; they are edited through the inline curves just like tracks.
- AudioTrack properties can now be animated through inline curves.
- Changed Marker show/hide to be undoable. Hide will also unselect markers. (1124661)
- Changed SignalReceivers show their enabled state in the inspector. (1131163)
- Changed Track Context Menu to show "Add Signal Emitter" at the top of the list of Marker commands. (1131166)
- Moved "Add Signal Emitter" and "Add Signal Emitter From Asset" commands out of their sub-menu. (1131166)

### Fixed
- Fixed markers being drawn outside their pane. (1124381)
- Fixed non-public tracks not being recognized by the Timeline Editor. (1122803)
- Fixed keyboard shortcuts for _Frame All_ (default: A) and _Frame Selected_ (default: F) to also apply horizontally ([1126623](https://issuetracker.unity3d.com/product/unity/issues/guid/1126623/))
- Fixed recording getting disabled when selecting a different GameObject while the Timeline Window is not locked. (1123119)
- Fixed time sync between Animation and Timeline windows when clips have non-default timescale or clip-in values. ([930909](https://issuetracker.unity3d.com/product/unity/issues/guid/930909/))
- Fixed animation window link not releasing when deleting the timeline asset. (1127425)
- Fixed an exception being raised when selecting both a Track marker and a Timeline marker at the same time. ([1113006](https://issuetracker.unity3d.com/product/unity/issues/guid/1113006/))
- Fixed the header marker area will so it no longer opens its context menu if it's hidden. (1124351)
- Fixed Signal emitters to show the Signals list when created on override tracks. (1102913)
- Fixed a crash on IL2CPP platforms when the VideoPlayer component is not used. (1129572)
- Fixed Timeline Duration changes in editor not being undoable. (1109279)
- Fixed _Match Offsets_ commands causing improper animation defaults to be applied. (911678)
- Fixed Timeline Inspectors leaving _EditorGUI.showMixedValue_ in the wrong state. ([1123895](https://issuetracker.unity3d.com/product/unity/issues/guid/1123895/))
- Fixed issue where performing undo after moving items on multiple tracks would not undo some items. (1131071)
- Fixed cog icon in the Signal Receiver inspector being blurry. (1130320)
- Fixed Timeline marker track hamburger icon not being centered vertically. (1131112)
- Fixed detection of signal receivers when track is in a group. (1131811)
- Fixed exception being thrown when deleting Signal entries. (1131065)
- Fixed Markers blocking against Clips when moving both Clips and Markers in Ripple mode. (1102594)
- Fixed NullReferenceException being thrown when muting an empty marker track. (1131106)
- Fixed SignalEmitter Inspector losing the Receiver UI when it is locked and another object is selected. (1116041)
- Fixed Marker and Clip appearing to be allowed to move to another track in Ripple mode. (1131123)
- Fixed issue where the Signal Emitter inspector did not show the Signal Receiver UI when placed on the timeline marker track. (1131811)
- Fixed Replace mode not drawing clips when moved together with a Marker. (1132605)
- Fixed inline curves to retain their state when performing undo/redo or keying from the inspector. ([1125443](https://issuetracker.unity3d.com/product/unity/issues/guid/1125443))
- Fixed an issue preventing Timeline from entering preview mode when an Audio Track is present an a full assembly reload is performed. (1132243)
- Fixed an issue where the Marker context menu would show a superfluous line at the bottom. (1132662)
- Fixed an issue preventing Timeline asset to be removed from a locked Timeline Window when a new scene is loaded. (1135073)
- Fixed EaseIn/Out shortcut for clips

## [1.0.0] - 2019-01-28
*Compatible with Unity 2019.1*
### Added
- This is the first release of Timeline, as a Package
- Added API calls to access all AnimationClips used by Timeline.
- Added support in the runtime API to Animate Properties used by template-style PlayableBehaviours used as Mixers.
- Added Markers. Markers are abstract types that represent a single point in time.
- Added Signal Emitters and Signal Assets. Signal Emitters are markers that send a notification, indicated by a SignalAsset, to a GameObject indicating an event has occurred during playback of the Timeline.
- Added Signal Receiver Components. Signal Receivers are MonoBehaviour that listen for Signals from Timeline and respond by invoking UnityEvents.
- Added Signal Tracks. Signal Tracks are Timeline Tracks that are used only for Signal Emitters.

### Fixed
- Signal Receiver will no longer throw exceptions when its inspector is locked ([1114526](https://issuetracker.unity3d.com/product/unity/issues/guid/1114526/))
- Context menu operations will now be applied on all selected tracks (1089820)
- Clip edit mode clutch keys will not get stuck when holding multiple keys at the same time (1097216)
- Marker inspector will be disabled when the marker is collapsed (1102860)
- Clip inspector will no longer throw exceptions when changing values when the inspector is locked (1115984)
- Fixed appearance of muted tracks (1018643)
- Fixed multiple issues where clips and markers were selectable when located under the time ruler and the marker header track ([1117925](https://issuetracker.unity3d.com/product/unity/issues/guid/1117925/), 1102598)
- A marker aligned with the edge of a clip is now easier to select (1102591)
- Changed behaviour of the Timeline Window to apply modifications immediately during Playmode ([922846](https://issuetracker.unity3d.com/product/unity/issues/guid/922846/), 1111908)
- PlayableDirector.played event is now called after entering or exiting Playmode ([1088918](https://issuetracker.unity3d.com/product/unity/issues/guid/1088918/))
- Undoing a paste track operation in a group will no longer corrupt the timeline (1116052)
- The correct context menu will now be displayed on the marker header track (1120857)
- Fixed an issue where a circular reference warning appeared in the Control Clip inspector even if there was no circular reference (1116520)
- Fixed preview mode when animation clips with root curves are used (case 1116297, case 1116007)
- Added option to disable foot IK on animation playable assets (case 1115652)
- Fixed unevaluated animation tracks causing default pose (case 1109118)
- Fixed drawing of Group Tracks when header is off-screen (case 876340)
- Fixed drag and drop of objects inside a group being inserted outside (case 1011381, case 1014774)
