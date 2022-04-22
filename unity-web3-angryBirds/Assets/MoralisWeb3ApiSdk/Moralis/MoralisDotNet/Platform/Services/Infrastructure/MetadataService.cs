using Moralis.Platform.Abstractions;

namespace Moralis.Platform.Services.Infrastructure
{
    public class MetadataService : IMetadataService
    {
        /// <summary>
        /// Information about your app.
        /// </summary>
        public IHostManifestData HostManifestData { get; set; }

        /// <summary>
        /// Information about the environment the library is operating in.
        /// </summary>
        public IEnvironmentData EnvironmentData { get; set; }
    }
}

