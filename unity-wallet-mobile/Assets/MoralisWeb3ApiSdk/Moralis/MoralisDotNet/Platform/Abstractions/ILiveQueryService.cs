using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moralis.Platform.Abstractions
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
    }
}
