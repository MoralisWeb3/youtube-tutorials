using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Lean.Common;

namespace Lean.Gui
{
	/// <summary>This component allows you to force selection of a selectable, or select a previously selected object.</summary>
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanSelectionManager")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Selection Manager")]
	public class LeanSelectionManager : MonoBehaviour
	{
		/// <summary>If the scene contains at least one selectable, and there is currently no selected object, should it be selected?
		/// NOTE: If there are multiple selectables in the scene, then one will be chosen based on the <b>LeanSelectionPriority</b> component setting.</summary>
		public bool ForceSelection { set { forceSelection = value; } get { return forceSelection; } } [SerializeField] private bool forceSelection;

		/// <summary>This stores a list of all selectables in the order they were selected. This allows selection to revert to a previous one if the current selectable becomes unselectable.</summary>
		public List<Selectable> SelectionHistory { get { if (selectionHistory == null) selectionHistory = new List<Selectable>(); return selectionHistory; } } [SerializeField] private List<Selectable> selectionHistory;

		private static List<Selectable> tempSelectables = new List<Selectable>();

#if UNITY_2019_1_OR_NEWER
		private static Selectable[] tempSelectablesArray = new Selectable[1024];
#endif

		private bool CanSelect(Selectable selectable)
		{
			return selectable != null && selectable.IsInteractable() == true && selectable.isActiveAndEnabled == true && selectable.navigation.mode != Navigation.Mode.None;
		}

		[ContextMenu("Select First Selectable")]
		public void SelectFirstSelectable()
		{
			tempSelectables.Clear();

#if UNITY_2019_1_OR_NEWER
	#if UNITY_2019_1_0 || UNITY_2019_1_1 || UNITY_2019_1_2 || UNITY_2019_1_3 || UNITY_2019_1_4
				var count = Selectable.AllSelectablesNoAlloc(ref tempSelectablesArray);
	#else
				var count = Selectable.AllSelectablesNoAlloc(tempSelectablesArray);
	#endif

				for (var i = 0; i < count; i++)
				{
					tempSelectables.Add(tempSelectablesArray[i]);
				}
#else
				tempSelectables.AddRange(Selectable.allSelectables);
#endif

			// Select from history?
			for (var i = SelectionHistory.Count - 1; i >= 0; i--) // NOTE: Property
			{
				var oldSelection = selectionHistory[i];

				if (oldSelection != null)
				{
					if (CanSelect(oldSelection) == true && tempSelectables.Contains(oldSelection) == true)
					{
						selectionHistory.RemoveRange(i, selectionHistory.Count - i);

						oldSelection.Select();

						return;
					}
				}
				else
				{
					selectionHistory.RemoveAt(i);
				}
			}

			var bestSelectable = default(Selectable);
			var bestPriority   = -1.0f;

			// Select from selectables?
			for (var i = 0; i < tempSelectables.Count; i++)
			{
				var selectable = tempSelectables[i];

				if (CanSelect(selectable) == true)
				{
					var selectionPriority = selectable.GetComponent<LeanSelectionPriority>();
					var priority          = 0.0f;

					if (selectionPriority != null)
					{
						priority = selectionPriority.Priority;
					}

					if (priority > bestPriority)
					{
						bestSelectable = selectable;
						bestPriority   = priority;
					}
				}
			}

			if (bestSelectable != null)
			{
				bestSelectable.Select();
			}
		}

		protected virtual void LateUpdate()
		{
			var eventSystem = EventSystem.current;

			if (eventSystem != null)
			{
				var selected = eventSystem.currentSelectedGameObject;

				// Deselect if the selection is not selectable?
				if (selected != null)
				{
					var selectable = selected.GetComponent<Selectable>();

					if (selectable != null)
					{
						for (var i = SelectionHistory.Count - 1; i >= 0; i--) // NOTE: Property
						{
							var oldSelection = selectionHistory[i];

							if (oldSelection == null || oldSelection == selectable)
							{
								selectionHistory.RemoveAt(i);
							}
						}

						selectionHistory.Add(selectable);
					}

					if (CanSelect(selectable) == false)
					{
						selected = null;

						eventSystem.SetSelectedGameObject(null);
					}
				}

				if (LeanGui.IsDragging == true)
				{
					selected = null;

					eventSystem.SetSelectedGameObject(null);
				}
				else
				{
					// Auto select?
					if (selected == null && ForceSelection == true)
					{
						SelectFirstSelectable();

						selected = eventSystem.currentSelectedGameObject;
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
	using TARGET = LeanSelectionManager;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectionManager_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("forceSelection", "If the scene contains at least one selectable, and there is currently no selected object, should it be selected?\n\nNOTE: If there are multiple selectables in the scene, then one will be chosen based on the LeanSelectionPriority component setting.");

			Separator();

			BeginDisabled(true);
				UnityEditor.EditorGUILayout.ObjectField("Current Selectable", EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null, typeof(GameObject), false);

				Separator();

				Draw("selectionHistory", "Enable this if you want ExitTransitions + OnExit to be invoked once for each mouse/finger that exits this element.");
			EndDisabled();
		}
	}
}
#endif