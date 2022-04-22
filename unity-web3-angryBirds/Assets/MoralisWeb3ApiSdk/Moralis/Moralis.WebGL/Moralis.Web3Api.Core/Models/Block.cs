using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.WebGL.Web3Api.Models
{
	[DataContract]
	public class Block
	{
		/// <summary>
		/// The block timestamp
		/// example: 5/7/2021 11:08:35 AM
		/// </summary>
		[DataMember(Name = "timestamp", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "timestamp")]
		public string Timestamp { get; set; }

		/// <summary>
		/// The block number
		/// example: 12386788
		/// </summary>
		[DataMember(Name = "number", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "number")]
		public string Number { get; set; }

		/// <summary>
		/// The block hash
		/// example: 0x9b559aef7ea858608c2e554246fe4a24287e7aeeb976848df2b9a2531f4b9171
		/// </summary>
		[DataMember(Name = "hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "hash")]
		public string Hash { get; set; }

		/// <summary>
		/// The block hash of the parent block
		/// example: 0x011d1fc45839de975cc55d758943f9f1d204f80a90eb631f3bf064b80d53e045
		/// </summary>
		[DataMember(Name = "parent_hash", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "parent_hash")]
		public string ParentHash { get; set; }

		/// <summary>
		/// The nonce
		/// example: 0xedeb2d8fd2b2bdec
		/// </summary>
		[DataMember(Name = "nonce", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "nonce")]
		public string Nonce { get; set; }

		/// <summary>
		/// example: 0x1dcc4de8dec75d7aab85b567b6ccd41ad312451b948a7413f0a142fd40d49347
		/// </summary>
		[DataMember(Name = "sha3_uncles", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "sha3_uncles")]
		public string Sha3Uncles { get; set; }

		/// <summary>
		/// example: 0xdde5fc46c5d8bcbd58207bc9f267bf43298e23791a326ff02661e99790da9996b3e0dd912c0b8202d389d282c56e4d11eb2dec4898a32b6b165f1f4cae6aa0079498eab50293f3b8defbf6af11bb75f0408a563ddfc26a3323d1ff5f9849e95d5f034d88a757ddea032c75c00708c9ff34d2207f997cc7d93fd1fa160a6bfaf62a54e31f9fe67ab95752106ba9d185bfdc9b6dc3e17427f844ee74e5c09b17b83ad6e8fc7360f5c7c3e4e1939e77a6374bee57d1fa6b2322b11ad56ad0398302de9b26d6fbfe414aa416bff141fad9d4af6aea19322e47595e342cd377403f417dfd396ab5f151095a5535f51cbc34a40ce9648927b7d1d72ab9daf253e31daf
		/// </summary>
		[DataMember(Name = "logs_bloom", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "logs_bloom")]
		public string LogsBloom { get; set; }

		/// <summary>
		/// example: 0xe4c7bf3aff7ad07f9e80d57f7189f0252592fee6321c2a9bd9b09b6ce0690d27
		/// </summary>
		[DataMember(Name = "transactions_root", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transactions_root")]
		public string TransactionsRoot { get; set; }

		/// <summary>
		/// example: 0x49e3bfe7b618e27fde8fa08884803a8458b502c6534af69873a3cc926a7c724b
		/// </summary>
		[DataMember(Name = "state_root", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "state_root")]
		public string StateRoot { get; set; }

		/// <summary>
		/// example: 0x7cf43d7e837284f036cf92c56973f5e27bdd253ca46168fa195a6b07fa719f23
		/// </summary>
		[DataMember(Name = "receipts_root", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "receipts_root")]
		public string ReceiptsRoot { get; set; }

		/// <summary>
		/// The address of the miner
		/// example: 0xea674fdde714fd979de3edf0f56aa9716b898ec8
		/// </summary>
		[DataMember(Name = "miner", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "miner")]
		public string Miner { get; set; }

		/// <summary>
		/// The difficulty of the block
		/// example: 7253857437305950
		/// </summary>
		[DataMember(Name = "difficulty", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "difficulty")]
		public string Difficulty { get; set; }

		/// <summary>
		/// The total difficulty
		/// example: 24325637817906576196890
		/// </summary>
		[DataMember(Name = "total_difficulty", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "total_difficulty")]
		public string TotalDifficulty { get; set; }

		/// <summary>
		/// The block size
		/// example: 61271
		/// </summary>
		[DataMember(Name = "size", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "size")]
		public string Size { get; set; }

		/// <summary>
		/// example: 0x65746865726d696e652d6575726f70652d7765737433
		/// </summary>
		[DataMember(Name = "extra_data", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "extra_data")]
		public string ExtraData { get; set; }

		/// <summary>
		/// The gas limit
		/// example: 14977947
		/// </summary>
		[DataMember(Name = "gas_limit", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "gas_limit")]
		public string GasLimit { get; set; }

		/// <summary>
		/// The gas used
		/// example: 14964688
		/// </summary>
		[DataMember(Name = "gas_used", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "gas_used")]
		public string GasUsed { get; set; }

		/// <summary>
		/// The number of transactions in the block
		/// example: 252
		/// </summary>
		[DataMember(Name = "transaction_count", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transaction_count")]
		public string TransactionCount { get; set; }

		/// <summary>
		/// The transactions in the block
		/// </summary>
		[DataMember(Name = "transactions", EmitDefaultValue = false)]
		[JsonProperty(PropertyName = "transactions")]
		public List<BlockTransaction> Transactions { get; set; }


		/// <summary>
		/// Get the string presentation of the object
		/// </summary>
		/// <returns>String presentation of the object</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("class Block{");
			sb.Append("  Timestamp ").Append(Timestamp).Append("\n");
			sb.Append("  Number ").Append(Number).Append("\n");
			sb.Append("  Hash ").Append(Hash).Append("\n");
			sb.Append("  ParentHash ").Append(ParentHash).Append("\n");
			sb.Append("  Nonce ").Append(Nonce).Append("\n");
			sb.Append("  Sha3Uncles ").Append(Sha3Uncles).Append("\n");
			sb.Append("  LogsBloom ").Append(LogsBloom).Append("\n");
			sb.Append("  TransactionsRoot ").Append(TransactionsRoot).Append("\n");
			sb.Append("  StateRoot ").Append(StateRoot).Append("\n");
			sb.Append("  ReceiptsRoot ").Append(ReceiptsRoot).Append("\n");
			sb.Append("  Miner ").Append(Miner).Append("\n");
			sb.Append("  Difficulty ").Append(Difficulty).Append("\n");
			sb.Append("  TotalDifficulty ").Append(TotalDifficulty).Append("\n");
			sb.Append("  Size ").Append(Size).Append("\n");
			sb.Append("  ExtraData ").Append(ExtraData).Append("\n");
			sb.Append("  GasLimit ").Append(GasLimit).Append("\n");
			sb.Append("  GasUsed ").Append(GasUsed).Append("\n");
			sb.Append("  TransactionCount ").Append(TransactionCount).Append("\n");
			sb.Append("  Transactions ").Append(Transactions).Append("\n");
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