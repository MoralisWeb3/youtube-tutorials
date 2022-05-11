using UnityEngine;

namespace Lean.Transition.Method
{
	/// <summary>This allows you to change where in the game loop transitions after this will update.
	/// NOTE: Once you submit the previous transitions, this will be reset to default.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanJoinDelay")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "JoinDelay" + LeanTransition.MethodsMenuSuffix + "(LeanJoinDelay)")]
	public class LeanJoinDelay : LeanMethod
	{
		public override void Register()
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState; LeanDelay.Register(Duration); LeanTransition.CurrentQueue = LeanTransition.PreviousState;
		}

		public float Duration;
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		/// <summary>This allows you to connect the previous and next transitions, and insert a delay. This means the next transition will only begin when the previous one finishes.</summary>
		public static T JoinDelayTransition<T>(this T target, float delay)
			where T : Component
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState; Method.LeanDelay.Register(delay); LeanTransition.CurrentQueue = LeanTransition.PreviousState; return target;
		}

		/// <summary>This allows you to connect the previous and next transitions, and insert a delay. This means the next transition will only begin when the previous one finishes.</summary>
		public static GameObject JoinDelayTransition(this GameObject target, float delay)
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState; Method.LeanDelay.Register(delay); LeanTransition.CurrentQueue = LeanTransition.PreviousState; return target;
		}
	}
}