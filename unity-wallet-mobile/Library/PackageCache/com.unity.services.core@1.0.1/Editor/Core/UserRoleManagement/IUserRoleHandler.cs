using System;

namespace Unity.Services.Core.Editor
{
    interface IUserRoleHandler
    {
        UserRole CurrentUserRole { get; }

        bool HasAuthorizationError { get; }

        bool IsBusy();

        void TrySendUserRoleRequest();

        event Action<UserRole> UserRoleChanged;
        event Action<UserRole> UserRoleRequestCompleted;
    }
}
