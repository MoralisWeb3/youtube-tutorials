using System;
using System.Reflection;
using Moralis.Platform.Abstractions;

namespace Moralis.Platform
{
    /// <summary>
    /// In the event that you would like to use the Moralis SDK
    /// from a completely portable project, with no platform-specific library required,
    /// to get full access to all of our features available on Parse Dashboard
    /// (A/B testing, slow queries, etc.), you must set the values of this struct
    /// to be appropriate for your platform.
    ///
    /// Any values set here will overwrite those that are automatically configured by
    /// any platform-specific migration library your app includes.
    /// </summary>
    public class HostManifestData : IHostManifestData
    {
        /// <summary>
        /// An instance of <see cref="HostManifestData"/> with inferred values based on the entry assembly.
        /// </summary>
        /// <remarks>Should not be used with Unity.</remarks>
        public static HostManifestData Inferred => new HostManifestData
        {
            Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
            Name = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? Assembly.GetEntryAssembly().GetName().Name,
            ShortVersion = Assembly.GetEntryAssembly().GetName().Version.ToString(),
            // TODO: For Xamarin, use manifest parsing, and for Unity, use some kind of package identifier API.
            Identifier = AppDomain.CurrentDomain.FriendlyName
        };

        /// <summary>
        /// The build version of your app.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The human-friendly display version number of your app.
        /// </summary>
        public string ShortVersion { get; set; }

        /// <summary>
        /// The identifier of the application
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// The friendly name of your app.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a value for whether or not this instance of <see cref="HostManifestData"/> is populated with default values.
        /// </summary>
        public bool IsDefault => Version is null && ShortVersion is null && Identifier is null && Name is null;

        /// <summary>
        /// Gets a value for whether or not this instance of <see cref="HostManifestData"/> can currently be used for the generation of <see cref="MetadataBasedRelativeCacheLocationGenerator.Inferred"/>.
        /// </summary>
        public bool CanBeUsedForInference => !(IsDefault || String.IsNullOrWhiteSpace(ShortVersion));
    }

}
