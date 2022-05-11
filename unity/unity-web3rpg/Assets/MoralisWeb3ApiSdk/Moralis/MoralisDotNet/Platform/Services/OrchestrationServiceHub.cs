using System;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;

namespace Moralis.Platform.Services
{
    public class OrchestrationServiceHub<TUser> : IServiceHub<TUser> where TUser : MoralisUser
    {
        public OrchestrationServiceHub()
        {
            AlwaysSave = true;
        }

        public bool AlwaysSave { get; set; }
        public IServiceHub<TUser> Default { get; set; }

        public IServiceHub<TUser> Custom { get; set; }

        //public IServiceHubCloner Cloner => Custom.Cloner ?? Default.Cloner;

        public IMetadataService MetadataService => Custom.MetadataService ?? Default.MetadataService;

        public IJsonSerializer JsonSerializer => Custom.JsonSerializer ?? Default.JsonSerializer;

        public IWebClient WebClient => Custom.WebClient ?? Default.WebClient;

        public ICacheService CacheService => Custom.CacheService ?? Default.CacheService;

        public IInstallationService InstallationService => Custom.InstallationService ?? Default.InstallationService;

        public IMoralisCommandRunner CommandRunner => Custom.CommandRunner ?? Default.CommandRunner;

        public ICloudFunctionService CloudFunctionService => Custom.CloudFunctionService ?? Default.CloudFunctionService;

        //public IParseConfigurationController ConfigurationController => Custom.ConfigurationController ?? Default.ConfigurationController;

        public IFileService FileService => Custom.FileService ?? Default.FileService;

        public IObjectService ObjectService => Custom.ObjectService ?? Default.ObjectService;

        public IQueryService QueryService => Custom.QueryService ?? Default.QueryService;

        public ISessionService<TUser> SessionService => Custom.SessionService ?? Default.SessionService;

        public IUserService<TUser> UserService => Custom.UserService ?? Default.UserService;

        public ICurrentUserService<TUser> CurrentUserService => Custom.CurrentUserService ?? Default.CurrentUserService;

        //public IParseAnalyticsController AnalyticsController => Custom.AnalyticsController ?? Default.AnalyticsController;

        //public IParsePushChannelsController PushChannelsController => Custom.PushChannelsController ?? Default.PushChannelsController;

        //public IParsePushController PushController => Custom.PushController ?? Default.PushController;

        public IInstallationService InstallationController => Custom.InstallationService ?? Default.InstallationService;

        public IServerConnectionData ServerConnectionData => Custom.ServerConnectionData ?? Default.ServerConnectionData;


        public T Create<T>(object[] parameters) where T : MoralisObject
        {
            T thing;

            if (parameters is { } && parameters.Length > 0)
                thing = (T)Activator.CreateInstance(typeof(T), parameters);
            else
                thing = (T)Activator.CreateInstance(typeof(T));

            thing.sessionToken = this.CurrentUserService.CurrentUser.sessionToken;
            thing.ObjectService = this.ObjectService;

            return thing;
        }
    }
}
