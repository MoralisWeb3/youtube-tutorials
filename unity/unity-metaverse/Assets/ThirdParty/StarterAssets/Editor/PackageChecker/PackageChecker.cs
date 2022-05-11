using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace StarterAssets
{
    public static class PackageChecker
    {
        private static ListRequest clientList;
        private static SearchRequest compatibleList;
        private static List<PackageEntry> packagesToAdd;

        private static AddRequest[] addRequests;
        private static bool[] installRequired;

        private static readonly string EditorFolderRoot = "Assets/StarterAssets/";
        private static readonly string PackagesToImportDataFile = "PackageImportList.txt";
        public static readonly string PackageCheckerScriptingDefine = "STARTER_ASSETS_PACKAGES_CHECKED";

        [InitializeOnLoadMethod]
        private static void CheckPackage()
        {
            // if we dont have the scripting define, it means the check has not been done
            if (!ScriptingDefineUtils.CheckScriptingDefine(PackageCheckerScriptingDefine))
            {
                packagesToAdd = new List<PackageEntry>();
                clientList = null;
                compatibleList = null;

                // find the projects required package list
                var requiredPackagesListFile = Directory.GetFiles(Application.dataPath, PackagesToImportDataFile,
                    SearchOption.AllDirectories);

                // if no PackageImportList.txt exists
                if (requiredPackagesListFile.Length == 0)
                {
                    Debug.LogError(
                        "[Auto Package] : Couldn't find the packages list. Be sure there is a file called PackageImportList in your project");
                }
                else
                {
                    packagesToAdd = new List<PackageEntry>();

                    string packageListPath = requiredPackagesListFile[0];
                    string[] content = File.ReadAllLines(packageListPath);

                    foreach (string line in content)
                    {
                        string[] split = line.Split('@');

                        // if no version is given, return null
                        PackageEntry entry = new PackageEntry
                            {name = split[0], version = split.Length > 1 ? split[1] : null};

                        packagesToAdd.Add(entry);
                    }

                    // Create a file in library that is queried to see if CheckPackage() has been run already
                    ScriptingDefineUtils.SetScriptingDefine(PackageCheckerScriptingDefine);

                    // create a list of compatible packages for current engine version
                    compatibleList = Client.SearchAll();

                    while (!compatibleList.IsCompleted)
                    {
                        if (compatibleList.Status == StatusCode.Failure || compatibleList.Error != null)
                        {
                            Debug.LogError(compatibleList.Error.message);
                            break;
                        }
                    }

                    // create a list of packages found in the engine
                    clientList = Client.List();

                    while (!clientList.IsCompleted)
                    {
                        if (clientList.Status == StatusCode.Failure || clientList.Error != null)
                        {
                            Debug.LogError(clientList.Error.message);
                            break;
                        }
                    }

                    addRequests = new AddRequest[packagesToAdd.Count];
                    installRequired = new bool[packagesToAdd.Count];

                    // default new packages to install = false. we will mark true after validating they're required
                    for (int i = 0; i < installRequired.Length; i++)
                    {
                        installRequired[i] = false;
                    }

                    // build data collections compatible packages for this project, and packages within the project
                    List<PackageInfo> compatiblePackages =
                        new List<PackageInfo>();
                    List<PackageInfo> clientPackages =
                        new List<PackageInfo>();

                    foreach (var result in compatibleList.Result)
                    {
                        compatiblePackages.Add(result);
                    }

                    foreach (var result in clientList.Result)
                    {
                        clientPackages.Add(result);
                    }

                    // check for the latest verified package version for each package that is missing a version
                    for (int i = 0; i < packagesToAdd.Count; i++)
                    {
                        // if a version number is not provided
                        if (packagesToAdd[i].version == null)
                        {
                            foreach (var package in compatiblePackages)
                            {
                                // if no latest verified version found, PackageChecker will just install latest release
                                if (packagesToAdd[i].name == package.name && package.versions.verified != string.Empty)
                                {
                                    // add latest verified version number to the packagetoadd list version
                                    // so that we get the latest verified version only
                                    packagesToAdd[i].version = package.versions.verified;

                                    // add to our install list
                                    installRequired[i] = true;

                                    //Debug.Log(string.Format("Requested {0}. Latest verified compatible package found: {1}",
                                    //    packagesToAdd[i].name, packagesToAdd[i].version));
                                }
                            }
                        }

                        // we don't need to catch packages that are not installed as their latest version has been collected
                        // from the campatiblelist result
                        foreach (var package in clientPackages)
                        {
                            if (packagesToAdd[i].name == package.name)
                            {
                                // see what version we have installed
                                switch (CompareVersion(packagesToAdd[i].version, package.version))
                                {
                                    // latest verified is ahead of installed version
                                    case 1:
                                        installRequired[i] = EditorUtility.DisplayDialog("Confirm Package Upgrade",
                                            $"The version of \"{packagesToAdd[i].name}\" in this project is {package.version}. The latest verified " +
                                            $"version is {packagesToAdd[i].version}. Would you like to upgrade it to the latest version? (Recommended)",
                                            "Yes", "No");

                                        Debug.Log(
                                            $"<b>Package version behind</b>: {package.packageId} is behind latest verified " +
                                            $"version {package.versions.verified}. prompting user install");
                                        break;

                                    // latest verified matches installed version
                                    case 0:
                                        installRequired[i] = false;

                                        Debug.Log(
                                            $"<b>Package version match</b>: {package.packageId} matches latest verified version " +
                                            $"{package.versions.verified}. Skipped install");
                                        break;

                                    // latest verified is behind installed version
                                    case -1:
                                        installRequired[i] = EditorUtility.DisplayDialog("Confirm Package Downgrade",
                                            $"The version of \"{packagesToAdd[i].name}\" in this project is {package.version}. The latest verified version is {packagesToAdd[i].version}. " +
                                            $"{package.version} is unverified. Would you like to downgrade it to the latest verified version? " +
                                            "(Recommended)", "Yes", "No");

                                        Debug.Log(
                                            $"<b>Package version ahead</b>: {package.packageId} is newer than latest verified " +
                                            $"version {package.versions.verified}, skipped install");
                                        break;
                                }
                            }
                        }
                    }

                    // install our packages and versions
                    for (int i = 0; i < packagesToAdd.Count; i++)
                    {
                        if (installRequired[i])
                        {
                            addRequests[i] = InstallSelectedPackage(packagesToAdd[i].name, packagesToAdd[i].version);
                        }
                    }

                    ReimportPackagesByKeyword();
                }
            }
        }

        private static AddRequest InstallSelectedPackage(string packageName, string packageVersion)
        {
            if (packageVersion != null)
            {
                packageName = packageName + "@" + packageVersion;
                Debug.Log($"<b>Adding package</b>: {packageName}");
            }

            AddRequest newPackage = Client.Add(packageName);

            while (!newPackage.IsCompleted)
            {
                if (newPackage.Status == StatusCode.Failure || newPackage.Error != null)
                {
                    Debug.LogError(newPackage.Error.message);
                    return null;
                }
            }

            return newPackage;
        }

        private static void ReimportPackagesByKeyword()
        {
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(EditorFolderRoot, ImportAssetOptions.ImportRecursive);
        }

        public static int CompareVersion(string latestVerifiedVersion, string projectVersion)
        {
            string[] latestVersionSplit = latestVerifiedVersion.Split('.');
            string[] projectVersionSplit = projectVersion.Split('.');
            int iteratorA = 0;
            int iteratorB = 0;

            while (iteratorA < latestVersionSplit.Length || iteratorB < projectVersionSplit.Length)
            {
                int latestVerified = 0;
                int installed = 0;

                if (iteratorA < latestVersionSplit.Length)
                {
                    latestVerified = Convert.ToInt32(latestVersionSplit[iteratorA]);
                }

                if (iteratorB < projectVersionSplit.Length)
                {
                    installed = Convert.ToInt32(projectVersionSplit[iteratorB]);
                }

                // latest verified is ahead of installed version
                if (latestVerified > installed) return 1;

                // latest verified is behind installed version
                if (latestVerified < installed) return -1;

                iteratorA++;
                iteratorB++;
            }

            // if the version is the same
            return 0;
        }

        public class PackageEntry
        {
            public string name;
            public string version;
        }
    }
}