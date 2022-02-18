using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Core.Editor
{
    class ServiceFlagRequest: IServiceFlagRequest
    {
        const string k_ServiceFlagsKey = "service_flags";

        DefaultCdnEndpointConfiguration m_DefaultCdnEndpointConfiguration;

        public IAsyncOperation<IServiceFlags> FetchServiceFlags()
        {
            var resultAsyncOp = new AsyncOperation<IServiceFlags>();
            try
            {
                resultAsyncOp.SetInProgress();
                var cdnEndpoint = new DefaultCdnConfiguredEndpoint();
                var configurationRequestTask = cdnEndpoint.GetConfiguration();
                configurationRequestTask.Completed += configOperation => QueryProjectFlags(configOperation, resultAsyncOp);
            }
            catch (Exception ex)
            {
                resultAsyncOp.Fail(ex);
            }

            return resultAsyncOp;
        }

        static void QueryProjectFlags(IAsyncOperation<DefaultCdnEndpointConfiguration> configurationRequestTask, AsyncOperation<IServiceFlags> resultAsyncOp)
        {
            try
            {
#if ENABLE_EDITOR_GAME_SERVICES
                var organizationKey = CloudProjectSettings.organizationKey;
#else
                var organizationKey = CloudProjectSettings.organizationId;
#endif
                var projectApiUrl = configurationRequestTask.Result.BuildProjectApiUrl(organizationKey, CloudProjectSettings.projectId);
                var getProjectFlagsRequest = new UnityWebRequest(projectApiUrl,
                    UnityWebRequest.kHttpVerbGET)
                {
                    downloadHandler = new DownloadHandlerBuffer()
                };
                getProjectFlagsRequest.SetRequestHeader("AUTHORIZATION", $"Bearer {CloudProjectSettings.accessToken}");
                var operation = getProjectFlagsRequest.SendWebRequest();
                operation.completed += op => OnFetchServiceFlagsCompleted(getProjectFlagsRequest, resultAsyncOp);
            }
            catch (Exception ex)
            {
                resultAsyncOp.Fail(ex);
            }
        }

        static void OnFetchServiceFlagsCompleted(UnityWebRequest getServiceFlagsRequest, AsyncOperation<IServiceFlags> resultAsyncOp)
        {
            try
            {
                resultAsyncOp.Succeed(ExtractServiceFlagsFromUnityWebRequest(getServiceFlagsRequest));
            }
            catch (Exception ex)
            {
                resultAsyncOp.Fail(ex);
            }
            finally
            {
                getServiceFlagsRequest.Dispose();
            }
        }

        static IServiceFlags ExtractServiceFlagsFromUnityWebRequest(UnityWebRequest unityWebRequest)
        {
            IDictionary<string, object> flags = null;
            if (UnityWebRequestHelper.IsUnityWebRequestReadyForTextExtract(unityWebRequest, out var jsonContent))
            {
                try
                {
                    var jsonEntries = (IDictionary<string, object>)MiniJson.Deserialize(jsonContent);
                    flags = (IDictionary<string, object>)jsonEntries[k_ServiceFlagsKey];
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception occurred when fetching service flags:\n{ex.Message}");
                    flags = new Dictionary<string, object>();
                }
            }

            return new ServiceFlags(flags);
        }
    }
}
