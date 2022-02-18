using System.Collections.Generic;

using NUnit.Framework;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using Codice.CM.Common;
using Unity.PlasticSCM.Editor.Views.PendingChanges;

namespace Unity.PlasticSCM.Tests.Editor.Views.PendingChanges
{
    [TestFixture]
    class UnityPendingChangesTreeTests
    {
        [Test]
        public void TestAddedNoMeta()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo added = new ChangeInfo()
            {
                Path = "/foo/foo.c"
            };

            pendingChanges.Added.Add(added);

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager());

            Assert.IsNull(
                tree.GetMetaChange(added),
                "Meta change should be null");
        }

        [Test]
        public void TestAddedWithMeta()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo added = new ChangeInfo()
            {
                Path = "/foo/foo.c"
            };

            ChangeInfo addedMeta = new ChangeInfo()
            {
                Path = "/foo/foo.c.meta"
            };

            pendingChanges.Added.Add(added);
            pendingChanges.Added.Add(addedMeta);

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager());

            Assert.IsTrue(
                pendingChanges.Added.Contains(added),
                "Pending changes should contain the change");

            Assert.IsFalse(
                pendingChanges.Added.Contains(addedMeta),
                "Pending changes should not contain the meta");

            Assert.AreEqual(addedMeta, tree.GetMetaChange(added));
        }

        [Test]
        public void TestFileAddedMetaPrivate()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo added = new ChangeInfo()
            {
                Path = "/foo/foo.c",
                ChangeTypes = ChangeTypes.Added,
            };

            ChangeInfo privateMeta = new ChangeInfo()
            {
                Path = "/foo/foo.c.meta",
                ChangeTypes = ChangeTypes.Private,
            };

            pendingChanges.Added.Add(added);
            pendingChanges.Added.Add(privateMeta);

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager());

            Assert.IsTrue(
                pendingChanges.Added.Contains(added),
                "Pending changes should contain the change");

            Assert.IsTrue(
                pendingChanges.Added.Contains(privateMeta),
                "Pending changes should contain the meta");
        }

        [Test]
        public void TestDeletedNoMeta()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo deleted = new ChangeInfo()
            {
                Path = "/foo/foo.c"
            };

            pendingChanges.Deleted.Add(deleted);

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager());

            Assert.IsNull(
                tree.GetMetaChange(deleted),
                "Meta change should be null");
        }

        [Test]
        public void TestDeletedWithMeta()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo deleted = new ChangeInfo()
            {
                Path = "/foo/foo.c"
            };

            ChangeInfo deletedMeta = new ChangeInfo()
            {
                Path = "/foo/foo.c.meta"
            };

            pendingChanges.Deleted.Add(deleted);
            pendingChanges.Deleted.Add(deletedMeta);

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager());

            Assert.IsTrue(
                pendingChanges.Deleted.Contains(deleted),
                "Pending changes should contain the change");

            Assert.IsFalse(
                pendingChanges.Deleted.Contains(deletedMeta),
                "Pending changes should not contain the meta");

            Assert.AreEqual(deletedMeta, tree.GetMetaChange(deleted));
        }

        [Test]
        public void TestChangedNoMeta()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo changed = new ChangeInfo()
            {
                Path = "/foo/foo.c"
            };

            pendingChanges.Changed.Add(changed);

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager());

            Assert.IsNull(
                tree.GetMetaChange(changed),
                "Meta change should be null");
        }

        [Test]
        public void TestChangedWithMeta()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo changed = new ChangeInfo()
            {
                Path = "/foo/foo.c"
            };

            ChangeInfo changedMeta = new ChangeInfo()
            {
                Path = "/foo/foo.c.meta"
            };

            pendingChanges.Changed.Add(changed);
            pendingChanges.Changed.Add(changedMeta);

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager());

            Assert.IsTrue(
                pendingChanges.Changed.Contains(changed),
                "Pending changes should contain the change");

            Assert.IsFalse(
                pendingChanges.Changed.Contains(changedMeta),
                "Pending changes should not contain the meta");

            Assert.AreEqual(changedMeta, tree.GetMetaChange(changed));
        }

        [Test]
        public void TestMovedNoMeta()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo moved = new ChangeInfo()
            {
                OldPath = "/foo/foo.c",
                Path = "/foo/bar/newfoo.c",
            };

            pendingChanges.Moved.Add(moved);

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager());

            Assert.IsNull(
                tree.GetMetaChange(moved),
                "Meta change should be null");
        }

        [Test]
        public void TestMovedWithMeta()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo moved = new ChangeInfo()
            {
                OldPath = "/foo/foo.c",
                Path = "/foo/bar/newfoo.c",
            };

            ChangeInfo movedMeta = new ChangeInfo()
            {
                OldPath = "/foo/foo.c.meta",
                Path = "/foo/bar/newfoo.c.meta",
            };

            pendingChanges.Moved.Add(moved);
            pendingChanges.Moved.Add(movedMeta);

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager());

            Assert.IsTrue(
                pendingChanges.Moved.Contains(moved),
                "Pending changes should contain the change");

            Assert.IsFalse(
                pendingChanges.Moved.Contains(movedMeta),
                "Pending changes should not contain the meta");

            Assert.AreEqual(movedMeta, tree.GetMetaChange(moved));
        }

        [Test]
        public void TestOnlyMeta()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo addedMeta = new ChangeInfo()
            {
                Path = "/foo/foo.c.meta",
            };

            pendingChanges.Added.Add(addedMeta);

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager());

            Assert.IsNull(
                tree.GetMetaChange(addedMeta),
                "Meta change should be null");
        }

        [Test]
        public void TestCheckedChangesWithMeta()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo added = new ChangeInfo()
            {
                Path = "/foo/foo.c",
            };

            ChangeInfo addedMeta = new ChangeInfo()
            {
                Path = "/foo/foo.c",
            };

            pendingChanges.Added.Add(added);
            pendingChanges.Added.Add(addedMeta);

            PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager checkedStateManager =
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager();

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                checkedStateManager);

            checkedStateManager.Update(added, true);

            List<ChangeInfo> changes;
            List<ChangeInfo> dependenciesCandidates;

            tree.GetCheckedChanges(
                false,
                out changes,
                out dependenciesCandidates);

            Assert.IsTrue(changes.Contains(added),
                "Changes should contains the change");

            Assert.IsTrue(changes.Contains(addedMeta),
                "Changes should contains the meta change");
        }

        [Test]
        public void TestCheckedChangesWithDependenciesCandidates()
        {
            UnityPendingChangesTree tree = new UnityPendingChangesTree();

            WorkspaceInfo wkInfo = new WorkspaceInfo("foo", "/foo");

            PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges pendingChanges =
                new PlasticGui.WorkspaceWindow.PendingChanges.PendingChanges(wkInfo);

            ChangeInfo dir = new ChangeInfo()
            {
                Path = "/foo/bar",
            };

            ChangeInfo dirMeta = new ChangeInfo()
            {
                Path = "/foo/bar.meta",
            };


            ChangeInfo added = new ChangeInfo()
            {
                Path = "/foo/bar/foo.c",
            };

            ChangeInfo addedMeta = new ChangeInfo()
            {
                Path = "/foo/bar/foo.c.meta",
            };

            pendingChanges.Added.Add(dir);
            pendingChanges.Added.Add(dirMeta);
            pendingChanges.Added.Add(added);
            pendingChanges.Added.Add(addedMeta);

            PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager checkedStateManager =
                new PlasticGui.WorkspaceWindow.PendingChanges.CheckedStateManager();

            tree.BuildChangeCategories(
                wkInfo.ClientPath,
                pendingChanges,
                checkedStateManager);

            checkedStateManager.Update(added, true);

            List<ChangeInfo> changes;
            List<ChangeInfo> dependenciesCandidates;

            tree.GetCheckedChanges(
                false,
                out changes,
                out dependenciesCandidates);

            Assert.IsTrue(changes.Contains(added),
                "Changes should contains the change");

            Assert.IsTrue(changes.Contains(addedMeta),
                "Changes should contains the meta change");

            Assert.IsTrue(dependenciesCandidates.Contains(dir),
                "Dependencies candidates should contains the dir");

            Assert.IsTrue(dependenciesCandidates.Contains(dirMeta),
                "Dependencies candidates should contains the meta ir");
        }
    }
}
