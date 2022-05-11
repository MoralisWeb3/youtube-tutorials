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
	public interface IStorageApi
	{
		/// <summary>
		/// Uploads multiple files and place them in a folder directory
		/// 
		/// </summary>
		/// <param name="abi">Array of JSON and Base64 Supported</param>
		/// <returns>Returns the path to the uploaded files</returns>
		Task<List<IpfsFile>> UploadFolder (List<IpfsFileRequest> abi);

	}
}
