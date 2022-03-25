using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class Erc20TokenBalance
	{
		/// <summary>
		/// The address of the token contract
		/// example: 0x2d30ca6f024dbc1307ac8a1a44ca27de6f797ec22ef20627a1307243b0ab7d09
		/// </summary>
		[DataMember(Name = "token_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "token_address")]
		public string TokenAddress { get; set; }

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
		/// The logo of the token
		/// example: https://cdn.moralis.io/eth/0x67b6d479c7bb412c54e03dca8e1bc6740ce6b99c.png
		/// </summary>
		[DataMember(Name = "logo", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "logo")]
		public string Logo { get; set; }

		/// <summary>
		/// The thumbnail of the logo
		/// example: https://cdn.moralis.io/eth/0x67b6d479c7bb412c54e03dca8e1bc6740ce6b99c_thumb.png
		/// </summary>
		[DataMember(Name = "thumbnail", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "thumbnail")]
		public string Thumbnail { get; set; }

		/// <summary>
		/// The number of decimals on of the token
		/// example: 18
		/// </summary>
		[DataMember(Name = "decimals", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "decimals")]
		public string Decimals { get; set; }

		/// <summary>
		/// Timestamp of when the contract was last synced with the node
		/// example: 123456789
		/// </summary>
		[DataMember(Name = "balance", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "balance")]
		public string Balance { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class Erc20TokenBalance{");
			sb.Append("  TokenAddress ").Append(TokenAddress).Append("\n");
			sb.Append("  Name ").Append(Name).Append("\n");
			sb.Append("  Symbol ").Append(Symbol).Append("\n");
			sb.Append("  Logo ").Append(Logo).Append("\n");
			sb.Append("  Thumbnail ").Append(Thumbnail).Append("\n");
			sb.Append("  Decimals ").Append(Decimals).Append("\n");
			sb.Append("  Balance ").Append(Balance).Append("\n");
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