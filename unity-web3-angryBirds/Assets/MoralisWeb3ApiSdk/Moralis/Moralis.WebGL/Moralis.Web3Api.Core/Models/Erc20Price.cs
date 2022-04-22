using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.WebGL.Web3Api.Models
{
	[DataContract]
	public class Erc20Price
	{
		/// <summary>
		/// </summary>
		[DataMember(Name = "nativePrice", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "nativePrice")]
		public NativeErc20Price NativePrice { get; set; }

		/// <summary>
		/// format: double
		/// The price in USD for the token
		/// example: 19.722370676
		/// </summary>
		[DataMember(Name = "usdPrice", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "usdPrice")]
		public decimal? UsdPrice { get; set; }

		/// <summary>
		/// The address of the exchange used to calculate the price
		/// example: 0x1f98431c8ad98523631ae4a59f267346ea31f984
		/// </summary>
		[DataMember(Name = "exchangeAddress", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "exchangeAddress")]
		public string ExchangeAddress { get; set; }

		/// <summary>
		/// The name of the exchange used for calculating the price
		/// example: Uniswap v3
		/// </summary>
		[DataMember(Name = "exchangeName", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "exchangeName")]
		public string ExchangeName { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class Erc20Price{");
			sb.Append("  NativePrice ").Append(NativePrice).Append("\n");
			sb.Append("  UsdPrice ").Append(UsdPrice).Append("\n");
			sb.Append("  ExchangeAddress ").Append(ExchangeAddress).Append("\n");
			sb.Append("  ExchangeName ").Append(ExchangeName).Append("\n");
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