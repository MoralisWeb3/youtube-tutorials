using System;
using System.Collections;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseItemManager : MonoBehaviour
{
    public static event Action PurchaseStarted;
    public static event Action<string> PurchaseCompleted;
    public static event Action PurchaseFailed;
    public static event Action ItemPanelClosed;

    [Header("Item UI Elements")]
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    
    [Header("Transaction Info")]
    [SerializeField] private TextMeshProUGUI transactionInfoText;

    //Control variables
    private ItemData _currentItemData;

    
    #region UNITY_LIFECYCLE

    private void OnEnable()
    {
        ShopItem.Selected += ActivateItemPanel;
    }

    private void OnDisable()
    {
        ShopItem.Selected -= ActivateItemPanel;
    }

    #endregion

    
    #region PUBLIC_METHODS

    public async void PurchaseItem()
    {
        PurchaseStarted?.Invoke();
        
        itemPanel.SetActive(false);
        
        transactionInfoText.gameObject.SetActive(true);
        transactionInfoText.text = "Creating and saving metadata to IPFS...";
        
        var metadataUrl = await CreateIpfsMetadata();

        if (metadataUrl is null)
        {
            transactionInfoText.text = "Metadata couldn't be saved to IPFS";
            StartCoroutine(DisableInfoText());
            
            PurchaseFailed?.Invoke();
            return;
        }
        
        transactionInfoText.text = "Metadata saved successfully";

        // I'm assuming that this is creating a different tokenId from the already minted tokens in the contract.
        // I can do that because I know I'm converting a unique objectId coming from the MoralisDB.
        long tokenId = MoralisTools.ConvertStringToLong(_currentItemData.objectId);
        
        transactionInfoText.text = "Please confirm transaction in your wallet";
        
        var result = await PurchaseItemFromContract(tokenId, metadataUrl);

        if (result is null)
        {
            transactionInfoText.text = "Transaction failed";
            StartCoroutine(DisableInfoText());
            
            PurchaseFailed?.Invoke();
            return;
        }
        
        transactionInfoText.text = "Transaction completed!";
        StartCoroutine(DisableInfoText());
        
        PurchaseCompleted?.Invoke(_currentItemData.objectId);
    }

    public void OnCloseButtonClicked()
    {
        itemPanel.SetActive(false);
        ItemPanelClosed?.Invoke();
    }

    #endregion
    
    
    #region EVENT_HANDLERS

    private void ActivateItemPanel(InventoryItem selectedItem)
    {
        _currentItemData = selectedItem.GetData();
        
        _currentItemData.objectId = selectedItem.GetId();
        
        itemName.text = selectedItem.GetData().name.ToUpper();
        itemDescription.text = selectedItem.GetData().description;
        itemIcon.sprite = selectedItem.GetSprite();
        
        itemPanel.SetActive(true);
    }

    #endregion

    
    #region PRIVATE_METHODS

    // We are minting the NFT and transferring it to the player
    private async Task<string> PurchaseItemFromContract(BigInteger tokenId, string metadataUrl)
    {
        byte[] data = Array.Empty<byte>();
        
        object[] parameters = {
            tokenId.ToString("x"),
            metadataUrl,
            data
        };

        // Set gas estimate
        HexBigInteger value = new HexBigInteger("0x0");
        HexBigInteger gas = new HexBigInteger(0);
        HexBigInteger gasPrice = new HexBigInteger("0x0");

        string resp = await Moralis.ExecuteContractFunction(GameManager.ContractAddress, GameManager.ContractAbi, "buyItem", parameters, value, gas, gasPrice);
        
        return resp;
    }

    private async UniTask<string> CreateIpfsMetadata()
    {
        // 1. Build Metadata
        object metadata = MoralisTools.BuildMetadata(_currentItemData.name, _currentItemData.description, _currentItemData.imageUrl);

        string metadataName = $"{_currentItemData.name}_{_currentItemData.objectId}.json";

        // 2. Encoding JSON
        string json = JsonConvert.SerializeObject(metadata);
        string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
      
        // 3. Save metadata to IPFS
        string ipfsMetadataPath = await MoralisTools.SaveToIpfs(metadataName, base64Data);

        return ipfsMetadataPath;
    }
    
    private IEnumerator DisableInfoText()
    {
        yield return new WaitForSeconds(1f);

        transactionInfoText.text = string.Empty;
        transactionInfoText.gameObject.SetActive(false);
    }

    #endregion
}
