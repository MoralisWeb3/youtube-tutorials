
using System.Threading;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Platform.Queries;
using Moralis.WebGL.Platform.Utilities;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Services.ClientServices
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
        public static async UniTask<MoralisSession> GetCurrentSessionAsync<TUser>(this IServiceHub<TUser> serviceHub) where TUser : MoralisUser
        {
            return await GetCurrentSessionAsync(serviceHub, CancellationToken.None);
        }

        /// <summary>
        /// Gets the current <see cref="ParseSession"/> object related to the current user.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        public static async UniTask<MoralisSession> GetCurrentSessionAsync<TUser>(this IServiceHub<TUser> serviceHub, CancellationToken cancellationToken) where TUser : MoralisUser
        {
            MoralisSession ms = await serviceHub.GetCurrentSessionAsync<TUser>(cancellationToken);

            if (ms == null) return default;

            string token = ms.GetCurrentSessionToken();

            if (token == null) return default;

            return await serviceHub.SessionService.GetSessionAsync(token, serviceHub, cancellationToken);
        }

        public static async UniTask RevokeSessionAsync<TUser>(this IServiceHub<TUser> serviceHub, string sessionToken, CancellationToken cancellationToken) where TUser : MoralisUser
        {
            if (sessionToken != null && serviceHub.SessionService.IsRevocableSessionToken(sessionToken))
                await serviceHub.SessionService.RevokeAsync(sessionToken, cancellationToken);
        }

        public static async UniTask<string> UpgradeToRevocableSessionAsync<TUser>(this IServiceHub<TUser> serviceHub, string sessionToken, CancellationToken cancellationToken) where TUser : MoralisUser
        {
            if (sessionToken is null || serviceHub.SessionService.IsRevocableSessionToken(sessionToken))
                return sessionToken;
            else
            {
                MoralisSession ms = await serviceHub.SessionService.UpgradeToRevocableSessionAsync(sessionToken, serviceHub, cancellationToken);

                return ms.sessionToken;
            }
        }
    }
}
