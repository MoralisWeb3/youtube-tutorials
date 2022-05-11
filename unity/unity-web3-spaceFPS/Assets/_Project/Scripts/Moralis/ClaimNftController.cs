using MoralisWeb3ApiSdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Numerics;
using TMPro;
#if UNITY_WEBGL
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Web3Api.Models;
using Moralis.WebGL.Hex.HexTypes;
#else
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Moralis.Platform.Objects;
using Moralis.Web3Api.Models;
#endif

namespace Main
{ 
    public class ClaimNftController : MonoBehaviour
    {
        [Header("NFT Data")]
        [SerializeField] private string contractAddress;
        [SerializeField] private string tokenId;
        
        [Header("3D Model")]
        [SerializeField] private GameObject model;
        
        [Header("UI Panels")]
        [SerializeField] private GameObject claimPanel;
        [SerializeField] private GameObject alreadyOwnedPanel;
        [SerializeField] private GameObject signingPanel;

        //State bools
        private bool _playerIsNear;
        private bool _checkedOwnership;
        private bool _alreadyOwned;
        private bool _signing;
        
        private const float RotatingSpeed = 0.1f;
        
        //Player Wallet Address
        private string _walletAddress;
        
        #region UNITY_LIFECYCLE

        private async void OnEnable()
        {
            //We get our wallet address.
            MoralisUser user = await MoralisInterface.GetUserAsync();
            _walletAddress = user.authData["moralisEth"]["id"].ToString();
        }

        async void Update()
        {
            model.transform.Rotate(0 ,0 ,RotatingSpeed);

            //That's pretty simple.
            if (!_playerIsNear) return;
            
            //We don't get to claim the NFT if we haven't checked if we already own it.
            if (_alreadyOwned || !_checkedOwnership) return;
            
            //If we are already signing the message in the wallet, we don't want to claim the reward again.
            if (_signing) return;
            //If we hit ENTER --> Claim NFT!
            if (Input.GetKeyDown(KeyCode.Return))
            {
                await ClaimNftAsync();
            }
        }

        #endregion

        
        #region PRIVATE_METHODS

        private async void CheckOwnership()
        {
            try
            {
                NftOwnerCollection noc =
                    await MoralisInterface.GetClient().Web3Api.Account.GetNFTsForContract(_walletAddress.ToLower(),
                        contractAddress,
                        ChainList.mumbai);
                
                IEnumerable<NftOwner> ownership = from n in noc.Result
                    where n.TokenId.Equals(tokenId)
                    select n;

                if (ownership != null && ownership.Count() > 0)
                {
                    alreadyOwnedPanel.SetActive(true);
                    
                    Debug.Log("Already Owns Mug.");
                    _alreadyOwned = true;
                }
                else
                {
                    claimPanel.SetActive(true);
                    Debug.Log("Does not own Mug.");
                }

                _checkedOwnership = true;
            }
            catch (Exception exp)
            {
                Debug.LogError(exp.Message);
            }
        }
        
        private async UniTask ClaimNftAsync()
        {
            // Do not process if already owned as the claim will fail in the contract call and waste gas fees.
            if (_alreadyOwned) return;

            _signing = true;
            
            claimPanel.SetActive(false);
            signingPanel.SetActive(true);

            // Convert token id to integer
            BigInteger bi = 0;

            if (BigInteger.TryParse(tokenId, out bi))
            {
                // Convert token id to hex as this is what the contract call expects
                object[] pars = new object[] { bi.ToString("x") };

                // Set gas estimate
                HexBigInteger gas = new HexBigInteger(0);
                
                // Call the contract to claim the NFT reward.
                string resp = await MoralisInterface.SendEvmTransactionAsync("Rewards", "mumbai", "claimReward", _walletAddress, gas, new HexBigInteger("0x0"), pars);

                //We have successfully claimed it.
                if (resp != null)
                {
                    _alreadyOwned = true;
                    transform.gameObject.SetActive(false);

                    Debug.Log("NFT claimed successfully!");
                }
                else
                {
                    Debug.Log("Claiming has been canceled.");
                }
                
                signingPanel.SetActive(false);
                _signing = false;
            }
        }

        #endregion
        
        
        #region ON_COLLISION/TRIGGER_EVENTS

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _playerIsNear = true;
                
                if (_checkedOwnership)
                {
                    if (_alreadyOwned)
                    {
                        alreadyOwnedPanel.SetActive(true);
                    }
                    else
                    {
                        if (_signing)
                        {
                            signingPanel.SetActive(true);
                        }
                        else
                        {
                            claimPanel.SetActive(true);
                        }
                    }
                    
                    return;
                }
                
                CheckOwnership();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _playerIsNear = false;
                
                //We deactivate all panels.
                claimPanel.SetActive(false);
                alreadyOwnedPanel.SetActive(false); 
                signingPanel.SetActive(false);
            }
        }

        #endregion
    }
}
