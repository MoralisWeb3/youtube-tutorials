using Moralis.Platform.Objects;

namespace Moralis.Platform.Queries.Live
{
    /// <summary>
    /// Delegate to define a event to fire when a connection to the live query 
    /// server has been established.
    /// </summary>
    public delegate void LiveQueryConnectedHandler();

    /// <summary>
    /// Delegate to define a event to fire when a substribtion to the live 
    /// query server has been established.
    /// </summary>
    /// <param name="requestId"></param>
    public delegate void LiveQuerySubscribedHandler(int requestId);

    /// <summary>
    /// Delegate to define an event to fire when a live query action has
    /// occurred.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="requestId"></param>
    public delegate void LiveQueryActionHandler<T>(T item, int requestId) where T : MoralisObject;

    /// <summary>
    /// A delgate to define an event to fire when a client has successfully 
    /// unsubscribed from the live query server.
    /// </summary>
    /// <param name="requestId"></param>
    public delegate void LiveQueryUnsubscribedHandler(int requestId);

    /// <summary>
    /// Provides a delegate for Moralis Live Query error messages.
    /// </summary>
    /// <param name="evt"></param>
    public delegate void LiveQueryErrorHandler(ErrorMessage evt);

    /// <summary>
    /// A delegate the client uses to handle an event when a message is received 
    /// from the live query server.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="bufferSize"></param>
    public delegate void LiveQueryMessageHandler(byte[] buffer, int bufferSize);

    public delegate void LiveQueryGeneralMessageHandler(string text);
}
