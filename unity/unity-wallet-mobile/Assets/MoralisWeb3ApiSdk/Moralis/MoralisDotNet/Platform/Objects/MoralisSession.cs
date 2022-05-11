

namespace Moralis.Platform.Objects
{
    public class MoralisSession : MoralisObject
    {
        public MoralisSession() : base("_Session") { }

       // [JsonProperty("sessionToken")]
        public string sessionToken { get; set; }
    }
}
