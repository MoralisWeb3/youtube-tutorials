using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class BlockTransaction
	{
		/// <summary>
		/// The hash of the transaction
		/// example: 0x1ed85b3757a6d31d01a4d6677fc52fd3911d649a0af21fe5ca3f886b153773ed
		/// </summary>
		[DataMember(Name = "hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "hash")]
		public string Hash { get; set; }

		/// <summary>
		/// The nonce
		/// example: 1848059
		/// </summary>
		[DataMember(Name = "nonce", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "nonce")]
		public string Nonce { get; set; }

		/// <summary>
		/// example: 108
		/// </summary>
		[DataMember(Name = "transaction_index", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_index")]
		public string TransactionIndex { get; set; }

		/// <summary>
		/// The from address
		/// example: 0x267be1c1d684f78cb4f6a176c4911b741e4ffdc0
		/// </summary>
		[DataMember(Name = "from_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "from_address")]
		public string FromAddress { get; set; }

		/// <summary>
		/// The to address
		/// example: 0x003dde3494f30d861d063232c6a8c04394b686ff
		/// </summary>
		[DataMember(Name = "to_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "to_address")]
		public string ToAddress { get; set; }

		/// <summary>
		/// The value sent
		/// example: 115580000000000000
		/// </summary>
		[DataMember(Name = "value", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "value")]
		public string Value { get; set; }

		/// <summary>
		/// example: 30000
		/// </summary>
		[DataMember(Name = "gas", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "gas")]
		public string Gas { get; set; }

		/// <summary>
		/// The gas price
		/// example: 52500000000
		/// </summary>
		[DataMember(Name = "gas_price", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "gas_price")]
		public string GasPrice { get; set; }

		/// <summary>
		/// example: 0x
		/// </summary>
		[DataMember(Name = "input", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "input")]
		public string Input { get; set; }

		/// <summary>
		/// example: 4923073
		/// </summary>
		[DataMember(Name = "receipt_cumulative_gas_used", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipt_cumulative_gas_used")]
		public string ReceiptCumulativeGasUsed { get; set; }

		/// <summary>
		/// example: 21000
		/// </summary>
		[DataMember(Name = "receipt_gas_used", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipt_gas_used")]
		public string ReceiptGasUsed { get; set; }

		/// <summary>
		/// </summary>
		[DataMember(Name = "receipt_contract_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipt_contract_address")]
		public string ReceiptContractAddress { get; set; }

		/// <summary>
		/// </summary>
		[DataMember(Name = "receipt_root", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipt_root")]
		public string ReceiptRoot { get; set; }

		/// <summary>
		/// example: 1
		/// </summary>
		[DataMember(Name = "receipt_status", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipt_status")]
		public string ReceiptStatus { get; set; }

		/// <summary>
		/// The block timestamp
		/// example: 5/7/2021 11:08:35 AM
		/// </summary>
		[DataMember(Name = "block_timestamp", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_timestamp")]
		public string BlockTimestamp { get; set; }

		/// <summary>
		/// The block number
		/// example: 12386788
		/// </summary>
		[DataMember(Name = "block_number", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_number")]
		public string BlockNumber { get; set; }

		/// <summary>
		/// The hash of the block
		/// example: 0x9b559aef7ea858608c2e554246fe4a24287e7aeeb976848df2b9a2531f4b9171
		/// </summary>
		[DataMember(Name = "block_hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_hash")]
		public string BlockHash { get; set; }

		/// <summary>
		/// The logs of the transaction
		/// </summary>
		[DataMember(Name = "logs", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "logs")]
		public List<Log> Logs { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class BlockTransaction{");
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
			sb.Append("  Logs ").Append(Logs).Append("\n");
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