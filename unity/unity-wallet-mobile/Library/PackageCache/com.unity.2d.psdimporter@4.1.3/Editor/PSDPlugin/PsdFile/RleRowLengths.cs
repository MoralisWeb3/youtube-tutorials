/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2014 Tao Yue
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;

namespace PhotoshopFile
{
    internal class RleRowLengths
    {
        public int[] Values { get; private set; }

        public long Total
        {
            get { return Values.Sum(x => (long)x); }
        }

        public int this[int i]
        {
            get { return Values[i]; }
            set { Values[i] = value; }
        }

        public RleRowLengths(int rowCount)
        {
            Values = new int[rowCount];
        }

        public RleRowLengths(PsdBinaryReader reader, int rowCount, bool isLargeDocument)
            : this(rowCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                Values[i] = isLargeDocument
                    ? reader.ReadInt32()
                    : reader.ReadUInt16();
            }
        }
    }
}
