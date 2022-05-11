#if ENABLE_EDITOR_GAME_SERVICES
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ActivationPopup
{
    interface IActivationPopupVisual : IDisposable
    {
        event Action Done;
        void Init(VisualElement parentVisual, IEnumerable<IEditorGameService> services, VisualElement buttonsContainer = null);
    }
}
#endif
