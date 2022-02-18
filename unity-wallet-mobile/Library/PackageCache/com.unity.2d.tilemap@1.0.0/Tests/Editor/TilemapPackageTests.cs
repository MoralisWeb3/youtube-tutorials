using NUnit.Framework;
using UnityEditor.Tilemaps;

namespace UnityEditor.U2D.Tilemap.Editor.PackageTests
{
    internal class TilemapPackageEditorTests
    {
        [Test]
        public void GridPaintPaletteWindow_IsLoadedFromDll()
        {
            Assert.That(typeof(GridPaintPaletteWindow).Assembly.FullName, Contains.Substring("Unity.2D.Tilemap.Editor"));
        }
    }
}
