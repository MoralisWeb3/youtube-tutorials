using UnityEngine;
using System.Collections.Generic;
using Lean.Common;

namespace Lean.Gui
{
	/// <summary>This component allows you to automatically close the top-most LeanWindow when you press the specified key.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanWindowCloser")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Window Closer")]
	public class LeanWindowCloser : MonoBehaviour
	{
		/// <summary>This stores all active and enabled LeanWindowCloser instances.</summary>
		public static List<LeanWindowCloser> Instances = new List<LeanWindowCloser>();

		/// <summary>This allows you to set the key that must be pressed to close the window on top.</summary>
		public KeyCode CloseKey { set { closeKey = value; } get { return closeKey; } } [SerializeField] private KeyCode closeKey = KeyCode.Escape;

		/// <summary>If every window is closed and you press the close key, this window will be opened. This can be used to open an options menu.</summary>
		public LeanWindow EmptyWindow { set { emptyWindow = value; } get { return emptyWindow; } } [SerializeField] private LeanWindow emptyWindow;

		/// <summary>This stores a list of all opened windows, in order of opening, so they can be closed in reverse order.</summary>
		public List<LeanWindow> WindowOrder { get { if (windowOrder == null) windowOrder = new List<LeanWindow>(); return windowOrder; } } [SerializeField] private List<LeanWindow> windowOrder;

		public static void Register(LeanWindow window)
		{
			if (Instances.Count > 0 && window != null)
			{
				Instances[0].RegisterNow(window);
			}
		}

		/// <summary>This allows you to close all open LeanWindows.</summary>
		[ContextMenu("Close All")]
		public void CloseAll()
		{
			for (var i = WindowOrder.Count - 1; i >= 0; i--) // NOTE: Property
			{
				var window = windowOrder[i];

				if (window != null && window.On == true)
				{
					window.TurnOff();
				}
			}

			windowOrder.Clear();
		}

		/// <summary>This allows you to close the top most LeanWindow.</summary>
		[ContextMenu("Close Top Most")]
		public void CloseTopMost()
		{
			for (var i = WindowOrder.Count - 1; i >= 0; i--) // NOTE: Property
			{
				var window = windowOrder[i];

				windowOrder.RemoveAt(i);

				if (window != null && window.On == true)
				{
					window.TurnOff();

					return;
				}
			}

			if (emptyWindow != null)
			{
				emptyWindow.TurnOn();
			}
		}

		protected virtual void OnEnable()
		{
			Instances.Add(this);
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);
		}

		protected virtual void Update()
		{
			if (this == Instances[0])
			{
				if (LeanInput.GetDown(CloseKey) == true)
				{
					CloseTopMost();
				}
			}
		}

		private void RegisterNow(LeanWindow window)
		{
			WindowOrder.Remove(window); // NOTE: Property

			windowOrder.Add(window);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
	using TARGET = LeanWindowCloser;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanWindowCloser_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("closeKey", "This allows you to set the key that must be pressed to close the window on top.");
			Draw("emptyWindow", "If every window is closed and you press the close key, this window will be opened. This can be used to open an options menu.");

			Separator();

			BeginDisabled(true);
				Draw("windowOrder", "This stores a list of all opened windows, in order of opening, so they can be closed in reverse order.");
			EndDisabled();
		}
	}
}
#endif