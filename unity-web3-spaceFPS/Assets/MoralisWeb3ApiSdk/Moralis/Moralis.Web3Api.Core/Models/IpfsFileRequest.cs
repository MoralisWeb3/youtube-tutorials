using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public class IpfsFileRequest
	{
		/// <summary>
		/// Path to file
		/// example: moralis/logo.jpg
		/// </summary>
		[DataMember(Name = "path", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "path")]
		public string Path { get; set; }

		/// <summary>
		/// base64 or JSON
		/// example: iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAApgAAAKYB3X3
		/// </summary>
		[DataMember(Name = "content", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "content")]
		public string Content { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class IpfsFileRequest{");
			sb.Append("  Path ").Append(Path).Append("\n");
			sb.Append("  Content ").Append(Content).Append("\n");
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