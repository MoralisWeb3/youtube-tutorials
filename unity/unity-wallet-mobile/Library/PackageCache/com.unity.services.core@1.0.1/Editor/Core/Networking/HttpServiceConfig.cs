using System;

namespace Unity.Services.Core.Editor
{
    [Serializable]
    struct HttpServiceConfig
    {
        public string ServiceId;

        public string BaseUrl;

        public HttpOptions DefaultOptions;
    }
}
