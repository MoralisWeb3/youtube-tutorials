using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Assets.Scripts;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using WalletConnectSharp.Core;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Network;
using WalletConnectSharp.Unity.Models;
using WalletConnectSharp.Unity.Network;
using WalletConnectSharp.Unity.Utils;

namespace WalletConnectSharp.Unity
{
    [RequireComponent(typeof(NativeWebSocketTransport))]
    public class WalletConnect : BindableMonoBehavior
    {
        public const string SessionKey = "__WALLETCONNECT_SESSION__";

        /// <summary>
        /// FOR FUTURE USE - when using W.C. for iOS this list will limit the wallets
        /// displayed to the user.
        /// </summary>
        public List<string> AllowedWalletIds;

        public Dictionary<string, AppEntry> SupportedWallets
        {
            get;
            private set;
        }
        
        public AppEntry SelectedWallet { get; set; }

        public Wallets DefaultWallet;

        [Serializable]
        public class WalletConnectEventNoSession : UnityEvent { }
        [Serializable]
        public class WalletConnectEventWithSession : UnityEvent<WalletConnectUnitySession> { }
        [Serializable]
        public class WalletConnectEventWithSessionData : UnityEvent<WCSessionData> { }
        
        public event EventHandler ConnectionStarted;

        [BindComponent]
        private NativeWebSocketTransport _transport;

        private static WalletConnect _instance;

        public static WalletConnect Instance
        {
            get
            {
                return _instance;
            }
        }
        
        public static WalletConnectUnitySession ActiveSession
        {
            get
            {
                return _instance.Session;
            }
        }

        public string ConnectURL
        {
            get
            {
                return Protocol.URI;
            }
        }

        public bool autoSaveAndResume = true;
        public bool connectOnAwake = false;
        public bool connectOnStart = true;
        public bool createNewSessionOnSessionDisconnect = true;
        public int connectSessionRetryCount = 3;
        public string customBridgeUrl;
        
        public int chainId = 1;

        public WalletConnectEventNoSession ConnectedEvent;

        public WalletConnectEventWithSessionData ConnectedEventSession;

        public WalletConnectEventWithSession DisconnectedEvent;
        
        public WalletConnectEventWithSession ConnectionFailedEvent;
        public WalletConnectEventWithSession NewSessionConnected;
        public WalletConnectEventWithSession ResumedSessionConnected;

        public WalletConnectUnitySession Session
        {
            get;
            private set;
        }

        [Obsolete("Use Session instead of Protocol")]
        public WalletConnectUnitySession Protocol {
            get { return Session; }
            private set
            {
                Session = value;
            }
        }

        public bool Connected
        {
            get
            {
                return Protocol.Connected;
            }
        }

        [SerializeField]
        public ClientMeta AppData;

        protected override async void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            _instance = this;

            base.Awake();

            if (connectOnAwake)
            {
                await Connect();
            }
        }
        
        async void Start() 
        {
            if (connectOnStart && !connectOnAwake)
            {
                await Connect();
            }
        }

        public async Task<WCSessionData> Connect()
        {
            SavedSession savedSession = null;
            if (PlayerPrefs.HasKey(SessionKey))
            {
                var json = PlayerPrefs.GetString(SessionKey);
                savedSession = JsonConvert.DeserializeObject<SavedSession>(json);
            }
            
            if (string.IsNullOrWhiteSpace(customBridgeUrl))
            {
                customBridgeUrl = null;
            }
            
            if (Session != null)
            {
                var currentKey = Session.KeyData;
                if (savedSession != null)
                {
                    if (currentKey != savedSession.Key)
                    {
                        if (Session.Connected)
                        {
                            await Session.Disconnect();
                        }
                        else if (Session.TransportConnected)
                        {
                            await Session.Transport.Close();
                        }
                    }
                    else if (!Session.Connected && !Session.Connecting)
                    {
                        StartCoroutine(SetupDefaultWallet());
                        
                        SetupEvents();

                        return await CompleteConnect();
                    }
                    else
                    {
                        return null; //Nothing to do
                    }
                }
                else if (Session.Connected)
                {
                    await Session.Disconnect();
                }
                else if (Session.TransportConnected)
                {
                    await Session.Transport.Close();
                } 
                else if (Session.Connecting)
                {
                    //We are still connecting, do nothing
                    return null;
                }
            }

            //default will be set by library
            ICipher ciper = null;
            
            #if UNITY_WEBGL
            ciper = new WebGlAESCipher();
            #endif
            
            if (savedSession != null)
            {
                Session = new WalletConnectUnitySession(savedSession, this, _transport);
            }
            else
            {
                Session = new WalletConnectUnitySession(AppData, this, customBridgeUrl, _transport, ciper, chainId);
            }

            StartCoroutine(SetupDefaultWallet());
            
            SetupEvents();

            return await CompleteConnect();
        }

        private void SetupEvents()
        {
            #if UNITY_EDITOR || DEBUG
            //Useful for debug logging
            Session.OnSessionConnect += (sender, session) =>
            {
                Debug.Log("[WalletConnect] Session Connected");
            };
            #endif
            
            Session.OnSessionDisconnect += SessionOnOnSessionDisconnect;
            Session.OnSessionCreated += SessionOnOnSessionCreated;
            Session.OnSessionResumed += SessionOnOnSessionResumed;
            
            #if UNITY_ANDROID || UNITY_IOS
            //Whenever we send a request to the Wallet, we want to open the Wallet app
            Session.OnSend += (sender, session) => OpenMobileWallet();
            #endif
        }

        private void TeardownEvents()
        {
            Session.OnSessionDisconnect -= SessionOnOnSessionDisconnect;
            Session.OnSessionCreated -= SessionOnOnSessionCreated;
            Session.OnSessionResumed -= SessionOnOnSessionResumed;
        }

        private void SessionOnOnSessionResumed(object sender, WalletConnectSession e)
        {
            if (this.ResumedSessionConnected != null)
                this.ResumedSessionConnected.Invoke(e as WalletConnectUnitySession ?? Session);
        }

        private void SessionOnOnSessionCreated(object sender, WalletConnectSession e)
        {
            if (this.NewSessionConnected != null)
                this.NewSessionConnected.Invoke(e as WalletConnectUnitySession ?? Session);
        }

        private async Task<WCSessionData> CompleteConnect()
        {
            Debug.Log("Waiting for Wallet connection");
            
            if (ConnectionStarted != null)
            {
                ConnectionStarted(this, EventArgs.Empty);
            }
            
            WalletConnectEventWithSessionData allEvents = new WalletConnectEventWithSessionData();
                
            allEvents.AddListener(delegate(WCSessionData arg0)
            {
                ConnectedEvent.Invoke();
                ConnectedEventSession.Invoke(arg0);
            });

            int tries = 0;
            while (tries < connectSessionRetryCount)
            {
                try
                {
                    var session = await Session.SourceConnectSession();

                    allEvents.Invoke(session);

                    return session;
                }
                catch (IOException e)
                {
                    tries++;

                    if (tries >= connectSessionRetryCount)
                        throw new IOException("Failed to request session connection after " + tries + " times.", e);
                }
            }
            
            throw new IOException("Failed to request session connection after " + tries + " times.");
        }

        private async void SessionOnOnSessionDisconnect(object sender, EventArgs e)
        {
            if (DisconnectedEvent != null)
                DisconnectedEvent.Invoke(ActiveSession);

            if (autoSaveAndResume && PlayerPrefs.HasKey(SessionKey))
            {
                PlayerPrefs.DeleteKey(SessionKey);
            }
            
            TeardownEvents();
            
            if (createNewSessionOnSessionDisconnect)
            {
                await Connect();
            }
        }

        private IEnumerator SetupDefaultWallet()
        {
            yield return FetchWalletList(false);

            var wallet = SupportedWallets.Values.FirstOrDefault(a => a.name.ToLower() == DefaultWallet.ToString().ToLower());

            if (wallet != null)
            {
                yield return DownloadImagesFor(wallet.id);
                SelectedWallet = wallet;
                Debug.Log("Setup default wallet " + wallet.name);
            }
        }

        private IEnumerator DownloadImagesFor(string id, string[] sizes = null)
        {
            if (sizes == null)
            {
                sizes = new string[] {"sm", "md", "lg"};
            }
            
            var data = SupportedWallets[id];

            foreach (var size in sizes)
            {
                var url = "https://registry.walletconnect.org/logo/" + size + "/" + id + ".jpeg";

                using (UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(url))
                {
                    yield return imageRequest.SendWebRequest();

                    if (imageRequest.isNetworkError)
                    {
                        Debug.Log("Error Getting Wallet Icon: " + imageRequest.error);
                    }
                    else
                    {
                        var texture = ((DownloadHandlerTexture) imageRequest.downloadHandler).texture;
                        var sprite = Sprite.Create(texture,
                            new Rect(0.0f, 0.0f, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f), 100.0f);

                        if (size == "sm")
                        {
                            data.smallIcon = sprite;
                        }
                        else if (size == "md")
                        {
                            data.medimumIcon = sprite;
                        }
                        else if (size == "lg")
                        {
                            data.largeIcon = sprite;
                        }
                    }
                }
            }
        }

        public IEnumerator FetchWalletList(bool downloadImages = true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://registry.walletconnect.org/data/wallets.json"))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                
                if (webRequest.isNetworkError)
                {
                    Debug.Log("Error Getting Wallet Info: " + webRequest.error);
                }
                else
                {
                    var json = webRequest.downloadHandler.text;

                    SupportedWallets = JsonConvert.DeserializeObject<Dictionary<string, AppEntry>>(json);

                    if (downloadImages)
                    {
                        foreach (var id in SupportedWallets.Keys)
                        {
                            yield return DownloadImagesFor(id);
                        }
                    }
                }
            }
        }

        private async void OnDestroy()
        {
            await SaveOrDisconnect();
        }

        private async void OnApplicationQuit()
        {
            await SaveOrDisconnect();
        }

        private async void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                await SaveOrDisconnect();
            }
            else if (PlayerPrefs.HasKey(SessionKey) && autoSaveAndResume)
            {
                await Connect();
            }
        }

        private async Task SaveOrDisconnect()
        {
            if (!Session.Connected)
                return;
            
            if (autoSaveAndResume)
            {
                var session = Session.SaveSession();
                var json = JsonConvert.SerializeObject(session);
                PlayerPrefs.SetString(SessionKey, json);

                await Session.Transport.Close();
            }
            else
            {
                await Session.Disconnect();
            }
        }

        public void OpenMobileWallet(AppEntry selectedWallet)
        {
            SelectedWallet = selectedWallet;
            
            OpenMobileWallet();
        }
        
        public void OpenDeepLink(AppEntry selectedWallet)
        {
            SelectedWallet = selectedWallet;
            
            OpenDeepLink();
        }

        public void OpenMobileWallet()
        {
#if UNITY_ANDROID
            var signingURL = ConnectURL.Split('@')[0];

            Application.OpenURL(signingURL);
#elif UNITY_IOS
            if (SelectedWallet == null)
            {
                throw new NotImplementedException(
                    "You must use OpenMobileWallet(AppEntry) or set SelectedWallet on iOS!");
            }
            else
            {
                string url;
                string encodedConnect = WebUtility.UrlEncode(ConnectURL);
                if (!string.IsNullOrWhiteSpace(SelectedWallet.mobile.universal))
                {
                    url = SelectedWallet.mobile.universal + "/wc?uri=" + encodedConnect;
                }
                else
                {
                    url = SelectedWallet.mobile.native + (SelectedWallet.mobile.native.EndsWith(":") ? "//" : "/") +
                          "wc?uri=" + encodedConnect;
                }

                var signingUrl = url.Split('?')[0];
                
                Debug.Log("Opening: " + signingUrl);
                Application.OpenURL(signingUrl);
            }
#else
            Debug.Log("Platform does not support deep linking");
            return;
#endif
        }

        public void OpenDeepLink()
        {
            if (!ActiveSession.ReadyForUserPrompt)
            {
                Debug.LogError("WalletConnectUnity.ActiveSession not ready for a user prompt" +
                               "\nWait for ActiveSession.ReadyForUserPrompt to be true");

                return;
            }
            
#if UNITY_ANDROID
            Debug.Log("[WalletConnect] Opening URL: " + ConnectURL);
            Application.OpenURL(ConnectURL);
#elif UNITY_IOS
            if (SelectedWallet == null)
            {
                throw new NotImplementedException(
                    "You must use OpenDeepLink(AppEntry) or set SelectedWallet on iOS!");
            }
            else
            {
                string url;
                string encodedConnect = WebUtility.UrlEncode(ConnectURL);
                if (!string.IsNullOrWhiteSpace(SelectedWallet.mobile.universal))
                {
                    url = SelectedWallet.mobile.universal + "/wc?uri=" + encodedConnect;
                }
                else
                {
                    url = SelectedWallet.mobile.native + (SelectedWallet.mobile.native.EndsWith(":") ? "//" : "/") +
                          "wc?uri=" + encodedConnect;
                }
                
                Debug.Log("Opening: " + url);
                Application.OpenURL(url);
            }
#else
            Debug.Log("Platform does not support deep linking");
            return;
#endif
        }

        public void CLearSession()
        {
            PlayerPrefs.DeleteKey(SessionKey);
        }
        
        public async void CloseSession(bool waitForNewSession = true)
        {
            if (ActiveSession == null)
                return;
            
            await ActiveSession.Disconnect();
        
            if (waitForNewSession)
                await ActiveSession.Connect();
        }
    }
}