using NUnit.Framework;

using Codice.Client.BaseCommands.Merge;
using Codice.Client.Commands;
using Codice.Client.Commands.Mount;
using Codice.CM.Common;
using Codice.CM.Common.Merge;
using PlasticGui.WorkspaceWindow.Merge;
using Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer;

namespace Unity.PlasticSCM.Tests.Editor.Views.IncomingChanges.Developer
{
    [TestFixture]
    class IsResolvedTests
    {
        [Test]
        public void IsDirectoryConflictResolved()
        {
            MergeChangeInfo dirConflict = BuilDirectoryConflict(true, 55);

            Assert.IsTrue(
                IsSolved.Conflict(dirConflict, null, null),
                "Conflict should be resolved");
        }

        [Test]
        public void IsDirectoryConflictNotResolved()
        {
            MergeChangeInfo dirConflict = BuilDirectoryConflict(false, 55);

            Assert.IsFalse(
                IsSolved.Conflict(dirConflict, null, null),
                "Conflict shouldn't be resolved");
        }


        [Test]
        public void IsFileConflictResolved()
        {
            long itemId = 55;
            MountPointId mountPointId = MountPointId.WORKSPACE_ROOT;

            MergeChangeInfo fileConflict = BuildFileConflict(mountPointId, itemId);

            MergeSolvedFileConflicts solvedFileConflicts = new MergeSolvedFileConflicts();
            solvedFileConflicts.AddResolveFile(mountPointId, itemId, "foo.c");

            Assert.IsTrue(
                IsSolved.Conflict(fileConflict, null, solvedFileConflicts),
                "Conflict should be resolved");
        }

        [Test]
        public void IsFileConflictNotResolved()
        {
            long itemId = 55;
            MountPointId mountPointId = MountPointId.WORKSPACE_ROOT;

            MergeChangeInfo fileConflict = BuildFileConflict(mountPointId, itemId);

            MergeSolvedFileConflicts solvedFileConflicts = new MergeSolvedFileConflicts();

            Assert.IsFalse(
                IsSolved.Conflict(fileConflict, null, solvedFileConflicts),
                "Conflict shouldn't be resolved");
        }

        [Test]
        public void IsDirectoryConflictWithMetaResolved()
        {
            MergeChangeInfo dirConflict = BuilDirectoryConflict(true, 55);
            MergeChangeInfo metaDirConflict = BuilDirectoryConflict(true, 55);

            Assert.IsTrue(
                IsSolved.Conflict(dirConflict, metaDirConflict, null),
                "Conflict should be resolved");
        }

        [Test]
        public void IsDirectoryConflictWithMetaNotResolved()
        {
            MergeChangeInfo dirConflict = BuilDirectoryConflict(true, 55);
            MergeChangeInfo metaDirConflict = BuilDirectoryConflict(false, 66);

            Assert.IsFalse(
                IsSolved.Conflict(dirConflict, metaDirConflict, null),
                "Conflict shouldn't be resolved");
        }

        [Test]
        public void IsFileConflictWithMetaResolved()
        {
            long itemId = 55;
            long metaItemId = 66;

            MountPointId mountPointId = MountPointId.WORKSPACE_ROOT;

            MergeChangeInfo fileConflict = BuildFileConflict(mountPointId, itemId);
            MergeChangeInfo metaFileConflict = BuildFileConflict(mountPointId, metaItemId);

            MergeSolvedFileConflicts solvedFileConflicts = new MergeSolvedFileConflicts();
            solvedFileConflicts.AddResolveFile(mountPointId, itemId, "foo.c");
            solvedFileConflicts.AddResolveFile(mountPointId, metaItemId, "foo.c.meta");

            Assert.IsTrue(
                IsSolved.Conflict(fileConflict, metaFileConflict, solvedFileConflicts),
                "Conflict should be resolved");
        }

        [Test]
        public void IsFileDirectoryConflictWithMetaNotResolved()
        {
            long itemId = 55;
            long metaItemId = 66;

            MountPointId mountPointId = MountPointId.WORKSPACE_ROOT;

            MergeChangeInfo fileConflict = BuildFileConflict(mountPointId, itemId);
            MergeChangeInfo metaFileConflict = BuildFileConflict(mountPointId, metaItemId);

            MergeSolvedFileConflicts solvedFileConflicts = new MergeSolvedFileConflicts();
            solvedFileConflicts.AddResolveFile(mountPointId, itemId, "foo.c");

            Assert.IsFalse(
                IsSolved.Conflict(fileConflict, metaFileConflict, solvedFileConflicts),
                "Conflict shouldn't be resolved");
        }

        MergeChangeInfo BuilDirectoryConflict(bool isResolved, long itemId)
        {
            DiffChanged src = new DiffChanged(
                CreateFileRevision(itemId), -1, "foo.c", -1,
                Difference.DiffNodeStatus.Added);

            DiffChanged dst = new DiffChanged(
                CreateFileRevision(itemId), -1, "foo.c", -1,
                Difference.DiffNodeStatus.Added);

            DirectoryConflict dirConflict = new EvilTwinConflict(src, dst);
            dirConflict.SetIsResolved(isResolved);

            MergeChangeInfo result = new MergeChangeInfo(
                new MountPointWithPath(
                    MountPointId.WORKSPACE_ROOT,
                    new RepositorySpec(),
                    "/"),
                dirConflict,
                null,
                null,
                MergeChangesCategory.Type.DirectoryConflicts,
                true);

            return result;
        }

        MergeChangeInfo BuildFileConflict(MountPointId mountPointId, long itemId)
        {
            DiffChanged src = new DiffChanged(
                CreateFileRevision(itemId), -1, "foo.c", -1,
                Difference.DiffNodeStatus.Changed);

            DiffChanged dst = new DiffChanged(
                CreateFileRevision(itemId), -1, "foo.c", -1,
                Difference.DiffNodeStatus.Changed);

            return new MergeChangeInfo(
                new MountPointWithPath(
                    mountPointId,
                    new RepositorySpec(),
                    "/"),
                new FileConflict(src, dst),
                MergeChangesCategory.Type.FileConflicts,
                true);
        }

        static RevisionInfo CreateFileRevision(long itemId)
        {
            RevisionInfo result = new RevisionInfo();
            result.Type = EnumRevisionType.enTextFile;
            result.Size = 128;
            result.ItemId = itemId;
            return result;
        }
    }
}
