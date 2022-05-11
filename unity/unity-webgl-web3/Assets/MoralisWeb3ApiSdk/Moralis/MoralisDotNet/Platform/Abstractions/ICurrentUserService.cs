using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Objects;

namespace Moralis.Platform.Abstractions
{
    public interface ICurrentUserService<TUser> : ICurrentObjectService<TUser, TUser> where TUser : MoralisUser
    {
        TUser CurrentUser { get; set; }

        Task<string> GetCurrentSessionTokenAsync(IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default);

        Task LogOutAsync(IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default);
    }
}
