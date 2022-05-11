
namespace Moralis.Platform.Queries.Live
{
    /// <summary>
    /// Define valid Live Query operations.
    /// </summary>
    public enum OperationTypes
    {
        /// <summary>
        /// Request Operation: The connect message is sent from a client to the LiveQuery 
        ///  server. It should be the first message sent from a client after the WebSocket 
        /// connection is established.
        /// </summary>
        connect,
        /// <summary>
        /// Event Operation: Received when a connection to the Live Query server is successful.
        /// </summary>
        connected,
        /// <summary>
        /// Request Operation: Used to subscribe to updates based on a query.
        /// </summary>
        subscribe,
        /// <summary>
        /// Event Operation: If a subscription is successful the server will respond 
        /// with subscribed.
        /// </summary>
        subscribed,
        /// <summary>
        /// Event Operation: A new object that meets the requirements of the query has 
        /// been created.
        /// </summary>
        create,
        /// <summary>
        /// Event Operation: Enter is returned by the server when a previous object that
        /// did not meet requirements of the query now does.
        /// </summary>
        enter,
        /// <summary>
        /// Event Operation: Sent when an object that meets the requirements of the query 
        /// is updated.
        /// </summary>
        update,
        /// <summary>
        /// Event Operation: Sent when an object changes so that it no longer meets the 
        /// requirements of the query.
        /// </summary>
        leave,
        /// <summary>
        /// Event Operation: Sent when an object that meets the requirements of the query 
        /// is deleted.
        /// </summary>
        delete,
        /// <summary>
        /// Request Operation: Client should send this request to server when the client no 
        /// longer wants to receive events related to query.
        /// </summary>
        unsubscribe,
        /// <summary>
        /// Event operation: Sent to a client when unsubcribe is successful.
        /// </summary>
        unsubscribed,
        /// <summary>
        /// Event operation: Sent when a client request cannot be handled by the server
        /// </summary>
        error
    };
}

