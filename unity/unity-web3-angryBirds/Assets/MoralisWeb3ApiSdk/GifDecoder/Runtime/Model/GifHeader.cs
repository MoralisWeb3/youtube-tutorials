namespace ThreeDISevenZeroR.UnityGifDecoder.Model
{
    public struct GifHeader
    {
        public GifVersion version;
        public int width;
        public int height;
        public bool hasGlobalColorTable;
        public int globalColorTableSize;
        public int transparentColorIndex;
        public bool sortColors;
        public int colorResolution;
        public int pixelAspectRatio;
    }
}