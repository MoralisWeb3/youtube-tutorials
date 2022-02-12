using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Services.Infrastructure;
using Moralis.WebGL.Platform.Objects;

namespace Moralis.WebGL.Platform.Services
{
    /// <summary>
    /// An <see cref="IServiceHubMutator"/> for the relative cache file location. This should be used if the relative cache file location is not created correctly by the SDK, such as platforms on which it is not possible to gather metadata about the client assembly, or ones on which <see cref="System.Environment.SpecialFolder.LocalApplicationData"/> is inaccsessible.
    /// </summary>
    public class RelativeCacheLocationMutator : IServiceHubMutator
    {
        /// <summary>
        /// An <see cref="IRelativeCacheLocationGenerator"/> implementation instance which creates a path that should be used as the <see cref="System.Environment.SpecialFolder.LocalApplicationData"/>-relative cache location.
        /// </summary>
        public IRelativeCacheLocationGenerator RelativeCacheLocationGenerator { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool Valid => RelativeCacheLocationGenerator is { };

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="target"><inheritdoc/></param>
        /// <param name="referenceHub"><inheritdoc/></param>
        public void Mutate<TUser>(ref IMutableServiceHub<TUser> target, in IServiceHub<TUser> referenceHub) where TUser : MoralisUser => target.CacheService = (target as IServiceHub<TUser>).CacheService switch
        {
            null => new MoralisCacheService<TUser> { RelativeCacheFilePath = RelativeCacheLocationGenerator.GetRelativeCacheFilePath(referenceHub) },
            IDiskFileCacheService { } controller => (Controller: controller, controller.RelativeCacheFilePath = RelativeCacheLocationGenerator.GetRelativeCacheFilePath(referenceHub)).Controller,
            { } controller => controller
        };
    }

}
