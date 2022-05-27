using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using TMPro;

using MoralisUnity;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Platform.Queries;
using Nethereum.Hex.HexTypes;
using UnityEngine.SceneManagement;

namespace Web3MoriaGates
{
    public class MoriaGatesEvent : MoralisObject
    {
        public bool result { get; set; }
        
        public MoriaGatesEvent() : base("MoriaGatesEvent") {}
    }
    
    public class GameManager : MonoBehaviour
    {
        //Smart Contract Data
        private const string ContractAddress = "";
        private const string ContractAbi = "";
        
        //Database Queries
        private MoralisQuery<MoriaGatesEvent> _getEventsQuery;
        private MoralisLiveQueryCallbacks<MoriaGatesEvent> _queryCallbacks;
        
        [Header("Main Elements")]
        [SerializeField] private PasswordPanel passwordPanel;
        [SerializeField] private GameObject correctPanel;
        [SerializeField] private GameObject incorrectPanel;
        
        [Header("Other")]
        [SerializeField] private TextMeshProUGUI statusLabel;

        private bool _listening;
        
        // Only for Editor using
        private bool _responseReceived;
        private bool _responseResult;
        

        #region UNITY_LIFECYCLE

        private void Awake()
        {
            statusLabel.text = string.Empty;
        }

        private void Update()
        {
            // We only do this in Editor because of single threading and UI elements issues
            if (!Application.isEditor) return;
            
            if (_responseReceived)
            {
                ShowResponsePanel(_responseResult);
                _responseReceived = false;
            }
        }

        #endregion

        
        #region AUTHENTICATIONKIT_HANDLERS

        public void StartGame()
        { 
            SubscribeToDatabaseEvents();
            passwordPanel.gameObject.SetActive(true);
        }
        
        public void ResetGame()
        {
            passwordPanel.gameObject.SetActive(false);
            correctPanel.gameObject.SetActive(false);
            incorrectPanel.gameObject.SetActive(false);
            statusLabel.text = string.Empty;
            
            MoralisLiveQueryController.RemoveSubscriptions("MoriaGatesEvent");
        }

        #endregion

        
        #region PUBLIC_METHODS

        public async void OpenGates()
        {
            statusLabel.text = "Please confirm transaction in your wallet";
            
            var response = await CallContractFunction(passwordPanel.passwordInput.text);

            if (response == null)
            {
                statusLabel.text = "Contract call failed";
                return;
            }
            
            statusLabel.text = "Waiting for contract event...";
            passwordPanel.gameObject.SetActive(false);

            _listening = true;
        }

        #endregion
        

        #region PRIVATE_METHODS

        private async void SubscribeToDatabaseEvents()
        {
            _getEventsQuery = await Moralis.GetClient().Query<MoriaGatesEvent>();
            _queryCallbacks = new MoralisLiveQueryCallbacks<MoriaGatesEvent>();

            _queryCallbacks.OnUpdateEvent += HandleContractEventResponse;

            MoralisLiveQueryController.AddSubscription<MoriaGatesEvent>("MoriaGatesEvent", _getEventsQuery, _queryCallbacks);
        }

        private async UniTask<string> CallContractFunction(string inputPassword)
        {
            object[] parameters = {
                inputPassword
            };

            // Set gas estimate
            HexBigInteger value = new HexBigInteger(0);
            HexBigInteger gas = new HexBigInteger(0);
            HexBigInteger gasPrice = new HexBigInteger(0);

            string resp = await Moralis.ExecuteContractFunction(ContractAddress, ContractAbi, "openGates", parameters, value, gas, gasPrice);
            
            return resp;
        }

        private void HandleContractEventResponse(MoriaGatesEvent newEvent, int requestId)
        {
            if (!_listening) return;

            // You will find this a bit different from the video. It's a quality improvement for Editor testing. Functionality continues in ShowResponsePanel() :)
            if (Application.isEditor)
            {
                _responseResult = newEvent.result;
                _responseReceived = true;

                return;
            }

            ShowResponsePanel(newEvent.result);
        }

        private void ShowResponsePanel(bool isCorrect)
        {
            if (isCorrect)
            {
                correctPanel.SetActive(true);
                Debug.Log("Correct password");
            }
            else
            {
                incorrectPanel.SetActive(true);
                Debug.Log("Incorrect password");
            }

            statusLabel.text = string.Empty;
            _listening = false;

            StartCoroutine(DoSomething(isCorrect));
        }

        private IEnumerator DoSomething(bool result)
        {
            yield return new WaitForSeconds(3f);

            if (result)
            {
                //We could load another game scene here
                //SceneManager.LoadScene("Main");
            }
            else
            {
                //Make the user type the password again
                incorrectPanel.SetActive(false);
                passwordPanel.gameObject.SetActive(true);
            }
        }

        #endregion
    }
}
