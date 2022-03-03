namespace ThreeDISevenZeroR.UnityGifDecoder.Model
{
    public enum GifDisposalMethod
    {
        /// <summary>
        /// Keep previous frame and draw new frame on top of it
        /// </summary>
        Keep,
            
        /// <summary>
        /// Clear previous region
        /// </summary>
        ClearToBackgroundColor,
            
        /// <summary>
        /// Revert previous drawing operation, so canvas will contain previous frame
        /// </summary>
        Revert
    }
}