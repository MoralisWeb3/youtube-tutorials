using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class NativeErc20Price
	{
		/// <summary>
		/// The native price of the token
		/// example: 8409770570506626
		/// </summary>
		[DataMember(Name = "value", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "value")]
		public string Value { get; set; }

		/// <summary>
		/// The number of decimals of the token
		/// example: 18
		/// </summary>
		[DataMember(Name = "decimals", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "decimals")]
		public int? Decimals { get; set; }

		/// <summary>
		/// The Name of the token
		/// example: Ether
		/// </summary>
		[DataMember(Name = "name", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		/// <summary>
		/// The Symbol of the token
		/// example: ETH
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
			sb.Append("class NativeErc20Price{");
			sb.Append("  Value ").Append(Value).Append("\n");
			sb.Append("  Decimals ").Append(Decimals).Append("\n");
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