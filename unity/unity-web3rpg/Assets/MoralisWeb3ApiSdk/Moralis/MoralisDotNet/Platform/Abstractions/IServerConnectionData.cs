using System.Collections.Generic;

namespace Moralis.Platform.Abstractions
{
    public interface IServerConnectionData
    {
        /// <summary>
        /// The App ID of your app.
        /// </summary>
        string ApplicationID { get; set; }

        /// <summary>
        /// A URI pointing to the target Moralis Server instance hosting the app targeted by <see cref="ApplicationID"/>.
        /// </summary>
        string ServerURI { get; set; }

        /// <summary>
        /// A URI pointing to the target Moralis WS/WSS server.
        /// </summary>
        string LiveQueryServerURI { get; set; }

        /// <summary>
        /// The Web3Api key, must be supplied to initialize Web3Api to use 
        /// standard REST server.
        /// </summary>
        string ApiKey { get; set; }

        /// <summary>
        /// The .NET Key for the Moralis app targeted by <see cref="ServerURI"/>.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// The Master Key for the Moralis app targeted by <see cref="Key"/>.
        /// </summary>
        string MasterKey { get; set; }

        /// <summary>
        /// Used to 
        /// </summary>
        public string LocalStoragePath { get; set; }

        /// <summary>
        /// Additional HTTP headers to be sent with network requests from the SDK.
        /// </summary>
        IDictionary<string, string> Headers { get; set; }
    }
}
