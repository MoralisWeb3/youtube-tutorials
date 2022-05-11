namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Contract for objects able to send an HTTP request.
    /// </summary>
    interface IHttpClient
    {
        /// <summary>
        /// Get the base URL to reach the service identified by the given <paramref name="serviceId"/>.
        /// </summary>
        /// <param name="serviceId">
        /// The ID of the remote service to get the base URL for.
        /// </param>
        /// <returns>
        /// Return the base URL for the service if it exists;
        /// throw otherwise.
        /// </returns>
        string GetBaseUrlFor(string serviceId);

        /// <summary>
        /// Get the default options for requests targeting the service
        /// identified by the given <paramref name="serviceId"/>.
        /// </summary>
        /// <param name="serviceId">
        /// The ID of the remote service to get default options for.
        /// </param>
        /// <returns>
        /// Return the default options for requests targeting the service if it exists;
        /// throw otherwise.
        /// </returns>
        HttpOptions GetDefaultOptionsFor(string serviceId);

        /// <summary>
        /// Create a new <see cref="HttpRequest"/> targeting the service
        /// identified by the given <paramref name="serviceId"/>.
        /// Also set its default options.
        /// </summary>
        /// <param name="serviceId">
        /// The ID of the remote service to create a request for.
        /// </param>
        /// <param name="resourcePath">
        /// The path to the resource to act on.
        /// </param>
        /// <returns>
        /// Return the created <see cref="HttpRequest"/> if the service exists.
        /// </returns>
        HttpRequest CreateRequestForService(string serviceId, string resourcePath);

        /// <summary>
        /// Send the given <paramref name="request"/>.
        /// Note: The success of the returned operation only means that the request could be handled
        /// gracefully; the request in itself can still fail (HTTP error or network error).
        /// </summary>
        /// <param name="request">
        /// The request to send.
        /// </param>
        /// <returns>
        /// Return a handle to monitor the progression of the request.
        /// The operation's result will contain the server's response if the request was sent successfully.
        /// </returns>
        IAsyncOperation<ReadOnlyHttpResponse> Send(HttpRequest request);
    }
}
