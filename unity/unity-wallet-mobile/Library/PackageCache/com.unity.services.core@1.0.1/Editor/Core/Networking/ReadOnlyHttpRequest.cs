using System.Collections.Generic;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Read-only handle to a <see cref="HttpRequest"/>.
    /// </summary>
    struct ReadOnlyHttpRequest
    {
        HttpRequest m_Request;

        /// <summary>
        /// Create a read-only handle to the given <paramref name="request"/>.
        /// </summary>
        /// <param name="request">
        /// The request to create the read-only handle for.
        /// </param>
        public ReadOnlyHttpRequest(HttpRequest request)
        {
            m_Request = request;
        }

        /// <inheritdoc cref="HttpRequest.Method"/>
        public string Method => m_Request.Method;

        /// <inheritdoc cref="HttpRequest.Url"/>
        public string Url => m_Request.Url;

        /// <inheritdoc cref="HttpRequest.Headers"/>
        public IReadOnlyDictionary<string, string> Headers => m_Request.Headers;

        /// <inheritdoc cref="HttpRequest.Body"/>
        public byte[] Body => m_Request.Body;
    }
}
