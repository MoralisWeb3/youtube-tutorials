using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

namespace Unity.Services.Core.Editor
{
    class UnityWebRequestClient : IHttpClient
    {
        /// <summary>
        /// Key: The identifier of the service.
        /// Value: The configuration of the HTTP client for the service.
        /// </summary>
        readonly Dictionary<string, HttpServiceConfig> m_ServiceIdToConfig
            = new Dictionary<string, HttpServiceConfig>();

        /// <inheritdoc/>
        public string GetBaseUrlFor(string serviceId)
        {
            return m_ServiceIdToConfig[serviceId].BaseUrl;
        }

        /// <inheritdoc/>
        public HttpOptions GetDefaultOptionsFor(string serviceId)
        {
            return m_ServiceIdToConfig[serviceId].DefaultOptions;
        }

        /// <inheritdoc/>
        public HttpRequest CreateRequestForService(string serviceId, string resourcePath)
        {
            var serviceConfig = m_ServiceIdToConfig[serviceId];
            var url = CombinePaths(serviceConfig.BaseUrl, resourcePath);
            var request = new HttpRequest()
                .SetUrl(url)
                .SetOptions(serviceConfig.DefaultOptions);

            return request;
        }

        internal static string CombinePaths(string path1, string path2)
        {
            //Replace '\' by '/' to unify separators used in the URL and make sure it is compatible with all platforms.
            return Path.Combine(path1, path2)
                .Replace('\\', '/');
        }

        /// <inheritdoc/>
        public IAsyncOperation<ReadOnlyHttpResponse> Send(HttpRequest request)
        {
            var operation = new AsyncOperation<ReadOnlyHttpResponse>();
            operation.SetInProgress();

            try
            {
                var webRequest = ConvertToWebRequest(request);
                webRequest.SendWebRequest()
                    .completed += OnWebRequestCompleted;
            }
            catch (Exception reason)
            {
                operation.Fail(reason);
            }

            return operation;

            void OnWebRequestCompleted(UnityEngine.AsyncOperation unityOperation)
            {
                var callbackWebRequest = ((UnityWebRequestAsyncOperation)unityOperation).webRequest;
                var response = ConvertToResponse(callbackWebRequest)
                    .SetRequest(request);
                var responseHandle = new ReadOnlyHttpResponse(response);
                callbackWebRequest.Dispose();

                operation.Succeed(responseHandle);
            }
        }

        static UnityWebRequest ConvertToWebRequest(HttpRequest request)
        {
            var webRequest = new UnityWebRequest(request.Url, request.Method)
            {
                downloadHandler = new DownloadHandlerBuffer(),
                redirectLimit = request.Options.RedirectLimit,
                timeout = request.Options.RequestTimeoutInSeconds
            };

            if (!(request.Body is null)
                && request.Body.Length > 0)
            {
                webRequest.uploadHandler = new UploadHandlerRaw(request.Body);
            }

            if (!(request.Headers is null))
            {
                foreach (var header in request.Headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            return webRequest;
        }

        static HttpResponse ConvertToResponse(UnityWebRequest webRequest)
        {
            var response = new HttpResponse()
                .SetHeaders(webRequest.GetResponseHeaders())
                .SetData(webRequest.downloadHandler?.data)
                .SetStatusCode(webRequest.responseCode)
                .SetErrorMessage(webRequest.error)
#if UNITY_2020_2_OR_NEWER
                .SetIsHttpError(webRequest.result == UnityWebRequest.Result.ProtocolError)
                .SetIsNetworkError(webRequest.result == UnityWebRequest.Result.ConnectionError);
#else
                .SetIsHttpError(webRequest.isHttpError)
                .SetIsNetworkError(webRequest.isNetworkError);
#endif

            return response;
        }

        internal void SetServiceConfig(HttpServiceConfig config)
        {
            m_ServiceIdToConfig[config.ServiceId] = config;
        }
    }
}
