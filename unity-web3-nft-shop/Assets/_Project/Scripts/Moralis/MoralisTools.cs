using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MoralisUnity;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Web3Api.Models;
using UnityEngine;

public class MoralisTools
{
    public static void CheckNftOnOpenSea(string contractAddress, string contractChain, string tokenId)
    {
        string url = $"https://testnets.opensea.io/assets/{contractChain}/{contractAddress}/{tokenId}";
        Application.OpenURL(url);
    }
    
    public static async UniTask<string> SaveToIpfs(string name, string data)
    {
        string pinPath = null;

        try
        {
            IpfsFileRequest req = new IpfsFileRequest()
            {
                Path = name,
                Content = data
            };

            List<IpfsFileRequest> reqs = new List<IpfsFileRequest>();
            reqs.Add(req);
            List<IpfsFile> resp = await Moralis.GetClient().Web3Api.Storage.UploadFolder(reqs);

            IpfsFile ipfs = resp.FirstOrDefault<IpfsFile>();

            if (ipfs != null)
            {
                pinPath = ipfs.Path;
            }
        }
        catch (Exception exp)
        {
            Debug.LogError($"IPFS Save failed: {exp.Message}");
        }

        return pinPath;
    }

    private async UniTask<string> SaveImageToIpfs(string name, byte[] data)
    {
        return await SaveToIpfs(name, Convert.ToBase64String(data));
    }

    public static object BuildMetadata(string name, string desc, string imageUrl)
    {
        object o = new { name = name, description = desc, image = imageUrl };

        return o; 
    }

    public static long ConvertStringToLong(string stringToConvert)
    {
        long longFromString = 0;

        foreach (char ch in stringToConvert)
        {
            //Apply whatever algorithm you want here. Adding up each char numerical value works for this application.
            longFromString += (long) char.GetNumericValue(ch);
        }

        return longFromString;
    }
    
    public static async UniTask<string> GetWalletAddress()
    {
        if (!Moralis.Initialized) return null;

        if (!Moralis.IsLoggedIn()) return null;
        
        MoralisUser user = await Moralis.GetUserAsync();
        var walletAddress = user.authData["moralisEth"]["id"].ToString();

        return walletAddress;
    }
}
