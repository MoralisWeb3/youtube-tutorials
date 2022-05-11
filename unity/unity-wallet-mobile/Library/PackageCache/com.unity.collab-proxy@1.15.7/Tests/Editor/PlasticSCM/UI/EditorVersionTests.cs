using NUnit.Framework;

using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Tests.Editor.UI
{
    [TestFixture]
    internal class EditorVersionTests
    {
        [Test]
        public void EditorVersionTests_Equals()
        {
            Assert.False(EditorVersion.IsEditorOlderThan("2019.1.10f1", "2019.1.10f1"));
        }

        [Test]
        public void EditorVersionTests_Newer()
        {
            Assert.False(EditorVersion.IsEditorOlderThan("2019.2.10f1", "2018.100.100f1"));
            Assert.False(EditorVersion.IsEditorOlderThan("2019.2.10f1", "2019.1.100f1"));
            Assert.False(EditorVersion.IsEditorOlderThan("2019.2.10f1", "2019.2.1f1"));
        }

        [Test]
        public void EditorVersionTests_Older()
        {
            Assert.True(EditorVersion.IsEditorOlderThan("2019.2.10f1", "2020.1.1f1"));
            Assert.True(EditorVersion.IsEditorOlderThan("2019.2.10f1", "2019.3.1f1"));
            Assert.True(EditorVersion.IsEditorOlderThan("2019.2.10f1", "2019.2.11f1"));
        }
    }
}
