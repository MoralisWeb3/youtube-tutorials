using System;
using System.Collections;
using System.Collections.Generic;
using MoralisWeb3ApiSdk;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using TMPro;
using UnityEngine;

namespace Web3MultiplayerRPG
{
    public class TransactionPanel : MonoBehaviour
    {
        public TextMeshProUGUI toUsername;
        public TextMeshProUGUI toAddress;

        public void SetInformation(string clickedUsername, string clickedWalletAddress)
        {
            toUsername.text = "Username: " + clickedUsername;
            toAddress.text = "Wallet Address: " + clickedWalletAddress;
        }

        //TODO Need to test this
        /*
        private async void SendTransactionAsync()
        {
            // Retrieve from address, the address used to athenticate the user.
            var user = await MoralisInterface.GetUserAsync();
            string fromAddress = user.authData["moralisEth"]["id"].ToString();

            // Create transaction request.
            TransactionInput txnRequest = new TransactionInput()
            {
                Data = String.Empty,
                From = fromAddress,
                To = toAddress.text,
                Value = new HexBigInteger(0)
            };

            try
            {
                // Execute the transaction.
                string txnHash = await MoralisInterface.Web3Client.Eth.TransactionManager.SendTransactionAsync(txnRequest);

                Debug.Log($"Transfered {0} WEI from {fromAddress} to {toAddress}.  TxnHash: {txnHash}");
            }
            catch (Exception exp)
            {
                Debug.Log($"Transfer of {0} WEI from {fromAddress} to {toAddress} failed!");
            }
        }
        
        public void SendTransaction()
        {
            SendTransactionAsync();
        }
         */
    }
}

