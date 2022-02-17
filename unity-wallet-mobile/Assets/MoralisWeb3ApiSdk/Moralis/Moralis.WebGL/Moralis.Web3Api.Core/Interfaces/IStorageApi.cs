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
	public interface IStorageApi
	{
		/// <summary>
		/// Uploads multiple files and place them in a folder directory
		/// 
		/// </summary>
		/// <param name="abi">Array of JSON and Base64 Supported</param>
		/// <returns>Returns the path to the uploaded files</returns>
		UniTask<List<IpfsFile>> UploadFolder (List<IpfsFileRequest> abi);

	}
}
