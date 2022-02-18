using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Sprites
{
    /// <summary>Base class the Sprite Editor Window custom module inherits from.</summary>
    /// <remarks>Sprite Editor Window functionality can be extended by providing custom module. By inheriting from SpriteEditorModuleBase, user will be able to activate the module's functionality from Sprite Editor Window.
    /// </remarks>
    public abstract class SpriteEditorModuleBase
    {
        /// <summary>The ISpriteEditor instance that instantiated the module.</summary>
        /// <value>An instance of ISpriteEditor</value>
        public ISpriteEditor spriteEditor { get; internal set; }
        /// <summary>The module name to display in Sprite Editor Window.</summary>
        /// <value>String to represent the name of the module</value>
        public abstract string moduleName { get; }
        /// <summary>Indicates if the module can be activated with the current ISpriteEditor state.</summary>
        /// <returns>Return true if the module can be activated.</returns>
        public abstract bool CanBeActivated();
        /// <summary>Implement this to draw on the Sprite Editor Window.</summary>
        /// <remarks>Called after Sprite Editor Window draws texture preview of the Asset.Use this to draw gizmos for Sprite data.</remarks>
        public abstract void DoMainGUI();
        /// <summary>Implement this to create a custom toolbar.</summary>
        /// <param name = "drawArea" > Area for drawing tool bar.</param>
        public abstract void DoToolbarGUI(Rect drawArea);
        /// <summary>This is called when the user activates the module.</summary>
        /// <remarks>When user switches to the module via Sprite Editor Window, this method will be called for the module to setup.</remarks>
        public abstract void OnModuleActivate();
        /// <summary>This is called when user switches to another module.</summary>
        /// <remarks>When user switches to the another module in Sprite Editor Window, this method will be called for the module to clean up.</remarks>
        public abstract void OnModuleDeactivate();
        /// <summary>Implement this to draw widgets in Sprite Editor Window.</summary>
        /// <remarks>This method is called last to allow drawing of widgets.</remarks>
        public abstract void DoPostGUI();
        /// <summary>This is called when user clicks on the Apply or Revert button in Sprite Editor Window.</summary>
        /// <param name="apply">True when user wants to apply the data, false when user wants to revert.</param>
        /// <returns>Return true to trigger a reimport.</returns>
        public abstract bool ApplyRevert(bool apply);
    }

    /// <summary>Interface that defines the functionality available for classes that inherits SpriteEditorModuleBase.</summary>
    /// <remarks>Used by Sprite Editor Window to encapsulate functionality accessible for Sprite Editor modules when editing Sprite data.</remarks>
    public interface ISpriteEditor
    {
        /// <summary>Sets current available Sprite rects.</summary>
        List<SpriteRect> spriteRects { set; }
        /// <summary>The current selected Sprite rect data.</summary>
        SpriteRect selectedSpriteRect { get; set; }
        /// <summary>Indicates if ISpriteEditor should be interested in mouse move events.</summary>
        bool enableMouseMoveEvent { set; }
        /// <summary>Indicates that if Sprite data editing should be disabled; for example when the Editor is in Play Mode.</summary>
        bool editingDisabled { get; }
        /// <summary>Property that defines the window's current screen position and size.</summary>
        Rect windowDimension { get; }
        /// <summary>
        /// Gets data provider that is supported by the current selected object.
        /// </summary>
        /// <typeparam name="T">Data provider type</typeparam>
        /// <returns>Instance of the data provider</returns>
        T GetDataProvider<T>() where T : class;
        /// <summary>The method updates ISpriteEditor.selectedSpriteRect based on current mouse down event and ISpriteEditor.spriteRects available.</summary>
        /// <returns>Returns true when ISpriteEditor.selectedSpriteRect is changed.</returns>
        bool HandleSpriteSelection();
        /// <summary>Request to repaint the current view.</summary>
        void RequestRepaint();
        /// <summary>Indicates that there has been a change of data. In Sprite Editor Window, this enables the 'Apply' and 'Revert' button.</summary>
        void SetDataModified();
        /// <summary>The method will inform current active SpriteEditorModuleBase to apply or revert any data changes.</summary>
        void ApplyOrRevertModification(bool apply);
        /// <summary>
        /// Returns a VisualElement for attaching child VisualElement onto the main view of a ISpriteEditor.
        /// </summary>
        /// <returns>Root VisualElement for the main view.</returns>
        /// <remarks>This method returns the root VisualElement for SpriteEditorModuleBase which uses the UIElement instead of IMGUI for its UI.A VisualElement that is added to this container has the same draw order as SpriteEditorModuleBase.DoPostGUI.</remarks>
        VisualElement GetMainVisualContainer();
        /// <summary>Sets a custom texture to be used by the ISpriteEditor during setup of the editing space.</summary>
        /// <param name = "texture" > The custom preview texture.</param>
        /// <param name = "width" > The width dimension to render the preview texture.</param>
        /// <param name = "height" > The height dimension to render the preview texture.</param>
        /// <remarks>When the method is called, the editing space's dimensions are set to the width and height values, affecting operations such as Zoom and Pan in the ISpriteEditor view. The preview texture is rendered as the background of the editing space.</remarks>
        void SetPreviewTexture(Texture2D texture, int width, int height);
        /// <summary> Resets the zoom and scroll of the Sprite Editor Windows.</summary>
        void ResetZoomAndScroll();
        /// <summary>Current zoom level of the ISpriteEditor view. </summary>
        float zoomLevel { get; set; }
        /// <summary>Current scroll position of the ISpriteEditor view. Determines the viewing location of the Texture displayed</summary>
        Vector2 scrollPosition { get; set; }
        /// <summary>Whether the ISpriteEditor view is visualizing the Alpha of the Texture displayed</summary>
        bool showAlpha { get; set; }
        /// <summary>The current Mip Level of the Texture displayed in ISpriteEditor view</summary>
        float mipLevel { get; set; }
    }

    /// <summary>Use this attribute on a class that inherits from SpriteEditorModuleBase to indicate what data provider it needs.</summary>
    /// <remarks>When you use this attribute, Sprite Editor Window will only make the module available for selection if ISpriteEditorDataProvider.HasDataProvider returns true for all the data providers the module needs.
    /// <code>
    /// using UnityEditor.U2D;
    /// using UnityEngine;
    ///
    /// [RequireSpriteDataProvider(typeof(ISpriteOutlineDataProvider), typeof(ITextureDataProvider))]
    /// public class MySpriteEditorCustomModule : SpriteEditorModuleBase
    /// {
    ///     public override string moduleName
    ///     {
    ///         get { return "MySpriteEditorCustomModule"; }
    ///     }
    ///     public override bool ApplyRevert(bool apply)
    ///     {
    ///         return true;
    ///     }
    ///     public override bool CanBeActivated()
    ///     {
    ///         return true;
    ///     }
    ///     public override void DoMainGUI() { }
    ///     public override void DoToolbarGUI(Rect drawArea) { }
    ///     public override void OnModuleActivate()
    ///     {
    ///         var outlineProvider = spriteEditor.GetDataProvider&lt;ISpriteOutlineDataProvider&gt;();
    ///         var spriteRects = spriteEditor.GetDataProvider&lt;ISpriteEditorDataProvider&gt;().GetSpriteRects();
    ///         foreach (var spriteRect in spriteRects)
    ///         {
    ///             // Access outline data
    ///             Debug.Log(outlineProvider.GetOutlines(spriteRect.spriteID));
    ///         }
    ///     }
    ///     public override void OnModuleDeactivate() { }
    ///     public override void DoPostGUI() { }
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class RequireSpriteDataProviderAttribute : Attribute
    {
        Type[] m_Types;

        /// <summary>Use the attribute to indicate the custom data provider that SpriteEditorBaseModule needs.</summary>
        /// <param name="types">Data provider type.</param>
        public RequireSpriteDataProviderAttribute(params Type[] types)
        {
            m_Types = types;
        }

        internal bool ContainsAllType(ISpriteEditorDataProvider provider)
        {
            return provider == null ? false : m_Types.Where(x =>
            {
                return provider.HasDataProvider(x);
            }).Count() == m_Types.Length;
        }
    }
}
