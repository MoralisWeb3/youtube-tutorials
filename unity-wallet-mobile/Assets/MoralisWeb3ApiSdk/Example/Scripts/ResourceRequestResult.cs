/**
 *           Module: ResourceData.cs
 *  Descriptiontion: Define a set of objects that represent a known response from 
 *                   a Moralis Cloud function used to load remote resources and so 
 *                   by-pass CORS issues when loading resources from remote servers.
 *                   NOTE: This was designed spoecifically to retrieve image files for the
 *                         sample TokenListController. Other resource may require modification.
 *           Author: Moralis Web3 Technology AB, 559307-5988 - David B. Goodrich
 *  
 *  Sample Moralis Cloud Function used to retrieve remote resources
 *  =================================================================
     Moralis.Cloud.define("loadResource", async (request) => {
      const logger = Moralis.Cloud.getLogger();
  
      return await Moralis.Cloud.httpRequest({
        url: request.params.url
      }).then(function(httpResponse) {
        let resp = {status: httpResponse.status, headers: httpResponse.headers, data: JSON.stringify(httpResponse.buffer)};
        return resp;
      },function(httpResponse) {
        // Error occurred
        logger.error('Request failed with response code ' + httpResponse.status);
        return httpResponse;
      });
    }, {
	    fields : ["url"]
    });
 *  
 *  
 *  MIT License
 *  
 *  Copyright (c) 2021 Moralis Web3 Technology AB, 559307-5988
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Define a set of objects that represent a known response from 
/// a Moralis Cloud function used to load remote resources and so 
/// by-pass CORS issues when loading resources from remote servers.
/// NOTE: This was designed spoecifically to retrieve image files for the
///       sample TokenListController. Other resource may require modification.
/// </summary>
namespace MoralisWeb3ApiSdk.Example.Scripts
{
    /// <summary>
    /// Raw data element of the resource loaded.
    /// </summary>
    public class ResourceData
    {
        public string type;
        public byte[] data;
    }

    /// <summary>
    /// Response body from the Cloud Function.
    /// </summary>
    public class ResourceResponse
    {
        public int status;
        public Dictionary<string, string> headers;
        public string data;

        public ResourceData resourceData
        {
            get
            {
                return JsonConvert.DeserializeObject<ResourceData>(data);
            }
        }
    }

    /// <summary>
    /// Result wrapper added by the server after the fact. 
    /// </summary>
    public class ResourceRequestResult
    {
        public ResourceResponse result;
    }
}
