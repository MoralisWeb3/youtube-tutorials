/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2015 Tao Yue
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PhotoshopFile
{
    internal enum LayerSectionType
    {
        Layer = 0,
        OpenFolder = 1,
        ClosedFolder = 2,
        SectionDivider = 3
    }

    internal enum LayerSectionSubtype
    {
        Normal = 0,
        SceneGroup = 1
    }

    /// <summary>
    /// Layer sections are known as Groups in the Photoshop UI.
    /// </summary>
    internal class LayerSectionInfo : LayerInfo
    {
        public override string Signature
        {
            get { return "8BIM"; }
        }

        private string key;
        public override string Key
        {
            get { return key; }
        }

        public LayerSectionType SectionType { get; set; }

        private LayerSectionSubtype? subtype;
        public LayerSectionSubtype Subtype
        {
            get { return subtype ?? LayerSectionSubtype.Normal; }
            set { subtype = value; }
        }

        private string blendModeKey;
        public string BlendModeKey
        {
            get { return blendModeKey; }
            set
            {
                if (value.Length != 4)
                {
                    throw new ArgumentException(
                        "BlendModeKey must be 4 characters in length.");
                }
                blendModeKey = value;
            }
        }

        public LayerSectionInfo(PsdBinaryReader reader, string key, int dataLength)
        {
            // The key for layer section info is documented to be "lsct".  However,
            // some Photoshop files use the undocumented key "lsdk", with apparently
            // the same data format.
            this.key = key;

            SectionType = (LayerSectionType)reader.ReadInt32();
            if (dataLength >= 12)
            {
                var signature = reader.ReadAsciiChars(4);
                if (signature != "8BIM")
                    throw new PsdInvalidException("Invalid section divider signature.");

                BlendModeKey = reader.ReadAsciiChars(4);
                if (dataLength >= 16)
                {
                    Subtype = (LayerSectionSubtype)reader.ReadInt32();
                }
            }
        }
    }
}
