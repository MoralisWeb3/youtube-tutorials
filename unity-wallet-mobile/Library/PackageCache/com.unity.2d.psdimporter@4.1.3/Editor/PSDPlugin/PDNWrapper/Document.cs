using System.Collections;
using System.Collections.Generic;

namespace PDNWrapper
{
    internal class Document
    {
        public int width, height;

        public Document(int w, int h)
        {
            width = w;
            height = h;
            Layers = new List<BitmapLayer>();
        }

        public void Dispose()
        {
            foreach (var layer in Layers)
                layer.Dispose();
        }

        public List<BitmapLayer> Layers { get; set; }

        public MeasurementUnit DpuUnit { get; set; }

        public double DpuX { get; set; }
        public double DpuY { get; set; }
    }
}
