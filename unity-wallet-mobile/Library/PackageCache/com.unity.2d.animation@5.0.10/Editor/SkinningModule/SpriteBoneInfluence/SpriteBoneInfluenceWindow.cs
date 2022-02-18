using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    using PopupWindow = UnityEngine.UIElements.PopupWindow;

    internal interface ISpriteBoneInfluenceWindow
    {
        string headerText { get; set; }
        void SetHiddenFromLayout(bool hide);
        void OnSpriteSelectionChanged();
        void OnSkeletonChanged();
        void OnMeshChanged();
        bool visible { get; }
        void OnBoneSelectionChanged();
        event Action onAddBone;
        event Action onRemoveBone;
        event Action<IEnumerable<BoneCache>> onReordered;
        event Action<IEnumerable<BoneCache>> onSelectionChanged;
        void SetController(SpriteBoneInflueceToolController controller);
    }

    internal class SpriteBoneInfluenceWindow : VisualElement, ISpriteBoneInfluenceWindow
    {
        public class CustomUxmlFactory : UxmlFactory<SpriteBoneInfluenceWindow, UxmlTraits> {}

        private SpriteBoneInfluenceListWidget m_InfluencesList;
        private PopupWindow m_HeaderLabel;
        SpriteBoneInflueceToolController m_Controller;

        public event Action onAddBone = () => {};
        public event Action onRemoveBone = () => {};
        public event Action<IEnumerable<BoneCache>> onReordered = _ => {};
        public event Action<IEnumerable<BoneCache>> onSelectionChanged = _ => {};

        public string headerText
        {
            get { return m_HeaderLabel.text; }
            set { m_HeaderLabel.text = value; }
        }

        static internal SpriteBoneInfluenceWindow CreateFromUXML()
        {
            var visualTree = ResourceLoader.Load<VisualTreeAsset>("SkinningModule/SpriteBoneInfluenceWindow.uxml");
            var ve = visualTree.CloneTree().Q("SpriteBoneInfluenceWindow") as SpriteBoneInfluenceWindow;
            ve.BindElements();
            return ve;
        }

        internal void BindElements()
        {
            m_InfluencesList = this.Q<SpriteBoneInfluenceListWidget>();
            m_InfluencesList.onAddBone = () => onAddBone();
            m_InfluencesList.onRemoveBone = () => onRemoveBone();
            m_InfluencesList.onReordered = s => onReordered(s);
            m_InfluencesList.onSelectionChanged = (s) => onSelectionChanged(s);
            m_InfluencesList.GetController = InternalGetController;
            m_HeaderLabel = this.Q<PopupWindow>();
            this.styleSheets.Add(ResourceLoader.Load<StyleSheet>("SkinningModule/SpriteBoneInfluenceWindowStyle.uss"));
        }

        SpriteBoneInflueceToolController InternalGetController()
        {
            return m_Controller;
        }

        public void OnSpriteSelectionChanged()
        {
            Update();
        }

        public void OnMeshChanged()
        {
            Update();
        }

        public void OnSkeletonChanged()
        {
            m_InfluencesList.Update();
        }

        public void OnBoneSelectionChanged()
        {
            m_InfluencesList.OnBoneSelectionChanged();
        }

        private void Update()
        {
            m_InfluencesList.Update();
            m_InfluencesList.OnBoneSelectionChanged();
        }

        void ISpriteBoneInfluenceWindow.SetController(SpriteBoneInflueceToolController controller)
        {
            m_Controller = controller;
        }

        void ISpriteBoneInfluenceWindow.SetHiddenFromLayout(bool hide)
        {
            this.SetHiddenFromLayout(hide);
        }
    }
}
