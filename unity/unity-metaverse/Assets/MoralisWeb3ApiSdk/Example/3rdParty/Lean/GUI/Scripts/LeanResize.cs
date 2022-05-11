using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Lean.Transition;
using Lean.Common;

namespace Lean.Gui
{
	/// <summary>This component allows you to resize the specified RectTransform when you drag on this UI element.</summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanResize")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Resize")]
	public class LeanResize : LeanSelectable, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		/// <summary>If you want this GameObject to act as a resize handle, and for a different object to actually be resized then specify the target object here.</summary>
		public RectTransform Target { set { target = value; } get { return target; } } [SerializeField] private RectTransform target;

		/// <summary>Should you be able to drag horizontally?</summary>
		public bool Horizontal { set { horizontal = value; } get { return horizontal; } } [SerializeField] private bool horizontal = true;

		/// <summary>Horizontal resize strength.
		/// 0 = none
		/// 1 = normal
		/// -1 = inverted
		/// 2 = centered</summary>
		public float HorizontalScale { set { horizontalScale = value; } get { return horizontalScale; } } [SerializeField] private float horizontalScale = 1.0f;

		/// <summary>Should the horizontal size value be clamped?</summary>
		public bool HorizontalClamp { set { horizontalClamp = value; } get { return horizontalClamp; } } [SerializeField] private bool horizontalClamp;

		/// <summary>The minimum size value.</summary>
		public float HorizontalMin { set { horizontalMin = value; } get { return horizontalMin; } } [SerializeField] private float horizontalMin = 50.0f;

		/// <summary>The maximum size value.</summary>
		public float HorizontalMax { set { horizontalMax = value; } get { return horizontalMax; } } [SerializeField] private float horizontalMax = 500.0f;

		/// <summary>Should you be able to drag vertically?</summary>
		public bool Vertical { set { vertical = value; } get { return vertical; } } [SerializeField] private bool vertical = true;

		/// <summary>Vertical resize strength.
		/// 0 = none
		/// 1 = normal
		/// -1 = inverted
		/// 2 = centered</summary>
		public float VerticalScale { set { verticalScale = value; } get { return verticalScale; } } [SerializeField] private float verticalScale = 1.0f;

		/// <summary>Should the vertical size value be clamped?</summary>
		public bool VerticalClamp { set { verticalClamp = value; } get { return verticalClamp; } } [SerializeField] private bool verticalClamp;

		/// <summary>The minimum size value.</summary>
		public float VerticalMin { set { verticalMin = value; } get { return verticalMin; } } [SerializeField] private float verticalMin = 50.0f;

		/// <summary>The maximum size value.</summary>
		public float VerticalMax { set { verticalMax = value; } get { return verticalMax; } } [SerializeField] private float verticalMax = 500.0f;

		/// <summary>This allows you to perform a transition when this element begins being resized.
		/// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		/// For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		/// NOTE: Any transitions you perform here must be reverted in the <b>Normal Transitions</b> setting using a matching transition component.</summary>
		public LeanPlayer BeginTransitions { get { if (beginTransitions == null) beginTransitions = new LeanPlayer(); return beginTransitions; } } [SerializeField] private LeanPlayer beginTransitions;

		/// <summary>This allows you to perform a transition when this element ends being resized.
		/// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		/// For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.</summary>
		public LeanPlayer EndTransitions { get { if (endTransitions == null) endTransitions = new LeanPlayer(); return endTransitions; } } [SerializeField] private LeanPlayer endTransitions;

		/// <summary>This allows you to perform an actions when this element begins being resized.</summary>
		public UnityEvent OnBegin { get { if (onBegin == null) { onBegin = new UnityEvent(); } return onBegin; } } [SerializeField] private UnityEvent onBegin;

		/// <summary>This allows you to perform an actions when this element ends being resized.</summary>
		public UnityEvent OnEnd { get { if (onEnd == null) { onEnd = new UnityEvent(); } return onEnd; } } [SerializeField] private UnityEvent onEnd;

		[System.NonSerialized]
		private bool dragging;

		[System.NonSerialized]
		private Vector2 startSize;

		[System.NonSerialized]
		private Vector2 startOffset;

		[System.NonSerialized]
		private RectTransform cachedRectTransform;

		[System.NonSerialized]
		private bool cachedRectTransformSet;

		public RectTransform TargetTransform
		{
			get
			{
				if (target != null)
				{
					return target;
				}

				if (cachedRectTransformSet == false)
				{
					cachedRectTransform    = GetComponent<RectTransform>();
					cachedRectTransformSet = true;
				}

				return cachedRectTransform;
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (MayDrag(eventData) == true)
			{
				var target = TargetTransform;
				var vector = default(Vector2);

				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out vector) == true)
				{
					dragging    = true;
					startSize   = target.sizeDelta;
					startOffset = vector - target.anchoredPosition;

					if (beginTransitions != null)
					{
						beginTransitions.Begin();
					}

					if (onBegin != null)
					{
						onBegin.Invoke();
					}
				}
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (dragging == true)
			{
				if (MayDrag(eventData) == true)
				{
					var vector = default(Vector2);
					var target = TargetTransform;

					if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out vector) == true)
					{
						var offsetDelta = (vector - target.anchoredPosition) - startOffset;
						var   sizeDelta = target.sizeDelta;

						if (horizontal == true)
						{
							sizeDelta.x = startSize.x + offsetDelta.x * horizontalScale;

							if (horizontalClamp == true)
							{
								sizeDelta.x = Mathf.Clamp(sizeDelta.x, horizontalMin, horizontalMax);
							}
						}

						if (vertical == true)
						{
							sizeDelta.y = startSize.y + offsetDelta.y * verticalScale;

							if (verticalClamp == true)
							{
								sizeDelta.y = Mathf.Clamp(sizeDelta.y, verticalMin, verticalMax);
							}
						}

						target.sizeDelta = sizeDelta;
					}
				}
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			dragging = false;

			if (endTransitions != null)
			{
				endTransitions.Begin();
			}

			if (onEnd != null)
			{
				onEnd.Invoke();
			}
		}
		
		protected override void Start()
		{
			base.Start();

			transition   = Transition.None;
			interactable = true;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			LeanGui.OnDraggingCheck += DraggingCheck;
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			LeanGui.OnDraggingCheck -= DraggingCheck;
		}

		private void DraggingCheck(ref bool check)
		{
			if (dragging == true)
			{
				check = true;
			}
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return IsActive() && IsInteractable();// && eventData.button == PointerEventData.InputButton.Left;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
	using TARGET = LeanResize;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanResize_Editor : LeanSelectable_Editor
	{
		protected override void DrawSelectableSettings()
		{
			base.DrawSelectableSettings();

			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			base.DrawSelectableSettings();

			Draw("target", "If you want this GameObject to act as a resize handle, and for a different object to actually be resized then specify the target object here.");

			Separator();

			Draw("horizontal", "Should you be able to drag horizontally?");
			if (Any(tgts, t => t.Horizontal == true))
			{
				BeginIndent();
					Draw("horizontalScale", "Horizontal resize strength.\n\n0 = none\n\n1 = normal\n\n-1 = inverted\n\n2 = centered", "Scale");
					Draw("horizontalClamp", "Should the horizontal position value be clamped?", "Clamp");
					if (Any(tgts, t => t.HorizontalClamp == true))
					{
						BeginIndent();
							Draw("horizontalMin", "The minimum position value.", "Min");
							Draw("horizontalMax", "The maximum position value.", "Max");
						EndIndent();
					}
				EndIndent();
			}

			Separator();

			Draw("vertical", "Should you be able to drag vertically?");
			if (Any(tgts, t => t.Vertical == true))
			{
				BeginIndent();
					Draw("verticalScale", "Vertical resize strength.\n\n0 = none\n\n1 = normal\n\n-1 = inverted\n\n2 = centered", "Scale");
					Draw("verticalClamp", "Should the vertical position value be clamped?", "Clamp");
					if (Any(tgts, t => t.VerticalClamp == true))
					{
						BeginIndent();
							Draw("verticalMin", "The minimum position value.", "Min");
							Draw("verticalMax", "The maximum position value.", "Max");
						EndIndent();
					}
				EndIndent();
			}
		}

		protected override void DrawSelectableTransitions(bool showUnusedEvents)
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			if (showUnusedEvents == true || Any(tgts, t => t.BeginTransitions.IsUsed == true))
			{
				Draw("beginTransitions", "This allows you to perform a transition when this element begins being dragged. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.\n\nNOTE: Any transitions you perform here must be reverted in the Normal Transitions setting using a matching transition component.");
			}

			if (showUnusedEvents == true || Any(tgts, t => t.EndTransitions.IsUsed == true))
			{
				Draw("endTransitions", "This allows you to perform a transition when this element ends being dragged. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.");
			}

			base.DrawSelectableTransitions(showUnusedEvents);
		}

		protected override void DrawSelectableEvents(bool showUnusedEvents)
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			if (showUnusedEvents == true || Any(tgts, t => t.OnBegin.GetPersistentEventCount() > 0))
			{
				Draw("onBegin");
			}

			if (showUnusedEvents == true || Any(tgts, t => t.OnEnd.GetPersistentEventCount() > 0))
			{
				Draw("onEnd");
			}

			base.DrawSelectableEvents(showUnusedEvents);
		}
	}
}
#endif