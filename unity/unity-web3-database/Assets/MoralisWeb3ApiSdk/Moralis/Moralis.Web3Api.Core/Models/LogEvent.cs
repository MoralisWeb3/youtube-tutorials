using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class LogEvent
	{
		/// <summary>
		/// The transaction hash
		/// example: 0x2d30ca6f024dbc1307ac8a1a44ca27de6f797ec22ef20627a1307243b0ab7d09
		/// </summary>
		[DataMember(Name = "transaction_hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_hash")]
		public string TransactionHash { get; set; }

		/// <summary>
		/// The address of the contract
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
		/// The content of the event
		/// </summary>
		[DataMember(Name = "data", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "data")]
		public object Data { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class LogEvent{");
			sb.Append("  TransactionHash ").Append(TransactionHash).Append("\n");
			sb.Append("  Address ").Append(Address).Append("\n");
			sb.Append("  BlockTimestamp ").Append(BlockTimestamp).Append("\n");
			sb.Append("  BlockNumber ").Append(BlockNumber).Append("\n");
			sb.Append("  BlockHash ").Append(BlockHash).Append("\n");
			sb.Append("  Data ").Append(Data).Append("\n");
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