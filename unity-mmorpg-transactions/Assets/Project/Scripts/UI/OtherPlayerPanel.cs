using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//MORALIS
using MoralisWeb3ApiSdk;
using Nethereum.Util;
#if UNITY_WEBGL
using System.Numerics;
using Moralis.WebGL.Hex.HexTypes;
using Moralis.WebGL.Models;
#else
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
#endif

namespace Web3MultiplayerRPG
{
    public class OtherPlayerPanel : MonoBehaviour
    {
        [Header("Info Panel")] 
        public GameObject infoPanel;
        public TextMeshProUGUI toUsernameLabel;
        public TextMeshProUGUI toAddressLabel;

        [Header("Transaction Panel")]
        public GameObject transactionPanel;
        public Button executeButton;
        public TMP_InputField inputField;

        private string _toAddress;
        private string _toUsername;

        private void OnEnable()
        {
            infoPanel.SetActive(true);
            transactionPanel.SetActive(false);
        }

        private void OnDisable()
        {
            inputField.text = String.Empty;
        }

        public void SetInformation(string clickedUsername, string clickedWalletAddress)
        {
            _toUsername = clickedUsername;
            _toAddress = clickedWalletAddress;
            
            toUsernameLabel.text = "Username:<br>" + clickedUsername;
            toAddressLabel.text = "Wallet Address:<br>" + clickedWalletAddress;
        }

        public void HandleTransferButtonClick()
        {
            #if UNITY_WEBGL
                ExecuteTransferWebGL();
            #else
                ExecuteTransfer();
            #endif
        }

        public void HandleOnValueChanged()
        {
            if (inputField.text == String.Empty)
            {
                executeButton.interactable = false;
            }
            else
            {
                executeButton.interactable = true;
            }
        }

#if UNITY_WEBGL
        private async void ExecuteTransferWebGL()
        {
            var nativeAmount = float.Parse(inputField.text);
            var weiAmount = UnitConversion.Convert.ToWei(nativeAmount);

            // Retrieve from address, the address used to authenticate the user.
            var user = await MoralisInterface.GetUserAsync();
            string fromAddress = user.authData["moralisEth"]["id"].ToString();
 
            // Create transaction request.
            TransactionInput txnRequest = new TransactionInput()
            {
                Data = String.Empty,
                From = fromAddress,
                To = _toAddress,
                Value = new HexBigInteger(weiAmount)
            };      

            try
            {
                // Execute the transaction.
                string txnHash = await MoralisInterface.SendTransactionAsync(txnRequest.To, txnRequest.Value);
                
                Debug.Log($"Transfered {weiAmount} WEI from {fromAddress} to {toAddressLabel}.  TxnHash: {txnHash}");
            }
            catch (Exception exp)
            {
                Debug.Log($"Transfer of {weiAmount} WEI from {fromAddress} to {toAddressLabel} failed!");
            }
        }
#else
        private async void ExecuteTransfer()
        {
            var nativeAmount = float.Parse(inputField.text);
            var weiAmount = UnitConversion.Convert.ToWei(nativeAmount);
            
            // Retrieve from address, the address used to authenticate the user.
            var user = await MoralisInterface.GetUserAsync();
            string fromAddress = user.authData["moralisEth"]["id"].ToString();
 
            // Create transaction request.
            TransactionInput txnRequest = new TransactionInput()
            {
                Data = String.Empty,
                From = fromAddress,
                To = _toAddress,
                Value = new HexBigInteger(weiAmount)
            };

            try
            {
                // Execute the transaction.
                string txnHash = await MoralisInterface.Web3Client.Eth.TransactionManager.SendTransactionAsync(txnRequest);

                Debug.Log($"Transfered {weiAmount} WEI from {fromAddress} to {_toAddress}.  TxnHash: {txnHash}");
            }
            catch (Exception exp)
            {
                Debug.Log($"Transfer of {weiAmount} WEI from {fromAddress} to {_toAddress} failed!");
            }
        }
#endif
    }
}

