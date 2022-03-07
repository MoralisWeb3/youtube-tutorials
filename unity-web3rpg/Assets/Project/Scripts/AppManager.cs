using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Moralis
using MoralisWeb3ApiSdk;
using UnityEngine.SceneManagement;

//WalletConnect
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;

public class AppManager : MonoBehaviour
{
    #region PUBLIC_FIELDS

    public MoralisController moralisController;
    public WalletConnect walletConnect;

    [Header("UI Elements")]
    public GameObject editorPanel;
    public GameObject mobilePanel;
    public GameObject loggedInPanel;
    public TextMeshProUGUI infoLabel;

    #endregion

    #region PRIVATE_FIELDS

    private GameObject _currentActivePanel;

    #endregion

    #region UNITY_LIFECYCLE

    private void Awake()
    {
        infoLabel.text = String.Empty;
        
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer
                                 || Application.platform == RuntimePlatform.WebGLPlayer) 
        {
            editorPanel.SetActive(true);
            _currentActivePanel = editorPanel;
        }
        else
        {
            //TODO iOS
            if (Application.platform == RuntimePlatform.Android)
            {
                mobilePanel.SetActive(true);
                _currentActivePanel = mobilePanel;
            }
        }
    }

    private async void Start()
    {
        if (moralisController != null)
        {
            await moralisController.Initialize();
        }
        else
        {
            Debug.LogError("MoralisController not found.");
        }
    }

    #endregion

    #region MORALIS AND WALLETCONNECT

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

        if (!MoralisInterface.IsLoggedIn())
        {
            Debug.Log($"Sending sign request for {address} ...");

            string signMessage = $"Moralis Authentication\n\nId: {appId}:{serverTime}";
            string response = await walletConnect.Session.EthPersonalSign(address, signMessage);
        
            Debug.Log(response);

            Debug.Log($"Signature {response} for {address} was returned.");

            // Create moralis auth data from message signing response.
            Dictionary<string, object> authData = new Dictionary<string, object> { { "id", address }, { "signature", response }, { "data", signMessage } }; 

            Debug.Log("Logging in user.");

            // Attempt to login user.
            var user = await MoralisInterface.LogInAsync(authData);

            if (user != null)
            {
                Debug.Log($"User {user.username} logged in successfully. ");
                infoLabel.text = $"User {user.username} logged in successfully!";
            }
            else
            {
                Debug.Log("User login failed.");
                infoLabel.text = "Login failed";
            }
        }

        UserLoggedInHandler();
    }

    public void WalletConnectSessionEstablished(WalletConnectUnitySession session)
    {
        InitializeWeb3();
    }
    
    private void InitializeWeb3()
    {
        MoralisInterface.SetupWeb3();
    }
    
    #endregion

    #region PRIVATE_METHODS

    private async void UserLoggedInHandler()
    {
        var user = await MoralisInterface.GetUserAsync();

        if (user != null)
        {
            _currentActivePanel.SetActive(false);
            loggedInPanel.SetActive(true);
            infoLabel.text = $"User {user.username} is logged in";
        }
    }
    
    private async void LogOut()
    {
        await walletConnect.Session.Disconnect();
        walletConnect.CLearSession();

        await MoralisInterface.LogOutAsync();
    }
    
    public void HandleWalletConnected()
    {
        infoLabel.text = "Connection successful. Please sign message";
    }

    public void HandleWalletDisconnected()
    {
        infoLabel.text = "Connection failed. Try again!";
    }

    #endregion
    
    #region EDITOR_METHODS

    public void OnJoinRoomClicked()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnCloseClicked()
    {
        LogOut();
        Application.Quit();
    }

    /*
    public void ChangeUserName()
    {
        EditUserNameAsync();
    }
    
    private async void EditUserNameAsync()
    {
        var user = await MoralisInterface.GetUserAsync();

        if (user != null)
        {
            user.username = "new username value";
            await user.SaveAsync();
        }
    }
     */

    #endregion
}