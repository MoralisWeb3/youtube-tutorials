using TARGET = UnityEngine.SpriteRenderer;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the SpriteRenderer's color value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanSpriteRendererColor")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "SpriteRenderer/SpriteRenderer.color" + LeanTransition.MethodsMenuSuffix + "(LeanSpriteRendererColor)")]
	public class LeanSpriteRendererColor : LeanMethodWithStateAndTarget
	{
		public override System.Type GetTargetType()
		{
			return typeof(TARGET);
		}

		public override void Register()
		{
			PreviousState = Register(GetAliasedTarget(Data.Target), Data.Value, Data.Duration, Data.Ease);
		}

		public static LeanState Register(TARGET target, UnityEngine.Color value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			var state = LeanTransition.SpawnWithTarget(State.Pool, target);

			state.Value = value;
			
			state.Ease = ease;

			return LeanTransition.Register(state, duration);
		}

		[System.Serializable]
		public class State : LeanStateWithTarget<TARGET>
		{
			[UnityEngine.Tooltip("The color value will transition to this.")]
			[UnityEngine.Serialization.FormerlySerializedAs("Color")]public UnityEngine.Color Value = UnityEngine.Color.white;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private UnityEngine.Color oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.color != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.color;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.color;
			}

			public override void UpdateWithTarget(float progress)
			{
				Target.color = UnityEngine.Color.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
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
		public static TARGET colorTransition(this TARGET target, UnityEngine.Color value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanSpriteRendererColor.Register(target, value, duration, ease); return target;
		}
	}
}