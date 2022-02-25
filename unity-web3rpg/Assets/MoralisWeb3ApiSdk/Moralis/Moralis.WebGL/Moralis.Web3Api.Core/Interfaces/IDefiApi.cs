using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Moralis.WebGL.Web3Api.Models;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Web3Api.Interfaces
{
	/// <summary>
	/// Represents a collection of functions to interact with the API endpoints
	/// </summary>
	public interface IDefiApi
	{
		/// <summary>
		/// Get the liquidity reserves for a given pair address
		/// </summary>
		/// <param name="pairAddress">Liquidity pair address</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="toBlock">To get the reserves at this block number</param>
		/// <param name="toDate">Get the reserves to this date (any format that is accepted by momentjs)
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <param name="providerUrl">web3 provider url to user when using local dev chain</param>
		/// <returns>Returns the pair reserves</returns>
		UniTask<ReservesCollection> GetPairReserves (string pairAddress, ChainList chain, string toBlock=null, string toDate=null, string providerUrl=null);

		/// <summary>
		/// Fetches and returns pair data of the provided token0+token1 combination.
		/// The token0 and token1 options are interchangable (ie. there is no different outcome in "token0=WETH and token1=USDT" or "token0=USDT and token1=WETH")
		/// 
		/// </summary>
		/// <param name="exchange">The factory name or address of the token exchange</param>
		/// <param name="token0Address">Token0 address</param>
		/// <param name="token1Address">Token1 address</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="toBlock">To get the reserves at this block number</param>
		/// <param name="toDate">Get the reserves to this date (any format that is accepted by momentjs)
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <returns>Returns the pair address of the two tokens</returns>
		UniTask<ReservesCollection> GetPairAddress (string exchange, string token0Address, string token1Address, ChainList chain, string toBlock=null, string toDate=null);

	}
}
