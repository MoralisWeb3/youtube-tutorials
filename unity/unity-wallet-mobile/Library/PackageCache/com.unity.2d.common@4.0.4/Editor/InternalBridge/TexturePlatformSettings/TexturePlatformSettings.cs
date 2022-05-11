using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System;
using UnityEditor.Modules;

namespace UnityEditor.U2D.Common
{

    internal interface ITexturePlatformSettingsDataProvider
    {
        bool textureTypeHasMultipleDifferentValues { get; }
        TextureImporterType textureType { get; }
        SpriteImportMode spriteImportMode { get; }

        int GetTargetCount();
        TextureImporterPlatformSettings GetPlatformTextureSettings(int i, string name);
        bool ShowPresetSettings();
        bool DoesSourceTextureHaveAlpha(int v);
        bool IsSourceTextureHDR(int v);
        void SetPlatformTextureSettings(int i, TextureImporterPlatformSettings platformSettings);
        void GetImporterSettings(int i, UnityEditor.TextureImporterSettings settings);
    }

    internal class TexturePlatformSettings : BaseTextureImportPlatformSettings
    {
        [SerializeField]
        TextureImportPlatformSettingsData m_Data = new TextureImportPlatformSettingsData();
        ITexturePlatformSettingsDataProvider m_DataProvider;
        Func<BaseTextureImportPlatformSettings> DefaultImportSettings;

        public override TextureImportPlatformSettingsData model
        {
            get => m_Data;
        }
        
        public TexturePlatformSettings(string name, BuildTarget target, ITexturePlatformSettingsDataProvider inspector, Func<BaseTextureImportPlatformSettings> defaultPlatform) : base(name, target)
        {
            m_DataProvider = inspector;
            DefaultImportSettings = defaultPlatform;
            Init();
        }


        public override bool textureTypeHasMultipleDifferentValues
        {
            get { return m_DataProvider.textureTypeHasMultipleDifferentValues; }
        }

        public override TextureImporterType textureType
        {
            get { return m_DataProvider.textureType; }
        }

        public override SpriteImportMode spriteImportMode
        {
            get { return m_DataProvider.spriteImportMode; }
        }

        public override int GetTargetCount()
        {
            return m_DataProvider.GetTargetCount();
        }

        public override bool ShowPresetSettings()
        {
            return m_DataProvider.ShowPresetSettings();
        }

        public override TextureImporterSettings GetImporterSettings(int i)
        {
            var textureImporterSettings = new TextureImporterSettings();
            m_DataProvider.GetImporterSettings(i, textureImporterSettings);
            return textureImporterSettings;
        }

        public override bool IsSourceTextureHDR(int i)
        {
            return m_DataProvider.IsSourceTextureHDR(i);
        }

        public override bool DoesSourceTextureHaveAlpha(int i)
        {
            return m_DataProvider.DoesSourceTextureHaveAlpha(i);
        }

        public override TextureImporterPlatformSettings GetPlatformTextureSettings(int i, string name)
        {
            return m_DataProvider.GetPlatformTextureSettings(i, name);
        }

        public override BaseTextureImportPlatformSettings GetDefaultImportSettings()
        {
            return DefaultImportSettings();
        }

        public override void SetPlatformTextureSettings(int i, TextureImporterPlatformSettings platformSettings)
        {
            platformSettings.name = GetFixedPlatformName(platformSettings.name);
            m_DataProvider.SetPlatformTextureSettings(i, platformSettings);
        }

        private string GetFixedPlatformName(string platform)
        {
            var targetGroup = BuildPipeline.GetBuildTargetGroupByName(platform);
            if (targetGroup != BuildTargetGroup.Unknown)
                return BuildPipeline.GetBuildTargetGroupName(targetGroup);
            return platform;
        }
    }

    internal class TexturePlatformSettingsHelper
    {
        [SerializeField]
        List<TexturePlatformSettings> m_PlatformSettings;
        ITexturePlatformSettingsDataProvider m_DataProvider;

        public TexturePlatformSettingsHelper(ITexturePlatformSettingsDataProvider dataProvider)
        {
            m_DataProvider = dataProvider;
            BuildPlatform[] validPlatforms = BaseTextureImportPlatformSettings.GetBuildPlayerValidPlatforms();

            m_PlatformSettings = new List<TexturePlatformSettings>();
            m_PlatformSettings.Add(new TexturePlatformSettings(TextureImporterInspector.s_DefaultPlatformName, BuildTarget.StandaloneWindows, dataProvider, DefaulTextureImportPlatformSettings));

            foreach (BuildPlatform bp in validPlatforms)
                m_PlatformSettings.Add(new TexturePlatformSettings(bp.name, bp.defaultTarget, dataProvider, DefaulTextureImportPlatformSettings));
        }

        BaseTextureImportPlatformSettings DefaulTextureImportPlatformSettings()
        {
            return m_PlatformSettings[0];
        }

        public static string defaultPlatformName { get => TextureImporterInspector.s_DefaultPlatformName; }

        public SpriteImportMode spriteImportMode
        {
            get { return m_DataProvider.spriteImportMode; }
        }

        public TextureImporterType textureType
        {
            get { return m_DataProvider.textureType; }
        }

        public bool textureTypeHasMultipleDifferentValues
        {
            get { return m_DataProvider.textureTypeHasMultipleDifferentValues; }
        }

        public void ShowPlatformSpecificSettings()
        {
            BaseTextureImportPlatformSettings.ShowPlatformSpecificSettings(m_PlatformSettings.ConvertAll<BaseTextureImportPlatformSettings>(x => x as BaseTextureImportPlatformSettings));
            SyncPlatformSettings();
        }

        public bool HasModified()
        {
            foreach (var ps in m_PlatformSettings)
            {
                if (ps.model.HasChanged())
                    return true;
            }
            return false;
        }

        void SyncPlatformSettings()
        {
            foreach (var ps in m_PlatformSettings)
                ps.Sync();
        }

        public void Apply()
        {
            foreach (var ps in m_PlatformSettings)
                ps.Apply();
        }

        public static string GetBuildTargetGroupName(BuildTarget target)
        {
            var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
            foreach (var bp in BuildPlatforms.instance.buildPlatforms)
            {
                if (bp.targetGroup == targetGroup)
                    return bp.name;
            }
            return TextureImporter.defaultPlatformName;
        }
    }
}
