# Changelog
All notable changes to this package will be documented in this file.

## [1.15.7] - 2021-12-02

Plastic SCM for Unity is now available as part of the Version Control Package! You can enable Plastic SCM via Window > Plastic SCM to get started!
If you have previously used the Unity Asset Store Plastic SCM plug-in, you can now simply use this package. Make sure you delete the plug-in from your project.
Removing a previously added Plastic SCM Asset Store Plug-In:
- Select the PlasticSCM folder in the Assets\Plugins folder on the Project tab, then click Edit > Delete
- Close the Unity Editor and open your project again. You will find the Plastic SCM menu item in the Window menu.

### Added
- Added option to "Save Revision as" to the context menu in the changesets view
- Added incoming changes overview bar for Gluon workspace

### Changed
- Moved Plastic Package settings to the Unity Project Settings window
- Refined styling for Updating Workplace success state
- Updated texts for empty state and overview bar
- Removed Incoming Changes notification from empty state
- Updated the text for Forced Checkout option
- Refined the status overlay icons
- Updated the refresh icon on the toolbar
- Updated the texts for empty checkin message dialog

### Fixed
- Fixed capitalization of Pending Changes and File History tab names
- Fixed the amount of spacing after the Item column title in the Pending Changes tab
- Removed pin striping from line items in File History tab
- Fixed project view context menu and icons missing after Collaborate project migration
- Fixed migrated projects not downloading correctly from Unity Hub

## [1.15.4] - 2021-11-10

Plastic SCM for Unity is now available as part of the Version Control Package! You can enable Plastic SCM via Window > Plastic SCM to get started!
If you have previously used the Unity Asset Store Plastic SCM plug-in, you can now simply use this package. Make sure you delete the plug-in from your project.
Removing a previously added Plastic SCM Asset Store Plug-In:
- Select the PlasticSCM folder in the Assets\Plugins folder on the Project tab, then click Edit > Delete
- Close the Unity Editor and open your project again. You will find the Plastic SCM menu item in the Window menu.

### Added
- Add option to "Add to ignore file" in context menu in the project view
- Added empty state message for Pending Changes tab
- Added success state message for Pending Changes tab
- Added metrics for Branches tab functionalities

### Changed
- Removed pinstriping in the Gluon Incoming Changes window
- Removed the “Nothing to download” bar from the Incoming Changes window when there are no items to download
- Changed the default metadata columns shown in the Incoming Changes screen
- Updated the alignment of sorting arrows to the right of the column

### Fixed
- Fixed UI overlays in Project view missing on changed assets when force checkout is disabled
- Fixed console error when selecting object in Scene view hierarchy or creating a new asset
- Fixed NullReferenceException after closing the Plastic SCM window

## [1.15.1] - 2021-10-21

Plastic SCM for Unity is now available as part of the Version Control Package! You can enable Plastic SCM via Window > Plastic SCM to get started!
If you have previously used the Unity Asset Store Plastic SCM plug-in, you can now simply use this package. Make sure you delete the plug-in from your project.
Removing a previously added Plastic SCM Asset Store Plug-In:
- Select the PlasticSCM folder in the Assets\Plugins folder on the Project tab, then click Edit > Delete
- Close the Unity Editor and open your project again. You will find the Plastic SCM menu item in the Window menu.

### Added
- Added visual overview bar to the incoming changes tab
- Added progress dialog for the migration process
- Added Branches tab that shows a list of all branches in the repository
- Added option and dialog to create a child branch from selected branch.
- Added option to switch to another branch
- Added option and dialog to rename a branch
- Added option to delete a branch
- Added a preference to save if the window should open the Branches tab by default
- Added metrics for Plastic SCM installation window usage

### Changed
- Updated texts for workspace modes selection and checkin comment box
- Updated status bar notification icons

### Fixed
- Fixed inverted text for the force checkout option
- Fixed typing capital O in checkin comment would open the selected item
- Fixed loading indicator not centered on Plastic SCM installation window
- Fixed installing Plastic SCM would sign out user from the plugin
- Removed extra refresh button on Gluon's Incoming Changes tab
- Fixed loading indicator not centered on Plastic SCM installation window
- Fixed missing Plastic SCM window option when user is not signed in on Unity Hub
- Removed meta file warning message for the deleted Beta folder
- Fixed Plastic SCM menu missing from Project view context menu

## [1.13.5] - 2021-09-27

Plastic SCM for Unity is now available as part of the Version Control Package! You can enable Plastic SCM via Window > Plastic SCM to get started!
If you have previously used the Unity Asset Store Plastic SCM plug-in, you can now simply use this package. Make sure you delete the plug-in from your project.
Removing a previously added Plastic SCM Asset Store Plug-In:
- Select the PlasticSCM folder in the Assets\Plugins folder on the Project tab, then click Edit > Delete
- Close the Unity Editor and open your project again. You will find the Plastic SCM menu item in the Window menu.

### Added
- Added workspace migration from Collab to Plastic which can be done with or without Plastic installed
- Added notification status icons
- Added light and dark mode versions of avatar icon

### Changed
- Updated texts for migration
- Improved usage analytics around Editor and Plugin version
- Workspace Migration Adjustments

### Fixed
- Renamed the CoreServices namespace so it doesn't conflict with other packages
- Devex integration to properly depend on Core
- Fixed some situations where the history window would be blank
- Fixed missing Enterprise login link
- Fixed low resolution icons in light theme

## [1.11.2] - 2021-08-27

Plastic SCM for Unity is now available as part of the Version Control Package! You can enable Plastic SCM via Window > Plastic SCM to get started!
If you have previously used the Unity Asset Store Plastic SCM plug-in, you can now simply use this package. Make sure you delete the plug-in from your project.
Removing a previously added Plastic SCM Asset Store Plug-In:
- Select the PlasticSCM folder in the Assets\Plugins folder on the Project tab, then click Edit > Delete
- Close the Unity Editor and open your project again. You will find the Plastic SCM menu item in the Window menu.

### Added
- Added horizontal scroll bar to Changesets list for easier viewing
- Added auto-login for SSO credentials handler
- Added metrics for changeset tab usage
- Added metrics for checkin actions
- Added new Undo icon
- Added missing API documentation
- Added ability to modify assets without checkout
- Added ability to allow empty checkin messages
- Added empty checking message localization
- Added Plastic toolbar button to Unity editor
- Added notification icon for incoming changes to Plastic toolbar button

### Changed
- Removed the unneeded refresh button from History Tab
- Moved search bar to the top right global icon section in all tabs
- Updated capitalization of options in the Settings context menu
- Updated tab button styling to be consistent with Unity Editor conventions
- Status bar visible across all tabs
- Moved refresh button to the toolbar at the top right corner of the window
- Moved changesets time period selector to the right corner of the window
- Removed "Changes of changeset" header on the Changesets tab
- Moved number of selected items next to "Item" metadata title on the Pending Changes tab
- Improved refresh icon resolution
- Changed changesets detail to appear in vertical column
- Reduced default number of columns in changesets tab
- The number of changesets is no longer displayed in changesets tab
- Changed Launch branch explorer into an icon with tooltip
- Removed the hide changes button in changesets tab
- Moved incoming change prompt and button into a status bar
- Changed "Launch Plastic" to "Launch Plastic SCM" in options menu
- Wording change for plastic installation
- Updated file status icons

### Fixed
- Fixed a bug where the Texture2D error would pop up after downloading a project
- Fixed a bug when context menu would sometimes disappear
- Fixed small textbox on checkin dialog when launched from context menu
- Fixed a workspace NullReferenceException bug
- Fixed notification icon not showing on Plastic window
- Fixed auto login errors not showing up for users
- Fixed unexpected error message after user switched workspace to a label

## [1.9.0] - 2021-07-13

Plastic SCM for Unity is now available as part of the Version Control Package! You can enable Plastic SCM via Window > Plastic SCM to get started!
If you have previously used the Unity Asset Store Plastic SCM plug-in, you can now simply use this package. Make sure you delete the plug-in from your project.
Removing a previously added Plastic SCM Asset Store Plug-In:
- Select the PlasticSCM folder in the Assets\Plugins folder on the Project tab, then click Edit > Delete
- Close the Unity Editor and open your project again. You will find the Plastic SCM menu item in the Window menu.

### Added
- Added Checkin and Update confirmation notification
- Added auto sign in when logged into Unity account

### Changed
- Simplified UI: decluttered UI
- Improved load time performance

### Fixed
- Fixed view not switching to workspace after creating an Enterprise Gluon workspace
- Fixed contextual menu not showing up in project view
- Fixed SSO renew token after password change
- Fixed some namespace collisions with Antlr3

## [1.7.1] - 2021-06-25

Plastic SCM for Unity is now available as part of the Version Control Package! You can enable Plastic SCM via Window > Plastic SCM to get started!
If you have previously used the Unity Asset Store Plastic SCM plug-in, you can now simply use this package. Make sure you delete the plug-in from your project.
Removing a previously added Plastic SCM Asset Store Plug-In:
- Select the PlasticSCM folder in the Assets\Plugins folder on the Project tab, then click Edit > Delete
- Close the Unity Editor and open your project again. You will find the Plastic SCM menu item in the Window menu.
### Added
- Added support for inviting other members. This option is available from the gear / settings icon.
- Added support for signing in with Cloud Edition. This is available during the onboarding screen if you have never signed in.
- Added support for turning off Plastic in their project. This option removes the Plastic metadata from your directory. This option is available under Assets > Plastic SCM > Turn off Plastic SCM
- Added notification on the Plastic SCM tab title to indicate incoming changes. Users will no longer need to have the Plastic SCM window visible to know there are incoming changes.
- Auto configuration of SSO
- Added date column in incoming changes
### Changed
- Updating license to better conform with expected customer usage.
- Updated documentation file to meet standards.
- Updated third-party usage.
- No longer requires downloading of the full Plastic client. Basic features will work without additional installation. Features that require the full Plastic client will allow download and install as needed.
- Usability improvements around checking in code
- Improved update workspace tab UX
- Plastic SCM context menu is now available even if the Plastic SCM window is closed
### Fixed
- Stability and performance improvements

## [1.5.7] - 2021-04-07
### Unreleased
- The Version Control package will be expanding to include both Collaborate and Plastic SCM version control interfaces. This release is preparing for that move and contains no new functionality or bug fixes for Collaborate.
### Changed
- Collaborate Package renamed to Version Control with changes to package display name and description.
### Fixed
- Fixed NPE when updating the version of the Collab package.

## [1.3.9] - 2020-07-13
### Fixed
- Unnecessary use of texture compression in icons that slowed down platform switching
- Update publish button state when selected changes update
- Use colorized icons when changes are available.

## [1.3.8] - 2020-06-08
### Fixed
- Fix incorrect priority of error messages
- Fix Collab button being stuck in inprogress state
- Fix error when partially publishing without the window open

## [1.3.7] - 2020-01-30
### Changed
- Bulk revert is now supported.
- Collab is blocked in play mode.
### Fixed
- Fixed services window's links to open Collab.

## [1.3.6] - 2020-01-21
### Fixed
- Fixed compile errors when removing the NUnit package by removing unnecessary references.

## [1.3.5] - 2020-01-08
### Fixed
- Fix "accept mine" / "accept remote" icon swap in conflicts view.

## [1.3.4] - 2019-12-16
### Changed
- Window state is no longer restored after the window is closed and opened.
### Fixed
- History tab failing to load on startup if it is left open in the previous session.
- Progress bar percentage not matching the bar.
- History list correctly updates after a new revision is published.
- UI instabilities when restoring or going back to a revision with a different package manifest.
- Improve handling of changes to the project id.

## [1.3.3] - 2019-12-10
### Changed
- Disable UI test cases that can be unstable.

## [1.3.2] - 2019-12-05
### Changed
- Update UX to UIElements.
- Increased minimum supported version to 2020.1.
- Update Documentation to required standards.

## [1.2.16] - 2019-02-11
### Fixed
- Update stylesheet to pass USS validation

## [1.2.15] - 2018-11-16
### Changed
- Added support for non-experimental UIElements.

## [1.2.11] - 2018-09-04
### Fixed
- Made some performance improvements to reduce impact on ReloadAssemblies.

## [1.2.9] - 2018-08-13
### Fixed
- Test issues for the Collab History Window are now fixed.

## [1.2.7] - 2018-08-07
### Fixed
- Toolbar drop-down will no longer show up when package is uninstalled.

## [1.2.6] - 2018-06-15
### Fixed
- Fixed an issue where Collab's History window wouldn't load properly.

## [1.2.5] - 2018-05-21
This is the first release of *Unity Package CollabProxy*.

### Added
- Collab history and toolbar windows
- Collab view and presenter classes
- Collab Editor tests for view and presenter
