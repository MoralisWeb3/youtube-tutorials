using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Lean.Common;

namespace Lean.Transition
{
	/// <summary>This class allows you to reference Transforms whose GameObjects contain transition components.
	/// If these transition components define TargetAlias names, then this class will also manage them in the inspector.</summary>
	[System.Serializable]
	public class LeanPlayer
	{
		[System.Serializable]
		public class Alias
		{
			public string Key;
			public Object Obj;

			[System.NonSerialized]
			public System.Type Type;
		}

		[System.Serializable]
		public class Entry
		{
			public Transform Root { set { root = value; } get { return root; } } [SerializeField] private Transform root;

			public float Speed { set { speed = value; } get { return speed; } } [SerializeField] private float speed = -1.0f;

			public List<Alias> Aliases { get { if (aliases == null) aliases = new List<Alias>(); return aliases; } } [SerializeField] private List<Alias> aliases;

			public void AddAlias(string key, Object obj)
			{
				foreach (var alias in Aliases)
				{
					if (alias.Key == key)
					{
						alias.Key = key;

						return;
					}
				}

				aliases.Add(new Alias() { Key = key, Obj = obj });
			}
		}

		// Legacy
		[SerializeField]
		private float speed = -1.0f;

		// Legacy
		[SerializeField]
		private List<Transform> roots = null;

		// Legacy
		[SerializeField]
		private List<string> aliases = null;

		// Legacy
		[SerializeField]
		private List<Object> targets = null;

		[SerializeField]
		private List<Entry> entries;

		public float Speed
		{
			set
			{
				speed = value;
			}

			get
			{
				return speed;
			}
		}

		private static Dictionary<string, Alias> tempAliases = new Dictionary<string, Alias>();

		public bool IsUsed
		{
			get
			{
				if (entries != null && entries.Count > 0)
				{
					foreach (var entry in entries)
					{
						if (entry.Root != null)
						{
							return true;
						}
					}
				}

				return false;
			}
		}

		/// <summary>This stores a list of all <b>Transform</b>s containing transitions that will be played, and their settings.</summary>
		public List<Entry> Entries
		{
			get
			{
				if (entries == null)
				{
					entries = new List<Entry>();
				}

				return entries;
			}
		}

		public void Validate(bool validateEntries)
		{
			if (roots != null && roots.Count > 0)
			{
				Entries.Clear();

				foreach (var root in roots)
				{
					if (root != null)
					{
						entries.Add(new Entry() { Root = root, Speed = speed > 0.0f ? speed : -1.0f });
					}
				}

				roots.Clear();
			}

			if (Entries.Count == 0)
			{
				entries.Add(new Entry());
			}

			if (aliases != null && aliases.Count > 0 && targets != null && targets.Count > 0)
			{
				var min = System.Math.Min(aliases.Count, targets.Count);

				for (var i = 0; i < min; i++)
				{
					foreach (var entry in Entries)
					{
						entry.AddAlias(aliases[i], targets[i]);
					}
				}

				aliases.Clear();
				targets.Clear();
			}

			if (validateEntries == true)
			{
				foreach (var entry in Entries)
				{
					if (entry != null)
					{
						var pairs = LeanTransition.FindAllAliasTypePairs(entry.Root);

						// Move entry.Aliases into dictionary
						foreach (var alias in entry.Aliases)
						{
							tempAliases.Add(alias.Key, alias);
						}

						entry.Aliases.Clear();

						// Rebuild entry.Aliases from dictionaries
						foreach (var pair in pairs)
						{
							Alias alias;

							// Use existing by set type again (it's non-serialized, so it must be set again)
							if (tempAliases.TryGetValue(pair.Key, out alias) == true)
							{
								alias.Type = pair.Value;

								entry.Aliases.Add(alias);
							}
							// Use new
							else
							{
								entry.Aliases.Add(new Alias() { Key = pair.Key, Type = pair.Value });
							}
						}

						// Discard remaining
						tempAliases.Clear();
					}
				}
			}
		}

		/// <summary>This method will begin all transition entries.</summary>
		public void Begin()
		{
			Validate(false);

			if (entries != null)
			{
				LeanTransition.CurrentAliases.Clear();

				foreach (var entry in entries)
				{
					foreach (var alias in entry.Aliases)
					{
						LeanTransition.AddAlias(alias.Key, alias.Obj);
					}

					LeanTransition.BeginAllTransitions(entry.Root, entry.Speed);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Transition.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(LeanPlayer))]
	public class LeanPlayerDrawer : PropertyDrawer
	{
		private static Color  color;
		private static float  height;
		private static float  heightStep;
		private static string title;
		private static string tooltip;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			height     = base.GetPropertyHeight(property, label);
			heightStep = height + 2.0f;

			ValidateAndUpdate(property.serializedObject, property);

			var count    = 0;
			var sEntries = property.FindPropertyRelative("entries");

			for (var i = 0; i < sEntries.arraySize; i++)
			{
				var sEntry   = sEntries.GetArrayElementAtIndex(i);
				var sSpeed   = sEntry.FindPropertyRelative("speed");
				var sAliases = sEntry.FindPropertyRelative("aliases");

				if (sSpeed.floatValue >= 0.0f)
				{
					count++;
				}

				count += sAliases.arraySize;

				count++;
			}

			return height + heightStep * System.Math.Max(0, count - 1);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var player   = LeanHelper.GetObjectFromSerializedProperty<LeanPlayer>(property.serializedObject.targetObject, property);
			var sObject  = property.serializedObject;
			var sEntries = property.FindPropertyRelative("entries");

			DrawPlay(position, sObject);

			color   = GUI.color; position.height = height;
			title   = label.text;
			tooltip = label.tooltip;

			for (var i = 0; i < sEntries.arraySize; i++)
			{
				var entry    = player.Entries[i];
				var sEntry   = sEntries.GetArrayElementAtIndex(i);
				var sRoot    = sEntry.FindPropertyRelative("root");
				var sSpeed   = sEntry.FindPropertyRelative("speed");
				var sAliases = sEntry.FindPropertyRelative("aliases");
				var rectL    = position; rectL.width = EditorGUIUtility.labelWidth - 16.0f;
				var rectR    = position; rectR.xMin += EditorGUIUtility.labelWidth;

				if (Event.current.isMouse == true && Event.current.button == 1 && rectL.Contains(Event.current.mousePosition) == true)
				{
					Event.current.Use();
					ShowMenu(sObject, sEntries, i, sRoot, sSpeed, title);
				}
			
				EditorGUI.PropertyField(position, sRoot, new GUIContent(title, tooltip)); position.y += heightStep;
				EditorGUI.indentLevel++;
					if (sSpeed.floatValue >= 0.0f)
					{
						EditorGUI.PropertyField(position, sSpeed); position.y += heightStep;
					}

					for (var j = 0; j < sAliases.arraySize; j++)
					{
						var alias  = entry.Aliases[j];
						var sAlias = sAliases.GetArrayElementAtIndex(j);
						var sKey   = sAlias.FindPropertyRelative("Key");
						var sObj   = sAlias.FindPropertyRelative("Obj");

						EditorGUI.BeginChangeCheck();

						EditorGUI.showMixedValue = sObj.hasMultipleDifferentValues;
							var obj = EditorGUI.ObjectField(position, new GUIContent(sKey.stringValue, ""), alias.Obj, alias.Type, true); position.y += heightStep;
						EditorGUI.showMixedValue = false;

						if (EditorGUI.EndChangeCheck() == true)
						{
							sObj.objectReferenceValue = obj;
						}
					}
				EditorGUI.indentLevel--;
			}

			GUI.color = color;
		}

		private void DrawPlay(Rect position, SerializedObject sObject)
		{
			var rectL = position; rectL.xMin += EditorGUIUtility.labelWidth - 15.0f; rectL.width = 14.0f; rectL.yMin += 1.0f; rectL.yMax -= 1.0f;
			var rectR = rectL; rectR.y -= 1.0f;

			if (GUI.Button(rectL, new GUIContent("", "Clicking this will play the transitions now.")) == true)
			{
				foreach (var targetObject in sObject.targetObjects)
				{
					((LeanPlayer)fieldInfo.GetValue(targetObject)).Begin();
				}
			}

			GUI.Label(rectR, "▶", EditorStyles.centeredGreyMiniLabel);
		}

		private void ShowMenu(SerializedObject sObject, SerializedProperty sEntries, int index, SerializedProperty sRoot, SerializedProperty sSpeed, string title)
		{
			var menu          = new GenericMenu();
			var methodPrefabs = AssetDatabase.FindAssets("t:GameObject").
				Select((guid) => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid))).
				Where((prefab) => prefab.GetComponent<LeanMethod>() != null);
			var targetComponent = sObject.targetObject as Component;

			if (targetComponent != null)
			{
				if (sRoot.objectReferenceValue == null)
				{
					menu.AddItem(new GUIContent("Create"), false, () =>
						{
							var root = new GameObject("[" + title + "]").transform;

							root.SetParent(targetComponent.transform, false);

							sRoot.objectReferenceValue = root;
							sObject.ApplyModifiedProperties();

							Selection.activeTransform = root;
						});
				}
				else
				{
					menu.AddItem(new GUIContent("Add"), false, () =>
					{
						sEntries.InsertArrayElementAtIndex(index + 1);
						var sEntry = sEntries.GetArrayElementAtIndex(index + 1);
						sEntry.FindPropertyRelative("root").objectReferenceValue = null;
						sEntry.FindPropertyRelative("speed").floatValue = -1.0f;
						sEntry.FindPropertyRelative("aliases").arraySize = 0;
						sObject.ApplyModifiedProperties();
					});
				}
			}

			if (sSpeed.floatValue <= -1.0f)
			{
				menu.AddItem(new GUIContent("Speed"), false, () =>
					{
						sSpeed.floatValue = 1.0f;
						sObject.ApplyModifiedProperties();
					});
			}
			else
			{
				menu.AddItem(new GUIContent("Speed"), true, () =>
					{
						sSpeed.floatValue = -1.0f;
						sObject.ApplyModifiedProperties();
					});
			}

			menu.AddSeparator("");

			foreach (var methodPrefab in methodPrefabs)
			{
				var root = methodPrefab.transform;

				menu.AddItem(new GUIContent("Prefab/" + methodPrefab.name), false, () =>
					{
						sRoot.objectReferenceValue = root;
						sObject.ApplyModifiedProperties();
					});
			}

			menu.AddSeparator("");

			menu.AddItem(new GUIContent("Remove"), false, () =>
				{
					sEntries.DeleteArrayElementAtIndex(index);
					sObject.ApplyModifiedProperties();
				});

			menu.ShowAsContext();
		}

		private void ValidateAndUpdate(SerializedObject sObject, SerializedProperty sPlayer)
		{
			sObject.ApplyModifiedProperties();

			foreach (var targetObject in sObject.targetObjects)
			{
				var player = LeanHelper.GetObjectFromSerializedProperty<LeanPlayer>(targetObject, sPlayer);

				if (player != null)
				{
					player.Validate(true);
				}
			}

			sObject.Update();
		}
	}
}
#endif