using UnityEngine;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to specify which transition must finish before the next transition can begin.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanQueue")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Queue" + LeanTransition.MethodsMenuSuffix + "(LeanQueue)")]
	public class LeanQueue : LeanMethod
	{
		public override void Register()
		{
			LeanTransition.CurrentQueue = Target != null ? Target.PreviousState : null;
		}

		[Tooltip("The next transition will only begin after this transition has finished.")]
		public LeanMethodWithState Target;
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		/// <summary>This allows you to specify which transition must finish before the next transition can begin.</summary>
		public static T QueueTransition<T>(this T target, LeanState beginAfter)
			where T : Component
		{
			LeanTransition.CurrentQueue = beginAfter; return target;
		}

		/// <summary>This allows you to specify which transition must finish before the next transition can begin.</summary>
		public static GameObject QueueTransition(this GameObject target, LeanState beginAfter)
		{
			LeanTransition.CurrentQueue = beginAfter; return target;
		}
	}
}