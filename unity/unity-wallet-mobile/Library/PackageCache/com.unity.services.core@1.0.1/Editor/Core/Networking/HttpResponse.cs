using System.Collections.Generic;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Contain all data from a server response to a <see cref="HttpRequest"/>.
    /// </summary>
    class HttpResponse
    {
        /// <summary>
        /// The request at the origin of this response.
        /// </summary>
        public ReadOnlyHttpRequest Request;

        /// <summary>
        /// The headers additional information.
        /// </summary>
        public Dictionary<string, string> Headers;

        /// <summary>
        /// The date sent by the server serialized into bytes.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// The status code sent by the server.
        /// </summary>
        public long StatusCode;

        /// <summary>
        /// The error message if an error occured.
        /// </summary>
        public string ErrorMessage;

        /// <summary>
        /// If true, the request failed due to an HTTP error.
        /// </summary>
        public bool IsHttpError;

        /// <summary>
        /// If true, the request failed due to a connectivity error.
        /// </summary>
        public bool IsNetworkError;

        /// <summary>
        /// Set the given <paramref name="request"/> to this response.
        /// </summary>
        /// <param name="request">
        /// The request at the origin of this response.
        /// </param>
        /// <returns>
        /// Return this response.
        /// </returns>
        public HttpResponse SetRequest(HttpRequest request)
        {
            Request = new ReadOnlyHttpRequest(request);

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="request"/> to this response.
        /// </summary>
        /// <param name="request">
        /// A handle to the request at the origin of this response.
        /// </param>
        /// <returns>
        /// Return this response.
        /// </returns>
        public HttpResponse SetRequest(ReadOnlyHttpRequest request)
        {
            Request = request;

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
        /// Return this response.
        /// </returns>
        public HttpResponse SetHeader(string key, string value)
        {
            Headers[key] = value;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="headers"/> to this response.
        /// </summary>
        /// <param name="headers">
        /// The headers to add to the request.
        /// </param>
        /// <returns>
        /// Return this response.
        /// </returns>
        public HttpResponse SetHeaders(Dictionary<string, string> headers)
        {
            Headers = headers;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="data"/> to this response.
        /// </summary>
        /// <param name="data">
        /// The response's data serialized into bytes.
        /// </param>
        /// <returns>
        /// Return this response.
        /// </returns>
        public HttpResponse SetData(byte[] data)
        {
            Data = data;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="statusCode"/> to this response.
        /// </summary>
        /// <param name="statusCode">
        /// The status code sent by the server.
        /// </param>
        /// <returns>
        /// Return this response.
        /// </returns>
        public HttpResponse SetStatusCode(long statusCode)
        {
            StatusCode = statusCode;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="errorMessage"/> to this response.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message if an error occured during the <see cref="Request"/> processing.
        /// </param>
        /// <returns>
        /// Return this response.
        /// </returns>
        public HttpResponse SetErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="isHttpError"/> to this response.
        /// </summary>
        /// <param name="isHttpError">
        /// A flag to determine if this response failed due to an HTTP error.
        /// </param>
        /// <returns>
        /// Return this response.
        /// </returns>
        public HttpResponse SetIsHttpError(bool isHttpError)
        {
            IsHttpError = isHttpError;

            return this;
        }

        /// <summary>
        /// Set the given <paramref name="isNetworkError"/> to this response.
        /// </summary>
        /// <param name="isNetworkError">
        /// A flag to determine if this response failed due to a connectivity error.
        /// </param>
        /// <returns>
        /// Return this response.
        /// </returns>
        public HttpResponse SetIsNetworkError(bool isNetworkError)
        {
            IsNetworkError = isNetworkError;

            return this;
        }
    }
}
