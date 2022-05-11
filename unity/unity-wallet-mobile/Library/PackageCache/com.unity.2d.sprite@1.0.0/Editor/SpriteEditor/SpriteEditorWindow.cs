using System;
using UnityEngine;
using System.Collections.Generic;
using UnityTexture2D = UnityEngine.Texture2D;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Sprites
{
    /// <summary>
    /// Interface for providing a ISpriteEditorDataProvider instance.
    /// </summary>
    /// <typeparam name="T">The object type the implemented interface is interested in.</typeparam>
    public interface ISpriteDataProviderFactory<T>
    {
        /// <summary>
        /// Implement the method to provide an instance of ISpriteEditorDataProvider for a given object.
        /// </summary>
        /// <param name="obj">The object that requires an instance of ISpriteEditorDataProvider.</param>
        /// <returns>An instance of ISpriteEditorDataProvider or null if not supported by the interface.</returns>
        ISpriteEditorDataProvider CreateDataProvider(T obj);
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal class SpriteEditorAssetPathProviderAttribute : Attribute
    {
        [RequiredSignature]
        private static string GetAssetPath(UnityEngine.Object obj)
        {
            return null;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal class SpriteObjectProviderAttribute : Attribute
    {
        [RequiredSignature]
        private static Sprite GetSpriteObject(UnityEngine.Object obj)
        {
            return null;
        }
    }

    /// <summary>
    /// Utility class that collects methods with SpriteDataProviderFactoryAttribute and SpriteDataProviderAssetPathProviderAttribute.
    /// </summary>
    public class SpriteDataProviderFactories
    {
        struct SpriteDataProviderFactory
        {
            public object instance;
            public MethodInfo method;
            public Type methodType;
        }

        static SpriteDataProviderFactory[] s_Factories;
        static TypeCache.MethodCollection s_AssetPathProvider;
        static TypeCache.MethodCollection s_SpriteObjectProvider;

        static SpriteDataProviderFactory[] GetFactories()
        {
            CacheDataProviders();
            return s_Factories;
        }

        static TypeCache.MethodCollection GetAssetPathProvider()
        {
            CacheDataProviders();
            return s_AssetPathProvider;
        }

        static TypeCache.MethodCollection GetSpriteObjectProvider()
        {
            CacheDataProviders();
            return s_SpriteObjectProvider;
        }

        static void CacheDataProviders()
        {
            if (s_Factories != null)
                return;

            var factories = TypeCache.GetTypesDerivedFrom(typeof(ISpriteDataProviderFactory<>));
            var factoryList = new List<SpriteDataProviderFactory>();
            foreach (var factory in factories)
            {
                try
                {
                    var ins = Activator.CreateInstance(factory);
                    foreach (var i in factory.GetInterfaces())
                    {
                        var genericArguments = i.GetGenericArguments();
                        if (genericArguments.Length == 1)
                        {
                            var s = new SpriteDataProviderFactory();
                            s.instance = ins;
                            var method = i.GetMethod("CreateDataProvider");
                            s.method = method;
                            s.methodType = genericArguments[0];
                            factoryList.Add(s);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogAssertion(ex);
                }
            }
            s_Factories = factoryList.ToArray();
            s_AssetPathProvider = TypeCache.GetMethodsWithAttribute<SpriteEditorAssetPathProviderAttribute>();
            s_SpriteObjectProvider = TypeCache.GetMethodsWithAttribute<SpriteObjectProviderAttribute>();
        }

        /// <summary>
        /// Initialized and collect methods with SpriteDataProviderFactoryAttribute and SpriteDataProviderAssetPathProviderAttribute.
        /// </summary>
        public void Init()
        {
            CacheDataProviders();
        }

        /// <summary>
        /// Given a UnityEngine.Object, determine the ISpriteEditorDataProvider associate with the object by going
        /// going through the methods with SpriteDataProviderFactoryAttribute.
        /// </summary>
        /// <remarks>When none of the methods is able to provide ISpriteEditorDataProvider for the object, the method will
        /// try to cast the AssetImporter of the object to ISpriteEditorDataProvider.</remarks>
        /// <param name="obj">The UnityEngine.Object to query.</param>
        /// <returns>The ISpriteEditorDataProvider associated with the object.</returns>
        public ISpriteEditorDataProvider GetSpriteEditorDataProviderFromObject(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                var objType = obj.GetType();
                foreach (var factory in GetFactories())
                {
                    try
                    {
                        if (factory.methodType == objType)
                        {
                            var dataProvider = factory.method.Invoke(factory.instance, new[] { obj }) as ISpriteEditorDataProvider;
                            if (dataProvider != null && !dataProvider.Equals(null))
                                return dataProvider;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogAssertion(ex);
                    }
                }

                if (obj is ISpriteEditorDataProvider)
                    return (ISpriteEditorDataProvider)obj;
                // now we try the importer
                var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj));
                return importer as ISpriteEditorDataProvider;
            }

            return null;
        }

        /// <summary>
        /// Given a UnityEngine.Object, determine the asset path associate with the object by going
        /// going through the methods with SpriteDataProviderAssetPathProviderAttribute.
        /// </summary>
        /// <remarks>When none of the methods is able to provide the asset path for the object, the method will return null</remarks>
        /// <param name="obj">The UnityEngine.Object to query</param>
        /// <returns>The asset path for the object</returns>
        internal string GetAssetPath(UnityEngine.Object obj)
        {
            foreach (var assetPathProvider in GetAssetPathProvider())
            {
                try
                {
                    var path = assetPathProvider.Invoke(null, new object[] { obj }) as string;
                    if (!string.IsNullOrEmpty(path))
                        return path;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            return null;
        }

        /// <summary>
        /// Given a UnityEngine.Object, determine the Sprite object associate with the object by going
        /// going through the methods with SpriteObjectProviderAttribute.
        /// </summary>
        /// <remarks>When none of the methods is able to provide a Sprite object, the method will return null</remarks>
        /// <param name="obj">The UnityEngine.Object to query</param>
        /// <returns>The Sprite object</returns>
        internal Sprite GetSpriteObject(UnityEngine.Object obj)
        {
            foreach (var spriteObjectProvider in GetSpriteObjectProvider())
            {
                try
                {
                    var sprite = spriteObjectProvider.Invoke(null, new object[] { obj }) as Sprite;
                    if (sprite != null && !sprite.Equals(null))
                        return sprite;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            return null;
        }
    }

    [InitializeOnLoad]
    internal class SpriteEditorWindow : SpriteUtilityWindow, ISpriteEditor
    {
        static SpriteEditorWindow()
        {
            UnityEditor.SpriteUtilityWindow.SetShowSpriteEditorWindowWithObject((x) =>
            {
                SpriteEditorWindow.GetWindow(x);
                return true;
            });
        }

        private class SpriteEditorWindowStyles
        {
            public static readonly GUIContent editingDisableMessageBecausePlaymodeLabel = EditorGUIUtility.TrTextContent("Editing is disabled during play mode");
            public static readonly GUIContent editingDisableMessageBecauseNonEditableLabel = EditorGUIUtility.TrTextContent("Editing is disabled because the asset is not editable.");
            public static readonly GUIContent revertButtonLabel = EditorGUIUtility.TrTextContent("Revert");
            public static readonly GUIContent applyButtonLabel = EditorGUIUtility.TrTextContent("Apply");

            public static readonly GUIContent spriteEditorWindowTitle = EditorGUIUtility.TrTextContent("Sprite Editor");

            public static readonly GUIContent pendingChangesDialogContent = EditorGUIUtility.TrTextContent("The asset was modified outside of Sprite Editor Window.\nDo you want to apply pending changes?");

            public static readonly GUIContent applyRevertDialogTitle = EditorGUIUtility.TrTextContent("Unapplied import settings");
            public static readonly GUIContent applyRevertDialogContent = EditorGUIUtility.TrTextContent("Unapplied import settings for '{0}'");

            public static readonly GUIContent noSelectionWarning = EditorGUIUtility.TrTextContent("No texture or sprite selected");
            public static readonly GUIContent noModuleWarning = EditorGUIUtility.TrTextContent("No Sprite Editor module available");
            public static readonly GUIContent applyRevertModuleDialogTitle = EditorGUIUtility.TrTextContent("Unapplied module changes");
            public static readonly GUIContent applyRevertModuleDialogContent = EditorGUIUtility.TrTextContent("You have unapplied changes from the current module");

            public static readonly GUIContent loadProgressTitle = EditorGUIUtility.TrTextContent("Loading");
            public static readonly GUIContent loadContentText = EditorGUIUtility.TrTextContent("Loading Sprites {0}/{1}");
            public static readonly string styleSheetPath = "Packages/com.unity.2d.sprite/Editor/UI/SpriteEditor/SpriteEditor.uss";
        }

        class CurrentResetContext
        {
            public string assetPath;
        }

        private const float k_MarginForFraming = 0.05f;
        private const float k_WarningMessageWidth = 250f;
        private const float k_WarningMessageHeight = 40f;
        private const float k_ModuleListWidth = 90f;
        private const string k_RefreshOnNextRepaintCommandEvent = "RefreshOnNextRepaintCommand";
        bool m_ResetOnNextRepaint;
        bool m_ResetCommandSent;

        private List<SpriteRect> m_RectsCache;
        ISpriteEditorDataProvider m_SpriteDataProvider;

        private bool m_RequestRepaint = false;

        public static bool s_OneClickDragStarted = false;
        string m_SelectedAssetPath;
        bool m_AssetNotEditable;

        private IEventSystem m_EventSystem;
        private IUndoSystem m_UndoSystem;
        private IAssetDatabase m_AssetDatabase;
        private IGUIUtility m_GUIUtility;
        private UnityTexture2D m_OutlineTexture;
        private UnityTexture2D m_ReadableTexture;
        private Dictionary<Type, RequireSpriteDataProviderAttribute> m_ModuleRequireSpriteDataProvider = new Dictionary<Type, RequireSpriteDataProviderAttribute>();

        private IMGUIContainer m_ToolbarIMGUIElement;
        private IMGUIContainer m_MainViewIMGUIElement;
        private VisualElement m_ModuleViewElement;
        private VisualElement m_MainViewElement;
        SpriteDataProviderFactories m_SpriteDataProviderFactories;

        [SerializeField]
        private UnityEngine.Object m_SelectedObject;

        [SerializeField]
        private string m_SelectedSpriteRectGUID;

        internal Func<string, string, bool> onHandleApplyRevertDialog = ShowHandleApplyRevertDialog;

        private CurrentResetContext m_CurrentResetContext = null;

        public static void GetWindow(UnityEngine.Object obj)
        {
            var window = EditorWindow.GetWindow<SpriteEditorWindow>();
            window.selectedObject = obj;
        }

        public SpriteEditorWindow()
        {
            m_EventSystem = new EventSystem();
            m_UndoSystem = new UndoSystem();
            m_AssetDatabase = new AssetDatabaseSystem();
            m_GUIUtility = new GUIUtilitySystem();
        }

        void ModifierKeysChanged()
        {
            if (EditorWindow.focusedWindow == this)
            {
                Repaint();
            }
        }

        private void OnFocus()
        {
            if (selectedObject != Selection.activeObject)
                OnSelectionChange();
            if (selectedProviderChanged)
                RefreshSpriteEditorWindow();
        }

        internal UnityEngine.Object selectedObject
        {
            get { return m_SelectedObject; }
            set
            {
                m_SelectedObject = value;
                RefreshSpriteEditorWindow();
            }
        }

        string selectedAssetPath
        {
            get => m_SelectedAssetPath;
            set
            {
                m_SelectedAssetPath = value;
                m_AssetNotEditable = !AssetDatabase.IsOpenForEdit(m_SelectedAssetPath);
            }
        }

        public void RefreshPropertiesCache()
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(selectedAssetPath);
            m_SpriteDataProvider = spriteDataProviderFactories.GetSpriteEditorDataProviderFromObject(obj);
            if (!IsSpriteDataProviderValid())
            {
                selectedAssetPath = "";
                return;
            }


            m_SpriteDataProvider.InitSpriteEditorDataProvider();

            var textureProvider = m_SpriteDataProvider.GetDataProvider<ITextureDataProvider>();
            if (textureProvider != null)
            {
                int width = 0, height = 0;
                textureProvider.GetTextureActualWidthAndHeight(out width, out height);
                m_Texture = textureProvider.previewTexture == null ? null : new PreviewTexture2D(textureProvider.previewTexture, width, height);
            }
        }

        internal string GetSelectionAssetPath()
        {
            var path = spriteDataProviderFactories.GetAssetPath(selectedObject);
            if (string.IsNullOrEmpty(path))
                path = m_AssetDatabase.GetAssetPath(selectedObject);
            return path;
        }

        public void InvalidatePropertiesCache()
        {
            m_RectsCache = null;
            m_SpriteDataProvider = null;
        }

        private Rect warningMessageRect
        {
            get
            {
                return new Rect(
                    position.width - k_WarningMessageWidth - k_InspectorWindowMargin - k_ScrollbarMargin,
                    k_InspectorWindowMargin + k_ScrollbarMargin,
                    k_WarningMessageWidth,
                    k_WarningMessageHeight);
            }
        }

        public SpriteImportMode spriteImportMode
        {
            get { return !IsSpriteDataProviderValid() ? SpriteImportMode.None : m_SpriteDataProvider.spriteImportMode; }
        }

        bool activeDataProviderSelected
        {
            get { return m_SpriteDataProvider != null; }
        }

        public bool textureIsDirty
        {
            get
            {
                return hasUnsavedChanges;
            }
            set
            {
                hasUnsavedChanges = value;
            }
        }

        public bool selectedProviderChanged
        {
            get
            {
                var assetPath = GetSelectionAssetPath();
                var dataProvider = spriteDataProviderFactories.GetSpriteEditorDataProviderFromObject(selectedObject);
                return dataProvider != null && selectedAssetPath != assetPath;
            }
        }

        void OnSelectionChange()
        {
            selectedObject = Selection.activeObject;
            RefreshSpriteEditorWindow();
        }

        void RefreshSpriteEditorWindow()
        {
            // In case of changed of texture/sprite or selected on non texture object
            bool updateModules = false;
            if (selectedProviderChanged)
            {
                HandleApplyRevertDialog(SpriteEditorWindowStyles.applyRevertDialogTitle.text,
                    String.Format(SpriteEditorWindowStyles.applyRevertDialogContent.text, selectedAssetPath));
                selectedAssetPath = GetSelectionAssetPath();
                ResetWindow();
                ResetZoomAndScroll();
                RefreshPropertiesCache();
                RefreshRects();
                updateModules = true;
            }

            if (m_RectsCache != null)
            {
                UpdateSelectedSpriteRectFromSelection();
            }

            // We only update modules when data provider changed
            if (updateModules)
                UpdateAvailableModules();
            Repaint();
        }

        private void UpdateSelectedSpriteRectFromSelection()
        {
            if (Selection.activeObject is UnityEngine.Sprite)
            {
                UpdateSelectedSpriteRect(Selection.activeObject as UnityEngine.Sprite);
            }
            else
            {
                var sprite = spriteDataProviderFactories.GetSpriteObject(Selection.activeObject);
                UpdateSelectedSpriteRect(sprite);
            }
        }

        public void ResetWindow()
        {
            InvalidatePropertiesCache();
            textureIsDirty = false;
            saveChangesMessage = SpriteEditorWindowStyles.applyRevertModuleDialogContent.text;
        }

        public void ResetZoomAndScroll()
        {
            m_Zoom = -1;
            m_ScrollPosition = Vector2.zero;
        }

        SpriteDataProviderFactories spriteDataProviderFactories
        {
            get
            {
                if (m_SpriteDataProviderFactories == null)
                {
                    m_SpriteDataProviderFactories = new SpriteDataProviderFactories();
                    m_SpriteDataProviderFactories.Init();
                }
                return m_SpriteDataProviderFactories;
            }
        }

        void OnEnable()
        {
            this.name = "SpriteEditorWindow";
            selectedObject = Selection.activeObject;
            minSize = new Vector2(360, 200);
            titleContent = SpriteEditorWindowStyles.spriteEditorWindowTitle;
            m_UndoSystem.RegisterUndoCallback(UndoRedoPerformed);
            EditorApplication.modifierKeysChanged += ModifierKeysChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.quitting += OnEditorApplicationQuit;

            if (selectedProviderChanged)
                selectedAssetPath = GetSelectionAssetPath();

            ResetWindow();
            RefreshPropertiesCache();
            bool noSelectedSprite = string.IsNullOrEmpty(m_SelectedSpriteRectGUID);
            RefreshRects();
            if (noSelectedSprite)
                UpdateSelectedSpriteRectFromSelection();
            UnityEditor.SpriteUtilityWindow.SetApplySpriteEditorWindow(RebuildCache);

            if (SetupVisualElements())
                InitModules();
        }

        void CreateGUI()
        {
            if (m_MainViewElement == null)
            {
                if (SetupVisualElements())
                    InitModules();
            }
        }

        private bool SetupVisualElements()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(SpriteEditorWindowStyles.styleSheetPath);
            if (styleSheet != null)
            {
                m_ToolbarIMGUIElement = new IMGUIContainer(DoToolbarGUI)
                {
                    name = "spriteEditorWindowToolbar",
                };
                m_MainViewIMGUIElement = new IMGUIContainer(DoTextureAndModulesGUI)
                {
                    name = "mainViewIMGUIElement"
                };
                m_MainViewElement = new VisualElement()
                {
                    name = "spriteEditorWindowMainView",
                };
                m_ModuleViewElement = new VisualElement()
                {
                    name = "moduleViewElement",
                    pickingMode = PickingMode.Ignore
                };
                m_MainViewElement.Add(m_MainViewIMGUIElement);
                m_MainViewElement.Add(m_ModuleViewElement);
                var root = rootVisualElement;
                root.styleSheetList.Add(styleSheet);
                root.Add(m_ToolbarIMGUIElement);
                root.Add(m_MainViewElement);
                return true;
            }
            return false;
        }

        private void UndoRedoPerformed()
        {
            // Was selected texture changed by undo?
            if (selectedProviderChanged)
                RefreshSpriteEditorWindow();

            InitSelectedSpriteRect();

            Repaint();
        }

        private void InitSelectedSpriteRect()
        {
            SpriteRect newSpriteRect = null;
            if (m_RectsCache != null && m_RectsCache.Count > 0)
            {
                if (selectedSpriteRect != null)
                    newSpriteRect = m_RectsCache.FirstOrDefault(x => x.spriteID == selectedSpriteRect.spriteID) != null ? selectedSpriteRect : m_RectsCache[0];
                else
                    newSpriteRect = m_RectsCache[0];
            }

            selectedSpriteRect = newSpriteRect;
        }

        void OnGUI()
        {
            CreateGUI();
        }

        public override void SaveChanges()
        {
            var oldDelegate = onHandleApplyRevertDialog;
            onHandleApplyRevertDialog = (x, y) => true;
            HandleApplyRevertDialog(SpriteEditorWindowStyles.applyRevertDialogTitle.text,
                String.Format(SpriteEditorWindowStyles.applyRevertDialogContent.text, selectedAssetPath));
            onHandleApplyRevertDialog = oldDelegate;
            base.SaveChanges();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            InvalidatePropertiesCache();
            EditorApplication.modifierKeysChanged -= ModifierKeysChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.quitting -= OnEditorApplicationQuit;

            if (m_OutlineTexture != null)
            {
                DestroyImmediate(m_OutlineTexture);
                m_OutlineTexture = null;
            }

            if (m_ReadableTexture)
            {
                DestroyImmediate(m_ReadableTexture);
                m_ReadableTexture = null;
            }

            if (m_CurrentModule != null)
                m_CurrentModule.OnModuleDeactivate();
            UnityEditor.SpriteUtilityWindow.SetApplySpriteEditorWindow(null);
            if (m_MainViewElement != null)
            {
                rootVisualElement.Remove(m_MainViewElement);
                m_MainViewElement = null;
            }
        }

        void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            if (PlayModeStateChange.EnteredPlayMode == playModeState || PlayModeStateChange.EnteredEditMode == playModeState)
            {
                RebuildCache();
            }
        }

        void OnEditorApplicationQuit()
        {
            HandleApplyRevertDialog(SpriteEditorWindowStyles.applyRevertDialogTitle.text,
                String.Format(SpriteEditorWindowStyles.applyRevertDialogContent.text, selectedAssetPath));
        }

        static bool ShowHandleApplyRevertDialog(string dialogTitle, string dialogContent)
        {
            return EditorUtility.DisplayDialog(dialogTitle, dialogContent,
                SpriteEditorWindowStyles.applyButtonLabel.text, SpriteEditorWindowStyles.revertButtonLabel.text);
        }

        void HandleApplyRevertDialog(string dialogTitle, string dialogContent)
        {
            if (textureIsDirty && IsSpriteDataProviderValid())
            {
                if (onHandleApplyRevertDialog(dialogTitle, dialogContent))
                    DoApply();
                else
                    DoRevert();

                SetupModule(m_CurrentModuleIndex);
            }
        }

        bool IsSpriteDataProviderValid()
        {
            return m_SpriteDataProvider != null && !m_SpriteDataProvider.Equals(null);
        }

        void RefreshRects()
        {
            m_RectsCache = null;
            if (IsSpriteDataProviderValid())
            {
                m_RectsCache = m_SpriteDataProvider.GetSpriteRects().ToList();
            }

            InitSelectedSpriteRect();
        }

        private void UpdateAssetSelectionChange()
        {
            if (selectedProviderChanged)
            {
                ResetOnNextRepaint();
            }

            if (m_ResetCommandSent || (UnityEngine.Event.current.type == EventType.ExecuteCommand && UnityEngine.Event.current.commandName == k_RefreshOnNextRepaintCommandEvent))
            {
                m_ResetCommandSent = false;
                if (selectedProviderChanged || !IsSpriteDataProviderValid())
                    selectedAssetPath = GetSelectionAssetPath();
                RebuildCache();
            }
        }

        internal void ResetOnNextRepaint()
        {
            //Because we can't show dialog in a repaint/layout event, we need to send event to IMGUI to trigger this.
            //The event is now sent through the Update loop.
            m_ResetOnNextRepaint = true;
            if (textureIsDirty)
            {
                // We can't depend on the existing data provider to set data because a reimport might cause
                // the data provider to be invalid. We store up the current asset path so that in DoApply()
                // the modified data can be set correctly to correct asset.
                if (m_CurrentResetContext != null)
                    Debug.LogError("Existing reset not completed for " + m_CurrentResetContext.assetPath);
                m_CurrentResetContext = new CurrentResetContext()
                {
                    assetPath = selectedAssetPath
                };
            }
        }

        void Update()
        {
            if (m_ResetOnNextRepaint)
            {
                m_ResetOnNextRepaint = false;
                m_ResetCommandSent = true;
                var e = EditorGUIUtility.CommandEvent(k_RefreshOnNextRepaintCommandEvent);
                this.SendEvent(e);
            }
        }

        private void RebuildCache()
        {
            HandleApplyRevertDialog(SpriteEditorWindowStyles.applyRevertDialogTitle.text, SpriteEditorWindowStyles.pendingChangesDialogContent.text);
            ResetWindow();
            RefreshPropertiesCache();
            RefreshRects();
            UpdateAvailableModules();
        }

        private void DoTextureAndModulesGUI()
        {
            // Don't do anything until reset event is sent
            if (m_ResetOnNextRepaint)
                return;
            InitStyles();
            UpdateAssetSelectionChange();
            if (m_ResetCommandSent)
                return;
            if (!activeDataProviderSelected)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    GUILayout.Label(SpriteEditorWindowStyles.noSelectionWarning);
                }
                return;
            }
            if (m_CurrentModule == null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    GUILayout.Label(SpriteEditorWindowStyles.noModuleWarning);
                }
                return;
            }
            textureViewRect = new Rect(0f, 0f, m_MainViewIMGUIElement.layout.width - k_ScrollbarMargin, m_MainViewIMGUIElement.layout.height - k_ScrollbarMargin);
            Matrix4x4 oldHandlesMatrix = Handles.matrix;
            DoTextureGUI();
            // Warning message if applicable
            DoEditingDisabledMessage();
            m_CurrentModule.DoPostGUI();
            Handles.matrix = oldHandlesMatrix;
            if (m_RequestRepaint)
            {
                Repaint();
                m_RequestRepaint = false;
            }
        }

        protected override void DoTextureGUIExtras()
        {
            HandleFrameSelected();

            if (m_EventSystem.current.type == EventType.Repaint)
            {
                SpriteEditorUtility.BeginLines(new Color(1f, 1f, 1f, 0.5f));
                var selectedRect = selectedSpriteRect != null ? selectedSpriteRect.spriteID : new GUID();
                for (int i = 0; i < m_RectsCache.Count; i++)
                {
                    if (m_RectsCache[i].spriteID != selectedRect)
                        SpriteEditorUtility.DrawBox(m_RectsCache[i].rect);
                }
                SpriteEditorUtility.EndLines();
            }

            m_CurrentModule.DoMainGUI();
        }

        private void DoToolbarGUI()
        {
            InitStyles();

            GUIStyle toolBarStyle = EditorStyles.toolbar;

            Rect toolbarRect = new Rect(0, 0, position.width, k_ToolbarHeight);
            if (m_EventSystem.current.type == EventType.Repaint)
            {
                toolBarStyle.Draw(toolbarRect, false, false, false, false);
            }

            if (!activeDataProviderSelected || m_CurrentModule == null)
                return;
            // Top menu bar

            // only show popup if there is more than 1 module.
            if (m_RegisteredModules.Count > 1)
            {
                float moduleWidthPercentage = k_ModuleListWidth / minSize.x;
                float moduleListWidth = position.width > minSize.x ? position.width * moduleWidthPercentage : k_ModuleListWidth;
                moduleListWidth = Mathf.Min(moduleListWidth, EditorStyles.toolbarPopup.CalcSize(m_RegisteredModuleNames[m_CurrentModuleIndex]).x);
                int module = EditorGUI.Popup(new Rect(0, 0, moduleListWidth, k_ToolbarHeight), m_CurrentModuleIndex, m_RegisteredModuleNames, EditorStyles.toolbarPopup);
                if (module != m_CurrentModuleIndex)
                {
                    if (textureIsDirty)
                    {
                        // Have pending module edit changes. Ask user if they want to apply or revert
                        if (EditorUtility.DisplayDialog(SpriteEditorWindowStyles.applyRevertModuleDialogTitle.text,
                            SpriteEditorWindowStyles.applyRevertModuleDialogContent.text,
                            SpriteEditorWindowStyles.applyButtonLabel.text, SpriteEditorWindowStyles.revertButtonLabel.text))
                            DoApply();
                        else
                            DoRevert();
                    }
                    m_LastUsedModuleTypeName = m_RegisteredModules[module].GetType().FullName;
                    SetupModule(module);
                }
                toolbarRect.x = moduleListWidth;
            }

            toolbarRect  = DoAlphaZoomToolbarGUI(toolbarRect);

            Rect applyRevertDrawArea = toolbarRect;
            applyRevertDrawArea.x = applyRevertDrawArea.width;

            using (new EditorGUI.DisabledScope(!textureIsDirty))
            {
                applyRevertDrawArea.width = EditorStyles.toolbarButton.CalcSize(SpriteEditorWindowStyles.applyButtonLabel).x;
                applyRevertDrawArea.x -= applyRevertDrawArea.width;

                if (GUI.Button(applyRevertDrawArea, SpriteEditorWindowStyles.applyButtonLabel, EditorStyles.toolbarButton))
                {
                    DoApply();
                    SetupModule(m_CurrentModuleIndex);
                }

                applyRevertDrawArea.width = EditorStyles.toolbarButton.CalcSize(SpriteEditorWindowStyles.revertButtonLabel).x;
                applyRevertDrawArea.x -= applyRevertDrawArea.width;
                if (GUI.Button(applyRevertDrawArea, SpriteEditorWindowStyles.revertButtonLabel, EditorStyles.toolbarButton))
                {
                    DoRevert();
                    SetupModule(m_CurrentModuleIndex);
                }
            }

            toolbarRect.width = applyRevertDrawArea.x - toolbarRect.x;
            m_CurrentModule.DoToolbarGUI(toolbarRect);
        }

        private void DoEditingDisabledMessage()
        {
            if (editingDisabled)
            {
                GUILayout.BeginArea(warningMessageRect);
                var disableMessage = m_AssetNotEditable ? SpriteEditorWindowStyles.editingDisableMessageBecauseNonEditableLabel.text : SpriteEditorWindowStyles.editingDisableMessageBecausePlaymodeLabel.text;
                EditorGUILayout.HelpBox(disableMessage, MessageType.Warning);
                GUILayout.EndArea();
            }
        }

        private void DoApply()
        {
            textureIsDirty = false;
            bool reimport = true;
            var dataProvider = m_SpriteDataProvider;
            if (m_CurrentResetContext != null)
            {
                m_SpriteDataProvider =
                    m_SpriteDataProviderFactories.GetSpriteEditorDataProviderFromObject(
                        AssetDatabase.LoadMainAssetAtPath(m_CurrentResetContext.assetPath));
                m_SpriteDataProvider.InitSpriteEditorDataProvider();
                m_CurrentResetContext = null;
            }

            if (m_SpriteDataProvider != null)
            {
                if (m_CurrentModule != null)
                    reimport = m_CurrentModule.ApplyRevert(true);
                m_SpriteDataProvider.Apply();
            }

            m_SpriteDataProvider = dataProvider;
            // Do this so that asset change save dialog will not show
            var originalValue = EditorPrefs.GetBool("VerifySavingAssets", false);
            EditorPrefs.SetBool("VerifySavingAssets", false);
            AssetDatabase.ForceReserializeAssets(new[] {selectedAssetPath}, ForceReserializeAssetsOptions.ReserializeMetadata);
            EditorPrefs.SetBool("VerifySavingAssets", originalValue);

            if (reimport)
                DoTextureReimport(selectedAssetPath);
            Repaint();
            RefreshRects();
        }

        private void DoRevert()
        {
            textureIsDirty = false;
            RefreshRects();
            GUI.FocusControl("");
            if (m_CurrentModule != null)
                m_CurrentModule.ApplyRevert(false);
        }

        public bool HandleSpriteSelection()
        {
            bool changed = false;

            if (m_EventSystem.current.type == EventType.MouseDown && m_EventSystem.current.button == 0 && GUIUtility.hotControl == 0 && !m_EventSystem.current.alt)
            {
                var oldSelected = selectedSpriteRect;

                var triedRect = TrySelect(m_EventSystem.current.mousePosition);
                if (triedRect != oldSelected)
                {
                    Undo.RegisterCompleteObjectUndo(this, "Sprite Selection");

                    selectedSpriteRect = triedRect;
                    changed = true;
                }

                if (selectedSpriteRect != null)
                    s_OneClickDragStarted = true;
                else
                    RequestRepaint();

                if (changed && selectedSpriteRect != null)
                {
                    m_EventSystem.current.Use();
                }
            }

            return changed;
        }

        private void HandleFrameSelected()
        {
            var evt = m_EventSystem.current;

            if ((evt.type == EventType.ValidateCommand || evt.type == EventType.ExecuteCommand)
                && evt.commandName == EventCommandNames.FrameSelected)
            {
                if (evt.type == EventType.ExecuteCommand)
                {
                    // Do not do frame if there is none selected
                    if (selectedSpriteRect == null)
                        return;

                    Rect rect = selectedSpriteRect.rect;

                    // Calculate the require pixel to display the frame, then get the zoom needed.
                    float targetZoom = m_Zoom;
                    if (rect.width < rect.height)
                        targetZoom = textureViewRect.height / (rect.height + textureViewRect.height * k_MarginForFraming);
                    else
                        targetZoom = textureViewRect.width / (rect.width + textureViewRect.width * k_MarginForFraming);

                    // Apply the zoom
                    zoomLevel = targetZoom;

                    // Calculate the scroll values to center the frame
                    m_ScrollPosition.x = (rect.center.x - (m_Texture.width * 0.5f)) * m_Zoom;
                    m_ScrollPosition.y = (rect.center.y - (m_Texture.height * 0.5f)) * m_Zoom * -1.0f;

                    Repaint();
                }

                evt.Use();
            }
        }

        void UpdateSelectedSpriteRect(UnityEngine.Sprite sprite)
        {
            if (m_RectsCache == null || sprite == null || sprite.Equals(null))
                return;

            var spriteGUID = sprite.GetSpriteID();
            for (int i = 0; i < m_RectsCache.Count; i++)
            {
                if (spriteGUID == m_RectsCache[i].spriteID)
                {
                    selectedSpriteRect = m_RectsCache[i];
                    return;
                }
            }
            selectedSpriteRect = null;
        }

        private SpriteRect TrySelect(Vector2 mousePosition)
        {
            float selectedSize = float.MaxValue;
            SpriteRect currentRect = null;
            mousePosition = Handles.inverseMatrix.MultiplyPoint(mousePosition);

            for (int i = 0; i < m_RectsCache.Count; i++)
            {
                var sr = m_RectsCache[i];
                if (sr.rect.Contains(mousePosition))
                {
                    // If we clicked inside an already selected spriterect, always persist that selection
                    if (sr == selectedSpriteRect)
                        return sr;

                    float width = sr.rect.width;
                    float height = sr.rect.height;
                    float newSize = width * height;
                    if (width > 0f && height > 0f && newSize < selectedSize)
                    {
                        currentRect = sr;
                        selectedSize = newSize;
                    }
                }
            }

            return currentRect;
        }

        public void DoTextureReimport(string path)
        {
            if (m_SpriteDataProvider != null)
            {
                try
                {
                    AssetDatabase.StartAssetEditing();
                    AssetDatabase.ImportAsset(path);
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
            }
        }

        GUIContent[] m_RegisteredModuleNames;
        List<SpriteEditorModuleBase> m_AllRegisteredModules;
        List<SpriteEditorModuleBase> m_RegisteredModules;
        SpriteEditorModuleBase m_CurrentModule = null;
        int m_CurrentModuleIndex = 0;
        [SerializeField]
        string m_LastUsedModuleTypeName;

        internal void SetupModule(int newModuleIndex)
        {
            m_ModuleViewElement.Clear();
            if (m_RegisteredModules.Count > newModuleIndex)
            {
                m_CurrentModuleIndex = newModuleIndex;
                if (m_CurrentModule != null)
                    m_CurrentModule.OnModuleDeactivate();

                m_CurrentModule = null;

                m_CurrentModule = m_RegisteredModules[newModuleIndex];

                m_CurrentModule.OnModuleActivate();
            }
            if (m_MainViewElement != null)
                m_MainViewElement.MarkDirtyRepaint();
            if (m_ModuleViewElement != null)
                m_ModuleViewElement.MarkDirtyRepaint();
        }

        void UpdateAvailableModules()
        {
            if (m_AllRegisteredModules == null)
                return;
            m_RegisteredModules = new List<SpriteEditorModuleBase>();
            foreach (var module in m_AllRegisteredModules)
            {
                if (module.CanBeActivated())
                {
                    RequireSpriteDataProviderAttribute attribute = null;
                    m_ModuleRequireSpriteDataProvider.TryGetValue(module.GetType(), out attribute);
                    if (attribute == null || attribute.ContainsAllType(m_SpriteDataProvider))
                        m_RegisteredModules.Add(module);
                }
            }

            m_RegisteredModuleNames = new GUIContent[m_RegisteredModules.Count];
            int lastUsedModuleIndex = 0;
            for (int i = 0; i < m_RegisteredModules.Count; i++)
            {
                m_RegisteredModuleNames[i] = new GUIContent(m_RegisteredModules[i].moduleName);
                if (m_RegisteredModules[i].GetType().FullName.Equals(m_LastUsedModuleTypeName))
                {
                    lastUsedModuleIndex = i;
                }
            }

            SetupModule(lastUsedModuleIndex);
        }

        void InitModules()
        {
            m_AllRegisteredModules = new List<SpriteEditorModuleBase>();
            m_ModuleRequireSpriteDataProvider.Clear();

            if (m_OutlineTexture == null)
            {
                m_OutlineTexture = new UnityTexture2D(1, 16, TextureFormat.RGBA32, false);
                m_OutlineTexture.SetPixels(new Color[]
                {
                    new Color(0.5f, 0.5f, 0.5f, 0.5f), new Color(0.5f, 0.5f, 0.5f, 0.5f), new Color(0.8f, 0.8f, 0.8f, 0.8f), new Color(0.8f, 0.8f, 0.8f, 0.8f),
                    Color.white, Color.white, Color.white, Color.white,
                    new Color(.8f, .8f, .8f, 1f), new Color(.5f, .5f, .5f, .8f), new Color(0.3f, 0.3f, 0.3f, 0.5f), new Color(0.3f, .3f, 0.3f, 0.5f),
                    new Color(0.3f, .3f, 0.3f, 0.3f), new Color(0.3f, .3f, 0.3f, 0.3f), new Color(0.1f, 0.1f, 0.1f, 0.1f), new Color(0.1f, .1f, 0.1f, 0.1f)
                });
                m_OutlineTexture.Apply();
                m_OutlineTexture.hideFlags = HideFlags.HideAndDontSave;
            }
            var outlineTexture = new Texture2DWrapper(m_OutlineTexture);

            // Add your modules here
            RegisterModule(new SpriteFrameModule(this, m_EventSystem, m_UndoSystem, m_AssetDatabase));
            RegisterModule(new SpritePolygonModeModule(this, m_EventSystem, m_UndoSystem, m_AssetDatabase));
            RegisterModule(new SpriteOutlineModule(this, m_EventSystem, m_UndoSystem, m_AssetDatabase, m_GUIUtility, new ShapeEditorFactory(), outlineTexture));
            RegisterModule(new SpritePhysicsShapeModule(this, m_EventSystem, m_UndoSystem, m_AssetDatabase, m_GUIUtility, new ShapeEditorFactory(), outlineTexture));
            RegisterCustomModules();
            UpdateAvailableModules();
        }

        void RegisterModule(SpriteEditorModuleBase module)
        {
            var type = module.GetType();
            var attributes = type.GetCustomAttributes(typeof(RequireSpriteDataProviderAttribute), false);
            if (attributes.Length == 1)
                m_ModuleRequireSpriteDataProvider.Add(type, (RequireSpriteDataProviderAttribute)attributes[0]);
            m_AllRegisteredModules.Add(module);
        }

        void RegisterCustomModules()
        {
            foreach (var moduleClassType in TypeCache.GetTypesDerivedFrom<SpriteEditorModuleBase>())
            {
                if (!moduleClassType.IsAbstract)
                {
                    bool moduleFound = false;
                    foreach (var module in m_AllRegisteredModules)
                    {
                        if (module.GetType() == moduleClassType)
                        {
                            moduleFound = true;
                            break;
                        }
                    }
                    if (!moduleFound)
                    {
                        var constructorType = new Type[0];
                        // Get the public instance constructor that takes ISpriteEditorModule parameter.
                        var constructorInfoObj = moduleClassType.GetConstructor(
                            BindingFlags.Instance | BindingFlags.Public, null,
                            CallingConventions.HasThis, constructorType, null);
                        if (constructorInfoObj != null)
                        {
                            try
                            {
                                var newInstance = constructorInfoObj.Invoke(new object[0]) as SpriteEditorModuleBase;
                                if (newInstance != null)
                                {
                                    newInstance.spriteEditor = this;
                                    RegisterModule(newInstance);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning("Unable to instantiate custom module " + moduleClassType.FullName + ". Exception:" + ex);
                            }
                        }
                        else
                            Debug.LogWarning(moduleClassType.FullName + " does not have a parameterless constructor");
                    }
                }
            }
        }

        internal List<SpriteEditorModuleBase> activatedModules
        {
            get { return m_RegisteredModules; }
        }

        public List<SpriteRect> spriteRects
        {
            set { m_RectsCache = value; }
        }

        public SpriteRect selectedSpriteRect
        {
            get
            {
                // Always return null if editing is disabled to prevent all possible action to selected frame.
                if (editingDisabled || m_RectsCache == null || string.IsNullOrEmpty(m_SelectedSpriteRectGUID))
                    return null;

                var guid = new GUID(m_SelectedSpriteRectGUID);
                return m_RectsCache.FirstOrDefault(x => x.spriteID == guid);
            }
            set
            {
                if (editingDisabled)
                    return;

                var oldSelected = m_SelectedSpriteRectGUID;
                m_SelectedSpriteRectGUID = value != null ? value.spriteID.ToString() : new GUID().ToString();
                if (oldSelected != m_SelectedSpriteRectGUID)
                {
                    if (m_MainViewIMGUIElement != null)
                        m_MainViewIMGUIElement.MarkDirtyRepaint();
                    if (m_MainViewElement != null)
                    {
                        m_MainViewElement.MarkDirtyRepaint();
                        using (var e = SpriteSelectionChangeEvent.GetPooled())
                        {
                            e.target = m_ModuleViewElement;
                            m_MainViewElement.SendEvent(e);
                        }
                    }
                }
            }
        }

        public ISpriteEditorDataProvider spriteEditorDataProvider
        {
            get { return m_SpriteDataProvider; }
        }

        public bool enableMouseMoveEvent
        {
            set { wantsMouseMove = value; }
        }

        public void RequestRepaint()
        {
            if (focusedWindow != this)
                Repaint();
            else
                m_RequestRepaint = true;
        }

        public void SetDataModified()
        {
            textureIsDirty = true;
        }

        public Rect windowDimension
        {
            get { return textureViewRect; }
        }

        public ITexture2D previewTexture
        {
            get { return m_Texture; }
        }

        public bool editingDisabled => EditorApplication.isPlayingOrWillChangePlaymode || m_AssetNotEditable;

        public void SetPreviewTexture(UnityTexture2D texture, int width, int height)
        {
            m_Texture = new PreviewTexture2D(texture, width, height);
        }

        public void ApplyOrRevertModification(bool apply)
        {
            if (apply)
                DoApply();
            else
                DoRevert();
        }

        internal class PreviewTexture2D : Texture2DWrapper
        {
            private int m_ActualWidth = 0;
            private int m_ActualHeight = 0;

            public PreviewTexture2D(UnityTexture2D t, int width, int height)
                : base(t)
            {
                m_ActualWidth = width;
                m_ActualHeight = height;
            }

            public override int width
            {
                get { return m_ActualWidth; }
            }

            public override int height
            {
                get { return m_ActualHeight; }
            }
        }

        public T GetDataProvider<T>() where T : class
        {
            return m_SpriteDataProvider != null ? m_SpriteDataProvider.GetDataProvider<T>() : null;
        }

        public VisualElement GetMainVisualContainer()
        {
            return m_ModuleViewElement;
        }

        static internal void OnTextureReimport(SpriteEditorWindow win, string path)
        {
            if (win.selectedAssetPath == path)
            {
                win.ResetOnNextRepaint();
            }
        }

        [MenuItem("Window/2D/Sprite Editor", false, 0)]
        static private void OpenSpriteEditorWindow()
        {
            SpriteEditorWindow.GetWindow(Selection.activeObject);
        }
    }

    internal class SpriteEditorTexturePostprocessor : AssetPostprocessor
    {
        public override int GetPostprocessOrder()
        {
            return 1;
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            UnityEngine.Object[] wins = Resources.FindObjectsOfTypeAll(typeof(SpriteEditorWindow));
            SpriteEditorWindow win = wins.Length > 0 ? (EditorWindow)(wins[0]) as SpriteEditorWindow : null;
            if (win != null)
            {
                foreach (var deletedAsset in deletedAssets)
                    SpriteEditorWindow.OnTextureReimport(win, deletedAsset);

                foreach (var importedAsset in importedAssets)
                    SpriteEditorWindow.OnTextureReimport(win, importedAsset);
            }
        }
    }

    internal class SpriteSelectionChangeEvent : EventBase<SpriteSelectionChangeEvent>
    {
    }
}
