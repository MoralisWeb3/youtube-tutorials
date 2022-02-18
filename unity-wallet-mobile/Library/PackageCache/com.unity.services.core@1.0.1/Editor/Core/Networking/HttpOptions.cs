namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Contract for objects containing all settings to customize the behaviour of a HTTP request sending.
    /// </summary>
    /// <remarks>
    /// More options will be added based on common needs.
    /// </remarks>
    struct HttpOptions
    {
        /// <summary>
        /// Delay, in seconds, after which the request will be considered a failure.
        /// </summary>
        public int RequestTimeoutInSeconds;

        /// <summary>
        /// Indicates the number of redirects the request can follow without failing.
        /// </summary>
        public int RedirectLimit;
    }
}
