using UnityEngine;

namespace Lean.Common.Examples
{
	/// <summary>This component is used by all the demo scenes to perform common tasks. Including modifying the current scene to make it look consistent between different rendering pipelines.</summary>
	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class LeanDemo : MonoBehaviour
	{
		/// <summary>If you enable this setting and your project is running with the new InputSystem then the <b>EventSystem's InputModule</b> component will be upgraded.</summary>
		public bool UpgradeInputModule { set { upgradeInputModule = value; } get { return upgradeInputModule; } } [SerializeField] private bool upgradeInputModule = true;

		/// <summary>If you set this then the specified skybox will be placed on top of the main camera.</summary>
		public Transform SkyboxRoot { set { skyboxRoot = value; } get { return skyboxRoot; } } [SerializeField] private Transform skyboxRoot;

		protected virtual void OnEnable()
		{
			if (upgradeInputModule == true)
			{
				TryUpgradeEventSystem();
			}
		}

		protected virtual void LateUpdate()
		{
			if (skyboxRoot != null)
			{
				var camera = Camera.main;

				if (camera != null)
				{
					skyboxRoot.position = camera.transform.position;
				}
			}
		}

		private void TryUpgradeEventSystem()
		{
#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
			var module = FindObjectOfType<UnityEngine.EventSystems.StandaloneInputModule>();

			if (module != null)
			{
				module.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

				DestroyImmediate(module);
			}
#endif
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Examples.Editor
{
	using UnityEditor;
	using TARGET = LeanDemo;

	[CustomEditor(typeof(TARGET))]
	public class LeanDemo_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("upgradeInputModule", "If you enable this setting and your project is running with the new InputSystem then the <b>EventSystem's InputModule</b> component will be upgraded.");
			Draw("skyboxRoot", "If you set this then the specified skybox will be placed on top of the main camera.");
		}
	}
}
#endif