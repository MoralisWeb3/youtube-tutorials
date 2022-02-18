using System;
using PhotoshopFile;
using UnityEngine;
using UnityEngine.Analytics;

namespace UnityEditor.U2D.PSD
{
    [Serializable]
    internal struct PSDApplyEvent
    {
        public int instance_id;
        public int texture_type;
        public int sprite_mode;
        public bool mosaic_layer;
        public bool import_hidden_layer;
        public bool character_mode;
        public bool generate_go_hierarchy;
        public bool reslice_from_layer;
        public bool is_character_rigged;
        public SpriteAlignment character_alignment;
        public bool is_psd;
        public PsdColorMode color_mode;

    }

    internal interface IAnalytics
    {
        AnalyticsResult SendApplyEvent(PSDApplyEvent evt);
    }

    internal static class AnalyticFactory
    {
        static IAnalytics s_Analytics;
        static public IAnalytics analytics
        {
            get
            {
                if (s_Analytics == null)
                    s_Analytics = new Analytics();
                return s_Analytics;
            }
            set { s_Analytics = value; }
        }
    }

    [InitializeOnLoad]
    internal class Analytics : IAnalytics
    {
        const int k_MaxEventsPerHour = 100;
        const int k_MaxNumberOfElements = 1000;
        const string k_VendorKey = "unity.2d.psdimporter";
        const int k_Version = 1;

        static Analytics()
        {
            EditorAnalytics.RegisterEventWithLimit("psdImporterApply", k_MaxEventsPerHour, k_MaxNumberOfElements, k_VendorKey, k_Version);
        }

        public AnalyticsResult SendApplyEvent(PSDApplyEvent evt)
        {
            return EditorAnalytics.SendEventWithLimit("psdImporterApply", evt, k_Version);
        }
    }
}
