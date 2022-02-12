using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Objects
{
    public class MoralisUser : MoralisObject
    {
        public MoralisUser()
        {
            this.ClassName = "_User";
        }

        public MoralisUser(string objectId = null,
            string userName = null,
            IDictionary<string, IDictionary<string, object>> authData = null,
            DateTime? createdAt = null,
            DateTime? updatedAt = null,
            MoralisAcl ACL = null,
            string sessionToken = null) : base("_User", objectId, sessionToken, createdAt, updatedAt, ACL)
        {
            this.username = userName;
            this.authData = authData ?? new Dictionary<string, IDictionary<string, object>>();
        }

        public string username;

        public IDictionary<string, IDictionary<string, object>> authData; 

        //public string sessionToken;
        public string password;
        public string email;


        internal static IDictionary<string, IAuthenticationProvider> Authenticators { get; } = new Dictionary<string, IAuthenticationProvider> { };
    
        internal static HashSet<string> ImmutableKeys { get; } = new HashSet<string> { "sessionToken", "isNew" };
       
        internal ICurrentUserService<MoralisUser> CurrentUserService { get; set; }


        internal Task SignUpAsync(Task toAwait, CancellationToken cancellationToken) => throw new NotFiniteNumberException();


        /// <summary>
        /// Signs up a new user. This will create a new ParseUser on the server and will also persist the
        /// session on disk so that you can access the user using <see cref="CurrentUser"/>. A username and
        /// password must be set before calling SignUpAsync.
        /// </summary>
        public Task SignUpAsync() => SignUpAsync(CancellationToken.None);

        /// <summary>
        /// Signs up a new user. This will create a new ParseUser on the server and will also persist the
        /// session on disk so that you can access the user using <see cref="CurrentUser"/>. A username and
        /// password must be set before calling SignUpAsync.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task SignUpAsync(CancellationToken cancellationToken) => TaskQueue.Enqueue(toAwait => SignUpAsync(toAwait, cancellationToken), cancellationToken);

        //protected override Task SaveAsync(Task toAwait, CancellationToken cancellationToken)
        //{
        //    lock (Mutex)
        //    {
        //        if (ObjectId is null)
        //        {
        //            throw new InvalidOperationException("You must call SignUpAsync before calling SaveAsync.");
        //        }

        //        return base.SaveAsync(toAwait, cancellationToken).OnSuccess(_ => Services.CurrentUserController.IsCurrent(this) ? Services.SaveCurrentUserAsync(this) : Task.CompletedTask).Unwrap();
        //    }
        //}

        // If this is already the current user, refresh its state on disk.
        //internal override Task<ParseObject> FetchAsyncInternal(Task toAwait, CancellationToken cancellationToken) => base.FetchAsyncInternal(toAwait, cancellationToken).OnSuccess(t => !Services.CurrentUserController.IsCurrent(this) ? Task.FromResult(t.Result) : Services.SaveCurrentUserAsync(this).OnSuccess(_ => t.Result)).Unwrap();

        //internal Task LogOutAsync(Task toAwait, CancellationToken cancellationToken)
        //{
        //    string oldSessionToken = SessionToken;
        //    if (oldSessionToken == null)
        //    {
        //        return Task.FromResult(0);
        //    }

        //    // Cleanup in-memory session.
        //    this.SessionToken = null;
        //    return Task.WhenAll(CurrentUserService.LogOutAsync();
        //}

        //internal Task UpgradeToRevocableSessionAsync() => UpgradeToRevocableSessionAsync(CancellationToken.None);

        //internal Task UpgradeToRevocableSessionAsync(CancellationToken cancellationToken) => TaskQueue.Enqueue(toAwait => UpgradeToRevocableSessionAsync(toAwait, cancellationToken), cancellationToken);

        //internal Task UpgradeToRevocableSessionAsync(Task toAwait, CancellationToken cancellationToken)
        //{
        //    string sessionToken = SessionToken;

        //    return toAwait.OnSuccess(_ => Services.UpgradeToRevocableSessionAsync(sessionToken, cancellationToken)).Unwrap().OnSuccess(task => SetSessionTokenAsync(task.Result)).Unwrap();
        //}




        /// <summary>
        /// Removes null values from authData (which exist temporarily for unlinking)
        /// </summary>
        void CleanupAuthData()
        {
            lock (Mutex)
            {
                IDictionary<string, IDictionary<string, object>> _authData = authData;

                if (_authData == null)
                {
                    return;
                }

                foreach (KeyValuePair<string, IDictionary<string, object>> pair in new Dictionary<string, IDictionary<string, object>>(_authData))
                {
                    if (pair.Value == null)
                    {
                        _authData.Remove(pair.Key);
                    }
                }
            }
        }
        
        internal static IAuthenticationProvider GetProvider(string providerName) => Authenticators.TryGetValue(providerName, out IAuthenticationProvider provider) ? provider : null;

        /// <summary>
        /// Synchronizes authData for all providers.
        /// </summary>
        internal void SynchronizeAllAuthData()
        {
            lock (Mutex)
            {
                IDictionary<string, IDictionary<string, object>> _authData = authData;

                if (_authData == null)
                {
                    return;
                }

                foreach (KeyValuePair<string, IDictionary<string, object>> pair in _authData)
                {
                    SynchronizeAuthData(GetProvider(pair.Key));
                }
            }
        }

        internal void SynchronizeAuthData(IAuthenticationProvider provider)
        {
            bool restorationSuccess = false;

            lock (Mutex)
            {
                IDictionary<string, IDictionary<string, object>> _authData = authData;

                if (_authData == null || provider == null)
                {
                    return;
                }

                if (_authData.TryGetValue(provider.AuthType, out IDictionary<string, object> data))
                {
                    restorationSuccess = provider.RestoreAuthentication(data);
                }
            }

            //if (!restorationSuccess)
            //{
            //    UnlinkFromAsync(provider.AuthType, CancellationToken.None);
            //}
        }

        //internal Task LinkWithAsync(string authType, IDictionary<string, object> data, CancellationToken cancellationToken) => TaskQueue.Enqueue(toAwait =>
        //{
        //    IDictionary<string, IDictionary<string, object>> authData = AuthData;

        //    if (authData == null)
        //    {
        //        authData = AuthData = new Dictionary<string, IDictionary<string, object>>();
        //    }

        //    authData[authType] = data;
        //    AuthData = authData;

        //    return SaveAsync(cancellationToken);
        //}, cancellationToken);


        /// <summary>
        /// Unlinks a user from a service.
        /// </summary>
        //internal Task UnlinkFromAsync(string authType, CancellationToken cancellationToken) => LinkWithAsync(authType, null, cancellationToken);

        /// <summary>
        /// Checks whether a user is linked to a service.
        /// </summary>
        internal bool IsLinked(string authType)
        {
            lock (Mutex)
            {
                return authData != null && authData.ContainsKey(authType) && authData[authType] != null;
            }
        }
    }
}
