using Newtonsoft.Json;

namespace Moralis.SolanaApi.Models
{
    public class CloudFunctionResult<T>
    {
        [JsonProperty("result")]
        public T Result { get; set; }
    }
}
