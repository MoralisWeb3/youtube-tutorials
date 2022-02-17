using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component allows you to spawn a prefab at the specified world point.
	/// NOTE: For this component to work you must manually call the <b>Spawn</b> method from somewhere.</summary>
	[HelpURL(LeanHelper.HelpUrlPrefix + "LeanSpawn")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Spawn")]
	public class LeanSpawn : MonoBehaviour
	{
		public enum SourceType
		{
			ThisTransform,
			Prefab
		}

		/// <summary>The prefab that this component can spawn.</summary>
		public Transform Prefab { set { prefab = value; } get { return prefab; } } [FSA("Prefab")] [SerializeField] private Transform prefab;

		/// <summary>If you call <b>Spawn()</b>, where should the position come from?</summary>
		public SourceType DefaultPosition { set { defaultPosition = value; } get { return defaultPosition; } } [FSA("DefaultPosition")] [SerializeField] private SourceType defaultPosition;

		/// <summary>If you call <b>Spawn()</b>, where should the rotation come from?</summary>
		public SourceType DefaultRotation { set { defaultRotation = value; } get { return defaultRotation; } } [FSA("DefaultRotation")] [SerializeField] private SourceType defaultRotation;

		/// <summary>This will spawn <b>Prefab</b> at the current <b>Transform.position</b>.</summary>
		public void Spawn()
		{
			if (prefab != null)
			{
				var position = defaultPosition == SourceType.Prefab ? prefab.position : transform.position;
				var rotation = defaultRotation == SourceType.Prefab ? prefab.rotation : transform.rotation;
				var clone    = Instantiate(prefab, position, rotation);

				clone.gameObject.SetActive(true);
			}
		}

		/// <summary>This will spawn <b>Prefab</b> at the specified position in world space.</summary>
		public void Spawn(Vector3 position)
		{
			if (prefab != null)
			{
				var rotation = defaultRotation == SourceType.Prefab ? prefab.rotation : transform.rotation;
				var clone    = Instantiate(prefab, position, rotation);

				clone.gameObject.SetActive(true);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanSpawn;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSpawn_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Prefab == null));
				Draw("prefab", "The prefab that this component can spawn.");
			EndError();
			Draw("defaultPosition", "If you call Spawn(), where should the position come from?");
			Draw("defaultRotation", "If you call Spawn(), where should the rotation come from?");
		}
	}
}
#endif