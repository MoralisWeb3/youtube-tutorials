using TARGET = UnityEngine.Transform;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the Transform's localScale value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformLocalScale")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Transform/Transform.localScale" + LeanTransition.MethodsMenuSuffix + "(LeanTransformLocalScale)")]
	public class LeanTransformLocalScale : LeanMethodWithStateAndTarget
	{
		public override System.Type GetTargetType()
		{
			return typeof(TARGET);
		}

		public override void Register()
		{
			PreviousState = Register(GetAliasedTarget(Data.Target), Data.Value, Data.Duration, Data.Ease);
		}

		public static LeanState Register(TARGET target, UnityEngine.Vector3 value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			var state = LeanTransition.SpawnWithTarget(State.Pool, target);

			state.Value = value;
			
			state.Ease = ease;

			return LeanTransition.Register(state, duration);
		}

		[System.Serializable]
		public class State : LeanStateWithTarget<TARGET>
		{
			[UnityEngine.Tooltip("The localScale value will transition to this.")]
			[UnityEngine.Serialization.FormerlySerializedAs("Scale")]public UnityEngine.Vector3 Value = UnityEngine.Vector3.one;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private UnityEngine.Vector3 oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.localScale != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.localScale;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.localScale;
			}

			public override void UpdateWithTarget(float progress)
			{
				Target.localScale = UnityEngine.Vector3.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
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
		public static TARGET localScaleTransition(this TARGET target, UnityEngine.Vector3 value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformLocalScale.Register(target, value, duration, ease); return target;
		}
	}
}