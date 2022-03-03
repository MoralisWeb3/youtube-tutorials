
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Services.Infrastructure;
using Moralis.WebGL.Platform.Objects;

namespace Moralis.WebGL.Platform.Services
{
    /// <summary>
    /// An <see cref="IServiceHubMutator"/> implementation which changes the <see cref="IServiceHub.CacheController"/>'s <see cref="IDiskFileCacheService.AbsoluteCacheFilePath"/> if available.
    /// </summary>
    public class AbsoluteCacheLocationMutator<TUser> : IServiceHubMutator
    {
        /// <summary>
        /// A custom absolute cache file path to be set on the active <see cref="IServiceHub.CacheController"/> if it implements <see cref="IDiskFileCacheService"/>.
        /// </summary>
        public string CustomAbsoluteCacheFilePath { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool Valid => CustomAbsoluteCacheFilePath is { };

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="target"><inheritdoc/></param>
        /// <param name="composedHub"><inheritdoc/></param>
        public void Mutate<TUser>(ref IMutableServiceHub<TUser> target, in IServiceHub<TUser> composedHub) where TUser : MoralisUser
        {
            if ((target as IServiceHub<TUser>).CacheService is IDiskFileCacheService { } diskFileCacheController)
            {
                diskFileCacheController.AbsoluteCacheFilePath = CustomAbsoluteCacheFilePath;
                diskFileCacheController.RefreshPaths();
            }
        }
    }

}
