using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Lean.Transition;
using Lean.Common;

namespace Lean.Gui
{
	/// <summary>This component allows you to perform a transition when this UI element is selected.</summary>
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanSelection")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Selection")]
	public class LeanSelection : MonoBehaviour
	{
		/// <summary>This allows you to perform a transition when this UI element is selected.
		/// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		/// For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		/// NOTE: Any transitions you perform here must be reverted in the <b>Deselect Transitions</b> setting using a matching transition component.</summary>
		public LeanPlayer SelectTransitions { get { if (selectTransitions == null) selectTransitions = new LeanPlayer(); return selectTransitions; } } [SerializeField] private LeanPlayer selectTransitions;

		/// <summary>This allows you to perform a transition when this UI element is deselected.
		/// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		/// For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		/// NOTE: Any transitions you perform here must be reverted in the <b>Select Transitions</b> setting using a matching transition component.</summary>
		public LeanPlayer DeselectTransitions { get { if (deselectTransitions == null) deselectTransitions = new LeanPlayer(); return deselectTransitions; } } [SerializeField] private LeanPlayer deselectTransitions;

		[System.NonSerialized]
		private bool lastSelected;

		protected virtual void Update()
		{
			var selected    = false;
			var eventSystem = EventSystem.current;

			if (eventSystem != null)
			{
				if (eventSystem.currentSelectedGameObject == gameObject)
				{
					selected = true;
				}
			}

			if (lastSelected != selected)
			{
				if (selected == true)
				{
					if (selectTransitions != null)
					{
						selectTransitions.Begin();
					}
				}
				else
				{
					if (deselectTransitions != null)
					{
						deselectTransitions.Begin();
					}
				}

				lastSelected = selected;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
	using TARGET = LeanSelection;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelection_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			if (Any(tgts, t => t.GetComponent<Selectable>() == null))
			{
				Error("This GameObject doesn't have a Selectable component. You must add one, such as a LeanButton.");
			}

			Draw("selectTransitions", "This allows you to perform a transition when this UI element is selected. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.\n\nNOTE: Any transitions you perform here must be reverted in the Deselect Transitions setting using a matching transition component.");
			Draw("deselectTransitions", "This allows you to perform a transition when this UI element is deselected. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.\n\nNOTE: Any transitions you perform here must be reverted in the Select Transitions setting using a matching transition component.");
		}
	}
}
#endif