using Moralis.WebGL.Platform.Objects;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface IMutableServiceHub<TUser> : IServiceHub<TUser> where TUser : MoralisUser
    {
        IServerConnectionData ServerConnectionData { set; }
        IMetadataService MetadataService { set; }
        IJsonSerializer JsonSerializer { set; }
        //IServiceHubCloner Cloner { set; }

        IWebClient WebClient { set; }
        ICacheService CacheService { set; }
        //IParseObjectClassController ClassController { set; }

        //IParseDataDecoder Decoder { set; }

        //IParseInstallationController InstallationController { set; }
        IMoralisCommandRunner CommandRunner { set; }

        //IParseCloudCodeController CloudCodeController { set; }
        //IParseConfigurationController ConfigurationController { set; }
        //IParseFileController FileController { set; }
        //IParseObjectController ObjectController { set; }
        //IParseQueryController QueryController { set; }
        //IParseSessionController SessionController { set; }
        IUserService<TUser> UserService { set; }
        ICurrentUserService<TUser> CurrentUserService { get; }

        //IParseAnalyticsController AnalyticsController { set; }

        //IParseInstallationCoder InstallationCoder { set; }

        //IParsePushChannelsController PushChannelsController { set; }
        //IParsePushController PushController { set; }
        //IParseCurrentInstallationController CurrentInstallationController { set; }
        //IParseInstallationDataFinalizer InstallationDataFinalizer { set; }
    }
}
