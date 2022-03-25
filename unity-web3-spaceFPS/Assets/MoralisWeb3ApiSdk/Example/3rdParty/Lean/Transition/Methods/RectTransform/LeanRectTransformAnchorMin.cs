using TARGET = UnityEngine.RectTransform;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the RectTransform's anchorMin value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanRectTransformAnchorMin")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "RectTransform/RectTransform.anchorMin" + LeanTransition.MethodsMenuSuffix + "(LeanRectTransformAnchorMin)")]
	public class LeanRectTransformAnchorMin : LeanMethodWithStateAndTarget
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
			[UnityEngine.Tooltip("The anchorMin value will transition to this.")]
			[UnityEngine.Serialization.FormerlySerializedAs("AnchorMin")]public UnityEngine.Vector2 Value;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private UnityEngine.Vector2 oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.anchorMin != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.anchorMin;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.anchorMin;
			}

			public override void UpdateWithTarget(float progress)
			{
				Target.anchorMin = UnityEngine.Vector2.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
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
		public static TARGET anchorMinTransition(this TARGET target, UnityEngine.Vector2 value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanRectTransformAnchorMin.Register(target, value, duration, ease); return target;
		}
	}
}