/**
*            Module: NativeApi.cs
*       Description: Represents a collection of functions to interact with the API endpoints
*            Author: Moralis Web3 Technology AB, 559307-5988 - David B. Goodrich
*  
* NOTE: THIS FILE HAS BEEN AUTOMATICALLY GENERATED. ANY CHANGES MADE TO THIS 
* FILE WILL BE LOST
*
* MIT License
*  
* Copyright (c) 2022 Moralis Web3 Technology AB, 559307-5988
*  
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/ 
            using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using Moralis.Web3Api.Client;
using Moralis.Web3Api.Interfaces;
using Moralis.Web3Api.Models;

namespace Moralis.Web3Api.CloudApi
{
	/// <summary>
	/// Represents a collection of functions to interact with the API endpoints
	/// </summary>
	public class NativeApi : INativeApi
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NativeApi"/> class.
		/// </summary>
		/// <param name="apiClient"> an instance of ApiClient (optional)</param>
		/// <returns></returns>
		public NativeApi(ApiClient apiClient = null)
		{
			if (apiClient == null) // use the default one in Configuration
				this.ApiClient = Configuration.DefaultApiClient; 
			else
				this.ApiClient = apiClient;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NativeApi"/> class.
		/// </summary>
		/// <returns></returns>
		public NativeApi(String basePath)
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
		public ApiClient ApiClient {get; set;}


		/// <summary>
		/// Gets the contents of a block by block hash
		/// </summary>
		/// <param name="blockNumberOrHash">The block hash or block number</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="subdomain">The subdomain of the moralis server to use (Only use when selecting local devchain as chain)</param>
		/// <returns>Returns the contents of a block</returns>
		public async Task<Block> GetBlock (string blockNumberOrHash, ChainList chain, string subdomain=null)
		{

			// Verify the required parameter 'blockNumberOrHash' is set
			if (blockNumberOrHash == null) throw new ApiException(400, "Missing required parameter 'blockNumberOrHash' when calling GetBlock");

			var postBody = new Dictionary<String, String>();
			var queryParams = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();
			var formParams = new Dictionary<String, String>();
			var fileParams = new Dictionary<String, FileParameter>();

			var path = "/functions/getBlock";
			if (blockNumberOrHash != null) postBody.Add("block_number_or_hash", ApiClient.ParameterToString(blockNumberOrHash));
			if (subdomain != null) postBody.Add("subdomain", ApiClient.ParameterToString(subdomain));
			postBody.Add("chain", ApiClient.ParameterToHex((long)chain));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, queryParams, bodyData, headerParams, formParams, fileParams, authSettings));

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetBlock: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetBlock: " + response.ErrorMessage, response.ErrorMessage);

			return ((CloudFunctionResult<Block>)ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<Block>), response.Headers)).Result;
		}
		/// <summary>
		/// Gets the closest block of the provided date
		/// </summary>
		/// <param name="date">Unix date in miliseconds or a datestring (any format that is accepted by momentjs)</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="providerUrl">web3 provider url to user when using local dev chain</param>
		/// <returns>Returns the blocknumber and corresponding date and timestamp</returns>
		public async Task<BlockDate> GetDateToBlock (string date, ChainList chain, string providerUrl=null)
		{

			// Verify the required parameter 'date' is set
			if (date == null) throw new ApiException(400, "Missing required parameter 'date' when calling GetDateToBlock");

			var postBody = new Dictionary<String, String>();
			var queryParams = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();
			var formParams = new Dictionary<String, String>();
			var fileParams = new Dictionary<String, FileParameter>();

			var path = "/functions/getDateToBlock";
			if (date != null) postBody.Add("date", ApiClient.ParameterToString(date));
			if (providerUrl != null) postBody.Add("providerUrl", ApiClient.ParameterToString(providerUrl));
			postBody.Add("chain", ApiClient.ParameterToHex((long)chain));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, queryParams, bodyData, headerParams, formParams, fileParams, authSettings));

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetDateToBlock: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetDateToBlock: " + response.ErrorMessage, response.ErrorMessage);

			return ((CloudFunctionResult<BlockDate>)ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<BlockDate>), response.Headers)).Result;
		}
		/// <summary>
		/// Gets the logs from an address
		/// </summary>
		/// <param name="address">address</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="subdomain">The subdomain of the moralis server to use (Only use when selecting local devchain as chain)</param>
		/// <param name="blockNumber">The block number
		/// * Provide the param 'block_numer' or ('from_block' and / or 'to_block')
		/// * If 'block_numer' is provided in conbinaison with 'from_block' and / or 'to_block', 'block_number' will will be used
		/// </param>
		/// <param name="fromBlock">The minimum block number from where to get the logs
		/// * Provide the param 'block_numer' or ('from_block' and / or 'to_block')
		/// * If 'block_numer' is provided in conbinaison with 'from_block' and / or 'to_block', 'block_number' will will be used
		/// </param>
		/// <param name="toBlock">The maximum block number from where to get the logs
		/// * Provide the param 'block_numer' or ('from_block' and / or 'to_block')
		/// * If 'block_numer' is provided in conbinaison with 'from_block' and / or 'to_block', 'block_number' will will be used
		/// </param>
		/// <param name="fromDate">The date from where to get the logs (any format that is accepted by momentjs)
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// * If 'from_date' and the block params are provided, the block params will be used. Please refer to the blocks params sections (block_number,from_block and to_block) on how to use them
		/// </param>
		/// <param name="toDate">Get the logs to this date (any format that is accepted by momentjs)
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// * If 'to_date' and the block params are provided, the block params will be used. Please refer to the blocks params sections (block_number,from_block and to_block) on how to use them
		/// </param>
		/// <param name="topic0">topic0</param>
		/// <param name="topic1">topic1</param>
		/// <param name="topic2">topic2</param>
		/// <param name="topic3">topic3</param>
		/// <returns>Returns the logs of an address</returns>
		public async Task<LogEventByAddress> GetLogsByAddress (string address, ChainList chain, string subdomain=null, string blockNumber=null, string fromBlock=null, string toBlock=null, string fromDate=null, string toDate=null, string topic0=null, string topic1=null, string topic2=null, string topic3=null)
		{

			// Verify the required parameter 'address' is set
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling GetLogsByAddress");

			var postBody = new Dictionary<String, String>();
			var queryParams = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();
			var formParams = new Dictionary<String, String>();
			var fileParams = new Dictionary<String, FileParameter>();

			var path = "/functions/getLogsByAddress";
			if (address != null) postBody.Add("address", ApiClient.ParameterToString(address));
			if (subdomain != null) postBody.Add("subdomain", ApiClient.ParameterToString(subdomain));
			if (blockNumber != null) postBody.Add("block_number", ApiClient.ParameterToString(blockNumber));
			if (fromBlock != null) postBody.Add("from_block", ApiClient.ParameterToString(fromBlock));
			if (toBlock != null) postBody.Add("to_block", ApiClient.ParameterToString(toBlock));
			if (fromDate != null) postBody.Add("from_date", ApiClient.ParameterToString(fromDate));
			if (toDate != null) postBody.Add("to_date", ApiClient.ParameterToString(toDate));
			if (topic0 != null) postBody.Add("topic0", ApiClient.ParameterToString(topic0));
			if (topic1 != null) postBody.Add("topic1", ApiClient.ParameterToString(topic1));
			if (topic2 != null) postBody.Add("topic2", ApiClient.ParameterToString(topic2));
			if (topic3 != null) postBody.Add("topic3", ApiClient.ParameterToString(topic3));
			postBody.Add("chain", ApiClient.ParameterToHex((long)chain));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, queryParams, bodyData, headerParams, formParams, fileParams, authSettings));

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetLogsByAddress: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetLogsByAddress: " + response.ErrorMessage, response.ErrorMessage);

			return ((CloudFunctionResult<LogEventByAddress>)ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<LogEventByAddress>), response.Headers)).Result;
		}
		/// <summary>
		/// Gets NFT transfers by block number or block hash
		/// </summary>
		/// <param name="blockNumberOrHash">The block hash or block number</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="subdomain">The subdomain of the moralis server to use (Only use when selecting local devchain as chain)</param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <param name="cursor">The cursor returned in the last response (for getting the next page)
		/// </param>
		/// <returns>Returns the contents of a block</returns>
		public async Task<NftTransferCollection> GetNFTTransfersByBlock (string blockNumberOrHash, ChainList chain, string subdomain=null, int? offset=null, int? limit=null, string cursor=null)
		{

			// Verify the required parameter 'blockNumberOrHash' is set
			if (blockNumberOrHash == null) throw new ApiException(400, "Missing required parameter 'blockNumberOrHash' when calling GetNFTTransfersByBlock");

			var postBody = new Dictionary<String, String>();
			var queryParams = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();
			var formParams = new Dictionary<String, String>();
			var fileParams = new Dictionary<String, FileParameter>();

			var path = "/functions/getNFTTransfersByBlock";
			if (blockNumberOrHash != null) postBody.Add("block_number_or_hash", ApiClient.ParameterToString(blockNumberOrHash));
			if (subdomain != null) postBody.Add("subdomain", ApiClient.ParameterToString(subdomain));
			if (offset != null) postBody.Add("offset", ApiClient.ParameterToString(offset));
			if (limit != null) postBody.Add("limit", ApiClient.ParameterToString(limit));
			if (cursor != null) postBody.Add("cursor", ApiClient.ParameterToString(cursor));
			postBody.Add("chain", ApiClient.ParameterToHex((long)chain));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, queryParams, bodyData, headerParams, formParams, fileParams, authSettings));

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTTransfersByBlock: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetNFTTransfersByBlock: " + response.ErrorMessage, response.ErrorMessage);

			return ((CloudFunctionResult<NftTransferCollection>)ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<NftTransferCollection>), response.Headers)).Result;
		}
		/// <summary>
		/// Gets the contents of a block transaction by hash
		/// </summary>
		/// <param name="transactionHash">The transaction hash</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="subdomain">The subdomain of the moralis server to use (Only use when selecting local devchain as chain)</param>
		/// <returns>Transaction details by transaction hash</returns>
		public async Task<BlockTransaction> GetTransaction (string transactionHash, ChainList chain, string subdomain=null)
		{

			// Verify the required parameter 'transactionHash' is set
			if (transactionHash == null) throw new ApiException(400, "Missing required parameter 'transactionHash' when calling GetTransaction");

			var postBody = new Dictionary<String, String>();
			var queryParams = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();
			var formParams = new Dictionary<String, String>();
			var fileParams = new Dictionary<String, FileParameter>();

			var path = "/functions/getTransaction";
			if (transactionHash != null) postBody.Add("transaction_hash", ApiClient.ParameterToString(transactionHash));
			if (subdomain != null) postBody.Add("subdomain", ApiClient.ParameterToString(subdomain));
			postBody.Add("chain", ApiClient.ParameterToHex((long)chain));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, queryParams, bodyData, headerParams, formParams, fileParams, authSettings));

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetTransaction: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetTransaction: " + response.ErrorMessage, response.ErrorMessage);

			return ((CloudFunctionResult<BlockTransaction>)ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<BlockTransaction>), response.Headers)).Result;
		}
		/// <summary>
		/// Gets events in descending order based on block number
		/// </summary>
		/// <param name="address">address</param>
		/// <param name="topic">The topic of the event</param>
		/// <param name="abi">ABI of the specific event</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="subdomain">The subdomain of the moralis server to use (Only use when selecting local devchain as chain)</param>
		/// <param name="providerUrl">web3 provider url to user when using local dev chain</param>
		/// <param name="fromBlock">The minimum block number from where to get the logs
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// </param>
		/// <param name="toBlock">The maximum block number from where to get the logs.
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <param name="fromDate">The date from where to get the logs (any format that is accepted by momentjs)
		/// * Provide the param 'from_block' or 'from_date'
		/// * If 'from_date' and 'from_block' are provided, 'from_block' will be used.
		/// </param>
		/// <param name="toDate">Get the logs to this date (any format that is accepted by momentjs)
		/// * Provide the param 'to_block' or 'to_date'
		/// * If 'to_date' and 'to_block' are provided, 'to_block' will be used.
		/// </param>
		/// <param name="offset">offset</param>
		/// <param name="limit">limit</param>
		/// <returns>Returns a collection of events by topic</returns>
		public async Task<List<LogEvent>> GetContractEvents (string address, string topic, object abi, ChainList chain, string subdomain=null, string providerUrl=null, int? fromBlock=null, int? toBlock=null, string fromDate=null, string toDate=null, int? offset=null, int? limit=null)
		{

			// Verify the required parameter 'address' is set
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling GetContractEvents");

			// Verify the required parameter 'topic' is set
			if (topic == null) throw new ApiException(400, "Missing required parameter 'topic' when calling GetContractEvents");

			// Verify the required parameter 'abi' is set
			if (abi == null) throw new ApiException(400, "Missing required parameter 'abi' when calling GetContractEvents");

			var postBody = new Dictionary<String, object>();
			var queryParams = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();
			var formParams = new Dictionary<String, String>();
			var fileParams = new Dictionary<String, FileParameter>();

			var path = "/functions/getContractEvents";
			if (address != null) postBody.Add("address", ApiClient.ParameterToString(address));
			if (topic != null) postBody.Add("topic", ApiClient.ParameterToString(topic));
			if (abi != null) postBody.Add("abi", abi);
			if (subdomain != null) postBody.Add("subdomain", ApiClient.ParameterToString(subdomain));
			if (providerUrl != null) postBody.Add("providerUrl", ApiClient.ParameterToString(providerUrl));
			if (fromBlock != null) postBody.Add("from_block", ApiClient.ParameterToString(fromBlock));
			if (toBlock != null) postBody.Add("to_block", ApiClient.ParameterToString(toBlock));
			if (fromDate != null) postBody.Add("from_date", ApiClient.ParameterToString(fromDate));
			if (toDate != null) postBody.Add("to_date", ApiClient.ParameterToString(toDate));
			if (offset != null) postBody.Add("offset", ApiClient.ParameterToString(offset));
			if (limit != null) postBody.Add("limit", ApiClient.ParameterToString(limit));
			postBody.Add("chain", ApiClient.ParameterToHex((long)chain));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, queryParams, bodyData, headerParams, formParams, fileParams, authSettings));

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling GetContractEvents: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling GetContractEvents: " + response.ErrorMessage, response.ErrorMessage);

			LogEventResponse resp = ((CloudFunctionResult<LogEventResponse>)ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<LogEventResponse>), response.Headers)).Result;
			//return ((CloudFunctionResult<List<LogEvent>>)ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<List<LogEvent>>), response.Headers)).Result;
			return resp.Events;
		}
		/// <summary>
		/// Runs a given function of a contract abi and returns readonly data
		/// </summary>
		/// <param name="address">address</param>
		/// <param name="functionName">function_name</param>
		/// <param name="abi">Body</param>
		/// <param name="chain">The chain to query</param>
		/// <param name="subdomain">The subdomain of the moralis server to use (Only use when selecting local devchain as chain)</param>
		/// <param name="providerUrl">web3 provider url to user when using local dev chain</param>
		/// <returns>Returns response of the function executed</returns>
		public async Task<string> RunContractFunction (string address, string functionName, RunContractDto abi, ChainList chain, string subdomain=null, string providerUrl=null)
		{
			// Verify the required parameter 'address' is set
			if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling RunContractFunction");

			// Verify the required parameter 'functionName' is set
			if (functionName == null) throw new ApiException(400, "Missing required parameter 'functionName' when calling RunContractFunction");

			// Verify the required parameter 'abi' is set
			if (abi == null) throw new ApiException(400, "Missing required parameter 'abi' when calling RunContractFunction");

			//			var postBody = new Dictionary<String, String>();
			var postBody = new Dictionary<String, object>();

			var queryParams = new Dictionary<String, String>();
			var headerParams = new Dictionary<String, String>();
			var formParams = new Dictionary<String, String>();

			var fileParams = new Dictionary<String, FileParameter>();

			var path = "/functions/runContractFunction";
			if (address != null) postBody.Add("address", ApiClient.ParameterToString(address));
			if (functionName != null) postBody.Add("function_name", ApiClient.ParameterToString(functionName));
			if (abi != null) postBody.Add("abi", abi.Abi);
			if (abi != null) postBody.Add("params", abi.Params);
			if (subdomain != null) postBody.Add("subdomain", ApiClient.ParameterToString(subdomain));
			if (providerUrl != null) postBody.Add("providerUrl", ApiClient.ParameterToString(providerUrl));
			postBody.Add("chain", ApiClient.ParameterToHex((long)chain));

			// Authentication setting, if any
			String[] authSettings = new String[] { "ApiKeyAuth" };

			string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

			IRestResponse response = (IRestResponse)(await ApiClient.CallApi(path, Method.POST, queryParams, bodyData, headerParams, formParams, fileParams, authSettings));

			if (((int)response.StatusCode) >= 400)
				throw new ApiException((int)response.StatusCode, "Error calling RunContractFunction: " + response.Content, response.Content);
			else if (((int)response.StatusCode) == 0)
				throw new ApiException((int)response.StatusCode, "Error calling RunContractFunction: " + response.ErrorMessage, response.ErrorMessage);


			object respObject = ((CloudFunctionResult<object>)ApiClient.Deserialize(response.Content, typeof(CloudFunctionResult<object>), response.Headers)).Result;

			return JsonConvert.SerializeObject(respObject);
		}
	}
}
