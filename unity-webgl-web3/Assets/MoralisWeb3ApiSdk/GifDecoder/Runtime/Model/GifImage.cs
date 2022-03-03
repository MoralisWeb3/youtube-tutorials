using UnityEngine;

namespace ThreeDISevenZeroR.UnityGifDecoder.Model
{
    public class GifImage
    {
        public bool userInput;
        public Color32[] colors;
        public int delay;

        public int DelayMs => delay * 10;
        public float SafeDelayMs => delay > 1 ? DelayMs : 100;
        
        public float DelaySeconds => delay / 100f;
        public float SafeDelaySeconds => SafeDelayMs / 1000f;
    }
}