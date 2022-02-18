using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Layout
{
    internal class ScrollableToolbar : VisualElement
    {
        public class ScrollableToolbarFactory : UxmlFactory<ScrollableToolbar, ScrollableToolbarUxmlTraits> {}
        public class ScrollableToolbarUxmlTraits : UxmlTraits
        {
            UxmlBoolAttributeDescription m_IsHorizontal;

            public ScrollableToolbarUxmlTraits()
            {
                m_IsHorizontal = new UxmlBoolAttributeDescription { name = "isHorizontal" };
            }

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ScrollableToolbar toolbar = ((ScrollableToolbar)ve);
                toolbar.isHorizontal = m_IsHorizontal.GetValueFromBag(bag, cc);
            }
        }

        private ScrollView m_ScrollView;
        private bool m_IsHorizontal;

        public bool isHorizontal
        {
            get { return m_IsHorizontal; }
            set
            {
                if (m_IsHorizontal != value)
                {
                    m_IsHorizontal = value;
                    SetupScrolling();
                }
            }
        }

        public ScrollableToolbar() : this(false)
        {
        }

        public ScrollableToolbar(bool isHorizontal)
        {
            m_ScrollView = new ScrollView() {name = "ScrollView"};;
            m_ScrollView.StretchToParentSize();
            hierarchy.Add(m_ScrollView);

            m_IsHorizontal = isHorizontal;
            SetupScrolling();

            styleSheets.Add(ResourceLoader.Load<StyleSheet>("LayoutOverlay/ScrollableToolbar.uss"));

            // TODO: Add onto current ScrollView internal WheelEvent
            m_ScrollView.RegisterCallback<WheelEvent>(OnScrollWheel);

            pickingMode = PickingMode.Ignore;
            m_ScrollView.pickingMode = PickingMode.Ignore;
            m_ScrollView.contentViewport.pickingMode = PickingMode.Ignore;
            m_ScrollView.contentContainer.pickingMode = PickingMode.Ignore;
        }
  
        public void AddToContainer(VisualElement element)
        {
            m_ScrollView.contentContainer.Add(element);
        }

        public void Collapse(bool collapse)
        {
            if (collapse)
                AddToClassList("Collapse");
            else
                RemoveFromClassList("Collapse");
        }

        private void SetupScrolling()
        {
            if (isHorizontal)
            {
                m_ScrollView.style.flexDirection = FlexDirection.Row;
                m_ScrollView.contentViewport.style.marginLeft = 10;
                m_ScrollView.contentViewport.style.marginRight = 10;
                m_ScrollView.contentViewport.style.marginTop = 0;
                m_ScrollView.contentViewport.style.marginBottom = 0;
                m_ScrollView.contentContainer.style.flexDirection = FlexDirection.Row;
                m_ScrollView.contentContainer.style.flexGrow = 1f;
            }
            else
            {
                m_ScrollView.style.flexDirection = FlexDirection.Column;
                m_ScrollView.contentViewport.style.marginLeft = 0;
                m_ScrollView.contentViewport.style.marginRight = 0;
                m_ScrollView.contentViewport.style.marginTop = 10;
                m_ScrollView.contentViewport.style.marginBottom = 10;
                m_ScrollView.contentContainer.style.flexDirection = FlexDirection.Column;
                m_ScrollView.contentContainer.style.flexGrow = 1f;
            }
        }

        void OnScrollWheel(WheelEvent evt)
        {
            /*
            // Handled by ScrollView
            if (!isHorizontal && m_ScrollView.contentContainer.layout.height - layout.height > 0)
            {
                if (evt.delta.y < 0)
                    m_ScrollView.verticalScroller.ScrollPageUp();
                else if (evt.delta.y > 0)
                    m_ScrollView.verticalScroller.ScrollPageDown();
            }
            */
            if (isHorizontal && m_ScrollView.contentContainer.layout.width - layout.width > 0)
            {
                // TODO: Does not provide delta.x for sidescrolling mouse wheel. Use delta.y for now.
                if (evt.delta.y < 0)
                    m_ScrollView.horizontalScroller.ScrollPageUp();
                else if (evt.delta.y > 0)
                    m_ScrollView.horizontalScroller.ScrollPageDown();
            }
            evt.StopPropagation();
        }
    }
}
