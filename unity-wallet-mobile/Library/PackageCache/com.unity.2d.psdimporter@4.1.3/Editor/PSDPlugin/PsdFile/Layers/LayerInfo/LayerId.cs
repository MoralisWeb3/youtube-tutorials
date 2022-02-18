/////////////////////////////////////////////////////////////////////////////////
// Author : leoyaik@unity3d.com
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PhotoshopFile
{
    /// <summary>
    /// Layers that are stored as Additional Info, rather than in the main
    /// Layers section of the PSD file.
    /// </summary>
    /// <remarks>
    /// Photoshop stores layers in the Additional Info section for 16-bit and
    /// 32-bit depth images.  The Layers section in the PSD file is left empty.
    ///
    /// This appears to be for backward-compatibility purposes, but it is not
    /// required.  Photoshop will successfully load a high-bitdepth image that
    /// puts the layers in the Layers section.
    /// </remarks>
    internal class LayerId : LayerInfo
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

        private int id = 0;
        public int ID
        {
            get { return id; }
        }


        public LayerId(string key)
        {
            switch (key)
            {
                // The key does not have to match the bit depth, but it does have to
                // be one of the known values.
                case "lyid":
                case "lnsr":
                    this.key = key;
                    break;
                default:
                    throw new PsdInvalidException(
                        "LayerId key should be lyid or lnsr");
            }
        }

        public LayerId(PsdBinaryReader reader,
                       string key, long dataLength)
            : this(key)
        {
            if (dataLength == 4)
                id = reader.ReadInt32();
            else
                throw new PsdInvalidException("LayerId data length should be 4");
        }
    }
}
