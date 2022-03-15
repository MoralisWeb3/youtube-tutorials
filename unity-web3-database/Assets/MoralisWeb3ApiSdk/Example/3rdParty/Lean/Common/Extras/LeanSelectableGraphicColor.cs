using UnityEngine;
using UnityEngine.UI;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component allows you to change the color of the Graphic (e.g. Image) attached to the current GameObject when selected.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Graphic))]
	[HelpURL(LeanHelper.HelpUrlPrefix + "LeanSelectableGraphicColor")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Selectable Graphic Color")]
	public class LeanSelectableGraphicColor : LeanSelectableBehaviour
	{
		/// <summary>The default color given to the SpriteRenderer.</summary>
		public Color DefaultColor { set { defaultColor = value; UpdateColor(); } get { return defaultColor; } } [FSA("DefaultColor")] [SerializeField] private Color defaultColor = Color.white;

		/// <summary>The color given to the SpriteRenderer when selected.</summary>
		public Color SelectedColor { set { selectedColor = value; UpdateColor(); } get { return selectedColor; } } [FSA("SelectedColor")] [SerializeField] private Color selectedColor = Color.green;

		[System.NonSerialized]
		private Graphic cachedGraphic;

		protected override void OnSelected()
		{
			UpdateColor();
		}

		protected override void OnDeselected()
		{
			UpdateColor();
		}

		public void UpdateColor()
		{
			if (cachedGraphic == null) cachedGraphic = GetComponent<Graphic>();

			var color = Selectable != null && Selectable.IsSelected == true ? selectedColor : defaultColor;

			cachedGraphic.color = color;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanSelectableGraphicColor;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectableGraphicColor_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var updateColor = false;

			Draw("defaultColor", ref updateColor, "The default color given to the SpriteRenderer.");
			Draw("selectedColor", ref updateColor, "The color given to the SpriteRenderer when selected.");

			if (updateColor == true)
			{
				Each(tgts, t => t.UpdateColor(), true);
			}
		}
	}
}
#endif