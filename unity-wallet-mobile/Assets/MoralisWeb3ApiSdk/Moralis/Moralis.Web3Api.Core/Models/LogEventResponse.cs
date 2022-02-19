using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Moralis.Web3Api.Models;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
    [DataContract]
    public class LogEventResponse
    {
        [DataMember(Name = "total", EmitDefaultValue = true)]
        [JsonProperty(PropertyName = "total")]
        public long Total { get; set; }

        [DataMember(Name = "page", EmitDefaultValue = true)]
        [JsonProperty(PropertyName = "page")]
        public long Page { get; set; }

        [DataMember(Name = "page_size", EmitDefaultValue = true)]
        [JsonProperty(PropertyName = "page_size")]
        public long PageSize { get; set; }

        [DataMember(Name = "result", EmitDefaultValue = true)]
        [JsonProperty(PropertyName = "result")]
        public List<LogEvent> Events { get; set; }
    }
}
