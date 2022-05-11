using System.Collections;
using System.Collections.Generic;

namespace PDNWrapper
{
    internal static class Layer
    {
        public static BitmapLayer CreateBackgroundLayer(int w, int h)
        {
            return new BitmapLayer(w, h);
        }
    }

    internal class BitmapLayer
    {
        int width, height;

        public Rectangle Bounds
        {
            get {return new Rectangle(0, 0, width, height); }
        }

        public void Dispose()
        {
            Surface.Dispose();
            foreach (var layer in ChildLayer)
                layer.Dispose();
        }

        public BitmapLayer(int w, int h)
        {
            Surface = new Surface(w, h);
            width = w;
            height = h;
            ChildLayer = new List<BitmapLayer>();
            IsGroup = false;
        }
        public int LayerID { get; set; }

        public bool IsGroup {get; set; }
        public BitmapLayer ParentLayer {get; set; }
        public List<BitmapLayer> ChildLayer { get; set; }
        public string Name { get; set; }
        public byte Opacity { get; set; }
        public bool Visible { get; set; }
        public LayerBlendMode BlendMode { get; set; }

        public Surface Surface { get; set; }
    }
}
