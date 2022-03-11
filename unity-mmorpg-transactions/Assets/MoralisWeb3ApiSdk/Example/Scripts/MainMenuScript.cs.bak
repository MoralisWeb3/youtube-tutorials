/**
 *           Module: MainMenuScript.cs
 *  Descriptiontion: Example class that demonstrates a game menu that incorporates
 *                   Wallet Connect and Moralis Authentication.
 *           Author: Moralis Web3 Technology AB, 559307-5988 - David B. Goodrich 
 *  
 *  MIT License
 *  
 *  Copyright (c) 2021 Moralis Web3 Technology AB, 559307-5988
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */
using System.Collections.Generic;
using System.Threading.Tasks;
using Moralis.Platform;
using Moralis.Platform.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;
using Assets.Scripts;
using Assets;

/// <summary>
/// Example class that demonstrates a game menu that incorporates Wallet 
/// Connect and Moralis Authentication.
/// </summary>
public class MainMenuScript : MonoBehaviour
{
    public string MoralisApplicationId;
    public string MoralisServerURI;
    public string ApplicationName;
    public string Version;
    public GameObject AuthenticationButton;
    public WalletConnect walletConnect;
    public GameObject qrMenu;
    public GameObject joystick;

    Image menuBackground;

    void Start()
    {
        menuBackground = (Image)gameObject.GetComponent(typeof(Image));

        HostManifestData hostManifestData = new HostManifestData()
        { 
            Version = Version,
            Identifier = ApplicationName,
            Name = ApplicationName,
            ShortVersion = Version
        };

        qrMenu.SetActive(false);

#if UNITY_ANDROID || UNITY_IOS
        // We're in mobile so show the joystick.
        joystick.SetActive(true);
#endif

        MoralisInterface.Initialize(MoralisApplicationId, MoralisServerURI, hostManifestData);

        if (MoralisInterface.IsLoggedIn())
        {
            Debug.Log("User is already logged in to Moralis.");

            // Transition to main game scene

        }
        else
        {
            AuthenticationButtonOn();
        }
    }

    /// <summary>
    /// Used to start the authentication process and then run the game. If the 
    /// user has a valid Moralis session thes user is redirected to the next 
    /// scene.
    /// </summary>
    public async void Play()
    {
        Debug.Log("PLAY");

        AuthenticationButtonOff();

        // If the user is still logged in just show game.
        if (MoralisInterface.IsLoggedIn())
        {
            Debug.Log("User is already logged in to Moralis.");

            // Transition to main game scene
            SceneManager.LoadScene(SceneMap.GAME_VIEW);
        }
        // User is not logged in, depending on build target, begin wallect connection.
        else
        {
            Debug.Log("User is not logged in.");
            //mainMenu.SetActive(false);

            // The mobile solutions for iOS and Android will be different once we
            // smooth out the interaction with Wallet Connect. For now the duplicated 
            // code below is on purpose just to keep the iOS and Android authentication
            // processes separate.
#if UNITY_ANDROID
            // By pass noraml Wallet Connect for now.
            //androidMenu.SetActive(true);

            // Use Moralis Connect page for authentication as we work to make the Wallet 
            // Connect experience better.
            await LoginViaConnectionPage();
#elif UNITY_IOS
            // By pass noraml Wallet Connect for now.
            //iOsMenu.SetActive(true);

            // Use Moralis Connect page for authentication as we work to make the Wallet 
            // Connect experience better.
            await LoginViaConnectionPage();
#else
            qrMenu.SetActive(true);
#endif
        }
    }

    /// <summary>
    /// Handles the Wallet Connect OnConnection event.
    /// When user grants wallet connection to application this 
    /// method is called. A request for signature is sent to wallet. 
    /// If users agrees to sign the message the signed message is 
    /// used to authenticate with Moralis.
    /// </summary>
    /// <param name="data">WCSessionData</param>
    public async void WalletConnectHandler(WCSessionData data)
    {
        Debug.Log("Wallet connection received");
        // Extract wallet address from the Wallet Connect Session data object.
        string address = data.accounts[0].ToLower();

        Debug.Log($"Sending sign request for {address} ...");

        string response = await walletConnect.Session.EthPersonalSign(address, "Moralis Authentication");

        Debug.Log($"Signature {response} for {address} was returned.");

        // Create moralis auth data from message signing response.
        Dictionary<string, object> authData = new Dictionary<string, object> { { "id", address }, { "signature", response }, { "data", "Moralis Authentication" } };

        Debug.Log("Logging in user.");

        // Attempt to login user.
        MoralisUser user = await MoralisInterface.LogInAsync(authData);

        if (user != null)
        {
            Debug.Log($"User {user.username} logged in successfully. ");
        }
        else
        {
            Debug.Log("User login failed.");
        }
    }

    /// <summary>
    /// Closeout connections and quit the application.
    /// </summary>
    public async void Quit()
    {
        Debug.Log("QUIT");
        
        // Disconnect wallet subscription.
        await walletConnect.Session.Disconnect();
        // CLear out the session so it is re-establish on sign-in.
        walletConnect.CLearSession();
        // Logout the Moralis User.
        await MoralisInterface.LogOutAsync();
        // Close out the application.
        Application.Quit();
    }

    /// <summary>
    /// Display Moralis connector login page
    /// </summary>
    private async Task LoginViaConnectionPage()
    {
        // Use Moralis Connect page for authentication as we work to make the Wallet 
        // Connect experience better.
        MoralisUser user = await MobileLogin.LogIn(MoralisServerURI, MoralisApplicationId);

        if (user != null)
        {
            // User is not null so login was successful, show first game scene.
            //SceneManager.LoadScene(SceneMap.GAME_VIEW);
            AuthenticationButtonOff();
        }
        else
        {
            AuthenticationButtonOn();
        }
    }

    private void AuthenticationButtonOff()
    {
        AuthenticationButton.SetActive(false);
        Color color = menuBackground.color;
        color = new Color(0f, 0f, 0f, 0f);
        menuBackground.color = color;
    }

    private void AuthenticationButtonOn()
    {
        AuthenticationButton.SetActive(true);
        Color color = menuBackground.color;
        color = new Color(0f, 0f, 0f, 0.7f);
        menuBackground.color = color;
    }
}
