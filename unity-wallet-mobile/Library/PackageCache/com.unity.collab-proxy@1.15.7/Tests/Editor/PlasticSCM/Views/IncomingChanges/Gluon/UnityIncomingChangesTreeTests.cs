using System.Collections.Generic;

using NUnit.Framework;

using Codice.Client.Commands;
using Codice.Client.Commands.Mount;
using Codice.Client.GameUI;
using Codice.Client.GameUI.Update;
using Codice.CM.Common;
using Codice.CM.Common.Merge;
using PlasticGui.Gluon.WorkspaceWindow.Views.IncomingChanges;
using Unity.PlasticSCM.Editor.Views.IncomingChanges.Gluon;

namespace Unity.PlasticSCM.Tests.Editor.Views.IncomingChanges.Gluon
{
    [TestFixture]
    class UnityIncomingChangesTreeTests
    {
        [Test]
        public void TestConflictNoMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            GluonFileConflict fileConflict = Build.FileConflict(
                rootMountPointWithPath,
                "/foo/bar.c");

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            fileConflicts.Add(fileConflict);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo conflictChangeInfo = FindChangeInfo.FromFileConflict(
                fileConflict, tree);

            Assert.IsNotNull(
                conflictChangeInfo,
                "ChangeInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(conflictChangeInfo),
                "Meta change should be null");
        }

        [Test]
        public void TestConflictWithMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            GluonFileConflict fileConflict = Build.FileConflict(
                rootMountPointWithPath,
                "/foo/bar.c");

            GluonFileConflict fileConflictMeta = Build.FileConflict(
                rootMountPointWithPath,
                "/foo/bar.c.meta");

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            fileConflicts.Add(fileConflict);
            fileConflicts.Add(fileConflictMeta);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo conflictChangeInfo = FindChangeInfo.FromFileConflict(
                fileConflict, tree);

            Assert.IsNotNull(
                conflictChangeInfo,
                "ChangeInfo not found");

            Assert.IsNotNull(
                tree.GetMetaChange(conflictChangeInfo),
                "Meta change should not be null");
        }

        [Test]
        public void TestChangedNoMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference changed = Build.ChangedDifference(
                "/foo/bar.c");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Changed.Add(changed);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfo = FindChangeInfo.FromDifference(
                changed, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangeInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(changeInfo),
                "Meta change should be null");
        }

        [Test]
        public void TestChangedWithMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference changed = Build.ChangedDifference(
                "/foo/bar.c");

            Difference changedMeta = Build.ChangedDifference(
                "/foo/bar.c.meta");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Changed.Add(changed);
            outOfDateItems.Changed.Add(changedMeta);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfo = FindChangeInfo.FromDifference(
                changed, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangeInfo not found");

            Assert.IsNotNull(
                tree.GetMetaChange(changeInfo),
                "Meta change should not be null");
        }

        [Test]
        public void TestMovedNoMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference moved = Build.MovedDifference(
                "/foo/bar.c",
                "/foo/var.c");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Changed.Add(moved);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfo = FindChangeInfo.FromDifference(
                moved, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangeInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(changeInfo),
                "Meta change should be null");
        }

        [Test]
        public void TestMovedWithMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference moved = Build.MovedDifference(
                "/foo/bar.c",
                "/foo/var.c");

            Difference movedMeta = Build.MovedDifference(
                "/foo/bar.c.meta",
                "/foo/var.c.meta");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Moved.Add(moved);
            outOfDateItems.Moved.Add(movedMeta);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfo = FindChangeInfo.FromDifference(
                moved, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangeInfo not found");

            Assert.IsNotNull(
                tree.GetMetaChange(changeInfo),
                "Meta change should not be null");
        }

        [Test]
        public void TestChangedMovedWithMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference changed =
                Build.ChangedDifference("/foo/var.c");

            Difference changedMeta =
                Build.ChangedDifference("/foo/var.c.meta");

            Difference moved =
                Build.MovedDifference(
                    "/foo/bar.c", "/foo/var.c");
            Difference movedMeta =
                Build.MovedDifference(
                    "/foo/bar.c.meta", "/foo/var.c.meta");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Changed.Add(changed);
            outOfDateItems.Changed.Add(changedMeta);
            outOfDateItems.Moved.Add(moved);
            outOfDateItems.Moved.Add(movedMeta);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfo = FindChangeInfo.FromDifference(
                changed, tree);
            IncomingChangeInfo movedChangeInfo = FindChangeInfo.FromDifference(
                moved, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangedInfo not found");

            Assert.IsNotNull(
                movedChangeInfo,
                "MovedChangedInfo not found");

            Assert.IsNotNull(
                tree.GetMetaChange(changeInfo),
                "Meta change for changed not should be null");

            Assert.IsNotNull(
                tree.GetMetaChange(movedChangeInfo),
                "Meta change for moved should not be null");
        }

        [Test]
        public void TestAddedNoMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference added = Build.AddedDifference(
                "/foo/bar.c");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Added.Add(added);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfo = FindChangeInfo.FromDifference(
                added, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangeInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(changeInfo),
                "Meta change should be null");
        }

        [Test]
        public void TestAddedWithMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference added = Build.AddedDifference(
                "/foo/bar.c");

            Difference addedMeta = Build.AddedDifference(
                "/foo/bar.c.meta");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Added.Add(added);
            outOfDateItems.Added.Add(addedMeta);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfo = FindChangeInfo.FromDifference(
                added, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangeInfo not found");

            Assert.IsNotNull(
                tree.GetMetaChange(changeInfo),
                "Meta change should not be null");
        }

        [Test]
        public void TestDeletedNoMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference deleted = Build.DeletedDifference(
                "/foo/bar.c");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Deleted.Add(deleted);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfo = FindChangeInfo.FromDifference(
                deleted, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangeInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(changeInfo),
                "Meta change should be null");
        }

        [Test]
        public void TestDeletedWithMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference deleted = Build.DeletedDifference(
                "/foo/bar.c");

            Difference deletedMeta = Build.DeletedDifference(
                "/foo/bar.c.meta");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Deleted.Add(deleted);
            outOfDateItems.Deleted.Add(deletedMeta);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfo = FindChangeInfo.FromDifference(
                deleted, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangeInfo not found");

            Assert.IsNotNull(
                tree.GetMetaChange(changeInfo),
                "Meta change should not be null");
        }

        [Test]
        public void TestChangedWithDeletedMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference changed = Build.ChangedDifference(
                "/foo/bar.c");

            Difference deletedMeta = Build.DeletedDifference(
                "/foo/bar.c.meta");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Changed.Add(changed);
            outOfDateItems.Deleted.Add(deletedMeta);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfo = FindChangeInfo.FromDifference(
                changed, tree);

            IncomingChangeInfo deleteInfo = FindChangeInfo.FromDifference(
                deletedMeta, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangeInfo not found");

            Assert.IsNotNull(
                deleteInfo,
                "DeleteInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(changeInfo),
                "Meta change should be null");
        }

        [Test]
        public void TestOnlyMeta()
        {
            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference changedMeta = Build.ChangedDifference(
                "/foo/bar.c.meta");

            OutOfDateItemsByMount outOfDateItems = new OutOfDateItemsByMount()
            {
                Mount = rootMountPointWithPath
            };

            outOfDateItems.Changed.Add(changedMeta);

            CheckedStateManager checkedStateManager = new CheckedStateManager();
            List<OutOfDateItemsByMount> outOfDateItemsByMount =
                new List<OutOfDateItemsByMount>();
            List<GluonFileConflict> fileConflicts = new List<GluonFileConflict>();

            outOfDateItemsByMount.Add(outOfDateItems);

            IncomingChangesTree innerTree = IncomingChangesTree.BuildIncomingChangeCategories(
                checkedStateManager,
                outOfDateItemsByMount,
                fileConflicts);

            UnityIncomingChangesTree tree =
                UnityIncomingChangesTree.BuildIncomingChangeCategories(innerTree);

            IncomingChangeInfo changeInfoMeta = FindChangeInfo.FromDifference(
                changedMeta, tree);

            Assert.IsNotNull(
                changeInfoMeta,
                "ChangeInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(changeInfoMeta),
                "Meta change should be null");
        }

        static MountPointWithPath BuildRootMountPointWithPath()
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

        class Build
        {
            internal static GluonFileConflict FileConflict(
                MountPointWithPath mount,
                string path)
            {
                return new GluonFileConflict(
                    mount,
                    BuildFileRevision(),
                    BuildFileRevision(),
                    path);
            }

            internal static Difference AddedDifference(
                string path)
            {
                return new DiffChanged(
                    BuildFileRevision(), -1, path, -1,
                    Difference.DiffNodeStatus.Added);
            }

            internal static Difference MovedDifference(
                string srcPath,
                string dstPath)
            {
                return new DiffMoved(
                    new RevisionInfo(),
                    -1,
                    srcPath,
                    -1,
                    dstPath, -1);
            }

            internal static Difference DeletedDifference(
                string path)
            {
                return new DiffChanged(
                    new RevisionInfo() { Type = EnumRevisionType.enTextFile },
                    -1,
                    path,
                    -1,
                    Difference.DiffNodeStatus.Deleted);
            }

            internal static Difference ChangedDifference(
                string path)
            {
                return new DiffChanged(
                    BuildFileRevision(),
                    -1,
                    path,
                    -1,
                    Difference.DiffNodeStatus.Changed);
            }

            static RevisionInfo BuildFileRevision()
            {
                RevisionInfo result = new RevisionInfo();
                result.Type = EnumRevisionType.enTextFile;
                result.Size = 128;
                return result;
            }
        }

        class FindChangeInfo
        {
            internal static IncomingChangeInfo FromFileConflict(
                GluonFileConflict fileConflict,
                UnityIncomingChangesTree tree)
            {
                foreach (IncomingChangeCategory category in tree.GetNodes())
                {
                    foreach (IncomingChangeInfo changeInfo in category.GetChanges())
                    {
                        if (changeInfo.CategoryType != IncomingChangeCategory.Type.Conflicted)
                            continue;

                        if (changeInfo.GetPath() != fileConflict.CmPath)
                            continue;

                        return changeInfo;
                    }
                }

                return null;
            }

            internal static IncomingChangeInfo FromDifference(
                Difference difference,
                UnityIncomingChangesTree tree)
            {
                foreach (IncomingChangeCategory category in tree.GetNodes())
                {
                    foreach (IncomingChangeInfo changeInfo in category.GetChanges())
                    {
                        if (changeInfo.CategoryType == IncomingChangeCategory.Type.Conflicted)
                            continue;

                        if (changeInfo.GetPath() != difference.Path)
                            continue;

                        return changeInfo;
                    }
                }

                return null;
            }
        }
    }
}
