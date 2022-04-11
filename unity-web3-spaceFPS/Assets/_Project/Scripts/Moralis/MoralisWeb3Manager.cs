using System;
using System.Collections;
using System.Collections.Generic;
using Moralis.Platform.Objects;
using Moralis.Platform.Queries;
using UnityEngine;
using TMPro;

//MORALIS & WALLET CONNECT
using MoralisWeb3ApiSdk;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;

namespace Main
{
    public class MoralisWeb3Manager : MonoBehaviour
    {
        public static Action LoggedInSuccessfully;
        public bool isDebug;
        
        [Header("Moralis and WalletConnect")]
        [SerializeField] private MoralisController moralisController;
        [SerializeField] private WalletConnect walletConnect;

        [Header("Login Panels")] 
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject qrPanel;

        [Header("Other UI Elements")] 
        [SerializeField] private TextMeshProUGUI debugLabel;

        #region UNITY_LIFECYCLE

        private async void Start()
        {
            //We just want to test the game
            if (isDebug)
            {
                mainPanel.SetActive(false);
                LoggedInSuccessfully?.Invoke();
                this.gameObject.SetActive(false);
                return;
            }
            
            if (moralisController != null && moralisController)
            {
                await moralisController.Initialize();
            }
            else
            {
                Debug.LogError("The MoralisInterface has not been set up, please check you MoralisController in the scene.");
                return;
            }

            debugLabel.text = "Scan QR Code to Login";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                LogOut();
            }
        }

        #endregion

        #region WALLET_LOGIN_METHODS

        public async void WalletConnectHandler(WCSessionData data)
        {
            Debug.Log("Wallet connection received");
            debugLabel.text = "Wallet connected";
            
            // Extract wallet address from the Wallet Connect Session data object.
            string address = data.accounts[0].ToLower();
            string appId = MoralisInterface.GetClient().ApplicationId;
            long serverTime = 0;

            // Retrieve server time from Moralis Server for message signature
            Dictionary<string, object> serverTimeResponse = await MoralisInterface.GetClient().Cloud.RunAsync<Dictionary<string, object>>("getServerTime", new Dictionary<string, object>());

            if (serverTimeResponse == null || !serverTimeResponse.ContainsKey("dateTime") ||
                !long.TryParse(serverTimeResponse["dateTime"].ToString(), out serverTime))
            {
                Debug.Log("Failed to retrieve server time from Moralis Server!");
                debugLabel.text = "Failed to retrieve server time from Moralis Server";
            }

            string signMessage = $"Moralis Authentication\n\nId: {appId}:{serverTime}";

            Debug.Log($"Sending sign request for {address} ...");
            debugLabel.text = "Please sign authentication message in your wallet";

            string response = await walletConnect.Session.EthPersonalSign(address, signMessage);

            Debug.Log($"Signature {response} for {address} was returned");

            // Create moralis auth data from message signing response.
            Dictionary<string, object> authData = new Dictionary<string, object> { { "id", address }, { "signature", response }, { "data", signMessage } }; 

            Debug.Log("Logging in user.");
            debugLabel.text = "Logging in user";

            // Attempt to login user.
            var user = await MoralisInterface.LogInAsync(authData);

            if (user != null)
            {
                Debug.Log($"User {user.username} logged in successfully. ");
                debugLabel.text = $"User {user.username} logged in successfully";
                
                mainPanel.SetActive(false);
                LoggedInSuccessfully?.Invoke();
            }
            else
            {
                Debug.Log("User login failed.");
                debugLabel.text = "User login failed";
            }
        }
        
        public void OnWalletConnectedHandler()
        {
            qrPanel.SetActive(false);
        }

        public void WalletConnectSessionEstablished(WalletConnectUnitySession session)
        {
            InitializeWeb3();
        }
        
        private async void LogOut()
        {
            // Disconnect wallet subscription.
            await walletConnect.Session.Disconnect();
            // CLear out the session so it is re-establish on sign-in.
            walletConnect.CLearSession();
            // Logout the Moralis User.
            await MoralisInterface.LogOutAsync();
        }

        #endregion

        #region PRIVATE_METHODS

        private void InitializeWeb3()
        {
            // RPC Node connection.
            MoralisInterface.SetupWeb3();

            // Create an entry for the Game Rewards Contract.
            MoralisInterface.InsertContractInstance("Rewards", Constants.MUG_ABI, Constants.MUG_CHAIN, Constants.MUG_CONTRACT_ADDRESS);
        }

        #endregion
    }
}
