using System.Threading;
using Moralis.WebGL.Platform.Objects;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface ICurrentUserService<TUser> : ICurrentObjectService<TUser, TUser> where TUser : MoralisUser
    {
        TUser CurrentUser { get; set; }

        UniTask<string> GetCurrentSessionTokenAsync(IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default);

        UniTask LogOutAsync(IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default);
    }
}
