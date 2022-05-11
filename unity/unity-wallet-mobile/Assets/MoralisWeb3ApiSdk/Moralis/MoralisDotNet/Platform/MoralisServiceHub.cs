using Moralis.Platform.Abstractions;
using Moralis.Platform;
using Moralis.Platform.Services;
using Moralis.Platform.Objects;
using Moralis.Platform.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Moralis.Platform
{
    public class MoralisServiceHub : ServiceHub<MoralisUser>
    {
        public MoralisServiceHub (HttpClient httpClient, IServerConnectionData connectionData, IJsonSerializer jsonSerializer) : base(connectionData, jsonSerializer, httpClient) { }
    }
}
