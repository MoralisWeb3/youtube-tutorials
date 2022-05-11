using NUnit.Framework;

using Unity.PlasticSCM.Editor;
using Unity.PlasticSCM.Editor.AssetMenu;

namespace Unity.PlasticSCM.Tests.Editor.AssetMenu
{
    [TestFixture]
    class AssetMenuUpdaterTests
    {
        [Test]
        public void TestCheckoutMenuEnabledForSingleSelection()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = true,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Checkout));
        }

        [Test]
        public void TestCheckoutMenuEnabledForMultipleSelection()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 5,
                IsFileSelection = true,
                IsCheckedInSelection = true,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Checkout));
        }

        [Test]
        public void TestCheckoutMenuDisabledForFolders()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = false,
                IsCheckedInSelection = true,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Checkout));
        }

        [Test]
        public void TestCheckoutMenuDisabledForAlreadyCheckedoutFiles()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = false,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Checkout));
        }

        [Test]
        public void TestCheckoutMenuDisabledForNotControlledSelection()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = false,
                IsControlledSelection = false,
                HasAnyAddedInSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Checkout));
        }

        [Test]
        public void TestDiffMenuEnabledForCheckedInFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = true,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Diff));
        }

        [Test]
        public void TestDiffMenuEnabledForCheckedOutFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = false,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Diff));
        }

        [Test]
        public void TestDiffIsDisabledForFolders()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = false,
                IsCheckedInSelection = false,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Diff));
        }

        [Test]
        public void TestDiffIsDisabledForPrivateItems()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = false,
                IsControlledSelection = false,
                HasAnyAddedInSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Diff));
        }

        [Test]
        public void TestDiffMenuDisabledForMultipleSelection()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 5,
                IsFileSelection = true,
                IsCheckedInSelection = true,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Diff));
        }

        [Test]
        public void TestDiffMenuDisabledForAddedFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = true,
                IsControlledSelection = true,
                HasAnyAddedInSelection = true,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Diff));
        }

        [Test]
        public void TestHistoryMenuEnabledForCheckedInFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = true,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.History));
        }

        [Test]
        public void TestHistoryMenuEnabledForCheckedOutFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = false,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.History));
        }

        [Test]
        public void TestHistoryIsEnabledForFolders()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = false,
                IsCheckedInSelection = false,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.History));
        }

        [Test]
        public void TestHistoryIsDisabledForPrivateItems()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = false,
                IsControlledSelection = false,
                HasAnyAddedInSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.History));
        }

        [Test]
        public void TestHistoryMenuDisabledForMultipleSelection()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 5,
                IsFileSelection = true,
                IsCheckedInSelection = true,
                IsControlledSelection = true,
                HasAnyAddedInSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.History));
        }


        [Test]
        public void TestHistoryMenuDisabledForAddedFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsFileSelection = true,
                IsCheckedInSelection = true,
                IsControlledSelection = true,
                HasAnyAddedInSelection = true,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.History));
        }

        [Test]
        public void TestAddMenuEnabledForPrivateFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsPrivateSelection = true,
                IsFileSelection = true,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Add));
        }

        [Test]
        public void TestAddMenuEnabledForMultiplePrivate()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 5,
                IsPrivateSelection = true,
                IsFileSelection = true,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Add));
        }

        [Test]
        public void TestAddMenuDisabledForControlledFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsPrivateSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Add));
        }

        [Test]
        public void TestAddMenuDisabledForDirectory()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsPrivateSelection = true,
                IsFileSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Add));
        }

        [Test]
        public void TestCheckinMenuEnabledForNonCheckedInFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsControlledSelection = true,
                IsCheckedOutSelection = true,
                IsFileSelection = true,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Checkin));
        }

        [Test]
        public void TestCheckinMenuDisabledForNotControlledSelection()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 5,
                IsControlledSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Checkin));
        }

        [Test]
        public void TestCheckinMenuDisabledForNotCheckedinFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsControlledSelection = true,
                IsCheckedInSelection = true,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Checkin));
        }

        [Test]
        public void TestCheckinMenuDisabledForDirectory()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsControlledSelection = true,
                IsCheckedInSelection = false,
                IsFileSelection = false
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Checkin));
        }

        [Test]
        public void TestUndoMenuEnabledForNonCheckedInFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsControlledSelection = true,
                IsCheckedOutSelection = true,
                IsFileSelection = true,
            };

            Assert.IsTrue(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Undo));
        }

        [Test]
        public void TestUndoMenuDisabledForNotControlledSelection()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 5,
                IsControlledSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Undo));
        }

        [Test]
        public void TestUndoMenuDisabledForNotCheckedinFile()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsControlledSelection = true,
                IsCheckedInSelection = true,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Undo));
        }

        [Test]
        public void TestUndoMenuDisabledForDirectory()
        {
            SelectedAssetGroupInfo groupInfo = new SelectedAssetGroupInfo()
            {
                SelectedCount = 1,
                IsControlledSelection = true,
                IsCheckedInSelection = false,
                IsFileSelection = false,
            };

            Assert.IsFalse(
                AssetMenuUpdater.GetAvailableMenuOperations(groupInfo)
                .HasFlag(AssetMenuOperations.Undo));
        }
    }
}
