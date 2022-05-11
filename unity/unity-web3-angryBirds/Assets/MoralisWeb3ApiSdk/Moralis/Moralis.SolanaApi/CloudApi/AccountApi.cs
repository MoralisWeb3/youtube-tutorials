using Moralis.SolanaApi.Client;
using Moralis.SolanaApi.Interfaces;
using Moralis.SolanaApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Moralis.SolanaApi.CloudApi
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
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling GetNFTMetadata");

			var postBody = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();

			var path = "/functions/sol-balance";
			postBody.Add("network", ApiClient.ParameterToString(network.ToString()));
			if (address != null) postBody.Add("address", ApiClient.ParameterToString(address));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			//IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, null, bodyData, headerParams, null, null, authSettings));
			HttpResponseMessage response = await ApiClient.CallApi(path, HttpMethod.Post, bodyData, headerParams, null, authSettings);

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTMetadata: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTMetadata: " + response.ReasonPhrase, response.ReasonPhrase);

			return ((CloudFunctionResult<NativeBalance>)(await ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<NativeBalance>), response.Headers))).Result;

		}

		public async Task<List<SplTokenBalanace>> GetSplTokens(NetworkTypes network, string address)
		{
			// Verify the required parameter 'pairAddress' is set
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling GetNFTMetadata");

			var postBody = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();

			var path = "/functions/sol-getSPL";
			postBody.Add("network", ApiClient.ParameterToString(network.ToString()));
			if (address != null) postBody.Add("address", ApiClient.ParameterToString(address));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			//IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, null, bodyData, headerParams, null, null, authSettings));
			HttpResponseMessage response = await ApiClient.CallApi(path, HttpMethod.Post, bodyData, headerParams, null, authSettings);

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTMetadata: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTMetadata: " + response.ReasonPhrase, response.ReasonPhrase);

			return ((CloudFunctionResult<List<SplTokenBalanace>>)(await ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<List<SplTokenBalanace>>), response.Headers))).Result;

		}

		public async Task<List<SplNft>> GetNFTs(NetworkTypes network, string address)
		{
			// Verify the required parameter 'pairAddress' is set
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling GetNFTMetadata");

			var postBody = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();

			var path = "/functions/sol-getNFTs";
			postBody.Add("network", ApiClient.ParameterToString(network.ToString()));
			if (address != null) postBody.Add("address", ApiClient.ParameterToString(address));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			//IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, null, bodyData, headerParams, null, null, authSettings));
			HttpResponseMessage response = await ApiClient.CallApi(path, HttpMethod.Post, bodyData, headerParams, null, authSettings);

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTMetadata: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTMetadata: " + response.ReasonPhrase, response.ReasonPhrase);

			return ((CloudFunctionResult<List<SplNft>>)(await ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<List<SplNft>>), response.Headers))).Result;

		}

		public async Task<Portfolio> GetPortfolio(NetworkTypes network, string address)
		{
			// Verify the required parameter 'pairAddress' is set
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling GetNFTMetadata");

			var postBody = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();

			var path = "/functions/sol-getPortfolio";
			postBody.Add("network", ApiClient.ParameterToString(network.ToString()));
			if (address != null) postBody.Add("address", ApiClient.ParameterToString(address));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			//IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, null, bodyData, headerParams, null, null, authSettings));
			HttpResponseMessage response = await ApiClient.CallApi(path, HttpMethod.Post, bodyData, headerParams, null, authSettings);

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTMetadata: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTMetadata: " + response.ReasonPhrase, response.ReasonPhrase);

			return ((CloudFunctionResult<Portfolio>)(await ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<Portfolio>), response.Headers))).Result;
		}
	}
}
