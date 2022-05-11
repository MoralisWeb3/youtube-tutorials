#if UNITY_WEBGL
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Moralis.WebGL.Models;


namespace Moralis.WebGL
{
    public class Web3GL
    {
        private static bool isConnected;

        [DllImport("__Internal")]
        private static extern void ConnectWeb3(string appLogo, string appTitile, string appDesc);

        [DllImport("__Internal")]
        private static extern void SendContractJs(string method, string abi, string contract, string args, string value, string gasLimit, string gasPrice);

        [DllImport("__Internal")]
        private static extern string SendContractResponse();

        [DllImport("__Internal")]
        private static extern void SetContractResponse(string value);

        [DllImport("__Internal")]
        private static extern void SendTransactionJs(string to, string value, string gasLimit, string gasPrice);

        [DllImport("__Internal")]
        private static extern string SendTransactionResponse();

        [DllImport("__Internal")]
        private static extern void SetTransactionResponse(string value);

        [DllImport("__Internal")]
        private static extern void SignMessage(string value);

        [DllImport("__Internal")]
        private static extern string SignMessageResponse();

        [DllImport("__Internal")]
        private static extern void SetSignMessageResponse(string value);

        [DllImport("__Internal")]
        private static extern int GetNetwork();

        [DllImport("__Internal")]
        private static extern string ConnectAccount();

        [DllImport("__Internal")]
        private static extern bool SetDebugMode(bool mode);

        /// <summary>
        /// Connect to the browser wallet with web3.
        /// </summary>
        /// <param name="clientMeta"></param>
        /// <returns>string - account address</returns>
        public async static UniTask<string> Connect(ClientMeta clientMeta)
        {
            string account = "";
            string appLogo = "https://moralis.io/wp-content/uploads/2021/06/Powered-by-Moralis-Badge-Black.svg";
            int waitLoops = 240; // approx 2min @ 60FPS
            int loop = 0;

            if (clientMeta.Icons.Length > 0)
            {
                appLogo = clientMeta.Icons[0];
            }

            ConnectWeb3(appLogo, clientMeta.Name, clientMeta.Description);

            while (loop < waitLoops && account.Length < 1)
            {
                await UniTask.DelayFrame(30);

                account = Account();

                loop++;
            }

            if (account.Length >  0)
            {
                isConnected = true;

                return account;
            }
            else
            {
                throw new Exception("Web3 Login timed out or failed.");
            }
        }


        /// <summary>
        /// Indicates if the web3 objects has been created and connected properly.
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsConnected() => isConnected;

        /// <summary>
        /// Executes a method on a contract
        /// </summary>
        /// <param name="_method">Function to call</param>
        /// <param name="_abi">Contract ABI</param>
        /// <param name="_contract">Contract Address</param>
        /// <param name="_args">Function parameters as json</param>
        /// <param name="_value">value to send as wei</param>
        /// <param name="_gasLimit">gas limit OPTIONAL</param>
        /// <param name="_gasPrice">gas price OPTIONAL</param>
        /// <returns></returns>
        public async static UniTask<string> SendContract(string _method, string _abi, string _contract, string _args, string _value, string _gasLimit = "", string _gasPrice = "")
        {
            // Set response to empty
            SetContractResponse("");
            SendContractJs(_method, _abi, _contract, _args, _value, _gasLimit, _gasPrice);
            string response = SendContractResponse();
            while (response == "")
            {
                await UniTask.DelayFrame(30);

                response = SendContractResponse();
            }
            SetContractResponse("");
            // check if user submmited or user rejected
            if (response.Length == 66)
            {
                return response;
            }
            else
            {
                throw new Exception(response);
            }
        }

        /// <summary>
        /// Transfer value
        /// </summary>
        /// <param name="_to"></param>
        /// <param name="_value"></param>
        /// <param name="_gasLimit"></param>
        /// <param name="_gasPrice"></param>
        /// <returns></returns>
        public async static UniTask<string> SendTransaction(string _to, string _value, string _gasLimit = "", string _gasPrice = "")
        {
            // Set response to empty
            SetTransactionResponse("");
            SendTransactionJs(_to, _value, _gasLimit, _gasPrice);
            string response = SendTransactionResponse();
            while (response == "")
            {
                await UniTask.DelayFrame(30);
                response = SendTransactionResponse();
            }
            SetTransactionResponse("");
            // check if user submmited or user rejected
            if (response.Length == 66)
            {
                return response;
            }
            else
            {
                throw new Exception(response);
            }
        }

        /// <summary>
        /// Generate a signature using message
        /// </summary>
        /// <param name="_message"></param>
        /// <returns></returns>
        public async static UniTask<string> Sign(string _message)
        {
            SignMessage(_message);
            string response = SignMessageResponse();
            while (response == "")
            {
                await UniTask.DelayFrame(30);
                response = SignMessageResponse();
            }
            // Set response to empty
            SetSignMessageResponse("");
            // check if user submmited or user rejected
            if (response.Length == 132)
            {
                return response;
            }
            else
            {
                throw new Exception(response);
            }
        }

        /// <summary>
        /// Returns the current chainId
        /// </summary>
        /// <returns></returns>
        public static int ChainId()
        {
            return GetNetwork();
        }

        /// <summary>
        /// Connected Account
        /// </summary>
        /// <returns></returns>
        public static string Account()
        {
            return ConnectAccount();
        }

        /// <summary>
        /// Turns web3
        /// </summary>
        /// <param name="mode"></param>
        public static void DebugMode(bool mode)
        {
            SetDebugMode(mode);
        }
    }
}
#endif