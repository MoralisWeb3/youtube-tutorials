using TARGET = UnityEngine.Transform;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the Transform's localPosition.z value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformLocalPosition_z")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Transform/Transform.localPosition.z" + LeanTransition.MethodsMenuSuffix + "(LeanTransformLocalPosition_z)")]
	public class LeanTransformLocalPosition_z : LeanMethodWithStateAndTarget
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
			[UnityEngine.Tooltip("The localPosition value will transition to this.")]
			[UnityEngine.Serialization.FormerlySerializedAs("Position")]public float Value;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private float oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.localPosition.z != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.localPosition.z;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.localPosition.z;
			}

			public override void UpdateWithTarget(float progress)
			{
				var vector = Target.localPosition;
				
				vector.z = UnityEngine.Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
				 
				Target.localPosition = vector;
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
		public static TARGET localPositionTransition_z(this TARGET target, float value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformLocalPosition_z.Register(target, value, duration, ease); return target;
		}
	}
}