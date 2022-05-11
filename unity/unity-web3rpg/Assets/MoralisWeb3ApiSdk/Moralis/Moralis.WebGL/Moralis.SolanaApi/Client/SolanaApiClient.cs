using Moralis.WebGL.SolanaApi.Interfaces;

namespace Moralis.WebGL.SolanaApi.Client
{
    /// <summary>
    /// Provides a wrapper class around the Moralis Web3Api REST service. 
    /// Automagically initializes client to use standard server or
    /// personal server Cloud Function API based on use of Api Key.
    /// </summary>
    public class SolanaApiClient : ISolanaApi
    {
        private static string defaultServerUrl = "https://deep-index.moralis.io/api/v2";

        /// <summary>
        /// AccountApi operations.
        /// </summary>
        public IAccountApi Account { get; private set; }

        /// <summary>
        /// DefiApi operations
        /// </summary>
        public INftApi Nft { get; private set; }

        /// <summary>
        /// Indicates that the client has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        // Default constructor.
        public SolanaApiClient() { }

        /// <summary>
        /// Initialize client using just server url.
        /// </summary>
        /// <param name="serverUrl"></param>
        public SolanaApiClient(string serverUrl)
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
                this.Account = new Moralis.WebGL.SolanaApi.Api.AccountApi(client);
                this.Nft = new Moralis.WebGL.SolanaApi.Api.NftApi(client);
            }
            // Api key not set assume the url is for moralis personal server
            // and Cloud Function API should be used.
            else
            {
                this.Account = new Moralis.WebGL.SolanaApi.CloudApi.AccountApi(client);
                this.Nft = new Moralis.WebGL.SolanaApi.CloudApi.NftApi(client);
            }

            // Indicate that the client is initialized.
            this.IsInitialized = true;
        }
    }
}
