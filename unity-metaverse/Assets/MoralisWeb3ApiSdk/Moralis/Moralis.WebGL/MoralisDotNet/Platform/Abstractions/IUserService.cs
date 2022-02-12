using System.Collections.Generic;
using System.Threading;
using Moralis.WebGL.Platform.Objects;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface IUserService<TUser> where TUser : MoralisUser
    {
        UniTask<TUser> SignUpAsync(IObjectState state, IDictionary<string, IMoralisFieldOperation> operations, CancellationToken cancellationToken = default);

        UniTask<TUser> LogInAsync(string username, string password, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default);

        UniTask<TUser> LogInAsync(string authType, IDictionary<string, object> data, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default);

        UniTask<TUser> GetUserAsync(string sessionToken, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default);

        UniTask RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);

        bool RevocableSessionEnabled { get; set; }

        object RevocableSessionEnabledMutex { get; }
    }

}
