#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class LoggedOutVisual : IActivationPopupVisual
    {
        public event Action Done;

        public void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer = null)
        {
            LoggedOutUiHelper.AddLoggedOutUI(parentVisual);

            CloudProjectSettingsEventManager.instance.projectStateChanged += OnProjectStateChanged;
        }

        void OnProjectStateChanged()
        {
            Done?.Invoke();
        }

        public void Dispose()
        {
            Done = null;
            CloudProjectSettingsEventManager.instance.projectStateChanged -= OnProjectStateChanged;
        }
    }
}
#endif
