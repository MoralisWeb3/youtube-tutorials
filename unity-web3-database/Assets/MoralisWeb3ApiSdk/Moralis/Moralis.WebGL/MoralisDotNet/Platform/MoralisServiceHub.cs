using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform;
using Moralis.WebGL.Platform.Services;
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Platform.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Moralis.WebGL.Platform
{
    public class MoralisServiceHub : ServiceHub<MoralisUser>
    {
        public MoralisServiceHub (HttpClient httpClient, IServerConnectionData connectionData, IJsonSerializer jsonSerializer) : base(connectionData, jsonSerializer, httpClient) { }
    }
}
