using UnityEngine;

namespace ThreeDISevenZeroR.UnityGifDecoder.Model
{
    public struct GifPlainText
    {
        public int left;
        public int top;
        public int width;
        public int height;
        public int charWidth;
        public int charHeight;
        public Color32 backgroundColor;
        public Color32 foregroundColor;
        public string text;
        public Color32[] colors;
    }
}