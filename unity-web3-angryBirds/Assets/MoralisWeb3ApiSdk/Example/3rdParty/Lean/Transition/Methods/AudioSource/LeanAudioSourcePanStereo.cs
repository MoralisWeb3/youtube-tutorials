using TARGET = UnityEngine.AudioSource;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the AudioSource's panStereo value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanAudioSourcePanStereo")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "AudioSource/AudioSource.panStereo" + LeanTransition.MethodsMenuSuffix + "(LeanAudioSourcePanStereo)")]
	public class LeanAudioSourcePanStereo : LeanMethodWithStateAndTarget
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
			[UnityEngine.Tooltip("The panStereo value will transition to this.")]
			[UnityEngine.Range(-1.0f, 1.0f)]public float Value;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private float oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.panStereo != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.panStereo;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.panStereo;
			}

			public override void UpdateWithTarget(float progress)
			{
				Target.panStereo = UnityEngine.Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
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
		public static TARGET panStereoTransition(this TARGET target, float value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanAudioSourcePanStereo.Register(target, value, duration, ease); return target;
		}
	}
}