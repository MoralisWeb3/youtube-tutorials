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
    class IsCurrentTests
    {
        [Test]
        public void NullValuesTest()
        {
            Assert.IsFalse(
                IsCurrent.Conflict(null, null, null),
                "Null values should return false");
        }

        [Test]
        public void NotCurrentConflictTest()
        {
            long itemId = 55;
            MountPointId mountPointId = MountPointId.WORKSPACE_ROOT;

            MergeChangeInfo changeInfo = BuildChangeInfo(mountPointId, itemId);
            MergeSolvedFileConflicts solvedFileConflicts = new MergeSolvedFileConflicts();

            Assert.IsFalse(
                IsCurrent.Conflict(changeInfo, null, solvedFileConflicts),
                "Should return false");
        }

        [Test]
        public void CurrentConflictTest()
        {
            long itemId = 55;
            MountPointId mountPointId = MountPointId.WORKSPACE_ROOT;

            MergeChangeInfo changeInfo = BuildChangeInfo(mountPointId, itemId);
            MergeSolvedFileConflicts solvedFileConflicts = new MergeSolvedFileConflicts();

            MergeSolvedFileConflicts.CurrentConflict currentConflict = new MergeSolvedFileConflicts.CurrentConflict(
                mountPointId, itemId, 0);

            solvedFileConflicts.SetCurrentConflict(currentConflict);

            Assert.IsTrue(
                IsCurrent.Conflict(changeInfo, null, solvedFileConflicts),
                "Should return true");
        }

        [Test]
        public void NotCurrentConflictWithMeta()
        {
            long itemId = 55;
            long metaItemId = 66;

            MountPointId mountPointId = MountPointId.WORKSPACE_ROOT;

            MergeChangeInfo changeInfo = BuildChangeInfo(mountPointId, itemId);
            MergeChangeInfo metaChangeInfo = BuildChangeInfo(mountPointId, metaItemId);

            MergeSolvedFileConflicts solvedFileConflicts = new MergeSolvedFileConflicts();

            Assert.IsFalse(
                IsCurrent.Conflict(changeInfo, metaChangeInfo, solvedFileConflicts),
                "Should return false");
        }

        [Test]
        public void CurrentConflictWithMetaCurrentIsNotTheMeta()
        {
            long itemId = 55;
            long metaItemId = 66;

            MountPointId mountPointId = MountPointId.WORKSPACE_ROOT;

            MergeChangeInfo changeInfo = BuildChangeInfo(mountPointId, itemId);
            MergeChangeInfo metaChangeInfo = BuildChangeInfo(mountPointId, metaItemId);

            MergeSolvedFileConflicts solvedFileConflicts = new MergeSolvedFileConflicts();

            MergeSolvedFileConflicts.CurrentConflict currentConflict = new MergeSolvedFileConflicts.CurrentConflict(
                mountPointId, itemId, 0);

            solvedFileConflicts.SetCurrentConflict(currentConflict);

            Assert.IsTrue(
                IsCurrent.Conflict(changeInfo, metaChangeInfo, solvedFileConflicts),
                "Should return true");
        }

        [Test]
        public void CurrentConflictWithMetaCurrentIsTheMeta()
        {
            long itemId = 55;
            long metaItemId = 66;

            MountPointId mountPointId = MountPointId.WORKSPACE_ROOT;

            MergeChangeInfo changeInfo = BuildChangeInfo(mountPointId, itemId);
            MergeChangeInfo metaChangeInfo = BuildChangeInfo(mountPointId, metaItemId);

            MergeSolvedFileConflicts solvedFileConflicts = new MergeSolvedFileConflicts();

            MergeSolvedFileConflicts.CurrentConflict currentConflict = new MergeSolvedFileConflicts.CurrentConflict(
                mountPointId, metaItemId, 0);

            solvedFileConflicts.SetCurrentConflict(currentConflict);

            Assert.IsTrue(
                IsCurrent.Conflict(changeInfo, metaChangeInfo, solvedFileConflicts),
                "Should return true");
        }

        MergeChangeInfo BuildChangeInfo(MountPointId mountId, long itemId)
        {
            return new MergeChangeInfo(
                new MountPointWithPath(
                    mountId,
                    new RepositorySpec(),
                    "/"),
                BuildFileConflict(itemId),
                MergeChangesCategory.Type.FileConflicts,
                true);
        }

        FileConflict BuildFileConflict(long itemId)
        {
            DiffChanged src = new DiffChanged(
                CreateFileRevision(itemId), -1, "foo.c", -1,
                Difference.DiffNodeStatus.Changed);

            DiffChanged dst = new DiffChanged(
                CreateFileRevision(itemId), -1, "foo.c", -1,
                Difference.DiffNodeStatus.Changed);

            return new FileConflict(src, dst);
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
