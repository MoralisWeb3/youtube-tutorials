/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2014 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using PDNWrapper;
using System.Globalization;
using Unity.Collections;

namespace PhotoshopFile
{
    internal class Mask
    {
        /// <summary>
        /// The layer to which this mask belongs
        /// </summary>
        public Layer Layer { get; private set; }

        /// <summary>
        /// The rectangle enclosing the mask.
        /// </summary>
        public Rectangle Rect { get; set; }

        private byte backgroundColor;
        public byte BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                if ((value != 0) && (value != 255))
                    throw new PsdInvalidException("Mask background must be fully-opaque or fully-transparent.");
                backgroundColor = value;
            }
        }

        private static int positionVsLayerBit = BitVector32.CreateMask();
        private static int disabledBit = BitVector32.CreateMask(positionVsLayerBit);
        private static int invertOnBlendBit = BitVector32.CreateMask(disabledBit);

        private BitVector32 flags;
        public BitVector32 Flags { get { return flags; } }

        /// <summary>
        /// If true, the position of the mask is relative to the layer.
        /// </summary>
        public bool PositionVsLayer
        {
            get { return flags[positionVsLayerBit]; }
            set { flags[positionVsLayerBit] = value; }
        }

        public bool Disabled
        {
            get { return flags[disabledBit]; }
            set { flags[disabledBit] = value; }
        }

        /// <summary>
        /// if true, invert the mask when blending.
        /// </summary>
        public bool InvertOnBlend
        {
            get { return flags[invertOnBlendBit]; }
            set { flags[invertOnBlendBit] = value; }
        }

        /// <summary>
        /// Mask image data.
        /// </summary>
        public NativeArray<byte> ImageData { get; set; }

        public Mask(Layer layer)
        {
            Layer = layer;
            this.flags = new BitVector32();
        }

        public Mask(Layer layer, Rectangle rect, byte color, BitVector32 flags)
        {
            Layer = layer;
            Rect = rect;
            BackgroundColor = color;
            this.flags = flags;
        }
    }

    /// <summary>
    /// Mask info for a layer.  Contains both the layer and user masks.
    /// </summary>
    internal class MaskInfo
    {
        public Mask LayerMask { get; set; }

        public Mask UserMask { get; set; }

        /// <summary>
        /// Construct MaskInfo with null masks.
        /// </summary>
        public MaskInfo()
        {
        }

        public MaskInfo(PsdBinaryReader reader, Layer layer)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, MaskInfo");

            var maskLength = reader.ReadUInt32();
            if (maskLength <= 0)
                return;

            var startPosition = reader.BaseStream.Position;
            var endPosition = startPosition + maskLength;

            // Read layer mask
            var rectangle = reader.ReadRectangle();
            var backgroundColor = reader.ReadByte();
            var flagsByte = reader.ReadByte();
            LayerMask = new Mask(layer, rectangle, backgroundColor, new BitVector32(flagsByte));

            // User mask is supplied separately when there is also a vector mask.
            if (maskLength == 36)
            {
                var userFlagsByte = reader.ReadByte();
                var userBackgroundColor = reader.ReadByte();
                var userRectangle = reader.ReadRectangle();
                UserMask = new Mask(layer, userRectangle, userBackgroundColor, new BitVector32(userFlagsByte));
            }

            // 20-byte mask data will end with padding.
            reader.BaseStream.Position = endPosition;

            Util.DebugMessage(reader.BaseStream, "Load, End, MaskInfo");
        }
    }
}
