using Newtonsoft.Json;

namespace Moralis.WebGL.SolanaApi.Models
{
    public class CloudFunctionResult<T>
    {
        [JsonProperty("result")]
        public T Result { get; set; }
    }
}
