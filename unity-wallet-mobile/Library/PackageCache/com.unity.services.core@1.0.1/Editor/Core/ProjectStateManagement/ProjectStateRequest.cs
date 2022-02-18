using UnityEditor;
using UnityEngine;

namespace Unity.Services.Core.Editor
{
    class ProjectStateRequest : IProjectStateRequest
    {
        public ProjectState GetProjectState()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            return new ProjectState(CloudProjectSettings.userId, CloudProjectSettings.userName, CloudProjectSettings.accessToken,
                CloudProjectSettings.projectId, CloudProjectSettings.projectName, CloudProjectSettings.organizationId,
                CloudProjectSettings.organizationName, CloudProjectSettings.coppaCompliance, CloudProjectSettings.projectBound,
                IsInternetReachable());
#else
            return new ProjectState(CloudProjectSettings.userId, CloudProjectSettings.userName, CloudProjectSettings.accessToken,
                CloudProjectSettings.projectId, CloudProjectSettings.projectName, CloudProjectSettings.organizationId,
                CloudProjectSettings.organizationName, false, IsInternetReachable());
#endif
        }

        static bool IsInternetReachable()
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }
}
