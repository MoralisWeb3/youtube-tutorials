using UnityEngine;
using Lean.Common;

namespace Lean.Transition.Method
{
	/// <summary>This allows you to change where in the game loop transitions after this will update.
	/// NOTE: Once you submit the previous transitions, this will be reset to default.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanJoin")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Join" + LeanTransition.MethodsMenuSuffix + "(LeanJoin)")]
	public class LeanJoin : LeanMethod
	{
		public override void Register()
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState;
		}
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		/// <summary>This allows you to connect the previous and next transitions. This means the next transition will only begin when the previous one finishes.</summary>
		public static T JoinTransition<T>(this T target)
			where T : Component
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState; return target;
		}

		/// <summary>This allows you to connect the previous and next transitions. This means the next transition will only begin when the previous one finishes.</summary>
		public static GameObject JoinTransition(this GameObject target)
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState; return target;
		}
	}
}