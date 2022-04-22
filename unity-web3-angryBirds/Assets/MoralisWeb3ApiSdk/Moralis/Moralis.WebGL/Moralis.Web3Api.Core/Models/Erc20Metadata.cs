using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.WebGL.Web3Api.Models
{
	[DataContract]
	public class Erc20Metadata
	{
		/// <summary>
		/// The address of the token contract
		/// example: 0x2d30ca6f024dbc1307ac8a1a44ca27de6f797ec22ef20627a1307243b0ab7d09
		/// </summary>
		[DataMember(Name = "address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "address")]
		public string Address { get; set; }

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
		/// The number of decimals on of the token
		/// example: 18
		/// </summary>
		[DataMember(Name = "decimals", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "decimals")]
		public string Decimals { get; set; }

		/// <summary>
		/// The logo of the token
		/// example: https://cdn.moralis.io/eth/0x67b6d479c7bb412c54e03dca8e1bc6740ce6b99c.png
		/// </summary>
		[DataMember(Name = "logo", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "logo")]
		public string Logo { get; set; }

		/// <summary>
		/// The logo hash
		/// example: ee7aa2cdf100649a3521a082116258e862e6971261a39b5cd4e4354fcccbc54d
		/// </summary>
		[DataMember(Name = "logo_hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "logo_hash")]
		public string LogoHash { get; set; }

		/// <summary>
		/// The thumbnail of the logo
		/// example: https://cdn.moralis.io/eth/0x67b6d479c7bb412c54e03dca8e1bc6740ce6b99c_thumb.png
		/// </summary>
		[DataMember(Name = "thumbnail", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "thumbnail")]
		public string Thumbnail { get; set; }

		/// <summary>
		/// </summary>
		[DataMember(Name = "block_number", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_number")]
		public string BlockNumber { get; set; }

		/// <summary>
		/// </summary>
		[DataMember(Name = "validated", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "validated")]
		public string Validated { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class Erc20Metadata{");
			sb.Append("  Address ").Append(Address).Append("\n");
			sb.Append("  Name ").Append(Name).Append("\n");
			sb.Append("  Symbol ").Append(Symbol).Append("\n");
			sb.Append("  Decimals ").Append(Decimals).Append("\n");
			sb.Append("  Logo ").Append(Logo).Append("\n");
			sb.Append("  LogoHash ").Append(LogoHash).Append("\n");
			sb.Append("  Thumbnail ").Append(Thumbnail).Append("\n");
			sb.Append("  BlockNumber ").Append(BlockNumber).Append("\n");
			sb.Append("  Validated ").Append(Validated).Append("\n");
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