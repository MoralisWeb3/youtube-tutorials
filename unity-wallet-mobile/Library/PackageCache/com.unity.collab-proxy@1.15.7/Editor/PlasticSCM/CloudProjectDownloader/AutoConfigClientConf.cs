using Codice.Client.Common;
using Codice.CM.Common;
using PlasticGui;
using Unity.PlasticSCM.Editor.WebApi;

namespace Unity.PlasticSCM.Editor.ProjectDownloader
{
    internal static class AutoConfigClientConf
    {
        internal static void FromUnityAccessToken(
            string unityAccessToken,
            string serverName,
            string projectPath)
        {
            CredentialsResponse response = WebRestApiClient.
                PlasticScm.GetCredentials(unityAccessToken);

            if (response.Error != null)
            {
                UnityEngine.Debug.LogErrorFormat(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.ErrorGettingCredentialsCloudProject),
                    response.Error.Message,
                    response.Error.ErrorCode);

                return;
            }

            ClientConfigData configData = BuildClientConfigData(
                serverName, projectPath, response);

            ClientConfig.Get().Save(configData);
        }

        static ClientConfigData BuildClientConfigData(
            string serverName,
            string projectPath,
            CredentialsResponse response)
        {
            SEIDWorkingMode workingMode = GetWorkingMode(response.Type);

            ClientConfigData configData = new ClientConfigData();

            configData.WorkspaceServer = serverName;
            configData.CurrentWorkspace = projectPath;
            configData.WorkingMode = workingMode.ToString();
            configData.SecurityConfig = UserInfo.GetSecurityConfigStr(
                workingMode,
                response.Email,
                GetPassword(response.Token, response.Type));
            configData.LastRunningEdition = InstalledEdition.Get();
            return configData;
        }

        static string GetPassword(
            string token,
            CredentialsResponse.TokenType tokenType)
        {
            if (tokenType == CredentialsResponse.TokenType.Bearer)
                return BEARER_PREFIX + token;

            return token;
        }

        static SEIDWorkingMode GetWorkingMode(CredentialsResponse.TokenType tokenType)
        {
            if (tokenType == CredentialsResponse.TokenType.Bearer)
                return SEIDWorkingMode.SSOWorkingMode;

            return SEIDWorkingMode.LDAPWorkingMode;
        }

        const string BEARER_PREFIX = "Bearer ";
    }
}
