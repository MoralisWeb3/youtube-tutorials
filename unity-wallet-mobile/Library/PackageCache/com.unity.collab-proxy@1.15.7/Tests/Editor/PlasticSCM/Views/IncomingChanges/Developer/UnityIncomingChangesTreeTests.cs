using NUnit.Framework;

using Codice.Client.Commands;
using Codice.Client.Commands.Mount;
using Codice.CM.Common;
using Codice.CM.Common.Merge;
using PlasticGui.WorkspaceWindow.Merge;
using Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer;

namespace Unity.PlasticSCM.Tests.Editor.Views.IncomingChanges.Developer
{
    [TestFixture]
    class UnityIncomingChangesTreeTests
    {
        [Test]
        public void TestConflictNoMeta()
        {
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            FileConflict fileConflict =
                BuildMergeTreeResult.CreateFileConflict("/foo/bar.c");

            conflicts.FileConflicts.Add(fileConflict);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo conflictChangeInfo = FindChangeInfo.FromFileConflict(
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
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            FileConflict fileConflict =
                BuildMergeTreeResult.CreateFileConflict("/foo/bar.c");

            FileConflict fileConflictMeta =
                BuildMergeTreeResult.CreateFileConflict("/foo/bar.c.meta");

            conflicts.FileConflicts.Add(fileConflict);
            conflicts.FileConflicts.Add(fileConflictMeta);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo conflictChangeInfo = FindChangeInfo.FromFileConflict(
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
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            FileConflict fileConflict =
                BuildMergeTreeResult.CreateModifiedDifference("/foo/bar.c");

            conflicts.FilesModifiedOnSource.Add(fileConflict);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo changeInfo = FindChangeInfo.FromFileConflict(
                fileConflict, tree);

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
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            FileConflict fileConflict =
                BuildMergeTreeResult.CreateFileConflict("/foo/bar.c");

            FileConflict fileConflictMeta =
                BuildMergeTreeResult.CreateFileConflict("/foo/bar.c.meta");

            conflicts.FilesModifiedOnSource.Add(fileConflict);
            conflicts.FilesModifiedOnSource.Add(fileConflictMeta);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo changeInfo = FindChangeInfo.FromFileConflict(
                fileConflict, tree);

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
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference difference =
                BuildMergeTreeResult.CreateMovedDifference("/foo/bar.c", "/foo/var.c");

            conflicts.MovesToApply.Add(difference);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo changeInfo = FindChangeInfo.FromDifference(
                difference, tree);

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
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference difference =
                BuildMergeTreeResult.CreateMovedDifference("/foo/bar.c", "/foo/var.c");
            Difference differenceMeta =
                BuildMergeTreeResult.CreateMovedDifference("/foo/bar.c.meta", "/foo/var.c.meta");

            conflicts.MovesToApply.Add(difference);
            conflicts.MovesToApply.Add(differenceMeta);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo changeInfo = FindChangeInfo.FromDifference(
                difference, tree);

            Assert.IsNotNull(
                changeInfo,
                "ChangeInfo not found");

            Assert.IsNotNull(
                tree.GetMetaChange(changeInfo),
                "Meta change not should be null");
        }

        [Test]
        public void TestChangedMovedWithMeta()
        {
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            FileConflict fileConflict =
                BuildMergeTreeResult.CreateFileConflict("/foo/var.c");

            FileConflict fileConflictMeta =
                BuildMergeTreeResult.CreateFileConflict("/foo/var.c.meta");

            Difference difference =
                BuildMergeTreeResult.CreateMovedDifference("/foo/bar.c", "/foo/var.c");
            Difference differenceMeta =
                BuildMergeTreeResult.CreateMovedDifference("/foo/bar.c.meta", "/foo/var.c.meta");

            conflicts.FilesModifiedOnSource.Add(fileConflict);
            conflicts.FilesModifiedOnSource.Add(fileConflictMeta);
            conflicts.MovesToApply.Add(difference);
            conflicts.MovesToApply.Add(differenceMeta);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo fileChangeInfo = FindChangeInfo.FromFileConflict(
                fileConflict, tree);
            MergeChangeInfo differenceChangeInfo = FindChangeInfo.FromDifference(
                difference, tree);

            Assert.IsNotNull(
                fileChangeInfo,
                "FileConflict not found");

            Assert.IsNotNull(
                differenceChangeInfo,
                "Difference not found");

            Assert.IsNotNull(
                tree.GetMetaChange(fileChangeInfo),
                "Meta change for file conflict not should be null");

            Assert.IsNotNull(
                tree.GetMetaChange(differenceChangeInfo),
                "Meta change for difference not should be null");
        }

        [Test]
        public void TestAddedNoMeta()
        {
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference difference =
                BuildMergeTreeResult.CreateFileAddedDifference("/foo/bar.c");

            conflicts.AddsToApply.Add(difference);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo addedChangeInfo = FindChangeInfo.FromDifference(
                difference, tree);

            Assert.IsNotNull(
                addedChangeInfo,
                "ChangeInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(addedChangeInfo),
                "Meta change should be null");
        }

        [Test]
        public void TestAddedWithMeta()
        {
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference difference =
                BuildMergeTreeResult.CreateFileAddedDifference("/foo/bar.c");
            Difference differenceMeta =
                BuildMergeTreeResult.CreateFileAddedDifference("/foo/bar.c.meta");

            conflicts.AddsToApply.Add(difference);
            conflicts.AddsToApply.Add(differenceMeta);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo addedChangeInfo = FindChangeInfo.FromDifference(
                difference, tree);

            Assert.IsNotNull(
                addedChangeInfo,
                "ChangeInfo not found");

            Assert.IsNotNull(
                tree.GetMetaChange(addedChangeInfo),
                "Meta change should not be null");
        }

        [Test]
        public void TestDeletedNoMeta()
        {
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference difference =
                BuildMergeTreeResult.CreateDeletedDifference("/foo/bar.c");

            conflicts.DeletesToApply.Add(difference);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo addedChangeInfo = FindChangeInfo.FromDifference(
                difference, tree);

            Assert.IsNotNull(
                addedChangeInfo,
                "ChangeInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(addedChangeInfo),
                "Meta change should be null");
        }

        [Test]
        public void TestDeletedWithMeta()
        {
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            Difference difference =
                BuildMergeTreeResult.CreateFileAddedDifference("/foo/bar.c");
            Difference differenceMeta =
                BuildMergeTreeResult.CreateFileAddedDifference("/foo/bar.c.meta");

            conflicts.DeletesToApply.Add(difference);
            conflicts.DeletesToApply.Add(differenceMeta);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo addedChangeInfo = FindChangeInfo.FromDifference(
                difference, tree);

            Assert.IsNotNull(
                addedChangeInfo,
                "ChangeInfo not found");

            Assert.IsNotNull(
                tree.GetMetaChange(addedChangeInfo),
                "Meta change should not be null");
        }

        [Test]
        public void TestOnlyMeta()
        {
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            FileConflict fileConflictMeta =
                BuildMergeTreeResult.CreateFileConflict("/foo/bar.c.meta");

            conflicts.FileConflicts.Add(fileConflictMeta);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo conflictMetaChangeInfo = FindChangeInfo.FromFileConflict(
                fileConflictMeta, tree);

            Assert.IsNotNull(
                conflictMetaChangeInfo,
                "ChangeInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(conflictMetaChangeInfo),
                "Meta change should be null");
        }

        [Test]
        public void TestDirectoryConflictNoMeta()
        {
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            DirectoryConflict directoryConflict =
                BuildMergeTreeResult.CreateEvilTwinConflict("/foo/bar.c");

            conflicts.EvilTwins.Add(directoryConflict);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo conflictChangeInfo = FindChangeInfo.FromDirectoryConflict(
                directoryConflict, tree);

            Assert.IsNotNull(
                conflictChangeInfo,
                "ChangeInfo not found");

            Assert.IsNull(
                tree.GetMetaChange(conflictChangeInfo),
                "Meta change should be null");
        }

        [Test]
        public void TestDirectoryConflictWithMeta()
        {
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            DirectoryConflict directoryConflict =
                BuildMergeTreeResult.CreateEvilTwinConflict("/foo/bar.c");
            DirectoryConflict directoryConflictMeta =
                BuildMergeTreeResult.CreateEvilTwinConflict("/foo/bar.c.meta");

            conflicts.EvilTwins.Add(directoryConflict);
            conflicts.EvilTwins.Add(directoryConflictMeta);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo conflictChangeInfo = FindChangeInfo.FromDirectoryConflict(
                directoryConflict, tree);

            Assert.IsNotNull(
                conflictChangeInfo,
                "ChangeInfo not found");

            Assert.IsNotNull(
                tree.GetMetaChange(conflictChangeInfo),
                "Meta change should not be null");
        }

        [Test]
        public void TestChangedWithDeletedMeta()
        {
            MergeTreeResult conflicts = new MergeTreeResult();

            MountPointWithPath rootMountPointWithPath = BuildRootMountPointWithPath();

            FileConflict modified =
                BuildMergeTreeResult.CreateModifiedDifference("/foo/bar.c");

            Difference deletedMeta =
                BuildMergeTreeResult.CreateDeletedDifference("/foo/bar.c.meta");

            conflicts.FilesModifiedOnSource.Add(modified);
            conflicts.DeletesToApply.Add(deletedMeta);

            UnityIncomingChangesTree tree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                MergeChangesTree.BuildForIncomingChangesView(
                    conflicts,
                    new GetConflictResolutionMock(),
                    rootMountPointWithPath));

            MergeChangeInfo changeInfo = FindChangeInfo.FromFileConflict(
                modified, tree);

            MergeChangeInfo deleteInfo = FindChangeInfo.FromDifference(
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

        class BuildMergeTreeResult
        {
            internal static Difference CreateFileAddedDifference(string path)
            {
                return new DiffChanged(
                    CreateFileRevision(128), -1, path, -1,
                    Difference.DiffNodeStatus.Added);
            }

            internal static Difference CreateMovedDifference(string srcPath, string dstPath)
            {
                return new DiffMoved(
                    new RevisionInfo(),
                    -1,
                    srcPath,
                    -1,
                    dstPath, -1);
            }

            internal static Difference CreateDeletedDifference(string path)
            {
                return new DiffChanged(
                    new RevisionInfo() { Type = EnumRevisionType.enTextFile },
                    -1,
                    path,
                    -1,
                    Difference.DiffNodeStatus.Deleted);
            }

            internal static FileConflict CreateModifiedDifference(string path)
            {
                DiffChanged src = new DiffChanged(
                    CreateFileRevision(128),
                    -1,
                    path,
                    -1,
                    Difference.DiffNodeStatus.Changed);

                return new FileConflict(src, null);
            }

            internal static FileConflict CreateFileConflict(string path)
            {
                DiffChanged src = new DiffChanged(
                    CreateFileRevision(128), -1, path, -1,
                    Difference.DiffNodeStatus.Changed);

                DiffChanged dst = new DiffChanged(
                    CreateFileRevision(128), -1, path, -1,
                    Difference.DiffNodeStatus.Changed);

                return new FileConflict(src, dst);
            }

            internal static EvilTwinConflict CreateEvilTwinConflict(string path)
            {
                DiffChanged src = new DiffChanged(
                    CreateFileRevision(128), -1, path, -1,
                    Difference.DiffNodeStatus.Added);

                DiffChanged dst = new DiffChanged(
                    CreateFileRevision(128), -1, path, -1,
                    Difference.DiffNodeStatus.Added);

                return new EvilTwinConflict(src, dst);
            }

            static RevisionInfo CreateDirectoryRevision()
            {
                RevisionInfo result = new RevisionInfo();
                result.Type = EnumRevisionType.enDirectory;
                return result;
            }

            static RevisionInfo CreateFileRevision(long size)
            {
                RevisionInfo result = new RevisionInfo();
                result.Type = EnumRevisionType.enTextFile;
                result.Size = size;
                return result;
            }
        }

        class GetConflictResolutionMock : MergeChangesTree.IGetConflictResolution
        {
            string MergeChangesTree.IGetConflictResolution.GetConflictResolution(
                DirectoryConflict conflict)
            {
                return string.Empty;
            }
        }

        class FindChangeInfo
        {
            internal static MergeChangeInfo FromFileConflict(
                FileConflict fileConflict,
                UnityIncomingChangesTree tree)
            {
                foreach (MergeChangesCategory category in tree.GetNodes())
                {
                    foreach (MergeChangeInfo changeInfo in category.GetChanges())
                    {
                        if (changeInfo.CategoryType != MergeChangesCategory.Type.FileConflicts &&
                            changeInfo.CategoryType != MergeChangesCategory.Type.Changed)
                            continue;

                        if (changeInfo.GetPath() != fileConflict.SrcDiff.Path)
                            continue;

                        return changeInfo;
                    }
                }

                return null;
            }

            internal static MergeChangeInfo FromDifference(
                Difference difference,
                UnityIncomingChangesTree tree)
            {
                foreach (MergeChangesCategory category in tree.GetNodes())
                {
                    foreach (MergeChangeInfo changeInfo in category.GetChanges())
                    {
                        if (changeInfo.CategoryType != MergeChangesCategory.Type.Added &&
                            changeInfo.CategoryType != MergeChangesCategory.Type.Moved &&
                            changeInfo.CategoryType != MergeChangesCategory.Type.Deleted)
                            continue;

                        if (changeInfo.GetPath() != difference.Path)
                            continue;

                        return changeInfo;
                    }
                }

                return null;
            }

            internal static MergeChangeInfo FromDirectoryConflict(
                DirectoryConflict directoryConflict,
                UnityIncomingChangesTree tree)
            {
                foreach (MergeChangesCategory category in tree.GetNodes())
                {
                    foreach (MergeChangeInfo changeInfo in category.GetChanges())
                    {
                        if (changeInfo.CategoryType != MergeChangesCategory.Type.DirectoryConflicts)
                            continue;

                        if (changeInfo.GetPath() != directoryConflict.SrcDiff.Path)
                            continue;

                        return changeInfo;
                    }
                }

                return null;
            }
        }
    }
}
