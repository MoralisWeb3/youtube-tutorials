using TARGET = UnityEngine.CanvasGroup;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the CanvasGroup's blocksRaycasts value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanCanvasGroupBlocksRaycasts")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "CanvasGroup/CanvasGroup.blocksRaycasts" + LeanTransition.MethodsMenuSuffix + "(LeanCanvasGroupBlocksRaycasts)")]
	public class LeanCanvasGroupBlocksRaycasts : LeanMethodWithStateAndTarget
	{
		public override System.Type GetTargetType()
		{
			return typeof(TARGET);
		}

		public override void Register()
		{
			PreviousState = Register(GetAliasedTarget(Data.Target), Data.Value, Data.Duration);
		}

		public static LeanState Register(TARGET target, bool value, float duration)
		{
			var state = LeanTransition.SpawnWithTarget(State.Pool, target);

			state.Value = value;

			return LeanTransition.Register(state, duration);
		}

		[System.Serializable]
		public class State : LeanStateWithTarget<TARGET>
		{
			[UnityEngine.Tooltip("The blocksRaycasts value will transition to this.")]
			[UnityEngine.Serialization.FormerlySerializedAs("BlocksRaycasts")]public bool Value = true;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.blocksRaycasts != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.blocksRaycasts;
			}

			public override void BeginWithTarget()
			{
			}

			public override void UpdateWithTarget(float progress)
			{
				if (progress == 1.0f)
				{
					Target.blocksRaycasts = Value;
				}
			}

			public static System.Collections.Generic.Stack<State> Pool = new System.Collections.Generic.Stack<State>(); public override void Despawn() { Pool.Push(this); }
		}

		public State Data;
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		public static TARGET blocksRaycastsTransition(this TARGET target, bool value, float duration)
		{
			Method.LeanCanvasGroupBlocksRaycasts.Register(target, value, duration); return target;
		}
	}
}