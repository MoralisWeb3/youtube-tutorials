using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class ReservesCollection
	{
		/// <summary>
		/// reserve0
		/// example: 1177323085102288091856004
		/// </summary>
		[DataMember(Name = "reserve0", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "reserve0")]
		public string Reserve0 { get; set; }

		/// <summary>
		/// reserve1
		/// example: 9424175928981149993184
		/// </summary>
		[DataMember(Name = "reserve1", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "reserve1")]
		public string Reserve1 { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class ReservesCollection{");
			sb.Append("  Reserve0 ").Append(Reserve0).Append("\n");
			sb.Append("  Reserve1 ").Append(Reserve1).Append("\n");
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