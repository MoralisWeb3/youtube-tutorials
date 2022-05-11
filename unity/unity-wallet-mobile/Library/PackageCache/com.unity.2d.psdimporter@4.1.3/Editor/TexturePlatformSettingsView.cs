using UnityEngine;

namespace UnityEditor.U2D.PSD
{
    internal class TexturePlatformSettingsView
    {
        class Styles
        {
            public readonly GUIContent textureFormatLabel = new GUIContent("Format");
            public readonly GUIContent maxTextureSizeLabel = new GUIContent("Max Texture Size", "Maximum size of the packed texture.");
            public readonly GUIContent compressionLabel = new GUIContent("Compression", "How will this texture be compressed?");
            public readonly GUIContent resizeAlgorithmLabel = new GUIContent("Resize", "Algorithm to use when resizing texture");
            public readonly GUIContent useCrunchedCompressionLabel = new GUIContent("Use Crunch Compression", "Texture is crunch-compressed to save space on disk when applicable.");
            public readonly GUIContent compressionQualityLabel = new GUIContent("Compressor Quality");
            public readonly GUIContent compressionQualitySliderLabel = new GUIContent("Compressor Quality", "Use the slider to adjust compression quality from 0 (Fastest) to 100 (Best)");

            public readonly int[] kMaxTextureSizeValues = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
            public readonly GUIContent[] kMaxTextureSizeStrings;

            public readonly GUIContent[] kTextureCompressionOptions =
            {
                new GUIContent("None", "Texture is not compressed."),
                new GUIContent("Low Quality", "Texture compressed with low quality but high performance, high compression format."),
                new GUIContent("Normal Quality", "Texture is compressed with a standard format."),
                new GUIContent("High Quality", "Texture compressed with a high quality format."),
            };

            public readonly GUIContent[] kResizeAlgoritmOptions =
            {
                new GUIContent(TextureResizeAlgorithm.Mitchell.ToString()),
                new GUIContent(TextureResizeAlgorithm.Bilinear.ToString()),
            };

            public readonly int[] kTextureCompressionValues =
            {
                (int)TextureImporterCompression.Uncompressed,
                (int)TextureImporterCompression.CompressedLQ,
                (int)TextureImporterCompression.Compressed,
                (int)TextureImporterCompression.CompressedHQ
            };

            public readonly GUIContent[] kMobileCompressionQualityOptions =
            {
                new GUIContent("Fast"),
                new GUIContent("Normal"),
                new GUIContent("Best")
            };

            public Styles()
            {
                kMaxTextureSizeStrings = new GUIContent[kMaxTextureSizeValues.Length];
                for (var i = 0; i < kMaxTextureSizeValues.Length; ++i)
                    kMaxTextureSizeStrings[i] = new GUIContent(string.Format("{0}", kMaxTextureSizeValues[i]));
            }
        }

        private static Styles s_Styles;

        public string buildPlatformTitle { get; set; }

        internal TexturePlatformSettingsView()
        {
            s_Styles = s_Styles ?? new Styles();
        }

        public virtual TextureResizeAlgorithm DrawResizeAlgorithm(TextureResizeAlgorithm defaultValue, bool isMixedValue, bool isDisabled, out bool changed)
        {
            using (new EditorGUI.DisabledScope(isDisabled))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = isMixedValue;
                defaultValue = (TextureResizeAlgorithm)EditorGUILayout.EnumPopup(s_Styles.resizeAlgorithmLabel, defaultValue);
                EditorGUI.showMixedValue = false;
                changed = EditorGUI.EndChangeCheck();
            }
            return defaultValue;
        }

        public virtual TextureImporterCompression DrawCompression(TextureImporterCompression defaultValue, bool isMixedValue, out bool changed)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = isMixedValue;
            defaultValue = (TextureImporterCompression)EditorGUILayout.IntPopup(s_Styles.compressionLabel, (int)defaultValue, s_Styles.kTextureCompressionOptions, s_Styles.kTextureCompressionValues);
            EditorGUI.showMixedValue = false;
            changed = EditorGUI.EndChangeCheck();
            return defaultValue;
        }

        public virtual bool DrawUseCrunchedCompression(bool defaultValue, bool isMixedValue, out bool changed)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = isMixedValue;
            defaultValue = EditorGUILayout.Toggle(s_Styles.useCrunchedCompressionLabel, defaultValue);
            EditorGUI.showMixedValue = false;
            changed = EditorGUI.EndChangeCheck();
            return defaultValue;
        }

        public virtual bool DrawOverride(bool defaultValue, bool isMixedValue, out bool changed)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = isMixedValue;
            defaultValue = EditorGUILayout.ToggleLeft(new GUIContent("Override"), defaultValue);
            EditorGUI.showMixedValue = false;
            changed = EditorGUI.EndChangeCheck();
            return defaultValue;
        }

        public virtual int DrawMaxSize(int defaultValue, bool isMixedValue, bool isDisabled, out bool changed)
        {
            using (new EditorGUI.DisabledScope(isDisabled))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = isMixedValue;
                defaultValue = EditorGUILayout.IntPopup(s_Styles.maxTextureSizeLabel, defaultValue, s_Styles.kMaxTextureSizeStrings, s_Styles.kMaxTextureSizeValues);
                EditorGUI.showMixedValue = false;
                changed = EditorGUI.EndChangeCheck();
                return defaultValue;
            }
        }

        public virtual TextureImporterFormat DrawFormat(TextureImporterFormat defaultValue, int[] displayValues, string[] displayStrings, bool isMixedValue, bool isDisabled, out bool changed)
        {
            using (new EditorGUI.DisabledScope(isDisabled))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = isMixedValue;
                defaultValue = (TextureImporterFormat)EditorGUILayout.IntPopup(s_Styles.textureFormatLabel.text, (int)defaultValue, displayStrings, displayValues);
                EditorGUI.showMixedValue = false;
                changed = EditorGUI.EndChangeCheck();
                return defaultValue;
            }
        }

        public virtual int DrawCompressionQualityPopup(int defaultValue, bool isMixedValue, out bool changed)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = isMixedValue;
            defaultValue = EditorGUILayout.Popup(s_Styles.compressionQualityLabel, defaultValue, s_Styles.kMobileCompressionQualityOptions);
            EditorGUI.showMixedValue = false;
            changed = EditorGUI.EndChangeCheck();
            return defaultValue;
        }

        public virtual int DrawCompressionQualitySlider(int defaultValue, bool isMixedValue, out bool changed)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = isMixedValue;
            defaultValue = EditorGUILayout.IntSlider(s_Styles.compressionQualitySliderLabel, defaultValue, 0, 100);
            EditorGUI.showMixedValue = false;
            changed = EditorGUI.EndChangeCheck();
            return defaultValue;
        }
    }
}
