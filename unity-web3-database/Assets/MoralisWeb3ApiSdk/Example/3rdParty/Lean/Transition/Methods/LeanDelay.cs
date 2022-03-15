using UnityEngine;
using System.Collections.Generic;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to delay for a specified duration.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanDelay")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Delay" + LeanTransition.MethodsMenuSuffix + "(LeanDelay)")]
	public class LeanDelay : LeanMethodWithState
	{
		public override void Register()
		{
			PreviousState = Register(Data.Duration);
		}

		public static LeanState Register(float duration)
		{
			var state = LeanTransition.Spawn(State.Pool);

			return LeanTransition.Register(state, duration);
		}

		[System.Serializable]
		public class State : LeanState
		{
			public override void Begin()
			{
				// No state to begin from
			}

			public override void Update(float progress)
			{
				// No state to update
			}

			public static Stack<State> Pool = new Stack<State>(); public override void Despawn() { Pool.Push(this); }
		}

		public State Data;
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		/// <summary>This will pause the animation for the specified amount of seconds.</summary>
		public static T DelayTransition<T>(this T target, float duration)
			where T : Component
		{
			Method.LeanDelay.Register(duration); return target;
		}

		/// <summary>This will pause the animation for the specified amount of seconds.</summary>
		public static GameObject DelayTransition(this GameObject target, float duration)
		{
			Method.LeanDelay.Register(duration); return target;
		}
	}
}