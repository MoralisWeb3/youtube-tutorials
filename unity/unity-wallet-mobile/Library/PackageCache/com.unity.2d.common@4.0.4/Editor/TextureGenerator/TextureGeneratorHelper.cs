using System;
using Unity.Collections;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace UnityEditor.U2D.Common
{
    internal interface ITextureSettings
    {
        void FillTextureGenerationSettings(ref TextureGenerationSettings settings);
    }


    [Serializable]
    internal class TextureSettings : ITextureSettings
    {
        [SerializeField]
        bool m_ColorTexture;
        [SerializeField]
        bool m_Readable;
        [SerializeField]
        TextureImporterNPOTScale m_NPOTScale;
        [SerializeField]
        FilterMode m_FilterMode;
        [SerializeField]
        int m_Aniso;
        [SerializeField]
        bool m_EnablePostProcessor;
        [SerializeField]
        SecondarySpriteTexture[] m_SecondaryTextures;

        public TextureSettings()
        {
            colorTexture = true;
            readable = false;
            npotScale = TextureImporterNPOTScale.None;
            filterMode = FilterMode.Bilinear;
            aniso = 1;
        }

        public TextureSettings(string assetPath, bool enablePostProcessor, bool colorTexture, bool readable, TextureImporterNPOTScale npotScale, FilterMode filterMode, int aniso, bool sourceContainsAlpha, bool sourceWasHDR)
        {
            this.assetPath = assetPath;
            this.enablePostProcessor = enablePostProcessor;
            this.colorTexture = colorTexture;
            this.readable = readable;
            this.npotScale = npotScale;
            this.filterMode = filterMode;
            this.aniso = aniso;
            this.containsAlpha = sourceContainsAlpha;
            this.hdr = sourceWasHDR;
        }

        public bool colorTexture { get { return m_ColorTexture; } set { m_ColorTexture = value; } } //sRGBTexture
        public bool readable { get { return m_Readable; } set { m_Readable = value; } }
        public TextureImporterNPOTScale npotScale { get { return m_NPOTScale; } set { m_NPOTScale = value; } }
        public FilterMode filterMode { get { return m_FilterMode; } set { m_FilterMode = value; } }
        public int aniso
        {
            get { return m_Aniso; }
            set { m_Aniso = value; }
        }
        public bool enablePostProcessor
        {
            get { return m_EnablePostProcessor; }
            set { m_EnablePostProcessor = value; }
        }

        public string assetPath { get; set; }
        public bool containsAlpha { get; set; }
        public bool hdr { get; set; }

        public SecondarySpriteTexture[] secondaryTextures { get { return m_SecondaryTextures;} set { m_SecondaryTextures = value; } }

        void ITextureSettings.FillTextureGenerationSettings(ref TextureGenerationSettings settings)
        {
            settings.textureImporterSettings.sRGBTexture = colorTexture;
            settings.textureImporterSettings.readable = readable;
            settings.textureImporterSettings.npotScale = npotScale;
            settings.textureImporterSettings.filterMode = filterMode;
            settings.textureImporterSettings.aniso = aniso;
            settings.assetPath = assetPath;
            settings.enablePostProcessor = enablePostProcessor;
            settings.sourceTextureInformation.containsAlpha = containsAlpha;
            settings.sourceTextureInformation.hdr = hdr;
            settings.secondarySpriteTextures = secondaryTextures;
        }
    }

    [Serializable]
    internal class TextureSpriteSettings : ITextureSettings
    {
        [SerializeField]
        string m_PackingTag;
        public string packingTag
        {
            get { return m_PackingTag; }
            set { m_PackingTag = value; }
        }

        [SerializeField]
        float m_PixelsPerUnit;
        public float pixelsPerUnit
        {
            get { return m_PixelsPerUnit; }
            set { m_PixelsPerUnit = value; }
        }

        [SerializeField]
        SpriteMeshType m_MeshType;
        public SpriteMeshType meshType
        {
            get { return m_MeshType; }
            set { m_MeshType = value; }
        }

        [SerializeField]
        uint m_ExtrudeEdges;
        public uint extrudeEdges
        {
            get { return m_ExtrudeEdges; }
            set { m_ExtrudeEdges = value; }
        }

        public bool qualifyForPacking { get; set; }
        public SpriteImportData[] spriteSheetData { get; set; }

        public TextureSpriteSettings()
        {
            packingTag = "";
            pixelsPerUnit = 100;
            meshType = SpriteMeshType.Tight;
            extrudeEdges = 1;
        }

        public TextureSpriteSettings(string packingTag, int pixelsPerUnit, SpriteMeshType meshType, uint extrudeEdges, bool qualifyForPacking, SpriteImportData[] spriteSheetData = null)
        {
            this.packingTag = packingTag;
            this.pixelsPerUnit = pixelsPerUnit;
            this.meshType = meshType;
            this.extrudeEdges = extrudeEdges;
            this.qualifyForPacking = qualifyForPacking;
            this.spriteSheetData = spriteSheetData;
        }

        void ITextureSettings.FillTextureGenerationSettings(ref TextureGenerationSettings settings)
        {
            settings.textureImporterSettings.spritePixelsPerUnit = pixelsPerUnit;
            settings.textureImporterSettings.spriteMeshType = meshType;
            settings.textureImporterSettings.spriteExtrude = extrudeEdges;
            settings.spritePackingTag = packingTag;
            settings.qualifyForSpritePacking = qualifyForPacking;
            settings.spriteImportData = spriteSheetData;
        }
    }

    [Serializable]
    internal class TextureWrapSettings : ITextureSettings
    {
        [SerializeField]
        TextureWrapMode m_WrapMode;
        [SerializeField]
        TextureWrapMode m_WrapModeU;
        [SerializeField]
        TextureWrapMode m_WrapModeV;
        [SerializeField]
        TextureWrapMode m_WrapModeW;

        public TextureWrapSettings()
        {
            wrapMode = wrapModeU = wrapModeV = wrapModeW = TextureWrapMode.Repeat;
        }

        public TextureWrapSettings(TextureWrapMode wrapMpde, TextureWrapMode wrapModeU, TextureWrapMode wrapModeV, TextureWrapMode wrapModeW)
        {
            this.wrapMode = wrapMode;
            this.wrapModeU = wrapModeU;
            this.wrapModeV = wrapModeV;
            this.wrapModeW = wrapModeW;
        }

        public TextureWrapMode wrapMode { get { return m_WrapMode; } set { m_WrapMode = value; } }
        public TextureWrapMode wrapModeU { get { return m_WrapModeU; } set { m_WrapModeU = value; } }
        public TextureWrapMode wrapModeV { get { return m_WrapModeV; } set { m_WrapModeV = value; } }
        public TextureWrapMode wrapModeW { get { return m_WrapModeW; } set { m_WrapModeW = value; } }

        void ITextureSettings.FillTextureGenerationSettings(ref TextureGenerationSettings settings)
        {
            settings.textureImporterSettings.wrapMode = wrapMode;
            settings.textureImporterSettings.wrapModeU = wrapModeU;
            settings.textureImporterSettings.wrapModeV = wrapModeV;
            settings.textureImporterSettings.wrapModeW = wrapModeW;
        }
    }

    [Serializable]
    internal class TextureAlphaSettings : ITextureSettings
    {
        [SerializeField]
        float m_AlphaTolerance;
        public float alphaTolerance
        {
            get { return m_AlphaTolerance; }
            set { m_AlphaTolerance = value; }
        }

        [SerializeField]
        TextureImporterAlphaSource m_AlphaSource;
        public TextureImporterAlphaSource alphaSource
        {
            get { return m_AlphaSource; }
            set { m_AlphaSource = value; }
        }

        public TextureAlphaSettings()
        {
            alphaTolerance = 0.5f;
            alphaSource = TextureImporterAlphaSource.FromInput;
        }

        public TextureAlphaSettings(TextureImporterAlphaSource alphaSource, float alphaTolerance)
        {
            this.alphaTolerance = alphaTolerance;
            this.alphaSource = alphaSource;
        }

        void ITextureSettings.FillTextureGenerationSettings(ref TextureGenerationSettings settings)
        {
            settings.textureImporterSettings.alphaIsTransparency = alphaSource != TextureImporterAlphaSource.None;
            settings.textureImporterSettings.alphaSource = alphaSource;
            settings.textureImporterSettings.alphaTestReferenceValue = alphaTolerance;
        }
    }

    [Serializable]
    internal class TextureMipmapSettings : ITextureSettings
    {
        [SerializeField]
        TextureImporterMipFilter m_Filter;
        public TextureImporterMipFilter filter
        {
            get { return m_Filter; }
            set { m_Filter = value; }
        }

        [SerializeField]
        bool m_BorderMipmap;
        public bool borderMipmap
        {
            get { return m_BorderMipmap; }
            set { m_BorderMipmap = value; }
        }

        [SerializeField]
        bool m_Fadeout;
        public bool fadeout
        {
            get { return m_Fadeout; }
            set { m_Fadeout = value; }
        }

        [SerializeField]
        bool m_PreserveCoverage;
        public bool preserveCoverage
        {
            get { return m_PreserveCoverage; }
            set { m_PreserveCoverage = value; }
        }

        [SerializeField]
        int m_FadeDistanceStart;
        public int fadeDistanceStart
        {
            get { return m_FadeDistanceStart; }
            set { m_FadeDistanceStart = value; }
        }

        [SerializeField]
        int m_FadeDistanceEnd;
        public int fadeDistanceEnd
        {
            get { return m_FadeDistanceEnd; }
            set { m_FadeDistanceEnd = value; }
        }

        public TextureMipmapSettings()
        {
            filter = TextureImporterMipFilter.BoxFilter;
            borderMipmap = false;
            fadeout = false;
            preserveCoverage = false;
            fadeDistanceStart = 1;
            fadeDistanceEnd = 3;
        }

        public TextureMipmapSettings(TextureImporterMipFilter filter, bool borderMipmap, bool fadeout, bool preserveCoverage, int fadeDistanceStart, int fadeDistanceEnd)
        {
            this.filter = filter;
            this.borderMipmap = borderMipmap;
            this.fadeout = fadeout;
            this.preserveCoverage = preserveCoverage;
            this.fadeDistanceStart = fadeDistanceStart;
            this.fadeDistanceEnd = fadeDistanceEnd;
        }

        void ITextureSettings.FillTextureGenerationSettings(ref TextureGenerationSettings settings)
        {
            settings.textureImporterSettings.mipmapEnabled = true;
            settings.textureImporterSettings.mipmapFilter = filter;
            settings.textureImporterSettings.borderMipmap = borderMipmap;
            settings.textureImporterSettings.fadeOut = fadeout;
            settings.textureImporterSettings.mipmapFadeDistanceStart = fadeDistanceStart;
            settings.textureImporterSettings.mipmapFadeDistanceEnd = fadeDistanceEnd;
            settings.textureImporterSettings.mipMapsPreserveCoverage = preserveCoverage;
        }
    }

    [Serializable]
    internal class TextureNormalSettings : ITextureSettings
    {
        [SerializeField]
        TextureImporterNormalFilter m_Filter;
        public TextureImporterNormalFilter filter
        {
            get { return m_Filter; }
            set { m_Filter = value; }
        }

        [SerializeField]
        bool m_GenerateFromGrayScale;
        public bool generateFromGrayScale
        {
            get { return m_GenerateFromGrayScale; }
            set { m_GenerateFromGrayScale = value; }
        }

        [SerializeField]
        float m_Bumpiness;
        public float bumpiness
        {
            get { return m_Bumpiness; }
            set { m_Bumpiness = value; }
        }

        public TextureNormalSettings()
        {
            filter = TextureImporterNormalFilter.Standard;
            generateFromGrayScale = false;
            bumpiness = 0.25f;
        }

        public TextureNormalSettings(TextureImporterNormalFilter filter, bool generateFromGrayScale, float bumpiness)
        {
            this.filter = filter;
            this.generateFromGrayScale = generateFromGrayScale;
            this.bumpiness = bumpiness;
        }

        void ITextureSettings.FillTextureGenerationSettings(ref TextureGenerationSettings settings)
        {
            settings.textureImporterSettings.normalMapFilter = filter;
            settings.textureImporterSettings.convertToNormalMap = generateFromGrayScale;
            settings.textureImporterSettings.heightmapScale = bumpiness;
        }
    }

    // If this is provided, textureType will be cubemap
    [Serializable]
    internal class TextureCubemapSettings : ITextureSettings
    {
        [SerializeField]
        TextureImporterCubemapConvolution m_Convolution;
        public TextureImporterCubemapConvolution convolution
        {
            get { return m_Convolution; }
            set { m_Convolution = value; }
        }

        [SerializeField]
        TextureImporterGenerateCubemap m_Mode;
        public TextureImporterGenerateCubemap mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        [SerializeField]
        bool m_Seamless;
        public bool seamless
        {
            get { return m_Seamless; }
            set { m_Seamless = value; }
        }
        public TextureCubemapSettings()
        {
            convolution = TextureImporterCubemapConvolution.None;
            mode = TextureImporterGenerateCubemap.AutoCubemap;
            seamless = false;
        }

        public TextureCubemapSettings(TextureImporterCubemapConvolution convolution, TextureImporterGenerateCubemap mode, bool seamless)
        {
            this.convolution = convolution;
            this.mode = mode;
            this.seamless = seamless;
        }

        void ITextureSettings.FillTextureGenerationSettings(ref TextureGenerationSettings settings)
        {
            settings.textureImporterSettings.textureShape = TextureImporterShape.TextureCube;
            settings.textureImporterSettings.cubemapConvolution = convolution;
            settings.textureImporterSettings.generateCubemap = mode;
            settings.textureImporterSettings.seamlessCubemap = seamless;
        }
    }

    internal static class TextureGeneratorHelper
    {
        public static TextureGenerationOutput GenerateTextureSprite(NativeArray<Color32> buffer, int bufferWidth, int bufferHeight, TextureSettings settings, TextureImporterPlatformSettings platformSettings,
            TextureSpriteSettings spriteSettings, TextureAlphaSettings alphaSettings = null, TextureMipmapSettings mipmapSettings = null, TextureWrapSettings wrapSettings = null)
        {
            if (alphaSettings == null)
                alphaSettings = new TextureAlphaSettings(TextureImporterAlphaSource.FromInput, 0.5f);
            if (wrapSettings == null)
                wrapSettings = new TextureWrapSettings(TextureWrapMode.Clamp, TextureWrapMode.Clamp, TextureWrapMode.Clamp, TextureWrapMode.Clamp);

            return GenerateTexture(buffer, bufferWidth, bufferHeight, TextureImporterType.Sprite, platformSettings, settings, spriteSettings, alphaSettings, mipmapSettings, wrapSettings);
        }

        public static TextureGenerationOutput GenerateLightmap(NativeArray<Color32> buffer, int bufferWidth, int bufferHeight, TextureSettings settings, TextureImporterPlatformSettings platformSettings,
            TextureMipmapSettings mipmapSettings = null, TextureWrapSettings wrapSettings = null)
        {
            settings.colorTexture = true;
            return GenerateTexture(buffer, bufferWidth, bufferHeight, TextureImporterType.Lightmap, platformSettings, settings, mipmapSettings, wrapSettings);
        }

        public static TextureGenerationOutput GenerateCookie(NativeArray<Color32> buffer, int bufferWidth, int bufferHeight, TextureSettings settings, TextureImporterPlatformSettings platformSettings,
            TextureAlphaSettings alphaSettings = null, TextureMipmapSettings mipmapSettings = null, TextureCubemapSettings cubemapSettings = null, TextureWrapSettings wrapSettings = null)
        {
            return GenerateTexture(buffer, bufferWidth, bufferHeight, TextureImporterType.Cookie, platformSettings, settings, alphaSettings, mipmapSettings, cubemapSettings, wrapSettings);
        }

        public static TextureGenerationOutput GenerateNormalMap(NativeArray<Color32> buffer, int bufferWidth, int bufferHeight, TextureSettings settings, TextureImporterPlatformSettings platformSettings,
            TextureNormalSettings normalSettings, TextureMipmapSettings mipmapSettings = null, TextureCubemapSettings cubemapSettings = null, TextureWrapSettings wrapSettings = null)
        {
            settings.colorTexture = false;
            return GenerateTexture(buffer, bufferWidth, bufferHeight, TextureImporterType.NormalMap, platformSettings, settings, normalSettings, mipmapSettings, cubemapSettings, wrapSettings);
        }

        public static TextureGenerationOutput GenerateTextureGUI(NativeArray<Color32> buffer, int bufferWidth, int bufferHeight, TextureSettings settings, TextureImporterPlatformSettings platformSettings,
            TextureAlphaSettings alphaSettings = null, TextureMipmapSettings mipmapSettings = null, TextureWrapSettings wrapSettings = null)
        {
            settings.colorTexture = false;
            if (wrapSettings == null)
                wrapSettings = new TextureWrapSettings(TextureWrapMode.Clamp, TextureWrapMode.Clamp, TextureWrapMode.Clamp, TextureWrapMode.Clamp);
            return GenerateTexture(buffer, bufferWidth, bufferHeight, TextureImporterType.GUI, platformSettings, settings, alphaSettings, mipmapSettings, wrapSettings);
        }

        public static TextureGenerationOutput GenerateTextureSingleChannel(NativeArray<Color32> buffer, int bufferWidth, int bufferHeight, TextureSettings settings, TextureImporterPlatformSettings platformSettings,
            TextureAlphaSettings alphaSettings = null, TextureMipmapSettings mipmapSettings = null, TextureCubemapSettings cubemapSettings = null, TextureWrapSettings wrapSettings = null)
        {
            settings.colorTexture = false;
            return GenerateTexture(buffer, bufferWidth, bufferHeight, TextureImporterType.SingleChannel, platformSettings, settings, alphaSettings, mipmapSettings, cubemapSettings, wrapSettings);
        }

        public static TextureGenerationOutput GenerateTextureCursor(NativeArray<Color32> buffer, int bufferWidth, int bufferHeight, TextureSettings settings, TextureImporterPlatformSettings platformSettings,
            TextureAlphaSettings alphaSettings = null, TextureMipmapSettings mipmapSettings = null, TextureWrapSettings wrapSettings = null)
        {
            if (alphaSettings == null)
                alphaSettings = new TextureAlphaSettings(TextureImporterAlphaSource.FromInput, 0.5f);
            if (wrapSettings == null)
                wrapSettings = new TextureWrapSettings(TextureWrapMode.Clamp, TextureWrapMode.Clamp, TextureWrapMode.Clamp, TextureWrapMode.Clamp);

            return GenerateTexture(buffer, bufferWidth, bufferHeight, TextureImporterType.Cursor, platformSettings, settings, alphaSettings, mipmapSettings, wrapSettings);
        }

        public static TextureGenerationOutput GenerateTextureDefault(NativeArray<Color32> buffer, int bufferWidth, int bufferHeight, TextureSettings settings, TextureImporterPlatformSettings platformSettings,
            TextureAlphaSettings alphaSettings = null, TextureMipmapSettings mipmapSettings = null, TextureCubemapSettings cubemapSettings = null, TextureWrapSettings wrapSettings = null)
        {
            if (mipmapSettings == null)
                mipmapSettings = new TextureMipmapSettings(TextureImporterMipFilter.BoxFilter, false, false, false, 1, 3);

            return GenerateTexture(buffer, bufferWidth, bufferHeight, TextureImporterType.Default, platformSettings, settings, alphaSettings, mipmapSettings, cubemapSettings, wrapSettings);
        }

        static TextureGenerationOutput GenerateTexture(NativeArray<Color32> imageBuffer, int imageBufferWidth, int imageBufferHeight, TextureImporterType type, TextureImporterPlatformSettings platformSettings, params ITextureSettings[] otherSettings)
        {
            var textureGenerationSettings = new TextureGenerationSettings();
            textureGenerationSettings.platformSettings = platformSettings;

            textureGenerationSettings.sourceTextureInformation = new SourceTextureInformation();
            textureGenerationSettings.sourceTextureInformation.height = imageBufferHeight;
            textureGenerationSettings.sourceTextureInformation.width = imageBufferWidth;

            textureGenerationSettings.textureImporterSettings = new TextureImporterSettings();
            textureGenerationSettings.textureImporterSettings.textureType = type;
            textureGenerationSettings.textureImporterSettings.textureShape = TextureImporterShape.Texture2D;

            textureGenerationSettings.textureImporterSettings.alphaIsTransparency = false;
            textureGenerationSettings.textureImporterSettings.convertToNormalMap = false;
            textureGenerationSettings.textureImporterSettings.mipmapEnabled = false;
            textureGenerationSettings.textureImporterSettings.sRGBTexture = true;
            textureGenerationSettings.textureImporterSettings.readable = false;
            textureGenerationSettings.textureImporterSettings.fadeOut = false;
            textureGenerationSettings.textureImporterSettings.wrapMode = TextureWrapMode.Repeat;
            textureGenerationSettings.textureImporterSettings.wrapModeU = TextureWrapMode.Repeat;
            textureGenerationSettings.textureImporterSettings.wrapModeV = TextureWrapMode.Repeat;
            textureGenerationSettings.textureImporterSettings.wrapModeW = TextureWrapMode.Repeat;

            foreach (var otherSetting in otherSettings)
            {
                if (otherSetting != null)
                    otherSetting.FillTextureGenerationSettings(ref textureGenerationSettings);
            }
            return TextureGenerator.GenerateTexture(textureGenerationSettings, imageBuffer);
        }

        static public TextureSettings ExtractTextureSettings(this TextureImporterSettings tis)
        {
            var ts = new TextureSettings();
            ts.colorTexture = tis.sRGBTexture;
            ts.readable = tis.readable;
            ts.npotScale = tis.npotScale;
            ts.filterMode = tis.filterMode;
            ts.aniso = tis.aniso;
            return ts;
        }

        static public TextureSpriteSettings ExtractTextureSpriteSettings(this TextureImporterSettings tis)
        {
            var ts = new TextureSpriteSettings();
            ts.pixelsPerUnit = tis.spritePixelsPerUnit;
            ts.meshType = tis.spriteMeshType;
            ts.extrudeEdges = tis.spriteExtrude;
            return ts;
        }

        static public TextureWrapSettings ExtractTextureWrapSettings(this TextureImporterSettings tis)
        {
            var ts = new TextureWrapSettings();
            ts.wrapMode = tis.wrapMode;
            ts.wrapModeU = tis.wrapModeU;
            ts.wrapModeV = tis.wrapModeV;
            ts.wrapModeW = tis.wrapModeW;
            return ts;
        }

        static public TextureAlphaSettings ExtractTextureAlphaSettings(this TextureImporterSettings settings)
        {
            if (settings.alphaIsTransparency == false)
                return null;

            var ts = new TextureAlphaSettings();
            ts.alphaSource = settings.alphaSource;
            ts.alphaTolerance = settings.alphaTestReferenceValue;
            return ts;
        }

        static public TextureMipmapSettings ExtractTextureMipmapSettings(this TextureImporterSettings settings)
        {
            if (!settings.mipmapEnabled)
                return null;

            var ts = new TextureMipmapSettings();
            ts.filter = settings.mipmapFilter;
            ts.borderMipmap = settings.borderMipmap;
            ts.fadeout = settings.fadeOut;
            ts.fadeDistanceStart = settings.mipmapFadeDistanceStart;
            ts.fadeDistanceEnd = settings.mipmapFadeDistanceEnd;
            ts.preserveCoverage = settings.mipMapsPreserveCoverage;
            return ts;
        }

        static public TextureNormalSettings ExtractTextureNormalSettings(this TextureImporterSettings settings)
        {
            var ts = new TextureNormalSettings();
            ts.filter = settings.normalMapFilter;
            ts.generateFromGrayScale = settings.convertToNormalMap;
            ts.bumpiness = settings.heightmapScale;
            return ts;
        }

        static public TextureCubemapSettings ExtractTextureCubemapSettings(this TextureImporterSettings settings)
        {
            if (settings.textureShape != TextureImporterShape.TextureCube)
                return null;
            var ts = new TextureCubemapSettings();
            ts.convolution = settings.cubemapConvolution;
            ts.mode = settings.generateCubemap;
            ts.seamless = settings.seamlessCubemap;
            return ts;
        }
    }
}
