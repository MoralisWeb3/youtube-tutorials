using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;
using Moralis.Platform.Services.ClientServices;

namespace Moralis.Platform.Queries
{
    public static class QueryExtensions
    {
        public static MoralisLiveQueryClient<T> Subscribe<T>(this MoralisQuery<T> query, ILiveQueryCallbacks<T> callbacks = null) where T : MoralisObject
        {
            if (!(callbacks is { }))
            {
                callbacks = new LiveQueryCallbacks<T>();
            }

            string sessionToken = query.SessionToken;
            string installationId = query.InstallationService.InstallationId?.ToString();
            
            MoralisLiveQueryClient<T>  subscription = 
                MoralisLiveQueryManager.CreateSubscription<T>(query, query.ServerConnectionData, callbacks, query.JsonSerializer, sessionToken, installationId);

            return subscription;
        }
    }
}
