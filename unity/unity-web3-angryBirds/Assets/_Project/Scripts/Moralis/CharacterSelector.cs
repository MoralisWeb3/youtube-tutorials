using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Moralis.Platform.Objects;
using Moralis.Web3Api.Models;
using UnityEngine;
using MoralisWeb3ApiSdk;
using MoralisWeb3ApiSdk.Example.Scripts;
using TMPro;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace AngryBirdsWeb3
{
    [Serializable]
    public class CustomNftMetadata
    {
        public string name;
        public string description;
        public string image;
    }
    
    public class CharacterSelector : MonoBehaviour
    {
        public static event Action<Texture2D> OnCharacterSelected; //TODO passar la imatge/sprite/textura
        
        [Header("NFT Data")]
        [SerializeField] private string contractAddress;
        [SerializeField] private string tokenId;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject characterPanel;
        [SerializeField] private RawImage characterImg;
        [SerializeField] private GameObject skipButton;
        [SerializeField] private TextMeshProUGUI debugLabel;
        
        private string _walletAddress;
        private ChainList _deployedChain = ChainList.mumbai;


        #region UNITY_LIFECYCLE

        private void Awake()
        {
            characterImg.gameObject.SetActive(false);
            debugLabel.text = "Claiming NFT Metadata...".ToUpper();
        }

        private async void OnEnable()
        {
            MoralisWeb3Manager.LoggedInSuccessfully += SelectCharacter;
        }

        private void OnDisable()
        {
            MoralisWeb3Manager.LoggedInSuccessfully -= SelectCharacter;
        }

        #endregion


        #region PRIVATE_METHODS

        private async void CheckOwnership()
        {
            //We get our wallet address.
            MoralisUser user = await MoralisInterface.GetUserAsync();
            _walletAddress = user.authData["moralisEth"]["id"].ToString();
            
            try
            {
                NftOwnerCollection noc =
                    await MoralisInterface.GetClient().Web3Api.Account.GetNFTsForContract(_walletAddress.ToLower(),
                        contractAddress,
                        _deployedChain);
                
                IEnumerable<NftOwner> ownership = from n in noc.Result
                    where n.TokenId.Equals(tokenId)
                    select n;

                var ownershipList = ownership.ToList();
                if (ownershipList.Any()) // If I'm the owner :)
                {
                    Debug.Log(ownershipList.First().Metadata);
                    var nftMetaData = ownershipList.First().Metadata;
                    
                    CustomNftMetadata formattedMetaData = JsonUtility.FromJson<CustomNftMetadata>(nftMetaData);
                    StartCoroutine(GetTexture(formattedMetaData.image));

                    debugLabel.text = "Success!<br>".ToUpper() + "Select the image to play with the NFT".ToUpper();
                    Debug.Log("Already owns NFT.");
                }
                else
                {
                    debugLabel.text = "You do not own the NFT".ToUpper() + "<br>" + "Press SKIP to play with a local character".ToUpper();
                    Debug.Log("Does not own NFT.");
                    
                    //TODO We could activate a claiming option.
                }
            }
            catch (Exception exp)
            {
                Debug.LogError(exp.Message);
            }
        }
        
        IEnumerator GetTexture(string imageUrl)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    
                    characterImg.gameObject.SetActive(true);
                    characterImg.texture = texture;
                }
            }
        }

        #endregion


        #region EVENT_HANDLERS

        private void SelectCharacter()
        {
            characterPanel.SetActive(true);
            CheckOwnership();
        }

        #endregion

        
        #region PUBLIC_METHODS

        public void NftButtonClicked()
        {
            //TODO
            OnCharacterSelected?.Invoke((Texture2D) characterImg.texture);
            characterPanel.SetActive(false);
        }
        
        public void SkipButtonClicked()
        {
            //TODO
            OnCharacterSelected?.Invoke(null);
            characterPanel.SetActive(false);
        }

        #endregion
    }
}

