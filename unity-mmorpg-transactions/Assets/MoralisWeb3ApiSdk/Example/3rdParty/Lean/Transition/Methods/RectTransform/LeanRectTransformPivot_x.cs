using TARGET = UnityEngine.RectTransform;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the RectTransform's pivot.x value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanRectTransformPivot_x")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "RectTransform/RectTransform.pivot.x" + LeanTransition.MethodsMenuSuffix + "(LeanRectTransformPivot_x)")]
	public class LeanRectTransformPivot_x : LeanMethodWithStateAndTarget
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
			[UnityEngine.Tooltip("The pivot value will transition to this.")]
			[UnityEngine.Serialization.FormerlySerializedAs("Pivot")]public float Value;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private float oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.pivot.x != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.pivot.x;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.pivot.x;
			}

			public override void UpdateWithTarget(float progress)
			{
				var vector = Target.pivot;
				
				vector.x = UnityEngine.Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
				 
				Target.pivot = vector;
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
		public static TARGET pivotTransition_x(this TARGET target, float value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanRectTransformPivot_x.Register(target, value, duration, ease); return target;
		}
	}
}