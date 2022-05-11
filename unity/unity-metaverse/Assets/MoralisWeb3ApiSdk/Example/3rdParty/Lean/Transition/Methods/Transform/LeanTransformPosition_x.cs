using TARGET = UnityEngine.Transform;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the Transform's position.x value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformPosition_X")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Transform/Transform.position.x" + LeanTransition.MethodsMenuSuffix + "(LeanTransformPosition_X)")]
	public class LeanTransformPosition_X : LeanMethodWithStateAndTarget
	{
		public override System.Type GetTargetType()
		{
			return typeof(TARGET);
		}

		public override void Register()
		{
			PreviousState = Register(GetAliasedTarget(Data.Target), Data.Value, Data.Duration, Data.Ease);
		}

		public static LeanState Register(TARGET target, float value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			var state = LeanTransition.SpawnWithTarget(State.Pool, target);

			state.Value = value;
			
			state.Ease = ease;

			return LeanTransition.Register(state, duration);
		}

		[System.Serializable]
		public class State : LeanStateWithTarget<TARGET>
		{
			[UnityEngine.Tooltip("The position value will transition to this.")]
			[UnityEngine.Serialization.FormerlySerializedAs("Position")]public float Value;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private float oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.position.x != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.position.x;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.position.x;
			}

			public override void UpdateWithTarget(float progress)
			{
				var vector = Target.position;
				
				vector.x = UnityEngine.Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
				 
				Target.position = vector;
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
		public static TARGET positionTransition_x(this TARGET target, float value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformPosition_X.Register(target, value, duration, ease); return target;
		}
	}
}