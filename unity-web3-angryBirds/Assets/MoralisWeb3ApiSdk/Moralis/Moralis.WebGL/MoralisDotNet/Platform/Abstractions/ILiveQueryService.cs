using System;
using System.Collections.Generic;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface ILiveQueryService : IDisposable
    {
        /// <summary>
        /// Adds a live query subscription to the service
        /// </summary>
        /// <param name="client"></param>
        void AddSubscription(ILiveQueryClient client);

        /// <summary>
        /// Returns all active subscriptions.
        /// </summary>
        /// <returns>IEnumerable<ILiveQueryClient></returns>
        IEnumerable<ILiveQueryClient> Subscriptions();

        /// <summary>
        /// Issues unsubscribe message to all subscriptions that are currently subscribed.
        /// </summary>
        /// <returns>Task</returns>
        void UnsubscribeAll();

        /// <summary>
        /// For WebGL Only should be called on Unity Update.
        /// </summary>
        void HandleUpdateEvent();
    }
}
