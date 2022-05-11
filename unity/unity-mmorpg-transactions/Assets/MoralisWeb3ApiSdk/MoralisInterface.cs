/**
 *           Module: MoralisInterface.cs
 *  Descriptiontion: Class that wraps moralis integration points. Provided as an 
 *                   example of how Moralis can be integrated into Unity
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
using System;
using System.Collections.Generic;
using System.Threading;
using Moralis;
using Newtonsoft.Json;
using UnityEngine;

#if UNITY_WEBGL
using Cysharp.Threading.Tasks;
using Moralis.WebGL;
using Moralis.WebGL.Models;
using Moralis.WebGL.Hex.HexTypes;
using Moralis.WebGL.Web3Api.Client;
using Moralis.WebGL.SolanaApi.Client;
using Moralis.WebGL.Platform;
using Moralis.WebGL.Platform.Objects;
#else
using System.Threading.Tasks;
using Moralis.Web3Api.Client;
using Moralis.SolanaApi.Client;
using Moralis.Platform;
using Moralis.Platform.Objects;
using Nethereum.Web3;
using WalletConnectSharp.Unity;
using WalletConnectSharp.Core;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.NEthereum;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
#endif

namespace MoralisWeb3ApiSdk
{
#if UNITY_WEBGL
    /// <summary>
    /// Class that wraps moralis integration points. Provided as an example of 
    /// how Moralis can be integrated into Unity
    /// </summary>
    public class MoralisInterface : MonoBehaviour
    {
        /// <summary>
        /// Indicates that the MoralisInterface has been initialized.
        /// </summary>
        public static bool Initialized { get; set; }
        /// <summary>
        /// Provide Web3 for WebGL client.
        /// </summary>
        public static Web3GL Web3Client { get; set; }

        private static string web3ClientRpcUrl;
        private static EvmContractManager contractManager;

        // Singleton instance of Moralis so that is it is available application 
        // wide after being initialized.
        private static Moralis.WebGL.MoralisClient moralis;
        private static ServerConnectionData connectionData;
        private static ClientMeta clientMetaData;

        // Since the user object is used so often, once the user is authenticated 
        // keep a local copy to save some cycles.
        private static MoralisUser user;

        /// <summary>
        /// Initializes the connection to a Moralis server.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="serverUri"></param>
        /// <param name="hostData"></param>
        /// <param name="web3ApiKey"></param>
        public static async UniTask Initialize(string applicationId, string serverUri, HostManifestData hostData, ClientMeta clientMeta = null, string web3ApiKey = null)
        {
            // Application Id is requried.
            if (string.IsNullOrEmpty(applicationId))
            {
                Debug.LogError("Application Id is required.");
                throw new ArgumentException("Application Id was not supplied.");
            }
            // Server URI is required.
            if (string.IsNullOrEmpty(serverUri))
            {
                Debug.LogError("Server URI is required.");
                throw new ArgumentException("Server URI was not supplied.");
            }

            // CHeck that requried Host data properties are set.
            if (hostData == null ||
                string.IsNullOrEmpty(hostData.Version) ||
                string.IsNullOrEmpty(hostData.Name) ||
                string.IsNullOrEmpty(hostData.ShortVersion) ||
                string.IsNullOrEmpty(hostData.Identifier))
            {
                Debug.LogError("Complete host manifest data are required.");
                throw new ArgumentException("Complete host manifest data was not supplied.");
            }

            // Create instance of Evm Contract Manager.
            contractManager = new EvmContractManager();

            // Set Moralis conenction values.
            connectionData = new ServerConnectionData();
            connectionData.ApplicationID = applicationId;
            connectionData.ServerURI = serverUri;
            connectionData.ApiKey = web3ApiKey;

            // For unity apps the local storage value must also be set.
            connectionData.LocalStoragePath = Application.persistentDataPath;

            Debug.Log($"Set LocalStoragePath to {connectionData.LocalStoragePath}");

            // TODO Make this optional!
            connectionData.Key = "";

            Debug.Log("Connecting to Moralis ...");

            // Set manifest / host data required so that the Moralis Client does not
            // attempt to infer them from Assembly values not available in Unity.
            Moralis.WebGL.MoralisClient.ManifestData = hostData;

            // Define a Unity specific Json Serializer.
            UnityNewtosoftSerializer jsonSerializer = new UnityNewtosoftSerializer();

            // If user passed web3apikey, add it to configuration.
            if (web3ApiKey is { }) Moralis.WebGL.SolanaApi.Client.Configuration.ApiKey["X-API-Key"] = web3ApiKey;
            if (web3ApiKey is { }) Moralis.WebGL.Web3Api.Client.Configuration.ApiKey["X-API-Key"] = web3ApiKey;

            // Create an instance of Moralis Server Client
            // NOTE: Web3ApiClient is optional. If you are not using the Moralis 
            // Web3Api REST API you can call the method with just connectionData
            // NOTE: If you are using a custom user object use 
            // new MoralisClient<YourUser>(connectionData, address, Web3ApiClient)
            moralis = new Moralis.WebGL.MoralisClient(connectionData, new Web3ApiClient(), new SolanaApiClient(), jsonSerializer);

            clientMetaData = clientMeta;

            if (moralis == null)
            {
                Debug.Log("Moralis connection failed!");
            }
            else
            {
                Initialized = true;
                Debug.Log("Connected to Moralis!");
                user = await moralis.GetCurrentUserAsync();
            }
        }

        /// <summary>
        /// Properly dispose Moralis Client, shuts down any subscriptions, etc.
        /// </summary>
        public static void Dispose()
        {
            moralis.Dispose();
        }

        /// <summary>
        /// Get the Moralis Server Client.
        /// </summary>
        /// <returns></returns>
        public static Moralis.WebGL.MoralisClient GetClient()
        {
            return moralis;
        }

        /// <summary>
        /// Provides the current authenticated user if Moralis 
        /// authentication has been completed.
        /// </summary>
        /// <returns>MoralisUser</returns>
        public static async UniTask<MoralisUser> GetUserAsync()
        {
            if (user == null)
            {
                user = await moralis.GetCurrentUserAsync();
            }

            return user;
        }

        /// <summary>
        /// Idicates if user is already logged in.
        /// </summary>
        /// <returns></returns>
        public static bool IsLoggedIn() { return user != null; }

        /// <summary>
        /// Authenicate the user by logging into Moralis using message signed by 
        /// Crypto Wallat. If this is a new user, the user's record is automatically 
        /// created.
        /// EXAMPLE: { { "id", address }, { "signature", response }, { "data", "Moralis Authentication" } }
        /// </summary>
        /// <param name="authData"></param>
        /// <returns></returns>
        public static async UniTask<MoralisUser> LogInAsync(IDictionary<string, object> authData)
        {
            return await moralis.LogInAsync(authData, CancellationToken.None);
        }

        /// <summary>
        /// Logout the user session.
        /// </summary>
        /// <returns></returns>
        public static UniTask LogOutAsync()
        {
            return moralis.LogOutAsync();
        }

        /// <summary>
        /// Initializes the Web3 connection to the supplied RPC Url. Call this to change the target chain.
        /// </summary>
        /// <param name="rpcUrl"></param>
        /// <returns></returns>
        public static async UniTask<string> SetupWeb3()
        {
            if (clientMetaData == null)
            {
                Debug.Log("Web3 Metadata not provided.");
                return null;
            }

            string userAcct = await Web3GL.Connect(clientMetaData);

            return userAcct;
        }

        /// <summary>
        /// Performs a transfer of value to receipient.
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="value"></param>
        /// <param name="gas"></param>
        /// <param name="gasPrice"></param>
        /// <returns></returns>
        public async static UniTask<string> SendTransactionAsync(string recipient, HexBigInteger value, HexBigInteger gas = null, HexBigInteger gasPrice = null)
        {
            string g = "";
            string gp = "";

            if (gas != null) g = gas.Value.ToString();
            if (gasPrice != null) gp = gasPrice.Value.ToString();
            string txnHash = await Web3GL.SendTransaction(recipient, value.Value.ToString(), g, gp);

            return txnHash;
        }

        /// <summary>
        /// Executes a contract function.
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="abi"></param>
        /// <param name="functionName"></param>
        /// <param name="args"></param>
        /// <param name="value"></param>
        /// <param name="gas"></param>
        /// <param name="gasPrice"></param>
        /// <returns></returns>
        public async static UniTask<string> ExecuteFunction(string contractAddress,
            string abi,
            string functionName,
            object[] args,
            HexBigInteger value,
            HexBigInteger gas,
            HexBigInteger gasPrice)
        {
            string gasValue = gas.Value.ToString();
            string gasPriceValue = gasPrice.ToString();

            if (gasValue.Equals("0") || gasValue.Equals("0x0")) gasValue = "";

            if (gasPriceValue.Equals("0") || gasPriceValue.Equals("0x0")) gasPriceValue = "";

            string functionArgs = JsonConvert.SerializeObject(args);
            string resp = await Web3GL.SendContract(functionName, abi, contractAddress, functionArgs, value.Value.ToString(), gasValue, gasPriceValue);

            return resp;
        }

        public static List<ChainEntry> SupportedChains => SupportedEvmChains.SupportedChains;

    }
#else

    /// <summary>
    /// Class that wraps moralis integration points. Provided as an example of 
    /// how Moralis can be integrated into Unity
    /// </summary>
    public class MoralisInterface : MonoBehaviour
    {
        private static EvmContractManager contractManager;

        // Singleton instance of Moralis so that is it is available application 
        // wide after being initialized.
        private static MoralisClient moralis;
        private static ServerConnectionData connectionData;
        // Since the user object is used so often, once the user is authenticated 
        // keep a local copy to save some cycles.
        private static MoralisUser user;

        private static ClientMeta clientMetaData;
        public static bool Initialized { get; set; }

        public static Web3 Web3Client { get; set; }

        /// <summary>
        /// Initializes the connection to a Moralis server.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="serverUri"></param>
        /// <param name="hostData"></param>
        /// <param name="web3ApiKey"></param>
        public static async Task Initialize(string applicationId, string serverUri, HostManifestData hostData, ClientMeta clientMeta, string web3ApiKey = null)
        {
            // Application Id is requried.
            if (string.IsNullOrEmpty(applicationId))
            {
                Debug.LogError("Application Id is required.");
                throw new ArgumentException("Application Id was not supplied.");
            }
            // Server URI is required.
            if (string.IsNullOrEmpty(serverUri))
            {
                Debug.LogError("Server URI is required.");
                throw new ArgumentException("Server URI was not supplied.");
            }

            // CHeck that requried Host data properties are set.
            if (hostData == null ||
                string.IsNullOrEmpty(hostData.Version) ||
                string.IsNullOrEmpty(hostData.Name) ||
                string.IsNullOrEmpty(hostData.ShortVersion) ||
                string.IsNullOrEmpty(hostData.Identifier))
            {
                Debug.LogError("Complete host manifest data are required.");
                throw new ArgumentException("Complete host manifest data was not supplied.");
            }

            // Create instance of Evm Contract Manager.
            contractManager = new EvmContractManager();

            // Set Moralis conenction values.
            connectionData = new ServerConnectionData();
            connectionData.ApplicationID = applicationId;
            connectionData.ServerURI = serverUri;
            connectionData.ApiKey = web3ApiKey;

            // For unity apps the local storage value must also be set.
            connectionData.LocalStoragePath = Application.persistentDataPath;

            // TODO Make this optional!
            connectionData.Key = "";

            Debug.Log("Connecting to Moralis ...");

            // Set manifest / host data required so that the Moralis Client does not
            // attempt to infer them from Assembly values not available in Unity.
            MoralisClient.ManifestData = hostData;

            // Define a Unity specific Json Serializer.
            UnityNewtosoftSerializer jsonSerializer = new UnityNewtosoftSerializer();

            // If user passed web3apikey, add it to configuration.
            if (web3ApiKey is { }) Moralis.SolanaApi.Client.Configuration.ApiKey["X-API-Key"] = web3ApiKey;
            if (web3ApiKey is { }) Moralis.Web3Api.Client.Configuration.ApiKey["X-API-Key"] = web3ApiKey;

            // Create an instance of Moralis Server Client
            // NOTE: Web3ApiClient is optional. If you are not using the Moralis 
            // Web3Api REST API you can call the method with just connectionData
            // NOTE: If you are using a custom user object use 
            // new MoralisClient<YourUser>(connectionData, address, Web3ApiClient)
            moralis = new MoralisClient(connectionData, new Web3ApiClient(), new SolanaApiClient(), jsonSerializer);

            clientMetaData = clientMeta;

            if (moralis == null)
            {
                Debug.Log("Moralis connection failed!");
            }
            else
            {
                Initialized = true;
                Debug.Log("Connected to Moralis!");
                user = moralis.GetCurrentUser();
            }
        }

        /// <summary>
        /// Properly dispose Moralis Client, shuts down any subscriptions, etc.
        /// </summary>
        public static void Dispose()
        {
            moralis.Dispose();
        }

        /// <summary>
        /// Get the Moralis Server Client.
        /// </summary>
        /// <returns></returns>
        public static MoralisClient GetClient()
        {
            return moralis;
        }

        /// <summary>
        /// Provides the current authenticated user if Moralis 
        /// authentication has been completed.
        /// </summary>
        /// <returns>MoralisUser</returns>
        public static MoralisUser GetUser()
        {
            if (user == null)
            {
                user = moralis.GetCurrentUser();
            }

            return user;
        }

        public static async Task<MoralisUser> GetUserAsync()
        {
            return await Task.Run(() => GetUser());
        }

        /// <summary>
        /// Idicates if user is already logged in.
        /// </summary>
        /// <returns></returns>
        public static bool IsLoggedIn() { return user != null; }

        /// <summary>
        /// Authenicate the user by logging into Moralis using message signed by 
        /// Crypto Wallat. If this is a new user, the user's record is automatically 
        /// created.
        /// EXAMPLE: { { "id", address }, { "signature", response }, { "data", "Moralis Authentication" } }
        /// </summary>
        /// <param name="authData"></param>
        /// <returns></returns>
        public static Task<MoralisUser> LogInAsync(IDictionary<string, object> authData)
        {
            return moralis.LogInAsync(authData, CancellationToken.None);
        }

        /// <summary>
        /// Login using username and password.
        /// </summary>
        /// <param name="username">username / Email</param>
        /// <param name="password">user password</param>
        /// <returns>MoralisUser</returns>
        public static Task<MoralisUser> LogInAsync(string username, string password)
        {
            return moralis.UserService.LogInAsync(username, password, moralis.ServiceHub);
        }

        /// <summary>
        /// Logout the user session.
        /// </summary>
        /// <returns></returns>
        public static Task LogOutAsync()
        {
            return moralis.LogOutAsync();
        }

        /// <summary>
        /// Initializes the Web3 connection to the supplied RPC Url. Call this to change the target chain.
        /// </summary>
        /// <param name="rpcUrl"></param>
        /// <returns></returns>
        public static void SetupWeb3()
        {
            if (clientMetaData == null)
            {
                Debug.Log("Wallet Connect Metadata not provided.");
                return;
            }

            WalletConnectSession client = WalletConnect.Instance.Session;

            // Create a web3 client using Wallet Connect as write client and a dummy client as read client.
            // Read operations should be via Web3API. Read operation are not implemented in the Web3 Client
            // Use the Web3API for read operations as available. If you must make run a read request that is
            // not supported by Web3API you will need to use the Wallet Connect method:
            // CreateProviderWithInfura(this WalletConnectProtocol protocol, string infruaId, string network = "mainnet", AuthenticationHeaderValue authenticationHeader = null)
            // We do not recommned this though
            Web3Client = new Web3(client.CreateProvider( new DeadRpcReadClient((string s) => {
                Debug.LogError(s);
            })));
            //
            //Web3Client = new Web3(client.CreateProvider(new Uri(rpcUrl)));
        }

        /// <summary>
        /// Creates and adds a contract instance based on ABI and associates it to specified chain and address.
        /// </summary>
        /// <param name="key">How you identify the contract instance.</param>
        /// <param name="abi">ABI of the contract in standard ABI json format</param>
        /// <param name="baseChainId">The initial chain Id used to interact with this contract</param>
        /// <param name="baseContractAddress">The initial contract address of the contract on specified chain</param>
        public static void InsertContractInstance(string key, string abi, string baseChainId, string baseContractAddress)
        {
            if (Web3Client == null)
            {
                Debug.LogError("Web3 has not been setup yet.");
            }
            else
            {
                EvmContractItem eci = new EvmContractItem(Web3Client, abi, baseChainId, baseContractAddress);

                contractManager.InsertContractInstance(key, eci);
            }
        }

        /// <summary>
        /// Adds a contract address for a chain to a specific contract. Contract for key must exist.
        /// </summary>
        /// <param name="key">How you identify the contract instance.</param>
        /// <param name="chainId">The The chain the contract is deployed on.</param>
        /// <param name="contractAddress">Address the contract is deployed at</param>
        public static void AddContractChainAddress(string key, string chainId, string contractAddress)
        {
            if (Web3Client == null)
            {
                Debug.LogError("Web3 has not been setup yet.");
            }
            else
            {
                contractManager.AddChainInstanceToContract(key, Web3Client, chainId, contractAddress);
            }
        }

        /// <summary>
        /// Retrieves the specified contract instance if it exists.
        /// </summary>
        /// <param name="key">How you identify the contract instance.</param>
        /// <param name="chainId">The The chain the contract is deployed on.</param>
        /// <returns>Nethereum.Contracts.Contract</returns>
        public static Contract EvmContractInstance(string key, string chainId)
        {
            Contract contract = null;

            if (Web3Client == null)
            {
                Debug.LogError("Web3 has not been setup yet.");
            }
            else
            {
                if (contractManager.Contracts.ContainsKey(key) && 
                    contractManager.Contracts[key].ChainContractMap.ContainsKey(chainId))
                {
                    contract = contractManager.Contracts[key].ChainContractMap[chainId].ContractInstance;
                }
            }

            return contract;
        }

        /// <summary>
        /// Get an Nethereum Function instance from a specific contract.
        /// </summary>
        /// <param name="key">How you identify the contract instance.</param>
        /// <param name="chainId">The The chain the contract is deployed on.</param>
        /// <param name="functionName">Name of the function to return</param>
        /// <returns>Function</returns>
        public static Function EvmContractFunctionInstance(string key, string chainId, string functionName)
        {
            Contract contract = EvmContractInstance(key, chainId);
            Function function = null;

            if (contract != null)
            {
                function = contract.GetFunction(functionName);
            }

            return function;
        }

        /// <summary>
        /// Executes a NEthereum SendTransactionAsync which executes a function 
        /// on a EVM contract (can change state) and returns response as a 
        /// string.
        /// </summary>
        /// <param name="contractKey">How you identify the contract instance.</param>
        /// <param name="chainId">he The chain the contract is deployed on.</param>
        /// <param name="functionName">name of function to call</param>
        /// <param name="transactionInput">NEthereum TransactionInput object</param>
        /// <param name="functionInput">Function params</param>
        /// <returns>string</returns>
        public static async Task<string> SendEvmTransactionAsync(string contractKey, string chainId, string functionName, TransactionInput transactionInput, object[] functionInput)
        {
            string result = null;

            if (Web3Client == null)
            {
                Debug.LogError("Web3 has not been setup yet.");
            }
            else
            {
                if (contractManager.Contracts.ContainsKey(contractKey) &&
                    contractManager.Contracts[contractKey].ChainContractMap.ContainsKey(chainId))
                {
                    Tuple<bool,string,string> resp = await contractManager.SendTransactionAsync(contractKey, chainId, functionName, transactionInput, functionInput);

                    if (resp.Item1) result = resp.Item2;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractKey"></param>
        /// <param name="chainId"></param>
        /// <param name="functionName"></param>
        /// <param name="fromaddress"></param>
        /// <param name="gas"></param>
        /// <param name="value"></param>
        /// <param name="functionInput"></param>
        /// <returns></returns>
        public static async Task<string> SendEvmTransactionAsync(string contractKey, string chainId, string functionName, string fromaddress, HexBigInteger gas, HexBigInteger value, object[] functionInput)
        {
            string result = null;

            if (Web3Client == null)
            {
                Debug.LogError("Web3 has not been setup yet.");
            }
            else
            {
                if (contractManager.Contracts.ContainsKey(contractKey) &&
                    contractManager.Contracts[contractKey].ChainContractMap.ContainsKey(chainId))
                {
                    Tuple<bool, string, string> resp = await contractManager.SendTransactionAsync(contractKey, chainId, functionName, fromaddress, gas, value, functionInput);
 
                    if (resp.Item1)
                    {
                        result = resp.Item2;
                    }
                    else
                    {
                        Debug.LogError($"Evm Transaction failed: {resp.Item3}");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractKey"></param>
        /// <param name="chainId"></param>
        /// <param name="functionName"></param>
        /// <param name="fromaddress"></param>
        /// <param name="gas"></param>
        /// <param name="value"></param>
        /// <param name="functionInput"></param>
        /// <returns></returns>
        public static async Task<string> SendTransactionAndWaitForReceiptAsync(string contractKey, string chainId, string functionName, string fromaddress, HexBigInteger gas, HexBigInteger value, object[] functionInput)
        {
            string result = null;

            if (Web3Client == null)
            {
                Debug.LogError("Web3 has not been setup yet.");
            }
            else
            {
                if (contractManager.Contracts.ContainsKey(contractKey) &&
                    contractManager.Contracts[contractKey].ChainContractMap.ContainsKey(chainId))
                {
                    Tuple<bool, string, string> resp = await contractManager.SendTransactionAndWaitForReceiptAsync(contractKey, chainId, functionName, fromaddress, gas, value, functionInput);

                    if (resp.Item1)
                    {
                        result = resp.Item2;
                    }
                    else
                    {
                        Debug.LogError($"Evm Transaction failed: {resp.Item3}");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Provide quick access to the Moralis Web3API Supported chains list.
        /// </summary>
        public static List<ChainEntry> SupportedChains => SupportedEvmChains.SupportedChains;
    }
#endif
}