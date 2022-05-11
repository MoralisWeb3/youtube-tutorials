using UnityEditor.Connect;

namespace Unity.Services.Core.Editor
{
#if ENABLE_EDITOR_GAME_SERVICES
    interface IProjectEditorDrawerFactory
    {
        IProjectEditorDrawer InstantiateDrawer();
    }
#endif
}
