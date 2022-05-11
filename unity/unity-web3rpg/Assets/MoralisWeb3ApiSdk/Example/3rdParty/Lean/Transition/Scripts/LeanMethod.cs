using UnityEngine;
using Lean.Common;
using System.Collections.Generic;

namespace Lean.Transition
{
	/// <summary>This is the base class for all transition methods.</summary>
	public abstract class LeanMethod : MonoBehaviour
	{
		public abstract void Register();

		[ContextMenu("Begin This Transition")]
		public void BeginThisTransition()
		{
			LeanTransition.RequireSubmitted();

			LeanTransition.CurrentAliases.Clear();

			Register();

			LeanTransition.Submit();
		}

		[ContextMenu("Begin All Transitions")]
		public void BeginAllTransitions()
		{
			LeanTransition.CurrentAliases.Clear();

			LeanTransition.BeginAllTransitions(transform);
		}

		/// <summary>This will take the input linear 0..1 value, and return a transformed version based on the specified easing function.</summary>
		public static float Smooth(LeanEase ease, float x)
		{
			switch (ease)
			{
				case LeanEase.Smooth:
				{
					x = x * x * (3.0f - 2.0f * x);
				}
				break;

				case LeanEase.Accelerate:
				{
					x *= x;
				}
				break;

				case LeanEase.Decelerate:
				{
					x = 1.0f - x;
					x *= x;
					x = 1.0f - x;
				}
				break;

				case LeanEase.Elastic:
				{
					var angle   = x * Mathf.PI * 4.0f;
					var weightA = 1.0f - Mathf.Pow(x, 0.125f);
					var weightB = 1.0f - Mathf.Pow(1.0f - x, 8.0f);

					x = Mathf.LerpUnclamped(0.0f, 1.0f - Mathf.Cos(angle) * weightA, weightB);
				}
				break;

				case LeanEase.Back:
				{
					x = 1.0f - x;
					x = x * x * x - x * Mathf.Sin(x * Mathf.PI);
					x = 1.0f - x;
				}
				break;

				case LeanEase.Bounce:
				{
					if (x < (4f/11f))
					{
						x = (121f/16f) * x * x;
					}
					else if (x < (8f/11f))
					{
						x = (121f/16f) * (x - (6f/11f)) * (x - (6f/11f)) + 0.75f;
					}
					else if (x < (10f/11f))
					{
						x = (121f/16f) * (x - (9f/11f)) * (x - (9f/11f)) + (15f/16f);
					}
					else
					{
						x = (121f/16f) * (x - (21f/22f)) * (x - (21f/22f)) + (63f/64f);
					}
				}
				break;

				case LeanEase.SineIn: return 1 - Mathf.Cos((x * Mathf.PI) / 2.0f);

				case LeanEase.SineOut: return Mathf.Sin((x * Mathf.PI) / 2.0f);

				case LeanEase.SineInOut: return -(Mathf.Cos(Mathf.PI * x) - 1.0f) / 2.0f;

				case LeanEase.QuadIn: return SmoothQuad(x);

				case LeanEase.QuadOut: return 1 - SmoothQuad(1 - x);

				case LeanEase.QuadInOut: return x < 0.5f ? SmoothQuad(x * 2) / 2 : 1 - SmoothQuad(2 - x * 2) / 2;

				case LeanEase.CubicIn: return SmoothCubic(x);

				case LeanEase.CubicOut: return 1 - SmoothCubic(1 - x);

				case LeanEase.CubicInOut: return x < 0.5f ? SmoothCubic(x * 2) / 2 : 1 - SmoothCubic(2 - x * 2) / 2;

				case LeanEase.QuartIn: return SmoothQuart(x);

				case LeanEase.QuartOut: return 1 - SmoothQuart(1 - x);

				case LeanEase.QuartInOut: return x < 0.5f ? SmoothQuart(x * 2) / 2 : 1 - SmoothQuart(2 - x * 2) / 2;

				case LeanEase.QuintIn: return SmoothQuint(x);

				case LeanEase.QuintOut: return 1 - SmoothQuint(1 - x);

				case LeanEase.QuintInOut: return x < 0.5f ? SmoothQuint(x * 2) / 2 : 1 - SmoothQuint(2 - x * 2) / 2;

				case LeanEase.ExpoIn: return SmoothExpo(x);

				case LeanEase.ExpoOut: return 1 - SmoothExpo(1 - x);

				case LeanEase.ExpoInOut: return x < 0.5f ? SmoothExpo(x * 2) / 2 : 1 - SmoothExpo(2 - x * 2) / 2;

				case LeanEase.CircIn: return SmoothCirc(x);

				case LeanEase.CircOut: return 1 - SmoothCirc(1 - x);

				case LeanEase.CircInOut: return x < 0.5f ? SmoothCirc(x * 2) / 2 : 1 - SmoothCirc(2 - x * 2) / 2;

				case LeanEase.BackIn: return SmoothBack(x);

				case LeanEase.BackOut: return 1 - SmoothBack(1 - x);

				case LeanEase.BackInOut: return x < 0.5f ? SmoothBack(x * 2) / 2 : 1 - SmoothBack(2 - x * 2) / 2;

				case LeanEase.ElasticIn: return SmoothElastic(x);

				case LeanEase.ElasticOut: return 1 - SmoothElastic(1 - x);

				case LeanEase.ElasticInOut: return x < 0.5f ? SmoothElastic(x * 2) / 2 : 1 - SmoothElastic(2 - x * 2) / 2;

				case LeanEase.BounceIn: return 1 - SmoothBounce(1 - x);

				case LeanEase.BounceOut: return SmoothBounce(x);

				case LeanEase.BounceInOut: return x < 0.5f ? 0.5f - SmoothBounce(1 - x * 2) / 2 : 0.5f + SmoothBounce(x * 2 - 1) / 2;
			}

			return x;
		}

		private static float SmoothQuad(float x)
		{
			return x * x;
		}

		private static float SmoothCubic(float x)
		{
			return x * x * x;
		}

		private static float SmoothQuart(float x)
		{
			return x * x * x * x;
		}

		private static float SmoothQuint(float x)
		{
			return x * x * x * x * x;
		}

		private static float SmoothExpo(float x)
		{
			return x == 0.0f ? 0.0f : Mathf.Pow(2.0f, 10.0f * x - 10.0f);
		}

		private static float SmoothCirc(float x)
		{
			return 1.0f - Mathf.Sqrt(1.0f - Mathf.Pow(x, 2.0f));
		}

		private static float SmoothBack(float x)
		{
			return 2.70158f * x * x * x - 1.70158f * x * x;
		}

		private static float SmoothElastic(float x)
		{
			return x == 0.0f ? 0.0f : x == 1.0f ? 1.0f : -Mathf.Pow(2.0f, 10.0f * x - 10.0f) * Mathf.Sin((x * 10.0f - 10.75f) * ((2.0f * Mathf.PI) / 3.0f));
		}

		private static float SmoothBounce(float x)
		{
			if (x < (4f/11f))
			{
				return (121f/16f) * x * x;
			}
			else if (x < (8f/11f))
			{
				return (121f/16f) * (x - (6f/11f)) * (x - (6f/11f)) + 0.75f;
			}
			else if (x < (10f/11f))
			{
				return (121f/16f) * (x - (9f/11f)) * (x - (9f/11f)) + (15f/16f);
			}
			else
			{
				return (121f/16f) * (x - (21f/22f)) * (x - (21f/22f)) + (63f/64f);
			}
		}
	}

	public abstract class LeanMethodWithState : LeanMethod
	{
		/// <summary>Each time this transition method registers a new state, it will be stored here.</summary>
		public LeanState PreviousState;
	}

	public abstract class LeanMethodWithStateAndTarget : LeanMethodWithState
	{
		public abstract System.Type GetTargetType();

		[UnityEngine.Serialization.FormerlySerializedAs("Data.TargetAlias")]
		public string Alias;

		/// <summary>This allows you to get the current <b>Target</b> value, or an aliased override.</summary>
		public T GetAliasedTarget<T>(T current)
			where T : Object
		{
			if (string.IsNullOrEmpty(Alias) == false)
			{
				var target = default(Object);

				if (LeanTransition.CurrentAliases.TryGetValue(Alias, out target) == true)
				{
					if (target is T)
					{
						return (T)target;
					}
					else if (target is GameObject)
					{
						var gameObject = (GameObject)target;

						return gameObject.GetComponent(typeof(T)) as T;
					}
				}
			}

			return current;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Transition.Editor
{
	using UnityEditor;
	using TARGET = LeanMethod;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET), true)]
	public class LeanMethod_Editor : LeanEditor
	{
		private static List<LeanMethod> tempMethods = new List<LeanMethod>();

		private bool IsLast
		{
			get
			{
				var tgt = (LeanMethod)target;

				tgt.GetComponents(tempMethods);
			
				return tempMethods.IndexOf(tgt) == tempMethods.Count - 1;
			}
		}

		private void DrawTargetAlias(SerializedProperty sTarget, SerializedProperty sAlias, System.Reflection.FieldInfo data)
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Separator();

			var rect  = Reserve();
			var rectF = rect; rectF.xMin += EditorGUIUtility.labelWidth - 50; rectF.width = 48;
			var rectM = rect; rectM.xMin += EditorGUIUtility.labelWidth;
			var rectL = rectM; rectL.xMax -= rectL.width / 2 + 1;
			var rectR = rectM; rectR.xMin += rectR.width / 2 + 1;

			if (data != null)
			{
				var state = data.GetValue(tgt) as LeanState;

				if (state != null && state.CanFill > -1)
				{
					var rectC = rectM; rectC.width = 28; rectC.x -= 30;

					BeginDisabled(state.CanFill == 0);
						if (GUI.Button(rectC, new GUIContent("fill", "Copy the current value from the Target into this component?"), EditorStyles.miniButton) == true)
						{
							state.Fill();
						}
					EndDisabled();
				}
			}

			var setTarget = sTarget.objectReferenceValue != null;
			var setAlias  = string.IsNullOrEmpty(sAlias.stringValue) == false;
			var setCount  = (setTarget ? 1 : 0) + (setAlias ? 1 : 0);

			if (setCount == 0 || setCount == 2)
			{
				EditorGUI.LabelField(rect, new GUIContent("Target : Alias", "Target = The component or object that will be modified by this transition\n\nAlias = If this transition is on a prefab, this allows you to give a name to the component or object that will be modified by this transition."));
			}
			else if (setTarget == true)
			{
				EditorGUI.LabelField(rect, new GUIContent("Target", "The component or object that will be modified by this transition"));
			}
			else
			{
				EditorGUI.LabelField(rect, new GUIContent("Alias", "If this transition is on a prefab, this allows you to give a name to the component or object that will be modified by this transition."));
			}

			BeginError(setCount == 0 || setCount == 2);
				if (setCount == 0 || setCount == 2)
				{
					EditorGUI.PropertyField(rectL, sTarget, GUIContent.none);
					EditorGUI.PropertyField(rectR, sAlias, GUIContent.none);
				}
				else if (setTarget == true)
				{
					EditorGUI.PropertyField(rectM, sTarget, GUIContent.none);
				}
				else
				{
					EditorGUI.PropertyField(rectM, sAlias, GUIContent.none);
				}
			EndError();

			if (setAlias == true)
			{
				var methodST      = (LeanMethodWithStateAndTarget)tgt;
				var expectedType  = methodST.GetTargetType();
				var expectedAlias = sAlias.stringValue;

				foreach (var method in tgt.GetComponents<LeanMethodWithStateAndTarget>())
				{
					var methodTargetType = method.GetTargetType();

					if (methodTargetType != expectedType)
					{
						if (method.Alias == expectedAlias)
						{
							if (methodTargetType.IsSubclassOf(typeof(Component)) == true && expectedType.IsSubclassOf(typeof(Component)) == true)
							{
								continue;
							}

							Error("This alias is used by multiple transitions. This only works if they all transition the same type (e.g. Transform.localPosition & Transform.localScale both transition Transform).");

							break;
						}
					}
				}
			}
		}

		private void DrawAutoFill(TARGET tgt, System.Reflection.FieldInfo data)
		{
			var state = data.GetValue(tgt) as LeanState;

			if (state != null && state.CanFill > -1)
			{
				var rect = Reserve();

				BeginDisabled(state.CanFill == 0);
					if (GUI.Button(rect, new GUIContent("auto fill", "Copy the current value from the scene into this component?"), EditorStyles.miniButton) == true)
					{
						state.Fill();
					}
				EndDisabled();
			}
		}

		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var dataProperty = serializedObject.FindProperty("Data");

			if (dataProperty != null)
			{
				var sTarget = serializedObject.FindProperty("Data.Target");
				var sAlias  = serializedObject.FindProperty("Alias");
				var data    = tgt.GetType().GetField("Data");

				Draw("Data.Duration", "The transition will complete after this many seconds.");

				if (sTarget != null && sAlias != null)
				{
					DrawTargetAlias(sTarget, sAlias, data);
				}

				dataProperty.NextVisible(true);

				while (true)
				{
					if (dataProperty.name != "Duration" && dataProperty.name != "Target" && dataProperty.name != "TargetAlias")
					{
						if (dataProperty.propertyType == SerializedPropertyType.Quaternion)
						{
							EditorGUI.BeginChangeCheck();
							EditorGUI.showMixedValue = dataProperty.hasMultipleDifferentValues;

							var eulerAngles = EditorGUILayout.Vector3Field(new GUIContent(dataProperty.displayName, dataProperty.tooltip), dataProperty.quaternionValue.eulerAngles);

							EditorGUI.showMixedValue = false;

							if (EditorGUI.EndChangeCheck() == true)
							{
								dataProperty.quaternionValue = Quaternion.Euler(eulerAngles);
							}
						}
						else
						{
							EditorGUILayout.PropertyField(dataProperty);
						}
					}

					if (dataProperty.NextVisible(false) == false)
					{
						break;
					}
				}

				if (sTarget == null && data != null)
				{
					DrawAutoFill(tgt, data);
				}
			}
			else
			{
				var property = serializedObject.GetIterator(); property.NextVisible(true);

				while (property.NextVisible(false) == true)
				{
					if (property.name == "Target")
					{
						BeginError(property.objectReferenceValue == null);
							EditorGUILayout.PropertyField(property);
						EndError();
					}
					else
					{
						EditorGUILayout.PropertyField(property);
					}
				}
			}

			if (tgt is Method.LeanJoin || tgt is Method.LeanJoinDelay || tgt is Method.LeanJoinInsert)
			{
				if (IsLast == true)
				{
					Error("This is a JOIN transition, but you have no transition after this component.");
				}
			}
		}
	}
}
#endif