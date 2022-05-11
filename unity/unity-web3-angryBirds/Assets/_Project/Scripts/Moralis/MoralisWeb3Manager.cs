using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//MORALIS & WALLET CONNECT
using MoralisWeb3ApiSdk;
using Moralis.Platform.Objects;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;

namespace AngryBirdsWeb3
{
    public class MoralisWeb3Manager : MonoBehaviour
    {
        public static event Action LoggedInSuccessfully;
        
        [Header("Moralis and WalletConnect")]
        [SerializeField] private MoralisController moralisController;
        [SerializeField] private WalletConnect walletConnect;
        [SerializeField] private bool autoLogout;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject mainCanvas;
        [SerializeField] private WalletConnectQRImage qrCode;
        [SerializeField] private TextMeshProUGUI debugText;

        private async void Start()
        {
            debugText.text = "Scan QR Code to authenticate";

            if (moralisController == null) return;
            await moralisController.Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Logout();
            }
        }

        private void OnApplicationQuit()
        {
            if (autoLogout)
            {
                Logout();
            }
        }

        #region EVENT_HANDLERS

        public async void OnWalletConnectEventHandler(WCSessionData connectionData)
        {
            qrCode.gameObject.SetActive(false);
            
            if (connectionData.approved)
            {
                debugText.text = "Wallet connected";
                
            }
            else
            {
                walletConnect.CLearSession();
                await walletConnect.Session.Disconnect();
                
                return;
            }

            // Extract wallet address from the Wallet Connect Session data object.
            string address = connectionData.accounts[0].ToLower();
            string appId = MoralisInterface.GetClient().ApplicationId;
            long serverTime = 0;

            // Retrieve server time from Moralis Server for message signature.
            Dictionary<string, object> serverTimeResponse = await MoralisInterface.GetClient().Cloud.RunAsync<Dictionary<string, object>>("getServerTime", new Dictionary<string, object>());

            if (serverTimeResponse == null || !serverTimeResponse.ContainsKey("dateTime") ||
                !long.TryParse(serverTimeResponse["dateTime"].ToString(), out serverTime))
            {
                Debug.LogError("Failed to retrieve server time from Moralis Server!");
                debugText.text = "Failed to retrieve time from server";
                
                qrCode.gameObject.SetActive(true);
                return;
            }

            if (MoralisInterface.IsLoggedIn())
            {
                debugText.text = "Authentication to successful!"; 
                mainCanvas.SetActive(false);
            
                MoralisInterface.SetupWeb3();
                LoggedInSuccessfully?.Invoke();
                return;
            }
            
            string signMessage = $"Moralis Authentication\n\nId: {appId}:{serverTime}";

            debugText.text = "Please sign message in your wallet";
            string response = await walletConnect.Session.EthPersonalSign(address, signMessage);
            
            debugText.text = "Authenticating..."; 

            // Create moralis auth data from message signing response.
            Dictionary<string, object> authData = new Dictionary<string, object> { { "id", address }, { "signature", response }, { "data", signMessage } };

            // Attempt to login user.
            MoralisUser user = await MoralisInterface.LogInAsync(authData);

            if (user is null)
            {
                debugText.text = "Authentication failed"; 
                return;
            }
            
            debugText.text = "Authentication to successful!"; 
            mainCanvas.SetActive(false);
            
            MoralisInterface.SetupWeb3();
            LoggedInSuccessfully?.Invoke();
        }

        public void OnDisconnectedEventHandler(WalletConnectUnitySession session)
        {
            qrCode.gameObject.SetActive(true);
            debugText.text = "Disconnected. Try scanning again";
        }

        public void OnConnectionFailedEventHandler(WalletConnectUnitySession session)
        {
            qrCode.gameObject.SetActive(true);
            debugText.text = "Connection failed. Try scanning again";
        }

        #endregion

        
        #region PRIVATE_METHODS

        private async void Logout()
        {
            try
            {
                // Logout the Moralis User.
                await MoralisInterface.LogOutAsync();
                // CLear out the session so it is re-establish on sign-in.
                walletConnect.CLearSession();
                // Disconnect wallet subscription.
                await walletConnect.Session.Disconnect();
            }
            catch (Exception exp)
            {
                // Send error to the log but not as an error as this is expected behavior from W.C.
                Debug.Log($"Quit Error - Has quit already been called? Error: {exp.Message}");
            }
        }
        
        #endregion
    }
}