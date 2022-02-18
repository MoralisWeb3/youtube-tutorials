/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2012 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PhotoshopFile
{
    /// <summary>
    /// Summary description for ResolutionInfo.
    /// </summary>
    internal class ResolutionInfo : ImageResource
    {
        public override ResourceID ID
        {
            get { return ResourceID.ResolutionInfo; }
        }

        /// <summary>
        /// Horizontal DPI.
        /// </summary>
        public UFixed16_16 HDpi { get; set; }

        /// <summary>
        /// Vertical DPI.
        /// </summary>
        public UFixed16_16 VDpi { get; set; }

        /// <summary>
        /// 1 = pixels per inch, 2 = pixels per centimeter
        /// </summary>
        internal enum ResUnit
        {
            PxPerInch = 1,
            PxPerCm = 2
        }

        /// <summary>
        /// Display units for horizontal resolution.  This only affects the
        /// user interface; the resolution is still stored in the PSD file
        /// as pixels/inch.
        /// </summary>
        public ResUnit HResDisplayUnit { get; set; }

        /// <summary>
        /// Display units for vertical resolution.
        /// </summary>
        public ResUnit VResDisplayUnit { get; set; }

        /// <summary>
        /// Physical units.
        /// </summary>
        internal enum Unit
        {
            Inches = 1,
            Centimeters = 2,
            Points = 3,
            Picas = 4,
            Columns = 5
        }

        public Unit WidthDisplayUnit { get; set; }

        public Unit HeightDisplayUnit { get; set; }

        public ResolutionInfo() : base(String.Empty)
        {
        }

        public ResolutionInfo(PsdBinaryReader reader, string name)
            : base(name)
        {
            this.HDpi = new UFixed16_16(reader.ReadUInt32());
            this.HResDisplayUnit = (ResUnit)reader.ReadInt16();
            this.WidthDisplayUnit = (Unit)reader.ReadInt16();

            this.VDpi = new UFixed16_16(reader.ReadUInt32());
            this.VResDisplayUnit = (ResUnit)reader.ReadInt16();
            this.HeightDisplayUnit = (Unit)reader.ReadInt16();
        }
    }
}
