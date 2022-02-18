using System;
using System.ComponentModel;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Base class for services which require service flag handling when toggling.
    /// </summary>
    public abstract class EditorGameServiceFlagEnabler : IEditorGameServiceEnabler
    {
        ServiceFlagEndpoint m_ServiceFlagEndpoint;

        /// <summary>
        /// Name of the service in the services flags API
        /// </summary>
        protected abstract string FlagName { get; }

        /// <summary>
        /// <inheritdoc cref="IEditorGameServiceEnabler.Enable"/>
        /// Also sends an API request to disable the service on the dashboard.
        /// </summary>
        public void Enable()
        {
            SetFlagStatus(true);
            EnableLocalSettings();
        }

        /// <summary>
        /// <inheritdoc cref="IEditorGameServiceEnabler.Disable"/>
        /// Also sends an API request to disable the service on the dashboard.
        /// </summary>
        public void Disable()
        {
            SetFlagStatus(false);
            DisableLocalSettings();
        }

        internal string GetFlagName()
        {
            return FlagName;
        }

        internal void InternalDisableLocalSettings()
        {
            DisableLocalSettings();
        }

        /// <inheritdoc cref="IEditorGameServiceEnabler.Enable"/>
        protected abstract void EnableLocalSettings();

        /// <inheritdoc cref="IEditorGameServiceEnabler.Disable"/>
        protected abstract void DisableLocalSettings();

        /// <inheritdoc cref="IEditorGameServiceEnabler.IsEnabled"/>
        public abstract bool IsEnabled();

        /// <summary>
        /// The event fired when the web request that handles setting the service flag is complete
        /// </summary>
        public event Action ServiceFlagRequestComplete;

        void SetFlagStatus(bool status)
        {
#if ENABLE_EDITOR_GAME_SERVICES
            if (string.IsNullOrEmpty(FlagName))
            {
                throw new InvalidEnumArgumentException("FlagName is null or empty. Set a proper value to properly manage the service flags.");
            }

            if (CloudProjectSettings.projectBound)
            {
                if (m_ServiceFlagEndpoint == null)
                {
                    m_ServiceFlagEndpoint = new ServiceFlagEndpoint();
                }

                var configAsyncOp = m_ServiceFlagEndpoint.GetConfiguration();
                configAsyncOp.Completed += asyncEndpointResponse => OnEndpointConfigurationReceived(asyncEndpointResponse, status);
            }
#endif
        }

        void OnEndpointConfigurationReceived(IAsyncOperation<ServiceFlagEndpointConfiguration> endpointResponse, bool status)
        {
            if (endpointResponse?.Result == null)
            {
                return;
            }

            var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(endpointResponse.Result.BuildPayload(FlagName, status)));
            var serviceFlagRequest = new UnityWebRequest(url: endpointResponse.Result.BuildServiceFlagUrl(CloudProjectSettings.projectId),
                UnityWebRequest.kHttpVerbPUT,
                null,
                uploadHandler);
            serviceFlagRequest.SetRequestHeader("AUTHORIZATION", $"Bearer {CloudProjectSettings.accessToken}");
            serviceFlagRequest.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");
            serviceFlagRequest.SendWebRequest().completed += async => { OnServiceFlagRequestCompleted(serviceFlagRequest); };
        }

        void OnServiceFlagRequestCompleted(UnityWebRequest webRequest)
        {
            webRequest?.Dispose();
            ServiceFlagRequestComplete?.Invoke();
        }
    }
}
