
using Moralis.Platform.Objects;

namespace Moralis.Platform.Abstractions
{
    public interface ICustomServiceHub<TUser> : IServiceHub<TUser> where TUser : MoralisUser
    {
        IServiceHub<TUser> Services { get; }
    }
}
