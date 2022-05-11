# Code Editor Package for Rider

## [2.0.7] - 2020-08-18

Improve performance
Add support for asmdef Root Namespace in .csproj generation
ProjectGeneration for custom roslyn analysers https://docs.unity3d.com/2020.2/Documentation/Manual/roslyn-analyzers.html
Switch target platform in Unity would regenerate csproj files (https://github.com/JetBrains/resharper-unity/issues/1740)


## [2.0.6] - 2020-08-10

Improve performance
Add support for asmdef Root Namespace in .csproj generation
ProjectGeneration for custom roslyn analysers https://docs.unity3d.com/2020.2/Documentation/Manual/roslyn-analyzers.html
Switch target platform in Unity would regenerate csproj files (https://github.com/JetBrains/resharper-unity/issues/1740)


## [2.0.5] - 2020-05-27

Fix Regression in 2.0.3: In Unity 2019.2.9 on Mac, changing csproj and calling AssetDatabase.Refresh is not regenerating csproj.
Regenerate projects on changes in manifest.json and Project Settings (EditorOnlyScriptingUserSettings.json) (#51)
Fix: Assembly references to package assemblies break IDE projects.
Fix: Reporting test duration.


## [2.0.2] - 2020-03-18

fix bug in searching Rider path on MacOS


## [2.0.1] - 2020-03-05

Speed improvements,
ProjectTypeGuids for unity-generated project
Improve UI for Project Generation settings
Changes in csc.rsp would cause project-generation
Remove NoWarn 0169 from generated csproj
Support custom JetBrains Toolbox installation location

## [1.2.1] - 2019-12-09

- Load optimised EditorPlugin version compiled to net 461, with fallback to previous version.
- On ExternalEditor settings page: reorder Generate all ... after Extensions handled
- Better presentation for Rider of some version in ExternalEditors list
- Initial support for Code Coverage with dotCover plugin in Rider
- Added support for Player Project generation

## [1.1.4] - 2019-11-21
 - Fix warning - unreachable code

## [1.1.3] - 2019-10-17

 - Update External Editor, when new toolbox build was installed
 - Add xaml to default list of extensions to include in csproj
 - Avoid initializing Rider package in secondary Unity process, which does Asset processing
 - Reflect multiple csc.rsp arguments to generated csproj files: https://github.com/JetBrains/resharper-unity/issues/1337
 - Setting, which allowed to override LangVersion removed in favor of langversion in csc.rsp
 - Environment.NewLine is used in generated project files instead of Windows line separator.

## [1.1.2] - 2019-09-18

performance optimizations:
 - avoid multiple evaluations
 - avoid reflection in DisableSyncSolutionOnceCallBack
 - project generation optimization
fixes:
 - avoid compilation error with incompatible `Test Framework` package

## [1.1.1] - 2019-08-26

parse nowarn in csc.rsp
warning, when Unity was started from Rider, but external editor was different
improved unit test support
workaround to avoid Unity internal project-generation (fix #28)


## [1.1.0] - 2019-07-02

new setting to manage list of extensions to be opened with Rider
avoid breaking everything on any unhandled exception in RiderScriptEditor cctor
hide Rider settings, when different Editor is selected
dynamically load only newer rider plugins
path detection (work on unix symlinks)
speed up for project generation
lots of bug fixing

## [1.0.8] - 2019-05-20

Fix NullReferenceException when External editor was pointing to non-existing Rider everything was broken by null-ref.

## [1.0.7] - 2019-05-16

Initial migration steps from rider plugin to package.
Fix OSX check and opening of files.

## [1.0.6] - 2019-04-30

Ensure asset database is refreshed when generating csproj and solution files.

## [1.0.5] - 2019-04-27

Add support for generating all csproj files.

## [1.0.4] - 2019-04-18

Fix relative package paths.
Fix opening editor on mac.

## [1.0.3] - 2019-04-12

Fixing null reference issue for callbacks to Asset pipeline.

## [1.0.2] - 2019-01-01

### This is the first release of *Unity Package rider_editor*.

Using the newly created api to integrate Rider with Unity.
