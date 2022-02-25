using Moralis.WebGL.Platform.Queries.Live;
using Moralis.WebGL.Platform.Objects;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface ILiveQueryCallbacks<T> where T : MoralisObject
    {
        /// <summary>
        /// Called when a connected event is returned from the server.
        /// </summary>
        void OnConnected ();

        /// <summary>
        /// Called when a subscribed event is returned from the server.
        /// </summary>
        void OnSubscribed (int requestId);

        /// <summary>
        /// Called when a create event is returned from the server.
        /// </summary>
        void OnCreate(T item, int requestId);

        /// <summary>
        /// Called when a delete event is returned from the server.
        /// </summary>
        void OnDelete(T item, int requestId);

        /// <summary>
        /// Called when a enter event is returned from the server.
        /// </summary>
        void OnEnter(T item, int requestId);

        /// <summary>
        /// Called when a leave event is returned from the server.
        /// </summary>
        void OnLeave(T item, int requestId);

        /// <summary>
        /// Called when a update event is returned from the server.
        /// </summary>
        void OnUpdate(T item, int requestId);

        /// <summary>
        /// Called when a ubsubscribed event is received.
        /// </summary>
        void OnUnsubscribed (int requestId);

        /// <summary>
        /// Called when an error message is recevied.
        /// </summary>
        /// <param name="em"></param>
        void OnError(ErrorMessage em);

        /// <summary>
        /// Called when a general message is reported by the client.
        /// </summary>
        /// <param name="text"></param>
        void OnGeneralMessage(string text);
    }
}
