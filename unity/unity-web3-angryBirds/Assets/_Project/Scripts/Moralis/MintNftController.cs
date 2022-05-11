using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Moralis.Web3Api.Models;
using UnityEngine;
using MoralisWeb3ApiSdk;
using MoralisWeb3ApiSdk.Example.Scripts;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

namespace AngryBirdsWeb3
{
    public class MintNftController : MonoBehaviour
    {
        [Header("Smart Contract Data")] 
        private const string ContractAbi = "YOUR CONTRACT ABI";
        private const string ContractAddress = "YOUR CONTRACT ADDRESS";
        private const ChainList DeploymentChain = ChainList.mumbai;
        private const string ContractInstanceName = "MyNftContract";
        
        [Header("UI Elements")]
        [SerializeField] private GameObject mintPanel;
        [SerializeField] private Image characterImg;
        [SerializeField] private Button mintButton;
        [SerializeField] private TextMeshProUGUI debugLabel;

        private string _characterImgFilePath;
        
        
        #region UNITY_LIFECYCLE

        private void OnEnable()
        {
            MoralisWeb3Manager.LoggedInSuccessfully += ActivateMintPanel;
        }

        private void OnDisable()
        {
            MoralisWeb3Manager.LoggedInSuccessfully -= ActivateMintPanel;
        }

        #endregion
        
        
        #region PUBLIC_METHODS

        public async void Mint()
        {
            mintButton.interactable = false;
            debugLabel.text = "Minting... Check on your wallet to confirm transaction";
            
            await RunNftMint();
        }

        #endregion
        

        #region PRIVATE_METHODS

        private async Task RunNftMint()
        {
            // Dummy TokenId based on current time.
            long tokenId = DateTime.Now.Ticks;
            
            // Dummy NFT Name.
            string nftName = $"Moralis_Web3_AngryBirds_{tokenId}";

            NftMinter minter = new NftMinter(ContractInstanceName, ContractAbi, DeploymentChain, ContractAddress);

            // Now call the mint process.
            if (await minter.MintNft(nftName, "NFT created using Moralis Unity SDK!", _characterImgFilePath, new BigInteger(tokenId)))
            {
                Debug.Log($"NFT {tokenId} has been minted!");
                debugLabel.text = $"NFT {tokenId} has been minted!" + 
                                  "<br>" + 
                                  $"<link=\"https://testnets.opensea.io/assets?search[query]={ContractAddress}\"><u>Check on OpenSea</u></link>";
            }
            else
            {
                Debug.Log("Failed to mint NFT :-(");
                debugLabel.text = "Failed to mint NFT. Try again";
                mintButton.interactable = true;
            }
        }

        #endregion
        

        #region EVENT_HANDLERS

        private void ActivateMintPanel()
        {
            #if UNITY_EDITOR
            _characterImgFilePath = AssetDatabase.GetAssetPath(characterImg.sprite);
            #endif

            mintPanel.SetActive(true);
            debugLabel.text = "Go ahead and hit MINT";
        }

        #endregion
    }
}

