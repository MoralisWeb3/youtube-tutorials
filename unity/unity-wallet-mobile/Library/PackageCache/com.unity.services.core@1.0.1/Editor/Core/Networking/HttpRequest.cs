using System.Collections.Generic;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Contain all data of an HTTP request.
    /// </summary>
    class HttpRequest
    {
        /// <summary>
        /// The HTTP method to use.
        /// </summary>
        /// <example>
        /// Common values are: DELETE, GET, PATCH, POST, PUT ...
        /// </example>
        public string Method;

        /// <summary>
        /// The targeted url.
        /// </summary>
        public string Url;

        /// <summary>
        /// The headers additional information.
        /// </summary>
        public Dictionary<string, string> Headers;

        /// <summary>
        /// The additional data of this request serialized into bytes.
        /// </summary>
        public byte[] Body;

        /// <summary>
        /// Settings to customize this request's behaviour.
        /// </summary>
        public HttpOptions Options;

        /// <summary>
        /// Initialize a new instance of the <see cref="HttpRequest"/> class.
        /// </summary>
        public HttpRequest() {}

        /// <summary>
        /// Initialize a new instance of the <see cref="HttpRequest"/> class with the given parameters.
        /// </summary>
        /// <param name="method">
        /// The HTTP method to use.
        /// Valid methods are: DELETE, GET, PATCH, POST, PUT.
        /// </param>
        /// <param name="url">
        /// The target url of the created request.
        /// </param>
        /// <param name="headers">
        /// The headers to add to the request.
        /// </param>
        /// <param name="body">
        /// The request's data serialized into bytes.
        /// </param>
        public HttpRequest(string method, string url, Dictionary<string, string> headers, byte[] body)
        {
            Method = method;
            Url = url;
            Headers = headers;
            Body = body;
        }

        /// <summary>
        /// Set the given <paramref name="method"/> to this request.
        /// </summary>
        /// <param name="method">
        /// The HTTP method to use.
        /// Valid methods are: DELETE, GET, PATCH, POST, PUT.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public HttpRequest SetMethod(string method)
        {
            Method = method;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="url"/> to this request.
        /// </summary>
        /// <param name="url">
        /// The target url of the created request.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public HttpRequest SetUrl(string url)
        {
            Url = url;

            return this;
        }

        /// <summary>
        /// Add or update a header to this request using the given
        /// <paramref name="key"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="key">
        /// The header's key.
        /// </param>
        /// <param name="value">
        /// The header's value.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public HttpRequest SetHeader(string key, string value)
        {
            if (Headers is null)
            {
                Headers = new Dictionary<string, string>(1);
            }

            Headers[key] = value;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="headers"/> to this request.
        /// </summary>
        /// <param name="headers">
        /// The headers to add to the request.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public HttpRequest SetHeaders(Dictionary<string, string> headers)
        {
            Headers = headers;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="body"/> to this request.
        /// </summary>
        /// <param name="body">
        /// The request's data serialized into bytes.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public HttpRequest SetBody(byte[] body)
        {
            Body = body;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="options"/> to this request.
        /// </summary>
        /// <param name="options">
        /// The settings to customize the request's behaviour.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public HttpRequest SetOptions(HttpOptions options)
        {
            Options = options;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="redirectLimit"/> to this request.
        /// </summary>
        /// <param name="redirectLimit">
        /// The number of redirects this request can follow without failing.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public HttpRequest SetRedirectLimit(int redirectLimit)
        {
            Options.RedirectLimit = redirectLimit;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="timeout"/> to this request.
        /// </summary>
        /// <param name="timeout">
        /// The delay, in seconds, after which the request will
        /// be considered a failure if it didn't have a response.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public HttpRequest SetTimeOutInSeconds(int timeout)
        {
            Options.RequestTimeoutInSeconds = timeout;

            return this;
        }
    }
}
