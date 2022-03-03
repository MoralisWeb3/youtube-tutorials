using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Lean.Transition;
using Lean.Common;

namespace Lean.Gui
{
	/// <summary>This component will place the current RectTransform above the currently selected object.</summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanSelectionHighlight")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Selection Highlight")]
	public class LeanSelectionHighlight : MonoBehaviour
	{
		/// <summary>The camera rendering the target transform/position.
		/// None = MainCamera.</summary>
		public Camera WorldCamera { set { worldCamera = value; } get { return worldCamera; } } [SerializeField] private Camera worldCamera;

		/// <summary>This allows you to perform a transition when the highlight begins.
		/// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		/// For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		/// NOTE: Any transitions you perform here must be reverted in the <b>Hide Transitions</b> setting using a matching transition component.</summary>
		public LeanPlayer ShowTransitions { get { if (showTransitions == null) showTransitions = new LeanPlayer(); return showTransitions; } } [SerializeField] private LeanPlayer showTransitions;

		/// <summary>This allows you to perform a transition when the highlight ends.
		/// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		/// For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		/// NOTE: Any transitions you perform here must be reverted in the <b>Show Transitions</b> setting using a matching transition component.</summary>
		public LeanPlayer HideTransitions { get { if (hideTransitions == null) hideTransitions = new LeanPlayer(); return hideTransitions; } } [SerializeField] private LeanPlayer hideTransitions;

		/// <summary>This allows you to perform an action when the highlight starts.</summary>
		public UnityEvent OnShow { get { if (onShow == null) onShow = new UnityEvent(); return onShow; } } [SerializeField] private UnityEvent onShow;

		/// <summary>This allows you to perform an action when the highlight ends.</summary>
		public UnityEvent OnHide { get { if (onHide == null) onHide = new UnityEvent(); return onHide; } } [SerializeField] private UnityEvent onHide;

		[SerializeField]
		private bool showing;

		[System.NonSerialized]
		protected RectTransform rectTransform;

		[System.NonSerialized]
		protected RectTransform canvasRectTransform;

		protected virtual void LateUpdate()
		{
			var eventSystem = EventSystem.current;
			var show        = false;

			if (eventSystem != null)
			{
				var selected = eventSystem.currentSelectedGameObject;

				if (selected != null)
				{
					var selectedRect = selected.GetComponent<RectTransform>();

					if (selectedRect != null)
					{
						show = UpdateRect(selectedRect);
					}
				}
			}

			if (showing != show)
			{
				showing = show;

				if (showing == true)
				{
					if (showTransitions != null)
					{
						showTransitions.Begin();
					}
				}
				else
				{
					if (hideTransitions != null)
					{
						hideTransitions.Begin();
					}
				}
			}
		}

		private bool UpdateRect(RectTransform target)
		{
			var camera = worldCamera;

			if (camera == null)
			{
				camera = Camera.main;
			}

			if (camera != null)
			{
				if (rectTransform == null)
				{
					rectTransform = GetComponent<RectTransform>();
				}

				if (canvasRectTransform == null)
				{
					var canvas = GetComponentInParent<Canvas>();

					if (canvas == null)
					{
						throw new System.Exception("Couldn't find attached canvas??");
					}

					canvasRectTransform = canvas.GetComponent<RectTransform>();
				}

				// Calculate viewport/anchor points
				var min     = target.rect.min;
				var max     = target.rect.max;
				var targetA = target.TransformPoint(min.x, min.y, 0.0f);
				var targetB = target.TransformPoint(max.x, min.y, 0.0f);
				var targetC = target.TransformPoint(min.x, max.y, 0.0f);
				var targetD = target.TransformPoint(max.x, max.y, 0.0f);

				var worldSpace     = target.GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace;
				var viewportPointA = WorldToViewportPoint(camera, targetA, worldSpace);
				var viewportPointB = WorldToViewportPoint(camera, targetB, worldSpace);
				var viewportPointC = WorldToViewportPoint(camera, targetC, worldSpace);
				var viewportPointD = WorldToViewportPoint(camera, targetD, worldSpace);

				// If outside frustum, hide line out of view
				if (LeanGui.InvaidViewportPoint(camera, viewportPointA) == true || LeanGui.InvaidViewportPoint(camera, viewportPointB) == true ||
					LeanGui.InvaidViewportPoint(camera, viewportPointC) == true || LeanGui.InvaidViewportPoint(camera, viewportPointD) == true)
				{
					viewportPointA = viewportPointB = viewportPointC = viewportPointD = new Vector3(10.0f, 10.0f);
				}

				var minX = Mathf.Min(Mathf.Min(viewportPointA.x, viewportPointB.x), Mathf.Min(viewportPointC.x, viewportPointD.x));
				var minY = Mathf.Min(Mathf.Min(viewportPointA.y, viewportPointB.y), Mathf.Min(viewportPointC.y, viewportPointD.y));
				var maxX = Mathf.Max(Mathf.Max(viewportPointA.x, viewportPointB.x), Mathf.Max(viewportPointC.x, viewportPointD.x));
				var maxY = Mathf.Max(Mathf.Max(viewportPointA.y, viewportPointB.y), Mathf.Max(viewportPointC.y, viewportPointD.y));

				// Convert viewport points to canvas points
				var canvasRect = canvasRectTransform.rect;
				var canvasXA   = canvasRect.xMin + canvasRect.width  * minX;
				var canvasYA   = canvasRect.yMin + canvasRect.height * minY;
				var canvasXB   = canvasRect.xMin + canvasRect.width  * maxX;
				var canvasYB   = canvasRect.yMin + canvasRect.height * maxY;

				// Find center, reset anchor, and convert canvas point to world point
				var canvasX = (canvasXA + canvasXB) * 0.5f;
				var canvasY = (canvasYA + canvasYB) * 0.5f;
				
				rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
				rectTransform.sizeDelta = new Vector2(canvasXB - canvasXA, canvasYB - canvasYA);
				rectTransform.position  = canvasRectTransform.TransformPoint(canvasX, canvasY, 0.0f);

				// Get vector between points

				return true;
			}

			return false;
		}

		private static Vector3 WorldToViewportPoint(Camera camera, Vector3 point, bool worldSpace)
		{
			if (worldSpace == false)
			{
				point = RectTransformUtility.WorldToScreenPoint(null, point);
				point.z = 0.5f;

				return camera.ScreenToViewportPoint(point);
			}

			return camera.WorldToViewportPoint(point);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
	using TARGET = LeanSelectionHighlight;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectionHighlight_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("worldCamera", "The camera rendering the target transform/position.\nNone = MainCamera.");

			Separator();

			Draw("showTransitions", "This allows you to perform a transition when the highlight begins. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.\n\nNOTE: Any transitions you perform here must be reverted in the Hide Transitions setting using a matching transition component.");
			Draw("hideTransitions", "This allows you to perform a transition when the highlight ends. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.\n\nNOTE: Any transitions you perform here must be reverted in the Show Transitions setting using a matching transition component.");
			
			Separator();

			Draw("onShow", "This allows you to perform an action when the highlight starts.");
			Draw("onHide", "This allows you to perform an action when the highlight ends.");
		}
	}
}
#endif