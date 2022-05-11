using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

using MoralisUnity;
using MoralisUnity.Platform.Objects;
using UnityEngine.InputSystem;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;

[RequireComponent(typeof(WalletConnect))]
public class MoralisStandaloneAuth : MoralisAuthenticator
{
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private WalletConnectQRImage qrCode;
    [SerializeField] private TextMeshProUGUI debugText;
    
    private WalletConnect _walletConnect;
    private GameInputActions _input;


    #region UNITY_LIFECYCLE

    private void Awake()
    {
        _walletConnect = GetComponent<WalletConnect>();
        
        // New Input System
        _input = new GameInputActions();
        _input.General.Logout.Enable();
    }

    private void OnEnable()
    {
        // That happens when we press "L"
        _input.General.Logout.performed += Logout;
    }

    private void OnDisable()
    {
        _input.General.Logout.performed -= Logout;
    }

    private async void Start()
    {
        debugText.text = "Scan QR Code to authenticate";
        
        await Initialize();

        if (!Moralis.IsLoggedIn())
        {
            // CLear out the session so it is re-establish on sign-in.
            _walletConnect.CLearSession();
        }
    }
    
    void OnApplicationQuit()
    {
        _walletConnect.CLearSession();
    }

    #endregion


    #region EVENT_HANDLERS

    public async void OnWalletConnectEventHandler(WCSessionData connectionData)
    {
        if (connectionData.approved)
        {
            debugText.text = "Wallet connected";
            qrCode.gameObject.SetActive(false);
        }
        else
        {
            _walletConnect.CLearSession();
            qrCode.gameObject.SetActive(false);
            
            await _walletConnect.Session.Disconnect();
            
            return;
        }

        // Extract wallet address from the Wallet Connect Session data object.
        string address = connectionData.accounts[0].ToLower();
        string appId = Moralis.GetClient().ApplicationId;

        // Retrieve server time from Moralis Server for message signature.
        Dictionary<string, object> serverTimeResponse = await Moralis.GetClient().Cloud.RunAsync<Dictionary<string, object>>("getServerTime", new Dictionary<string, object>());

        if (serverTimeResponse == null || !serverTimeResponse.ContainsKey("dateTime") ||
            !long.TryParse(serverTimeResponse["dateTime"].ToString(), out var serverTime))
        {
            Debug.LogError("Failed to retrieve server time from Moralis Server!");
            debugText.text = "Failed to retrieve time from server";
            
            qrCode.gameObject.SetActive(true);
            return;
        }

        if (Moralis.IsLoggedIn())
        {
            debugText.text = "Authentication to successful!"; 
            mainCanvas.SetActive(false);
        
            Authenticated?.Invoke();
            return;
        }
        
        string signMessage = $"Moralis Authentication\n\nId: {appId}:{serverTime}";

        debugText.text = "Please sign message in your wallet";
        string response = await _walletConnect.Session.EthPersonalSign(address, signMessage);
        
        debugText.text = "Authenticating..."; 

        // Create moralis auth data from message signing response.
        Dictionary<string, object> authData = new Dictionary<string, object> { { "id", address }, { "signature", response }, { "data", signMessage } };

        // Attempt to login user.
        MoralisUser user = await Moralis.LogInAsync(authData);

        if (user is null)
        {
            debugText.text = "Authentication failed"; 
            return;
        }
        
        debugText.text = "Authentication to successful!"; 
        mainCanvas.SetActive(false);
        
        Authenticated?.Invoke();
    }
    
    public void OnSessionConnectedHandler(WalletConnectUnitySession session)
    {
        InitializeWeb3();
    }
 
    public void OnDisconnectedEventHandler(WalletConnectUnitySession session)
    {
        mainCanvas.SetActive(true);
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

    private async UniTask Initialize()
    {
        if (!Moralis.Initialized)
        {
            ClientMeta clientMeta = new ClientMeta()
            {
                Name = MoralisSettings.MoralisData.ApplicationName,
                Description = MoralisSettings.MoralisData.ApplicationDescription,
                Icons = new[] { MoralisSettings.MoralisData.ApplicationIconUri },
                URL = MoralisSettings.MoralisData.ApplicationUrl
            };

            _walletConnect.AppData = clientMeta;

            // Initialize and register the Moralis, Moralis Web3Api and
            // NEthereum Web3 clients
            await Moralis.Start();
        }
    }
    
    private async void InitializeWeb3()
    {
        await Moralis.SetupWeb3();
    }
    
    private async void Logout(InputAction.CallbackContext context)
    {
        // We're only able to logout during Authenticating or Free state
        if (GameManager.Instance.GetCurrentState() != GameManager.State.Authenticating &&
            GameManager.Instance.GetCurrentState() != GameManager.State.Free) return;
        
        try
        {
            // Logout the Moralis User.
            await Moralis.LogOutAsync();
            // Disconnect wallet subscription.
            await _walletConnect.Session.Disconnect();
            // CLear out the session so it is re-establish on sign-in.
            _walletConnect.CLearSession();
        }
        catch (Exception exp)
        {
            // Send error to the log but not as an error as this is expected behavior from W.C.
            Debug.Log($"Quit Error - Has quit already been called? Error: {exp.Message}");
        }
    }
    
    #endregion
}
