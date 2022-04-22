using System;
using Moralis.Platform.Objects;
using Moralis.Platform.Queries.Live;

namespace Moralis.Platform.Abstractions
{
    public interface ILiveQueryClient : IDisposable
    {
        /// <summary>
        /// Called when a connected event is returned from the server.
        /// </summary>
        event LiveQueryConnectedHandler OnConnected;

        /// <summary>
        /// Called when a subscribed event is returned from the server.
        /// </summary>
        event LiveQuerySubscribedHandler OnSubscribed;

        /// <summary>
        /// Called when a ubsubscribed event is received.
        /// </summary>
        event LiveQueryUnsubscribedHandler OnUnsubscribed;

        /// <summary>
        /// Called when an ErrorMEssage is received.
        /// </summary>
        event LiveQueryErrorHandler OnError;

        /// <summary>
        /// Indicates the current status of this client.
        /// </summary>
        LiveQueryClientStatusTypes ClientStatus { get; set; }

        /// <summary>
        /// Server connection settings.
        /// </summary>
        IServerConnectionData ConncetionData { get; set; }

        string InstallationId { get; set; }
        /// <summary>
        /// Request Id used for this subscription
        /// </summary>
        int RequestId { get; }
        /// <summary>
        /// User session token.
        /// </summary>
        string SessionToken { get; set; }

        /// <summary>
        /// Unsubscribe from the live query server.
        /// </summary>
        void Unsubscribe();

        /// <summary>
        ///  Create a query subscription to the Live Query server.
        /// </summary>
        void Subscribe();
    }
}
