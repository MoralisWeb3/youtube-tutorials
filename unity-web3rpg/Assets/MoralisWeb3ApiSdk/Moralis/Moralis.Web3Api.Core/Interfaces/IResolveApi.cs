using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Moralis.Web3Api.Models;

namespace Moralis.Web3Api.Interfaces
{
	/// <summary>
	/// Represents a collection of functions to interact with the API endpoints
	/// </summary>
	public interface IResolveApi
	{
		/// <summary>
		/// Resolves an Unstoppable domain and returns the address
		/// 
		/// </summary>
		/// <param name="domain">Domain to be resolved</param>
		/// <param name="currency">The currency to query</param>
		/// <returns>Returns an address</returns>
		Task<Resolve> ResolveDomain (string domain, string currency=null);

		/// <summary>
		/// Resolves an ETH address and find the ENS name
		/// 
		/// </summary>
		/// <param name="address">The address to be resolved</param>
		/// <returns>Returns an ENS</returns>
		Task<Ens> ResolveAddress (string address);

	}
}
