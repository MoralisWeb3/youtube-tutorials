using Unity.Collections;
using UnityEngine;

namespace PDNWrapper
{
    internal class Surface
    {
        NativeArray<Color32> m_Color;
        public Surface(int w, int h)
        {
            width = w;
            height = h;
            m_Color = new NativeArray<Color32>(width * height, Allocator.Persistent);
        }

        public void Dispose()
        {
            m_Color.Dispose();
        }

        public NativeArray<Color32> color
        {
            get { return m_Color; }
        }

        public int width { get; private set; }
        public int height { get; private set; }
    }
}
