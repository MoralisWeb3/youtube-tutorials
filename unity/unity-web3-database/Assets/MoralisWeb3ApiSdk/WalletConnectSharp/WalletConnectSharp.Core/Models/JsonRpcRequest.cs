using Newtonsoft.Json;
using WalletConnectSharp.Core.Utils;

namespace WalletConnectSharp.Core.Models
{
    public class JsonRpcRequest : IEventSource
    {
        [JsonProperty]
        private long id;
        [JsonProperty]
        private string jsonrpc = "2.0";
        
        [JsonProperty("method")]
        public virtual string Method { get; protected set; }

        public JsonRpcRequest()
        {
            if (this.id == 0)
            {
                this.id = RpcPayloadId.Generate();
            }
        }
        
        [JsonIgnore]
        public long ID
        {
            get { return id; }
        }

        [JsonIgnore]
        public string JsonRPC
        {
            get { return jsonrpc; }
        }

        [JsonIgnore]
        public string Event
        {
            get { return Method; }
        }
    }
}