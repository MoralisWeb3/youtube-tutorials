using UnityEngine;
using Lean.Common;

namespace Lean.Gui
{
	/// <summary>This component will automatically constrain the current <b>RectTransform</b> to its parent.</summary>
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanConstrainToParent")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Constrain To Parent")]
	public class LeanConstrainToParent : MonoBehaviour
	{
		/// <summary>Constrain horizontally?</summary>
		public bool Horizontal { set { horizontal = value; } get { return horizontal; } } [SerializeField] private bool horizontal = true;

		/// <summary>Constrain vertically?</summary>
		public bool Vertical { set { vertical = value; } get { return vertical; } } [SerializeField] private bool vertical = true;

		[System.NonSerialized]
		private RectTransform cachedParentRectTransform;

		[System.NonSerialized]
		private RectTransform cachedRectTransform;

		protected virtual void OnEnable()
		{
			cachedRectTransform = GetComponent<RectTransform>();
		}

		protected virtual void LateUpdate()
		{
			if (cachedParentRectTransform != cachedRectTransform.parent)
			{
				cachedParentRectTransform = cachedRectTransform.parent as RectTransform;
			}

			if (cachedParentRectTransform != null)
			{
				var anchoredPosition = cachedRectTransform.anchoredPosition;
				var rect             = cachedRectTransform.rect;
				var boundary         = cachedParentRectTransform.rect;

				if (horizontal == true)
				{
					boundary.xMin -= rect.xMin;
					boundary.xMax -= rect.xMax;

					anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, boundary.xMin, boundary.xMax);
				}

				if (vertical == true)
				{
					boundary.yMin -= rect.yMin;
					boundary.yMax -= rect.yMax;

					anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, boundary.yMin, boundary.yMax);
				}

				cachedRectTransform.anchoredPosition = anchoredPosition;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
	using TARGET = LeanConstrainToParent;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanConstrainToParent_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("horizontal", "Constrain horizontally?");
			Draw("vertical", "Constrain vertically?");
		}
	}
}
#endif