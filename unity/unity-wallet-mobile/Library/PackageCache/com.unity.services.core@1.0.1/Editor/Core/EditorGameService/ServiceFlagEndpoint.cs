using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Unity.Services.Core.Editor
{
    class ServiceFlagEndpoint : CdnConfiguredEndpoint<ServiceFlagEndpointConfiguration> {}

    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class ServiceFlagEndpointConfiguration
    {
        const string k_ServiceFlagFormat = "/projects/{0}/service_flags";

        [SerializeField]
        string core;

        public string Core => core;

        string BuildApiUrl()
        {
            return Core + "/api";
        }

        public string BuildServiceFlagUrl(string projectId)
        {
            return string.Format(BuildApiUrl() + k_ServiceFlagFormat, projectId);
        }

        public string BuildPayload(string serviceFlagName, bool status)
        {
            const string payloadFieldName = "service_flags";

            // A Dictionary is used here because both the key and the value must be mutable.
            // The key is the the service flag name and the value is the status bool.
            var serviceFlags = new Dictionary<string, bool>
            {
                [serviceFlagName] = status,
            };

            var payload = new Dictionary<string, object>
            {
                [payloadFieldName] = serviceFlags,
            };

            return MiniJson.Serialize(payload);
        }
    }
}
