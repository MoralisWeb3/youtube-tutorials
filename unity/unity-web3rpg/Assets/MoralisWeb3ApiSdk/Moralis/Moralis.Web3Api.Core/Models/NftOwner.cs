using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class NftOwner
	{
		/// <summary>
		/// The address of the contract of the NFT
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "token_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "token_address")]
		public string TokenAddress { get; set; }

		/// <summary>
		/// The token id of the NFT
		/// example: 15
		/// </summary>
		[DataMember(Name = "token_id", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "token_id")]
		public string TokenId { get; set; }

		/// <summary>
		/// The type of NFT contract standard
		/// example: ERC721
		/// </summary>
		[DataMember(Name = "contract_type", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "contract_type")]
		public string ContractType { get; set; }

		/// <summary>
		/// The address of the owner of the NFT
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "owner_of", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "owner_of")]
		public string OwnerOf { get; set; }

		/// <summary>
		/// The blocknumber when the amount or owner changed
		/// example: 88256
		/// </summary>
		[DataMember(Name = "block_number", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_number")]
		public string BlockNumber { get; set; }

		/// <summary>
		/// The blocknumber when the NFT was minted
		/// example: 88256
		/// </summary>
		[DataMember(Name = "block_number_minted", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_number_minted")]
		public string BlockNumberMinted { get; set; }

		/// <summary>
		/// The uri to the metadata of the token
		/// </summary>
		[DataMember(Name = "token_uri", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "token_uri")]
		public string TokenUri { get; set; }

		/// <summary>
		/// The metadata of the token
		/// </summary>
		[DataMember(Name = "metadata", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "metadata")]
		public string Metadata { get; set; }

		/// <summary>
		/// when the metadata was last updated
		/// </summary>
		[DataMember(Name = "synced_at", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "synced_at")]
		public string SyncedAt { get; set; }

		/// <summary>
		/// The number of this item the user owns (used by ERC1155)
		/// example: 1
		/// </summary>
		[DataMember(Name = "amount", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "amount")]
		public string Amount { get; set; }

		/// <summary>
		/// The name of the Token contract
		/// example: CryptoKitties
		/// </summary>
		[DataMember(Name = "name", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		/// <summary>
		/// The symbol of the NFT contract
		/// example: RARI
		/// </summary>
		[DataMember(Name = "symbol", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "symbol")]
		public string Symbol { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class NftOwner{");
			sb.Append("  TokenAddress ").Append(TokenAddress).Append("\n");
			sb.Append("  TokenId ").Append(TokenId).Append("\n");
			sb.Append("  ContractType ").Append(ContractType).Append("\n");
			sb.Append("  OwnerOf ").Append(OwnerOf).Append("\n");
			sb.Append("  BlockNumber ").Append(BlockNumber).Append("\n");
			sb.Append("  BlockNumberMinted ").Append(BlockNumberMinted).Append("\n");
			sb.Append("  TokenUri ").Append(TokenUri).Append("\n");
			sb.Append("  Metadata ").Append(Metadata).Append("\n");
			sb.Append("  SyncedAt ").Append(SyncedAt).Append("\n");
			sb.Append("  Amount ").Append(Amount).Append("\n");
			sb.Append("  Name ").Append(Name).Append("\n");
			sb.Append("  Symbol ").Append(Symbol).Append("\n");
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