using Moralis.Platform.Queries.Live;
using Moralis.Platform.Objects;

namespace Moralis.Platform.Abstractions
{
    public class LiveQueryCallbacks<T> : ILiveQueryCallbacks<T> where T : MoralisObject
    {
        /// <summary>
        /// Called when a connected event is returned from the server.
        /// </summary>
        public virtual void OnConnected() { }

        /// <summary>
        /// Called when a subscribed event is returned from the server.
        /// </summary>
        public virtual void OnSubscribed(int requestId) { }

        /// <summary>
        /// Called when a create event is returned from the server.
        /// </summary>
        public virtual void OnCreate(T item, int requestId) { }

        /// <summary>
        /// Called when a delete event is returned from the server.
        /// </summary>
        public virtual void OnDelete(T item, int requestId) { }

        /// <summary>
        /// Called when a enter event is returned from the server.
        /// </summary>
        public virtual void OnEnter(T item, int requestId) { }

        /// <summary>
        /// Called when a leave event is returned from the server.
        /// </summary>
        public virtual void OnLeave(T item, int requestId) { }

        /// <summary>
        /// Called when a update event is returned from the server.
        /// </summary>
        public virtual void OnUpdate(T item, int requestId) { }

        /// <summary>
        /// Called when a ubsubscribed event is received.
        /// </summary>
        public virtual void OnUnsubscribed(int requestId) { }

        /// <summary>
        /// Called when an error message is recevied.
        /// </summary>
        /// <param name="em"></param>
        public virtual void OnError(ErrorMessage em) { }

        public virtual void OnGeneralMessage(string text) { }
    }
}
