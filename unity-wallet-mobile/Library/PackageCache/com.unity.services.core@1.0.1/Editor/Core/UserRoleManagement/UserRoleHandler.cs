using System;
using UnityEditor;
using UnityEngine;
using Unity.Services.Core.Editor;

namespace Unity.Services.Core.Editor
{
    class UserRoleHandler : IDisposable, IUserRoleHandler
    {
        IAsyncOperation<UserRole> m_CurrentUserRoleOperation;

        public event Action<UserRole> UserRoleChanged;
        public event Action<UserRole> UserRoleRequestCompleted;
        public UserRole CurrentUserRole { get; private set; }
        public bool HasAuthorizationError { get; private set; }

        public UserRoleHandler()
        {
            CurrentUserRole = UserRole.Unknown;

#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged += OnProjectStateChanged;
#endif
        }

        void OnProjectStateChanged()
        {
            var projectState = new ProjectStateRequest().GetProjectState();
            if (ShouldResetUserRole(projectState))
            {
                CurrentUserRole = UserRole.Unknown;
            }
            else
            {
                TrySendUserRoleRequest();
            }
        }

        static bool ShouldResetUserRole(ProjectState projectState)
        {
            var output = !EditorGameServiceSettingsProvider.IsUserOnline(projectState) ||
                !EditorGameServiceSettingsProvider.IsUserLoggedIn(projectState);

#if ENABLE_EDITOR_GAME_SERVICES
            output |= !EditorGameServiceSettingsProvider.IsProjectBound(projectState);
#endif

            return output;
        }

        public void Dispose()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged -= OnProjectStateChanged;
#endif
        }

        public bool IsBusy()
        {
            return m_CurrentUserRoleOperation != null &&
                m_CurrentUserRoleOperation.Status == AsyncOperationStatus.InProgress;
        }

        public void TrySendUserRoleRequest()
        {
            if (ShouldSendUserRoleRequest())
            {
                SendUserRoleRequest();
            }
            else
            {
#if ENABLE_EDITOR_GAME_SERVICES
                CloudProjectSettingsEventManager.instance.projectStateChanged -= TrySendUserRoleRequest;
                CloudProjectSettingsEventManager.instance.projectStateChanged += TrySendUserRoleRequest;
#endif
            }
        }

        bool ShouldSendUserRoleRequest()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            return CloudProjectSettings.projectBound && !string.IsNullOrEmpty(CloudProjectSettings.organizationKey);
#else
            return true;
#endif
        }

        void SendUserRoleRequest()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged -= TrySendUserRoleRequest;
#endif

            m_CurrentUserRoleOperation = new UserRoleRequest().GetUserRole();
            m_CurrentUserRoleOperation.Completed += OnUserRoleRequestOperationCompleted;
        }

        void OnUserRoleRequestOperationCompleted(IAsyncOperation<UserRole> asyncOperation)
        {
            m_CurrentUserRoleOperation = null;

            var previousUserRole = CurrentUserRole;
            CurrentUserRole = UserRole.Unknown;
            SetAuthorizationErrorFlag(asyncOperation.Exception);
            if (asyncOperation.Status == AsyncOperationStatus.Succeeded)
            {
                CurrentUserRole = asyncOperation.Result;
            }

            UserRoleRequestCompleted?.Invoke(CurrentUserRole);

            if (previousUserRole != CurrentUserRole)
            {
                UserRoleChanged?.Invoke(CurrentUserRole);
            }
        }

        void SetAuthorizationErrorFlag(Exception exception)
        {
            HasAuthorizationError = exception?.GetType() == typeof(UserRoleRequest.RequestNotAuthorizedException);
        }
    }
}
