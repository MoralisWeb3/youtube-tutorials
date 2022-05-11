using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEditor.U2D.Common;
using System.Collections.Generic;
using UnityEditor.U2D.SpriteShape;

namespace UnityEditor.U2D
{
    [CustomEditor(typeof(UnityEngine.U2D.SpriteShape)), CanEditMultipleObjects]
    public class SpriteShapeEditor : Editor, IAngleRangeCache
    {
        private static class Contents
        {
            public static readonly GUIContent fillTextureLabel = new GUIContent("Texture", "Fill texture used for Shape Fill.");
            public static readonly GUIContent fillScaleLabel = new GUIContent("Offset", "Determines Border Offset for Shape.");
            public static readonly GUIContent useSpriteBorderLabel = new GUIContent("Use Sprite Borders", "Draw Sprite Borders on discontinuities");
            public static readonly GUIContent cornerTypeLabel = new GUIContent("Corner Type", "Corner type sprite used.");
            public static readonly GUIContent controlPointsLabel = new GUIContent("Control Points");
            public static readonly GUIContent fillLabel = new GUIContent("Fill");
            public static readonly GUIContent cornerLabel = new GUIContent("Corners");
            public static readonly GUIContent cornerListLabel = new GUIContent("Corner List");
            public static readonly GUIContent cornerSpriteTypeLabel = new GUIContent("Corner Sprite");
            public static readonly GUIContent angleRangesLabel = new GUIContent("Angle Ranges");
            public static readonly GUIContent spritesLabel = new GUIContent("Sprites");
            public static readonly GUIContent angleRangeLabel = new GUIContent("Angle Range ({0})");
            public static readonly GUIContent wrapModeErrorLabel = new GUIContent("Fill texture must have wrap modes set to Repeat. Please re-import.");

            public static readonly Color proBackgroundColor = new Color32(49, 77, 121, 255);
            public static readonly Color proBackgroundRangeColor = new Color32(25, 25, 25, 128);
            public static readonly Color proColor1 = new Color32(10, 46, 42, 255);
            public static readonly Color proColor2 = new Color32(33, 151, 138, 255);
            public static readonly Color defaultColor1 = new Color32(25, 61, 57, 255);
            public static readonly Color defaultColor2 = new Color32(47, 166, 153, 255);
            public static readonly Color defaultBackgroundColor = new Color32(64, 92, 136, 255);
        }

        private SerializedProperty m_FillTextureProp;
        private SerializedProperty m_AngleRangesProp;
        private SerializedProperty m_CornerSpritesProp;
        private SerializedProperty m_FillOffsetProp;
        private SerializedProperty m_UseSpriteBordersProp;

        private ReorderableList m_AngleRangeSpriteList = null;
        private ReorderableList m_EmptySpriteList = null;

        [SerializeField]
        private float m_PreviewAngle = 0f;
        [SerializeField]
        private int m_SelectedIndex;
        private const int kInvalidMinimum = -1;
        private Rect m_AngleRangeRect;
        private AngleRangeController controller;
        private AngleRange m_CurrentAngleRange;
        private Dictionary<int, int> m_SpriteSelection = new Dictionary<int, int>();


        private Sprite m_PreviewSprite;
        private Mesh m_PreviewSpriteMesh;
        private Mesh previewSpriteMesh
        {
            get
            {
                if (m_PreviewSpriteMesh == null)
                {
                    m_PreviewSpriteMesh = new Mesh();
                    m_PreviewSpriteMesh.MarkDynamic();
                    m_PreviewSpriteMesh.hideFlags = HideFlags.DontSave;
                }

                return m_PreviewSpriteMesh;
            }
        }

        public List<AngleRange> angleRanges
        {
            get
            {
                if (spriteShape == null)
                    return new List<AngleRange>();
                Debug.Assert(spriteShape != null);
                return spriteShape.angleRanges;
            }
        }

        public int selectedIndex
        {
            get { return m_SelectedIndex; }
            set { m_SelectedIndex = value; }
        }

        bool isSelectedIndexValid
        {
            get { return (selectedIndex != kInvalidMinimum && selectedIndex < angleRanges.Count); }
        }

        public float previewAngle
        {
            get { return m_PreviewAngle; }
            set
            {
                m_PreviewAngle = value;
                SessionState.SetFloat("SpriteShape/PreviewAngle/" + target.GetInstanceID(), value);
            }
        }

        public UnityEngine.U2D.SpriteShape spriteShape
        {
            get
            {
                if (target == null)
                    return null;
                return target as UnityEngine.U2D.SpriteShape;
            }
        }

        public void RegisterUndo(string name)
        {
            Undo.RegisterCompleteObjectUndo(spriteShape, name);
            Undo.RegisterCompleteObjectUndo(this, name);
            EditorUtility.SetDirty(spriteShape);
        }

        public void OnEnable()
        {
            if (targets == null || targets.Length == 0)
                return;

            m_PreviewAngle = SessionState.GetFloat("SpriteShape/PreviewAngle/" + target.GetInstanceID(), m_PreviewAngle);

            m_FillTextureProp = this.serializedObject.FindProperty("m_FillTexture");
            m_UseSpriteBordersProp = serializedObject.FindProperty("m_UseSpriteBorders");
            m_AngleRangesProp = this.serializedObject.FindProperty("m_Angles");
            m_CornerSpritesProp = this.serializedObject.FindProperty("m_CornerSprites");
            m_FillOffsetProp = this.serializedObject.FindProperty("m_FillOffset");

            selectedIndex = SpriteShapeEditorUtility.GetRangeIndexFromAngle(angleRanges, m_PreviewAngle);

            SetupAngleRangeController();

            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private void SetupAngleRangeController()
        {
            var radius = 125f;
            var angleOffset = -90f;
            var color1 = Contents.defaultColor1;
            var color2 = Contents.defaultColor2;

            if (!EditorGUIUtility.isProSkin)
            {
                color1 = Contents.proColor1;
                color2 = Contents.proColor2;
            }

            controller = new AngleRangeController();
            controller.view = new AngleRangeView();
            controller.cache = this;
            controller.radius = radius;
            controller.angleOffset = angleOffset;
            controller.gradientMin = color1;
            controller.gradientMid = color2;
            controller.gradientMax = color1;
            controller.snap = true;
            controller.selectionChanged += OnSelectionChange;

            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            CreateReorderableSpriteList();

            EditorApplication.delayCall += () =>
                {
                    m_CurrentAngleRange = controller.selectedAngleRange;
                };
        }

        private void OnDestroy()
        {
            if (m_PreviewSpriteMesh)
                Object.DestroyImmediate(m_PreviewSpriteMesh);

            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void UndoRedoPerformed()
        {
            OnSelectionChange();
        }

        private void OnSelelectSpriteCallback(ReorderableList list)
        {
            if (selectedIndex >= 0)
            {
                SetPreviewSpriteIndex(selectedIndex, list.index);
            }
        }

        private bool OnCanAddCallback(ReorderableList list)
        {
            return (list.count < 64);
        }

        private void OnRemoveSprite(ReorderableList list)
        {
            var count = list.count;
            var index = list.index;

            ReorderableList.defaultBehaviours.DoRemoveButton(list);

            if (list.count < count && list.count > 0)
            {
                list.index = Mathf.Clamp(index, 0, list.count - 1);
                OnSelelectSpriteCallback(list);
            }
        }

        private void DrawSpriteListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, Contents.spritesLabel);
            HandleAngleSpriteListGUI(rect);
        }

        private void DrawSpriteListElement(Rect rect, int index, bool selected, bool focused)
        {
            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;
            var sprite = m_AngleRangesProp.GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("m_Sprites").GetArrayElementAtIndex(index);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, sprite, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                m_AngleRangeSpriteList.index = index;
                OnSelelectSpriteCallback(m_AngleRangeSpriteList);
            }
        }

        public void DrawHeader(GUIContent content)
        {
            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
        }

        private void SetPreviewSpriteIndex(int rangeIndex, int index)
        {
            m_SpriteSelection[rangeIndex] = index;
        }

        private int GetPreviewSpriteIndex(int rangeIndex)
        {
            int index;
            m_SpriteSelection.TryGetValue(rangeIndex, out index);

            return index;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            DrawHeader(Contents.controlPointsLabel);
            EditorGUILayout.PropertyField(m_UseSpriteBordersProp, Contents.useSpriteBorderLabel);


            EditorGUILayout.Space();
            DrawHeader(Contents.fillLabel);
            EditorGUILayout.PropertyField(m_FillTextureProp, Contents.fillTextureLabel);
            EditorGUILayout.Slider(m_FillOffsetProp, -0.5f, 0.5f, Contents.fillScaleLabel);


            if (m_FillTextureProp.objectReferenceValue != null)
            {
                var fillTex = m_FillTextureProp.objectReferenceValue as Texture2D;
                if (fillTex.wrapModeU != TextureWrapMode.Repeat || fillTex.wrapModeV != TextureWrapMode.Repeat)
                    EditorGUILayout.HelpBox(Contents.wrapModeErrorLabel.text, MessageType.Warning);
            }

            EditorGUILayout.Space();
            DrawHeader(Contents.angleRangesLabel);
            DoRangesGUI();

            if (targets.Length == 1)
            {
                DoRangeInspector();
                DoCreateRangeButton();
            }

            EditorGUILayout.Space();
            DrawHeader(Contents.cornerLabel);

            HashSet<Sprite> tightMeshSprites = new HashSet<Sprite>();

            for (int i = 0; i < angleRanges.Count; ++i)
            {
                AngleRange angleRange = angleRanges[i];
                foreach (Sprite sprite in angleRange.sprites)
                {
                    if (sprite != null)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(sprite);
                        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                        if (importer != null)
                        { 
                            TextureImporterSettings textureSettings = new TextureImporterSettings();
                            importer.ReadTextureSettings(textureSettings);
                            if (textureSettings.spriteMeshType == SpriteMeshType.Tight)
                                tightMeshSprites.Add(sprite);
                        }
                    }
                }
            }

            EditorGUIUtility.labelWidth = EditorGUIUtility.labelWidth + 20f;

            for (int i = 0; i < m_CornerSpritesProp.arraySize; ++i)
            {
                var m_CornerProp = m_CornerSpritesProp.GetArrayElementAtIndex(i);
                var m_CornerType = m_CornerProp.FindPropertyRelative("m_CornerType");
                var m_CornerSprite = m_CornerProp.FindPropertyRelative("m_Sprites").GetArrayElementAtIndex(0);

                EditorGUILayout.PropertyField(m_CornerSprite, new GUIContent(m_CornerType.enumDisplayNames[m_CornerType.intValue]));

                var sprite = m_CornerSprite.objectReferenceValue as Sprite;
                if (sprite != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(sprite);
                    TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    if (importer != null)
                    { 
                        TextureImporterSettings textureSettings = new TextureImporterSettings();
                        importer.ReadTextureSettings(textureSettings);
                        if (textureSettings.spriteMeshType == SpriteMeshType.Tight)
                            tightMeshSprites.Add(sprite);
                    }
                }
            }


            EditorGUIUtility.labelWidth = 0;

            serializedObject.ApplyModifiedProperties();

            if (tightMeshSprites.Count > 0)
            {
                int i = 0;
                string tightSpriteWarning = "The following sprites ( ";
                foreach (var sprite in tightMeshSprites)
                {
                    string appendString = (i < tightMeshSprites.Count - 1) ? ", " : " ) ";
                    tightSpriteWarning += (sprite.name + appendString);
                    ++i;
                }
                tightSpriteWarning += "are imported as Sprites using Tight mesh. This can lead to Rendering Artifacts. Please use Full Rect.";
                EditorGUILayout.HelpBox(tightSpriteWarning, MessageType.Warning);
            }

            controller.view.DoCreateRangeTooltip();
        }

        private void DoRangeInspector()
        {
            var start = 0f;
            var end = 0f;
            var order = 0;

            if (m_CurrentAngleRange != null)
            {
                start = m_CurrentAngleRange.start;
                end = m_CurrentAngleRange.end;
                order = m_CurrentAngleRange.order;
            }

            using (new EditorGUI.DisabledGroupScope(m_CurrentAngleRange == null))
            {
                DrawHeader(new GUIContent(string.Format(Contents.angleRangeLabel.text, (end - start))));

                EditorGUIUtility.labelWidth = 0f;
                EditorGUI.BeginChangeCheck();

                RangeField(ref start, ref end, ref order);

                if (EditorGUI.EndChangeCheck() && m_CurrentAngleRange != null)
                {
                    RegisterUndo("Set Range");

                    m_CurrentAngleRange.order = order;
                    controller.SetRange(m_CurrentAngleRange, start, end);

                    if (start >= end)
                        controller.RemoveInvalidRanges();
                }

                EditorGUILayout.Space();

                var arSize = m_AngleRangesProp.arraySize;
                
                if (m_AngleRangeSpriteList != null && arSize > 0)
                    m_AngleRangeSpriteList.DoLayoutList();
                else
                    m_EmptySpriteList.DoLayoutList();
            }
        }

        private void DoCreateRangeButton()
        {
            if (selectedIndex != kInvalidMinimum && angleRanges.Count != 0)
                return;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create Range", GUILayout.MaxWidth(100f)))
            {
                RegisterUndo("Create Range");
                controller.CreateRange();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void RangeField(ref float start, ref float end, ref int order)
        {
            var values = new int[] { Mathf.RoundToInt(-start), Mathf.RoundToInt(-end), order };
            var labels = new GUIContent[] { new GUIContent("Start"), new GUIContent("End"), new GUIContent("Order") };

            var position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            SpriteShapeEditorGUI.MultiDelayedIntField(position, labels, values, 40f);
            if (EditorGUI.EndChangeCheck())
            {
                start = -1f * values[0];
                end = -1f * values[1];
                order = values[2];
            }
        }

        private void HandleAngleSpriteListGUI(Rect rect)
        {
            if (m_CurrentAngleRange == null || !isSelectedIndexValid)
                return;

            var currentEvent = Event.current;
            var usedEvent = false;
            var sprites = m_AngleRangesProp.GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("m_Sprites");
            switch (currentEvent.type)
            {
                case EventType.DragExited:
                    if (GUI.enabled)
                        HandleUtility.Repaint();
                    break;

                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (rect.Contains(currentEvent.mousePosition) && GUI.enabled)
                    {
                        // Check each single object, so we can add multiple objects in a single drag.
                        var didAcceptDrag = false;
                        var references = DragAndDrop.objectReferences;
                        foreach (var obj in references)
                        {
                            if (obj is Sprite)
                            {
                                Sprite spr = obj as Sprite;
                                if (spr.texture != null)
                                {
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                    if (currentEvent.type == EventType.DragPerform && sprites.arraySize < 64)
                                    {
                                        sprites.InsertArrayElementAtIndex(sprites.arraySize);
                                        var spriteProp = sprites.GetArrayElementAtIndex(sprites.arraySize - 1);
                                        spriteProp.objectReferenceValue = obj;
                                        didAcceptDrag = true;
                                        DragAndDrop.activeControlID = 0;
                                    }
                                }
                            }
                        }

                        serializedObject.ApplyModifiedProperties();

                        if (didAcceptDrag)
                        {
                            GUI.changed = true;
                            DragAndDrop.AcceptDrag();
                            usedEvent = true;
                        }
                    }
                    break;
            }

            if (usedEvent)
                currentEvent.Use();
        }

        private void DoRangesGUI()
        {
            var radius = controller.radius;

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            var rect = EditorGUILayout.GetControlRect(false, radius * 2f);

            if (Event.current.type == EventType.Repaint)
                m_AngleRangeRect = rect;

            {   //Draw background
                var backgroundColor = Contents.proBackgroundColor;
                var backgroundRangeColor = Contents.proBackgroundRangeColor;

                if (!EditorGUIUtility.isProSkin)
                {
                    backgroundColor = Contents.defaultBackgroundColor;
                    backgroundRangeColor.a = 0.1f;
                }
                var c = Handles.color;
                Handles.color = backgroundRangeColor;
                SpriteShapeHandleUtility.DrawSolidArc(rect.center, Vector3.forward, Vector3.right, 360f, radius, AngleRangeGUI.kRangeWidth);
                Handles.color = backgroundColor;
                Handles.DrawSolidDisc(rect.center, Vector3.forward, radius - AngleRangeGUI.kRangeWidth + 1f);
                Handles.color = c;
            }

            if (targets.Length == 1)
            {
                {   //Draw fill texture and sprite preview
                    SpriteShapeHandleUtility.DrawTextureArc(
                        m_FillTextureProp.objectReferenceValue as Texture, 100.0f,
                        rect.center, Vector3.forward, Quaternion.AngleAxis(m_PreviewAngle, Vector3.forward) * Vector3.right, 180f,
                        radius - AngleRangeGUI.kRangeWidth);

                    var rectSize = Vector2.one * (radius - AngleRangeGUI.kRangeWidth) * 2f;
                    rectSize.y *= 0.33f;
                    var spriteRect = new Rect(rect.center - rectSize * 0.5f, rectSize);
                    DrawSpritePreview(spriteRect);
                    HandleSpritePreviewCycle(spriteRect);
                }

                controller.rect = m_AngleRangeRect;
                controller.OnGUI();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private void CreateReorderableSpriteList()
        {
            if (m_EmptySpriteList == null)
            {
                m_EmptySpriteList = new ReorderableList(new List<Sprite>(), typeof(Sprite), false, true, false, false)
                {
                    drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, Contents.spritesLabel); }
                };
            }

            m_AngleRangeSpriteList = null;

            serializedObject.UpdateIfRequiredOrScript();

            Debug.Assert(angleRanges.Count == m_AngleRangesProp.arraySize);
            Debug.Assert(selectedIndex < angleRanges.Count || selectedIndex == 0);

            if (targets.Length == 1 && isSelectedIndexValid)
            {
                var spritesProp = m_AngleRangesProp.GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("m_Sprites");
                m_AngleRangeSpriteList = new ReorderableList(spritesProp.serializedObject, spritesProp)
                {
                    drawElementCallback = DrawSpriteListElement,
                    drawHeaderCallback = DrawSpriteListHeader,
                    onSelectCallback = OnSelelectSpriteCallback,
                    onRemoveCallback = OnRemoveSprite,
                    onCanAddCallback = OnCanAddCallback,
                    elementHeight = EditorGUIUtility.singleLineHeight + 6f
                };
            }
        }

        private void DrawSpritePreview(Rect rect)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (!isSelectedIndexValid)
                return;

            var sprites = angleRanges[selectedIndex].sprites;

            if (sprites.Count == 0)
                return;

            var selectedSpriteIndex = GetPreviewSpriteIndex(selectedIndex);

            if (selectedSpriteIndex == kInvalidMinimum || selectedSpriteIndex >= sprites.Count)
                return;

            var sprite = sprites[selectedSpriteIndex];

            if (sprite == null)
                return;

            if (m_PreviewSprite != sprite)
            {
                m_PreviewSprite = sprite;
                EditorSpriteGUIUtility.DrawSpriteInRectPrepare(rect, sprite, EditorSpriteGUIUtility.FitMode.Tiled, true, true, previewSpriteMesh);
            }

            var material = EditorSpriteGUIUtility.spriteMaterial;
            material.mainTexture = EditorSpriteGUIUtility.GetOriginalSpriteTexture(sprite);

            EditorSpriteGUIUtility.DrawMesh(previewSpriteMesh, material, rect.center, Quaternion.AngleAxis(m_PreviewAngle, Vector3.forward), new Vector3(1f, -1f, 1f));
        }

        private void HandleSpritePreviewCycle(Rect rect)
        {
            if (!isSelectedIndexValid)
                return;

            Debug.Assert(m_AngleRangeSpriteList != null);

            var spriteIndex = GetPreviewSpriteIndex(selectedIndex);
            var sprites = angleRanges[selectedIndex].sprites;

            var ev = Event.current;
            if (ev.type == EventType.MouseDown && ev.button == 0 && HandleUtility.nearestControl == 0 &&
                ContainsPosition(rect, ev.mousePosition, m_PreviewAngle) && spriteIndex != kInvalidMinimum && sprites.Count > 0)
            {
                spriteIndex = Mathf.RoundToInt(Mathf.Repeat(spriteIndex + 1f, sprites.Count));
                SetPreviewSpriteIndex(selectedIndex, spriteIndex);

                m_AngleRangeSpriteList.GrabKeyboardFocus();
                m_AngleRangeSpriteList.index = spriteIndex;

                ev.Use();
            }
        }

        private bool ContainsPosition(Rect rect, Vector2 position, float angle)
        {
            Vector2 delta = position - rect.center;
            position = (Vector2)(Quaternion.AngleAxis(-angle, Vector3.forward) * (Vector3)delta) + rect.center;
            return rect.Contains(position);
        }
    }
}
