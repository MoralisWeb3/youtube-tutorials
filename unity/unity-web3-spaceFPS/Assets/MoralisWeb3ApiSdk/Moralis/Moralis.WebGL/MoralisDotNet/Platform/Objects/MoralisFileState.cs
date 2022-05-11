using System;

namespace Moralis.WebGL.Platform.Objects
{
    public class MoralisFileState
    {
        static string SecureHyperTextTransferScheme { get; } = "https";

        public string name { get; set; }
        public string mediatype { get; set; }
        public Uri url { get; set; }

        public MoralisFileState() { }

        public Uri SecureLocation => url switch
        {
            { Host: "files.parsetfss.com" } location => new UriBuilder(location)
            {
                Scheme = SecureHyperTextTransferScheme,

                // This makes URIBuilder assign the default port for the URL scheme.

                Port = -1,
            }.Uri,
            _ => url
        };
    }
}
