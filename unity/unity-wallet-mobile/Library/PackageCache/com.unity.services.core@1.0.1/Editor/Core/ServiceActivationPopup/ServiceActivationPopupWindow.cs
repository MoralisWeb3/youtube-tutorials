#if ENABLE_EDITOR_GAME_SERVICES
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class ServiceActivationPopupWindow : EditorWindow
    {
        static readonly Vector2 k_PopupSize = new Vector2(600, 400);
        const string k_WindowTitle = "Service Activation";

        ServiceActivationPopupVisual m_PopupVisual;

        ///<remarks>required to recover from domain reloads</remarks>
        [SerializeField] List<string> m_ServiceTypeNames;

        public static void CreateAndShowPopup(IEnumerable<IEditorGameService> services)
        {
            var popupWindow = GetWindow<ServiceActivationPopupWindow>(true, k_WindowTitle);
            popupWindow.Initialize(services);
        }

        void Initialize(IEnumerable<IEditorGameService> services)
        {
            m_PopupVisual?.Dispose();

            m_ServiceTypeNames = GetServiceTypeNames(services);

            m_PopupVisual = new ServiceActivationPopupVisual();
            m_PopupVisual.Init(rootVisualElement, services);
            m_PopupVisual.Done += DisposeAndClose;

            maxSize = minSize = k_PopupSize;
        }

        static List<string> GetServiceTypeNames(IEnumerable<IEditorGameService> services)
        {
            var output = new List<string>();
            foreach (var editorGameService in services)
            {
                output.Add(editorGameService.GetType().FullName);
            }

            return output;
        }

        void DisposeAndClose()
        {
            m_PopupVisual?.Dispose();
            Close();
        }

        void Update()
        {
            if (RequiresInitialization())
            {
                Initialize(GetEditorGameServicesFromNames(m_ServiceTypeNames));
            }
        }

        static IEnumerable<IEditorGameService> GetEditorGameServicesFromNames(IEnumerable<string> editorGameServiceTypeNames)
        {
            var output = new HashSet<IEditorGameService>();

            foreach (var kvp in EditorGameServiceRegistry.Instance.Services)
            {
                if (editorGameServiceTypeNames.Contains(kvp.Value.GetType().FullName))
                {
                    output.Add(kvp.Value);
                }
            }

            return output;
        }

        bool RequiresInitialization()
        {
            return m_PopupVisual == null;
        }
    }
}
#endif
