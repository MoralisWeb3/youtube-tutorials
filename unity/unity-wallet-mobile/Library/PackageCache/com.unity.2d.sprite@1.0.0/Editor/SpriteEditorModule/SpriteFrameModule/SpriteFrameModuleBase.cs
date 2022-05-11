using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEditorInternal;

namespace UnityEditor.U2D.Sprites
{
    internal class SpriteRectModel : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<SpriteRect> m_SpriteRects;
        private HashSet<string> m_Names;
        private HashSet<long> m_InternalIds;

        private IReadOnlyList<SpriteRect> m_SpriteReadOnlyList;
        public IReadOnlyList<SpriteRect> spriteRects
        {
            get { return m_SpriteReadOnlyList; }
        }

        private SpriteRectModel()
        {
            m_Names = new HashSet<string>();
            m_InternalIds = new HashSet<long>();
        }

        public void SetSpriteRects(List<SpriteRect> newSpriteRects)
        {
            m_SpriteRects = newSpriteRects;
            foreach (var spriteRect in m_SpriteRects)
            {
                m_Names.Add(spriteRect.name);
                m_InternalIds.Add(spriteRect.internalID);
            }
            m_SpriteReadOnlyList = m_SpriteRects.AsReadOnly();
        }

        public int FindIndex(Predicate<SpriteRect> match)
        {
            int i = 0;
            foreach (var spriteRect in m_SpriteRects)
            {
                if (match.Invoke(spriteRect))
                    return i;
                i++;
            }
            return -1;
        }

        public void Clear()
        {
            m_SpriteRects = new List<SpriteRect>();
            m_InternalIds.Clear();
            m_Names.Clear();
            m_SpriteReadOnlyList = m_SpriteRects.AsReadOnly();
        }

        public bool Add(SpriteRect spriteRect)
        {
            if (m_Names.Contains(spriteRect.name))
                return false;
            if (spriteRect.internalID != 0 && m_InternalIds.Contains(spriteRect.internalID))
                return false;
            m_Names.Add(spriteRect.name);
            if (spriteRect.internalID != 0)
                m_InternalIds.Add(spriteRect.internalID);
            m_SpriteRects.Add(spriteRect);
            m_SpriteReadOnlyList = m_SpriteRects.AsReadOnly();
            return true;
        }

        public void Remove(SpriteRect spriteRect)
        {
            m_Names.Remove(spriteRect.name);
            if (spriteRect.internalID != 0)
                m_InternalIds.Remove(spriteRect.internalID);
            m_SpriteRects.Remove(spriteRect);
            m_SpriteReadOnlyList = m_SpriteRects.AsReadOnly();
        }

        public bool HasName(string rectName)
        {
            return m_Names.Contains(rectName);
        }

        public bool HasInternalID(long internalID)
        {
            return m_InternalIds.Contains(internalID);
        }

        public List<SpriteRect> GetSpriteRects()
        {
            return m_SpriteRects;
        }

        public void Rename(string oldName, string newName)
        {
            m_Names.Remove(oldName);
            m_Names.Add(newName);
            m_SpriteReadOnlyList = m_SpriteRects.AsReadOnly();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {}

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_SpriteReadOnlyList = m_SpriteRects.AsReadOnly();
            m_Names.Clear();
            m_InternalIds.Clear();
            foreach (var sprite in m_SpriteReadOnlyList)
            {
                m_Names.Add(sprite.name);
                m_InternalIds.Add(sprite.internalID);
            }
        }
    }

    internal abstract partial class SpriteFrameModuleBase : SpriteEditorModuleBase
    {
        protected static UnityType spriteType = UnityType.FindTypeByName("Sprite");

        protected SpriteRectModel m_RectsCache;
        protected ITextureDataProvider m_TextureDataProvider;
        protected ISpriteEditorDataProvider m_SpriteDataProvider;
        string m_ModuleName;

        internal enum PivotUnitMode
        {
            Normalized,
            Pixels
        }

        private PivotUnitMode m_PivotUnitMode = PivotUnitMode.Normalized;

        protected SpriteFrameModuleBase(string name, ISpriteEditor sw, IEventSystem es, IUndoSystem us, IAssetDatabase ad)
        {
            spriteEditor = sw;
            eventSystem = es;
            undoSystem = us;
            assetDatabase = ad;
            m_ModuleName = name;
        }

        // implements ISpriteEditorModule

        public override void OnModuleActivate()
        {
            spriteImportMode = SpriteFrameModule.GetSpriteImportMode(spriteEditor.GetDataProvider<ISpriteEditorDataProvider>());
            m_TextureDataProvider = spriteEditor.GetDataProvider<ITextureDataProvider>();
            m_SpriteDataProvider = spriteEditor.GetDataProvider<ISpriteEditorDataProvider>();
            int width, height;
            m_TextureDataProvider.GetTextureActualWidthAndHeight(out width, out height);
            textureActualWidth = width;
            textureActualHeight = height;
            m_RectsCache = ScriptableObject.CreateInstance<SpriteRectModel>();
            m_RectsCache.hideFlags = HideFlags.HideAndDontSave;
            var spriteList = m_SpriteDataProvider.GetSpriteRects().ToList();
            m_RectsCache.SetSpriteRects(spriteList);
            spriteEditor.spriteRects = spriteList;
            if (spriteEditor.selectedSpriteRect != null)
                spriteEditor.selectedSpriteRect = m_RectsCache.spriteRects.FirstOrDefault(x => x.spriteID == spriteEditor.selectedSpriteRect.spriteID);
            AddMainUI(spriteEditor.GetMainVisualContainer());
            undoSystem.RegisterUndoCallback(UndoCallback);
        }

        public override void OnModuleDeactivate()
        {
            if (m_RectsCache != null)
            {
                undoSystem.ClearUndo(m_RectsCache);
                ScriptableObject.DestroyImmediate(m_RectsCache);
                m_RectsCache = null;
            }
            undoSystem.UnregisterUndoCallback(UndoCallback);
            RemoveMainUI(spriteEditor.GetMainVisualContainer());
        }

        public override bool ApplyRevert(bool apply)
        {
            if (apply)
            {
                if (containsMultipleSprites)
                {
                    var oldNames = new List<string>();
                    var newNames = new List<string>();
                    var ids = new List<long>();
                    var names = new List<string>();

                    foreach (var spriteRect in m_RectsCache.spriteRects)
                    {
                        if (string.IsNullOrEmpty(spriteRect.name))
                            spriteRect.name = "Empty";

                        if (!string.IsNullOrEmpty(spriteRect.originalName))
                        {
                            oldNames.Add(spriteRect.originalName);
                            newNames.Add(spriteRect.name);
                        }

                        if (spriteRect.m_RegisterInternalID)
                        {
                            ids.Add(spriteRect.internalID);
                            names.Add(spriteRect.name);
                        }
                        spriteRect.m_RegisterInternalID = false;
                    }
                    var so = new SerializedObject(m_SpriteDataProvider.targetObject);
                    if (so.isValid && ids.Count > 0)
                    {
                        ImportSettingInternalID.RegisterInternalID(so, spriteType, ids, names);
                        so.ApplyModifiedPropertiesWithoutUndo();
                    }

                    AssetImporter assetImporter = m_SpriteDataProvider.targetObject as AssetImporter;
                    if (oldNames.Count > 0 && assetImporter != null)
                    {
                        assetImporter.RenameSubAssets(spriteType.persistentTypeID, oldNames.ToArray(), newNames.ToArray());
                        so.ApplyModifiedPropertiesWithoutUndo();
                    }
                }
                var array = m_RectsCache != null ? m_RectsCache.spriteRects.ToArray() : null;
                m_SpriteDataProvider.SetSpriteRects(array);
                if (m_RectsCache != null)
                    undoSystem.ClearUndo(m_RectsCache);
            }
            else
            {
                if (m_RectsCache != null)
                {
                    undoSystem.ClearUndo(m_RectsCache);
                    var spriteList = m_SpriteDataProvider.GetSpriteRects().ToList();
                    m_RectsCache.SetSpriteRects(spriteList);
                    spriteEditor.spriteRects = spriteList;
                    if (spriteEditor.selectedSpriteRect != null)
                        spriteEditor.selectedSpriteRect = m_RectsCache.spriteRects.FirstOrDefault(x => x.spriteID == spriteEditor.selectedSpriteRect.spriteID);
                }
            }

            return true;
        }

        public override string moduleName
        {
            get { return m_ModuleName; }
        }

        // injected interfaces
        protected IEventSystem eventSystem
        {
            get;
            private set;
        }

        protected IUndoSystem undoSystem
        {
            get;
            private set;
        }

        protected IAssetDatabase assetDatabase
        {
            get;
            private set;
        }

        protected SpriteRect selected
        {
            get { return spriteEditor.selectedSpriteRect; }
            set { spriteEditor.selectedSpriteRect = value; }
        }

        protected SpriteImportMode spriteImportMode
        {
            get; private set;
        }

        protected string spriteAssetPath
        {
            get { return assetDatabase.GetAssetPath(m_TextureDataProvider.texture); }
        }

        public bool hasSelected
        {
            get { return spriteEditor.selectedSpriteRect != null; }
        }

        public SpriteAlignment selectedSpriteAlignment
        {
            get { return selected.alignment; }
        }

        public Vector2 selectedSpritePivot
        {
            get { return selected.pivot; }
        }

        private Vector2 selectedSpritePivotInCurUnitMode
        {
            get
            {
                return m_PivotUnitMode == PivotUnitMode.Pixels
                    ? ConvertFromNormalizedToRectSpace(selectedSpritePivot, selectedSpriteRect)
                    : selectedSpritePivot;
            }
        }

        public int CurrentSelectedSpriteIndex()
        {
            if (m_RectsCache != null && selected != null)
                return m_RectsCache.FindIndex(x => x.spriteID == selected.spriteID);
            return -1;
        }

        public Vector4 selectedSpriteBorder
        {
            get { return ClampSpriteBorderToRect(selected.border, selected.rect); }
            set
            {
                undoSystem.RegisterCompleteObjectUndo(m_RectsCache, "Change Sprite Border");
                spriteEditor.SetDataModified();
                selected.border = ClampSpriteBorderToRect(value, selected.rect);
            }
        }

        public Rect selectedSpriteRect
        {
            get { return selected.rect; }
            set
            {
                undoSystem.RegisterCompleteObjectUndo(m_RectsCache, "Change Sprite rect");
                spriteEditor.SetDataModified();
                selected.rect = ClampSpriteRect(value, textureActualWidth, textureActualHeight);
            }
        }

        public string selectedSpriteName
        {
            get { return selected.name; }
            set
            {
                if (selected.name == value)
                    return;
                if (m_RectsCache.HasName(value))
                    return;

                undoSystem.RegisterCompleteObjectUndo(m_RectsCache, "Change Sprite Name");
                spriteEditor.SetDataModified();

                string oldName = selected.name;
                string newName = InternalEditorUtility.RemoveInvalidCharsFromFileName(value, true);

                // These can only be changed in sprite multiple mode
                if (string.IsNullOrEmpty(selected.originalName) && (newName != oldName))
                    selected.originalName = oldName;

                // Is the name empty?
                if (string.IsNullOrEmpty(newName))
                    newName = oldName;

                m_RectsCache.Rename(oldName, newName);
                selected.name = newName;
            }
        }

        public int spriteCount
        {
            get { return m_RectsCache.spriteRects.Count; }
        }

        public Vector4 GetSpriteBorderAt(int i)
        {
            return m_RectsCache.spriteRects[i].border;
        }

        public Rect GetSpriteRectAt(int i)
        {
            return m_RectsCache.spriteRects[i].rect;
        }

        public int textureActualWidth { get; private set; }
        public int textureActualHeight { get; private set; }

        public void SetSpritePivotAndAlignment(Vector2 pivot, SpriteAlignment alignment)
        {
            undoSystem.RegisterCompleteObjectUndo(m_RectsCache, "Change Sprite Pivot");
            spriteEditor.SetDataModified();
            selected.alignment = alignment;
            selected.pivot = SpriteEditorUtility.GetPivotValue(alignment, pivot);
        }

        public bool containsMultipleSprites
        {
            get { return spriteImportMode == SpriteImportMode.Multiple; }
        }

        protected void SnapPivotToSnapPoints(Vector2 pivot, out Vector2 outPivot, out SpriteAlignment outAlignment)
        {
            Rect rect = selectedSpriteRect;

            // Convert from normalized space to texture space
            Vector2 texturePos = new Vector2(rect.xMin + rect.width * pivot.x, rect.yMin + rect.height * pivot.y);

            Vector2[] snapPoints = GetSnapPointsArray(rect);

            // Snapping is now a firm action, it will always snap to one of the snapping points.
            SpriteAlignment snappedAlignment = SpriteAlignment.Custom;
            float nearestDistance = float.MaxValue;
            for (int alignment = 0; alignment < snapPoints.Length; alignment++)
            {
                float distance = (texturePos - snapPoints[alignment]).magnitude * m_Zoom;
                if (distance < nearestDistance)
                {
                    snappedAlignment = (SpriteAlignment)alignment;
                    nearestDistance = distance;
                }
            }

            outAlignment = snappedAlignment;
            outPivot = ConvertFromTextureToNormalizedSpace(snapPoints[(int)snappedAlignment], rect);
        }

        protected void SnapPivotToPixels(Vector2 pivot, out Vector2 outPivot, out SpriteAlignment outAlignment)
        {
            outAlignment = SpriteAlignment.Custom;

            Rect rect = selectedSpriteRect;
            float unitsPerPixelX = 1.0f / rect.width;
            float unitsPerPixelY = 1.0f / rect.height;
            outPivot.x = Mathf.Round(pivot.x / unitsPerPixelX) * unitsPerPixelX;
            outPivot.y = Mathf.Round(pivot.y / unitsPerPixelY) * unitsPerPixelY;
        }

        private void UndoCallback()
        {
            UIUndoCallback();
        }

        protected static Rect ClampSpriteRect(Rect rect, float maxX, float maxY)
        {
            // Clamp rect to width height
            rect = FlipNegativeRect(rect);
            Rect newRect = new Rect();

            newRect.xMin = Mathf.Clamp(rect.xMin, 0, maxX - 1);
            newRect.yMin = Mathf.Clamp(rect.yMin, 0, maxY - 1);
            newRect.xMax = Mathf.Clamp(rect.xMax, 1, maxX);
            newRect.yMax = Mathf.Clamp(rect.yMax, 1, maxY);

            // Prevent width and height to be 0 value after clamping.
            if (Mathf.RoundToInt(newRect.width) == 0)
                newRect.width = 1;
            if (Mathf.RoundToInt(newRect.height) == 0)
                newRect.height = 1;

            return SpriteEditorUtility.RoundedRect(newRect);
        }

        protected static Rect FlipNegativeRect(Rect rect)
        {
            Rect newRect = new Rect();

            newRect.xMin = Mathf.Min(rect.xMin, rect.xMax);
            newRect.yMin = Mathf.Min(rect.yMin, rect.yMax);
            newRect.xMax = Mathf.Max(rect.xMin, rect.xMax);
            newRect.yMax = Mathf.Max(rect.yMin, rect.yMax);

            return newRect;
        }

        protected static Vector4 ClampSpriteBorderToRect(Vector4 border, Rect rect)
        {
            Rect flipRect = FlipNegativeRect(rect);
            float w = flipRect.width;
            float h = flipRect.height;

            Vector4 newBorder = new Vector4();

            // Make sure borders are within the width/height and left < right and top < bottom
            newBorder.x = Mathf.RoundToInt(Mathf.Clamp(border.x, 0, Mathf.Min(Mathf.Abs(w - border.z), w))); // Left
            newBorder.z = Mathf.RoundToInt(Mathf.Clamp(border.z, 0, Mathf.Min(Mathf.Abs(w - newBorder.x), w))); // Right

            newBorder.y = Mathf.RoundToInt(Mathf.Clamp(border.y, 0, Mathf.Min(Mathf.Abs(h - border.w), h))); // Bottom
            newBorder.w = Mathf.RoundToInt(Mathf.Clamp(border.w, 0, Mathf.Min(Mathf.Abs(h - newBorder.y), h))); // Top

            return newBorder;
        }
    }
}
