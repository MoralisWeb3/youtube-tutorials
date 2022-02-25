using Newtonsoft.Json;

namespace WalletConnectSharp.Core.Models
{
    public class JsonRpcResponse : IEventSource
    {
        [JsonProperty]
        private long id;
        
        [JsonProperty]
        private string jsonrpc = "2.0";

        [JsonProperty]
        private JsonRpcError error;

        [JsonIgnore]
        public JsonRpcError Error
        {
            get { return error; }
        }

        [JsonIgnore]
        public bool IsError
        {
            get { return error != null; }
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

        public class JsonRpcError
        {
            [JsonProperty]
            private int? code;
            
            [JsonProperty]
            private string message;

            [JsonIgnore]
            public int? Code
            {
                get { return code; }
            }

            [JsonIgnore]
            public string Message
            {
                get { return message; }
            }
        }

        [JsonIgnore]
        public string Event
        {
            get { return "response:" + ID; }
        }
    }
}