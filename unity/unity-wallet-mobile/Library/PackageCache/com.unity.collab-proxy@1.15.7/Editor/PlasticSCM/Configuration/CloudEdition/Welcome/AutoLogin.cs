using Codice.Client.Common.Connection;
using Codice.Client.Common.Servers;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.LogWrapper;
using PlasticGui.Configuration.CloudEdition.Welcome;
using PlasticGui.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class AutoLogin : OAuthSignIn.INotify
    {
        internal enum State : byte
        { 
            Off = 0,
            Started = 1,
            Running = 2,
            ResponseInit = 3,
            ResponseEnd = 6,
            ResponseSuccess = 7,
            OrganizationChoosed = 8,
            InitializingPlastic = 9,
            ErrorNoToken = 20,
            ErrorTokenException = 21,
            ErrorResponseNull = 22,
            ErrorResponseError = 23,
            ErrorTokenEmpty = 24,
            ErrorResponseCancel = 25
        }

        internal string AccessToken;
        internal string UserName;

        void OAuthSignIn.INotify.SuccessForConfigure(
            List<string> organizations,
            bool canCreateAnOrganization,
            string userName,
            string accessToken)
        {
            mPlasticWindow.GetWelcomeView().autoLoginState = AutoLogin.State.ResponseSuccess;
            ChooseOrganization(organizations, canCreateAnOrganization);
        }

        void OAuthSignIn.INotify.SuccessForSSO(string organization)
        {
        }

        void OAuthSignIn.INotify.SuccessForProfile(string email)
        {
        }

        void OAuthSignIn.INotify.SuccessForCredentials(
            string email,
            string accessToken)
        {
        }

        void OAuthSignIn.INotify.Cancel(string errorMessage)
        {
            mPlasticWindow.GetWelcomeView().autoLoginState = AutoLogin.State.ErrorResponseCancel;
        }

        internal ResponseType Run()
        {
            mPlasticWindow = GetPlasticWindow();

            if (!string.IsNullOrEmpty(CloudProjectSettings.accessToken))
            {
                ExchangeTokensAndJoinOrganization(CloudProjectSettings.accessToken);
                return ResponseType.Ok;
            }
            else
            {
                mPlasticWindow.GetWelcomeView().autoLoginState = AutoLogin.State.ErrorNoToken;
                return ResponseType.None;
            }
        }

        void ExchangeTokensAndJoinOrganization(string unityAccessToken)
        {
            int ini = Environment.TickCount;

            TokenExchangeResponse response = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
            /*threadOperationDelegate*/ delegate
            {
                mPlasticWindow.GetWelcomeView().autoLoginState = AutoLogin.State.ResponseInit;
                response = WebRestApiClient.PlasticScm.TokenExchange(unityAccessToken);
            },
            /*afterOperationDelegate*/ delegate
            {
                mLog.DebugFormat(
                    "TokenExchange time {0} ms",
                    Environment.TickCount - ini);

                if (waiter.Exception != null)
                {
                    mPlasticWindow.GetWelcomeView().autoLoginState = AutoLogin.State.ErrorTokenException;
                    ExceptionsHandler.LogException(
                        "TokenExchangeSetting",
                        waiter.Exception);
                    Debug.LogWarning(waiter.Exception.Message);
                    return;
                }

                if (response == null)
                {
                    mPlasticWindow.GetWelcomeView().autoLoginState = AutoLogin.State.ErrorResponseNull;
                    Debug.LogWarning("Auto Login response null");
                    return;
                }
                   
                if (response.Error != null)
                {
                    mPlasticWindow.GetWelcomeView().autoLoginState = AutoLogin.State.ErrorResponseError;
                    var warning = string.Format(
                            "Unable to exchange token: {0} [code {1}]",
                            response.Error.Message, response.Error.ErrorCode);
                    mLog.ErrorFormat(warning);
                    Debug.LogWarning(warning);
                    return;
                }

                if (string.IsNullOrEmpty(response.AccessToken))
                {
                    mPlasticWindow.GetWelcomeView().autoLoginState = AutoLogin.State.ErrorTokenEmpty;
                    var warning = string.Format(
                        "Access token is empty for user: {0}",
                        response.User);
                    mLog.InfoFormat(warning);
                    Debug.LogWarning(warning);
                    return;
                }

                mPlasticWindow.GetWelcomeView().autoLoginState = AutoLogin.State.ResponseEnd;
                AccessToken = response.AccessToken;
                UserName = response.User;
                GetOrganizationList();
            });
        }

        internal void ExchangeTokens(string unityAccessToken)
        {
            int ini = Environment.TickCount;

            TokenExchangeResponse response = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
            /*threadOperationDelegate*/ delegate
            {
                response = WebRestApiClient.PlasticScm.TokenExchange(unityAccessToken);
            },
            /*afterOperationDelegate*/ delegate
            {
                mLog.DebugFormat(
                    "TokenExchange time {0} ms",
                    Environment.TickCount - ini);
            
                if (waiter.Exception != null)
                {
                    ExceptionsHandler.LogException(
                        "TokenExchangeSetting",
                        waiter.Exception);
                    return;
                }
            
                if (response == null)
                {
                    Debug.Log("response null");
                    return;
                }
            
                if (response.Error != null)
                {
                    mLog.ErrorFormat(
                        "Unable to exchange token: {0} [code {1}]",
                        response.Error.Message,
                        response.Error.ErrorCode);
                    return;
                }
            
                if (string.IsNullOrEmpty(response.AccessToken))
                {
                    mLog.InfoFormat(
                        "Access token is empty for user: {0}",
                        response.User);
                    return;
                }

                AccessToken = response.AccessToken;
                UserName = response.User;
            });
        }

        internal void GetOrganizationList()
        {
            OAuthSignIn.GetOrganizationsFromAccessToken(
                mPlasticWindow.PlasticWebRestApiForTesting,
                new Editor.UI.Progress.ProgressControlsForDialogs(),
                this,
                CloudProjectSettings.userName,
                AccessToken
            );
        }

        PlasticWindow GetPlasticWindow()
        {
            var windows = Resources.FindObjectsOfTypeAll<PlasticWindow>();
            PlasticWindow plasticWindow = windows.Length > 0 ? windows[0] : null;

            if (plasticWindow == null)
                plasticWindow = ShowWindow.Plastic();

            return plasticWindow;
        }

        internal void ChooseOrganization(
            List<string> organizations,
            bool canCreateAnOrganization)
        {
            mPlasticWindow = GetPlasticWindow();

            CloudEditionWelcomeWindow.ShowWindow(
                   mPlasticWindow.PlasticWebRestApiForTesting,
                   mPlasticWindow.CmConnectionForTesting,null, true);

            mCloudEditionWelcomeWindow = CloudEditionWelcomeWindow.GetWelcomeWindow();
            mCloudEditionWelcomeWindow.FillUserAndToken(UserName, AccessToken);
            if (organizations.Count == 1)
            {
                mCloudEditionWelcomeWindow.JoinOrganizationAndWelcomePage(organizations[0]);
                return;
            }
            mCloudEditionWelcomeWindow.ShowOrganizationPanelFromAutoLogin(organizations, canCreateAnOrganization);
        }

        internal AskCredentialsToUser.DialogData BuildCredentialsDialogData(ResponseType dialogResult)
        {
            return new AskCredentialsToUser.DialogData(
                dialogResult == ResponseType.Ok,
                UserName,
                AccessToken, 
                false,
                SEIDWorkingMode.SSOWorkingMode);
        }

        PlasticWindow mPlasticWindow;
        CloudEditionWelcomeWindow mCloudEditionWelcomeWindow;

        static readonly ILog mLog = LogManager.GetLogger("TokensExchange");
    }

}
