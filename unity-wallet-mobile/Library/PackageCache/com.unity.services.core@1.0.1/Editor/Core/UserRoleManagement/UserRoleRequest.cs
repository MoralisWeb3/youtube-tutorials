using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace Unity.Services.Core.Editor
{
    class UserRoleRequest : IUserRoleRequest
    {
        public IAsyncOperation<UserRole> GetUserRole()
        {
            var resultAsyncOp = new AsyncOperation<UserRole>();
            try
            {
                resultAsyncOp.SetInProgress();
                var cdnEndpoint = new DefaultCdnConfiguredEndpoint();
                var configurationRequestTask = cdnEndpoint.GetConfiguration();
                configurationRequestTask.Completed += configOperation => QueryProjectUsers(configOperation, resultAsyncOp);
            }
            catch (Exception ex)
            {
                resultAsyncOp.Fail(ex);
            }

            return resultAsyncOp;
        }

        static void QueryProjectUsers(IAsyncOperation<DefaultCdnEndpointConfiguration> configurationRequestTask, AsyncOperation<UserRole> resultAsyncOp)
        {
            try
            {
#if ENABLE_EDITOR_GAME_SERVICES
                var organizationKey = CloudProjectSettings.organizationKey;
#else
                var organizationKey = CloudProjectSettings.organizationId;
#endif
                var usersUrl = configurationRequestTask.Result.BuildUsersUrl(organizationKey, CloudProjectSettings.projectId);
                var getProjectUsersRequest = new UnityWebRequest(usersUrl,
                    UnityWebRequest.kHttpVerbGET)
                {
                    downloadHandler = new DownloadHandlerBuffer()
                };
                getProjectUsersRequest.SetRequestHeader("AUTHORIZATION", $"Bearer {CloudProjectSettings.accessToken}");
                var operation = getProjectUsersRequest.SendWebRequest();
                operation.completed += op => FindUserRoleToFinishAsyncOperation(getProjectUsersRequest, resultAsyncOp);
            }
            catch (Exception ex)
            {
                resultAsyncOp.Fail(ex);
            }
        }

        static void FindUserRoleToFinishAsyncOperation(UnityWebRequest getProjectUsersRequest, AsyncOperation<UserRole> resultAsyncOp)
        {
            const int requestNotAuthorizedCode = 401;
            try
            {
                if (getProjectUsersRequest.responseCode == requestNotAuthorizedCode)
                {
                    throw new RequestNotAuthorizedException();
                }

                var currentUserRole = UserRole.Unknown;
                var userList = ExtractUserListFromUnityWebRequest(getProjectUsersRequest);
                if (userList != null)
                {
                    var currentUser = FindCurrentUserInList(CloudProjectSettings.userId, userList.Users);
                    if (currentUser != null)
                    {
                        currentUserRole = currentUser.Role;
                    }
                    else
                    {
                        throw new CurrentUserNotFoundException();
                    }
                }
                else
                {
                    throw new UserListNotFoundException();
                }

                resultAsyncOp.Succeed(currentUserRole);
            }
            catch (Exception ex)
            {
                resultAsyncOp.Fail(ex);
            }
            finally
            {
                getProjectUsersRequest.Dispose();
            }
        }

        static UserList ExtractUserListFromUnityWebRequest(UnityWebRequest unityWebRequest)
        {
            if (UnityWebRequestHelper.IsUnityWebRequestReadyForTextExtract(unityWebRequest, out var jsonContent))
            {
                UserList userList = null;
                if (JsonHelper.TryJsonDeserialize(jsonContent, ref userList))
                {
                    return userList;
                }
            }

            return null;
        }

        static User FindCurrentUserInList(string currentUserId, IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                if (user.Id.Equals(currentUserId))
                {
                    return user;
                }
            }

            return null;
        }

        /// <remarks>
        /// Serialized field don't follow naming conventions for interoperability reasons.
        /// </remarks>
        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        class UserList
        {
            [SerializeField]
            User[] users;

            public User[] Users => users;
        }

        /// <remarks>
        /// Serialized field don't follow naming conventions for interoperability reasons.
        /// </remarks>
        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        class User
        {
            [SerializeField]
            string foreign_key;

            public string Id => foreign_key;

            [SerializeField]
            string name;

            public string Name => name;

            [SerializeField]
            string email;

            public string Email => email;

            [SerializeField]
            string access_granted_by;

            public string AccessGrantedBy => access_granted_by;

            [SerializeField]
            string role;

            public UserRole Role
            {
                get
                {
                    if (Enum.TryParse(role, true, out UserRole userRole))
                    {
                        return userRole;
                    }

                    throw new UnknownUserRoleException();
                }
            }
        }

        internal class RequestNotAuthorizedException : Exception { }

        class CurrentUserNotFoundException : Exception { }

        class UserListNotFoundException : Exception { }

        class UnknownUserRoleException : Exception { }
    }
}
