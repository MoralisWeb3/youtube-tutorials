using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.WebGL.Web3Api.Models
{
	[DataContract]
	public class LogEventByAddress
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
		/// The data of the log
		/// example: 0x00000000000000000000000000000000000000000000000de05239bccd4d537400000000000000000000000000024dbc80a9f80e3d5fc0a0ee30e2693781a443
		/// </summary>
		[DataMember(Name = "data", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "data")]
		public string Data { get; set; }

		/// <summary>
		/// example: 0x2caecd17d02f56fa897705dcc740da2d237c373f70686f4e0d9bd3bf0400ea7a
		/// </summary>
		[DataMember(Name = "topic0", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "topic0")]
		public string Topic0 { get; set; }

		/// <summary>
		/// example: 0x000000000000000000000000031002d15b0d0cd7c9129d6f644446368deae391
		/// </summary>
		[DataMember(Name = "topic1", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "topic1")]
		public string Topic1 { get; set; }

		/// <summary>
		/// example: 0x000000000000000000000000d25943be09f968ba740e0782a34e710100defae9
		/// </summary>
		[DataMember(Name = "topic2", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "topic2")]
		public string Topic2 { get; set; }

		/// <summary>
		/// </summary>
		[DataMember(Name = "topic3", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "topic3")]
		public string Topic3 { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class LogEventByAddress{");
			sb.Append("  TransactionHash ").Append(TransactionHash).Append("\n");
			sb.Append("  Address ").Append(Address).Append("\n");
			sb.Append("  BlockTimestamp ").Append(BlockTimestamp).Append("\n");
			sb.Append("  BlockNumber ").Append(BlockNumber).Append("\n");
			sb.Append("  BlockHash ").Append(BlockHash).Append("\n");
			sb.Append("  Data ").Append(Data).Append("\n");
			sb.Append("  Topic0 ").Append(Topic0).Append("\n");
			sb.Append("  Topic1 ").Append(Topic1).Append("\n");
			sb.Append("  Topic2 ").Append(Topic2).Append("\n");
			sb.Append("  Topic3 ").Append(Topic3).Append("\n");
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