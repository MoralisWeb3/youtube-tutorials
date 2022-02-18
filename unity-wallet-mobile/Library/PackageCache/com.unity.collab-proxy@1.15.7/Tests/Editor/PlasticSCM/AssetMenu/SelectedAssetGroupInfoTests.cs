using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using UnityEditor.VersionControl;

using Codice.Client.Commands.WkTree;
using Codice.CM.Common;
using PlasticGui;

using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Tests.Editor.Mock;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;

namespace Unity.PlasticSCM.Tests.Editor.AssetMenu
{
    [TestFixture]
    class SelectedAssetGroupInfoTests
    {
        [Test]
        public void TestSelectedCount()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.Controlled();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.AreEqual(2, groupInfo.SelectedCount);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsCheckedInSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.Controlled();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsTrue(groupInfo.IsCheckedInSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }

        }

        [Test]
        public void TestIsNotCheckedInSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.CheckedOut();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsFalse(groupInfo.IsCheckedInSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsControlledSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.Controlled();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsTrue(groupInfo.IsControlledSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsNotControlledSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsFalse(groupInfo.IsControlledSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestHasAnyAddedInSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.Added();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsTrue(groupInfo.HasAnyAddedInSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestHasAnyNoAddedInSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.Controlled();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsFalse(groupInfo.HasAnyAddedInSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsFileSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.Controlled();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsTrue(groupInfo.IsFileSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsNotFileSelection()
        {
            string barPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");

                Directory.CreateDirectory(barPath);

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsFalse(groupInfo.IsFileSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());

                if (Directory.Exists(barPath))
                    Directory.Delete(barPath);
            }
        }

        [Test]
        public void TestHasNotAnyLockedRemoteInSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.Controlled();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                AssetStatusCacheMock assetStatusCache = new AssetStatusCacheMock();

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        assetStatusCache);

                Assert.IsFalse(groupInfo.HasAnyRemoteLockedInSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestHasAnyLockedRemoteInSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.Controlled();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                AssetStatusCacheMock assetStatusCache = new AssetStatusCacheMock();

                assetStatusCache.SetStatus(fooPath, AssetStatus.LockedRemote);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        assetStatusCache);

                Assert.IsTrue(groupInfo.HasAnyRemoteLockedInSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsCheckedOutSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.CheckedOut();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.CheckedOut();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsTrue(groupInfo.IsCheckedOutSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsNotCheckedOutSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.CheckedOut();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsFalse(groupInfo.IsCheckedOutSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsPrivateSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                apiMock.SetupGetWorkspaceTreeNode(fooPath, null);
                apiMock.SetupGetWorkspaceTreeNode(barPath, null);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsTrue(groupInfo.IsPrivateSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsNotPrivateSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Controlled();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, null);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsFalse(groupInfo.IsPrivateSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsAddedSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Added();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.Added();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsTrue(groupInfo.IsAddedSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        [Test]
        public void TestIsNotAddedSelection()
        {
            try
            {
                PlasticApiMock apiMock = new PlasticApiMock();

                PlasticGui.Plastic.InitializeAPIForTesting(apiMock);

                string fooPath = Path.Combine(Path.GetTempPath(), "foo.c");
                string barPath = Path.Combine(Path.GetTempPath(), "bar.c");

                Asset fooAsset = new Asset(fooPath);
                Asset barAsset = new Asset(barPath);

                WorkspaceTreeNode fooNode = BuildWorkspaceTreeNode.Added();
                WorkspaceTreeNode barNode = BuildWorkspaceTreeNode.CheckedOut();

                apiMock.SetupGetWorkspaceTreeNode(fooPath, fooNode);
                apiMock.SetupGetWorkspaceTreeNode(barPath, barNode);
                apiMock.SetupGetWorkingBranch(new BranchInfo());

                AssetList assetList = new AssetList();
                assetList.Add(fooAsset);
                assetList.Add(barAsset);

                SelectedAssetGroupInfo groupInfo =
                    SelectedAssetGroupInfo.BuildFromAssetList(
                        assetList,
                        new AssetStatusCacheMock());

                Assert.IsFalse(groupInfo.IsAddedSelection);
            }
            finally
            {
                PlasticGui.Plastic.InitializeAPIForTesting(new PlasticAPI());
            }
        }

        class AssetStatusCacheMock : IAssetStatusCache
        {
            internal void SetStatus(string fullPath, AssetStatus status)
            {
                mData.Add(fullPath, status);
            }

            AssetStatus IAssetStatusCache.GetStatusForPath(string fullPath)
            {
                AssetStatus status = AssetStatus.None;

                mData.TryGetValue(fullPath, out status);

                return status;
            }

            AssetStatus IAssetStatusCache.GetStatusForGuid(string guid)
            {
                return AssetStatus.None;
            }

            LockStatusData IAssetStatusCache.GetLockStatusData(string guid)
            {
                return null;
            }
            LockStatusData IAssetStatusCache.GetLockStatusDataForPath(string path)
            {
                return null;
            }

            void IAssetStatusCache.Clear()
            {
                mData.Clear();
            }

            Dictionary<string, AssetStatus> mData =
                new Dictionary<string, AssetStatus>();
        }
    }
}
