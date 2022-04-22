
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using WebRequest = Moralis.WebGL.SolanaApi.Models.WebRequest;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Moralis.WebGL.SolanaApi.Interfaces;

namespace Moralis.WebGL.SolanaApi
{
    /// <summary>
    /// A universal implementation of <see cref="IWebClient"/>.
    /// </summary>
    public class UniversalWebClient : IWebClient
    {
        static HashSet<string> ContentHeaders { get; } = new HashSet<string>
        {
            { "Allow" },
            { "Content-Disposition" },
            { "Content-Encoding" },
            { "Content-Language" },
            { "Content-Length" },
            { "Content-Location" },
            { "Content-MD5" },
            { "Content-Range" },
            { "Content-Type" },
            { "Expires" },
            { "Last-Modified" }
        };

        static List<string> allowedHeaders { get; } = new List<string>
        {
            "x-parse-application-id",
            "x-parse-installation-id",
            "content-type"
        };

        public UniversalWebClient() { }

        public async UniTask<Tuple<HttpStatusCode, Dictionary<string, string>, string>> ExecuteAsync(WebRequest httpRequest) 
        {
            Tuple<HttpStatusCode, Dictionary<string, string>, string> result = default;

            UnityWebRequest webRequest;
            
            switch (httpRequest.Method)
            {
                case "DELETE":
                    webRequest = UnityWebRequest.Delete(httpRequest.Target);
                    break;
                case "POST":
                    webRequest = CreatePostRequest(httpRequest);
                    break;
                case "PUT":
                    webRequest = CreatePutRequest(httpRequest);
                    break;
                default:
                    webRequest = UnityWebRequest.Get(httpRequest.Target);
                    break;
            }

            if (httpRequest.Headers != null)
            {
                if (httpRequest.Headers != null)
                {
                    foreach (KeyValuePair<string, string> header in httpRequest.Headers)
                    {
                        if (webRequest.GetRequestHeader(header.Key) != null) continue;

                        if (!String.IsNullOrWhiteSpace(header.Value) &&
                            allowedHeaders.Contains(header.Key.ToLower()))
                        {
                            webRequest.SetRequestHeader(header.Key, header.Value);
                        }
                    }
                }
            }

            try
            {
                await webRequest.SendWebRequest();
            }
            catch (Exception exp)
            {
                Debug.LogError($"Error: {exp.Message}");
            }

            HttpStatusCode responseStatus = HttpStatusCode.BadRequest;
            string responseText = null;

            if (Enum.IsDefined(typeof(HttpStatusCode), (int)webRequest.responseCode))
            {
                responseStatus = (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), webRequest.responseCode);
            }

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error Getting Wallet Info: " + webRequest.error);
                responseText = webRequest.error;
            }
            else
            {
                responseText = webRequest.downloadHandler.text;
            }
            
            result = new Tuple<HttpStatusCode, Dictionary<string, string>, string>(responseStatus, webRequest.GetResponseHeaders(), responseText);

            return result;
        }

        private UnityWebRequest CreatePostRequest(WebRequest httpRequest)
        {
            //string requestData = null;
            var req = new UnityWebRequest(httpRequest.Target, "POST");
            Stream data = httpRequest.Data;

            if ((httpRequest.Data is null && httpRequest.Method.ToLower().Equals("post") ? new MemoryStream(new byte[0]) : httpRequest.Data) is Stream { } adjData)
            {
                data = adjData;
            }

            byte[] buffer = new byte[data.Length];
            data.Read(buffer, 0, buffer.Length);
            data.Position = 0;

            //requestData = Encoding.UTF8.GetString(buffer);
            req.uploadHandler = (UploadHandler)new UploadHandlerRaw(buffer);
            req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            return req; // UnityWebRequest.Post(httpRequest.Target, requestData);
        }

        private UnityWebRequest CreatePutRequest(WebRequest httpRequest)
        {
            //string requestData = null;

            var req = new UnityWebRequest(httpRequest.Target, "PUT");
            Stream data = httpRequest.Data;

            if ((httpRequest.Data is null && httpRequest.Method.ToLower().Equals("put") ? new MemoryStream(new byte[0]) : httpRequest.Data) is Stream { } adjData)
            {
                data = adjData;
            }

            byte[] buffer = new byte[data.Length];
            data.Read(buffer, 0, buffer.Length);
            data.Position = 0;

            //requestData = Encoding.UTF8.GetString(buffer);

            req.uploadHandler = (UploadHandler)new UploadHandlerRaw(buffer);
            req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            return req; // return UnityWebRequest.Put(httpRequest.Target, requestData);
        }
    }
}
