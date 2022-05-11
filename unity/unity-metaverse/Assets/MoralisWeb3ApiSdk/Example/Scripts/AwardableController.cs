
/**
 *           Module: AwardableController.cs
 *  Descriptiontion: Sample game script used to demo how to use NFTs and interact 
 *                   with Nethereum contract calls.
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
using MoralisWeb3ApiSdk;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Nethereum.Contracts;
using System.Numerics;
#if UNITY_WEBGL
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Web3Api.Models;
#else
using System.Threading.Tasks;
using Moralis.Platform.Objects;
using Moralis.Web3Api.Models;
#endif

/// <summary>
/// Sample game script used to demo how to use NFTs and interact with Nethereum contract calls.
/// </summary>
public class AwardableController : MonoBehaviour
{
    public string NftTokenId;
    public string AwardContractAddress;

    public bool isOwned = false;

    private bool isInitialized = false;
    private bool canBeClaimed = false;

    // Start is called before the first frame update
    async void Awake()
    {
       
    }
    
    // Update is called once per frame
    async void Update()
    {
        // Note this is for demonstration purposes only and is not
        // the most efficiant place for this check.
        if (!isInitialized && MoralisInterface.Initialized && MoralisInterface.IsLoggedIn())
        {
            isInitialized = true;

            MoralisUser user = await MoralisInterface.GetUserAsync();

            string addr = user.authData["moralisEth"]["id"].ToString();

            try
            {
#if UNITY_WEBGL
                NftOwnerCollection noc =
                    await MoralisInterface.GetClient().Web3Api.Account.GetNFTsForContract(addr.ToLower(),
                    AwardContractAddress,
                    ChainList.mumbai);
#else
                NftOwnerCollection noc =
                    await MoralisInterface.GetClient().Web3Api.Account.GetNFTsForContract(addr.ToLower(),
                    AwardContractAddress,
                    ChainList.mumbai);
#endif
                IEnumerable<NftOwner> ownership = from n in noc.Result
                                                  where n.TokenId.Equals(NftTokenId.ToString())
                                                  select n;

                if (ownership != null && ownership.Count() > 0)
                {
                    isOwned = true;
                    // Hide the NFT Gmae object since it is already owned.
                    transform.gameObject.SetActive(false);
                }
            }
            catch (Exception exp)
            {
                Debug.LogError(exp.Message);
            }
        }

        // Process mouse click on the NFT Gameobject if intialized, NFT can be
        // claimed and is not already owned.
        if (isInitialized && 
            canBeClaimed && 
            !isOwned &&
            Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var diff = UnityEngine.Vector3.Distance(hit.transform.position, transform.position);
                // If the click is very close to the location of the NFT process.
                // This may not be the best way to detect a click on the object
                // but it seems to work good enough for this example.
                if (diff < 0.9f)
                {
                    await ClaimRewardAsync();
                }
            }
        }
    }

    private async UniTask ClaimRewardAsync()
    {
        // Do not process if already owned as the claim will fail in the contract call and waste gas fees.
        if (isOwned) return;

        // Need the user for the wallet address
        MoralisUser user = await MoralisInterface.GetUserAsync();

        string addr = user.authData["moralisEth"]["id"].ToString();

        // Convert token id to integer
        BigInteger bi = 0;

        if (BigInteger.TryParse(NftTokenId, out bi))
        {
            // Convert token id to hex as this is what the contract call expects
            object[] pars = new object[] { bi.ToString("x") };

            // Set gas estimate
            HexBigInteger gas = new HexBigInteger(80000); 

            // Call the contract to claim the NFT reward.
            string resp = await MoralisInterface.SendEvmTransactionAsync("Rewards", "mumbai", "claimReward", addr, gas, new HexBigInteger("0x0"), pars);

            // Hide the NFT GameObject since it has been claimed
            // You could also play a victory sound etc.
            transform.gameObject.SetActive(false);
        }
    }

    public void Display(UnityEngine.Vector3 vec3)
    {
        transform.Translate(vec3);
    }

    public void SetCanBeClaimed()
    {
        canBeClaimed = true;
    }
}
