using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class NftContractMetadata
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
		/// example: KryptoKitties
		/// </summary>
		[DataMember(Name = "name", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		/// <summary>
		/// Timestamp of when the contract was last synced with the node
		/// </summary>
		[DataMember(Name = "synced_at", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "synced_at")]
		public string SyncedAt { get; set; }

		/// <summary>
		/// The symbol of the NFT contract
		/// example: RARI
		/// </summary>
		[DataMember(Name = "symbol", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "symbol")]
		public string Symbol { get; set; }

		/// <summary>
		/// The type of NFT contract
		/// example: ERC721
		/// </summary>
		[DataMember(Name = "contract_type", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "contract_type")]
		public string ContractType { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class NftContractMetadata{");
			sb.Append("  TokenAddress ").Append(TokenAddress).Append("\n");
			sb.Append("  Name ").Append(Name).Append("\n");
			sb.Append("  SyncedAt ").Append(SyncedAt).Append("\n");
			sb.Append("  Symbol ").Append(Symbol).Append("\n");
			sb.Append("  ContractType ").Append(ContractType).Append("\n");
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