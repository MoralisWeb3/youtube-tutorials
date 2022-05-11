using TARGET = UnityEngine.RectTransform;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the RectTransform's sizeDelta value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanRectTransformSizeDelta")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "RectTransform/RectTransform.sizeDelta" + LeanTransition.MethodsMenuSuffix + "(LeanRectTransformSizeDelta)")]
	public class LeanRectTransformSizeDelta : LeanMethodWithStateAndTarget
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
			[UnityEngine.Tooltip("The sizeDelta value will transition to this.")]
			[UnityEngine.Serialization.FormerlySerializedAs("SizeDelta")]public UnityEngine.Vector2 Value;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private UnityEngine.Vector2 oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.sizeDelta != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.sizeDelta;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.sizeDelta;
			}

			public override void UpdateWithTarget(float progress)
			{
				Target.sizeDelta = UnityEngine.Vector2.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
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
		public static TARGET sizeDeltaTransition(this TARGET target, UnityEngine.Vector2 value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanRectTransformSizeDelta.Register(target, value, duration, ease); return target;
		}
	}
}