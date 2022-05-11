using System;
using ThreeDISevenZeroR.UnityGifDecoder.Model;
using UnityEngine;

namespace ThreeDISevenZeroR.UnityGifDecoder
{
    /// <summary>
    /// GIF Canvas buffer for drawing decoded frames
    /// </summary>
    public class GifCanvas
    {
        /// <summary>
        /// Get color array from this Canvas
        /// For performance reasons, actual canvas array is returned, so 
        /// </summary>
        public Color32[] Colors => canvasColors;
        
        /// <summary>
        /// <p>Since pixel rows for Texture2D start from bottom, original gif image will look upside down<br/></p>
        /// <br/>
        /// <p>So gif decoder can flip image Without performance hit,
        /// and you can provide resulting array to Texture2D without flipping it manually</p>
        /// <br/>
        /// <p>Default value is <b>TRUE</b>, if you want original color order
        /// (which will look flipped on Texture2D) you can set this to false</p>
        /// </summary>
        public bool FlipVertically { get; set; } = true;

        /// <summary>
        /// Color which will be used for background fill<br/>
        /// Note, that background is always transparent, so only r, g, b components are used (alpha is 0)
        /// </summary>
        public Color32 BackgroundColor { get; set; }

        private Color32[] canvasColors;
        private Color32[] revertDisposalBuffer;
        private int canvasWidth;
        private int canvasHeight;
        private bool canvasIsEmpty;

        private Color32[] framePalette;
        private GifDisposalMethod frameDisposalMethod;

        private int frameCanvasPosition;
        private int frameCanvasRowEndPosition;
        private int frameTransparentColorIndex;
        private int frameRowCurrent;
        private int frameX;
        private int frameY;
        private int frameWidth;
        private int frameHeight;
        private int[] frameRowStart;
        private int[] frameRowEnd;

        public GifCanvas()
        {
            canvasIsEmpty = true;
        }
        
        public GifCanvas(int width, int height) : this()
        {
            SetSize(width, height);
        }

        /// <summary>
        /// Resizes canvas and resets its initial state
        /// </summary>
        /// <param name="width">New canvas width</param>
        /// <param name="height">New canvas height</param>
        public void SetSize(int width, int height)
        {
            if (width != canvasWidth || height != canvasHeight)
            {
                var size = width * height;
                canvasColors = new Color32[size];
                frameRowStart = new int[height];
                frameRowEnd = new int[height];
                revertDisposalBuffer = null;

                canvasWidth = width;
                canvasHeight = height;
            }

            Reset();
        }

        /// <summary>
        /// Clears canvas colors and resets it to initial state
        /// </summary>
        public void Reset()
        {
            frameDisposalMethod = GifDisposalMethod.Keep;
            frameX = 0;
            frameY = 0;
            frameWidth = canvasWidth;
            frameHeight = canvasHeight;

            if (!canvasIsEmpty)
            {
                FillWithColor(0, 0, canvasWidth, canvasHeight, new Color32(BackgroundColor.r, BackgroundColor.g, BackgroundColor.b, 0));
                canvasIsEmpty = true;
            }
        }

        /// <summary>
        /// Method initiates new drawing, sequential calls to OutputPixel will draw everything on canvas
        /// </summary>
        /// <param name="x">Left offset of frame</param>
        /// <param name="y">Top offset of frame</param>
        /// <param name="width">Frame width</param>
        /// <param name="height">Frame height</param>
        /// <param name="palette">Color palette for this frame</param>
        /// <param name="transparentColorIndex">Index of transparent color, color from this index will be treated as transparent</param>
        /// <param name="isInterlaced">Apply deinterlacing during drawing</param>
        /// <param name="disposalMethod">Specifies, how to handle this frame when next frame is drawn</param>
        public void BeginNewFrame(int x, int y, int width, int height, Color32[] palette, 
            int transparentColorIndex, bool isInterlaced, GifDisposalMethod disposalMethod)
        {
            switch (frameDisposalMethod)
            {
                case GifDisposalMethod.ClearToBackgroundColor:
                    FillWithColor(frameX, frameY, frameWidth, frameHeight, 
                        new Color32(BackgroundColor.r, BackgroundColor.g, BackgroundColor.b, 0));

                    break;

                case GifDisposalMethod.Revert:
                    if(disposalMethod != GifDisposalMethod.Keep)
                        Array.Copy(revertDisposalBuffer, 0, canvasColors, 0, revertDisposalBuffer.Length);
                    break;
            }

            switch (disposalMethod)
            {
                case GifDisposalMethod.Revert:
                    if (revertDisposalBuffer == null)
                        revertDisposalBuffer = new Color32[canvasColors.Length];

                    Array.Copy(canvasColors, 0,
                        revertDisposalBuffer, 0, revertDisposalBuffer.Length);
                    break;
            }

            framePalette = palette;
            frameDisposalMethod = disposalMethod;
            canvasIsEmpty = false;
            frameWidth = width;
            frameHeight = height;
            frameX = x;
            frameY = y;

            // Start before canvas, so next pixel output will load correct region
            frameCanvasPosition = 0;
            frameRowCurrent = -1;
            frameCanvasRowEndPosition = -1;
            frameTransparentColorIndex = transparentColorIndex;
            
            RouteFrameDrawing(x, y, width, height, isInterlaced);
        }

        /// <summary>
        /// <p>Place pixel on canvas</p>
        /// <br/>
        /// <p>Pixel will be placed inside region specified by "BeginNewFrame",
        /// sequential calls to "OutputPixel" will fill region eventually</p>
        /// </summary>
        /// <param name="color">Index of color from palette to place on canvas</param>
        public void OutputPixel(int color)
        {
            if (frameCanvasPosition >= frameCanvasRowEndPosition)
            {
                frameRowCurrent++;
                frameCanvasPosition = frameRowStart[frameRowCurrent];
                frameCanvasRowEndPosition = frameRowEnd[frameRowCurrent];
            }
            
            if (color != frameTransparentColorIndex)
                canvasColors[frameCanvasPosition] = framePalette[color];

            frameCanvasPosition++;
        }
        
        /// <summary>
        /// Fill specified region with single color
        /// </summary>
        public void FillWithColor(int x, int y, int width, int height, Color32 color)
        {
            if (width == canvasWidth && height == canvasHeight)
            {
                for (var i = canvasColors.Length - 1; i >= 0; i--)
                    canvasColors[i] = color;
            }
            else
            {
                int yStart;
                int yEnd;
                    
                if (FlipVertically)
                {
                    yEnd = (canvasHeight - y) * canvasWidth + x;
                    yStart = yEnd - canvasWidth * height;
                }
                else
                {
                    yStart = y * canvasWidth + x;
                    yEnd = yStart + height * canvasWidth;
                }

                for (var ySrc = yStart; ySrc < yEnd; ySrc += canvasWidth)
                {
                    var rowEnd = ySrc + width;
                    for (var i = ySrc; i < rowEnd; i++)
                        canvasColors[i] = color;
                }
            }
        }

        /// <summary>
        /// <p>Plan most optimal image drawing route</p>
        /// <p>So colors can be written to final locations right from the start,
        /// without intermediate buffers or sorting</p>
        /// </summary>
        private void RouteFrameDrawing(int x, int y, int width, int height, bool deinterlace)
        {
            var currentRow = 0;

            void ScheduleRowIndex(int row)
            {
                var startPosition = FlipVertically 
                    ? (canvasHeight - 1 - (y + row)) * canvasWidth + x
                    : (y + row) * canvasWidth + x;
                
                frameRowStart[currentRow] = startPosition;
                frameRowEnd[currentRow] = startPosition + width;
                currentRow++;
            }

            if (deinterlace)
            {
                for (var i = 0; i < height; i += 8) ScheduleRowIndex(i); // every 8, start with 0
                for (var i = 4; i < height; i += 8) ScheduleRowIndex(i); // every 8, start with 4
                for (var i = 2; i < height; i += 4) ScheduleRowIndex(i); // every 4, start with 2
                for (var i = 1; i < height; i += 2) ScheduleRowIndex(i); // every 2, start with 1
            }
            else
            {
                for (var i = 0; i < height; i++) ScheduleRowIndex(i); // every row in order
            }
        }
    }
}