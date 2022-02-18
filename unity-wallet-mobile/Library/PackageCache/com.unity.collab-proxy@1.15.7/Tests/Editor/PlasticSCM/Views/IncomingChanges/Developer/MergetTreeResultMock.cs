using System;

using Codice.Client.Commands.Merge;
using Codice.CM.Common;
using Codice.CM.Common.Merge;
using Codice.CM.Common.Tree;

namespace Unity.PlasticSCM.Tests.Editor.Views.IncomingChanges.Developer
{
    internal static class MergeTreeResultMock
    {
        internal static MergeTreeResult Build0()
        {
            MergeTreeResult mergeTreeResult = new MergeTreeResult();

            mergeTreeResult.AddsToApply.Add(CreateDirectoryAddedDifference());
            mergeTreeResult.AddsToApply.Add(CreateFileAddedDifference(1024));
            mergeTreeResult.AddsToApply.Add(CreateFileAddedDifference(512));

            mergeTreeResult.MovesToApply.Add(CreateMovedDifference());
            mergeTreeResult.MovesToApply.Add(CreateMovedDifference());

            mergeTreeResult.DeletesToApply.Add(CreateDeletedDifference());

            mergeTreeResult.FilesModifiedOnSource.Add(CreateModifiedDifference(312));
            mergeTreeResult.FilesModifiedOnSource.Add(CreateModifiedDifference(10));

            return mergeTreeResult;
        }

        internal static MergeTreeResult Build1()
        {
            MergeTreeResult mergeTreeResult = new MergeTreeResult();
            MergeTreeResult firstLevelXlinkMergeTreeResult = new MergeTreeResult();
            MergeTreeResult secondLevelXlinkMergeTreeResult = new MergeTreeResult();

            ClientWritableXlinkConflict firstLevelXlinkConflict = CreateWritableXlinkConflict();
            firstLevelXlinkConflict.XlinkMergeResult = new MergeResult();
            firstLevelXlinkConflict.XlinkMergeResult.ResultConflicts = firstLevelXlinkMergeTreeResult;

            ClientWritableXlinkConflict secondLevelXlinkConflict = CreateWritableXlinkConflict();
            secondLevelXlinkConflict.XlinkMergeResult = new MergeResult();
            secondLevelXlinkConflict.XlinkMergeResult.ResultConflicts = secondLevelXlinkMergeTreeResult;

            secondLevelXlinkMergeTreeResult.AddsToApply.Add(CreateFileAddedDifference(10));
            secondLevelXlinkMergeTreeResult.FilesModifiedOnSource.Add(CreateModifiedDifference(20));

            firstLevelXlinkMergeTreeResult.WritableXlinkConflicts.Add(secondLevelXlinkConflict);
            firstLevelXlinkMergeTreeResult.AddsToApply.Add(CreateFileAddedDifference(512));
            firstLevelXlinkMergeTreeResult.DeletesToApply.Add(CreateDeletedDifference());

            mergeTreeResult.WritableXlinkConflicts.Add(firstLevelXlinkConflict);
            mergeTreeResult.AddsToApply.Add(CreateFileAddedDifference(1024));
            return mergeTreeResult;
        }

        internal static MergeTreeResult Build2()
        {
            MergeTreeResult mergeTreeResult = new MergeTreeResult();

            mergeTreeResult.FileConflicts.Add(CreateFileConflict(312));
            mergeTreeResult.FileConflicts.Add(CreateFileConflict(318));
            mergeTreeResult.FileConflicts.Add(CreateFileConflict(398));

            mergeTreeResult.AddsToApply.Add(CreateDirectoryAddedDifference());
            mergeTreeResult.AddsToApply.Add(CreateFileAddedDifference(1024));
            mergeTreeResult.AddsToApply.Add(CreateFileAddedDifference(512));

            mergeTreeResult.MovesToApply.Add(CreateMovedDifference());
            mergeTreeResult.MovesToApply.Add(CreateMovedDifference());

            mergeTreeResult.DeletesToApply.Add(CreateDeletedDifference());

            mergeTreeResult.FilesModifiedOnSource.Add(CreateModifiedDifference(312));
            mergeTreeResult.FilesModifiedOnSource.Add(CreateModifiedDifference(10));

            return mergeTreeResult;
        }

        static Difference CreateDirectoryAddedDifference()
        {
            return new DiffChanged(
                CreateDirectoryRevision(), -1, CreateRandomPath(), -1,
                Difference.DiffNodeStatus.Added);
        }

        static Difference CreateFileAddedDifference(long size)
        {
            return new DiffChanged(
                CreateFileRevision(size), -1, CreateRandomPath(), -1,
                Difference.DiffNodeStatus.Added);
        }

        static Difference CreateMovedDifference()
        {
            return new DiffMoved(
                new RevisionInfo(), -1, CreateRandomPath(), -1,
                CreateRandomPath(), -1);
        }

        static Difference CreateDeletedDifference()
        {
            return new DiffChanged(
                new RevisionInfo() { Type = EnumRevisionType.enTextFile }, -1, CreateRandomPath(), -1,
                Difference.DiffNodeStatus.Deleted);
        }

        static FileConflict CreateModifiedDifference(long size)
        {
            DiffChanged src = new DiffChanged(
                CreateFileRevision(size), -1, CreateRandomPath(), -1,
                Difference.DiffNodeStatus.Changed);

            return new FileConflict(src, null);
        }

        static FileConflict CreateFileConflict(long size)
        {
            DiffChanged src = new DiffChanged(
                CreateFileRevision(size), -1, CreateRandomPath(), -1,
                Difference.DiffNodeStatus.Changed);

            DiffChanged dst = new DiffChanged(
                CreateFileRevision(size), -1, CreateRandomPath(), -1,
                Difference.DiffNodeStatus.Changed);

            return new FileConflict(src, dst);
        }

        static DiffXlinkChanged CreateAddedDiffXlinkChanged()
        {
            return new DiffXlinkChanged(
                new Xlink(), -1, "/", -1, Difference.DiffNodeStatus.Added);
        }

        static FileConflict CreateModifiedDiffXlinkChanged()
        {
            DiffXlinkChanged src = new DiffXlinkChanged(
                new Xlink(), -1, "/", -1, Difference.DiffNodeStatus.Changed);

            return new FileConflict(src, null);
        }

        static ChangeDeleteConflict CreateChangeDeleteConflict(
            long changeSize, long deleteSize, bool isChangeDelete)
        {
            DiffChanged src = new DiffChanged(
                CreateFileRevision(changeSize), -1, CreateRandomPath(), -1,
                Difference.DiffNodeStatus.Changed);

            DiffChanged dst = new DiffChanged(
                CreateFileRevision(deleteSize), -1, CreateRandomPath(), -1,
                Difference.DiffNodeStatus.Deleted);

            if (isChangeDelete)
                return new ChangeDeleteConflict(src, dst);

            return new ChangeDeleteConflict(dst, src);
        }

        static EvilTwinConflict CreateEvilTwinConflict(int addedSize)
        {
            DiffChanged src = new DiffChanged(
                CreateFileRevision(addedSize), -1, CreateRandomPath(), -1,
                Difference.DiffNodeStatus.Added);

            DiffChanged dst = new DiffChanged(
                CreateFileRevision(addedSize), -1, CreateRandomPath(), -1,
                Difference.DiffNodeStatus.Added);

            return new EvilTwinConflict(src, dst);
        }

        static ClientWritableXlinkConflict CreateWritableXlinkConflict()
        {
            Xlink baseXlink = new Xlink();
            Xlink srcXlink = new Xlink();
            Xlink dstXlink = new Xlink();

            DiffXlinkChanged src = new DiffXlinkChanged(
                srcXlink, -1, "/", -1, Difference.DiffNodeStatus.Changed);
            src.BaseXlink = baseXlink;
            DiffXlinkChanged dst = new DiffXlinkChanged(
                dstXlink, -1, "/", -1, Difference.DiffNodeStatus.Changed);

            return new ClientWritableXlinkConflict(src, dst);
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

        static string CreateRandomPath()
        {
            return "/" + Guid.NewGuid().ToString().Substring(0, 4);
        }
    }
}
