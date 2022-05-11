
namespace Moralis.Platform.Abstractions
{
    /// <summary>
    /// Information about the environment in which the library will be operating.
    /// </summary>
    public interface IEnvironmentData
    {
        /// <summary>
        /// The currently active time zone when the library will be used.
        /// </summary>
        string TimeZone { get; }

        /// <summary>
        /// The operating system version of the platform the SDK is operating in.
        /// </summary>
        string OSVersion { get; }

        /// <summary>
        /// An identifier of the platform.
        /// </summary>
        /// <remarks>Expected to be one of ios, android, winrt, winphone, or dotnet.</remarks>
        public string Platform { get; set; }
    }
}
