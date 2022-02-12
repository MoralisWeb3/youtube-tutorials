using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;
using Moralis.Platform.Queries;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.ClientServices
{
    public static class UserServiceExtensions
    {
        internal static Task<string> GetCurrentSessionTokenAsync<TUser>(this IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default) where TUser : MoralisUser => serviceHub.CurrentUserService.GetCurrentSessionTokenAsync(serviceHub, cancellationToken);

        //public static Task<TUser> LogInWithAsync<TUser>(this IServiceHub<TUser> serviceHub, string authType, IDictionary<string, object> data, CancellationToken cancellationToken) where TUser : MoralisUser
        //{
        //    TUser user = null;

        //    return serviceHub.UserService.LogInAsync(authType, data, serviceHub, cancellationToken).ContinueWith(task => SaveCurrentUserAsync((IServiceHub<TUser>)serviceHub, task.Result).OnSuccess(_ => task.Result)).Unwrap();
        //        //OnSuccess(task => task.Result );
        //}

        public static Task<TUser> LogInWithAsync<TUser>(this IServiceHub<TUser> serviceHub, string authType, CancellationToken cancellationToken) where TUser : MoralisUser
        {
            IAuthenticationProvider provider = MoralisUser.GetProvider(authType);
            return provider.AuthenticateAsync(cancellationToken).OnSuccess(authData => LogInWithAsync(serviceHub, authType, authData.Result, cancellationToken)).Unwrap();
        }

        /// <summary>
        /// Logs in a user with a username and password. On success, this saves the session to disk or to memory so you can retrieve the currently logged in user using <see cref="GetCurrentUser(IServiceHub)"/>.
        /// </summary>
        /// <param name="serviceHub">The <see cref="IServiceHub"/> instance to target when logging in.</param>
        /// <param name="username">The username to log in with.</param>
        /// <param name="password">The password to log in with.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly logged-in user.</returns>
        public static Task<TUser> LogInAsync<TUser>(this IServiceHub<TUser> serviceHub, string username, string password, CancellationToken cancellationToken = default) where TUser : MoralisUser => serviceHub.UserService.LogInAsync(username, password, serviceHub, cancellationToken).OnSuccess(task =>
        {
            TUser user = task.Result;
            return SaveCurrentUserAsync<TUser>(serviceHub, user).OnSuccess(_ => user);
        }).Unwrap();

        /// <summary>
        /// Logs in a user with a username and password. On success, this saves the session to disk so you
        /// can retrieve the currently logged in user using <see cref="GetCurrentUser()"/>.
        /// </summary>
        /// <param name="sessionToken">The session token to authorize with</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user if authorization was successful</returns>
        public static Task<TUser> BecomeAsync<TUser>(this IServiceHub<TUser> serviceHub, string sessionToken, CancellationToken cancellationToken = default) where TUser : MoralisUser => serviceHub.UserService.GetUserAsync(sessionToken, serviceHub, cancellationToken).OnSuccess(t =>
        {
            TUser user = t.Result;
            return SaveCurrentUserAsync<TUser>(serviceHub, user).OnSuccess(_ => user);
        }).Unwrap();

        /// <summary>
        /// Logs out the currently logged in user session. This will remove the session from disk, log out of
        /// linked services, and future calls to <see cref="GetCurrentUser()"/> will return <c>null</c>.
        /// </summary>
        /// <remarks>
        /// Typically, you should use <see cref="LogOutAsync()"/>, unless you are managing your own threading.
        /// </remarks>
        public static void LogOut<TUser>(this IServiceHub<TUser> serviceHub) where TUser : MoralisUser => LogOutAsync(serviceHub).Wait(); // TODO (hallucinogen): this will without a doubt fail in Unity. But what else can we do?

        /// <summary>
        /// Logs out the currently logged in user session. This will remove the session from disk, log out of
        /// linked services, and future calls to <see cref="GetCurrentUser()"/> will return <c>null</c>.
        /// </summary>
        /// <remarks>
        /// This is preferable to using <see cref="LogOut()"/>, unless your code is already running from a
        /// background thread.
        /// </remarks>
        public static Task LogOutAsync<TUser>(this IServiceHub<TUser> serviceHub) where TUser : MoralisUser => LogOutAsync(serviceHub, CancellationToken.None);

        /// <summary>
        /// Logs out the currently logged in user session. This will remove the session from disk, log out of
        /// linked services, and future calls to <see cref="GetCurrentUser(IServiceHub)"/> will return <c>null</c>.
        ///
        /// This is preferable to using <see cref="LogOut()"/>, unless your code is already running from a
        /// background thread.
        /// </summary>
        public static Task LogOutAsync<TUser>(this IServiceHub<TUser> serviceHub, CancellationToken cancellationToken) where TUser : MoralisUser => GetCurrentUserAsync(serviceHub).OnSuccess(task =>
        {
            LogOutWithProviders();
            return task.Result is { } user ? user.TaskQueue.Enqueue(toAwait => serviceHub.CurrentUserService.LogOutAsync(serviceHub, cancellationToken), cancellationToken) : Task.CompletedTask;
            //user.LogOutAsync(toAwait, cancellationToken), cancellationToken) : Task.CompletedTask;
        }).Unwrap();

        static void LogOutWithProviders()
        {
            foreach (IAuthenticationProvider provider in MoralisUser.Authenticators.Values)
            {
                provider.Deauthenticate();
            }
        }

        /// <summary>
        /// Gets the currently logged in MoralisUser with a valid session, either from memory or disk
        /// if necessary.
        /// TODO Decide if this shoiuld be made async since the WebGL version is.
        /// </summary>
        public static MoralisUser GetCurrentUser<TUser>(this IServiceHub<TUser> serviceHub) where TUser : MoralisUser
        {
            Task<TUser> userTask = GetCurrentUserAsync(serviceHub);
            // Wait for get user task to complete.
            userTask.Wait();
            return userTask.Result;
        }

        /// <summary>
        /// Gets the currently logged in MoralisUser with a valid session, either from memory or disk
        /// if necessary, asynchronously.
        /// </summary>
        internal static Task<TUser> GetCurrentUserAsync<TUser>(this IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default) where TUser : MoralisUser => serviceHub.CurrentUserService.GetAsync(serviceHub, cancellationToken);

        internal static Task SaveCurrentUserAsync<TUser>(this IServiceHub<TUser> serviceHub, TUser user, CancellationToken cancellationToken = default) where TUser : MoralisUser => serviceHub.CurrentUserService.SetAsync(user, cancellationToken);

        internal static void ClearInMemoryUser<TUser>(this IServiceHub<TUser> serviceHub) where TUser : MoralisUser => serviceHub.CurrentUserService.ClearFromMemory();

        /// <summary>
        /// Constructs a <see cref="MoralisQuery{MoralisUser}"/> for <see cref="MoralisUser"/>s.
        /// </summary>
        public static MoralisQuery<TUser> GetUserQuery<TUser>(this IServiceHub<TUser> serviceHub) where TUser : MoralisUser => serviceHub.GetQuery<TUser, TUser>();

        #region Legacy / Revocable Session Tokens

        /// <summary>
        /// Tells server to use revocable session on LogIn and SignUp, even when App's Settings
        /// has "Require Revocable Session" turned off. Issues network request in background to
        /// migrate the sessionToken on disk to revocable session.
        /// </summary>
        /// <returns>The Task that upgrades the session.</returns>
        public static Task EnableRevocableSessionAsync(this IServiceHub<MoralisUser> serviceHub, CancellationToken cancellationToken = default)
        {
            lock (serviceHub.UserService.RevocableSessionEnabledMutex)
            {
                serviceHub.UserService.RevocableSessionEnabled = true;
            }

            return GetCurrentUserAsync(serviceHub, cancellationToken).OnSuccess(task => task.Result);
        }

        internal static void DisableRevocableSession(this IServiceHub<MoralisUser> serviceHub)
        {
            lock (serviceHub.UserService.RevocableSessionEnabledMutex)
            {
                serviceHub.UserService.RevocableSessionEnabled = false;
            }
        }

        internal static bool GetIsRevocableSessionEnabled(this IServiceHub<MoralisUser> serviceHub)
        {
            lock (serviceHub.UserService.RevocableSessionEnabledMutex)
            {
                return serviceHub.UserService.RevocableSessionEnabled;
            }
        }

        #endregion

        /// <summary>
        /// Requests a password reset email to be sent to the specified email address associated with the
        /// user account. This email allows the user to securely reset their password on the Moralis site.
        /// </summary>
        /// <param name="email">The email address associated with the user that forgot their password.</param>
        public static Task RequestPasswordResetAsync<TUser>(this IServiceHub<TUser> serviceHub, string email) where TUser : MoralisUser => RequestPasswordResetAsync(serviceHub, email, CancellationToken.None);

        /// <summary>
        /// Requests a password reset email to be sent to the specified email address associated with the
        /// user account. This email allows the user to securely reset their password on the Moralis site.
        /// </summary>
        /// <param name="email">The email address associated with the user that forgot their password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static Task RequestPasswordResetAsync<TUser>(this IServiceHub<TUser> serviceHub, string email, CancellationToken cancellationToken) where TUser : MoralisUser => serviceHub.UserService.RequestPasswordResetAsync(email, cancellationToken);

        public static Task<TUser> LogInWithAsync<TUser>(this IServiceHub<TUser> serviceHub, string authType, IDictionary<string, object> data, CancellationToken cancellationToken) where TUser : MoralisUser
        {
            TUser user = null;

            return serviceHub.UserService.LogInAsync(authType, data, serviceHub, cancellationToken).OnSuccess(task =>
            {
                user = task.Result;

                lock (user.Mutex)
                {
                    if (user.authData == null)
                    {
                        user.authData = new Dictionary<string, IDictionary<string, object>>();
                    }

                    user.authData[authType] = data;

#warning Check if SynchronizeAllAuthData should accept an IServiceHub for consistency on which actions take place on which IServiceHub implementation instance.

                    user.SynchronizeAllAuthData();
                }

                return SaveCurrentUserAsync(serviceHub, user);
            }).Unwrap().OnSuccess(t => user);
        }

        //public static Task<TUser> LogInWithAsync<TUser>(this IServiceHub<TUser> serviceHub, string authType, CancellationToken cancellationToken) where TUser : MoralisUser
        //{
        //    IAuthenticationProvider provider = MoralisUser.GetProvider(authType);
        //    return provider.AuthenticateAsync(cancellationToken).OnSuccess(authData => LogInWithAsync(serviceHub, authType, authData.Result, cancellationToken)).Unwrap();
        //}

        internal static void RegisterProvider<TUser>(this IServiceHub<TUser> serviceHub, IAuthenticationProvider provider) where TUser : MoralisUser
        {
            MoralisUser.Authenticators[provider.AuthType] = provider;
            MoralisUser curUser = GetCurrentUser(serviceHub);

            if (curUser != null)
            {
#warning Check if SynchronizeAllAuthData should accept an IServiceHub for consistency on which actions take place on which IServiceHub implementation instance.

                curUser.SynchronizeAuthData(provider);
            }
        }

    }
}
