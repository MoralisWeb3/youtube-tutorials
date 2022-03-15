
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;
using Moralis.Platform.Queries;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.ClientServices
{
    public static class SessionServiceExtensions
    {
        /// <summary>
        /// Constructs a <see cref="ParseQuery{ParseSession}"/> for ParseSession.
        /// </summary>
        public static MoralisQuery<MoralisSession> GetSessionQuery<TUser>(this IServiceHub<TUser> serviceHub) where TUser : MoralisUser => serviceHub.GetQuery<MoralisSession, TUser>();

        /// <summary>
        /// Gets the current <see cref="ParseSession"/> object related to the current user.
        /// </summary>
        public static Task<MoralisSession> GetCurrentSessionAsync<TUser>(this IServiceHub<TUser> serviceHub) where TUser : MoralisUser => GetCurrentSessionAsync(serviceHub, CancellationToken.None);

        /// <summary>
        /// Gets the current <see cref="ParseSession"/> object related to the current user.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        public static Task<MoralisSession> GetCurrentSessionAsync<TUser>(this IServiceHub<TUser> serviceHub, CancellationToken cancellationToken) where TUser : MoralisUser => serviceHub.GetCurrentSessionAsync<TUser>(cancellationToken).OnSuccess(task => task.Result switch
        {
            null => Task.FromResult<MoralisSession>(default),
            { sessionToken: null } => Task.FromResult<MoralisSession>(default),
            { sessionToken: { } sessionToken } => serviceHub.SessionService.GetSessionAsync(sessionToken, serviceHub, cancellationToken).OnSuccess(successTask => successTask.Result)
        }).Unwrap();

        public static Task RevokeSessionAsync<TUser>(this IServiceHub<TUser> serviceHub, string sessionToken, CancellationToken cancellationToken) where TUser : MoralisUser => sessionToken is null || !serviceHub.SessionService.IsRevocableSessionToken(sessionToken) ? Task.CompletedTask : serviceHub.SessionService.RevokeAsync(sessionToken, cancellationToken);

        public static Task<string> UpgradeToRevocableSessionAsync<TUser>(this IServiceHub<TUser> serviceHub, string sessionToken, CancellationToken cancellationToken) where TUser : MoralisUser => sessionToken is null || serviceHub.SessionService.IsRevocableSessionToken(sessionToken) ? Task.FromResult(sessionToken) : serviceHub.SessionService.UpgradeToRevocableSessionAsync(sessionToken, serviceHub, cancellationToken).OnSuccess(task => task.Result.sessionToken);
    }
}
