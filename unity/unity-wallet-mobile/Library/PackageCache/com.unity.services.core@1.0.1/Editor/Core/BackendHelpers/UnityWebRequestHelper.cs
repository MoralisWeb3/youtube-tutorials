using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Core.Editor
{
    static class UnityWebRequestHelper
    {
            internal static bool IsUnityWebRequestReadyForTextExtract(UnityWebRequest unityWebRequest, out string downloadHandlerText)
            {
#if UNITY_2020_1_OR_NEWER
                var result = unityWebRequest != null && unityWebRequest.result == UnityWebRequest.Result.Success;
#else
                var result = unityWebRequest != null &&
                    !unityWebRequest.isHttpError &&
                    !unityWebRequest.isNetworkError;
#endif
                if (result)
                {
                    downloadHandlerText = unityWebRequest.downloadHandler?.text;
                    return !string.IsNullOrEmpty(downloadHandlerText);
                }
                downloadHandlerText = null;
                return false;
            }
    }
}
