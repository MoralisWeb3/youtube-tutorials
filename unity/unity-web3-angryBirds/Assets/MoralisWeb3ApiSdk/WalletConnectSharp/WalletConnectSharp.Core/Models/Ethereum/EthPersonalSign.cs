using Newtonsoft.Json;

namespace WalletConnectSharp.Core.Models.Ethereum
{
    public sealed class EthPersonalSign : JsonRpcRequest
    {
        [JsonProperty("params")] 
        private string[] _parameters;

        [JsonIgnore]
        public string[] Parameters => _parameters;

        public EthPersonalSign(string address, string hexData, string password = "") : base()
        {
            this.Method = "personal_sign";
            this._parameters = new[] {hexData, address, password};
        }
    }
}