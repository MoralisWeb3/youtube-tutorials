using Newtonsoft.Json;

namespace WalletConnectSharp.Core.Models
{
    public class WCSessionUpdate : JsonRpcRequest
    {
        public override string Method
        {
            get { return "wc_sessionUpdate"; }
        }
        
        [JsonProperty("params")]
        public WCSessionData[] parameters;

        public WCSessionUpdate(WCSessionData data)
        {
            this.parameters = new[] {data};
        }
    }
}