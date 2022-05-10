using System;
using System.Collections.Generic;
using System.Linq;
using MoralisUnity;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Web3Api.Models;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class NftMetadata
{
    public string name;
    public string description;
    public string image;
}

public class PlayerInventory : Inventory
{
    private GameInputActions _gameInputActions;
    private InputAction _openInventoryAction;
    
    #region UNITY_LIFECYCLE

    protected override void Awake()
    {
        base.Awake();
        
        titleText.text = "PLAYER INVENTORY";

        _gameInputActions = new GameInputActions();
        _openInventoryAction = _gameInputActions.General.OpenInventory;
        _openInventoryAction.Enable();
    }
    
    private void OnEnable()
    {
        _openInventoryAction.performed += OpenInventory;
    }

    private void OnDisable()
    {
        _openInventoryAction.performed -= OpenInventory;
    }

    #endregion


    #region INPUT_SYSTEM_HANDLERS

    private void OpenInventory(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.GetCurrentState() != GameManager.State.Free) return;

        if (uiPanel.activeSelf) return;
        
        ActivatePanel(true);
        Opened?.Invoke();
        
        LoadPurchasedItems();
    }

    #endregion


    #region PRIVATE_METHODS

    private async void LoadPurchasedItems()
    {
        //We get our wallet address.
        MoralisUser user = await Moralis.GetUserAsync();
        var playerAddress = user.authData["moralisEth"]["id"].ToString();

        try
        {
            NftOwnerCollection noc =
                await Moralis.GetClient().Web3Api.Account.GetNFTsForContract(playerAddress.ToLower(),
                    GameManager.ContractAddress,
                    GameManager.ContractChain);
            
            List<NftOwner> nftOwners = noc.Result;

            // We only proceed if we find some
            if (!nftOwners.Any())
            {
                Debug.Log("You don't own any NFT");
                return;
            }
            
            foreach (var nftOwner in nftOwners)
            {
                var nftMetaData = nftOwner.Metadata;
                NftMetadata formattedMetaData = JsonUtility.FromJson<NftMetadata>(nftMetaData);

                PopulatePlayerItem(nftOwner.TokenId, formattedMetaData);
            }
        }
        catch (Exception exp)
        {
            Debug.LogError(exp.Message);
        }
    }
    
    private void PopulatePlayerItem(string tokenId, NftMetadata data)
    {
        InventoryItem newItem = Instantiate(item, itemsGrid.transform);
        
        newItem.Init(tokenId, data);
    }

    #endregion
}
