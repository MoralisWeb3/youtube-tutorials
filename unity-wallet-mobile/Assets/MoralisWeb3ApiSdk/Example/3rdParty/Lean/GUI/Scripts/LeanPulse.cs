using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Lean.Transition;
using Lean.Common;

namespace Lean.Gui
{
	/// <summary>This component allows you to trigger a transition and/or event at regular intervals.</summary>
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanPulse")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Pulse")]
	public class LeanPulse : MonoBehaviour
	{
		[System.Serializable] public class IntEvent : UnityEvent<int> {}

		/// <summary>This stores all active and enabled LeanPulse instances, so you can manually pulse them by name from anywhere.</summary>
		public static LinkedList<LeanPulse> Instances = new LinkedList<LeanPulse>();

		/// <summary>This tells you how many pulses remain until this component stops.
		/// -1 = unlimited</summary>
		public int RemainingPulses { set { remainingPulses = value; } get { return remainingPulses; } } [SerializeField] private int remainingPulses = -1;

		/// <summary>When this reaches 0, and RemainingPulses is not 0, this component will pulse.</summary>
		public float RemainingTime { set { remainingTime = value; } get { return remainingTime; } } [SerializeField] private float remainingTime;

		/// <summary>This allows you to control the amount of seconds between each pulse.
		/// -1 = Manual Pulses Only</summary>
		public float TimeInterval { set { timeInterval = value; } get { return timeInterval; } } [SerializeField] private float timeInterval = 1.0f;

		/// <summary>This allows you to choose where in the game loop this pulse will update.</summary>
		public LeanTiming Timing { set { timing = value; } get { return timing; } } [SerializeField] private LeanTiming timing;

		/// <summary>This allows you to perform an animation when this UI element pulses.
		/// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		/// For example, the <b>LeanPlaySound (Play Sound Transition)</b> component can be used to play a pulse sound.</summary>
		public LeanPlayer PulseTransitions { get { if (pulseTransitions == null) pulseTransitions = new LeanPlayer(); return pulseTransitions; } } [SerializeField] private LeanPlayer pulseTransitions;

		/// <summary>This allows you to perform an action when this UI element pulses.
		/// int = RemainingPulses</summary>
		public IntEvent OnPulse { get { if (onPulse == null) onPulse = new IntEvent(); return onPulse; } } [SerializeField] private IntEvent onPulse;

		[System.NonSerialized]
		private LinkedListNode<LeanPulse> link;

		/// <summary>This will pulse, as long as you have remaining pulses.</summary>
		[ContextMenu("Try Pulse")]
		public void TryPulse()
		{
			if (remainingPulses >= 0)
			{
				if (remainingPulses > 0)
				{
					remainingPulses--;
				}
				else
				{
					return;
				}
			}

			remainingTime = timeInterval;

			Pulse();
		}

		/// <summary>This allows you to manually force this component to pulse right now.</summary>
		[ContextMenu("Pulse")]
		public void Pulse()
		{
			if (pulseTransitions != null)
			{
				pulseTransitions.Begin();
			}

			if (onPulse != null)
			{
				onPulse.Invoke(remainingPulses);
			}
		}

		/// <summary>This method calls TryPulse on all active and enabled LeanPulse instances with the specified GameObject name.</summary>
		public static void TryPulseAll(string name)
		{
			var node = Instances.First;

			for (var i = Instances.Count - 1; i >= 0; i--)
			{
				var instance = node.Value;

				if (instance.name == name)
				{
					instance.TryPulse();
				}

				node = node.Next;
			}
		}

		/// <summary>This method calls Pulse on all active and enabled LeanPulse instances with the specified GameObject name.</summary>
		public static void PulseAll(string name)
		{
			var node = Instances.First;

			for (var i = Instances.Count - 1; i >= 0; i--)
			{
				var instance = node.Value;

				if (instance.name == name)
				{
					instance.Pulse();
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

		protected virtual void Update()
		{
			var finalUpdate = LeanTransition.GetTiming(timing);

			if (LeanTransition.GetTimingAbs(finalUpdate) == LeanTiming.Update)
			{
				UpdatePulse(finalUpdate > 0 ? Time.deltaTime : Time.unscaledDeltaTime);
			}
		}

		protected virtual void LateUpdate()
		{
			var finalUpdate = LeanTransition.GetTiming(timing);

			if (LeanTransition.GetTimingAbs(finalUpdate) == LeanTiming.LateUpdate)
			{
				UpdatePulse(finalUpdate > 0 ? Time.deltaTime : Time.unscaledDeltaTime);
			}
		}

		protected virtual void FixedUpdate()
		{
			var finalUpdate = LeanTransition.GetTiming(timing);

			if (LeanTransition.GetTimingAbs(finalUpdate) == LeanTiming.FixedUpdate)
			{
				UpdatePulse(finalUpdate > 0 ? Time.fixedDeltaTime : Time.fixedUnscaledDeltaTime);
			}
		}

		private void UpdatePulse(float delta)
		{
			remainingTime -= delta;

			if (remainingTime <= 0.0f)
			{
				remainingTime = timeInterval;

				TryPulse();
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
	using TARGET = LeanPulse;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanPulse_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("remainingPulses", "This tells you how many pulses remain until this component stops.\n\n-1 = unlimited");
			Draw("remainingTime", "When this reaches 0, and RemainingPulses is not 0, this component will pulse.");
			Draw("timeInterval", "This allows you to control the amount of seconds between each pulse.\n\n-1 = Manual Pulses Only");
			Draw("timing", "This allows you to choose where in the game loop this pulse will update.");

			Separator();

			Draw("pulseTransitions", "This allows you to perform an animation when this UI element pulses.");

			Separator();

			Draw("onPulse");
		}
	}
}
#endif