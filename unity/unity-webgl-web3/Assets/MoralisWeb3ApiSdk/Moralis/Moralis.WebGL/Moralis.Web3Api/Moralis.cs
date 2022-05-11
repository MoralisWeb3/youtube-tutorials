using System;
using Moralis.WebGL.Web3Api.Client;

namespace Moralis.WebGL.Web3Api
{
    /// <summary>
    /// Provides an easy to wrapper around the Moralis Web3Api REST services.
    /// </summary>
    public class MoralisClient
    {
        Web3ApiClient client = new Web3ApiClient();

        static MoralisClient instance = new MoralisClient();

        private MoralisClient() { }

        /// <summary>
        /// Initialize Moralis Web3API. Use this to initialize to your personal 
        /// Moralis server. Major advantage is api key is supplied 
        /// </summary>
        /// <param name="url"></param>
        public static void Initialize(string url) => instance.client.Initialize(url);

        /// <summary>
        /// Initialize Moralis Web3API. 
        /// </summary>
        /// <param name="useStandardServer">If true enforces use of the standard REST server</param>
        /// <param name="apiKey">Required if useStandardServer is true</param>
        /// <param name="url">Optional server url. If not provided default standard server Url is used.</param>
        public static void Initialize(bool useStandardServer, string apiKey = null, string url = null)
        {
            if (useStandardServer && !(apiKey is { })) throw new ArgumentException("API Key is required for Standard REST server.");

            if (apiKey is { }) Configuration.ApiKey["X-API-Key"] = apiKey;
            instance.client.Initialize(url);
        }

        /// <summary>
        /// Gets the Web3ApiClient instance. Moralis.Initialize must be called first.
        /// If Moralis is not initialized this will throw an ApiException.
        /// </summary>
        /// <exception cref="ApiException">Thrown when Moralis.Initialize has not been called.</exception>
        public static Web3ApiClient Web3Api
        {
            get => instance.client.IsInitialized ? instance.client : throw new ApiException(109, "Moralis must be initialized before use.");
        }
    }
}
