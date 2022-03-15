using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ThreeDISevenZeroR.UnityGifDecoder.Decode;
using ThreeDISevenZeroR.UnityGifDecoder.Model;
using ThreeDISevenZeroR.UnityGifDecoder.Utils;
using UnityEngine;

namespace ThreeDISevenZeroR.UnityGifDecoder
{
    /// <summary>
    /// Main class for gif decoding
    ///
    /// Reads and decodes gif file sequentially in PullParser manner <br/>
    /// This class can be called from any thread but one at a time, there is no thead safety mechanism<br/>
    /// <br/>
    /// Example usage:<br/>
    /// <code>
    /// using (var gifStream = new GifStream(&lt;yourFile&gt;))
    /// {
    ///     while (gifStream.HasMoreData)
    ///     {
    ///         switch (gifStream.CurrentToken)
    ///         {
    ///             case GifStream.Token.Image:
    ///                 var image = gifStream.ReadImage();
    ///                 // do something with image
    ///                 break;
    ///                     
    ///             case GifStream.Token.Comment:
    ///                 var comment = gifStream.ReadComment();
    ///                 // log this comment
    ///                 break;
    /// 
    ///             default:
    ///                 gifStream.SkipToken();
    ///                 // this token has no use for you, skip it
    ///                 break;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </summary>
    public class GifStream : IDisposable
    {
        /// <summary>
        /// See: <see cref="GifCanvas.FlipVertically" />
        /// </summary>
        public bool FlipVertically
        {
            get => canvas.FlipVertically;
            set => canvas.FlipVertically = value;
        }

        /// <summary>
        /// <p>Plain text block is not fully supported (since no one uses it), but if you want,
        /// you can at least fill text drawing region with background color</p>
        /// <br/>
        /// <p>False by default, since web browsers skip text rendering completely</p>
        /// </summary>
        public bool DrawPlainTextBackground { get; set; }
        
        /// <summary>
        /// Last encountered header data from stream
        /// </summary>
        public GifHeader Header => header;

        /// <summary>
        /// <p>Is this stream hast more data or it is completed.
        /// If this value is False, you can close this stream or call Reset() and read everything again</p>
        /// <br/>
        /// <p>Essentially it equals to CurrentToken != EndOfFile</p>
        /// </summary>
        public bool HasMoreData => CurrentToken != Token.EndOfFile;
        
        /// <summary>
        /// <p>Current stream token</p>
        /// <p>You should call matching read method or skip it</p>
        /// </summary>
        public Token CurrentToken { get; private set; }
        
        /// <summary>
        /// Underlying stream which is used for gif data loading
        /// </summary>
        public Stream BaseStream
        {
            get => currentStream;
            set => SetStream(value);
        }
        
        private Stream currentStream;
        private long headerStartPosition;
        private long firstFrameStartPosition;

        private GifHeader header;
        private GifGraphicControl graphicControl;
        private GifImageDescriptor imageDescriptor;

        private GifCanvas canvas;
        private GifLzwDictionary lzwDictionary;
        private GifBitBlockReader blockReader;

        private Color32[] globalColorTable;
        private Color32[] localColorTable;
        private readonly byte[] headerBuffer;
        private readonly byte[] colorTableBuffer;
        private readonly byte[] extensionApplicationBuffer;
        private bool nextPaletteIsGlobal;

        /// <summary>
        /// Creates GifStream instance without Stream and preallocates resources for gif decoding
        /// </summary>
        public GifStream()
        {
            lzwDictionary = new GifLzwDictionary();
            canvas = new GifCanvas();
            blockReader = new GifBitBlockReader();
            
            globalColorTable = new Color32[256]; 
            localColorTable = new Color32[256]; 
            headerBuffer = new byte[6]; 
            extensionApplicationBuffer = new byte[11];
            colorTableBuffer = new byte[768];
        }

        /// <summary>
        /// <p>Convenience constructor</p>
        /// <p>Invokes original constructor and sets stream to read from</p>
        /// </summary>
        /// <param name="stream">Stream to read gif from</param>
        public GifStream(Stream stream) : this()
        {
            SetStream(stream);
        }

        /// <summary>
        /// <p>Convenience constructor</p>
        /// <p>Invokes original constructor and sets MemoryStream with specified bytes</p>
        /// </summary>
        /// <param name="gifBytes">bytes of gif file</param>
        public GifStream(byte[] gifBytes) : this(new MemoryStream(gifBytes)) { }

        /// <summary>
        /// <p>Convenience constructor</p>
        /// <p>Invokes original constructor and open stream from file path</p>
        /// <b>Don't forget to call Dispose() to close file</b>
        /// </summary>
        /// <param name="path">Path to gif file</param>
        public GifStream(string path) : this(File.OpenRead(path)) { }

        /// <summary>
        /// <p>Sets new stream to read gif data from</p>
        /// <br/>
        /// <p>GifStream is reusable, you can change stream and read new gif from it.
        /// That way you reuse allocations that you've made, and keep your memory usage to minimum</p>
        /// <br/>
        /// <p>Stream will be reset to its initial state</p>
        /// </summary>
        /// <param name="stream">new stream with gif data</param>
        /// <param name="disposePrevious">Dispose previous stream</param>
        public void SetStream(Stream stream, bool disposePrevious = false)
        {
            if (disposePrevious)
                currentStream?.Dispose();

            header = new GifHeader();
            imageDescriptor = new GifImageDescriptor();
            graphicControl = new GifGraphicControl();
            
            currentStream = stream;
            CurrentToken = Token.Header;
            blockReader.SetStream(stream);
        }

        /// <summary>
        /// Disposes underlying Stream
        /// </summary>
        public void Dispose()
        {
            currentStream?.Dispose();
        }

        /// <summary>
        /// <p>Skips current token</p>
        /// <br/>
        /// <p>Despite the name, this method not always skips data, it will read and decode all image related data,
        /// since next image rendering will break if this data is skipped.
        /// But it skips comments, extensions or plain text blocks without memory allocations</p>
        /// </summary>
        /// <exception cref="InvalidOperationException">If this is unskippable token</exception>
        public void SkipToken()
        {
            switch (CurrentToken)
            {
                case Token.Header: ReadHeader(); break;
                case Token.Palette: ReadPalette(); break;
                case Token.GraphicsControl: ReadGraphicsControl(); break;
                case Token.ImageDescriptor: ReadImageDescriptor(); break;
                case Token.Image: ReadImage(); break;
                case Token.Comment: SkipComment(); break;
                case Token.PlainText: SkipPlainText(); break;
                case Token.NetscapeExtension: SkipNetscapeExtension(); break;
                case Token.ApplicationExtension: SkipApplicationExtension(); break;
                default: throw new InvalidOperationException($"Cannot skip token {CurrentToken}");
            }
        }
        
        /// <summary>
        /// Resets gif stream state, so you can read it from beginning again<br/>
        /// This is useful when you need to playback gif from memory
        /// </summary>
        public void Reset(bool skipHeader = true, bool resetCanvas = true)
        {
            var targetPosition = skipHeader && firstFrameStartPosition != -1
                ? firstFrameStartPosition
                : headerStartPosition;
            
            if (currentStream.Position != targetPosition)
                currentStream.Position = targetPosition;

            SetCurrentToken(Token.Header);

            if (resetCanvas)
                canvas.Reset();
        }

        /// <summary>
        /// Read gif header
        /// </summary>
        /// <returns>Data of gif header</returns>
        /// <exception cref="ArgumentException">If file is not gif file</exception>
        public GifHeader ReadHeader()
        {
            AssertToken(Token.Header);

            // Header
            headerStartPosition = currentStream.Position;
            firstFrameStartPosition = -1;
            currentStream.Read(headerBuffer, 0, headerBuffer.Length);

            if(BitUtils.CheckString(headerBuffer, "GIF87a")) 
                header.version = GifVersion.Gif87a;
            else if (BitUtils.CheckString(headerBuffer, "GIF89a"))
                header.version = GifVersion.Gif89a;
            else
                throw new ArgumentException("Invalid or corrupted Gif file");

            // Screen descriptor
            header.width = currentStream.ReadInt16LittleEndian();
            header.height = currentStream.ReadInt16LittleEndian();

            var flags = currentStream.ReadByte8();
            header.globalColorTableSize = BitUtils.GetColorTableSize(flags.GetBitsFromByte(0, 3));
            header.sortColors = flags.GetBitFromByte(3);
            header.colorResolution = flags.GetBitsFromByte(4, 3);
            header.hasGlobalColorTable = flags.GetBitFromByte(7);
            
            header.transparentColorIndex = currentStream.ReadByte8();
            header.pixelAspectRatio = currentStream.ReadByte8();

            canvas.SetSize(header.width, header.height);

            if (header.hasGlobalColorTable)
            {
                SetCurrentToken(Token.Palette);
                nextPaletteIsGlobal = true;
            }
            else
            {
                DetermineNextToken();
            }

            return header;
        }

        public GifPalette ReadPalette()
        {
            AssertToken(Token.Palette);

            var size = nextPaletteIsGlobal ? header.globalColorTableSize : imageDescriptor.localColorTableSize;
            var palette = nextPaletteIsGlobal ? globalColorTable : localColorTable;

            currentStream.Read(colorTableBuffer, 0, size * 3);

            var position = 0;
            for (var i = 0; i < size; i++)
            {
                palette[i] = new Color32(
                    colorTableBuffer[position++],
                    colorTableBuffer[position++],
                    colorTableBuffer[position++],
                    255);
            }

            if (nextPaletteIsGlobal)
            {
                firstFrameStartPosition = currentStream.Position;
                DetermineNextToken();
            }
            else
            {
                SetCurrentToken(Token.Image);
            }

            return new GifPalette
            {
                palette = palette,
                size = size,
                isGlobal = nextPaletteIsGlobal
            };
        }

        public GifGraphicControl ReadGraphicsControl()
        {
            AssertToken(Token.GraphicsControl);
            
            currentStream.AssertByte(0x04);
            var graphicsFlags = currentStream.ReadByte8();
            var disposalMethodValue = graphicsFlags.GetBitsFromByte(2, 3);

            graphicControl.hasTransparency = graphicsFlags.GetBitFromByte(0);
            graphicControl.userInput = graphicsFlags.GetBitFromByte(1);
            graphicControl.delayTime = currentStream.ReadInt16LittleEndian();
            graphicControl.transparentColorIndex = currentStream.ReadByte8();

            // Color index should be read anyway, so there is no point to not read original transparentColorIndex value
            if (!graphicControl.hasTransparency)
                graphicControl.transparentColorIndex = -1;

            switch (disposalMethodValue)
            {
                case 0:
                case 1: graphicControl.disposalMethod = GifDisposalMethod.Keep; break;
                case 2: graphicControl.disposalMethod = GifDisposalMethod.ClearToBackgroundColor; break;
                case 3: graphicControl.disposalMethod = GifDisposalMethod.Revert; break;
                default: throw new ArgumentException($"Invalid disposal method type: {disposalMethodValue}");
            }

            currentStream.AssertByte(0x00);
            DetermineNextToken();

            return graphicControl;
        }

        public GifImageDescriptor ReadImageDescriptor()
        {
            AssertToken(Token.ImageDescriptor);
            imageDescriptor.left = currentStream.ReadInt16LittleEndian();
            imageDescriptor.top = currentStream.ReadInt16LittleEndian();
            imageDescriptor.width = currentStream.ReadInt16LittleEndian();
            imageDescriptor.height = currentStream.ReadInt16LittleEndian();
            
            var flags = currentStream.ReadByte8();

            imageDescriptor.localColorTableSize = BitUtils.GetColorTableSize(flags.GetBitsFromByte(0, 3));
            imageDescriptor.isInterlaced = flags.GetBitFromByte(6);
            imageDescriptor.hasLocalColorTable = flags.GetBitFromByte(7);

            if (imageDescriptor.hasLocalColorTable)
            {
                nextPaletteIsGlobal = false;
                SetCurrentToken(Token.Palette);
            }
            else
            {
                SetCurrentToken(Token.Image);
            }

            return imageDescriptor;
        }

        /// <summary>
        /// Read and construct actual frame from previous encountered gif data
        /// </summary>
        /// <returns></returns>
        public GifImage ReadImage()
        {
            AssertToken(Token.Image);
            
            var usedColorTable = imageDescriptor.hasLocalColorTable
                ? localColorTable
                : globalColorTable;

            var lzwMinCodeSize = currentStream.ReadByte8();
            
            if(lzwMinCodeSize == 0 || lzwMinCodeSize > 8)
                throw new ArgumentException("Invalid lzw min code size");

            DecodeLzwImageToCanvas(lzwMinCodeSize, 
                imageDescriptor.left, imageDescriptor.top, 
                imageDescriptor.width, imageDescriptor.height, usedColorTable,
                graphicControl.transparentColorIndex, 
                imageDescriptor.isInterlaced, graphicControl.disposalMethod);
            DetermineNextToken();
            
            return new GifImage
            {
                colors = canvas.Colors,
                userInput = graphicControl.userInput,
                delay = graphicControl.delayTime
            };
        }

        public string ReadComment()
        {
            AssertToken(Token.Comment);
            var text = Encoding.ASCII.GetString(BitUtils.ReadGifBlocks(currentStream));
            DetermineNextToken();
            return text;
        }

        public void SkipComment() => SkipBlock(Token.Comment);

        public GifPlainText ReadPlainText()
        {
            AssertToken(Token.PlainText);
            currentStream.AssertByte(0x0c);
            
            var result = new GifPlainText();
            result.left = currentStream.ReadInt16LittleEndian();
            result.top = currentStream.ReadInt16LittleEndian();
            result.width = currentStream.ReadInt16LittleEndian();
            result.height = currentStream.ReadInt16LittleEndian();
            result.charWidth = currentStream.ReadByte8();
            result.charHeight = currentStream.ReadByte8();
            result.foregroundColor = globalColorTable[currentStream.ReadByte8()];
            result.backgroundColor = globalColorTable[currentStream.ReadByte8()];
            result.text = Encoding.ASCII.GetString(BitUtils.ReadGifBlocks(currentStream));
            result.colors = canvas.Colors;

            if (DrawPlainTextBackground)
                FillPlainTextBackground(result);

            DetermineNextToken();

            return result;
        }

        public void SkipPlainText()
        {
            if (DrawPlainTextBackground)
                ReadPlainText();
            else
                SkipBlock(Token.PlainText);
        }

        public GifNetscapeExtension ReadNetscapeExtension()
        {
            AssertToken(Token.NetscapeExtension);

            var hasBufferSize = false;
            var hasLoopCount = false;
            var loopCount = 0;
            var bufferSize = 0;

            while (true)
            {
                var blockSize = currentStream.ReadByte8();

                if (blockSize == 0)
                    break;

                var blockId = currentStream.ReadByte8();

                switch (blockId)
                {
                    case 0x01:
                        hasLoopCount = true;
                        loopCount = currentStream.ReadInt16LittleEndian();
                        break;
                    
                    case 0x02:
                        hasBufferSize = true;
                        bufferSize = currentStream.ReadInt32LittleEndian();
                        break;
                    
                    default:
                        currentStream.Seek(blockSize - 1, SeekOrigin.Current);
                        break;
                }
            }

            DetermineNextToken();
            
            return new GifNetscapeExtension
            {
                hasLoopCount = hasLoopCount,
                hasBufferSize = hasBufferSize,
                loopCount = loopCount,
                bufferSize = bufferSize
            };
        }
        
        public void SkipNetscapeExtension() => SkipBlock(Token.NetscapeExtension);
        
        public GifApplicationExtension ReadApplicationExtension()
        {
            AssertToken(Token.ApplicationExtension);
            
            var blocks = new List<byte[]>();
            var appName = Encoding.ASCII.GetString(extensionApplicationBuffer, 0, 8);
            var appCode = Encoding.ASCII.GetString(extensionApplicationBuffer, 8, 3);

            while (true)
            {
                var blockSize = currentStream.ReadByte8();

                if (blockSize == 0)
                    break;
                
                var array = new byte[blockSize];
                currentStream.Read(array, 0, blockSize);
                blocks.Add(array);
            }
            
            DetermineNextToken();
            
            return new GifApplicationExtension
            {
                applicationIdentifier = appName,
                applicationAuthCode = appCode,
                applicationData = blocks.ToArray()
            };
        }

        public void SkipApplicationExtension() => SkipBlock(Token.ApplicationExtension);
        
        private void DecodeLzwImageToCanvas(int lzwMinCodeSize, int x, int y, int width, int height,
            Color32[] colorTable, int transparentColorIndex, bool isInterlaced, GifDisposalMethod disposalMethod)
        {
            if (header.hasGlobalColorTable)
                canvas.BackgroundColor = globalColorTable[header.transparentColorIndex];
            
            canvas.BeginNewFrame(x, y, width, height, colorTable, transparentColorIndex, isInterlaced, disposalMethod);
            
            lzwDictionary.InitWithWordSize(lzwMinCodeSize);
            blockReader.StartNewReading();
            
            lzwDictionary.DecodeStream(blockReader, canvas);
            blockReader.FinishReading();
        }
        
        private Token DetermineNextToken()
        {
            while (true)
            {
                var blockType = currentStream.ReadByte8();
                switch (blockType)
                {
                    case ExtensionBlock:
                        var extensionType = currentStream.ReadByte8();
                        switch (extensionType)
                        {
                            case commentLabel: return SetCurrentToken(Token.Comment);
                            case PlainTextLabel: return SetCurrentToken(Token.PlainText);
                            case GraphicControlLabel: return SetCurrentToken(Token.GraphicsControl);
                            case applicationExtensionLabel:
                            {
                                currentStream.AssertByte(11);
                                currentStream.Read(extensionApplicationBuffer, 0, 11);

                                var token = BitUtils.CheckString(extensionApplicationBuffer, "NETSCAPE2.0")
                                    ? Token.NetscapeExtension
                                    : Token.ApplicationExtension;
                                
                                return SetCurrentToken(token);
                            }
                                
                            default: BitUtils.SkipGifBlocks(currentStream); break;
                        }

                        break;

                    case ImageDescriptorBlock: return SetCurrentToken(Token.ImageDescriptor);
                    case EndOfFile: return SetCurrentToken(Token.EndOfFile);
                    default: throw new ArgumentException($"Unknown block type {blockType}");
                }
            }
        }

        private Token SetCurrentToken(Token token)
        {
            CurrentToken = token;
            return token;
        }

        private void FillPlainTextBackground(GifPlainText text)
        {
            canvas.BeginNewFrame(text.left, text.top, text.width, text.height, globalColorTable,
                graphicControl.transparentColorIndex, imageDescriptor.isInterlaced, graphicControl.disposalMethod);
            
            canvas.FillWithColor(text.left, text.top, 
                text.width, text.height, text.backgroundColor);
        }
        
        private void AssertToken(Token token)
        {
            if (CurrentToken != token)
                throw new InvalidOperationException(
                    $"Cannot invoke this method while current token is \"{CurrentToken}\", " +
                    $"method should be called when token is {token}");
        }
        
        private void SkipBlock(Token token)
        {
            AssertToken(token);
            BitUtils.SkipGifBlocks(currentStream);
            DetermineNextToken();
        }

        public enum Token
        {
            Header,
            Palette,
            GraphicsControl,
            ImageDescriptor,
            Image,
            Comment,
            PlainText,
            NetscapeExtension,
            ApplicationExtension,
            EndOfFile
        }

        private const int ExtensionBlock = 0x21;
        private const int ImageDescriptorBlock = 0x2c;
        private const int EndOfFile = 0x3b;

        private const int PlainTextLabel = 0x01;
        private const int GraphicControlLabel = 0xf9;
        private const int commentLabel = 0xfe;
        private const int applicationExtensionLabel = 0xff;
    }
}