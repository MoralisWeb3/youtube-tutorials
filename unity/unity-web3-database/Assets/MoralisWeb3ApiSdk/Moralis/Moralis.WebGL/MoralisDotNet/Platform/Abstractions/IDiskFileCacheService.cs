using System;

namespace Moralis.WebGL.Platform.Abstractions
{
    /// <summary>
    /// An <see cref="ICacheService"/> which stores the cache on disk via a file.
    /// </summary>
    public interface IDiskFileCacheService : ICacheService
    {
        /// <summary>
        /// The path to a persistent user-specific storage location specific to the final client assembly of the Moralis library.
        /// </summary>
        public string AbsoluteCacheFilePath { get; set; }

        /// <summary>
        /// The relative path from the <see cref="Environment.SpecialFolder.LocalApplicationData"/> on the device an to application-specific persistent storage folder.
        /// </summary>
        public string RelativeCacheFilePath { get; set; }

        /// <summary>
        /// Refreshes this cache controller's internal tracked cache file to reflect the <see cref="AbsoluteCacheFilePath"/> and/or <see cref="RelativeCacheFilePath"/>.
        /// </summary>
        /// <remarks>This will not delete the active tracked cache file that will be un-tracked after a call to this method. To do so, call <see cref="ICacheService.Clear()"/>.</remarks>
        void RefreshPaths();
    }

}
