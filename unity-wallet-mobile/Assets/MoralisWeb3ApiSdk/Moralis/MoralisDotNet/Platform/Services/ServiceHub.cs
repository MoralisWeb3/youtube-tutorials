using Moralis.Platform.Abstractions;
using Moralis.Platform.Services.Infrastructure;
using Moralis.Platform.Services.ClientServices;
using Moralis.Platform.Objects;
using Moralis.Platform.Utilities;
using System;
using System.Net.Http;

namespace Moralis.Platform.Services
{
    /// <summary>
    /// A service hub that uses late initialization to efficiently provide controllers and other dependencies to internal Moralis SDK systems.
    /// </summary>
    public class ServiceHub<TUser> : IServiceHub<TUser> where TUser : MoralisUser
    {
        private static ServiceHub<TUser> instance;

        #region Instance
        public bool AlwaysSave { get; set; }
        LateInitializer LateInitializer { get; } = new LateInitializer { };
        private UniversalWebClient webClient;

        public ServiceHub()
        {
            webClient = new UniversalWebClient { };
        }

        public ServiceHub(IServerConnectionData connectionData, IJsonSerializer jsonSerializer, HttpClient httpClient = null)
        {
            httpClient = httpClient is { } ? httpClient : new HttpClient();
            webClient = new UniversalWebClient(httpClient);
            httpClient.DefaultRequestHeaders.Remove("IfModifiedSince");
            ServerConnectionData ??= connectionData;
            JsonSerializer = jsonSerializer is { } ? jsonSerializer : throw new ArgumentException("jsonSerializer cannot be null.");

            AlwaysSave = true;
        }

        /// <summary>
        /// Included so that this can be set prior to initialization for systems
        /// (Unity, Xamarin, etc.) that may not have Assembly Attributes available.
        /// </summary>
        public static HostManifestData ManifestData { get; set; }

        public IServerConnectionData ServerConnectionData { get; set; }
        /// <summary>
        /// Provides Serialization / Deserialization services.
        /// </summary>
        public IJsonSerializer JsonSerializer { get; set; }
        public IMetadataService MetadataService => LateInitializer.GetValue(() => new MetadataService { HostManifestData = ManifestData ?? HostManifestData.Inferred, EnvironmentData = EnvironmentData.Inferred });

        public IWebClient WebClient => LateInitializer.GetValue(() => webClient);
        public ICacheService CacheService => LateInitializer.GetValue(() => new MoralisCacheService<TUser> (MoralisCacheService<TUser>.DefineRelativeFilePath("Moralis\\moralis.cachefile")));
        public IInstallationService InstallationService => LateInitializer.GetValue(() => new InstallationService(CacheService));
        public IMoralisCommandRunner CommandRunner => LateInitializer.GetValue(() => new MoralisCommandRunner<TUser>(WebClient, InstallationService, MetadataService, ServerConnectionData, new Lazy<IUserService<TUser>>(() => UserService)));
        public IUserService<TUser> UserService => LateInitializer.GetValue(() => new MoralisUserService<TUser>(CommandRunner, ObjectService, JsonSerializer));
        public ICurrentUserService<TUser> CurrentUserService => LateInitializer.GetValue(() => new MoralisCurrentUserService<TUser>(CacheService, JsonSerializer));
        public IObjectService ObjectService => LateInitializer.GetValue(() => new MoralisObjectService(CommandRunner, ServerConnectionData, JsonSerializer));
        public IQueryService QueryService => LateInitializer.GetValue(() => new MoralisQueryService(CommandRunner, this.CurrentUserService.CurrentUser?.sessionToken, JsonSerializer, ObjectService));
        public ISessionService<TUser> SessionService => LateInitializer.GetValue(() => new MoralisSessionService<TUser>(CommandRunner, JsonSerializer));
        public ICloudFunctionService CloudFunctionService => LateInitializer.GetValue(() => new MoralisCloudFunctionService(CommandRunner, ServerConnectionData, JsonSerializer));
        public IFileService FileService => LateInitializer.GetValue(() => new MoralisFileService(CommandRunner, JsonSerializer));
        

        public bool Reset() => LateInitializer.Used && LateInitializer.Reset();

        public T Create<T>(object[] parameters) where T : MoralisObject
        {
            T thing;
            
            if (parameters is { } && parameters.Length > 0)
                thing = (T)Activator.CreateInstance(typeof(T), parameters);
            else
                thing = (T)Activator.CreateInstance(typeof(T));

            thing.sessionToken = this.CurrentUserService.CurrentUser?.sessionToken;
            thing.ObjectService = this.ObjectService;

            return thing;
        }
        #endregion

        public static ServiceHub<TUser> GetInstance() {
            if (!(instance is { }))
                instance = new ServiceHub<TUser>();

            return instance;
        }

        public static ServiceHub<TUser> GetInstance(IServerConnectionData connectionData, HttpClient httpClient = null, IJsonSerializer jsonSerializer = null)
        {
            if (!(instance is { }))
                instance = new ServiceHub<TUser>(connectionData, jsonSerializer, httpClient);

            return instance;
        }
    }

}
