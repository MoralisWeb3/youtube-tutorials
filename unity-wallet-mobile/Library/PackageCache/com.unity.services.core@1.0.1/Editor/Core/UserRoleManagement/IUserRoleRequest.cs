using System;

namespace Unity.Services.Core.Editor
{
    interface IUserRoleRequest
    {
        IAsyncOperation<UserRole> GetUserRole();
    }
}
