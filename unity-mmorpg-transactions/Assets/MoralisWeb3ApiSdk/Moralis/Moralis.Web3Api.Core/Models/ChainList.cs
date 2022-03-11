using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.Web3Api.Models
{
	[DataContract]
	public enum ChainList
	{
		eth = 0x1,
		ropsten = 0x3,
		rinkeby = 0x4,
		goerli = 0x5,
		kovan = 0x2a,
		polygon = 0x89,
		mumbai = 0x13881,
		bsc = 0x38,
		bsc_testnet = 0x61,
		avalanche = 0xa86a,
		avalanche_testnet = 0xa869,
		fantom = 0xfa
	};

}