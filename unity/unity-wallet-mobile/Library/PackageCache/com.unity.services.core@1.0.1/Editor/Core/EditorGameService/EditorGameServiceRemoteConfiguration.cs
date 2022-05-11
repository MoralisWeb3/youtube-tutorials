using System;
using System.Text;
using UnityEditor;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Fetches and parses JSON config object or returns a newly constructed object on failure.
    /// If the config is successfully loaded it will be cached in SessionState
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize</typeparam>
    public class EditorGameServiceRemoteConfiguration<T> where T : new()
    {
        string m_ConfigUrl;
        string m_SessionStateKey;
        T m_CachedConfiguration;
        bool m_IsConfigurationLoaded;
        IAsyncOperation<string> m_FetchOperation;
        IHttpClient m_HttpClient;

        internal virtual IHttpClient GetHttpClient()
        {
            if (m_HttpClient == null)
                m_HttpClient = new UnityWebRequestClient();
            return m_HttpClient;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configUrl">The url to use to fetch the config</param>
        public EditorGameServiceRemoteConfiguration(string configUrl)
        {
            m_ConfigUrl = configUrl;
            m_SessionStateKey = $"EditorGameServiceConfig::{configUrl.GetHashCode()}";
        }

        /// <summary>
        /// Retrieves the configuration from the cache or server and provides it to the caller.
        /// Makes an effort to return the cached object first, then checks SessionState, and finally fetches from
        /// the server. If all else fails returns a default constructed object.
        /// NB: Newly constructed instances of this class will use SessionState if it's available before calling
        /// out to the server.
        /// </summary>
        /// <param name="onGetConfigurationCompleted">
        ///     Callback action to retrieve the configuration.
        /// </param>>
        public void GetConfiguration(Action<T> onGetConfigurationCompleted)
        {
            var configAsyncOp = GetConfiguration();
            configAsyncOp.Completed += asyncGetConfigurationResponse
                => onGetConfigurationCompleted?.Invoke(asyncGetConfigurationResponse.Result);
        }

        internal IAsyncOperation<T> GetConfiguration()
        {
            var operation = new AsyncOperation<T>();
            operation.SetInProgress();

            if (!m_IsConfigurationLoaded)
            {
                m_IsConfigurationLoaded = JsonHelper.TryJsonDeserialize(
                    SessionState.GetString(m_SessionStateKey, null), ref m_CachedConfiguration);
            }

            if (m_IsConfigurationLoaded)
            {
                operation.Succeed(m_CachedConfiguration);
            }
            else
            {
                if (m_FetchOperation == null || m_FetchOperation.IsDone)
                    m_FetchOperation = FetchConfigurationFromCdn();

                m_FetchOperation.Completed += OnConfigurationFetched;
            }

            return operation;

            void OnConfigurationFetched(IAsyncOperation<string> fetchOperation)
            {
                if (!m_IsConfigurationLoaded)
                {
                    var json = fetchOperation.Result;
                    if (JsonHelper.TryJsonDeserialize(json, ref m_CachedConfiguration))
                    {
                        SessionState.SetString(m_SessionStateKey, json);
                        m_IsConfigurationLoaded = true;
                    }
                    else
                        m_CachedConfiguration = new T();
                }

                operation.Succeed(m_CachedConfiguration);
            }
        }

        /// <summary>
        /// Erases the configuration from SessionState and from memory. Will force the configuration to be fetched
        /// from the server the next time it is requested.
        /// </summary>
        public void ClearCache()
        {
            SessionState.EraseString(m_SessionStateKey);
            m_IsConfigurationLoaded = false;
            m_CachedConfiguration = default;
        }

        IAsyncOperation<string> FetchConfigurationFromCdn()
        {
            var operation = new AsyncOperation<string>();
            operation.SetInProgress();

            var configRequest = new HttpRequest()
                .AsGet()
                .SetUrl(m_ConfigUrl);
            configRequest.Options.RedirectLimit = 5;
            GetHttpClient().Send(configRequest).Completed += OnRequestCompleted;

            return operation;

            void OnRequestCompleted(IAsyncOperation<ReadOnlyHttpResponse> configFetchOperation)
            {
                var config = configFetchOperation.Status == AsyncOperationStatus.Succeeded
                    ? SafeGetUTF8StringFromBytes(configFetchOperation.Result.Data)
                    : null;
                operation.Succeed(config);
            }
        }

        static string SafeGetUTF8StringFromBytes(byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    return Encoding.UTF8.GetString(bytes);
                }
                catch (Exception)
                {
                    // ignored, String decoding failed, we'll return null
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Helper class for managing CDN based endpoint configurations
    /// </summary>
    /// <typeparam name="T">The object to populate with fields from the CDN JSON file</typeparam>
    public class CdnConfiguredEndpoint<T> : EditorGameServiceRemoteConfiguration<T> where T : new()
    {
        const string k_CdnUrl = "https://public-cdn.cloud.unity3d.com/config/proxy/production";

        /// <summary>
        /// Initializes a new instance of the <see cref="CdnConfiguredEndpoint{T}" /> class.
        /// </summary>
        public CdnConfiguredEndpoint()
            : base(k_CdnUrl) {}
    }
}
