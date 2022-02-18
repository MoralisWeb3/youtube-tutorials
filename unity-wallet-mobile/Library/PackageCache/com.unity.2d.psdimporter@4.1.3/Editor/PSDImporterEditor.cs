using System;
using System.Collections.Generic;
using System.IO;
using PhotoshopFile;
using UnityEditor.AssetImporters;
using UnityEditor.U2D.Animation;
using UnityEditor.U2D.Common;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.U2D.PSD
{
    /// <summary>
    /// Inspector for PSDImporter
    /// </summary>
    [CustomEditor(typeof(PSDImporter))]
    [MovedFrom("UnityEditor.Experimental.AssetImporters")]
    public class PSDImporterEditor : ScriptedImporterEditor, ITexturePlatformSettingsDataProvider
    {
        SerializedProperty m_TextureType;
        SerializedProperty m_TextureShape;
        SerializedProperty m_SpriteMode;
        SerializedProperty m_SpritePixelsToUnits;
        SerializedProperty m_SpriteMeshType;
        SerializedProperty m_SpriteExtrude;
        SerializedProperty m_Alignment;
        SerializedProperty m_SpritePivot;
        SerializedProperty m_NPOTScale;
        SerializedProperty m_IsReadable;
        SerializedProperty m_sRGBTexture;
        SerializedProperty m_AlphaSource;
        SerializedProperty m_MipMapMode;
        SerializedProperty m_EnableMipMap;
        SerializedProperty m_FadeOut;
        SerializedProperty m_BorderMipMap;
        SerializedProperty m_MipMapsPreserveCoverage;
        SerializedProperty m_AlphaTestReferenceValue;
        SerializedProperty m_MipMapFadeDistanceStart;
        SerializedProperty m_MipMapFadeDistanceEnd;
        SerializedProperty m_AlphaIsTransparency;
        SerializedProperty m_FilterMode;
        SerializedProperty m_Aniso;

        SerializedProperty m_WrapU;
        SerializedProperty m_WrapV;
        SerializedProperty m_WrapW;
        SerializedProperty m_ConvertToNormalMap;
        SerializedProperty m_MosaicLayers;
        SerializedProperty m_ImportHiddenLayers;
        SerializedProperty m_ResliceFromLayer;
        SerializedProperty m_CharacterMode;
        SerializedProperty m_DocumentPivot;
        SerializedProperty m_DocumentAlignment;
        SerializedProperty m_GenerateGOHierarchy;
        SerializedProperty m_PaperDollMode;
        SerializedProperty m_KeepDupilcateSpriteName;

        readonly int[] m_FilterModeOptions = (int[])(Enum.GetValues(typeof(FilterMode)));

        bool    m_IsPOT = false;
        bool m_ShowAdvanced = false;
        bool m_ShowExperimental = false;
        Dictionary<TextureImporterType, Action[]> m_AdvanceInspectorGUI = new Dictionary<TextureImporterType, Action[]>();
        int m_PlatformSettingsIndex;
        bool m_ShowPerAxisWrapModes = false;

        TexturePlatformSettingsHelper m_TexturePlatformSettingsHelper;

        TexturePlatformSettingsView m_TexturePlatformSettingsView = new TexturePlatformSettingsView();
        TexturePlatformSettingsController m_TexturePlatformSettingsController = new TexturePlatformSettingsController();

        /// <summary>
        /// Implementation of AssetImporterEditor.OnEnable
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            m_MosaicLayers = serializedObject.FindProperty("m_MosaicLayers");
            m_ImportHiddenLayers = serializedObject.FindProperty("m_ImportHiddenLayers");
            m_ResliceFromLayer = serializedObject.FindProperty("m_ResliceFromLayer");
            m_CharacterMode = serializedObject.FindProperty("m_CharacterMode");
            m_DocumentPivot = serializedObject.FindProperty("m_DocumentPivot");
            m_DocumentAlignment = serializedObject.FindProperty("m_DocumentAlignment");
            m_GenerateGOHierarchy = serializedObject.FindProperty("m_GenerateGOHierarchy");
            m_PaperDollMode = serializedObject.FindProperty("m_PaperDollMode");
            m_KeepDupilcateSpriteName = serializedObject.FindProperty("m_KeepDupilcateSpriteName");

            var textureImporterSettingsSP = serializedObject.FindProperty("m_TextureImporterSettings");
            m_TextureType = textureImporterSettingsSP.FindPropertyRelative("m_TextureType");
            m_TextureShape = textureImporterSettingsSP.FindPropertyRelative("m_TextureShape");
            m_ConvertToNormalMap = textureImporterSettingsSP.FindPropertyRelative("m_ConvertToNormalMap");
            m_SpriteMode = textureImporterSettingsSP.FindPropertyRelative("m_SpriteMode");
            m_SpritePixelsToUnits = textureImporterSettingsSP.FindPropertyRelative("m_SpritePixelsToUnits");
            m_SpriteMeshType = textureImporterSettingsSP.FindPropertyRelative("m_SpriteMeshType");
            m_SpriteExtrude = textureImporterSettingsSP.FindPropertyRelative("m_SpriteExtrude");
            m_Alignment = textureImporterSettingsSP.FindPropertyRelative("m_Alignment");
            m_SpritePivot = textureImporterSettingsSP.FindPropertyRelative("m_SpritePivot");
            m_NPOTScale = textureImporterSettingsSP.FindPropertyRelative("m_NPOTScale");
            m_IsReadable = textureImporterSettingsSP.FindPropertyRelative("m_IsReadable");
            m_sRGBTexture = textureImporterSettingsSP.FindPropertyRelative("m_sRGBTexture");
            m_AlphaSource = textureImporterSettingsSP.FindPropertyRelative("m_AlphaSource");
            m_MipMapMode = textureImporterSettingsSP.FindPropertyRelative("m_MipMapMode");
            m_EnableMipMap = textureImporterSettingsSP.FindPropertyRelative("m_EnableMipMap");
            m_FadeOut = textureImporterSettingsSP.FindPropertyRelative("m_FadeOut");
            m_BorderMipMap = textureImporterSettingsSP.FindPropertyRelative("m_BorderMipMap");
            m_MipMapsPreserveCoverage = textureImporterSettingsSP.FindPropertyRelative("m_MipMapsPreserveCoverage");
            m_AlphaTestReferenceValue = textureImporterSettingsSP.FindPropertyRelative("m_AlphaTestReferenceValue");
            m_MipMapFadeDistanceStart = textureImporterSettingsSP.FindPropertyRelative("m_MipMapFadeDistanceStart");
            m_MipMapFadeDistanceEnd = textureImporterSettingsSP.FindPropertyRelative("m_MipMapFadeDistanceEnd");
            m_AlphaIsTransparency = textureImporterSettingsSP.FindPropertyRelative("m_AlphaIsTransparency");
            m_FilterMode = textureImporterSettingsSP.FindPropertyRelative("m_FilterMode");
            m_Aniso = textureImporterSettingsSP.FindPropertyRelative("m_Aniso");
            m_WrapU = textureImporterSettingsSP.FindPropertyRelative("m_WrapU");
            m_WrapV = textureImporterSettingsSP.FindPropertyRelative("m_WrapV");
            m_WrapW = textureImporterSettingsSP.FindPropertyRelative("m_WrapW");

            var textureWidth = serializedObject.FindProperty("m_TextureActualWidth");
            var textureHeight = serializedObject.FindProperty("m_TextureActualHeight");
            m_IsPOT = Mathf.IsPowerOfTwo(textureWidth.intValue) && Mathf.IsPowerOfTwo(textureHeight.intValue);


            var advanceGUIAction = new Action[]
            {
                ColorSpaceGUI,
                AlphaHandlingGUI,
                POTScaleGUI,
                ReadableGUI,
                MipMapGUI
            };
            m_AdvanceInspectorGUI.Add(TextureImporterType.Sprite, advanceGUIAction);

            advanceGUIAction = new Action[]
            {
                POTScaleGUI,
                ReadableGUI,
                MipMapGUI
            };
            m_AdvanceInspectorGUI.Add(TextureImporterType.Default, advanceGUIAction);
            LoadPlatformSettings();
            m_TexturePlatformSettingsHelper = new TexturePlatformSettingsHelper(this);
        }

        /// <summary>
        /// Implementation of AssetImporterEditor.OnInspectorGUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (s_Styles == null)
                s_Styles = new Styles();

            EditorGUI.showMixedValue = m_TextureType.hasMultipleDifferentValues;
            m_TextureType.intValue = EditorGUILayout.IntPopup(s_Styles.textureTypeTitle, m_TextureType.intValue, s_Styles.textureTypeOptions, s_Styles.textureTypeValues);
            EditorGUI.showMixedValue = false;

            switch ((TextureImporterType)m_TextureType.intValue)
            {
                case TextureImporterType.Sprite:
                    DoSpriteInspector();
                    break;
                case TextureImporterType.Default:
                    DoTextureDefaultInspector();
                    break;
                default:
                    Debug.LogWarning("We only support Default or Sprite texture type for now. Texture type is set to default.");
                    m_TextureType.intValue = (int)TextureImporterType.Default;
                    break;
            }

            DoAdvanceInspector();
            CommonTextureSettingsGUI();
            GUILayout.Space(10);
            DoPlatformSettings();
            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }

        /// <summary>
        /// Implementation of AssetImporterEditor.Apply
        /// </summary>
        protected override void Apply()
        {
            FileStream fileStream = new FileStream(((AssetImporter)target).assetPath, FileMode.Open, FileAccess.Read);
            var doc = PaintDotNet.Data.PhotoshopFileType.PsdLoad.Load(fileStream, ELoadFlag.Header | ELoadFlag.ColorMode);

            PSDApplyEvent evt = new PSDApplyEvent()
            {
                instance_id = target.GetInstanceID(),
                texture_type = m_TextureType.intValue,
                sprite_mode = m_SpriteMode.intValue,
                mosaic_layer = m_MosaicLayers.boolValue,
                import_hidden_layer = m_ImportHiddenLayers.boolValue,
                character_mode = m_CharacterMode.boolValue,
                generate_go_hierarchy = m_GenerateGOHierarchy.boolValue,
                reslice_from_layer = m_ResliceFromLayer.boolValue,
                is_character_rigged = IsCharacterRigged(),
                is_psd = IsPSD(doc),
                color_mode = FileColorMode(doc)
            };
            doc.Cleanup();
            AnalyticFactory.analytics.SendApplyEvent(evt);
            m_TexturePlatformSettingsHelper.Apply();
            base.Apply();
            Selection.activeObject = null;
            Unsupported.SceneTrackerFlushDirty();
            PSDImportPostProcessor.currentApplyAssetPath = ((PSDImporter) target).assetPath;
        }

        static bool IsPSD(PsdFile doc)
        {
            return !doc.IsLargeDocument;
        }

        static PsdColorMode FileColorMode(PsdFile doc)
        {
            return doc.ColorMode;
        }

        bool IsCharacterRigged()
        {
            var importer = target as PSDImporter;
            if (importer != null)
            {
                var characterProvider = importer.GetDataProvider<ICharacterDataProvider>();
                var meshDataProvider = importer.GetDataProvider<ISpriteMeshDataProvider>();
                if (characterProvider != null && meshDataProvider != null)
                {
                    var character = characterProvider.GetCharacterData();
                    foreach (var parts in character.parts)
                    {
                        var vert = meshDataProvider.GetVertices(new GUID(parts.spriteId));
                        var indices = meshDataProvider.GetIndices(new GUID(parts.spriteId));
                        if (parts.bones != null && parts.bones.Length > 0 &&
                            vert != null && vert.Length > 0 &&
                            indices != null && indices.Length > 0)
                            return true;
                    }
                }
            }
            return false;
        }

        Dictionary<string, List<TextureImporterPlatformSettings>> m_PlatfromSettings = new Dictionary<string, List<TextureImporterPlatformSettings>>();
        void LoadPlatformSettings()
        {
            foreach (var t in targets)
            {
                var importer = ((PSDImporter)t);
                var importerPlatformSettings = importer.GetAllPlatformSettings();
                for (int i = 0; i < importerPlatformSettings.Length; ++i)
                {
                    var tip = importerPlatformSettings[i];
                    List<TextureImporterPlatformSettings> platformSettings = null;
                    m_PlatfromSettings.TryGetValue(tip.name, out platformSettings);
                    if (platformSettings == null)
                    {
                        platformSettings = new List<TextureImporterPlatformSettings>();
                        m_PlatfromSettings.Add(tip.name, platformSettings);
                    }
                    platformSettings.Add(tip);
                }
            }
        }

        void StorePlatformSettings()
        {
            var platformSettingsSP = serializedObject.FindProperty("m_PlatformSettings");
            platformSettingsSP.ClearArray();
            foreach (var keyValue in m_PlatfromSettings)
            {
                if (!keyValue.Value[0].overridden)
                    continue;

                SerializedProperty platformSettingSP = null;
                for (int i = 0; i < platformSettingsSP.arraySize; ++i)
                {
                    var sp = platformSettingsSP.GetArrayElementAtIndex(i);
                    if (sp.FindPropertyRelative("m_Name").stringValue == keyValue.Key)
                        platformSettingSP = sp;
                }
                if (platformSettingSP == null)
                {
                    platformSettingsSP.InsertArrayElementAtIndex(platformSettingsSP.arraySize);
                    platformSettingSP = platformSettingsSP.GetArrayElementAtIndex(platformSettingsSP.arraySize - 1);
                }

                var tip = keyValue.Value[0];
                platformSettingSP.FindPropertyRelative("m_Name").stringValue = tip.name;
                platformSettingSP.FindPropertyRelative("m_Overridden").intValue = tip.overridden ? 1 : 0;
                platformSettingSP.FindPropertyRelative("m_MaxTextureSize").intValue = tip.maxTextureSize;
                platformSettingSP.FindPropertyRelative("m_ResizeAlgorithm").intValue = (int)tip.resizeAlgorithm;
                platformSettingSP.FindPropertyRelative("m_TextureFormat").intValue = (int)tip.format;
                platformSettingSP.FindPropertyRelative("m_TextureCompression").intValue = (int)tip.textureCompression;
                platformSettingSP.FindPropertyRelative("m_CompressionQuality").intValue = tip.compressionQuality;
                platformSettingSP.FindPropertyRelative("m_CrunchedCompression").intValue = tip.crunchedCompression ? 1 : 0;
                platformSettingSP.FindPropertyRelative("m_AllowsAlphaSplitting").intValue = tip.allowsAlphaSplitting ? 1 : 0;
            }
        }

        void DoPlatformSettings()
        {
            m_TexturePlatformSettingsHelper.ShowPlatformSpecificSettings();
        }

        void DoAdvanceInspector()
        {
            if (!m_TextureType.hasMultipleDifferentValues)
            {
                if (m_AdvanceInspectorGUI.ContainsKey((TextureImporterType)m_TextureType.intValue))
                {
                    EditorGUILayout.Space();

                    m_ShowAdvanced = EditorGUILayout.Foldout(m_ShowAdvanced, s_Styles.showAdvanced, true);
                    if (m_ShowAdvanced)
                    {
                        foreach (var action in m_AdvanceInspectorGUI[(TextureImporterType)m_TextureType.intValue])
                        {
                            action();
                        }
                    }
                }
            }
            EditorGUILayout.Space();
        }

        void CommonTextureSettingsGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Wrap mode
            bool isVolume = false;
            WrapModePopup(m_WrapU, m_WrapV, m_WrapW, isVolume, ref m_ShowPerAxisWrapModes);


            // Display warning about repeat wrap mode on restricted npot emulation
            if (m_NPOTScale.intValue == (int)TextureImporterNPOTScale.None &&
                (m_WrapU.intValue == (int)TextureWrapMode.Repeat || m_WrapV.intValue == (int)TextureWrapMode.Repeat) &&
                !InternalEditorBridge.DoesHardwareSupportsFullNPOT())
            {
                bool displayWarning = false;
                foreach (var target in targets)
                {
                    var imp = (PSDImporter)target;
                    int w = imp.textureActualWidth;
                    int h = imp.textureActualHeight;
                    if (!Mathf.IsPowerOfTwo(w) || !Mathf.IsPowerOfTwo(h))
                    {
                        displayWarning = true;
                        break;
                    }
                }

                if (displayWarning)
                {
                    EditorGUILayout.HelpBox(s_Styles.warpNotSupportWarning.text, MessageType.Warning, true);
                }
            }

            // Filter mode
            EditorGUI.showMixedValue = m_FilterMode.hasMultipleDifferentValues;
            FilterMode filter = (FilterMode)m_FilterMode.intValue;
            if ((int)filter == -1)
            {
                if (m_FadeOut.intValue > 0 || m_ConvertToNormalMap.intValue > 0)
                    filter = FilterMode.Trilinear;
                else
                    filter = FilterMode.Bilinear;
            }
            filter = (FilterMode)EditorGUILayout.IntPopup(s_Styles.filterMode, (int)filter, s_Styles.filterModeOptions, m_FilterModeOptions);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                m_FilterMode.intValue = (int)filter;

            // Aniso
            bool showAniso = (FilterMode)m_FilterMode.intValue != FilterMode.Point
                && m_EnableMipMap.intValue > 0
                && (TextureImporterShape)m_TextureShape.intValue != TextureImporterShape.TextureCube;
            using (new EditorGUI.DisabledScope(!showAniso))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = m_Aniso.hasMultipleDifferentValues;
                int aniso = m_Aniso.intValue;
                if (aniso == -1)
                    aniso = 1;
                aniso = EditorGUILayout.IntSlider(s_Styles.anisoLevelLabel, aniso, 0, 16);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                    m_Aniso.intValue = aniso;

                if (aniso > 1)
                {
                    if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable)
                        EditorGUILayout.HelpBox(s_Styles.anisotropicDisableInfo.text, MessageType.Info);
                    else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
                        EditorGUILayout.HelpBox(s_Styles.anisotropicForceEnableInfo.text, MessageType.Info);
                }
            }
        }

        private static bool IsAnyTextureObjectUsingPerAxisWrapMode(UnityEngine.Object[] objects, bool isVolumeTexture)
        {
            foreach (var o in objects)
            {
                int u = 0, v = 0, w = 0;
                // the objects can be Textures themselves, or texture-related importers
                if (o is Texture)
                {
                    var ti = (Texture)o;
                    u = (int)ti.wrapModeU;
                    v = (int)ti.wrapModeV;
                    w = (int)ti.wrapModeW;
                }
                if (o is TextureImporter)
                {
                    var ti = (TextureImporter)o;
                    u = (int)ti.wrapModeU;
                    v = (int)ti.wrapModeV;
                    w = (int)ti.wrapModeW;
                }
                if (o is IHVImageFormatImporter)
                {
                    var ti = (IHVImageFormatImporter)o;
                    u = (int)ti.wrapModeU;
                    v = (int)ti.wrapModeV;
                    w = (int)ti.wrapModeW;
                }
                u = Mathf.Max(0, u);
                v = Mathf.Max(0, v);
                w = Mathf.Max(0, w);
                if (u != v)
                {
                    return true;
                }
                if (isVolumeTexture)
                {
                    if (u != w || v != w)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // showPerAxisWrapModes is state of whether "Per-Axis" mode should be active in the main dropdown.
        // It is set automatically if wrap modes in UVW are different, or if user explicitly picks "Per-Axis" option -- when that one is picked,
        // then it should stay true even if UVW wrap modes will initially be the same.
        //
        // Note: W wrapping mode is only shown when isVolumeTexture is true.
        internal static void WrapModePopup(SerializedProperty wrapU, SerializedProperty wrapV, SerializedProperty wrapW, bool isVolumeTexture, ref bool showPerAxisWrapModes)
        {
            if (s_Styles == null)
                s_Styles = new Styles();

            // In texture importer settings, serialized properties for things like wrap modes can contain -1;
            // that seems to indicate "use defaults, user has not changed them to anything" but not totally sure.
            // Show them as Repeat wrap modes in the popups.
            var wu = (TextureWrapMode)Mathf.Max(wrapU.intValue, 0);
            var wv = (TextureWrapMode)Mathf.Max(wrapV.intValue, 0);
            var ww = (TextureWrapMode)Mathf.Max(wrapW.intValue, 0);

            // automatically go into per-axis mode if values are already different
            if (wu != wv)
                showPerAxisWrapModes = true;
            if (isVolumeTexture)
            {
                if (wu != ww || wv != ww)
                    showPerAxisWrapModes = true;
            }

            // It's not possible to determine whether any single texture in the whole selection is using per-axis wrap modes
            // just from SerializedProperty values. They can only tell if "some values in whole selection are different" (e.g.
            // wrap value on U axis is not the same among all textures), and can return value of "some" object in the selection
            // (typically based on object loading order). So in order for more intuitive behavior with multi-selection,
            // we go over the actual objects when there's >1 object selected and some wrap modes are different.
            if (!showPerAxisWrapModes)
            {
                if (wrapU.hasMultipleDifferentValues || wrapV.hasMultipleDifferentValues || (isVolumeTexture && wrapW.hasMultipleDifferentValues))
                {
                    if (IsAnyTextureObjectUsingPerAxisWrapMode(wrapU.serializedObject.targetObjects, isVolumeTexture))
                    {
                        showPerAxisWrapModes = true;
                    }
                }
            }

            int value = showPerAxisWrapModes ? -1 : (int)wu;

            // main wrap mode popup
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = !showPerAxisWrapModes && (wrapU.hasMultipleDifferentValues || wrapV.hasMultipleDifferentValues || (isVolumeTexture && wrapW.hasMultipleDifferentValues));
            value = EditorGUILayout.IntPopup(s_Styles.wrapModeLabel, value, s_Styles.wrapModeContents, s_Styles.wrapModeValues);
            if (EditorGUI.EndChangeCheck() && value != -1)
            {
                // assign the same wrap mode to all axes, and hide per-axis popups
                wrapU.intValue = value;
                wrapV.intValue = value;
                wrapW.intValue = value;
                showPerAxisWrapModes = false;
            }

            // show per-axis popups if needed
            if (value == -1)
            {
                showPerAxisWrapModes = true;
                EditorGUI.indentLevel++;
                WrapModeAxisPopup(s_Styles.wrapU, wrapU);
                WrapModeAxisPopup(s_Styles.wrapV, wrapV);
                if (isVolumeTexture)
                {
                    WrapModeAxisPopup(s_Styles.wrapW, wrapW);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.showMixedValue = false;
        }

        static void WrapModeAxisPopup(GUIContent label, SerializedProperty wrapProperty)
        {
            // In texture importer settings, serialized properties for wrap modes can contain -1, which means "use default".
            var wrap = (TextureWrapMode)Mathf.Max(wrapProperty.intValue, 0);
            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(rect, label, wrapProperty);
            wrap = (TextureWrapMode)EditorGUI.EnumPopup(rect, label, wrap);
            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck())
            {
                wrapProperty.intValue = (int)wrap;
            }
        }

        void DoWrapModePopup()
        {
            WrapModePopup(m_WrapU, m_WrapV, m_WrapW, IsVolume(), ref m_ShowPerAxisWrapModes);
        }

        bool IsVolume()
        {
            var t = target as Texture;
            return t != null && t.dimension == UnityEngine.Rendering.TextureDimension.Tex3D;
        }

        void DoSpriteInspector()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.IntPopup(m_SpriteMode, s_Styles.spriteModeOptions, new[] { 1, 2, 3 }, s_Styles.spriteMode);

            // Ensure that PropertyField focus will be cleared when we change spriteMode.
            if (EditorGUI.EndChangeCheck())
            {
                GUIUtility.keyboardControl = 0;
            }

            EditorGUI.indentLevel++;

            // Show generic attributes
            if (m_SpriteMode.intValue != 0)
            {
                EditorGUILayout.PropertyField(m_SpritePixelsToUnits, s_Styles.spritePixelsPerUnit);

                if (m_SpriteMode.intValue != (int)SpriteImportMode.Polygon && !m_SpriteMode.hasMultipleDifferentValues)
                {
                    EditorGUILayout.IntPopup(m_SpriteMeshType, s_Styles.spriteMeshTypeOptions, new[] { 0, 1 }, s_Styles.spriteMeshType);
                }

                EditorGUILayout.IntSlider(m_SpriteExtrude, 0, 32, s_Styles.spriteExtrude);

                if (m_SpriteMode.intValue == 1)
                {
                    EditorGUILayout.IntPopup(m_Alignment, s_Styles.spriteAlignmentOptions, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, s_Styles.spriteAlignment);

                    if (m_Alignment.intValue == (int)SpriteAlignment.Custom)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(m_SpritePivot, new GUIContent());
                        GUILayout.EndHorizontal();
                    }
                }
            }

            EditorGUILayout.PropertyField(m_ImportHiddenLayers, s_Styles.importHiddenLayer);
            if (m_SpriteMode.intValue == (int)SpriteImportMode.Multiple && !m_SpriteMode.hasMultipleDifferentValues)
            {
                EditorGUILayout.PropertyField(m_MosaicLayers, s_Styles.mosaicLayers);
                using (new EditorGUI.DisabledScope(!m_MosaicLayers.boolValue))
                {
                    EditorGUILayout.PropertyField(m_CharacterMode, s_Styles.characterMode);
                    using (new EditorGUI.DisabledScope(!m_CharacterMode.boolValue))
                    {
                        EditorGUILayout.PropertyField(m_GenerateGOHierarchy, s_Styles.generateGOHierarchy);
                        EditorGUILayout.IntPopup(m_DocumentAlignment, s_Styles.spriteAlignmentOptions, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, s_Styles.characterAlignment);
                        if (m_DocumentAlignment.intValue == (int)SpriteAlignment.Custom)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(EditorGUIUtility.labelWidth);
                            EditorGUILayout.PropertyField(m_DocumentPivot, new GUIContent());
                            GUILayout.EndHorizontal();
                        }
                        //EditorGUILayout.PropertyField(m_PaperDollMode, s_Styles.paperDollMode);
                    }


                    EditorGUILayout.PropertyField(m_ResliceFromLayer, s_Styles.resliceFromLayer);
                    if (m_ResliceFromLayer.boolValue)
                    {
                        EditorGUILayout.HelpBox(s_Styles.resliceFromLayerWarning.text, MessageType.Info, true);
                    }
                }
                m_ShowExperimental = EditorGUILayout.Foldout(m_ShowExperimental, s_Styles.experimental, true);
                if (m_ShowExperimental)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_KeepDupilcateSpriteName, s_Styles.keepDuplicateSpriteName);
                    EditorGUI.indentLevel--;
                }
            }

            using (new EditorGUI.DisabledScope(targets.Length != 1))
            {
                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();
                if (GUILayout.Button(s_Styles.spriteEditorButtonLabel))
                {
                    if (HasModified())
                    {
                        // To ensure Sprite Editor Window to have the latest texture import setting,
                        // We must applied those modified values first.
                        string dialogText = string.Format(s_Styles.unappliedSettingsDialogContent.text, ((AssetImporter)target).assetPath);
                        if (EditorUtility.DisplayDialog(s_Styles.unappliedSettingsDialogTitle.text,
                            dialogText, s_Styles.applyButtonLabel.text, s_Styles.revertButtonLabel.text))
                        {
                            ApplyAndImport();
                            InternalEditorBridge.ShowSpriteEditorWindow(this.assetTarget);

                            // We reimported the asset which destroyed the editor, so we can't keep running the UI here.
                            GUIUtility.ExitGUI();
                        }
                    }
                    else
                    {
                        InternalEditorBridge.ShowSpriteEditorWindow(this.assetTarget);
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        void DoTextureDefaultInspector()
        {
            ColorSpaceGUI();
            AlphaHandlingGUI();
        }

        void ColorSpaceGUI()
        {
            ToggleFromInt(m_sRGBTexture, s_Styles.sRGBTexture);
        }

        void POTScaleGUI()
        {
            using (new EditorGUI.DisabledScope(m_IsPOT || m_TextureType.intValue == (int)TextureImporterType.Sprite))
            {
                EnumPopup(m_NPOTScale, typeof(TextureImporterNPOTScale), s_Styles.npot);
            }
        }

        void ReadableGUI()
        {
            ToggleFromInt(m_IsReadable, s_Styles.readWrite);
        }

        void AlphaHandlingGUI()
        {
            EditorGUI.showMixedValue = m_AlphaSource.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int newAlphaUsage = EditorGUILayout.IntPopup(s_Styles.alphaSource, m_AlphaSource.intValue, s_Styles.alphaSourceOptions, s_Styles.alphaSourceValues);

            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                m_AlphaSource.intValue = newAlphaUsage;
            }

            bool showAlphaIsTransparency = (TextureImporterAlphaSource)m_AlphaSource.intValue != TextureImporterAlphaSource.None;
            using (new EditorGUI.DisabledScope(!showAlphaIsTransparency))
            {
                ToggleFromInt(m_AlphaIsTransparency, s_Styles.alphaIsTransparency);
            }
        }

        void MipMapGUI()
        {
            ToggleFromInt(m_EnableMipMap, s_Styles.generateMipMaps);

            if (m_EnableMipMap.boolValue && !m_EnableMipMap.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                ToggleFromInt(m_BorderMipMap, s_Styles.borderMipMaps);
                EditorGUILayout.Popup(s_Styles.mipMapFilter, m_MipMapMode.intValue, s_Styles.mipMapFilterOptions);

                ToggleFromInt(m_MipMapsPreserveCoverage, s_Styles.mipMapsPreserveCoverage);
                if (m_MipMapsPreserveCoverage.intValue != 0 && !m_MipMapsPreserveCoverage.hasMultipleDifferentValues)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_AlphaTestReferenceValue, s_Styles.alphaTestReferenceValue);
                    EditorGUI.indentLevel--;
                }

                // Mipmap fadeout
                ToggleFromInt(m_FadeOut, s_Styles.mipmapFadeOutToggle);
                if (m_FadeOut.intValue > 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    float min = m_MipMapFadeDistanceStart.intValue;
                    float max = m_MipMapFadeDistanceEnd.intValue;
                    EditorGUILayout.MinMaxSlider(s_Styles.mipmapFadeOut, ref min, ref max, 0, 10);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_MipMapFadeDistanceStart.intValue = Mathf.RoundToInt(min);
                        m_MipMapFadeDistanceEnd.intValue = Mathf.RoundToInt(max);
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        void ToggleFromInt(SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            int value = EditorGUILayout.Toggle(label, property.intValue > 0) ? 1 : 0;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                property.intValue = value;
        }

        void EnumPopup(SerializedProperty property, System.Type type, GUIContent label)
        {
            EditorGUILayout.IntPopup(label.text, property.intValue,
                System.Enum.GetNames(type),
                System.Enum.GetValues(type) as int[]);
        }

        void ExportMosaicTexture()
        {
            var assetPath = ((AssetImporter)target).assetPath;
            var texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture2D == null)
                return;
            if (!texture2D.isReadable)
                texture2D = InternalEditorBridge.CreateTemporaryDuplicate(texture2D, texture2D.width, texture2D.height);
            var pixelData = texture2D.GetPixels();
            texture2D = new Texture2D(texture2D.width, texture2D.height);
            texture2D.SetPixels(pixelData);
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
            var filePath = Path.GetDirectoryName(assetPath);
            var savePath = Path.Combine(filePath, fileName + ".png");
            File.WriteAllBytes(savePath, bytes);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Implementation of AssetImporterEditor.ResetValues.
        /// </summary>
        protected override void ResetValues()
        {
            base.ResetValues();
            LoadPlatformSettings();
            m_TexturePlatformSettingsHelper = new TexturePlatformSettingsHelper(this);
        }

        /// <summary>
        /// Implementation of ITexturePlatformSettingsDataProvider.GetTargetCount.
        /// </summary>
        /// <returns>Returns the number of selected targets.</returns>
        public int GetTargetCount()
        {
            return targets.Length;
        }

        /// <summary>
        /// ITexturePlatformSettingsDataProvider.GetPlatformTextureSettings.
        /// </summary>
        /// <param name="i">Selected target index.</param>
        /// <param name="name">Name of the platform.</param>
        /// <returns>TextureImporterPlatformSettings for the given platform name and selected target index.</returns>
        public TextureImporterPlatformSettings GetPlatformTextureSettings(int i, string name)
        {
            if(m_PlatfromSettings.ContainsKey(name))
                if(m_PlatfromSettings[name].Count > i)
                    return m_PlatfromSettings[name][i];
            return new TextureImporterPlatformSettings()
            {
                name = name,
                overridden = false
            };
        }


        /// <summary>
        /// Implementation of ITexturePlatformSettingsDataProvider.ShowPresetSettings.
        /// </summary>
        /// <returns>True if valid asset is selected, false otherwiser.</returns>
        public bool ShowPresetSettings()
        {
            return assetTarget == null;
        }

        /// <summary>
        /// Implementation of ITexturePlatformSettingsDataProvider.DoesSourceTextureHaveAlpha.
        /// </summary>
        /// <param name="i">Index to selected target.</param>
        /// <returns>Always returns true since importer deals with source file that has alpha.</returns>
        public bool DoesSourceTextureHaveAlpha(int i)
        {
            return true;
        }

        /// <summary>
        /// Implementation of ITexturePlatformSettingsDataProvider.IsSourceTextureHDR.
        /// </summary>
        /// <param name="i">Index to selected target.</param>
        /// <returns>Always returns false since importer does not handle HDR textures.</returns>
        public bool IsSourceTextureHDR(int i)
        {
            return false;
        }

        /// <summary>
        /// Implementation of ITexturePlatformSettingsDataProvider.SetPlatformTextureSettings.
        /// </summary>
        /// <param name="i">Selected target index.</param>
        /// <param name="platformSettings">TextureImporterPlatformSettings to apply to target.</param>
        public void SetPlatformTextureSettings(int i, TextureImporterPlatformSettings platformSettings)
        {
            var psdImporter = ((PSDImporter)targets[i]);
            psdImporter.SetPlatformTextureSettings(platformSettings);
            psdImporter.Apply();
        }

        /// <summary>
        /// Implementation of ITexturePlatformSettingsDataProvider.GetImporterSettings.
        /// </summary>
        /// <param name="i">Selected target index.</param>
        /// <param name="settings">TextureImporterPlatformSettings reference for data retrieval.</param>
        public void GetImporterSettings(int i, TextureImporterSettings settings)
        {
            ((PSDImporter)targets[i]).ReadTextureSettings(settings);
            // Get settings that have been changed in the inspector
            GetSerializedPropertySettings(settings);
        }

        internal TextureImporterSettings GetSerializedPropertySettings(TextureImporterSettings settings)
        {
            if (!m_AlphaSource.hasMultipleDifferentValues)
                settings.alphaSource = (TextureImporterAlphaSource)m_AlphaSource.intValue;

            if (!m_ConvertToNormalMap.hasMultipleDifferentValues)
                settings.convertToNormalMap = m_ConvertToNormalMap.intValue > 0;

            if (!m_BorderMipMap.hasMultipleDifferentValues)
                settings.borderMipmap = m_BorderMipMap.intValue > 0;

            if (!m_MipMapsPreserveCoverage.hasMultipleDifferentValues)
                settings.mipMapsPreserveCoverage = m_MipMapsPreserveCoverage.intValue > 0;

            if (!m_AlphaTestReferenceValue.hasMultipleDifferentValues)
                settings.alphaTestReferenceValue = m_AlphaTestReferenceValue.floatValue;

            if (!m_NPOTScale.hasMultipleDifferentValues)
                settings.npotScale = (TextureImporterNPOTScale)m_NPOTScale.intValue;

            if (!m_IsReadable.hasMultipleDifferentValues)
                settings.readable = m_IsReadable.intValue > 0;

            if (!m_sRGBTexture.hasMultipleDifferentValues)
                settings.sRGBTexture = m_sRGBTexture.intValue > 0;

            if (!m_EnableMipMap.hasMultipleDifferentValues)
                settings.mipmapEnabled = m_EnableMipMap.intValue > 0;

            if (!m_MipMapMode.hasMultipleDifferentValues)
                settings.mipmapFilter = (TextureImporterMipFilter)m_MipMapMode.intValue;

            if (!m_FadeOut.hasMultipleDifferentValues)
                settings.fadeOut = m_FadeOut.intValue > 0;

            if (!m_MipMapFadeDistanceStart.hasMultipleDifferentValues)
                settings.mipmapFadeDistanceStart = m_MipMapFadeDistanceStart.intValue;

            if (!m_MipMapFadeDistanceEnd.hasMultipleDifferentValues)
                settings.mipmapFadeDistanceEnd = m_MipMapFadeDistanceEnd.intValue;

            if (!m_SpriteMode.hasMultipleDifferentValues)
                settings.spriteMode = m_SpriteMode.intValue;

            if (!m_SpritePixelsToUnits.hasMultipleDifferentValues)
                settings.spritePixelsPerUnit = m_SpritePixelsToUnits.floatValue;

            if (!m_SpriteExtrude.hasMultipleDifferentValues)
                settings.spriteExtrude = (uint)m_SpriteExtrude.intValue;

            if (!m_SpriteMeshType.hasMultipleDifferentValues)
                settings.spriteMeshType = (SpriteMeshType)m_SpriteMeshType.intValue;

            if (!m_Alignment.hasMultipleDifferentValues)
                settings.spriteAlignment = m_Alignment.intValue;

            if (!m_SpritePivot.hasMultipleDifferentValues)
                settings.spritePivot = m_SpritePivot.vector2Value;

            if (!m_WrapU.hasMultipleDifferentValues)
                settings.wrapModeU = (TextureWrapMode)m_WrapU.intValue;
            if (!m_WrapV.hasMultipleDifferentValues)
                settings.wrapModeU = (TextureWrapMode)m_WrapV.intValue;
            if (!m_WrapW.hasMultipleDifferentValues)
                settings.wrapModeU = (TextureWrapMode)m_WrapW.intValue;

            if (!m_FilterMode.hasMultipleDifferentValues)
                settings.filterMode = (FilterMode)m_FilterMode.intValue;

            if (!m_Aniso.hasMultipleDifferentValues)
                settings.aniso = m_Aniso.intValue;


            if (!m_AlphaIsTransparency.hasMultipleDifferentValues)
                settings.alphaIsTransparency = m_AlphaIsTransparency.intValue > 0;

            if (!m_TextureType.hasMultipleDifferentValues)
                settings.textureType = (TextureImporterType)m_TextureType.intValue;

            if (!m_TextureShape.hasMultipleDifferentValues)
                settings.textureShape = (TextureImporterShape)m_TextureShape.intValue;

            return settings;
        }
        /// <summary>
        /// Override of AssetImporterEditor.showImportedObject
        /// The property always returns false so that imported objects does not show up in the Inspector.
        /// </summary>
        /// <returns>false</returns>
        public override bool showImportedObject
        {
            get { return false; }
        }
        
        /// <summary>
        /// Implementation of ITexturePlatformSettingsDataProvider.textureTypeHasMultipleDifferentValues.
        /// </summary>
        public bool textureTypeHasMultipleDifferentValues
        {
           get { return m_TextureType.hasMultipleDifferentValues; }
        }

        /// <summary>
        /// Implementation of ITexturePlatformSettingsDataProvider.textureType.
        /// </summary>
        public TextureImporterType textureType
        {
           get { return (TextureImporterType)m_TextureType.intValue; }
        }

        /// <summary>
        /// Implementation of ITexturePlatformSettingsDataProvider.spriteImportMode.
        /// </summary>
        public SpriteImportMode spriteImportMode
        {
            get { return (SpriteImportMode)m_SpriteMode.intValue; }
        }

        /// <summary>
        /// Override of AssetImporterEditor.HasModified.
        /// </summary>
        /// <returns>Returns True if has modified data. False otherwise.</returns>
        public override bool HasModified()
        {
            if (base.HasModified())
                return true;

            return m_TexturePlatformSettingsHelper.HasModified();
        }

        internal class Styles
        {
            public readonly GUIContent textureTypeTitle = new GUIContent("Texture Type", "What will this texture be used for?");
            public readonly GUIContent[] textureTypeOptions =
            {
                new GUIContent("Default", "Texture is a normal image such as a diffuse texture or other."),
                new GUIContent("Sprite (2D and UI)", "Texture is used for a sprite."),
            };
            public readonly int[] textureTypeValues =
            {
                (int)TextureImporterType.Default,
                (int)TextureImporterType.Sprite,
            };

            public readonly GUIContent textureShape = new GUIContent("Texture Shape", "What shape is this texture?");
            private readonly GUIContent textureShape2D = new GUIContent("2D, Texture is 2D.");
            private readonly  GUIContent textureShapeCube = new GUIContent("Cube", "Texture is a Cubemap.");
            public readonly Dictionary<TextureImporterShape, GUIContent[]> textureShapeOptionsDictionnary = new Dictionary<TextureImporterShape, GUIContent[]>();
            public readonly Dictionary<TextureImporterShape, int[]> textureShapeValuesDictionnary = new Dictionary<TextureImporterShape, int[]>();


            public readonly GUIContent filterMode = new GUIContent("Filter Mode");
            public readonly GUIContent[] filterModeOptions =
            {
                new GUIContent("Point (no filter)"),
                new GUIContent("Bilinear"),
                new GUIContent("Trilinear")
            };

            public readonly GUIContent textureFormat = new GUIContent("Format");

            public readonly GUIContent defaultPlatform = new GUIContent("Default");
            public readonly GUIContent mipmapFadeOutToggle = new GUIContent("Fadeout Mip Maps");
            public readonly GUIContent mipmapFadeOut = new GUIContent("Fade Range");
            public readonly GUIContent readWrite = new GUIContent("Read/Write Enabled", "Enable to be able to access the raw pixel data from code.");

            public readonly GUIContent alphaSource = new GUIContent("Alpha Source", "How is the alpha generated for the imported texture.");
            public readonly GUIContent[] alphaSourceOptions =
            {
                new GUIContent("None", "No Alpha will be used."),
                new GUIContent("Input Texture Alpha", "Use Alpha from the input texture if one is provided."),
                new GUIContent("From Gray Scale", "Generate Alpha from image gray scale."),
            };
            public readonly int[] alphaSourceValues =
            {
                (int)TextureImporterAlphaSource.None,
                (int)TextureImporterAlphaSource.FromInput,
                (int)TextureImporterAlphaSource.FromGrayScale,
            };

            public readonly GUIContent generateMipMaps = new GUIContent("Generate Mip Maps");
            public readonly GUIContent sRGBTexture = new GUIContent("sRGB (Color Texture)", "Texture content is stored in gamma space. Non-HDR color textures should enable this flag (except if used for IMGUI).");
            public readonly GUIContent borderMipMaps = new GUIContent("Border Mip Maps");
            public readonly GUIContent mipMapsPreserveCoverage = new GUIContent("Mip Maps Preserve Coverage", "The alpha channel of generated Mip Maps will preserve coverage during the alpha test.");
            public readonly GUIContent alphaTestReferenceValue = new GUIContent("Alpha Cutoff Value", "The reference value used during the alpha test. Controls Mip Map coverage.");
            public readonly GUIContent mipMapFilter = new GUIContent("Mip Map Filtering");
            public readonly GUIContent[] mipMapFilterOptions =
            {
                new GUIContent("Box"),
                new GUIContent("Kaiser"),
            };
            public readonly GUIContent npot = new GUIContent("Non Power of 2", "How non-power-of-two textures are scaled on import.");

            public readonly GUIContent compressionQuality = new GUIContent("Compressor Quality");
            public readonly GUIContent compressionQualitySlider = new GUIContent("Compressor Quality", "Use the slider to adjust compression quality from 0 (Fastest) to 100 (Best)");
            public readonly GUIContent[] mobileCompressionQualityOptions =
            {
                new GUIContent("Fast"),
                new GUIContent("Normal"),
                new GUIContent("Best")
            };

            public readonly GUIContent spriteMode = new GUIContent("Sprite Mode");
            public readonly GUIContent[] spriteModeOptions =
            {
                new GUIContent("Single"),
                new GUIContent("Multiple"),
                new GUIContent("Polygon"),
            };
            public readonly GUIContent[] spriteMeshTypeOptions =
            {
                new GUIContent("Full Rect"),
                new GUIContent("Tight"),
            };

            public readonly GUIContent spritePackingTag = new GUIContent("Packing Tag", "Tag for the Sprite Packing system.");
            public readonly GUIContent spritePixelsPerUnit = new GUIContent("Pixels Per Unit", "How many pixels in the sprite correspond to one unit in the world.");
            public readonly GUIContent spriteExtrude = new GUIContent("Extrude Edges", "How much empty area to leave around the sprite in the generated mesh.");
            public readonly GUIContent spriteMeshType = new GUIContent("Mesh Type", "Type of sprite mesh to generate.");
            public readonly GUIContent spriteAlignment = new GUIContent("Pivot", "Sprite pivot point in its local space. May be used for syncing animation frames of different sizes.");
            public readonly GUIContent characterAlignment = new GUIContent("Pivot", "Character pivot point in its local space using normalized value i.e. 0 - 1");

            public readonly GUIContent[] spriteAlignmentOptions =
            {
                new GUIContent("Center"),
                new GUIContent("Top Left"),
                new GUIContent("Top"),
                new GUIContent("Top Right"),
                new GUIContent("Left"),
                new GUIContent("Right"),
                new GUIContent("Bottom Left"),
                new GUIContent("Bottom"),
                new GUIContent("Bottom Right"),
                new GUIContent("Custom"),
            };

            public readonly GUIContent warpNotSupportWarning = new GUIContent("Graphics device doesn't support Repeat wrap mode on NPOT textures. Falling back to Clamp.");
            public readonly GUIContent anisoLevelLabel = new GUIContent("Aniso Level");
            public readonly GUIContent anisotropicDisableInfo = new GUIContent("Anisotropic filtering is disabled for all textures in Quality Settings.");
            public readonly GUIContent anisotropicForceEnableInfo = new GUIContent("Anisotropic filtering is enabled for all textures in Quality Settings.");
            public readonly GUIContent unappliedSettingsDialogTitle = new GUIContent("Unapplied import settings");
            public readonly GUIContent unappliedSettingsDialogContent = new GUIContent("Unapplied import settings for \'{0}\'.\nApply and continue to sprite editor or cancel.");
            public readonly GUIContent applyButtonLabel = new GUIContent("Apply");
            public readonly GUIContent revertButtonLabel = new GUIContent("Revert");
            public readonly GUIContent spriteEditorButtonLabel = new GUIContent("Sprite Editor");
            public readonly GUIContent resliceFromLayerWarning = new GUIContent("This will reinitialize and recreate all Sprites based on the file’s layer data. Existing Sprite metadata from previously generated Sprites are copied over.");
            public readonly GUIContent alphaIsTransparency = new GUIContent("Alpha Is Transparency", "If the provided alpha channel is transparency, enable this to pre-filter the color to avoid texture filtering artifacts. This is not supported for HDR textures.");
            public readonly GUIContent etc1Compression = new GUIContent("Compress using ETC1 (split alpha channel)|Alpha for this texture will be preserved by splitting the alpha channel to another texture, and both resulting textures will be compressed using ETC1.");
            public readonly GUIContent crunchedCompression = new GUIContent("Use Crunch Compression", "Texture is crunch-compressed to save space on disk when applicable.");

            public readonly GUIContent showAdvanced = new GUIContent("Advanced", "Show advanced settings.");

            public readonly GUIContent platformSettingsLabel  = new GUIContent("Platform Setttings");

            public readonly GUIContent[] platformSettingsSelection;

            public readonly GUIContent wrapModeLabel = new GUIContent("Wrap Mode");
            public readonly GUIContent wrapU = new GUIContent("U axis");
            public readonly GUIContent wrapV = new GUIContent("V axis");
            public readonly GUIContent wrapW = new GUIContent("W axis");


            public readonly GUIContent[] wrapModeContents =
            {
                new GUIContent("Repeat"),
                new GUIContent("Clamp"),
                new GUIContent("Mirror"),
                new GUIContent("Mirror Once"),
                new GUIContent("Per-axis")
            };
            public readonly int[] wrapModeValues =
            {
                (int)TextureWrapMode.Repeat,
                (int)TextureWrapMode.Clamp,
                (int)TextureWrapMode.Mirror,
                (int)TextureWrapMode.MirrorOnce,
                -1
            };

            public readonly GUIContent importHiddenLayer = new GUIContent(L10n.Tr("Import Hidden"), L10n.Tr("Import hidden layers"));
            public readonly GUIContent mosaicLayers = new GUIContent(L10n.Tr("Mosaic"), L10n.Tr("Layers will be imported as individual Sprites"));
            public readonly GUIContent characterMode = new GUIContent(L10n.Tr("Character Rig"), L10n.Tr("Enable to support 2D Animation character rigging"));
            public readonly GUIContent generateGOHierarchy = new GUIContent(L10n.Tr("Use Layer Grouping"), L10n.Tr("GameObjects are grouped according to source file layer grouping"));
            public readonly GUIContent resliceFromLayer = new GUIContent(L10n.Tr("Reslice"), L10n.Tr("Recreate Sprite rects from file"));
            public readonly GUIContent paperDollMode = new GUIContent(L10n.Tr("Paper Doll Mode"), L10n.Tr("Special mode to generate a Prefab for Paper Doll use case"));
            public readonly GUIContent experimental =  new GUIContent(L10n.Tr("Experimental"));
            public readonly GUIContent keepDuplicateSpriteName = new GUIContent(L10n.Tr("Keep Duplicate Name"), L10n.Tr("Keep Sprite name same as Layer Name even if there are duplicated Layer Name"));

            public Styles()
            {
                // This is far from ideal, but it's better than having tons of logic in the GUI code itself.
                // The combination should not grow too much anyway since only Texture3D will be added later.
                GUIContent[] s2D_Options = { textureShape2D };
                GUIContent[] sCube_Options = { textureShapeCube };
                GUIContent[] s2D_Cube_Options = { textureShape2D, textureShapeCube };
                textureShapeOptionsDictionnary.Add(TextureImporterShape.Texture2D, s2D_Options);
                textureShapeOptionsDictionnary.Add(TextureImporterShape.TextureCube, sCube_Options);
                textureShapeOptionsDictionnary.Add(TextureImporterShape.Texture2D | TextureImporterShape.TextureCube, s2D_Cube_Options);

                int[] s2D_Values = { (int)TextureImporterShape.Texture2D };
                int[] sCube_Values = { (int)TextureImporterShape.TextureCube };
                int[] s2D_Cube_Values = { (int)TextureImporterShape.Texture2D, (int)TextureImporterShape.TextureCube };
                textureShapeValuesDictionnary.Add(TextureImporterShape.Texture2D, s2D_Values);
                textureShapeValuesDictionnary.Add(TextureImporterShape.TextureCube, sCube_Values);
                textureShapeValuesDictionnary.Add(TextureImporterShape.Texture2D | TextureImporterShape.TextureCube, s2D_Cube_Values);

                platformSettingsSelection = new GUIContent[TexturePlatformSettingsModal.kValidBuildPlatform.Length];
                for (int i = 0; i < TexturePlatformSettingsModal.kValidBuildPlatform.Length; ++i)
                {
                    platformSettingsSelection[i] = new GUIContent(TexturePlatformSettingsModal.kValidBuildPlatform[i].buildTargetName);
                }
            }
        }

        internal static Styles s_Styles;
    }
}
