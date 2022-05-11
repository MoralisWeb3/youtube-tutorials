namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Utility extensions on <see cref="HttpRequest"/>.
    /// </summary>
    static class HttpRequestExtensions
    {
        /// <summary>
        /// Set this method to "GET".
        /// </summary>
        /// <param name="self">
        /// The request to update the method of.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public static HttpRequest AsGet(this HttpRequest self)
        {
            return self.SetMethod("GET");
        }

        /// <summary>
        /// Set this method to "POST".
        /// </summary>
        /// <param name="self">
        /// The request to update the method of.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public static HttpRequest AsPost(this HttpRequest self)
        {
            return self.SetMethod("POST");
        }

        /// <summary>
        /// Set this method to "PUT".
        /// </summary>
        /// <param name="self">
        /// The request to update the method of.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public static HttpRequest AsPut(this HttpRequest self)
        {
            return self.SetMethod("PUT");
        }

        /// <summary>
        /// Set this method to "DELETE".
        /// </summary>
        /// <param name="self">
        /// The request to update the method of.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public static HttpRequest AsDelete(this HttpRequest self)
        {
            return self.SetMethod("DELETE");
        }

        /// <summary>
        /// Set this method to "PATCH".
        /// </summary>
        /// <param name="self">
        /// The request to update the method of.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public static HttpRequest AsPatch(this HttpRequest self)
        {
            return self.SetMethod("PATCH");
        }

        /// <summary>
        /// Set this method to "HEAD".
        /// </summary>
        /// <param name="self">
        /// The request to update the method of.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public static HttpRequest AsHead(this HttpRequest self)
        {
            return self.SetMethod("HEAD");
        }

        /// <summary>
        /// Set this method to "CONNECT".
        /// </summary>
        /// <param name="self">
        /// The request to update the method of.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public static HttpRequest AsConnect(this HttpRequest self)
        {
            return self.SetMethod("CONNECT");
        }

        /// <summary>
        /// Set this method to "OPTIONS".
        /// </summary>
        /// <param name="self">
        /// The request to update the method of.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public static HttpRequest AsOptions(this HttpRequest self)
        {
            return self.SetMethod("OPTIONS");
        }

        /// <summary>
        /// Set this method to "TRACE".
        /// </summary>
        /// <param name="self">
        /// The request to update the method of.
        /// </param>
        /// <returns>
        /// Return this request.
        /// </returns>
        public static HttpRequest AsTrace(this HttpRequest self)
        {
            return self.SetMethod("TRACE");
        }
    }
}
