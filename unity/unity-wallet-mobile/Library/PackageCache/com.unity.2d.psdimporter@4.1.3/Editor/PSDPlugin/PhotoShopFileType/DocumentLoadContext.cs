/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2016 Tao Yue
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhotoshopFile;

namespace PaintDotNet.Data.PhotoshopFileType
{
    /// <summary>
    /// Controls the loading of a PSD file into a Paint.NET Document.
    /// </summary>
    internal class DocumentLoadContext : LoadContext
    {
        public DocumentLoadContext() : base()
        {
        }

        public override void OnLoadLayersHeader(PsdFile psdFile)
        {
            PsdLoad.CheckSufficientMemory(psdFile);
        }

        public override void OnLoadLayerHeader(PhotoshopFile.Layer layer)
        {
            var psdFile = layer.PsdFile;
            if (psdFile.ColorMode == PsdColorMode.Multichannel)
            {
                PsdLoad.CheckSufficientMemory(psdFile);
            }
        }
    }
}
