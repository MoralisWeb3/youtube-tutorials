
using System;
using System.Reflection;
using Moralis.WebGL.Platform.Objects;

namespace Moralis.WebGL.Platform.Abstractions
{
    public abstract class CustomServiceHub<TUser> : ICustomServiceHub<TUser> where TUser : MoralisUser
    {
        public bool AlwaysSave { get; set; }

        public virtual IServiceHub<TUser> Services { get; internal set; }

        //public virtual IServiceHubCloner Cloner => Services.Cloner;

        public virtual IMetadataService MetadataService => Services.MetadataService;

        /// <summary>
        /// Provides Serialization / Deserialization services.
        /// </summary>
        public virtual IJsonSerializer JsonSerializer => Services.JsonSerializer;

        public virtual IWebClient WebClient => Services.WebClient;

        public virtual ICacheService CacheService => Services.CacheService;

        //public virtual IParseObjectClassController ClassController => Services.ClassController;

        public IInstallationService InstallationService => Services.InstallationService;

        public virtual IMoralisCommandRunner CommandRunner => Services.CommandRunner;

        public virtual ICloudFunctionService CloudFunctionService => Services.CloudFunctionService;

        //public virtual IParseConfigurationController ConfigurationController => Services.ConfigurationController;

        public virtual IFileService FileService => Services.FileService;

        public virtual IObjectService ObjectService => Services.ObjectService;

        public virtual IQueryService QueryService => Services.QueryService;

        public virtual ISessionService<TUser> SessionService => Services.SessionService;

        public virtual IUserService<TUser> UserService => Services.UserService;

        public virtual ICurrentUserService<TUser> CurrentUserService => Services.CurrentUserService;

        //public virtual IParseAnalyticsController AnalyticsController => Services.AnalyticsController;

        //public virtual IParseInstallationCoder InstallationCoder => Services.InstallationCoder;

        //public virtual IParsePushChannelsController PushChannelsController => Services.PushChannelsController;

        //public virtual IParsePushController PushController => Services.PushController;

        //public virtual IParseCurrentInstallationController CurrentInstallationController => Services.CurrentInstallationController;

        public virtual IServerConnectionData ServerConnectionData => Services.ServerConnectionData;

        //public virtual IParseDataDecoder Decoder => Services.Decoder;

        //public virtual IParseInstallationDataFinalizer InstallationDataFinalizer => Services.InstallationDataFinalizer;
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
