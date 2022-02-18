using System.Collections.Generic;

using NUnit.Framework;

using Codice.Client.Commands;
using Codice.Client.Commands.Mount;
using Codice.CM.Common;
using Codice.CM.Common.Merge;
using Codice.Utils;
using PlasticGui.WorkspaceWindow.Diff;
using Unity.PlasticSCM.Editor.Views.Diff;

namespace Unity.PlasticSCM.Tests.Editor.Views.Diff
{
    [TestFixture]
    class UnityDiffTreeTests
    {
        [Test]
        public void TestAddedNoMeta()
        {
            ClientDiff added = Build.Added("/foo/bar.c");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(added);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                added,
                ChangeCategoryType.Added,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Added not found");

            Assert.IsFalse(
                diffTree.HasMeta(clientDiffInfo),
                "Meta exists");
        }

        [Test]
        public void TestChangedNoMeta()
        {
            ClientDiff changed = Build.Changed("/foo/bar.c");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(changed);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                changed,
                ChangeCategoryType.Changed,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Changed not found");

            Assert.IsFalse(
                diffTree.HasMeta(clientDiffInfo),
                "Meta exists");
        }

        [Test]
        public void TestDeletedNoMeta()
        {
            ClientDiff deleted = Build.Deleted("/foo/bar.c");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(deleted);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                deleted,
                ChangeCategoryType.Deleted,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Deleted not found");

            Assert.IsFalse(
                diffTree.HasMeta(clientDiffInfo),
                "Meta exists");
        }

        [Test]
        public void TestMovedNoMeta()
        {
            ClientDiff moved = Build.Moved("/foo/bar.src.c", "/foo/bar.c");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(moved);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                moved,
                ChangeCategoryType.Moved,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Moved not found");

            Assert.IsFalse(
                diffTree.HasMeta(clientDiffInfo),
                "Meta exists");
        }

        [Test]
        public void TestFsPermissionsNoMeta()
        {
            ClientDiff changedFsPermissions = Build.ChangedFsPermissions("/foo/bar.c");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(changedFsPermissions);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                changedFsPermissions,
                ChangeCategoryType.FSProtection,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Changed fs protection not found");

            Assert.IsFalse(
                diffTree.HasMeta(clientDiffInfo),
                "Meta exists");
        }

        [Test]
        public void TestMergedNoMeta()
        {
            ClientDiff merged = Build.Merged("/foo/bar.c");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(merged);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                merged,
                ChangeCategoryType.Merged,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Merged not found");

            Assert.IsFalse(
                diffTree.HasMeta(clientDiffInfo),
                "Meta exists");
        }

        [Test]
        public void TestAddedWithMeta()
        {
            ClientDiff added = Build.Added("/foo/bar.c");
            ClientDiff addedMeta = Build.Added("/foo/bar.c.meta");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(added);
            diffs.Add(addedMeta);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                added,
                ChangeCategoryType.Added,
                diffTree);

            ClientDiffInfo clientDiffInfoMeta = FindClientDiffInfo.FromClientDiff(
                addedMeta,
                ChangeCategoryType.Added,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Added not found");

            Assert.IsNull(
                clientDiffInfoMeta,
                "Added meta found");

            Assert.IsTrue(
                diffTree.HasMeta(clientDiffInfo),
                "Meta does not exist");
        }

        [Test]
        public void TestChangedWithMeta()
        {
            ClientDiff changed = Build.Changed("/foo/bar.c");
            ClientDiff changedMeta = Build.Changed("/foo/bar.c.meta");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(changed);
            diffs.Add(changedMeta);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                changed,
                ChangeCategoryType.Changed,
                diffTree);

            ClientDiffInfo clientDiffInfoMeta = FindClientDiffInfo.FromClientDiff(
                changedMeta,
                ChangeCategoryType.Changed,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Changed not found");

            Assert.IsNull(
                clientDiffInfoMeta,
                "Changed meta found");

            Assert.IsTrue(
                diffTree.HasMeta(clientDiffInfo),
                "Meta does not exist");
        }

        [Test]
        public void TestDeletedWithMeta()
        {
            ClientDiff deleted = Build.Deleted("/foo/bar.c");
            ClientDiff deletedMeta = Build.Deleted("/foo/bar.c.meta");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(deleted);
            diffs.Add(deletedMeta);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                deleted,
                ChangeCategoryType.Deleted,
                diffTree);

            ClientDiffInfo clientDiffInfoMeta = FindClientDiffInfo.FromClientDiff(
                deletedMeta,
                ChangeCategoryType.Deleted,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Deleted not found");

            Assert.IsNull(
                clientDiffInfoMeta,
                "Deleted meta found");

            Assert.IsTrue(
                diffTree.HasMeta(clientDiffInfo),
                "Meta does not exist");
        }

        [Test]
        public void TestMovedWithMeta()
        {
            ClientDiff moved = Build.Moved("/foo/bar.src.c", "/foo/bar.c");
            ClientDiff movedMeta = Build.Moved("/foo/bar.src.c.meta", "/foo/bar.c.meta");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(moved);
            diffs.Add(movedMeta);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                moved,
                ChangeCategoryType.Moved,
                diffTree);

            ClientDiffInfo clientDiffInfoMeta = FindClientDiffInfo.FromClientDiff(
                movedMeta,
                ChangeCategoryType.Moved,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Moved not found");

            Assert.IsNull(
                clientDiffInfoMeta,
                "Moved meta found");

            Assert.IsTrue(
                diffTree.HasMeta(clientDiffInfo),
                "Meta does not exist");
        }

        [Test]
        public void TestFsPermissionsWithMeta()
        {
            ClientDiff fsPermissionsChange = Build.ChangedFsPermissions("/foo/bar.c");
            ClientDiff fsPermissionsChangeMeta = Build.ChangedFsPermissions("/foo/bar.c.meta");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(fsPermissionsChange);
            diffs.Add(fsPermissionsChangeMeta);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                fsPermissionsChange,
                ChangeCategoryType.FSProtection,
                diffTree);

            ClientDiffInfo clientDiffInfoMeta = FindClientDiffInfo.FromClientDiff(
                fsPermissionsChangeMeta,
                ChangeCategoryType.FSProtection,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Change fs permissions not found");

            Assert.IsNull(
                clientDiffInfoMeta,
                "Change fs permissions meta found");

            Assert.IsTrue(
                diffTree.HasMeta(clientDiffInfo),
                "Meta does not exist");
        }

        [Test]
        public void TestMergedWithMeta()
        {
            ClientDiff merged = Build.Merged("/foo/bar.c");
            ClientDiff mergedMeta = Build.Merged("/foo/bar.c.meta");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(merged);
            diffs.Add(mergedMeta);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                merged,
                ChangeCategoryType.Merged,
                diffTree);

            ClientDiffInfo clientDiffInfoMeta = FindClientDiffInfo.FromClientDiff(
                mergedMeta,
                ChangeCategoryType.Merged,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Merged not found");

            Assert.IsNull(
                clientDiffInfoMeta,
                "Merged meta found");

            Assert.IsTrue(
                diffTree.HasMeta(clientDiffInfo),
                "Meta does not exist");
        }

        [Test]
        public void TestChangeWithDeletedMeta()
        {
            ClientDiff changed = Build.Changed("/foo/bar.c");
            ClientDiff deletedMeta = Build.Deleted("/foo/bar.c.meta");

            List<ClientDiff> diffs = new List<ClientDiff>();
            diffs.Add(changed);
            diffs.Add(deletedMeta);

            UnityDiffTree diffTree = new UnityDiffTree();
            diffTree.BuildCategories(diffs, false);

            ClientDiffInfo clientDiffInfo = FindClientDiffInfo.FromClientDiff(
                changed,
                ChangeCategoryType.Changed,
                diffTree);

            ClientDiffInfo clientDiffInfoMeta = FindClientDiffInfo.FromClientDiff(
                deletedMeta,
                ChangeCategoryType.Deleted,
                diffTree);

            Assert.IsNotNull(
                clientDiffInfo,
                "Changed not found");

            Assert.IsNotNull(
                clientDiffInfoMeta,
                "Deleted meta not found");

            Assert.IsFalse(
                diffTree.HasMeta(clientDiffInfo),
                "Meta exists");
        }

        static class Build
        {
            internal static ClientDiff Added(string path)
            {
                return new ClientDiff(
                    BuildRootMountPoint(),
                    BuildAddedDifference(path),
                    null);
            }


            internal static ClientDiff Changed(string path)
            {
                return new ClientDiff(
                    BuildRootMountPoint(),
                    BuildChangedDifference(path),
                    null);
            }

            internal static ClientDiff ChangedFsPermissions(string path)
            {
                return new ClientDiff(
                    BuildRootMountPoint(),
                    BuildChangedFsPermissionsDifference(path),
                    null);
            }

            internal static ClientDiff Deleted(string path)
            {
                return new ClientDiff(
                    BuildRootMountPoint(),
                    BuildDeletedDifference(path),
                    null);
            }

            internal static ClientDiff Merged(string path)
            {
                return new ClientDiff(
                    BuildRootMountPoint(),
                    BuildChangedDifference(path),
                    new ItemDiffMerge(new List<RevisionMerge>()
                    {
                        new RevisionMerge()
                        {
                            SrcRevId = 15,
                            DstRevId = 16,
                            Link = new MergeLinkRealizationInfo(),
                            Type = MergeChangeType.Merged
                        }
                    }));
            }

            internal static ClientDiff Moved(
                string srcPath,
                string dstPath)
            {
                return new ClientDiff(
                    BuildRootMountPoint(),
                    BuildMovedDifference(srcPath, dstPath),
                    null);
            }

            static Difference BuildAddedDifference(string path)
            {
                return new DiffChanged(
                    new RevisionInfo() { Type = EnumRevisionType.enTextFile },
                    -1,
                    path,
                    -1,
                    Difference.DiffNodeStatus.Added);
            }

            static Difference BuildChangedDifference(string path)
            {
                return new DiffChanged(
                    new RevisionInfo() { Type = EnumRevisionType.enTextFile },
                    -1,
                    path,
                    -1,
                    Difference.DiffNodeStatus.Changed);
            }

            static Difference BuildChangedFsPermissionsDifference(string path)
            {
                return new DiffChanged(
                    new RevisionInfo() { Type = EnumRevisionType.enTextFile },
                    -1,
                    path,
                    -1,
                    Difference.DiffNodeStatus.Changed,
                    null,
                    456);
            }

            static Difference BuildDeletedDifference(string path)
            {
                return new DiffChanged(
                    new RevisionInfo() { Type = EnumRevisionType.enTextFile },
                    -1,
                    path,
                    -1,
                    Difference.DiffNodeStatus.Deleted);
            }

            static Difference BuildMovedDifference(string srcPath, string dstPath)
            {
                return new DiffMoved(
                    new RevisionInfo() { Type = EnumRevisionType.enTextFile },
                    -1,
                    srcPath,
                    -1,
                    dstPath,
                    -1);
            }

            static MountPointWithPath BuildRootMountPoint()
            {
                return new MountPointWithPath(
                    MountPointId.WORKSPACE_ROOT,
                    new RepositorySpec()
                    {
                        Name = "myrep",
                        Server = "myserver:8084"
                    },
                    "/myroot");
            }
        }

        static class FindClientDiffInfo
        {
            internal static ClientDiffInfo FromClientDiff(
                ClientDiff clientDiff,
                ChangeCategoryType type,
                UnityDiffTree diffTree)
            {
                foreach (ITreeViewNode node in diffTree.GetNodes())
                {
                    ClientDiffInfo result = FindClientDiffOfType(
                        clientDiff.Diff.Path,
                        type,
                        node);

                    if (result != null)
                        return result;
                }

                return null;
            }

            static ClientDiffInfo FindClientDiffOfType(
                string path,
                ChangeCategoryType type,
                ITreeViewNode node)
            {
                if (node is ClientDiffInfo)
                {
                    ClientDiffInfo clientDiffInfo = (ClientDiffInfo)node;
                    ChangeCategory category = (ChangeCategory)node.GetParent();

                    if (category.Type == type &&
                        clientDiffInfo.DiffWithMount.Difference.Path == path)
                    {
                        return (ClientDiffInfo)node;
                    }
                }

                for (int i = 0; i < node.GetChildrenCount(); i++)
                {
                    ClientDiffInfo result = FindClientDiffOfType(path, type, node.GetChild(i));
                    if (result != null)
                        return result;
                }

                return null;
            }
        }
    }
}
