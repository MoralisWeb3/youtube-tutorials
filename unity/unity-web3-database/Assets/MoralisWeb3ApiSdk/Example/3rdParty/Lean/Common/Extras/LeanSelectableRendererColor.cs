using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component allows you to change the color of the Renderer (e.g. MeshRenderer) attached to the current GameObject when selected.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Renderer))]
	[HelpURL(LeanHelper.HelpUrlPrefix + "LeanSelectableRendererColor")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Selectable Renderer Color")]
	public class LeanSelectableRendererColor : LeanSelectableBehaviour
	{
		/// <summary>The default color given to the SpriteRenderer.</summary>
		public Color DefaultColor { set { defaultColor = value; UpdateColor(); } get { return defaultColor; } } [FSA("DefaultColor")] [SerializeField] private Color defaultColor = Color.white;

		/// <summary>The color given to the SpriteRenderer when selected.</summary>
		public Color SelectedColor { set { selectedColor = value; UpdateColor(); } get { return selectedColor; } } [FSA("SelectedColor")] [SerializeField] private Color selectedColor = Color.green;

		[System.NonSerialized]
		private Renderer cachedRenderer;

		[System.NonSerialized]
		private MaterialPropertyBlock properties;

		protected override void OnSelected()
		{
			UpdateColor();
		}

		protected override void OnDeselected()
		{
			UpdateColor();
		}

		protected override void Start()
		{
			base.Start();

			UpdateColor();
		}

		public void UpdateColor()
		{
			if (cachedRenderer == null) cachedRenderer = GetComponent<Renderer>();

			var color = Selectable != null && Selectable.IsSelected == true ? selectedColor : defaultColor;

			if (properties == null)
			{
				properties = new MaterialPropertyBlock();
			}

			cachedRenderer.GetPropertyBlock(properties);

			properties.SetColor("_Color", color);

			cachedRenderer.SetPropertyBlock(properties);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanSelectableRendererColor;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectableRendererColor_Editor : LeanEditor
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