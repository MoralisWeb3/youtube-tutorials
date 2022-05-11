
namespace Moralis.Platform.Queries.Live
{
    /// <summary>
    /// Defines the base live query request / event message object.
    /// </summary>
    public class QueryEventMessage
    {
        /// <summary>
        /// REQUIRED: The operation being requested or event reported.
        /// </summary>
        public string op { get; set; }
    }
}
