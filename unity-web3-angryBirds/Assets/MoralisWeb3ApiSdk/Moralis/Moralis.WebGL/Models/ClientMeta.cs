using Newtonsoft.Json;
using System;

namespace Moralis.WebGL.Models
{
    [Serializable]
    public class ClientMeta
    {
        [JsonProperty("description")]
        public string Description;

        [JsonProperty("url")]
        public string URL;

        [JsonProperty("icons")]
        public string[] Icons;

        [JsonProperty("name")]
        public string Name;
    }
}
