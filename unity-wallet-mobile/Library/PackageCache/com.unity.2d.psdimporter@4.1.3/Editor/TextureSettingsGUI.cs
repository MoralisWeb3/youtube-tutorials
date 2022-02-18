//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace UnityEditor.U2D
//{
//    public class TextureSettingsGUI
//    {
//        public SerializedProperty colorTexture;
//        public SerializedProperty readable;
//        public SerializedProperty npotScale;
//        public SerializedProperty filterMode;
//        public SerializedProperty aniso;
//        public SerializedProperty enablePostProcessor;

//        readonly int[] m_FilterModeOptions = (int[])(Enum.GetValues(typeof(FilterMode)));
//        public TextureSettingsGUI(SerializedProperty sp)
//        {
//            colorTexture = sp.FindPropertyRelative("m_ColorTexture");
//            readable = sp.FindPropertyRelative("m_Readable");
//            npotScale = sp.FindPropertyRelative("m_NPOTScale");
//            filterMode = sp.FindPropertyRelative("m_FilterMode");
//            aniso = sp.FindPropertyRelative("m_Aniso");
//            enablePostProcessor = sp.FindPropertyRelative("m_EnablePostProcessor");
//        }

//        public void OnInspectorGUI(bool isPOT, bool isNormalMap, bool hasMipMap, bool isCubeMap, bool hasMipmapFadeout)
//        {

//            TextureSettingsGUIUtils.ToggleFromInt(colorTexture, TextureSettingsGUIUtils.s_Styles.sRGBTexture);
//            TextureSettingsGUIUtils.ToggleFromInt(readable, TextureSettingsGUIUtils.s_Styles.readWrite);
//            using (new EditorGUI.DisabledScope(isPOT))
//            {
//                TextureSettingsGUIUtils.EnumPopup(npotScale, typeof(TextureImporterNPOTScale), TextureSettingsGUIUtils.s_Styles.npot);
//            }

//            EditorGUI.BeginChangeCheck();
//            // Filter mode
//            EditorGUI.showMixedValue = filterMode.hasMultipleDifferentValues;
//            FilterMode filter = (FilterMode)filterMode.intValue;
//            if ((int)filter == -1)
//            {
//                if (hasMipmapFadeout || isNormalMap)
//                    filter = FilterMode.Trilinear;
//                else
//                    filter = FilterMode.Bilinear;
//            }
//            filter = (FilterMode)EditorGUILayout.IntPopup(TextureSettingsGUIUtils.s_Styles.filterMode, (int)filter, TextureSettingsGUIUtils.s_Styles.filterModeOptions, m_FilterModeOptions);
//            EditorGUI.showMixedValue = false;
//            if (EditorGUI.EndChangeCheck())
//                filterMode.intValue = (int)filter;

//            // Aniso
//            bool showAniso = (FilterMode)filter != FilterMode.Point && hasMipMap && isCubeMap;
//            using (new EditorGUI.DisabledScope(!showAniso))
//            {
//                EditorGUI.BeginChangeCheck();
//                EditorGUI.showMixedValue = aniso.hasMultipleDifferentValues;
//                int anisoValue = aniso.intValue;
//                if (anisoValue == -1)
//                    anisoValue = 1;
//                //aniso = EditorGUILayout.IntSlider("Aniso Level", aniso, 0, 16);
//                EditorGUI.showMixedValue = false;
//                if (EditorGUI.EndChangeCheck())
//                    aniso.intValue = anisoValue;

//                if (anisoValue > 1)
//                {
//                    if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable)
//                        EditorGUILayout.HelpBox("Anisotropic filtering is disabled for all textures in Quality Settings.", MessageType.Info);
//                    else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
//                        EditorGUILayout.HelpBox("Anisotropic filtering is enabled for all textures in Quality Settings.", MessageType.Info);
//                }
//            }
//        }
//    }

//    public class TextureSpriteSettingsGUI
//    {
//        public SerializedProperty packingTag;
//        public SerializedProperty ppu;
//        public SerializedProperty meshType;
//        public SerializedProperty extrudeEdges;

//        public TextureSpriteSettingsGUI(SerializedProperty sp)
//        {
//            packingTag = sp.FindPropertyRelative("m_PackingTag");
//            ppu = sp.FindPropertyRelative("m_PixelsPerUnit");
//            meshType = sp.FindPropertyRelative("m_MeshType");
//            extrudeEdges = sp.FindPropertyRelative("m_ExtrudeEdges");
//        }

//        public void OnInspectorGUI()
//        {
//            //// Show generic attributes
//            //if (m_SpriteMode.intValue != 0)
//            //{
//            //    EditorGUILayout.PropertyField(m_SpritePackingTag, s_Styles.spritePackingTag);
//            //    EditorGUILayout.PropertyField(m_SpritePixelsToUnits, s_Styles.spritePixelsPerUnit);

//            //    if (m_SpriteMode.intValue != (int)SpriteImportMode.Polygon && !m_SpriteMode.hasMultipleDifferentValues)
//            //    {
//            //        EditorGUILayout.IntPopup(m_SpriteMeshType, s_Styles.spriteMeshTypeOptions, new[] { 0, 1 }, s_Styles.spriteMeshType);
//            //    }
//            //    EditorGUILayout.EndFadeGroup();

//            //    EditorGUILayout.IntSlider(m_SpriteExtrude, 0, 32, s_Styles.spriteExtrude);

//            //    if (m_SpriteMode.intValue == 1)
//            //    {
//            //        EditorGUILayout.IntPopup(m_Alignment, s_Styles.spriteAlignmentOptions, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, s_Styles.spriteAlignment);

//            //        if (m_Alignment.intValue == (int)SpriteAlignment.Custom)
//            //        {
//            //            GUILayout.BeginHorizontal();
//            //            EditorGUILayout.PropertyField(m_SpritePivot, new GUIContent());
//            //            GUILayout.EndHorizontal();
//            //        }
//            //    }
//            //}
//        }
//    }

//    public class TextureWrapSettingsGUI
//    {
//        public SerializedProperty wrapMode;
//        public SerializedProperty wrapModeU;
//        public SerializedProperty wrapModeV;
//        public SerializedProperty wrapModeW;

//        public TextureWrapSettingsGUI(SerializedProperty sp)
//        {
//            wrapMode = sp.FindPropertyRelative("m_WrapMode");
//            wrapModeU = sp.FindPropertyRelative("m_WrapModeU");
//            wrapModeV = sp.FindPropertyRelative("m_WrapModeV");
//            wrapModeW = sp.FindPropertyRelative("m_WrapModeW");
//        }

//        public void OnInspectorGUI()
//        {

//        }
//    }


//    public class TextureAlphaSettingsGUI
//    {
//        public SerializedProperty tolerance;
//        public SerializedProperty source;

//        public TextureAlphaSettingsGUI(SerializedProperty sp)
//        {
//            tolerance = sp.FindPropertyRelative("m_AlphaTolerance");
//            source = sp.FindPropertyRelative("m_AlphaSource");
//        }

//        public void OnInspectorGUI()
//        {

//        }
//    }

//    public class TextureMipmapSettingsGUI
//    {
//        public SerializedProperty filter;
//        public SerializedProperty borderMipmap;
//        public SerializedProperty fadeout;
//        public SerializedProperty preserveCoverage;
//        public SerializedProperty fadeDistanceStart;
//        public SerializedProperty fadeDistanceEnd;

//        public TextureMipmapSettingsGUI(SerializedProperty sp)
//        {
//            filter = sp.FindPropertyRelative("m_Filter");
//            borderMipmap = sp.FindPropertyRelative("m_BorderMipmap");
//            fadeout = sp.FindPropertyRelative("m_Fadeout");
//            preserveCoverage = sp.FindPropertyRelative("m_PreserveCoverage");
//            fadeDistanceStart = sp.FindPropertyRelative("m_FadeDistanceStart");
//            fadeDistanceEnd = sp.FindPropertyRelative("m_FadeDistanceEnd");
//        }

//        public void OnInspectorGUI()
//        {

//        }
//    }

//    public class TextureNormalSettingsGUI
//    {
//        public SerializedProperty filter;
//        public SerializedProperty generateFromGrayScale;
//        public SerializedProperty bumpiness;

//        public TextureNormalSettingsGUI(SerializedProperty sp)
//        {
//            filter = sp.FindPropertyRelative("m_Filter");
//            generateFromGrayScale = sp.FindPropertyRelative("m_GenerateFromGrayScale");
//            bumpiness = sp.FindPropertyRelative("m_Bumpiness");
//        }

//        public void OnInspectorGUI()
//        {

//        }
//    }


//    public class TextureCubemapSettingsGUI
//    {
//        public SerializedProperty convolution;
//        public SerializedProperty mode;
//        public SerializedProperty seamless;

//        public TextureCubemapSettingsGUI(SerializedProperty sp)
//        {
//            convolution = sp.FindPropertyRelative("m_Convolution");
//            mode = sp.FindPropertyRelative("m_Mode");
//            seamless = sp.FindPropertyRelative("m_Seamless");
//        }

//        public void OnInspectorGUI()
//        {

//        }
//    }


//    static class TextureSettingsGUIUtils
//    {
//        public static void ToggleFromInt(SerializedProperty property, GUIContent label)
//        {
//            EditorGUI.BeginChangeCheck();
//            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
//            int value = EditorGUILayout.Toggle(label, property.intValue > 0) ? 1 : 0;
//            EditorGUI.showMixedValue = false;
//            if (EditorGUI.EndChangeCheck())
//                property.intValue = value;
//        }

//        public static void EnumPopup(SerializedProperty property, System.Type type, GUIContent label)
//        {
//            EditorGUILayout.IntPopup(label.text, property.intValue,
//                System.Enum.GetNames(type),
//                System.Enum.GetValues(type) as int[]);
//        }

//        internal class Styles
//        {
//            public readonly GUIContent textureTypeTitle = new GUIContent("Texture Type", "What will this texture be used for?");
//            public readonly GUIContent[] textureTypeOptions =
//            {
//                new GUIContent("Default", "Texture is a normal image such as a diffuse texture or other."),
//                new GUIContent("Sprite (2D and UI)", "Texture is used for a sprite."),
//            };
//            public readonly int[] textureTypeValues =
//            {
//                (int)TextureImporterType.Default,
//                (int)TextureImporterType.Sprite,
//            };

//            public readonly GUIContent textureShape = new GUIContent("Texture Shape", "What shape is this texture?");
//            private readonly GUIContent textureShape2D = new GUIContent("2D, Texture is 2D.");
//            private readonly  GUIContent textureShapeCube = new GUIContent("Cube", "Texture is a Cubemap.");
//            public readonly Dictionary<TextureImporterShape, GUIContent[]> textureShapeOptionsDictionnary = new Dictionary<TextureImporterShape, GUIContent[]>();
//            public readonly Dictionary<TextureImporterShape, int[]> textureShapeValuesDictionnary = new Dictionary<TextureImporterShape, int[]>();


//            public readonly GUIContent filterMode = new GUIContent("Filter Mode");
//            public readonly GUIContent[] filterModeOptions =
//            {
//                new GUIContent("Point (no filter)"),
//                new GUIContent("Bilinear"),
//                new GUIContent("Trilinear")
//            };

//            public readonly GUIContent textureFormat = new GUIContent("Format");

//            public readonly GUIContent defaultPlatform = new GUIContent("Default");
//            public readonly GUIContent mipmapFadeOutToggle = new GUIContent("Fadeout Mip Maps");
//            public readonly GUIContent mipmapFadeOut = new GUIContent("Fade Range");
//            public readonly GUIContent readWrite = new GUIContent("Read/Write Enabled", "Enable to be able to access the raw pixel data from code.");

//            public readonly GUIContent alphaSource = new GUIContent("Alpha Source", "How is the alpha generated for the imported texture.");
//            public readonly GUIContent[] alphaSourceOptions =
//            {
//                new GUIContent("None", "No Alpha will be used."),
//                new GUIContent("Input Texture Alpha", "Use Alpha from the input texture if one is provided."),
//                new GUIContent("From Gray Scale", "Generate Alpha from image gray scale."),
//            };
//            public readonly int[] alphaSourceValues =
//            {
//                (int)TextureImporterAlphaSource.None,
//                (int)TextureImporterAlphaSource.FromInput,
//                (int)TextureImporterAlphaSource.FromGrayScale,
//            };

//            public readonly GUIContent generateMipMaps = new GUIContent("Generate Mip Maps");
//            public readonly GUIContent sRGBTexture = new GUIContent("sRGB (Color Texture)", "Texture content is stored in gamma space. Non-HDR color textures should enable this flag (except if used for IMGUI).");
//            public readonly GUIContent borderMipMaps = new GUIContent("Border Mip Maps");
//            public readonly GUIContent mipMapsPreserveCoverage = new GUIContent("Mip Maps Preserve Coverage", "The alpha channel of generated Mip Maps will preserve coverage during the alpha test.");
//            public readonly GUIContent alphaTestReferenceValue = new GUIContent("Alpha Cutoff Value", "The reference value used during the alpha test. Controls Mip Map coverage.");
//            public readonly GUIContent mipMapFilter = new GUIContent("Mip Map Filtering");
//            public readonly GUIContent[] mipMapFilterOptions =
//            {
//                new GUIContent("Box"),
//                new GUIContent("Kaiser"),
//            };
//            public readonly GUIContent npot = new GUIContent("Non Power of 2", "How non-power-of-two textures are scaled on import.");

//            public readonly GUIContent compressionQuality = new GUIContent("Compressor Quality");
//            public readonly GUIContent compressionQualitySlider = new GUIContent("Compressor Quality", "Use the slider to adjust compression quality from 0 (Fastest) to 100 (Best)");
//            public readonly GUIContent[] mobileCompressionQualityOptions =
//            {
//                new GUIContent("Fast"),
//                new GUIContent("Normal"),
//                new GUIContent("Best")
//            };

//            public readonly GUIContent spriteMode = new GUIContent("Sprite Mode");
//            public readonly GUIContent[] spriteModeOptions =
//            {
//                new GUIContent("Single"),
//                new GUIContent("Multiple"),
//                new GUIContent("Polygon"),
//            };
//            public readonly GUIContent[] spriteMeshTypeOptions =
//            {
//                new GUIContent("Full Rect"),
//                new GUIContent("Tight"),
//            };

//            public readonly GUIContent spritePackingTag = new GUIContent("Packing Tag", "Tag for the Sprite Packing system.");
//            public readonly GUIContent spritePixelsPerUnit = new GUIContent("Pixels Per Unit", "How many pixels in the sprite correspond to one unit in the world.");
//            public readonly GUIContent spriteExtrude = new GUIContent("Extrude Edges", "How much empty area to leave around the sprite in the generated mesh.");
//            public readonly GUIContent spriteMeshType = new GUIContent("Mesh Type", "Type of sprite mesh to generate.");
//            public readonly GUIContent spriteAlignment = new GUIContent("Pivot", "Sprite pivot point in its localspace. May be used for syncing animation frames of different sizes.");
//            public readonly GUIContent[] spriteAlignmentOptions =
//            {
//                new GUIContent("Center"),
//                new GUIContent("Top Left"),
//                new GUIContent("Top"),
//                new GUIContent("Top Right"),
//                new GUIContent("Left"),
//                new GUIContent("Right"),
//                new GUIContent("Bottom Left"),
//                new GUIContent("Bottom"),
//                new GUIContent("Bottom Right"),
//                new GUIContent("Custom"),
//            };

//            public readonly GUIContent alphaIsTransparency = new GUIContent("Alpha Is Transparency", "If the provided alpha channel is transparency, enable this to pre-filter the color to avoid texture filtering artifacts. This is not supported for HDR textures.");
//            public readonly GUIContent etc1Compression = new GUIContent("Compress using ETC1 (split alpha channel)|Alpha for this texture will be preserved by splitting the alpha channel to another texture, and both resulting textures will be compressed using ETC1.");

//            public readonly GUIContent crunchedCompression = new GUIContent("Use Crunch Compression", "Texture is crunch-compressed to save space on disk when applicable.");

//            public readonly GUIContent showAdvanced = new GUIContent("Advanced", "Show advanced settings.");

//            public Styles()
//            {
//                // This is far from ideal, but it's better than having tons of logic in the GUI code itself.
//                // The combination should not grow too much anyway since only Texture3D will be added later.
//                GUIContent[] s2D_Options = { textureShape2D };
//                GUIContent[] sCube_Options = { textureShapeCube };
//                GUIContent[] s2D_Cube_Options = { textureShape2D, textureShapeCube };
//                textureShapeOptionsDictionnary.Add(TextureImporterShape.Texture2D, s2D_Options);
//                textureShapeOptionsDictionnary.Add(TextureImporterShape.TextureCube, sCube_Options);
//                textureShapeOptionsDictionnary.Add(TextureImporterShape.Texture2D | TextureImporterShape.TextureCube, s2D_Cube_Options);

//                int[] s2D_Values = { (int)TextureImporterShape.Texture2D };
//                int[] sCube_Values = { (int)TextureImporterShape.TextureCube };
//                int[] s2D_Cube_Values = { (int)TextureImporterShape.Texture2D, (int)TextureImporterShape.TextureCube };
//                textureShapeValuesDictionnary.Add(TextureImporterShape.Texture2D, s2D_Values);
//                textureShapeValuesDictionnary.Add(TextureImporterShape.TextureCube, sCube_Values);
//                textureShapeValuesDictionnary.Add(TextureImporterShape.Texture2D | TextureImporterShape.TextureCube, s2D_Cube_Values);

//            }
//        }

//        internal static Styles s_Styles;
//    }
//}
