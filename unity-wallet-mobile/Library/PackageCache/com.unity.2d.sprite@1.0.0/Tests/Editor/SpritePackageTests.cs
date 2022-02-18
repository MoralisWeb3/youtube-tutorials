using NUnit.Framework;

namespace UnityEditor.U2D.Sprites.EditorTests
{
    internal class SpritePackageEditorTests
    {
        [Test]
        public void SpriteEditorWindow_IsLoadedFromDll()
        {
            Assert.That(typeof(SpriteEditorWindow).Assembly.FullName, Contains.Substring("Unity.2D.Sprite.Editor"));
        }
    }
}
