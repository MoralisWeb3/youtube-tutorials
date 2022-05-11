using TARGET = UnityEngine.Transform;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the Transform's position.xy value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformPosition_xy")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Transform/Transform.position.xy" + LeanTransition.MethodsMenuSuffix + "(LeanTransformPosition_xy)")]
	public class LeanTransformPosition_xy : LeanMethodWithStateAndTarget
	{
		public override System.Type GetTargetType()
		{
			return typeof(TARGET);
		}

		public override void Register()
		{
			PreviousState = Register(GetAliasedTarget(Data.Target), Data.Value, Data.Duration, Data.Ease);
		}

		public static LeanState Register(TARGET target, UnityEngine.Vector2 value, float duration, LeanEase ease = LeanEase.Smooth)
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
			public UnityEngine.Vector2 Value;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private UnityEngine.Vector2 oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && (Target.position.x != Value.x || Target.position.y != Value.y) ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				var vector = Target.position;
				
				Value.x = vector.x;
				Value.y = vector.y;
			}

			public override void BeginWithTarget()
			{
				var vector = Target.position;
				
				oldValue.x = vector.x;
				oldValue.y = vector.y;
			}

			public override void UpdateWithTarget(float progress)
			{
				var vector = Target.position;
				var smooth = Smooth(Ease, progress);
				
				vector.x = UnityEngine.Mathf.LerpUnclamped(oldValue.x, Value.x, smooth);
				vector.y = UnityEngine.Mathf.LerpUnclamped(oldValue.y, Value.y, smooth);
				 
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
		public static TARGET positionTransition_xy(this TARGET target, UnityEngine.Vector2 value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformPosition_xy.Register(target, value, duration, ease); return target;
		}
	}
}