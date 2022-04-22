using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// Authenticates with the service.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        UniTask<IDictionary<string, object>> AuthenticateAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Deauthenticates (logs out) the user associated with this provider. This
        /// call may block.
        /// </summary>
        void Deauthenticate();

        /// <summary>
        /// Restores authentication that has been serialized, such as session keys,
        /// etc.
        /// </summary>
        /// <param name="authData">The auth data for the provider. This value may be null
        /// when unlinking an account.</param>
        /// <returns><c>true</c> iff the authData was successfully synchronized. A <c>false</c> return
        /// value indicates that the user should no longer be associated because of bad auth
        /// data.</returns>
        bool RestoreAuthentication(IDictionary<string, object> authData);

        /// <summary>
        /// Provides a unique name for the type of authentication the provider does.
        /// For example, the FacebookAuthenticationProvider would return "facebook".
        /// </summary>
        string AuthType { get; }
    }
}
