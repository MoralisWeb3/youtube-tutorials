using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.WebGL.Web3Api.Models
{
	[DataContract]
	public class Transaction
	{
		/// <summary>
		/// The hash of the transaction
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "hash")]
		public string Hash { get; set; }

		/// <summary>
		/// The nonce of the transaction
		/// example: 326595425
		/// </summary>
		[DataMember(Name = "nonce", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "nonce")]
		public string Nonce { get; set; }

		/// <summary>
		/// The transaction index
		/// example: 25
		/// </summary>
		[DataMember(Name = "transaction_index", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_index")]
		public string TransactionIndex { get; set; }

		/// <summary>
		/// The sender
		/// example: 0xd4a3BebD824189481FC45363602b83C9c7e9cbDf
		/// </summary>
		[DataMember(Name = "from_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "from_address")]
		public string FromAddress { get; set; }

		/// <summary>
		/// The recipient
		/// example: 0xa71db868318f0a0bae9411347cd4a6fa23d8d4ef
		/// </summary>
		[DataMember(Name = "to_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "to_address")]
		public string ToAddress { get; set; }

		/// <summary>
		/// The value that was transfered (in wei)
		/// example: 650000000000000000
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
		/// The input
		/// </summary>
		[DataMember(Name = "input", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "input")]
		public string Input { get; set; }

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
		/// The receipt contract address
		/// example: 0x1d6a4cf64b52f6c73f201839aded7379ce58059c
		/// </summary>
		[DataMember(Name = "receipt_contract_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipt_contract_address")]
		public string ReceiptContractAddress { get; set; }

		/// <summary>
		/// The receipt root
		/// </summary>
		[DataMember(Name = "receipt_root", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipt_root")]
		public string ReceiptRoot { get; set; }

		/// <summary>
		/// The receipt status
		/// example: 1
		/// </summary>
		[DataMember(Name = "receipt_status", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipt_status")]
		public string ReceiptStatus { get; set; }

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
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class Transaction{");
			sb.Append("  Hash ").Append(Hash).Append("\n");
			sb.Append("  Nonce ").Append(Nonce).Append("\n");
			sb.Append("  TransactionIndex ").Append(TransactionIndex).Append("\n");
			sb.Append("  FromAddress ").Append(FromAddress).Append("\n");
			sb.Append("  ToAddress ").Append(ToAddress).Append("\n");
			sb.Append("  Value ").Append(Value).Append("\n");
			sb.Append("  Gas ").Append(Gas).Append("\n");
			sb.Append("  GasPrice ").Append(GasPrice).Append("\n");
			sb.Append("  Input ").Append(Input).Append("\n");
			sb.Append("  ReceiptCumulativeGasUsed ").Append(ReceiptCumulativeGasUsed).Append("\n");
			sb.Append("  ReceiptGasUsed ").Append(ReceiptGasUsed).Append("\n");
			sb.Append("  ReceiptContractAddress ").Append(ReceiptContractAddress).Append("\n");
			sb.Append("  ReceiptRoot ").Append(ReceiptRoot).Append("\n");
			sb.Append("  ReceiptStatus ").Append(ReceiptStatus).Append("\n");
			sb.Append("  BlockTimestamp ").Append(BlockTimestamp).Append("\n");
			sb.Append("  BlockNumber ").Append(BlockNumber).Append("\n");
			sb.Append("  BlockHash ").Append(BlockHash).Append("\n");
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