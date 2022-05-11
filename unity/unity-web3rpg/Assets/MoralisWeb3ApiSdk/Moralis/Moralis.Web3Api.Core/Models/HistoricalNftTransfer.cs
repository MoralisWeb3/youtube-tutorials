using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class HistoricalNftTransfer
	{
		/// <summary>
		/// The transaction hash
		/// example: 0x2d30ca6f024dbc1307ac8a1a44ca27de6f797ec22ef20627a1307243b0ab7d09
		/// </summary>
		[DataMember(Name = "transaction_hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_hash")]
		public string TransactionHash { get; set; }

		/// <summary>
		/// The address of the token
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "address")]
		public string Address { get; set; }

		/// <summary>
		/// The block timestamp
		/// example: 4/2/2021 10:07:54 AM
		/// </summary>
		[DataMember(Name = "block_timestamp", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_timestamp")]
		public string BlockTimestamp { get; set; }

		/// <summary>
		/// The block number
		/// example: 12526958
		/// </summary>
		[DataMember(Name = "block_number", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_number")]
		public string BlockNumber { get; set; }

		/// <summary>
		/// The block hash
		/// example: 0x0372c302e3c52e8f2e15d155e2c545e6d802e479236564af052759253b20fd86
		/// </summary>
		[DataMember(Name = "block_hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_hash")]
		public string BlockHash { get; set; }

		/// <summary>
		/// The recipient
		/// example: 0x62AED87d21Ad0F3cdE4D147Fdcc9245401Af0044
		/// </summary>
		[DataMember(Name = "to_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "to_address")]
		public string ToAddress { get; set; }

		/// <summary>
		/// The sender
		/// example: 0xd4a3BebD824189481FC45363602b83C9c7e9cbDf
		/// </summary>
		[DataMember(Name = "from_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "from_address")]
		public string FromAddress { get; set; }

		/// <summary>
		/// The token ids of the tokens that were transfered
		/// </summary>
		[DataMember(Name = "token_ids", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "token_ids")]
		public List<String> TokenIds { get; set; }

		/// <summary>
		/// The amounts that were transfered
		/// </summary>
		[DataMember(Name = "amounts", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "amounts")]
		public List<String> Amounts { get; set; }

		/// <summary>
		/// They contract type of the transfer
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
			sb.Append("class HistoricalNftTransfer{");
			sb.Append("  TransactionHash ").Append(TransactionHash).Append("\n");
			sb.Append("  Address ").Append(Address).Append("\n");
			sb.Append("  BlockTimestamp ").Append(BlockTimestamp).Append("\n");
			sb.Append("  BlockNumber ").Append(BlockNumber).Append("\n");
			sb.Append("  BlockHash ").Append(BlockHash).Append("\n");
			sb.Append("  ToAddress ").Append(ToAddress).Append("\n");
			sb.Append("  FromAddress ").Append(FromAddress).Append("\n");
			sb.Append("  TokenIds ").Append(TokenIds).Append("\n");
			sb.Append("  Amounts ").Append(Amounts).Append("\n");
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