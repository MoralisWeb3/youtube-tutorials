
namespace Moralis.WebGL.Platform.Queries.Live
{
    /// <summary>
    /// The connect request is sent from a client to the LiveQuery server. It 
    /// should be the first message sent from a client after the WebSocket 
    /// connection is established
    /// </summary>
    public class ConnectRequest : QueryEventMessage
    {
        /// <summary>
        /// REQUIRED: Moralis Application Id
        /// </summary>
        public string applicationId { get; set; }

        /// <summary>
        /// OPTIONAL: For future use.
        /// </summary>
         public string restApiKey { get; set; }

        /// <summary>
        /// OPTIONAL:  For future use.
        /// </summary>
        public string javascriptKey { get; set; }

        /// <summary>
        /// OPTIONAL:  For future use.
        /// </summary>
        public string clientKey { get; set; }

        /// <summary>
        /// OPTIONAL:  For future use.
        /// </summary>
        public string windowsKey { get; set; }

        /// <summary>
        /// OPTIONAL: Moralis Master Key - CAUTION should not be used from client 
        /// application. If an operation requires a master key it is recommended 
        /// that that operation be called via a Cloud Function.
        /// </summary>
        public string masterKey { get; set; }

        /// <summary>
        /// OPTIONAL: Moralis current user session token.
        /// </summary>
        public string sessionToken { get; set; }

        /// <summary>
        /// OPTIONAL: Moralis Instanllation Id.
        /// </summary>
        public string installationId { get; set; }

        public ConnectRequest()
        {
            op = OperationTypes.connect.ToString();
        }
    }
}
