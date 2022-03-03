using UnityEngine;
using System.Collections.Generic;
using Lean.Common;

namespace Lean.Transition
{
	/// <summary>This component allows you to manually begin transitions from UI button events and other sources.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanManualAnimation")]
	[AddComponentMenu(LeanTransition.ComponentMenuPrefix + "Lean Manual Animation")]
	public class LeanManualAnimation : MonoBehaviour
	{
		/// <summary>This allows you to specify the transitions this component will begin.
		/// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		/// For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color back to normal.</summary>
		public LeanPlayer Transitions { get { if (transitions == null) transitions = new LeanPlayer(); return transitions; } } [SerializeField] [UnityEngine.Serialization.FormerlySerializedAs("Transitions")] private LeanPlayer transitions;

		[System.NonSerialized]
		private HashSet<LeanState> states = new HashSet<LeanState>();

		[System.NonSerialized]
		private bool registered;

		/// <summary>This method will execute all transitions on the <b>Transform</b> specified in the <b>Transitions</b> setting.</summary>
		[ContextMenu("Begin Transitions")]
		public void BeginTransitions()
		{
			if (transitions != null)
			{
				if (registered == false)
				{
					registered = true;

					LeanTransition.OnFinished += HandleFinished;
				}

				LeanTransition.OnRegistered += HandleRegistered;

				transitions.Begin();

				LeanTransition.OnRegistered -= HandleRegistered;
			}
		}

		/// <summary>This method will stop all transitions that were begun from this component.</summary>
		[ContextMenu("Stop Transitions")]
		public void StopTransitions()
		{
			foreach (var state in states)
			{
				state.Stop();
			}
		}

		/// <summary>This method will skip all transitions that were begun from this component.</summary>
		[ContextMenu("Skip Transitions")]
		public void SkipTransitions()
		{
			foreach (var state in states)
			{
				state.Skip();
			}
		}

		protected virtual void OnDestroy()
		{
			if (registered == true)
			{
				// Comment this out in case you call BeginTransitions after destruction?
				//registered = false;

				LeanTransition.OnFinished -= HandleFinished;
			}
		}

		private void HandleRegistered(LeanState state)
		{
			states.Add(state);
		}

		private void HandleFinished(LeanState state)
		{
			states.Remove(state);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Transition.Editor
{
	using TARGET = LeanManualAnimation;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanManualAnimation_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("transitions", "This stores the Transforms containing all the transitions that will be performed.");
		}
	}
}
#endif