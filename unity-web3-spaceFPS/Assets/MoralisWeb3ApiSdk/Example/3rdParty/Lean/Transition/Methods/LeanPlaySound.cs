using UnityEngine;
using System.Collections.Generic;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to play a sound after the specified duration.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanPlaySound")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Play Sound" + LeanTransition.MethodsMenuSuffix + "(LeanPlaySound)")]
	public class LeanPlaySound : LeanMethodWithStateAndTarget
	{
		public override System.Type GetTargetType()
		{
			return typeof(AudioClip);
		}

		public override void Register()
		{
			PreviousState = Register(GetAliasedTarget(Data.Target), Data.Duration, Data.Volume);
		}

		public static LeanState Register(AudioClip target, float duration, float volume = 1.0f)
		{
			var state = LeanTransition.SpawnWithTarget(State.Pool, target);

			state.Volume = volume;

			return LeanTransition.Register(state, duration);
		}

		[System.Serializable]
		public class State : LeanStateWithTarget<AudioClip>
		{
			[Range(0.0f, 1.0f)]
			public float Volume = 1.0f;

			public override int CanFill
			{
				get
				{
					return 0;
				}
			}

			public override void UpdateWithTarget(float progress)
			{
				if (progress == 1.0f)
				{
#if UNITY_EDITOR
					if (Application.isPlaying == false)
					{
						return;
					}
#endif
					var gameObject  = new GameObject(Target.name);
					var audioSource = gameObject.AddComponent<AudioSource>();

					audioSource.clip   = Target;
					audioSource.volume = Volume;

					audioSource.Play();

					Destroy(gameObject, Target.length);
				}
			}

			public static Stack<State> Pool = new Stack<State>(); public override void Despawn() { Pool.Push(this); }
		}

		public State Data;
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		public static T PlaySoundTransition<T>(this T target, AudioClip clip, float duration = 0.0f, float volume = 1.0f)
			where T : Component
		{
			Method.LeanPlaySound.Register(clip, duration, volume); return target;
		}

		public static GameObject PlaySoundTransition(this GameObject target, AudioClip clip, float duration = 0.0f, float volume = 1.0f)
		{
			Method.LeanPlaySound.Register(clip, duration, volume); return target;
		}
	}
}