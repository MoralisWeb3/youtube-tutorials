using Newtonsoft.Json;

namespace WalletConnectSharp.Core.Models
{
    public class WcSessionRequest : JsonRpcRequest
    {
        public override string Method
        {
            get { return "wc_sessionRequest"; }
        }

        [JsonProperty("params")]
        public WcSessionRequestRequestParams[] parameters;

        public WcSessionRequest(ClientMeta clientMeta, string clientId, int chainId = 1)
        {
            this.parameters = new[]
            {
                new WcSessionRequestRequestParams()
                {
                    peerId = clientId,
                    chainId = chainId,
                    peerMeta = clientMeta
                }
            };
        }

        public class WcSessionRequestRequestParams
        {
            public string peerId;
            public ClientMeta peerMeta;
            public int chainId;
        }
    }
}