using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.WebGL.Web3Api.Models
{
	[DataContract]
	public class Trade
	{
		/// <summary>
		/// The transaction hash
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "transaction_hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_hash")]
		public string TransactionHash { get; set; }

		/// <summary>
		/// The transaction index
		/// </summary>
		[DataMember(Name = "transaction_index", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_index")]
		public string TransactionIndex { get; set; }

		/// <summary>
		/// The token id(s) traded
		/// </summary>
		[DataMember(Name = "token_ids", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "token_ids")]
		public List<object> TokenIds { get; set; }

		/// <summary>
		/// The address that sold the NFT
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "seller_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "seller_address")]
		public string SellerAddress { get; set; }

		/// <summary>
		/// The address that bought the NFT
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "buyer_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "buyer_address")]
		public string BuyerAddress { get; set; }

		/// <summary>
		/// The address of the contract that traded the NFT
		/// example: 0x057Ec652A4F150f7FF94f089A38008f49a0DF88e
		/// </summary>
		[DataMember(Name = "marketplace_address", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "marketplace_address")]
		public string MarketplaceAddress { get; set; }

		/// <summary>
		/// The value that was sent in the transaction (ETH/BNB/etc..)
		/// example: 1000000000000000
		/// </summary>
		[DataMember(Name = "price", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "price")]
		public string Price { get; set; }

		/// <summary>
		/// The block timestamp
		/// example: 6/4/2021 4:00:15 PM
		/// </summary>
		[DataMember(Name = "block_timestamp", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_timestamp")]
		public string BlockTimestamp { get; set; }

		/// <summary>
		/// The blocknumber of the transaction
		/// example: 13680123
		/// </summary>
		[DataMember(Name = "block_number", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "block_number")]
		public string BlockNumber { get; set; }

		/// <summary>
		/// The block hash
		/// example: 0x4a7c916ca4a970358b9df90051008f729685ff05e9724a9dddba32630c37cb96
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
			sb.Append("class Trade{");
			sb.Append("  TransactionHash ").Append(TransactionHash).Append("\n");
			sb.Append("  TransactionIndex ").Append(TransactionIndex).Append("\n");
			sb.Append("  TokenIds ").Append(TokenIds).Append("\n");
			sb.Append("  SellerAddress ").Append(SellerAddress).Append("\n");
			sb.Append("  BuyerAddress ").Append(BuyerAddress).Append("\n");
			sb.Append("  MarketplaceAddress ").Append(MarketplaceAddress).Append("\n");
			sb.Append("  Price ").Append(Price).Append("\n");
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