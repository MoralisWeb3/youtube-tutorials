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
using System.Threading;

using PhotoshopFile;
using UnityEngine;
using PDNWrapper;
using Unity.Collections;
using Unity.Jobs;

namespace PaintDotNet.Data.PhotoshopFileType
{
    internal static class PsdLoad
    {
        public static PsdFile Load(System.IO.Stream input, ELoadFlag loadFlag)
        {
            var loadContext = new DocumentLoadContext();
            return new PsdFile(input, loadContext, loadFlag);
        }
        
        public static Document Load(System.IO.Stream input)
        {
            // Load and decompress Photoshop file structures
            var loadContext = new DocumentLoadContext();
            var psdFile = new PsdFile(input, loadContext);

            // Multichannel images are loaded by processing each channel as a
            // grayscale layer.
            if (psdFile.ColorMode == PsdColorMode.Multichannel)
            {
                CreateLayersFromChannels(psdFile);
                psdFile.ColorMode = PsdColorMode.Grayscale;
            }

            // Convert into Paint.NET internal representation
            var document = new Document(psdFile.ColumnCount, psdFile.RowCount);

            if (psdFile.Layers.Count == 0)
            {
                psdFile.BaseLayer.CreateMissingChannels();
                var layer = PDNWrapper.Layer.CreateBackgroundLayer(psdFile.ColumnCount, psdFile.RowCount);
                ImageDecoderPdn.DecodeImage(layer, psdFile.BaseLayer);
                layer.Name = String.IsNullOrEmpty(psdFile.BaseLayer.Name)? "Background" : psdFile.BaseLayer.Name;
                layer.Opacity = psdFile.BaseLayer.Opacity;
                layer.Visible = psdFile.BaseLayer.Visible;
                layer.IsGroup = psdFile.BaseLayer.IsGroup;
                layer.LayerID = psdFile.BaseLayer.LayerID;
                layer.BlendMode = BlendModeMapping.FromPsdBlendMode(psdFile.BaseLayer.BlendModeKey);
                document.Layers.Add(layer);
            }
            else
            {
                psdFile.VerifyLayerSections();
                ApplyLayerSections(psdFile.Layers);

                //var pdnLayers = psdFile.Layers.AsParallel().AsOrdered()
                //  .Select(psdLayer => psdLayer.DecodeToPdnLayer())
                //  .ToList();
                //document.Layers.AddRange(pdnLayers);
                /*
                        foreach (var l in psdFile.Layers)
                {
                    document.Layers.Add(l.DecodeToPdnLayer());
                }
                */
                BitmapLayer parent = null;
                JobHandle jobHandle = default(JobHandle);
                foreach (var l in Enumerable.Reverse(psdFile.Layers))
                {
                    if (l.IsEndGroupMarker)
                    {
                        parent = parent != null ? parent.ParentLayer : null;
                        continue;
                    }
                    BitmapLayer b = null;
                    jobHandle = l.DecodeToPdnLayer(jobHandle, out b);
                    b.ParentLayer = parent;
                    if (parent != null)
                        parent.ChildLayer.Add(b);
                    else
                        document.Layers.Add(b);

                    if (b.IsGroup)
                        parent = b;
                }
                jobHandle.Complete();
            }
            SetPdnResolutionInfo(psdFile, document);
            psdFile.Cleanup();
            return document;
        }

        internal static JobHandle DecodeToPdnLayer(this PhotoshopFile.Layer psdLayer, JobHandle inputDeps, out BitmapLayer pdnLayer)
        {
            var psdFile = psdLayer.PsdFile;
            psdLayer.CreateMissingChannels();

            pdnLayer = new BitmapLayer(psdFile.ColumnCount, psdFile.RowCount);
            pdnLayer.Name = psdLayer.Name;
            pdnLayer.Opacity = psdLayer.Opacity;
            pdnLayer.Visible = psdLayer.Visible;
            pdnLayer.IsGroup = psdLayer.IsGroup;
            pdnLayer.LayerID = psdLayer.LayerID;
            pdnLayer.BlendMode = BlendModeMapping.FromPsdBlendMode(psdLayer.BlendModeKey);
            return ImageDecoderPdn.DecodeImage(pdnLayer, psdLayer, inputDeps);
        }

        /// <summary>
        /// Creates a layer for each channel in a multichannel image.
        /// </summary>
        private static void CreateLayersFromChannels(PsdFile psdFile)
        {
            if (psdFile.ColorMode != PsdColorMode.Multichannel)
                throw new Exception("Not a multichannel image.");
            if (psdFile.Layers.Count > 0)
                throw new PsdInvalidException("Multichannel image should not have layers.");

            // Get alpha channel names, preferably in Unicode.
            var alphaChannelNames = (AlphaChannelNames)psdFile.ImageResources
                .Get(ResourceID.AlphaChannelNames);
            var unicodeAlphaNames = (UnicodeAlphaNames)psdFile.ImageResources
                .Get(ResourceID.UnicodeAlphaNames);
            if ((alphaChannelNames == null) && (unicodeAlphaNames == null))
                throw new PsdInvalidException("No channel names found.");

            var channelNames = (unicodeAlphaNames != null)
                ? unicodeAlphaNames.ChannelNames
                : alphaChannelNames.ChannelNames;
            var channels = psdFile.BaseLayer.Channels;
            if (channels.Count > channelNames.Count)
                throw new PsdInvalidException("More channels than channel names.");

            // Channels are stored from top to bottom, but layers are stored from
            // bottom to top.
            for (int i = channels.Count - 1; i >= 0; i--)
            {
                var channel = channels[i];
                var channelName = channelNames[i];

                // Copy metadata over from base layer
                var layer = new PhotoshopFile.Layer(psdFile);
                layer.Rect = psdFile.BaseLayer.Rect;
                layer.Visible = true;
                layer.Masks = new MaskInfo();
                layer.BlendingRangesData = new BlendingRanges(layer);

                // We do not attempt to reconstruct the appearance of the image, but
                // only to provide access to the channels image data.
                layer.Name = channelName;
                layer.BlendModeKey = PsdBlendMode.Darken;
                layer.Opacity = 255;

                // Copy channel image data into the new grayscale layer
                var layerChannel = new Channel(0, layer);
                layerChannel.ImageCompression = channel.ImageCompression;
                layerChannel.ImageData = new NativeArray<byte>(channel.ImageData, Allocator.Persistent);
                layer.Channels.Add(layerChannel);

                psdFile.Layers.Add(layer);
            }
        }

        /// <summary>
        /// Transform Photoshop's layer tree to Paint.NET's flat layer list.
        /// Indicate where layer sections begin and end, and hide all layers within
        /// hidden layer sections.
        /// </summary>
        private static void ApplyLayerSections(List<PhotoshopFile.Layer> layers)
        {
            // BUG: PsdPluginResources.GetString will always return English resource,
            // because Paint.NET does not set the CurrentUICulture when OnLoad is
            // called.  This situation should be resolved with Paint.NET 4.0, which
            // will provide an alternative mechanism to retrieve the UI language.

            // Track the depth of the topmost hidden section.  Any nested sections
            // will be hidden, whether or not they themselves have the flag set.
            int topHiddenSectionDepth = Int32.MaxValue;
            var layerSectionNames = new Stack<string>();

            // Layers are stored bottom-to-top, but layer sections are specified
            // top-to-bottom.
            foreach (var layer in Enumerable.Reverse(layers))
            {
                // Leo: Since we are importing, we don't care if the group is collapsed
                // Apply to all layers within the layer section, as well as the
                // closing layer.
                //if (layerSectionNames.Count > topHiddenSectionDepth)
                //    layer.Visible = false;

                var sectionInfo = (LayerSectionInfo)layer.AdditionalInfo
                    .SingleOrDefault(x => x is LayerSectionInfo);
                if (sectionInfo == null)
                    continue;

                switch (sectionInfo.SectionType)
                {
                    case LayerSectionType.OpenFolder:
                    case LayerSectionType.ClosedFolder:
                        // Start a new layer section
                        if ((!layer.Visible) && (topHiddenSectionDepth == Int32.MaxValue))
                            topHiddenSectionDepth = layerSectionNames.Count;
                        layerSectionNames.Push(layer.Name);
                        layer.IsGroup = true;
                        //layer.Name = String.Format(beginSectionWrapper, layer.Name);
                        break;

                    case LayerSectionType.SectionDivider:
                        // End the current layer section
                        //var layerSectionName = layerSectionNames.Pop ();
                        if (layerSectionNames.Count == topHiddenSectionDepth)
                            topHiddenSectionDepth = Int32.MaxValue;
                        layer.IsEndGroupMarker = true;
                        //layer.Name = String.Format(endSectionWrapper, layerSectionName);
                        break;
                }
            }
        }

        /// <summary>
        /// Set the resolution on the Paint.NET Document to match the PSD file.
        /// </summary>
        private static void SetPdnResolutionInfo(PsdFile psdFile, Document document)
        {
            if (psdFile.Resolution != null)
            {
                // PSD files always specify the resolution in DPI.  When loading and
                // saving cm, we will have to round-trip the conversion, but doubles
                // have plenty of precision to spare vs. PSD's 16/16 fixed-point.

                if ((psdFile.Resolution.HResDisplayUnit == ResolutionInfo.ResUnit.PxPerCm)
                    && (psdFile.Resolution.VResDisplayUnit == ResolutionInfo.ResUnit.PxPerCm))
                {
                    document.DpuUnit = MeasurementUnit.Centimeter;

                    // HACK: Paint.NET truncates DpuX and DpuY to three decimal places,
                    // so add 0.0005 to get a rounded value instead.
                    document.DpuX = psdFile.Resolution.HDpi / 2.54 + 0.0005;
                    document.DpuY = psdFile.Resolution.VDpi / 2.54 + 0.0005;
                }
                else
                {
                    document.DpuUnit = MeasurementUnit.Inch;
                    document.DpuX = psdFile.Resolution.HDpi;
                    document.DpuY = psdFile.Resolution.VDpi;
                }
            }
        }

        /// <summary>
        /// Verify that the PSD file will fit into physical memory once loaded
        /// and converted to Paint.NET format.
        /// </summary>
        /// <remarks>
        /// This check is necessary because layers in Paint.NET have the same
        /// dimensions as the canvas.  Thus, PSD files that contain lots of
        /// tiny adjustment layers may blow up in size by several
        /// orders of magnitude.
        /// </remarks>
        internal static void CheckSufficientMemory(PsdFile psdFile)
        {
            /*
            Remove memory check since we can't properly ensure there will
            be enough memory to import
            
            // Multichannel images have channels converted to layers
            var numLayers = (psdFile.ColorMode == PsdColorMode.Multichannel)
                ? psdFile.BaseLayer.Channels.Count
                : Math.Max(psdFile.Layers.Count, 1);

            // Paint.NET also requires a scratch layer and composite layer
            numLayers += 2;

            long numPixels = (long)psdFile.ColumnCount * psdFile.RowCount;
            ulong bytesRequired = (ulong)(checked(4 * numPixels * numLayers));

            // Check that the file will fit entirely into physical memory, so that we
            // do not thrash and make the Paint.NET UI nonresponsive.  We also have
            // to check against virtual memory address space because 32-bit processes
            // cannot access all 4 GB.
            //var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
            //var accessibleMemory = Math.Min(computerInfo.TotalPhysicalMemory,
            //  computerInfo.TotalVirtualMemory);
            var accessibleMemory = (ulong)SystemInfo.systemMemorySize * 1024 * 1024;
            if (bytesRequired > accessibleMemory)
            {
                throw new OutOfMemoryException();
            }
            */
        }
    }
}
