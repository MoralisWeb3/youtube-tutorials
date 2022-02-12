using UnityEngine;

namespace Lean.Transition.Method
{
	/// <summary>This allows you to change where in the game loop transitions after this will update.
	/// NOTE: Once you submit the previous transitions, this will be reset to default.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanJoinInsert")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "JoinInsert" + LeanTransition.MethodsMenuSuffix + "(LeanJoinInsert)")]
	public class LeanJoinInsert : LeanMethod
	{
		public Transform Target;

		public float Speed = 1.0f;

		public override void Register()
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState; LeanTransition.InsertTransitions(Target); LeanTransition.CurrentQueue = LeanTransition.PreviousState;
		}
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		/// <summary>This will insert all transitions inside the specified GameObject, as if they were added manually.</summary>
		public static T JoinInsertTransition<T>(this T target, GameObject root, float speed = 1.0f)
			where T : Component
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState; LeanTransition.InsertTransitions(root, speed); LeanTransition.CurrentQueue = LeanTransition.PreviousState; return target;
		}

		/// <summary>This will insert all transitions inside the specified GameObject, as if they were added manually.</summary>
		public static T JoinInsertTransition<T>(this T target, Transform root, float speed = 1.0f)
			where T : Component
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState; LeanTransition.InsertTransitions(root, speed); LeanTransition.CurrentQueue = LeanTransition.PreviousState; return target;
		}

		/// <summary>This will insert all transitions inside the specified GameObject, as if they were added manually.</summary>
		public static GameObject JoinInsertTransition(this GameObject target, GameObject root, float speed = 1.0f)
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState; LeanTransition.InsertTransitions(root, speed); LeanTransition.CurrentQueue = LeanTransition.PreviousState; return target;
		}

		/// <summary>This will insert all transitions inside the specified GameObject, as if they were added manually.</summary>
		public static GameObject JoinInsertTransition(this GameObject target, Transform root, float speed = 1.0f)
		{
			LeanTransition.CurrentQueue = LeanTransition.PreviousState; LeanTransition.InsertTransitions(root, speed); LeanTransition.CurrentQueue = LeanTransition.PreviousState; return target;
		}
	}
}