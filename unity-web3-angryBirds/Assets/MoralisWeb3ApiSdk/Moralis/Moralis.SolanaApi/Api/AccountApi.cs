using Moralis.SolanaApi.Client;
using Moralis.SolanaApi.Interfaces;
using Moralis.SolanaApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Moralis.SolanaApi.Api
{
    public class AccountApi : IAccountApi
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AccountApi"/> class.
		/// </summary>
		/// <param name="apiClient"> an instance of ApiClient (optional)</param>
		/// <returns></returns>
		public AccountApi(ApiClient apiClient = null)
		{
			if (apiClient == null) // use the default one in Configuration
				this.ApiClient = Configuration.DefaultApiClient;
			else
				this.ApiClient = apiClient;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountApi"/> class.
		/// </summary>
		/// <returns></returns>
		public AccountApi(String basePath)
		{
			this.ApiClient = new ApiClient(basePath);
		}

		/// <summary>
		/// Sets the base path of the API client.
		/// </summary>
		/// <param name="basePath">The base path</param>
		/// <value>The base path</value>
		public void SetBasePath(String basePath)
		{
			this.ApiClient.BasePath = basePath;
		}

		/// <summary>
		/// Gets the base path of the API client.
		/// </summary>
		/// <param name="basePath">The base path</param>
		/// <value>The base path</value>
		public String GetBasePath(String basePath)
		{
			return this.ApiClient.BasePath;
		}

		/// <summary>
		/// Gets or sets the API client.
		/// </summary>
		/// <value>An instance of the ApiClient</value>
		public ApiClient ApiClient { get; set; }

		public async Task<NativeBalance> Balance(NetworkTypes network, string address)
        {
			// Verify the required parameter 'pairAddress' is set
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling Balance");

			var headerParams = new Dictionary<String, String>();

			var path = "/account/{network}/{address}/balance";
			path = path.Replace("{" + "network" + "}", ApiClient.ParameterToString(network.ToString()));
			path = path.Replace("{" + "address" + "}", ApiClient.ParameterToString(address));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			//IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.GET, null, null, headerParams, null, null, authSettings));
			HttpResponseMessage response = await ApiClient.CallApi(path, HttpMethod.Get, null, headerParams, null, authSettings);

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling Balance: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling Balance: " + response.ReasonPhrase, response.ReasonPhrase);

			return (NativeBalance)(await ApiClient.Deserialize(response.Content, typeof(NativeBalance), response.Headers));

		}

		public async Task<List<SplTokenBalanace>> GetSplTokens(NetworkTypes network, string address)
		{
			// Verify the required parameter 'pairAddress' is set
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling GetSplTokens");

			var headerParams = new Dictionary<String, String>();

			var path = "/account/{network}/{address}/tokens";
			path = path.Replace("{" + "network" + "}", ApiClient.ParameterToString(network.ToString()));
			path = path.Replace("{" + "address" + "}", ApiClient.ParameterToString(address));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			//IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.GET, null, null, headerParams, null, null, authSettings));
			HttpResponseMessage response = await ApiClient.CallApi(path, HttpMethod.Get, null, headerParams, null, authSettings);

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetSplTokens: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetSplTokens: " + response.ReasonPhrase, response.ReasonPhrase);

			return (List<SplTokenBalanace>)(await ApiClient.Deserialize(response.Content, typeof(List<SplTokenBalanace>), response.Headers));
		}

		public async Task<List<SplNft>> GetNFTs(NetworkTypes network, string address)
        {
			// Verify the required parameter 'pairAddress' is set
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling GetNFTs");

			var headerParams = new Dictionary<String, String>();

			var path = "/account/{network}/{address}/nft";
			path = path.Replace("{" + "network" + "}", ApiClient.ParameterToString(network.ToString()));
			path = path.Replace("{" + "address" + "}", ApiClient.ParameterToString(address));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			//IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.GET, null, null, headerParams, null, null, authSettings));
			HttpResponseMessage response = await ApiClient.CallApi(path, HttpMethod.Get, null, headerParams, null, authSettings);

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTs: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTs: " + response.ReasonPhrase, response.ReasonPhrase);

			return (List<SplNft>)(await ApiClient .Deserialize(response.Content, typeof(List<SplNft>), response.Headers));
		}

		public async Task<Portfolio> GetPortfolio(NetworkTypes network, string address)
        {
			// Verify the required parameter 'pairAddress' is set
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling GetPortfolio");

			var headerParams = new Dictionary<String, String>();

			var path = "/account/{network}/{address}/portfolio";
			path = path.Replace("{" + "network" + "}", ApiClient.ParameterToString(network.ToString()));
			path = path.Replace("{" + "address" + "}", ApiClient.ParameterToString(address));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			//IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.GET, null, null, headerParams, null, null, authSettings));
			HttpResponseMessage response = await ApiClient.CallApi(path, HttpMethod.Get, null, headerParams, null, authSettings);

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetPortfolio: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetPortfolio: " + response.ReasonPhrase, response.ReasonPhrase);

			return (Portfolio)(await ApiClient.Deserialize(response.Content, typeof(Portfolio), response.Headers));
		}
	}
}
