namespace ThreeDISevenZeroR.UnityGifDecoder.Model
{
    public struct GifImageDescriptor
    {
        public int left;
        public int top;
        public int width;
        public int height;

        public bool isInterlaced;
        public bool hasLocalColorTable;
        public int localColorTableSize;
    }
}