using UnityEditor.U2D.Animation;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Layout
{
	internal class LayoutOverlay : VisualElement
	{
		public class LayoutOverlayFactory : UxmlFactory<LayoutOverlay, LayoutOverlayUxmlTraits> {}
		public class LayoutOverlayUxmlTraits : UxmlTraits {}

		private ScrollableToolbar m_HorizontalToolbar;
		private ScrollableToolbar m_VerticalToolbar;
		private VisualElement m_HorizontalHolder;
		private VisualElement m_LeftOverlay;
		private VisualElement m_RightOverlay;
		private DropdownMenu m_DropdownOverlay;

		public ScrollableToolbar horizontalToolbar
		{
			get
			{
				if (m_HorizontalToolbar == null)
					m_HorizontalToolbar = this.Q<ScrollableToolbar>("HorizontalToolbar");
				return m_HorizontalToolbar;
			}
		}

		public ScrollableToolbar verticalToolbar
		{
			get
			{
				if (m_VerticalToolbar == null)
					m_VerticalToolbar = this.Q<ScrollableToolbar>("VerticalToolbar");
				return m_VerticalToolbar;
			}
		}

		public VisualElement horizontalHolder
		{
			get
			{
				if (m_HorizontalHolder == null)
					m_HorizontalHolder = this.Q<VisualElement>("HorizontalHolder");
				return m_HorizontalHolder;
			}
		}

		public VisualElement leftOverlay
		{
			get
			{
				if (m_LeftOverlay == null)
					m_LeftOverlay = this.Q<VisualElement>("LeftOverlay");
				return m_LeftOverlay;
			}
		}

		public VisualElement rightOverlay
		{
			get
			{
				if (m_RightOverlay == null)
					m_RightOverlay = this.Q<VisualElement>("RightOverlay");
				return m_RightOverlay;
			}
		}

		public void VisibilityWindowOn(bool on)
		{
			if(on)
				rightOverlay.AddToClassList("VisibilityWindowOn");
			else
				rightOverlay.RemoveFromClassList("VisibilityWindowOn");
		}
		public DropdownMenu dropdownOverlay
		{
			get
			{
				if (m_DropdownOverlay == null)
					m_DropdownOverlay = this.Q<DropdownMenu>("DropdownOverlay");
				return m_DropdownOverlay;
			}
		}

		public bool hasScrollbar
		{
			get { return this.ClassListContains("HasScrollbar"); }
			set
			{
				if (value)
					this.AddToClassList("HasScrollbar");
				else
					this.RemoveFromClassList("HasScrollbar");
			}
		}

		public LayoutOverlay()
		{
			this.StretchToParentSize();
			styleSheets.Add(ResourceLoader.Load<StyleSheet>("LayoutOverlay/LayoutOverlayStyle.uss"));
		}
	}
}

