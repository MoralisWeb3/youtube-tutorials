using System;
using System.Threading.Tasks;
using Moralis.Platform;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;
using Moralis.Platform.Queries;
using Moralis.Platform.Services.ClientServices;
using System.Collections.Generic;
using System.Threading;
using Moralis.Web3Api.Interfaces;
using Moralis.Platform.Services.Infrastructure;

namespace Moralis
{
    public class MoralisClient<TUser> where TUser : MoralisUser
    {
        string serverAuthToken = "";
        string serverAuthType = "";

        public MoralisClient(ServerConnectionData connectionData, IWeb3Api web3Api = null, IJsonSerializer jsonSerializer = null)
        {
            if (jsonSerializer == null)
            {
                throw new ArgumentException("jsonSerializer cannot be null.");
            }

            connectionData.Key = connectionData.Key ?? "";

            moralisService = new MoralisService<TUser>(connectionData.ApplicationID, connectionData.ServerURI, connectionData.Key, jsonSerializer);
            moralisService.ServerConnectionData.Key = connectionData.Key;
            moralisService.ServerConnectionData.ServerURI = connectionData.ServerURI;
            moralisService.ServerConnectionData.ApplicationID = connectionData.ApplicationID; moralisService.ServerConnectionData.LocalStoragePath = connectionData.LocalStoragePath;

            // Make sure local folder for Unity apps is used if defined.
            MoralisCacheService<TUser>.BaseFilePath = connectionData.LocalStoragePath;

            // Make sure singleton instance is available.
            moralisService.Publicize();

            this.Web3Api = web3Api;

            if (this.Web3Api is { })
            {
                if (connectionData.ApiKey is { })
                {
                    this.Web3Api.Initialize();
                }
                else
                {
                    this.Web3Api.Initialize(connectionData.ServerURI);
                }

            }
        }


        public string EthAddress { get; }

        public IServiceHub<TUser> ServiceHub => moralisService.Services;

        MoralisService<TUser> moralisService;

        public void SetLocalDatastoreController()
        {
            throw new NotImplementedException();
        }

        public string ApplicationId
        {
            get => moralisService.ServerConnectionData.ApplicationID;
            set
            {
                moralisService.ServerConnectionData.ApplicationID = value;
            }
        }

        public string Key
        {
            get => moralisService.ServerConnectionData.Key;
            set
            {
                moralisService.ServerConnectionData.Key = value;
            }
        }

        public string MasterKey
        {
            get => moralisService.ServerConnectionData.MasterKey;
            set
            {
                moralisService.ServerConnectionData.MasterKey = value;
            }
        }

        public string ServerUrl
        {
            get => moralisService.ServerConnectionData.ServerURI;
            set
            {
                moralisService.ServerConnectionData.ServerURI = value;
            }
        }

        public void SetServerAuthToken(string value)
        {
            serverAuthToken = value;
        }

        public string GetServerAuthToken()
        {
            return serverAuthToken;
        }

        public void SetServerAuthType(string value)
        {
            serverAuthToken = value;
        }

        public string GetServerAuthType()
        {
            return serverAuthToken;
        }

        public void SetLiveQueryServerURL(string value)
        {
            throw new NotImplementedException();
        }

        public string GetLiveQueryServerURL()
        {
            throw new NotImplementedException();
        }

        public void SetEncryptedUser(string value)
        {
            throw new NotImplementedException();
        }

        public string GetEncryptedUser()
        {
            throw new NotImplementedException();
        }

        public void SetSecret(string value)
        {
            throw new NotImplementedException();
        }

        public string GetSecret()
        {
            throw new NotImplementedException();
        }

        public void SetIdempotency(string value)
        {
            throw new NotImplementedException();
        }

        public string GetIdempotency()
        {
            throw new NotImplementedException();
        }

        public IFileService File => moralisService.FileService;

        public IInstallationService InstallationService => moralisService.InstallationService;
        public IQueryService QueryService => moralisService.QueryService;
        public ISessionService<TUser> Session => moralisService.SessionService;
        public IUserService<TUser> UserService => moralisService.UserService;

        public MoralisCloud<TUser> Cloud => moralisService.Cloud;


        public async Task<Guid?> GetInstallationIdAsync() => await InstallationService.GetAsync();

        public MoralisQuery<T> Query<T>() where T : MoralisObject
        {
            return new MoralisQuery<T>(this.ServiceHub.QueryService, InstallationService, moralisService.ServerConnectionData, moralisService.JsonSerializer, this.ServiceHub.CurrentUserService.CurrentUser.SessionToken);
        }

        public T Create<T>(object[] parameters = null) where T : MoralisObject
        {
            return this.ServiceHub.Create<T>(parameters);
        }

        public MoralisUser GetCurrentUser() => this.ServiceHub.GetCurrentUser();


        public void Dispose()
        {
            MoralisLiveQueryManager.DisposeService();

        }

        /// <summary>
        /// Retrieve user object by sesion token.
        /// </summary>
        /// <param name="sessionToken"></param>
        /// <returns>Task<MoralisUser</returns>
        public Task<TUser> UserFromSession(string sessionToken)
        {
            return this.ServiceHub.BecomeAsync<TUser>(sessionToken);
        }

        /// <summary>
        /// Provid async user login.
        /// data: 
        ///     id: Address
        ///     signature: Signature from wallet
        ///     data: Message that was signed
        /// </summary>
        /// <param name="data">Authentication data</param>
        /// <returns>Task<TUser></returns>
        public Task<TUser> LogInAsync(IDictionary<string, object> data)
        {
            return this.LogInAsync(data, CancellationToken.None);
        }

        /// Provid async user login.
        /// data: 
        ///     id: Address
        ///     signature: Signature from wallet
        ///     data: Message that was signed
        /// </summary>
        /// <param name="data">Authentication data</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task<TUser></returns>
        public Task<TUser> LogInAsync(IDictionary<string, object> data, CancellationToken cancellationToken)
        {
            return this.ServiceHub.LogInWithAsync("moralisEth", data, cancellationToken);
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns></returns>
        public Task LogOutAsync()
        {
            return this.ServiceHub.LogOutAsync<TUser>();
        }
        /// <summary>
        /// Constructs a query that is the and of the given queries.
        /// </summary>
        /// <typeparam name="T">The type of MoralisObject being queried.</typeparam>
        /// <param name="serviceHub"></param>
        /// <param name="source">An initial query to 'and' with additional queries.</param>
        /// <param name="queries">The list of MoralisQueries to 'and' together.</param>
        /// <returns>A query that is the and of the given queries.</returns>
        public MoralisQuery<T> BuildAndQuery<T>(MoralisQuery<T> source, params MoralisQuery<T>[] queries) where T : MoralisObject
        {
            return ServiceHub.ConstructAndQuery<T, TUser>(source, queries);
        }

        /// <summary>
        /// Construct a query that is the and of two or more queries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceHub"></param>
        /// <param name="queries">The list of MoralisQueries to 'and' together.</param>
        /// <returns>A MoralisQquery that is the 'and' of the passed in queries.</returns>
        public MoralisQuery<T> BuildAndQuery<T>(IEnumerable<MoralisQuery<T>> queries) where T : MoralisObject
        {
            return ServiceHub.ConstructAndQuery<T, TUser>(queries);
        }

        /// <summary>
        /// Constructs a query that is the or of the given queries.
        /// </summary>
        /// <typeparam name="T">The type of MoralisObject being queried.</typeparam>
        /// <param name="source">An initial query to 'or' with additional queries.</param>
        /// <param name="queries">The list of MoralisQueries to 'or' together.</param>
        /// <returns>A query that is the or of the given queries.</returns>
        public MoralisQuery<T> BuildOrQuery<T>(MoralisQuery<T> source, params MoralisQuery<T>[] queries) where T : MoralisObject
        {
            return ServiceHub.ConstructOrQuery<T, TUser>(source, queries);
        }

        /// <summary>
        /// Construct a query that is the 'or' of two or more queries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceHub"></param>
        /// <param name="queries">The list of MoralisQueries to 'and' together.</param>
        /// <returns>A MoralisQquery that is the 'or' of the passed in queries.</returns>
        public MoralisQuery<T> BuildOrQuery<T>(IEnumerable<MoralisQuery<T>> queries) where T : MoralisObject
        {
            return ServiceHub.ConstructOrQuery<T, TUser>(queries);
        }

        /// <summary>
        /// Constructs a query that is the nor of the given queries.
        /// </summary>
        /// <typeparam name="T">The type of MoralisObject being queried.</typeparam>
        /// <param name="source">An initial query to 'or' with additional queries.</param>
        /// <param name="queries">The list of MoralisQueries to 'or' together.</param>
        /// <returns>A query that is the nor of the given queries.</returns>
        public MoralisQuery<T> BuildNorQuery<T>(MoralisQuery<T> source, params MoralisQuery<T>[] queries) where T : MoralisObject
        {
            return ServiceHub.ConstructNorQuery<T, TUser>(source, queries);
        }

        /// <summary>
        /// Construct a query that is the 'nor' of two or more queries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceHub"></param>
        /// <param name="queries">The list of MoralisQueries to 'and' together.</param>
        /// <returns>A MoralisQquery that is the 'nor' of the passed in queries.</returns>
        public MoralisQuery<T> BuildNorQuery<T>(IEnumerable<MoralisQuery<T>> queries) where T : MoralisObject
        {
            return ServiceHub.ConstructNorQuery<T, TUser>(queries);
        }

        /// <summary>
        /// Deletes target object from the Moralis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public Task DeleteAsync<T>(T target) where T : MoralisObject
        {
            return target.DeleteAsync();
        }

        /// <summary>
        /// Provide an object hook for Web3Api incase developer supplies a
        /// web3api client at initialize
        /// </summary>
        public IWeb3Api Web3Api { get; private set; }

        /// <summary>
        /// Included so that this can be set prior to initialization for systems
        /// (Unity, Xamarin, etc.) that may not have Assembly Attributes available.
        /// </summary>
        public static HostManifestData ManifestData
        {
            get => Moralis.Platform.Services.ServiceHub<TUser>.ManifestData;
            set
            {
                Moralis.Platform.Services.ServiceHub<TUser>.ManifestData = value;
                Moralis.Platform.Services.MutableServiceHub<TUser>.ManifestData = value;
            }
        }
    }


}
