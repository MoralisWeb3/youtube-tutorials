
using Moralis.WebGL.Platform.Objects;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface ICustomServiceHub<TUser> : IServiceHub<TUser> where TUser : MoralisUser
    {
        IServiceHub<TUser> Services { get; }
    }
}
