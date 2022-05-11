using TARGET = UnityEngine.UI.Image;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the Image's fillAmount value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanImageFillAmount")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Image/Image.fillAmount" + LeanTransition.MethodsMenuSuffix + "(LeanImageFillAmount)")]
	public class LeanImageFillAmount : LeanMethodWithStateAndTarget
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
			[UnityEngine.Tooltip("The fillAmount value will transition to this.")]
			[UnityEngine.Serialization.FormerlySerializedAs("FillAmount")]public float Value;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private float oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.fillAmount != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.fillAmount;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.fillAmount;
			}

			public override void UpdateWithTarget(float progress)
			{
				Target.fillAmount = UnityEngine.Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
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
		public static TARGET fillAmountTransition(this TARGET target, float value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanImageFillAmount.Register(target, value, duration, ease); return target;
		}
	}
}