using System;
using System.Collections.Generic;
using Moralis.WebGL.Platform.Abstractions;

namespace Moralis.WebGL.Platform
{
    /// <summary>
    /// Represents the configuration of the Moralis SDK.
    /// </summary>
    public struct ServerConnectionData : IServerConnectionData
    {
        public bool Test { get; set; }

        /// <summary>
        /// The App ID of your app.
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// A URI pointing to the target Moralis Server instance hosting the app targeted by <see cref="ApplicationID"/>.
        /// </summary>
        public string ServerURI { get; set; }

        /// <summary>
        /// A URI pointing to the target Moralis WS/WSS server.
        /// </summary>
        public string LiveQueryServerURI { get; set; }

        /// <summary>
        /// The Web3Api key, must be supplied to initialize Web3Api to use 
        /// standard REST server.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The .NET Key for the Moralis app targeted by <see cref="ServerURI"/>.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Master Key for the Moralis app targeted by <see cref="Key"/>.
        /// </summary>
        public string MasterKey { get; set; }

        /// <summary>
        /// Used to 
        /// </summary>
        public string LocalStoragePath { get; set; }

        /// <summary>
        /// Additional HTTP headers to be sent with network requests from the SDK.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }
    }

}
