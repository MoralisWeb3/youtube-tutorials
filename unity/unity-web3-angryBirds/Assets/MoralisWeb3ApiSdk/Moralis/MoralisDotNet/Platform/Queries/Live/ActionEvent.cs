using Moralis.Platform.Objects;

namespace Moralis.Platform.Queries.Live
{
    /// <summary>
    /// This action object is used for Connected, Create, Update, Enter, Delete, 
    /// Leave, Subscribed, and Unsubscribed events.
    /// Note: For subscribed and unscubscribed events, Object will always be null or default
    /// </summary>
    /// <typeparam name="T">Response object</typeparam>
    public class ActionEvent<T> : QueryEventMessage where T : MoralisObject
    {
        /// <summary>
        /// REQUIRED: Client generated 
        /// </summary>
        public int requestId { get; set; }

        /// <summary>
        /// OPTIONAL: Moralis current user session token.
        /// </summary>
        public T Object { get; set; }
    }
}
