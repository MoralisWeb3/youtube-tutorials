using UnityEngine;
using System.Collections.Generic;
using Lean.Common;

namespace Lean.Transition
{
	/// <summary>This component updates all active transition methods, both in game, and in the editor.</summary>
	[ExecuteInEditMode]
	[HelpURL(HelpUrlPrefix + "LeanTransition")]
	[AddComponentMenu(ComponentMenuPrefix + "Lean Transition")]
	public class LeanTransition : MonoBehaviour
	{
		public const string ComponentMenuPrefix = "Lean/Transition/";

		public const string MethodsMenuPrefix = "Lean/Transition/Methods/";

		public const string MethodsMenuSuffix = " Transition ";

		public const string HelpUrlPrefix = "https://carloswilkes.com/Documentation/LeanTransition#";

		/// <summary>This allows you to set where in the game loop animations are updated when timing = LeanTime.Default.</summary>
		public LeanTiming DefaultTiming { set { defaultTiming = value; } get { return defaultTiming; } } [SerializeField] [UnityEngine.Serialization.FormerlySerializedAs("Timing")] private LeanTiming defaultTiming = LeanTiming.UnscaledUpdate;

		/// <summary>This stores a list of all active and enabled <b>LeanTransition</b> instances in the scene.</summary>
		public static List<LeanTransition> Instances = new List<LeanTransition>();

		public static event System.Action<LeanState> OnRegistered;

		public static event System.Action<LeanState> OnFinished;

		private static List<LeanState> unscaledUpdateStates = new List<LeanState>();

		private static List<LeanState> unscaledLateUpdateStates = new List<LeanState>();

		private static List<LeanState> unscaledFixedUpdateStates = new List<LeanState>();

		private static List<LeanState> updateStates = new List<LeanState>();

		private static List<LeanState> lateUpdateStates = new List<LeanState>();

		private static List<LeanState> fixedUpdateStates = new List<LeanState>();

		private static List<LeanMethod> tempBaseMethods = new List<LeanMethod>();

		private static List<LeanMethod> baseMethodStack = new List<LeanMethod>();

		private static Dictionary<string, System.Type> aliasTypePairs = new Dictionary<string, System.Type>();

		private static LeanState previousState;

		private static LeanState currentQueue;

		private static LeanState defaultQueue;

		private static LeanTiming currentTiming;

		private static float currentSpeed = 1.0f;

		private static bool started;

		private static Dictionary<string, Object> currentAliases = new Dictionary<string, Object>();

		/// <summary>This property gives you the first <b>DefaultTiming</b> instance value.</summary>
		public static LeanTiming CurrentDefaultTiming
		{
			get
			{
				if (Instances.Count > 0)
				{
					return Instances[0].defaultTiming;
				}

				return default(LeanTiming);
			}
		}

		/// <summary>This tells you how many transitions are currently running.</summary>
		public static int Count
		{
			get
			{
				return unscaledUpdateStates.Count + unscaledLateUpdateStates.Count + unscaledFixedUpdateStates.Count + updateStates.Count + lateUpdateStates.Count + fixedUpdateStates.Count;
			}
		}

		/// <summary>After a transition state is registered, it will be stored here. This allows you to copy it out for later use.</summary>
		public static LeanState PreviousState
		{
			get
			{
				return previousState;
			}
		}

		/// <summary>If you want the next registered transition state to automatically begin after an existing transition state, then specify it here.</summary>
		public static LeanState CurrentQueue
		{
			set
			{
				currentQueue = value;
			}
		}

		/// <summary>This allows you to change where in the game loop all future transitions in the current animation will be updated.</summary>
		public static LeanTiming CurrentTiming
		{
			set
			{
				currentTiming = value;
			}
		}

		/// <summary>This allows you to change the transition speed multiplier of all future transitions in the current animation.</summary>
		public static float CurrentSpeed
		{
			set
			{
				currentSpeed = value;
			}

			get
			{
				return currentSpeed;
			}
		}

		/// <summary>This allows you to change the alias name to UnityEngine.Object association of all future transitions in the current animation.</summary>
		public static Dictionary<string, Object> CurrentAliases
		{
			get
			{
				return currentAliases;
			}
		}

		public static void AddAlias(string key, Object obj)
		{
			currentAliases.Remove(key);
			currentAliases.Add(key, obj);
		}

		/// <summary>This method will return the specified timing, unless it's set to <b>Default</b>, then it will return <b>UnscaledTime</b>.</summary>
		public static LeanTiming GetTiming(LeanTiming current = LeanTiming.Default)
		{
			if (current == LeanTiming.Default)
			{
				current = LeanTiming.UnscaledUpdate;
			}

			return current;
		}

		/// <summary>This method works like <b>GetTiming</b>, but it won't return any unscaled times.</summary>
		public static LeanTiming GetTimingAbs(LeanTiming current)
		{
			return (LeanTiming)System.Math.Abs((int)current);
		}

		/// <summary>If you failed to submit a previous transition then this will throw an error, and then submit them.</summary>
		public static void RequireSubmitted()
		{
			if (currentQueue != null)
			{
				Debug.LogError("You forgot to submit the last transition! " + currentQueue.GetType() + " - " + currentQueue.GetTarget());

				Submit();
			}

			if (baseMethodStack.Count > 0)
			{
				Debug.LogError("Failed to submit all methods.");

				Submit();
			}
		}

		/// <summary>This will reset any previously called <b>CurrentTiming</b> calls.</summary>
		public static void ResetTiming()
		{
			currentTiming = CurrentDefaultTiming;
		}

		/// <summary>This will reset any previously called <b>CurrentQueue</b> calls.</summary>
		public static void ResetQueue()
		{
			currentQueue = null;
		}

		/// <summary>This will reset any previously called <b>CurrentSpeed</b> calls.</summary>
		public static void ResetSpeed()
		{
			currentSpeed = 1.0f;
		}

		/// <summary>This will reset the <b>CurrentTiming</b>, <b>CurrentQueue</b>, and <b>CurrentSpeed</b> values.</summary>
		public static void ResetState()
		{
			defaultQueue = null;

			ResetTiming();

			ResetQueue();

			ResetSpeed();
		}

		/// <summary>This will submit any previously registered transitions, and reset the timing.</summary>
		public static void Submit()
		{
			ResetState();

			baseMethodStack.Clear();
		}

		/// <summary>This will begin all transitions on the specified GameObject, all its children, and then submit them.
		/// If you failed to submit a previous transition then this will also throw an error.</summary>
		public static void BeginAllTransitions(Transform root, float speed = 1.0f)
		{
			ResetState();

			if (root != null)
			{
				RequireSubmitted();

				InsertTransitions(root, speed);

				Submit();
			}
		}

		/// <summary>This will begin all transitions on the specified GameObject, and all its children.</summary>
		public static void InsertTransitions(GameObject root, float speed = 1.0f, LeanState parentHead = null)
		{
			if (root != null)
			{
				InsertTransitions(root.transform, speed);
			}
		}

		/// <summary>This will begin all transitions on the specified Transform, and all its children.</summary>
		public static void InsertTransitions(Transform root, float speed = 1.0f, LeanState parentHead = null)
		{
			if (root != null)
			{
				var spd = currentSpeed;
				var min = baseMethodStack.Count; root.GetComponents(tempBaseMethods); baseMethodStack.AddRange(tempBaseMethods); tempBaseMethods.Clear();
				var max = baseMethodStack.Count;

				currentSpeed *= speed;

				if (parentHead != null)
				{
					previousState = parentHead;
					currentQueue  = parentHead;
				}

				defaultQueue = parentHead;

				for (var i = min; i < max; i++)
				{
					baseMethodStack[i].Register();
				}

				baseMethodStack.RemoveRange(min, max - min);

				var childParentHead = previousState;

				for (var i = 0; i < root.childCount; i++)
				{
					InsertTransitions(root.GetChild(i), 1.0f, childParentHead);
				}

				currentSpeed = spd;
			}
		}

		/// <summary>This method returns all TargetAliases on all transitions on the specified Transform.</summary>
		public static Dictionary<string,System.Type> FindAllAliasTypePairs(Transform root)
		{
			aliasTypePairs.Clear();

			AddAliasTypePairs(root);

			return aliasTypePairs;
		}

		private static void AddAliasTypePairs(Transform root)
		{
			if (root != null)
			{
				root.GetComponents(tempBaseMethods);

				for (var i = 0; i < tempBaseMethods.Count; i++)
				{
					var baseMethod = tempBaseMethods[i] as LeanMethodWithStateAndTarget;

					if (baseMethod != null)
					{
						var targetType = baseMethod.GetTargetType();
						var alias      = baseMethod.Alias;

						if (string.IsNullOrEmpty(alias) == false)
						{
							var existingType = default(System.Type);

							// Exists?
							if (aliasTypePairs.TryGetValue(alias, out existingType) == true)
							{
								// Clashing types?
								if (existingType != targetType)
								{
									// If both are components then the clash can be resolved by using GameObject
									if (targetType.IsSubclassOf(typeof(Component)) == true)
									{
										// If it's already a GameObject, skip
										if (existingType == typeof(GameObject))
										{
											continue;
										}
										// Change existing type to GameObject?
										else if (existingType.IsSubclassOf(typeof(Component)) == true)
										{
											aliasTypePairs[alias] = typeof(GameObject);

											continue;
										}
									}

									// If the clash cannot be resolved, throw an error
									Debug.LogError("The (" + root.name + ") GameObject contains multiple transitions that define a target alias of (" + alias + "), but these transitions use different types (" + existingType + ") + (" + targetType + "). You must give them different aliases.", root);
								}
							}
							// Add new?
							else
							{
								aliasTypePairs.Add(alias, targetType);
							}
						}
					}
				}
			}
		}

		public static T SpawnWithTarget<T, U>(Stack<T> pool, U target)
			where T : LeanStateWithTarget<U>, new()
			where U : Object
		{
			var data = Spawn(pool);

			data.Target = target;

			return data;
		}

		public static T Spawn<T>(Stack<T> pool)
			where T : LeanState, new()
		{
			// Make sure the transition manager exists
			if (Instances.Count == 0)
			{
				new GameObject("LeanTransition").AddComponent<LeanTransition>();
			}

			// Setup initial data
			var state = pool.Count > 0 ? pool.Pop() : new T();

			state.Age  = -1.0f;
			state.Ignore = false;

			state.Prev.Clear();
			state.Next.Clear();

			// Join to previous transition?
			if (currentQueue != null)
			{
				state.BeginAfter(currentQueue);

				currentQueue = defaultQueue;
			}

			// Make this the new head
			previousState = state;

			return state;
		}

		public static LeanState Register(LeanState state, float duration)
		{
			state.Duration = duration;

			// Execute immediately?
			if (duration == 0.0f && state.Prev.Count == 0)
			{
				FinishState(state);

				if (previousState == state)
				{
					previousState = null;
				}

				return null;
			}
			// Register for later execution?
			else
			{
				if (currentSpeed > 0.0f)
				{
					state.Duration /= currentSpeed;
				}

				// Convert currentTiming if it's set to default, then register the state in the correct list
				var finalUpdate = GetTiming(currentTiming);

				switch (finalUpdate)
				{
					case LeanTiming.UnscaledFixedUpdate: unscaledFixedUpdateStates.Add(state); break;
					case LeanTiming.UnscaledLateUpdate:   unscaledLateUpdateStates.Add(state); break;
					case LeanTiming.UnscaledUpdate:           unscaledUpdateStates.Add(state); break;
					case LeanTiming.Update:                           updateStates.Add(state); break;
					case LeanTiming.LateUpdate:                   lateUpdateStates.Add(state); break;
					case LeanTiming.FixedUpdate:                 fixedUpdateStates.Add(state); break;
				}
			}

			if (OnRegistered != null)
			{
				OnRegistered(state);
			}

			return state;
		}

		protected virtual void OnEnable()
		{
			Instances.Add(this);

			ResetState();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.update -= HandleUpdateInEditor;
			UnityEditor.EditorApplication.update += HandleUpdateInEditor;
#endif
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);

			if (Instances.Count == 0)
			{
				unscaledFixedUpdateStates.Clear();
				 unscaledLateUpdateStates.Clear();
				     unscaledUpdateStates.Clear();
				             updateStates.Clear();
				         lateUpdateStates.Clear();
				        fixedUpdateStates.Clear();
			}
		}

#if UNITY_EDITOR
		private void HandleUpdateInEditor()
		{
			var delta = Time.deltaTime;

			if (Application.isPlaying == false)
			{
				UpdateAll(unscaledFixedUpdateStates, delta);
				UpdateAll( unscaledLateUpdateStates, delta);
				UpdateAll(     unscaledUpdateStates, delta);
				UpdateAll(             updateStates, delta);
				UpdateAll(         lateUpdateStates, delta);
				UpdateAll(        fixedUpdateStates, delta);
			}
		}
#endif

		protected virtual void Update()
		{
			if (this == Instances[0] && Application.isPlaying == true && started == true)
			{
				UpdateAll(unscaledUpdateStates, Time.unscaledDeltaTime);
				UpdateAll(        updateStates, Time.deltaTime        );
			}
		}

		protected virtual void LateUpdate()
		{
			if (this == Instances[0] && Application.isPlaying == true)
			{
				if (started == true)
				{
					UpdateAll(unscaledLateUpdateStates, Time.unscaledDeltaTime);
					UpdateAll(        lateUpdateStates, Time.deltaTime        );
				}
				else
				{
					started = true;
				}
			}
		}

		protected virtual void FixedUpdate()
		{
			if (this == Instances[0] && Application.isPlaying == true && started == true)
			{
				UpdateAll(unscaledFixedUpdateStates, Time.fixedUnscaledDeltaTime);
				UpdateAll(        fixedUpdateStates, Time.fixedDeltaTime        );
			}
		}

		/// <summary>This method will mark all transitions as Skip = true if they match the transition type and target object of the specified transition.</summary>
		private void RemoveConflictsBefore(List<LeanState> states, LeanState currentState, int currentIndex)
		{
			var currentConflict = currentState.Conflict;

			if (currentConflict != LeanState.ConflictType.None)
			{
				var currentType   = currentState.GetType();
				var currentTarget = currentState.GetTarget();

				for (var i = 0; i < currentIndex; i++)
				{
					var transition = states[i];

					if (transition.Ignore == false && transition.GetType() == currentType && transition.GetTarget() == currentTarget)
					{
						transition.Ignore = true;

						if (currentConflict == LeanState.ConflictType.Complete)
						{
							transition.Update(1.0f);
						}
					}
				}
			}
		}

		private void UpdateAll(List<LeanState> states, float delta)
		{
			ResetState();

			for (var i = states.Count - 1; i >= 0; i--)
			{
				var state = states[i];

				// If we have a negative duration, skip ahead of time?
				if (state.Prev.Count > 0 && state.Duration < 0.0f)
				{
					var skip = -state.Duration;

					for (var j = state.Prev.Count - 1; j >= 0; j--)
					{
						var prev = state.Prev[j];

						if (prev.Remaining <= skip)
						{
							prev.Next.Remove(state);

							state.Prev.RemoveAt(j);
						}
					}
				}

				// Only update if the previous transitions have finished
				if (state.Prev.Count == 0)
				{
					// If the transition age is negative, it hasn't started yet
					if (state.Age < 0.0f)
					{
						state.Age = 0.0f;

						// If this newly beginning transition is identical to an already registered one, mark the existing one as conflicting so it doesn't get updated
						RemoveConflictsBefore(states, state, i);

						// Begin the transition (this will often copy the current state of the variable that is being transitioned)
						state.Begin();
					}

					// Age
					state.Age += delta;

					// Finished?
					if (state.Age >= state.Duration)
					{
						FinishState(state);

						states.RemoveAt(i);
					}
					// Update
					else
					{
						if (state.Ignore == false)
						{
							state.Update(state.Age / state.Duration);
						}

#if UNITY_EDITOR
						DirtyTarget(state);
#endif
					}
				}
			}
		}

		private static void FinishState(LeanState state)
		{
			// Activate all chained states and clear them
			for (var j = state.Next.Count - 1; j >= 0; j--)
			{
				state.Next[j].Prev.Remove(state);
			}

			state.Next.Clear();

			// Make sure we call update one final time with a progress value of exactly 1.0
			if (state.Ignore == false)
			{
				state.Update(1.0f);
			}

			if (OnFinished != null)
			{
				OnFinished(state);
			}

#if UNITY_EDITOR
			DirtyTarget(state);
#endif

			state.Despawn();
		}

#if UNITY_EDITOR
		/// <summary>If a transition is being animated in the editor, then the target object may not update, so this method will automatically dirty it so that it will.</summary>
		private static void DirtyTarget(LeanState transition)
		{
			if (Application.isPlaying == false)
			{
				var targetField = transition.GetType().GetField("Target");

				if (targetField != null)
				{
					var target = targetField.GetValue(transition) as Object;

					if (target != null)
					{
						UnityEditor.EditorUtility.SetDirty(target);
					}
				}
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Lean.Transition.Editor
{
	using TARGET = LeanTransition;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanTransition_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("defaultTiming", "This allows you to set where in the game loop animations are updated when timing = LeanTime.Default.");

			Separator();

			BeginDisabled(true);
				UnityEditor.EditorGUILayout.IntField("Transition Count", LeanTransition.Count);
			EndDisabled();
		}

		[UnityEditor.MenuItem("GameObject/Lean/Transition", false, 1)]
		private static void CreateLocalization()
		{
			var gameObject = new GameObject(typeof(LeanTransition).Name);

			UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "Create LeanTransition");

			gameObject.AddComponent<LeanTransition>();

			UnityEditor.Selection.activeGameObject = gameObject;
		}
	}
}
#endif