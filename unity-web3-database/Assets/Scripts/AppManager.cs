using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Moralis and WalletConnect SDK
using MoralisWeb3ApiSdk;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;

public class AppManager : MonoBehaviour
{
    [Header("Moralis and WalletConnect")]
    [SerializeField] private MoralisController moralisController;
    [SerializeField] private WalletConnect walletConnect;

    [Header("UI Elements")]
    [SerializeField] private GameObject qrPanel;
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] private GameObject enemiesPanel;
    [SerializeField] private TextMeshProUGUI infoLabel;
    
    private async void Start()
    {
        if (moralisController != null && moralisController)
        {
            await moralisController.Initialize();
        }
        else
        {
            Debug.LogError("The MoralisInterface has not been set up, please check your MoralisController in the scene.");
            return;
        }
        
        infoLabel.text = "Scan QR Code to Login.";
    }

    public async void WalletConnectHandler(WCSessionData data)
    {
        Debug.Log("Wallet connection received");
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
        }

        string signMessage = $"Moralis Authentication\n\nId: {appId}:{serverTime}";

        Debug.Log($"Sending sign request for {address} ...");
        infoLabel.text = "Please sign message in your wallet.";

        string response = await walletConnect.Session.EthPersonalSign(address, signMessage);

        Debug.Log($"Signature {response} for {address} was returned.");

        // Create moralis auth data from message signing response.
        Dictionary<string, object> authData = new Dictionary<string, object> { { "id", address }, { "signature", response }, { "data", signMessage } }; 

        Debug.Log("Logging in user.");
        infoLabel.text = "Logging in user.";

        // Attempt to login user.
        var user = await MoralisInterface.LogInAsync(authData);

        if (user != null)
        {
            Debug.Log($"User {user.username} logged in successfully.");
            infoLabel.text = $"User {user.username} logged in successfully.";
            
            qrPanel.SetActive(false);
            selectionPanel.SetActive(true); 
        }
        else
        {
            Debug.Log("User login failed.");
            infoLabel.text = "User login failed.";
        }
    }

    public void OnWalletConnectedEventHandler()
    {
        infoLabel.text = "Wallet connected.";
    }

    public void OnWalletDisconnectedEventHandler(WalletConnectUnitySession session)
    {
        selectionPanel.SetActive(false);
        usernamePanel.SetActive(false);
        enemiesPanel.SetActive(false);
        qrPanel.SetActive(true);
        
        infoLabel.text = "Wallet disconnected. Try scanning again or reset application.";
    }

    public async void LogOut()
    {
        if (walletConnect.Session.Connected && walletConnect.Connected)
        {
            //Log out automatically
            await MoralisInterface.LogOutAsync();
        
            await walletConnect.Session.Disconnect();
            // CLear out the session so it is re-establish on sign-in.
            walletConnect.CLearSession();

            infoLabel.text = "Logged out from Moralis and disconnected from your wallet.";   
        }
    }
}
