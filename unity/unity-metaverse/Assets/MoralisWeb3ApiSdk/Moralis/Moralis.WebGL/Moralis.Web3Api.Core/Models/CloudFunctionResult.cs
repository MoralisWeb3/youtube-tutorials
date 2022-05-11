using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Moralis.WebGL.Web3Api.Models
{
    public class CloudFunctionResult<T>
    {
        [JsonProperty("result")]
        public T Result { get; set; }
    }
}