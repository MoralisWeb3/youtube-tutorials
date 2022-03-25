using UnityEngine;
using System.Collections.Generic;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the specified <b>Material</b>'s <b>color</b> to the target value.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanMaterialColor")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Material/Material color" + LeanTransition.MethodsMenuSuffix + "(LeanMaterialColor)")]
	public class LeanMaterialColor : LeanMethodWithStateAndTarget
	{
		public override System.Type GetTargetType()
		{
			return typeof(Material);
		}

		public override void Register()
		{
			PreviousState = Register(GetAliasedTarget(Data.Target), Data.Property, Data.Color, Data.Duration, Data.Ease);
		}

		public static LeanState Register(Material target, string property, Color color, float duration, LeanEase ease = LeanEase.Smooth)
		{
			var state = LeanTransition.SpawnWithTarget(State.Pool, target);

			state.Property = property;
			state.Color    = color;
			state.Ease     = ease;

			return LeanTransition.Register(state, duration);
		}

		[System.Serializable]
		public class State : LeanStateWithTarget<Material>
		{
			[Tooltip("The name of the color property in the shader.")]
			public string Property = "_Color";

			[Tooltip("The color we will transition to.")]
			public Color Color = Color.white;

			[Tooltip("The ease method that will be used for the transition.")]
			public LeanEase Ease  = LeanEase.Smooth;

			[System.NonSerialized] private Color oldColor;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.GetColor(Property) != Color ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Color = Target.GetColor(Property);
			}

			public override void BeginWithTarget()
			{
				oldColor = Target.GetColor(Property);
			}

			public override void UpdateWithTarget(float progress)
			{
				Target.SetColor(Property, Color.LerpUnclamped(oldColor, Color, Smooth(Ease, progress)));
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
		public static Material colorTransition(this Material target, string property, Color color, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanMaterialColor.Register(target, property, color, duration, ease); return target;
		}
	}
}