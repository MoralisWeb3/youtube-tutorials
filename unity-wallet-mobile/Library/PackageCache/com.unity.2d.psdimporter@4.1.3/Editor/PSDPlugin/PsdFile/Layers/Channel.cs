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
using System.Linq;
using Unity.Collections;
using PhotoshopFile.Compression;

namespace PhotoshopFile
{
    internal class ChannelList : List<Channel>
    {
        /// <summary>
        /// Returns channels with nonnegative IDs as an array, so that accessing
        /// a channel by Id can be optimized into pointer arithmetic rather than
        /// being implemented as a List scan.
        /// </summary>
        /// <remarks>
        /// This optimization is crucial for blitting lots of pixels back and
        /// forth between Photoshop's per-channel representation, and Paint.NET's
        /// per-pixel BGRA representation.
        /// </remarks>
        public Channel[] ToIdArray()
        {
            var maxId = this.Max(x => x.ID);
            var idArray = new Channel[maxId + 1];
            foreach (var channel in this)
            {
                if (channel.ID >= 0)
                    idArray[channel.ID] = channel;
            }
            return idArray;
        }

        public ChannelList()
            : base()
        {
        }

        public Channel GetId(int id)
        {
            return this.Single(x => x.ID == id);
        }

        public bool ContainsId(int id)
        {
            return this.Exists(x => x.ID == id);
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    [DebuggerDisplay("ID = {ID}")]
    internal class Channel
    {
        /// <summary>
        /// The layer to which this channel belongs
        /// </summary>
        public Layer Layer { get; private set; }

        /// <summary>
        /// Channel ID.
        /// <list type="bullet">
        /// <item>-1 = transparency mask</item>
        /// <item>-2 = user-supplied layer mask, or vector mask</item>
        /// <item>-3 = user-supplied layer mask, if channel -2 contains a vector mask</item>
        /// <item>
        /// Nonnegative channel IDs give the actual image channels, in the
        /// order defined by the colormode.  For example, 0, 1, 2 = R, G, B.
        /// </item>
        /// </list>
        /// </summary>
        public short ID { get; set; }

        public Rectangle Rect
        {
            get
            {
                switch (ID)
                {
                    case -2:
                        return Layer.Masks.LayerMask.Rect;
                    case -3:
                        return Layer.Masks.UserMask.Rect;
                    default:
                        return Layer.Rect;
                }
            }
        }

        /// <summary>
        /// Total length of the channel data, including compression headers.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Raw image data for this color channel, in compressed on-disk format.
        /// </summary>
        /// <remarks>
        /// If null, the ImageData will be automatically compressed during save.
        /// </remarks>
        public byte[] ImageDataRaw { get; set; }

        /// <summary>
        /// Decompressed image data for this color channel.
        /// </summary>
        /// <remarks>
        /// When making changes to the ImageData, set ImageDataRaw to null so that
        /// the correct data will be compressed during save.
        /// </remarks>
        public NativeArray<byte> ImageData { get; set; }

        /// <summary>
        /// Image compression method used.
        /// </summary>
        public ImageCompression ImageCompression { get; set; }

        /// <summary>
        /// RLE-compressed length of each row.
        /// </summary>
        public RleRowLengths RleRowLengths { get; set; }

        //////////////////////////////////////////////////////////////////

        internal Channel(short id, Layer layer)
        {
            ID = id;
            Layer = layer;
        }

        internal Channel(PsdBinaryReader reader, Layer layer)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, Channel");

            ID = reader.ReadInt16();
            Length = (layer.PsdFile.IsLargeDocument)
                ? reader.ReadInt64()
                : reader.ReadInt32();
            Layer = layer;

            Util.DebugMessage(reader.BaseStream, "Load, End, Channel, {0}", ID);
        }
        
        internal void Cleanup()
        {
            if (ImageData.IsCreated)
                ImageData.Dispose();
        }
        //////////////////////////////////////////////////////////////////

        internal void LoadPixelData(PsdBinaryReader reader)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, Channel image");

            if (Length == 0)
            {
                ImageCompression = ImageCompression.Raw;
                ImageDataRaw = new byte[0];
                return;
            }

            var endPosition = reader.BaseStream.Position + this.Length;
            ImageCompression = (ImageCompression)reader.ReadInt16();
            var longDataLength = this.Length - 2;
            Util.CheckByteArrayLength(longDataLength);
            var dataLength = (int)longDataLength;

            switch (ImageCompression)
            {
                case ImageCompression.Raw:
                    ImageDataRaw = reader.ReadBytes(dataLength);
                    break;
                case ImageCompression.Rle:
                    // RLE row lengths
                    RleRowLengths = new RleRowLengths(reader, Rect.Height, Layer.PsdFile.IsLargeDocument);
                    var rleDataLength = (int)(endPosition - reader.BaseStream.Position);
                    Debug.Assert(rleDataLength == RleRowLengths.Total,
                    "RLE row lengths do not sum to length of channel image data.");

                    // The PSD specification states that rows are padded to even sizes.
                    // However, Photoshop doesn't actually do this.  RLE rows can have
                    // odd lengths in the header, and there is no padding between rows.
                    ImageDataRaw = reader.ReadBytes(rleDataLength);
                    break;
                case ImageCompression.Zip:
                case ImageCompression.ZipPrediction:
                    ImageDataRaw = reader.ReadBytes(dataLength);
                    break;
            }

            Util.DebugMessage(reader.BaseStream, "Load, End, Channel image, {0}", ID, Layer.Name);
            Debug.Assert(reader.BaseStream.Position == endPosition, "Pixel data was not fully read in.");
        }

        /// <summary>
        /// Decodes the raw image data from the compressed on-disk format into
        /// an uncompressed bitmap, in native byte order.
        /// </summary>
        public void DecodeImageData()
        {
            if ((ImageCompression == ImageCompression.Raw) && (Layer.PsdFile.BitDepth <= 8))
            {
                ImageData = new NativeArray<byte>(ImageDataRaw, Allocator.TempJob);
                return;
            }

            var image = ImageDataFactory.Create(this, ImageDataRaw);
            var longLength = (long)image.BytesPerRow * Rect.Height;
            Util.CheckByteArrayLength(longLength);
            var LocalImageData = new byte[longLength];
            image.Read(LocalImageData);
            ImageData = new NativeArray<byte>(LocalImageData, Allocator.TempJob);
            ImageDataRaw = null; // no longer needed.
        }
    }
}
