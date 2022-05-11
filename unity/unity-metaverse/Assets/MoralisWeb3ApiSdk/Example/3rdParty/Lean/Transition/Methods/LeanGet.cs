using UnityEngine;

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		/// <summary>This will give you the previously registered transition state.</summary>
		public static LeanState GetTransition<T>(this T target)
			where T : Component
		{
			return LeanTransition.PreviousState;
		}

		/// <summary>This will give you the previously registered transition state.</summary>
		public static LeanState GetTransition(this GameObject target)
		{
			return LeanTransition.PreviousState;
		}
	}
}