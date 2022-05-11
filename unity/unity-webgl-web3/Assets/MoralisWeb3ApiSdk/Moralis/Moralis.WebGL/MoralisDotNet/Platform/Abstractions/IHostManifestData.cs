

namespace Moralis.WebGL.Platform.Abstractions
{
    /// <summary>
    /// Information about the application using the Parse SDK.
    /// </summary>
    public interface IHostManifestData
    {
        /// <summary>
        /// The build number of your app.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// The human friendly version number of your app.
        /// </summary>
        string ShortVersion { get; }

        /// <summary>
        /// A unique string representing your app.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// The name of your app.
        /// </summary>
        string Name { get; }
    }

}
