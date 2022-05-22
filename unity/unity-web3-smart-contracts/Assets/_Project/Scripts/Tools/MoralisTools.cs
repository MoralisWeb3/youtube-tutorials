using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MoralisUnity;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Web3Api.Models;
using UnityEngine;

namespace Web3MoriaGates
{ 
    public static class MoralisTools 
    {
        #region MAIN

        public static async UniTask<string> GetWalletAddress()
        {
            MoralisUser user = await Moralis.GetUserAsync();

            if (user is null) return null;
            
            var walletAddress = user.authData["moralisEth"]["id"].ToString();

            return walletAddress;
        }

        #endregion
        

        #region IPFS

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

        private static async UniTask<string> SaveImageToIpfs(string name, byte[] data)
        {
            return await SaveToIpfs(name, Convert.ToBase64String(data));
        }

        public static object BuildMetadata(string name, string desc, string imageUrl)
        {
            object o = new { name = name, description = desc, image = imageUrl };

            return o; 
        }

        #endregion


        #region OTHER

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

        #endregion
    }   
}
