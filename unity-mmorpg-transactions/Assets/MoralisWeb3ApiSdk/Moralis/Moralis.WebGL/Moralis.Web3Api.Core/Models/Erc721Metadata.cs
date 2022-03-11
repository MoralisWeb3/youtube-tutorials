using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.WebGL.Web3Api.Models
{
	[DataContract]
	public class Erc721Metadata
	{
		/// <summary>
		/// The name of the token Contract
		/// example: Kylin Network
		/// </summary>
		[DataMember(Name = "name", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		/// <summary>
		/// The symbol of the NFT contract
		/// example: KYL
		/// </summary>
		[DataMember(Name = "symbol", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "symbol")]
		public string Symbol { get; set; }

		/// <summary>
		/// </summary>
		[DataMember(Name = "token_uri", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "token_uri")]
		public string TokenUri { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class Erc721Metadata{");
			sb.Append("  Name ").Append(Name).Append("\n");
			sb.Append("  Symbol ").Append(Symbol).Append("\n");
			sb.Append("  TokenUri ").Append(TokenUri).Append("\n");
			sb.Append("}");

			return sb.ToString();
		}

		/// <summary>
		/// Get the JSON string presentation of the object
		/// </summary>
		/// <returns>JSON string presentation of the object</returns>
		public string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

	}
}