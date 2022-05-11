using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class TradesCollection
	{
		/// <summary>
		/// The token id(s) traded
		/// </summary>
		[DataMember(Name = "token_ids", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "token_ids")]
		public List<object> TokenIds { get; set; }

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
		/// The gas of the transaction
		/// example: 6721975
		/// </summary>
		[DataMember(Name = "gas", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "gas")]
		public string Gas { get; set; }

		/// <summary>
		/// The gas price
		/// example: 20000000000
		/// </summary>
		[DataMember(Name = "gas_price", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "gas_price")]
		public string GasPrice { get; set; }

		/// <summary>
		/// The receipt cumulative gas used
		/// example: 1340925
		/// </summary>
		[DataMember(Name = "receipt_cumulative_gas_used", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipt_cumulative_gas_used")]
		public string ReceiptCumulativeGasUsed { get; set; }

		/// <summary>
		/// The receipt gas used
		/// example: 1340925
		/// </summary>
		[DataMember(Name = "receipt_gas_used", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipt_gas_used")]
		public string ReceiptGasUsed { get; set; }

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
		/// The transaction hash
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "transaction_hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_hash")]
		public string TransactionHash { get; set; }

		/// <summary>
		/// The transaction index
		/// </summary>
		[DataMember(Name = "transaction_index", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_index")]
		public string TransactionIndex { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class TradesCollection{");
			sb.Append("  TokenIds ").Append(TokenIds).Append("\n");
			sb.Append("  FromAddress ").Append(FromAddress).Append("\n");
			sb.Append("  ToAddress ").Append(ToAddress).Append("\n");
			sb.Append("  Value ").Append(Value).Append("\n");
			sb.Append("  Gas ").Append(Gas).Append("\n");
			sb.Append("  GasPrice ").Append(GasPrice).Append("\n");
			sb.Append("  ReceiptCumulativeGasUsed ").Append(ReceiptCumulativeGasUsed).Append("\n");
			sb.Append("  ReceiptGasUsed ").Append(ReceiptGasUsed).Append("\n");
			sb.Append("  BlockNumber ").Append(BlockNumber).Append("\n");
			sb.Append("  BlockTimestamp ").Append(BlockTimestamp).Append("\n");
			sb.Append("  TransactionHash ").Append(TransactionHash).Append("\n");
			sb.Append("  TransactionIndex ").Append(TransactionIndex).Append("\n");
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