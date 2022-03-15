using System;
using System.Collections.Generic;
using System.IO;

namespace Moralis.WebGL.Web3Api.Models
{
    /// <summary>
    /// <code>IHttpRequest</code> is an interface that provides an API to execute HTTP request data.
    /// </summary>
    public class WebRequest
    {
        public Uri Target => new Uri($"{Resource}{Path}"); //new Uri(new Uri(Resource), Path);

        public string Resource { get; set; }

        public string Path { get; set; }

        public IList<KeyValuePair<string, string>> Headers { get; set; }

        /// <summary>
        /// Data stream to be uploaded.
        /// </summary>
        public virtual Stream Data { get; set; }

        /// <summary>
        /// HTTP method. One of <c>DELETE</c>, <c>GET</c>, <c>HEAD</c>, <c>POST</c> or <c>PUT</c>
        /// </summary>
        public string Method { get; set; }
    }
}
