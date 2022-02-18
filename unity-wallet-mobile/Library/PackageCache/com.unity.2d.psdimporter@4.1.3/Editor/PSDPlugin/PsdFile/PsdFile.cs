/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2016 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PDNWrapper;
using System.IO;
using System.Linq;
using System.Text;
using PaintDotNet.Data.PhotoshopFileType;

namespace PhotoshopFile
{
    [Flags]
    internal enum ELoadFlag
    {
        Header = 1,
        ColorMode = Header | 1 << 2,
        ImageData = ColorMode | 1 << 3,
        All = Header | ColorMode | ImageData
    }


    internal enum PsdColorMode
    {
        Bitmap = 0,
        Grayscale = 1,
        Indexed = 2,
        RGB = 3,
        CMYK = 4,
        Multichannel = 7,
        Duotone = 8,
        Lab = 9
    };

    internal enum PsdFileVersion : short
    {
        Psd = 1,
        PsbLargeDocument = 2
    }

    internal class PsdFile
    {
        #region Constructors

        ELoadFlag m_LoadFlag;

        public PsdFile(PsdFileVersion version = PsdFileVersion.Psd)
        {
            Version = version;

            BaseLayer = new Layer(this);
            ImageResources = new ImageResources();
            Layers = new List<Layer>();
            AdditionalInfo = new List<LayerInfo>();
        }

        public PsdFile(string filename, LoadContext loadContext, ELoadFlag loadFlag = ELoadFlag.All)
            : this()
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                Load(stream, loadContext, loadFlag);
            }
        }

        public PsdFile(Stream stream, LoadContext loadContext, ELoadFlag loadFlag = ELoadFlag.All)
            : this()
        {
            Load(stream, loadContext, loadFlag);
        }

        #endregion

        #region Load and save

        internal LoadContext LoadContext { get; private set; }

        private void Load(Stream stream, LoadContext loadContext, ELoadFlag loadFlag)
        {
            LoadContext = loadContext;
            var reader = new PsdBinaryReader(stream, loadContext.Encoding);

            if ((loadFlag & ELoadFlag.Header) == ELoadFlag.Header)
                LoadHeader(reader);

            if ((loadFlag & ELoadFlag.ColorMode) == ELoadFlag.ColorMode)
                LoadColorModeData(reader);

            if ((loadFlag & ELoadFlag.ImageData) == ELoadFlag.ImageData)
            {
                LoadImageResources(reader);
                LoadLayerAndMaskInfo(reader);

                LoadImage(reader);
                DecompressImages();
            }
        }

        #endregion

        #region Header

        /// <summary>
        /// Photoshop file format version.
        /// </summary>
        public PsdFileVersion Version { get; private set; }

        public bool IsLargeDocument
        {
            get { return Version == PsdFileVersion.PsbLargeDocument; }
        }

        private Int16 channelCount;
        /// <summary>
        /// The number of channels in the image, including any alpha channels.
        /// </summary>
        public Int16 ChannelCount
        {
            get { return channelCount; }
            set
            {
                if (value < 1 || value > 56)
                    throw new ArgumentException("Number of channels must be from 1 to 56.");
                channelCount = value;
            }
        }

        private void CheckDimension(int dimension)
        {
            if (dimension < 1)
            {
                throw new ArgumentException("Image dimension must be at least 1.");
            }
            if ((Version == PsdFileVersion.Psd) && (dimension > 30000))
            {
                throw new ArgumentException("PSD image dimension cannot exceed 30000.");
            }
            if ((Version == PsdFileVersion.PsbLargeDocument) && (dimension > 300000))
            {
                throw new ArgumentException("PSB image dimension cannot exceed 300000.");
            }
        }

        /// <summary>
        /// The height of the image in pixels.
        /// </summary>
        public int RowCount
        {
            get { return this.BaseLayer.Rect.Height; }
            set
            {
                CheckDimension(value);
                BaseLayer.Rect = new Rectangle(0, 0, BaseLayer.Rect.Width, value);
            }
        }


        /// <summary>
        /// The width of the image in pixels.
        /// </summary>
        public int ColumnCount
        {
            get { return this.BaseLayer.Rect.Width; }
            set
            {
                CheckDimension(value);
                BaseLayer.Rect = new Rectangle(0, 0, value, BaseLayer.Rect.Height);
            }
        }

        private int bitDepth;
        /// <summary>
        /// The number of bits per channel. Supported values are 1, 8, 16, and 32.
        /// </summary>
        public int BitDepth
        {
            get { return bitDepth; }
            set
            {
                switch (value)
                {
                    case 1:
                    case 8:
                    case 16:
                    case 32:
                        bitDepth = value;
                        break;
                    default:
                        throw new NotImplementedException("Invalid bit depth.");
                }
            }
        }

        /// <summary>
        /// The color mode of the file.
        /// </summary>
        public PsdColorMode ColorMode { get; set; }

        ///////////////////////////////////////////////////////////////////////////

        private void LoadHeader(PsdBinaryReader reader)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, File header");

            var signature = reader.ReadAsciiChars(4);
            if (signature != "8BPS")
                throw new PsdInvalidException("The given stream is not a valid PSD file");

            Version = (PsdFileVersion)reader.ReadInt16();
            Util.DebugMessage(reader.BaseStream, "Load, Info, Version {0}", (int)Version);
            if ((Version != PsdFileVersion.Psd)
                && (Version != PsdFileVersion.PsbLargeDocument))
            {
                throw new PsdInvalidException("The PSD file has an unknown version");
            }

            //6 bytes reserved
            reader.BaseStream.Position += 6;

            this.ChannelCount = reader.ReadInt16();
            this.RowCount = reader.ReadInt32();
            this.ColumnCount = reader.ReadInt32();
            BitDepth = reader.ReadInt16();
            ColorMode = (PsdColorMode)reader.ReadInt16();

            Util.DebugMessage(reader.BaseStream, "Load, End, File header");
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region ColorModeData

        /// <summary>
        /// If ColorMode is ColorModes.Indexed, the following 768 bytes will contain
        /// a 256-color palette. If the ColorMode is ColorModes.Duotone, the data
        /// following presumably consists of screen parameters and other related information.
        /// Unfortunately, it is intentionally not documented by Adobe, and non-Photoshop
        /// readers are advised to treat duotone images as gray-scale images.
        /// </summary>
        public byte[] ColorModeData = new byte[0];

        private void LoadColorModeData(PsdBinaryReader reader)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, ColorModeData");

            var paletteLength = reader.ReadUInt32();
            if (paletteLength > 0)
            {
                ColorModeData = reader.ReadBytes((int)paletteLength);
            }

            Util.DebugMessage(reader.BaseStream, "Load, End, ColorModeData");
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region ImageResources

        /// <summary>
        /// The Image resource blocks for the file
        /// </summary>
        public ImageResources ImageResources { get; set; }

        public ResolutionInfo Resolution
        {
            get
            {
                return (ResolutionInfo)ImageResources.Get(ResourceID.ResolutionInfo);
            }

            set
            {
                ImageResources.Set(value);
            }
        }


        ///////////////////////////////////////////////////////////////////////////

        private void LoadImageResources(PsdBinaryReader reader)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, ImageResources");

            var imageResourcesLength = reader.ReadUInt32();
            if (imageResourcesLength <= 0)
                return;

            var startPosition = reader.BaseStream.Position;
            var endPosition = startPosition + imageResourcesLength;
            while (reader.BaseStream.Position < endPosition)
            {
                var imageResource = ImageResourceFactory.CreateImageResource(reader);
                ImageResources.Add(imageResource);
            }

            Util.DebugMessage(reader.BaseStream, "Load, End, ImageResources");

            //-----------------------------------------------------------------------
            // make sure we are not on a wrong offset, so set the stream position
            // manually
            reader.BaseStream.Position = startPosition + imageResourcesLength;
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region LayerAndMaskInfo

        public List<Layer> Layers { get; private set; }

        public List<LayerInfo> AdditionalInfo { get; private set; }

        public bool AbsoluteAlpha { get; set; }

        ///////////////////////////////////////////////////////////////////////////

        private void LoadLayerAndMaskInfo(PsdBinaryReader reader)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, Layer and mask info");

            var layersAndMaskLength = IsLargeDocument
                ? reader.ReadInt64()
                : reader.ReadUInt32();
            if (layersAndMaskLength <= 0)
                return;

            var startPosition = reader.BaseStream.Position;
            var endPosition = startPosition + layersAndMaskLength;

            LoadLayers(reader, true);
            LoadGlobalLayerMask(reader);

            //-----------------------------------------------------------------------
            // Load Additional Layer Information

            while (reader.BaseStream.Position < endPosition)
            {
                var info = LayerInfoFactory.Load(reader, this, true, endPosition);
                AdditionalInfo.Add(info);

                if (info is RawLayerInfo)
                {
                    var layerInfo = (RawLayerInfo)info;
                    switch (info.Key)
                    {
                        case "LMsk":
                            m_GlobalLayerMaskData = layerInfo.Data;
                            break;
                    }
                }
            }

            Util.DebugMessage(reader.BaseStream, "Load, End, Layer and mask info");

            //-----------------------------------------------------------------------
            // make sure we are not on a wrong offset, so set the stream position
            // manually
            reader.BaseStream.Position = startPosition + layersAndMaskLength;
        }

        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Load Layers Info section, including image data.
        /// </summary>
        /// <param name="reader">PSD reader.</param>
        /// <param name="hasHeader">Whether the Layers Info section has a length header.</param>
        internal void LoadLayers(PsdBinaryReader reader, bool hasHeader)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, Layers Info section");

            long sectionLength = 0;
            if (hasHeader)
            {
                sectionLength = IsLargeDocument
                    ? reader.ReadInt64()
                    : reader.ReadUInt32();

                if (sectionLength <= 0)
                {
                    // The callback may take action when there are 0 layers, so it must
                    // be called even though the Layers Info section is empty.
                    LoadContext.OnLoadLayersHeader(this);
                    Util.DebugMessage(reader.BaseStream, "Load, End, Layers Info section");
                    return;
                }
            }

            var startPosition = reader.BaseStream.Position;
            var numLayers = reader.ReadInt16();

            // If numLayers < 0, then number of layers is absolute value,
            // and the first alpha channel contains the transparency data for
            // the merged result.
            if (numLayers < 0)
            {
                AbsoluteAlpha = true;
                numLayers = Math.Abs(numLayers);
            }

            for (int i = 0; i < numLayers; i++)
            {
                var layer = new Layer(reader, this);
                Layers.Add(layer);
            }

            // Header is complete just before loading pixel data
            LoadContext.OnLoadLayersHeader(this);

            //-----------------------------------------------------------------------

            // Load image data for all channels.
            foreach (var layer in Layers)
            {
                Util.DebugMessage(reader.BaseStream,
                    "Load, Begin, Layer image, layer.Name");
                foreach (var channel in layer.Channels)
                {
                    channel.LoadPixelData(reader);
                }
                Util.DebugMessage(reader.BaseStream,
                    "Load, End, Layer image, layer.Name");
            }

            // Length is set to 0 when called on higher bitdepth layers.
            if (sectionLength > 0)
            {
                // Layers Info section is documented to be even-padded, but Photoshop
                // actually pads to 4 bytes.
                var endPosition = startPosition + sectionLength;
                var positionOffset = reader.BaseStream.Position - endPosition;
                Debug.Assert(positionOffset > -4,
                    "LoadLayers did not read the full length of the Layers Info section.");
                Debug.Assert(positionOffset <= 0,
                    "LoadLayers read past the end of the Layers Info section.");


                if (reader.BaseStream.Position < endPosition)
                    reader.BaseStream.Position = endPosition;
            }

            Util.DebugMessage(reader.BaseStream, "Load, End, Layers");
        }

        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Cleanup()
        {
            var layersAndComposite = Layers.Concat(new[] { BaseLayer });

            foreach (var lac in layersAndComposite)
            {
                foreach (var c in lac.Channels)
                {
                    c.Cleanup();
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Decompress the document image data and all the layers' image data, in parallel.
        /// </summary>
        private void DecompressImages()
        {
            var layersAndComposite = Layers.Concat(new[] { BaseLayer });
            //var channels = layersAndComposite.SelectMany(x => x.Channels);
            //Parallel.ForEach(channels, channel =>
            //{
            //  channel.DecodeImageData();
            //});
            foreach (var lac in layersAndComposite)
            {
                foreach (var c in lac.Channels)
                {
                    c.DecodeImageData();
                }
            }

            foreach (var layer in Layers)
            {
                foreach (var channel in layer.Channels)
                {
                    if (channel.ID == -2)
                        layer.Masks.LayerMask.ImageData = channel.ImageData;
                    else if (channel.ID == -3)
                        layer.Masks.UserMask.ImageData = channel.ImageData;
                }
            }
        }

        /// <summary>
        /// Verifies that any Additional Info layers are consistent.
        /// </summary>
        private void VerifyInfoLayers()
        {
            var infoLayersCount = AdditionalInfo.Count(x => x is InfoLayers);
            if (infoLayersCount > 1)
            {
                throw new PsdInvalidException(
                    "Cannot have more than one InfoLayers in a PSD file.");
            }
            if ((infoLayersCount > 0) && (Layers.Count == 0))
            {
                throw new PsdInvalidException(
                    "InfoLayers cannot exist when there are 0 layers.");
            }
        }

        /// <summary>
        /// Verify validity of layer sections.  Each start marker should have a
        /// matching end marker.
        /// </summary>
        internal void VerifyLayerSections()
        {
            int depth = 0;
            foreach (var layer in Enumerable.Reverse(Layers))
            {
                var layerSectionInfo = layer.AdditionalInfo.SingleOrDefault(
                        x => x is LayerSectionInfo);
                if (layerSectionInfo == null)
                    continue;

                var sectionInfo = (LayerSectionInfo)layerSectionInfo;
                switch (sectionInfo.SectionType)
                {
                    case LayerSectionType.OpenFolder:
                    case LayerSectionType.ClosedFolder:
                        depth++;
                        break;

                    case LayerSectionType.SectionDivider:
                        depth--;
                        if (depth < 0)
                            throw new PsdInvalidException("Layer section ended without matching start marker.");
                        break;
                    
                    case LayerSectionType.Layer: // Nothing to do here yet.
                        break;
                    
                    default:
                        throw new PsdInvalidException("Unrecognized layer section type.");
                }
            }

            if (depth != 0)
                throw new PsdInvalidException("Layer section not closed by end marker.");
        }

        /// <summary>
        /// Set the VersionInfo resource on the file.
        /// </summary>
        public void SetVersionInfo()
        {
            var versionInfo = (VersionInfo)ImageResources.Get(ResourceID.VersionInfo);
            if (versionInfo == null)
            {
                versionInfo = new VersionInfo();
                ImageResources.Set(versionInfo);

                // Get the version string.  We don't use the fourth part (revision).
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                var versionString = version.Major + "." + version.Minor + "." + version.Build;

                // Strings are not localized since they are not shown to the user.
                versionInfo.Version = 1;
                versionInfo.HasRealMergedData = true;
                versionInfo.ReaderName = "Paint.NET PSD Plugin";
                versionInfo.WriterName = "Paint.NET PSD Plugin " + versionString;
                versionInfo.FileVersion = 1;
            }
        }
        

        ///////////////////////////////////////////////////////////////////////////

        byte[] m_GlobalLayerMaskData;

        private void LoadGlobalLayerMask(PsdBinaryReader reader)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, GlobalLayerMask");

            var maskLength = reader.ReadUInt32();
            if (maskLength <= 0)
            {
                Util.DebugMessage(reader.BaseStream, "Load, End, GlobalLayerMask");
                return;
            }

            m_GlobalLayerMaskData = reader.ReadBytes((int)maskLength);

            Util.DebugMessage(reader.BaseStream, "Load, End, GlobalLayerMask");
        }

        public byte[] globalLayerMaskData
        {
            get { return m_GlobalLayerMaskData; }
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region Composite image

        /// <summary>
        /// Represents the composite image.
        /// </summary>
        public Layer BaseLayer { get; set; }

        public ImageCompression ImageCompression { get; set; }

        private void LoadImage(PsdBinaryReader reader)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, Composite image");

            ImageCompression = (ImageCompression)reader.ReadInt16();

            // Create channels
            for (Int16 i = 0; i < ChannelCount; i++)
            {
                Util.DebugMessage(reader.BaseStream, "Load, Begin, Channel image data");

                var channel = new Channel(i, this.BaseLayer);
                channel.ImageCompression = ImageCompression;
                channel.Length = this.RowCount
                    * Util.BytesPerRow(BaseLayer.Rect.Size, BitDepth);

                // The composite image stores all RLE headers up-front, rather than
                // with each channel.
                if (ImageCompression == ImageCompression.Rle)
                {
                    channel.RleRowLengths = new RleRowLengths(reader, RowCount, IsLargeDocument);
                    channel.Length = channel.RleRowLengths.Total;
                }

                BaseLayer.Channels.Add(channel);
                Util.DebugMessage(reader.BaseStream, "Load, End, Channel image data");
            }

            foreach (var channel in this.BaseLayer.Channels)
            {
                Util.DebugMessage(reader.BaseStream, "Load, Begin, Channel image data");
                Util.CheckByteArrayLength(channel.Length);
                channel.ImageDataRaw = reader.ReadBytes((int)channel.Length);
                Util.DebugMessage(reader.BaseStream, "Load, End, Channel image data");
            }

            // If there is exactly one more channel than we need, then it is the
            // alpha channel.
            if ((ColorMode != PsdColorMode.Multichannel)
                && (ChannelCount == ColorMode.MinChannelCount() + 1))
            {
                var alphaChannel = BaseLayer.Channels.Last();
                alphaChannel.ID = -1;
            }

            Util.DebugMessage(reader.BaseStream, "Load, End, Composite image");
        }

        #endregion
    }


    /// <summary>
    /// The possible Compression methods.
    /// </summary>
    internal enum ImageCompression
    {
        /// <summary>
        /// Raw data
        /// </summary>
        Raw = 0,
        /// <summary>
        /// RLE compressed
        /// </summary>
        Rle = 1,
        /// <summary>
        /// ZIP without prediction.
        /// </summary>
        Zip = 2,
        /// <summary>
        /// ZIP with prediction.
        /// </summary>
        ZipPrediction = 3
    }
}
