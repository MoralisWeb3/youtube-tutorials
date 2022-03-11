using Newtonsoft.Json;

namespace WalletConnectSharp.Core.Models
{
    public class InternalEvent
    {
        [JsonProperty("event")]
        public string @event;
    }
}