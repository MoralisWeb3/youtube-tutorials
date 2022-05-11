using Moralis.Web3Api.Interfaces;

namespace Moralis.Web3Api.Client
{
    /// <summary>
    /// Provides a wrapper class around the Moralis Web3Api REST service. 
    /// Automagically initializes client to use standard server or
    /// personal server Cloud Function API based on use of Api Key.
    /// </summary>
    public class Web3ApiClient : IWeb3Api
    {
        private static string defaultServerUrl = "https://deep-index.moralis.io/api/v2";

        /// <summary>
        /// AccountApi operations.
        /// </summary>
        public IAccountApi Account { get; private set; }

        /// <summary>
        /// DefiApi operations
        /// </summary>
        public IDefiApi Defi { get; private set; }

        /// <summary>
        /// NativeApi operations.
        /// </summary>
        public INativeApi Native { get; private set; }

        /// <summary>
        /// ResolveApi operations.
        /// </summary>
        public IResolveApi Resolve { get; private set; }

        /// <summary>
        /// StorageApi operations.
        /// </summary>
        public IStorageApi Storage { get; private set; }

        /// <summary>
        /// TokenApi operations.
        /// </summary>
        public ITokenApi Token { get; private set; }

        /// <summary>
        /// Indicates that the client has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        // Default constructor.
        public Web3ApiClient() { }

        /// <summary>
        /// Initialize client using just server url.
        /// </summary>
        /// <param name="serverUrl"></param>
        public Web3ApiClient(string serverUrl)
        {
            Initialize(serverUrl);
        }

        /// <summary>
        /// Initialize the client using serverUrl. If serverUrl is null default is used. 
        /// ApiKey is passed via Configuration signleton.
        /// </summary>
        /// <param name="serverUrl"></param>
        public void Initialize(string serverUrl=null)
        {
            // Initialize client
            ApiClient client = new ApiClient(serverUrl is { } ? serverUrl : defaultServerUrl);

            // Set endpoints based on api key. If apikey is set
            // use the direct Web3Api server.
            if (Configuration.ApiKey.ContainsKey("X-API-Key"))
            {
                this.Account = new Moralis.Web3Api.Api.AccountApi(client);
                this.Defi = new Moralis.Web3Api.Api.DefiApi(client);
                this.Native = new Moralis.Web3Api.Api.NativeApi(client);
                this.Resolve = new Moralis.Web3Api.Api.ResolveApi(client);
                this.Storage = new Moralis.Web3Api.Api.StorageApi(client);
                this.Token = new Moralis.Web3Api.Api.TokenApi(client);
            }
            // Api key not set assume the url is for moralis personal server
            // and Cloud Function API should be used.
            else
            {
                client.ApiVersion = "server";

                this.Account = new Moralis.Web3Api.CloudApi.AccountApi(client);
                this.Defi = new Moralis.Web3Api.CloudApi.DefiApi(client);
                this.Native = new Moralis.Web3Api.CloudApi.NativeApi(client);
                this.Resolve = new Moralis.Web3Api.CloudApi.ResolveApi(client);
                this.Storage = new Moralis.Web3Api.CloudApi.StorageApi(client);
                this.Token = new Moralis.Web3Api.CloudApi.TokenApi(client);
            }

            // Indicate that the client is initialized.
            this.IsInitialized = true;
        }
    }
}
