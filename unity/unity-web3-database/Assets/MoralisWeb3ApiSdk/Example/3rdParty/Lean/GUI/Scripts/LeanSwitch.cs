using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Lean.Transition;
using Lean.Common;

namespace Lean.Gui
{
	/// <summary>This component allows you to make an UI element that can switch between any number of states.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanSwitch")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Switch")]
	public class LeanSwitch : MonoBehaviour
	{
		[System.Serializable] public class IntUnityEvent : UnityEvent<int> {}

		/// <summary>This stores all active and enabled LeanSwitch instances.</summary>
		public static LinkedList<LeanSwitch> Instances = new LinkedList<LeanSwitch>();

		/// <summary>This is the currently active state of the switch.
		/// For example, if this is 0 then the switch is currently in the first transition state.</summary>
		public int State { set { Switch(value); } get { return state; } } [SerializeField] private int state;

		/// <summary>This stores a list of all switch transition states. This controls how many states can be switched between.
		/// For example, if you want to be able to switch between 4 states, then make sure the size of this list = 4.</summary>
		public List<LeanPlayer> States { get { if (states == null) states = new List<LeanPlayer>(); return states; } } [SerializeField] private List<LeanPlayer> states;

		/// <summary>This allows you to perform a transition when this switch changes to a different state.
		/// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		/// For example, the <b>LeanPlaySound (Play Sound Transition)</b> component can be used to play a switch sound.</summary>
		public LeanPlayer ChangedStateTransitions { get { if (changedStateTransitions == null) changedStateTransitions = new LeanPlayer(); return changedStateTransitions; } } [SerializeField] private LeanPlayer changedStateTransitions;

		/// <summary>This allows you to perform an action when this switch changes to a different state.</summary>
		public IntUnityEvent OnChangedState { get { if (onChangedState == null) onChangedState = new IntUnityEvent(); return onChangedState; } } [SerializeField] private IntUnityEvent onChangedState;

		[System.NonSerialized]
		private LinkedListNode<LeanSwitch> link;
		
		/// <summary>This allows you to switch to a different state, where 0 is the first state. The amount of states is defined by the size of the <b>Transitions</b> list.</summary>
		public void Switch(int newState)
		{
			if (states != null && newState >= 0 && newState < states.Count)
			{
				if (state != newState)
				{
					state = newState;

					var stateTransitions = states[state];

					if (stateTransitions != null)
					{
						stateTransitions.Begin();
					}

					if (changedStateTransitions != null)
					{
						changedStateTransitions.Begin();
					}

					if (onChangedState != null)
					{
						onChangedState.Invoke(state);
					}
				}
			}
		}

		/// <summary>This allows you to switch all active and enabled states with the specified name to the specified state.</summary>
		public static void SwitchAll(string name, int state)
		{
			var node = Instances.First;

			for (var i = Instances.Count - 1; i >= 0; i--)
			{
				var instance = node.Value;

				if (instance.name == name)
				{
					instance.Switch(state);
				}

				node = node.Next;
			}
		}

		protected virtual void OnEnable()
		{
			link = Instances.AddLast(this);
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(link);

			link = null;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
	using UnityEditor;
	using TARGET = LeanSwitch;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSwitch_Editor : LeanEditor
	{
		private void DrawState(TARGET tgt, TARGET[] tgts)
		{
			var state = tgt.State;

			EditorGUI.BeginChangeCheck();

			EditorGUI.showMixedValue = Any(tgts, t => t.State != state);
				state = EditorGUILayout.IntSlider("State", state, 0, tgt.States.Count - 1);
			EditorGUI.showMixedValue = false;

			if (EditorGUI.EndChangeCheck() == true)
			{
				Each(tgts, t => t.State = state);
			}
		}

		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			DrawState(tgt, tgts);
			Draw("states", "This stores a list of all switch transition states. This controls how many states can be switched between. For example, if you want to be able to switch between 4 states, then make sure the size of this list = 4.");
			
			Separator();
			
			Draw("changedStateTransitions", "This allows you to perform a transition when this switch changes to a different state. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the LeanPlaySound (Play Sound Transition) component can be used to play a switch sound.");
			
			Separator();
			
			Draw("onChangedState", "This allows you to perform an action when this switch changes to a different state.");
		}
	}
}
#endif