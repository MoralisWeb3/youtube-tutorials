using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.WebGL.Web3Api.Models
{
	[DataContract]
	public class NftTransfer
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
		/// The address that sent the NFT
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "from_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "from_address")]
		public string FromAddress { get; set; }

		/// <summary>
		/// The address that recieved the NFT
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "to_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "to_address")]
		public string ToAddress { get; set; }

		/// <summary>
		/// The value that was sent in the transaction (ETH/BNB/etc..)
		/// example: 1000000000000000
		/// </summary>
		[DataMember(Name = "value", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "value")]
		public string Value { get; set; }

		/// <summary>
		/// The number of tokens transferred
		/// example: 1
		/// </summary>
		[DataMember(Name = "amount", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "amount")]
		public string Amount { get; set; }

		/// <summary>
		/// The type of NFT contract standard
		/// example: ERC721
		/// </summary>
		[DataMember(Name = "contract_type", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "contract_type")]
		public string ContractType { get; set; }

		/// <summary>
		/// The blocknumber of the transaction
		/// example: 88256
		/// </summary>
		[DataMember(Name = "block_number", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_number")]
		public string BlockNumber { get; set; }

		/// <summary>
		/// The block timestamp
		/// example: 6/4/2021 4:00:15 PM
		/// </summary>
		[DataMember(Name = "block_timestamp", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_timestamp")]
		public string BlockTimestamp { get; set; }

		/// <summary>
		/// The block hash of the transaction
		/// </summary>
		[DataMember(Name = "block_hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_hash")]
		public string BlockHash { get; set; }

		/// <summary>
		/// The transaction hash
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "transaction_hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_hash")]
		public string TransactionHash { get; set; }

		/// <summary>
		/// The transaction type
		/// </summary>
		[DataMember(Name = "transaction_type", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_type")]
		public string TransactionType { get; set; }

		/// <summary>
		/// The transaction index
		/// </summary>
		[DataMember(Name = "transaction_index", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_index")]
		public string TransactionIndex { get; set; }

		/// <summary>
		/// The log index
		/// </summary>
		[DataMember(Name = "log_index", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "log_index")]
		public int? LogIndex { get; set; }

		/// <summary>
		/// The operator present only for ERC1155 Transfers
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "operator", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "operator")]
		public string Operator { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class NftTransfer{");
			sb.Append("  TokenAddress ").Append(TokenAddress).Append("\n");
			sb.Append("  TokenId ").Append(TokenId).Append("\n");
			sb.Append("  FromAddress ").Append(FromAddress).Append("\n");
			sb.Append("  ToAddress ").Append(ToAddress).Append("\n");
			sb.Append("  Value ").Append(Value).Append("\n");
			sb.Append("  Amount ").Append(Amount).Append("\n");
			sb.Append("  ContractType ").Append(ContractType).Append("\n");
			sb.Append("  BlockNumber ").Append(BlockNumber).Append("\n");
			sb.Append("  BlockTimestamp ").Append(BlockTimestamp).Append("\n");
			sb.Append("  BlockHash ").Append(BlockHash).Append("\n");
			sb.Append("  TransactionHash ").Append(TransactionHash).Append("\n");
			sb.Append("  TransactionType ").Append(TransactionType).Append("\n");
			sb.Append("  TransactionIndex ").Append(TransactionIndex).Append("\n");
			sb.Append("  LogIndex ").Append(LogIndex).Append("\n");
			sb.Append("  Operator ").Append(Operator).Append("\n");
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