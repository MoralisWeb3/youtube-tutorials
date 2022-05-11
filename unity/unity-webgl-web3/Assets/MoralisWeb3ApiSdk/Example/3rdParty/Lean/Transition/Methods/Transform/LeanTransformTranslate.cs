using UnityEngine;
using System.Collections.Generic;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the specified Transform.Translate to the target value.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformTranslate")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Transform/Transform.Translate" + LeanTransition.MethodsMenuSuffix + "(LeanTransformTranslate)")]
	public class LeanTransformTranslate : LeanMethodWithStateAndTarget
	{
		public override System.Type GetTargetType()
		{
			return typeof(Transform);
		}

		public override void Register()
		{
			if (Data.RelativeTo != null)
			{
				PreviousState = Register(GetAliasedTarget(Data.Target), Data.Translation, Data.RelativeTo, Data.Duration, Data.Ease);
			}
			else
			{
				PreviousState = Register(GetAliasedTarget(Data.Target), Data.Translation, Data.Space, Data.Duration, Data.Ease);
			}
		}

		public static LeanState Register(Transform target, Vector3 translation, Transform relativeTo, float duration, LeanEase ease = LeanEase.Smooth)
		{
			var state = LeanTransition.SpawnWithTarget(State.Pool, target);

			state.Translation = translation;
			state.Space       = Space.Self;
			state.RelativeTo  = relativeTo;
			state.Ease        = ease;

			return LeanTransition.Register(state, duration);
		}

		public static LeanState Register(Transform target, Vector3 translation, Space space, float duration, LeanEase ease = LeanEase.Smooth)
		{
			var state = LeanTransition.SpawnWithTarget(State.Pool, target);

			state.Translation = translation;
			state.Space       = space;
			state.RelativeTo  = null;
			state.Ease        = ease;

			return LeanTransition.Register(state, duration);
		}

		[System.Serializable]
		public class State : LeanStateWithTarget<Transform>
		{
			[Tooltip("The amount we will translate.")]
			public Vector3 Translation;

			[Tooltip("The space we will transition in.")]
			public Space Space = Space.Self;

			[Tooltip("The space we will transition in.")]
			public Transform RelativeTo;

			[Tooltip("The ease method that will be used for the transition.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private Vector3 oldTranslation;

			public override ConflictType Conflict
			{
				get
				{
					return ConflictType.None;
				}
			}

			public override void BeginWithTarget()
			{
				oldTranslation = Vector3.zero;
			}

			public override void UpdateWithTarget(float progress)
			{
				var newTranslation = Translation * Smooth(Ease, progress);

				if (RelativeTo != null)
				{
					Target.Translate(newTranslation - oldTranslation, RelativeTo);
				}
				else
				{
					Target.Translate(newTranslation - oldTranslation, Space);
				}

				oldTranslation = newTranslation;
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
		public static Transform TranslateTransition(this Transform target, float x, float y, float z, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformTranslate.Register(target, new Vector3(x, y, z), Space.Self, duration, ease); return target;
		}

		public static Transform TranslateTransition(this Transform target, Vector3 translation, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformTranslate.Register(target, translation, Space.Self, duration, ease); return target;
		}

		public static Transform TranslateTransition(this Transform target, float x, float y, float z, Space space, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformTranslate.Register(target, new Vector3(x, y, z), space, duration, ease); return target;
		}

		public static Transform TranslateTransition(this Transform target, Vector3 translation, Space space, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformTranslate.Register(target, translation, space, duration, ease); return target;
		}

		public static Transform TranslateTransition(this Transform target, float x, float y, float z, Transform relativeTo, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformTranslate.Register(target, new Vector3(x, y, z), relativeTo, duration, ease); return target;
		}

		public static Transform TranslateTransition(this Transform target, Vector3 translation, Transform relativeTo, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformTranslate.Register(target, translation, relativeTo, duration, ease); return target;
		}
	}
}