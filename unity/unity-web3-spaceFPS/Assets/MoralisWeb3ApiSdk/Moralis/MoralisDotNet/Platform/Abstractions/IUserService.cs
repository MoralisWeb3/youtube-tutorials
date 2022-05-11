using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Objects;

namespace Moralis.Platform.Abstractions
{
    public interface IUserService<TUser> where TUser : MoralisUser
    {
        Task<TUser> SignUpAsync(IObjectState state, IDictionary<string, IMoralisFieldOperation> operations, CancellationToken cancellationToken = default);

        Task<TUser> LogInAsync(string username, string password, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default);

        Task<TUser> LogInAsync(string authType, IDictionary<string, object> data, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default);

        Task<TUser> GetUserAsync(string sessionToken, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default);

        Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);

        bool RevocableSessionEnabled { get; set; }

        object RevocableSessionEnabledMutex { get; }
    }

}
