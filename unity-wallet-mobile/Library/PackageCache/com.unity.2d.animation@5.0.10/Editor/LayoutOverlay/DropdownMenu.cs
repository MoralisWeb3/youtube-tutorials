using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Layout
{
	internal class DropdownMenu : VisualElement
	{
		public class DropdownMenuFactory : UxmlFactory<DropdownMenu, DropdownMenuUxmlTraits> {}
		public class DropdownMenuUxmlTraits : UxmlTraits {}

		/*
		private ButtonGroup m_ButtonGroup;

		public DropdownMenu()
		{
			RegisterCallback<FocusOutEvent>(OnFocusOut, Capture.NoCapture);
			RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
		}

		public void InitialiseWithButtonGroup(ButtonGroup buttonGroup)
		{
			if (m_ButtonGroup == buttonGroup)
				return;

			m_ButtonGroup = buttonGroup;
			var buttonGroupLocalPosition = parent.WorldToLocal(new Vector2(buttonGroup.worldBound.x, buttonGroup.worldBound.y));
			style.positionType = PositionType.Absolute;
			style.positionLeft = buttonGroupLocalPosition.x;
			style.positionTop = buttonGroupLocalPosition.y;
			style.flexDirection = buttonGroup.isHorizontal ? FlexDirection.Row : FlexDirection.Column;
			foreach (var element in buttonGroup.elements)
				Add(element);
		}

		private void OnMouseLeaveEvent(MouseLeaveEvent evt)
		{
			Close();
		}

		private void OnFocusOut(FocusOutEvent evt)
		{
			Close();
		}

		private void Close()
		{
			foreach (var element in contentContainer.Children())
				m_ButtonGroup.elements.Add(element);
			this.contentContainer.Clear();

			style.width = 0;
			style.height = 0;
			m_ButtonGroup = null;
		}
		*/
	}
}
