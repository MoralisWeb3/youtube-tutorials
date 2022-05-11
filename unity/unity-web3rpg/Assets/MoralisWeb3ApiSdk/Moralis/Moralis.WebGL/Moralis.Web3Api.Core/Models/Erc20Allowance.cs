using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.WebGL.Web3Api.Models
{
	[DataContract]
	public class Erc20Allowance
	{
		/// <summary>
		/// The allowance
		/// </summary>
		[DataMember(Name = "allowance", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "allowance")]
		public string Allowance { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class Erc20Allowance{");
			sb.Append("  Allowance ").Append(Allowance).Append("\n");
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