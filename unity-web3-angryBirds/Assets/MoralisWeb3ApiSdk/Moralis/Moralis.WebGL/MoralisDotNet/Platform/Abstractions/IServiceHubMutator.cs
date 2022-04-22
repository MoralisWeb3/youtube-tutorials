using Moralis.WebGL.Platform.Objects;

namespace Moralis.WebGL.Platform.Abstractions
{
    /// <summary>
    /// A class which makes a deliberate mutation to a service.
    /// </summary>
    public interface IServiceHubMutator
    {
        /// <summary>
        /// A value which dictates whether or not the <see cref="IServiceHubMutator"/> should be considered in a valid state.
        /// </summary>
        bool Valid { get; }

        /// <summary>
        /// A method which mutates an <see cref="IMutableServiceHub"/> implementation instance.
        /// </summary>
        /// <param name="target">The target <see cref="IMutableServiceHub"/> implementation instance</param>
        /// <param name="composedHub">A hub which the <paramref name="target"/> is composed onto that should be used when <see cref="Mutate(ref IMutableServiceHub, in IServiceHub)"/> needs to access services.</param>
        void Mutate<TUser>(ref IMutableServiceHub<TUser> target, in IServiceHub<TUser> composedHub) where TUser : MoralisUser;
    }
}
