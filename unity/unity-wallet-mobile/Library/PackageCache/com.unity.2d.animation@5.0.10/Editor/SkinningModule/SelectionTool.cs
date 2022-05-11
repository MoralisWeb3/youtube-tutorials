using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Layout;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class SelectionTool : BaseTool
    {
        bool m_ForceSelectedToSpriteEditor = false;
        int m_LastMouseButtonDown = -1;

        public event Func<bool> CanSelect = () => true;
        List<SpriteCache> m_Sprites;
        public ISpriteEditor spriteEditor { get; set; }
        SpriteCache selectedSprite
        {
            get => skinningCache.selectedSprite;
            set
            {
                if (selectedSprite != value)
                {
                    skinningCache.vertexSelection.Clear();

                    if (skinningCache.mode == SkinningMode.SpriteSheet)
                    {
                        skinningCache.skeletonSelection.Clear();
                        skinningCache.events.boneSelectionChanged.Invoke();
                    }

                    skinningCache.selectedSprite = value;
                    SetToSpriteEditor();
                    skinningCache.events.selectedSpriteChanged.Invoke(value);
                }
            }
        }

        string selectedSpriteAssetID
        {
            get
            {
                var sprite = Selection.activeObject as Sprite;

                if (sprite != null)
                    return sprite.GetSpriteID().ToString();

                return "";
            }
        }

        protected override void OnAfterDeserialize()
        {
            m_ForceSelectedToSpriteEditor = true;
        }

        public override void Initialize(LayoutOverlay layoutOverlay)
        {
            m_Sprites = new List<SpriteCache>(skinningCache.GetSprites());
            SetFromSpriteEditor();
        }

        protected override void OnActivate()
        {
            SetToSpriteEditor();
            skinningCache.events.selectedSpriteChanged.AddListener(OnSpriteSelectionChange);
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
        }

        protected override void OnDeactivate()
        {
            skinningCache.events.selectedSpriteChanged.RemoveListener(OnSpriteSelectionChange);
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSpriteSelectionChange(SpriteCache sprite)
        {
            skinningCache.events.selectedSpriteChanged.RemoveListener(OnSpriteSelectionChange);
            selectedSprite = sprite;
            skinningCache.events.selectedSpriteChanged.AddListener(OnSpriteSelectionChange);
        }

        void OnSelectionChanged()
        {
            if (m_ForceSelectedToSpriteEditor)
            {
                SetToSpriteEditor();
                m_ForceSelectedToSpriteEditor = false;
            }
            else
            {
                using (skinningCache.UndoScope(TextContent.selectionChange))
                {
                    SetFromSpriteEditor();
                }
            }
        }

        void SetFromSpriteEditor()
        {
            if (selectedSprite == null)
                selectedSprite = skinningCache.GetSprite(selectedSpriteAssetID);
            spriteEditor.RequestRepaint();
        }

        void SetToSpriteEditor()
        {
            var id = "";

            if (selectedSprite != null)
                id = selectedSprite.id;

            spriteEditor.selectedSpriteRect = new SpriteRect() { spriteID = new GUID(id) };
        }

        protected override void OnGUI()
        {
            HandleSpriteSelection();
        }
        
        void HandleSpriteSelection()
        {
            Debug.Assert(Event.current != null);

            if (Event.current.type == EventType.MouseDown)
            {
                if (IsSelectionRequested())
                {
                    var mousePosition = Handles.inverseMatrix.MultiplyPoint(Event.current.mousePosition);
                    var newSelected = TrySelect(mousePosition);
                    if (selectedSprite != newSelected)
                    {
                        using (skinningCache.UndoScope(TextContent.selectionChange))
                        {
                            selectedSprite = newSelected;
                        }
                        
                        Event.current.Use();
                    }
                }
                else
                    m_LastMouseButtonDown = Event.current.button;
            }
        }

        SpriteCache TrySelect(Vector2 mousePosition)
        {
            m_Sprites.Remove(selectedSprite);

            if (selectedSprite != null)
                m_Sprites.Add(selectedSprite);

            var currentSelectedIndex = m_Sprites.FindIndex(x => x == selectedSprite) + 1;
            var notVisiblePart = skinningCache.hasCharacter && skinningCache.mode == SkinningMode.Character
                ? skinningCache.character.parts.Where(x => !x.isVisible).Select(x => x.sprite) : new SpriteCache[0];
            for (int index = 0; index < m_Sprites.Count; ++index)
            {
                var sprite = m_Sprites[(currentSelectedIndex + index) % m_Sprites.Count];
                var meshPreview = sprite.GetMeshPreview();
                if (notVisiblePart.Contains(sprite))
                    continue;

                Debug.Assert(meshPreview != null);

                var spritePosition = sprite.GetLocalToWorldMatrixFromMode().MultiplyPoint3x4(Vector3.zero);
                var ray = new Ray((Vector3)mousePosition - spritePosition + Vector3.back, Vector3.forward);
                var bounds = meshPreview.mesh.bounds;

                if (sprite.GetMesh().indices.Count >= 3)
                {
                    if (bounds.IntersectRay(ray))
                    {
                        var mesh = sprite.GetMesh();

                        Debug.Assert(mesh != null);

                        var indices = mesh.indices;
                        for (var i = 0; i < indices.Count; i += 3)
                        {
                            var p1 = meshPreview.vertices[indices[i]];
                            var p2 = meshPreview.vertices[indices[i + 1]];
                            var p3 = meshPreview.vertices[indices[i + 2]];

                            if (MathUtility.Intersect(p1, p2, p3, ray))
                                return sprite;
                        }
                    }
                }
                else
                {
                    if (meshPreview.defaultMesh.bounds.IntersectRay(ray))
                    {
                        return sprite;
                    }
                }
            }

            return null;
        }
        
        bool IsSelectionRequested()
        {
            return Event.current.button == 0 && m_LastMouseButtonDown == 0 && GUIUtility.hotControl == 0 &&
                !Event.current.alt && Event.current.clickCount == 2 && CanSelect();
        }
    }
}
