using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.U2D.PSD
{
    internal class TexturePlatformSettingsController
    {
        public bool HandleDefaultSettings(List<TextureImporterPlatformSettings> platformSettings, TexturePlatformSettingsView view)
        {
            Assert.IsTrue(platformSettings.Count > 0, "At least 1 platform setting is needed to display the texture platform setting UI.");

            int allSize = platformSettings[0].maxTextureSize;
            TextureImporterCompression allCompression = platformSettings[0].textureCompression;
            bool allUseCrunchedCompression = platformSettings[0].crunchedCompression;
            int allCompressionQuality = platformSettings[0].compressionQuality;
            TextureResizeAlgorithm allResizeAlgorithm = platformSettings[0].resizeAlgorithm;

            var newSize = allSize;
            var newCompression = allCompression;
            var newUseCrunchedCompression = allUseCrunchedCompression;
            var newCompressionQuality = allCompressionQuality;
            var newResizeAlgorithm = allResizeAlgorithm;

            bool mixedSize = false;
            bool mixedCompression = false;
            bool mixedUseCrunchedCompression = false;
            bool mixedCompressionQuality = false;
            bool mixedResizeAlgorithm = false;

            bool sizeChanged = false;
            bool compressionChanged = false;
            bool useCrunchedCompressionChanged = false;
            bool compressionQualityChanged = false;
            bool resizedChanged = false;

            for (var i = 1; i < platformSettings.Count; ++i)
            {
                var settings = platformSettings[i];
                if (settings.maxTextureSize != allSize)
                    mixedSize = true;
                if (settings.textureCompression != allCompression)
                    mixedCompression = true;
                if (settings.crunchedCompression != allUseCrunchedCompression)
                    mixedUseCrunchedCompression = true;
                if (settings.compressionQuality != allCompressionQuality)
                    mixedCompressionQuality = true;
                if (settings.resizeAlgorithm != allResizeAlgorithm)
                    mixedResizeAlgorithm = true;
            }

            EditorGUI.indentLevel++;
            newSize = view.DrawMaxSize(allSize, mixedSize, false,  out sizeChanged);
            newResizeAlgorithm = view.DrawResizeAlgorithm(allResizeAlgorithm, mixedResizeAlgorithm, false,  out resizedChanged);
            newCompression = view.DrawCompression(allCompression, mixedCompression, out compressionChanged);
            if (!mixedCompression && allCompression != TextureImporterCompression.Uncompressed)
            {
                newUseCrunchedCompression = view.DrawUseCrunchedCompression(allUseCrunchedCompression, mixedUseCrunchedCompression, out useCrunchedCompressionChanged);

                if (!mixedUseCrunchedCompression && allUseCrunchedCompression)
                {
                    newCompressionQuality = view.DrawCompressionQualitySlider(allCompressionQuality, mixedCompressionQuality, out compressionQualityChanged);
                }
            }
            EditorGUI.indentLevel--;

            if (sizeChanged || compressionChanged || useCrunchedCompressionChanged || compressionQualityChanged || resizedChanged)
            {
                for (var i = 0; i < platformSettings.Count; ++i)
                {
                    if (sizeChanged)
                        platformSettings[i].maxTextureSize = newSize;
                    if (compressionChanged)
                        platformSettings[i].textureCompression = newCompression;
                    if (useCrunchedCompressionChanged)
                        platformSettings[i].crunchedCompression = newUseCrunchedCompression;
                    if (compressionQualityChanged)
                        platformSettings[i].compressionQuality = newCompressionQuality;
                    if (resizedChanged)
                        platformSettings[i].resizeAlgorithm = newResizeAlgorithm;
                }
                return true;
            }
            else
                return false;
        }

        public bool HandlePlatformSettings(BuildTarget buildTarget, List<TextureImporterPlatformSettings> platformSettings, TexturePlatformSettingsView view)
        {
            if (buildTarget == BuildTarget.NoTarget)
            {
                return HandleDefaultSettings(platformSettings, view);
            }
            Assert.IsTrue(platformSettings.Count > 0, "At least 1 platform setting is needed to display the texture platform setting UI.");

            bool allOverride = platformSettings[0].overridden;
            int allSize = platformSettings[0].maxTextureSize;
            TextureImporterFormat allFormat = platformSettings[0].format;
            int allCompressionQuality = platformSettings[0].compressionQuality;
            TextureResizeAlgorithm allResizeAlgorithm = platformSettings[0].resizeAlgorithm;
            var newResizeAlgorithm = allResizeAlgorithm;

            var newOverride = allOverride;
            var newSize = allSize;
            var newFormat = allFormat;
            var newCompressionQuality = allCompressionQuality;

            bool mixedOverride = false;
            bool mixedSize = false;
            bool mixedFormat = false;
            bool mixedCompression = false;
            bool mixedResizeAlgorithm = false;

            bool overrideChanged = false;
            bool sizeChanged = false;
            bool formatChanged = false;
            bool compressionChanged = false;
            bool resizedChanged = false;

            for (var i = 1; i < platformSettings.Count; ++i)
            {
                var settings = platformSettings[i];
                if (settings.overridden != allOverride)
                    mixedOverride = true;
                if (settings.maxTextureSize != allSize)
                    mixedSize = true;
                if (settings.format != allFormat)
                    mixedFormat = true;
                if (settings.compressionQuality != allCompressionQuality)
                    mixedCompression = true;
                if (settings.resizeAlgorithm != allResizeAlgorithm)
                    mixedResizeAlgorithm = true;
            }

            EditorGUI.indentLevel++;
            newOverride = view.DrawOverride(allOverride, mixedOverride, out overrideChanged);
            newResizeAlgorithm = view.DrawResizeAlgorithm(allResizeAlgorithm, mixedResizeAlgorithm, mixedOverride || !allOverride, out resizedChanged);
            newSize = view.DrawMaxSize(allSize, mixedSize, mixedOverride || !allOverride, out sizeChanged);

            int[] formatValues = null;
            string[] formatStrings = null;
            AcquireTextureFormatValuesAndStrings(buildTarget, out formatValues, out formatStrings);

            newFormat = view.DrawFormat(allFormat, formatValues, formatStrings, mixedFormat, mixedOverride || !allOverride, out formatChanged);


            if (!mixedFormat && !mixedOverride && allOverride && IsFormatRequireCompressionSetting(allFormat))
            {
                bool showAsEnum =
                    buildTarget == BuildTarget.iOS ||
                    buildTarget == BuildTarget.tvOS ||
                    buildTarget == BuildTarget.Android
                ;

                if (showAsEnum)
                {
                    int compressionMode = 1;
                    if (allCompressionQuality == (int)TextureCompressionQuality.Fast)
                        compressionMode = 0;
                    else if (allCompressionQuality == (int)TextureCompressionQuality.Best)
                        compressionMode = 2;

                    var returnValue = view.DrawCompressionQualityPopup(compressionMode, mixedCompression, out compressionChanged);

                    if (compressionChanged)
                    {
                        switch (returnValue)
                        {
                            case 0:
                                newCompressionQuality = (int)TextureCompressionQuality.Fast;
                                break;
                            case 1:
                                newCompressionQuality = (int)TextureCompressionQuality.Normal;
                                break;
                            case 2:
                                newCompressionQuality = (int)TextureCompressionQuality.Best;
                                break;

                            default:
                                Assert.IsTrue(false, "ITexturePlatformSettingsView.DrawCompressionQualityPopup should never return compression option value that's not 0, 1 or 2.");
                                break;
                        }
                    }
                }
                else
                {
                    newCompressionQuality = view.DrawCompressionQualitySlider(allCompressionQuality, mixedCompression, out compressionChanged);
                }
            }
            EditorGUI.indentLevel--;

            if (overrideChanged || sizeChanged || formatChanged || compressionChanged || resizedChanged)
            {
                for (var i = 0; i < platformSettings.Count; ++i)
                {
                    if (overrideChanged)
                        platformSettings[i].overridden = newOverride;
                    if (sizeChanged)
                        platformSettings[i].maxTextureSize = newSize;
                    if (formatChanged)
                        platformSettings[i].format = newFormat;
                    if (compressionChanged)
                        platformSettings[i].compressionQuality = newCompressionQuality;
                    if (resizedChanged)
                        platformSettings[i].resizeAlgorithm = newResizeAlgorithm;
                }

                return true;
            }
            else
                return false;
        }

        public void AcquireTextureFormatValuesAndStrings(BuildTarget buildTarget, out int[] formatValues, out string[] formatStrings)
        {
            if (IsGLESMobileTargetPlatform(buildTarget))
            {
                if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS)
                {
                    formatValues = TexturePlatformSettingsModal.kTextureFormatsValueApplePVR;
                    formatStrings = TexturePlatformSettingsModal.s_TextureFormatStringsApplePVR;
                }
                else
                {
                    formatValues = TexturePlatformSettingsModal.kTextureFormatsValueAndroid;
                    formatStrings = TexturePlatformSettingsModal.s_TextureFormatStringsAndroid;
                }
            }
            else
            {
                if (buildTarget == BuildTarget.WebGL)
                {
                    formatValues = TexturePlatformSettingsModal.kTextureFormatsValueWebGL;
                    formatStrings = TexturePlatformSettingsModal.s_TextureFormatStringsWebGL;
                }
                else
                {
                    formatValues = TexturePlatformSettingsModal.kTextureFormatsValueDefault;
                    formatStrings = TexturePlatformSettingsModal.s_TextureFormatStringsDefault;
                }
            }
        }

        internal static bool IsFormatRequireCompressionSetting(TextureImporterFormat format)
        {
            return ArrayUtility.Contains<TextureImporterFormat>(TexturePlatformSettingsModal.kFormatsWithCompressionSettings, format);
        }

        internal static bool IsGLESMobileTargetPlatform(BuildTarget target)
        {
            return target == BuildTarget.iOS || target == BuildTarget.tvOS || target == BuildTarget.Android;
        }
    }
}
