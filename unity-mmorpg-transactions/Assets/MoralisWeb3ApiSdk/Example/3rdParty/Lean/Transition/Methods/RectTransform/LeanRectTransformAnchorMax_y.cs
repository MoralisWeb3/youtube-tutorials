using TARGET = UnityEngine.RectTransform;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the RectTransform's anchorMax.y value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanRectTransformAnchorMax_y")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "RectTransform/RectTransform.anchorMax.y" + LeanTransition.MethodsMenuSuffix + "(LeanRectTransformAnchorMax_y)")]
	public class LeanRectTransformAnchorMax_y : LeanMethodWithStateAndTarget
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
			[UnityEngine.Tooltip("The anchorMax value will transition to this.")]
			public float Value;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private float oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.anchorMax.y != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.anchorMax.y;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.anchorMax.y;
			}

			public override void UpdateWithTarget(float progress)
			{
				var vector = Target.anchorMax;
				
				vector.y = UnityEngine.Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
				 
				Target.anchorMax = vector;
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
		public static TARGET anchorMaxTransition_y(this TARGET target, float value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanRectTransformAnchorMax_y.Register(target, value, duration, ease); return target;
		}
	}
}