using Moralis.Platform.Objects;

namespace Moralis.Platform.Abstractions
{
    public interface IServiceHubComposer<TUser> where TUser : MoralisUser
    {
        public IServiceHub<TUser> BuildHub(IMutableServiceHub<TUser> serviceHub = default, IServiceHub<TUser> extension = default, params IServiceHubMutator[] configurators) ;
    }
}
