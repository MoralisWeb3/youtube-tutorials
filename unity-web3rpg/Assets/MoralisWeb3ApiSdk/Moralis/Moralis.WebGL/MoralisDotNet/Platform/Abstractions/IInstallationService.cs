using System;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface IInstallationService
    {
        /// <summary>
        /// The current installation Id.
        /// </summary>
        Guid? InstallationId { get; }

        /// <summary>
        /// Sets current <code>installationId</code> and saves it to local storage.
        /// </summary>
        /// <param name="installationId">The <code>installationId</code> to be saved.</param>
        UniTask SetAsync(Guid? installationId);

        /// <summary>
        /// Gets current <code>installationId</code> from local storage. Generates a none exists.
        /// </summary>
        /// <returns>Current <code>installationId</code>.</returns>
        UniTask<Guid?> GetAsync();

        /// <summary>
        /// Clears current installationId from memory and local storage.
        /// </summary>
        UniTask ClearAsync();
    }
}
