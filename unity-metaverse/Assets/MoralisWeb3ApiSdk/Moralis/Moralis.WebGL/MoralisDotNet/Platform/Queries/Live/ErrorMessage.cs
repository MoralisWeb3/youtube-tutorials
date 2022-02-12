
namespace Moralis.WebGL.Platform.Queries.Live
{
    public class ErrorMessage : QueryEventMessage
    {
        public ErrorMessage() => op = OperationTypes.error.ToString();

        /// <summary>
        /// REQUIRED: Client generated 
        /// </summary>
        public int? requestId { get; set; }

        /// <summary>
        /// A number which represents the type of the error.
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string error { get; set; }

        /// <summary>
        /// Indicates whether a client can reconnect to the LiveQuery server after this error. 
        /// </summary>
        public bool reconnect { get; set; }
    }
}
