using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Lean.Common;

namespace Lean.Gui
{
	/// <summary>This class contains useful data specific to Lean GUI.</summary>
	public static class LeanGui
	{
		public delegate void DraggingDelegate(ref bool dragging);

		public const string ComponentMenuPrefix = "Lean/GUI/Lean ";

		public const string HelpUrlPrefix = "http://carloswilkes.github.io/Documentation/LeanGUI#";

		public static DraggingDelegate OnDraggingCheck;

		// Used by RaycastGui
		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

		// Used by RaycastGui
		private static PointerEventData tempPointerEventData;

		// Used by RaycastGui
		private static EventSystem tempEventSystem;

		public static bool IsDragging
		{
			get
			{
				var dragging = false;

				if (OnDraggingCheck != null)
				{
					OnDraggingCheck(ref dragging);
				}

				return dragging;
			}
		}

		/// <summary>This will return all the RaycastResults under the specified screen point using the specified layerMask.
		/// NOTE: The first result (0) will be the top UI element that was first hit.</summary>
		public static List<RaycastResult> RaycastGui(Vector2 screenPosition, LayerMask layerMask)
		{
			tempRaycastResults.Clear();

			var currentEventSystem = EventSystem.current;

			if (currentEventSystem != null)
			{
				// Create point event data for this event system?
				if (currentEventSystem != tempEventSystem)
				{
					tempEventSystem = currentEventSystem;

					if (tempPointerEventData == null)
					{
						tempPointerEventData = new PointerEventData(tempEventSystem);
					}
					else
					{
						tempPointerEventData.Reset();
					}
				}

				// Raycast event system at the specified point
				tempPointerEventData.position = screenPosition;

				currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

				// Loop through all results and remove any that don't match the layer mask
				if (tempRaycastResults.Count > 0)
				{
					for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
					{
						var raycastResult = tempRaycastResults[i];
						var raycastLayer  = 1 << raycastResult.gameObject.layer;

						if ((raycastLayer & layerMask) == 0)
						{
							tempRaycastResults.RemoveAt(i);
						}
					}
				}
			}
			else
			{
				Debug.LogError("Failed to RaycastGui because your scene doesn't have an event system! To add one, go to: GameObject/UI/EventSystem");
			}

			return tempRaycastResults;
		}

		public static bool InvaidViewportPoint(Camera camera, Vector3 point)
		{
			if (point.x < -10.0f) return true;
			if (point.x >  10.0f) return true;
			if (point.y < -10.0f) return true;
			if (point.y >  10.0f) return true;
			if (point.z < camera.nearClipPlane) return true;

			return false;
		}
	}
}