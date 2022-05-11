using System.Collections.Generic;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Read-only handle to a <see cref="HttpResponse"/>.
    /// </summary>
    struct ReadOnlyHttpResponse
    {
        HttpResponse m_Response;

        /// <summary>
        /// Create a read-only handle to the given <paramref name="response"/>.
        /// </summary>
        /// <param name="response">
        /// The response to create the read-only handle for.
        /// </param>
        public ReadOnlyHttpResponse(HttpResponse response)
        {
            m_Response = response;
        }

        /// <inheritdoc cref="HttpResponse.Request"/>
        public ReadOnlyHttpRequest Request => m_Response.Request;

        /// <inheritdoc cref="HttpResponse.Headers"/>
        public IReadOnlyDictionary<string, string> Headers => m_Response.Headers;

        /// <inheritdoc cref="HttpResponse.Data"/>
        public byte[] Data => m_Response.Data;

        /// <inheritdoc cref="HttpResponse.StatusCode"/>
        public long StatusCode => m_Response.StatusCode;

        /// <inheritdoc cref="HttpResponse.ErrorMessage"/>
        /// <remarks>
        /// Can be filled by the server for HTTP errors or by the local request manager for network errors.
        /// </remarks>
        public string ErrorMessage => m_Response.ErrorMessage;

        /// <inheritdoc cref="HttpResponse.IsHttpError"/>
        public bool IsHttpError => m_Response.IsHttpError;

        /// <inheritdoc cref="HttpResponse.IsNetworkError"/>
        public bool IsNetworkError => m_Response.IsNetworkError;
    }
}
