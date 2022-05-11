using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class BlockDate
	{
		/// <summary>
		/// The date of the block
		/// example: 12/31/2019 7:00:00 PM
		/// </summary>
		[DataMember(Name = "date", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "date")]
		public string Date { get; set; }

		/// <summary>
		/// The blocknumber
		/// example: 9193266
		/// </summary>
		[DataMember(Name = "block", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block")]
		public decimal? Block { get; set; }

		/// <summary>
		/// The timestamp of the block
		/// example: 1577836811
		/// </summary>
		[DataMember(Name = "timestamp", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "timestamp")]
		public decimal? Timestamp { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class BlockDate{");
			sb.Append("  Date ").Append(Date).Append("\n");
			sb.Append("  Block ").Append(Block).Append("\n");
			sb.Append("  Timestamp ").Append(Timestamp).Append("\n");
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