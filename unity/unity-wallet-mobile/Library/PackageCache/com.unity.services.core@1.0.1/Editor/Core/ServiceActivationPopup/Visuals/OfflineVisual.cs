#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    class OfflineVisual : IActivationPopupVisual
    {
        public event Action Done;

        public void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer = null)
        {
            OfflineUiHelper.AddOfflineUI(parentVisual, OnButtonClicked);
        }

        void OnButtonClicked()
        {
            Done?.Invoke();
        }

        public void Dispose()
        {
            Done = null;
        }
    }
}
#endif
