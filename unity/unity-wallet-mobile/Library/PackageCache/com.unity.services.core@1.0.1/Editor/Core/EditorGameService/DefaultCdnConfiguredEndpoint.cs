using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Unity.Services.Core.Editor
{
    class DefaultCdnConfiguredEndpoint : CdnConfiguredEndpoint<DefaultCdnEndpointConfiguration> {}

    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class DefaultCdnEndpointConfiguration
    {
        [SerializeField]
        string core;

        public string Core => core;

        internal string BuildApiUrl()
        {
            return Core + "/api";
        }

        internal string BuildProjectsApiUrl(string organizationKey)
        {
            return string.Format(BuildApiUrl() + "/orgs/{0}/projects", organizationKey);
        }

        internal string BuildProjectApiUrl(string organizationKey, string projectId)
        {
            return string.Format(BuildProjectsApiUrl(organizationKey) + "/{0}", projectId);
        }

        internal string BuildUsersUrl(string organizationKey, string projectId)
        {
            return BuildProjectApiUrl(organizationKey, projectId) + "/users";
        }
    }
}
