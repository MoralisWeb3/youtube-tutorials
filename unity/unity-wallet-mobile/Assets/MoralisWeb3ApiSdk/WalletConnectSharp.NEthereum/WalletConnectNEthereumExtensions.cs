using System;
using System.Net.Http.Headers;
using Nethereum.JsonRpc.Client;
using WalletConnectSharp.Core;
using WalletConnectSharp.NEthereum.Client;

namespace WalletConnectSharp.NEthereum
{
    public static class WalletConnectNEthereumExtensions
    {
        /// <summary>
        /// Create a new NEtehereum IClient instance that uses Infura as the read client. You must specify your
        /// infura project ID. The Infura instance will only be used to make read calls (such as eth_call
        /// or eth_estimateGas), all other calls (eth_sendTransaction) will go through the WalletConnectProtocol
        /// instance given. The returned IClient instance can be used as a Provider in an NEthereum Web3 instance
        /// </summary>
        /// <param name="protocol">The WalletConnectProtocol to use</param>
        /// <param name="infruaId">The project ID of the Infura instance to connect to for read calls</param>
        /// <param name="network">An optional network name to use. Used in the Infura URL</param>
        /// <param name="authenticationHeader">An optional authentication header to provide to the endpoint</param>
        /// <returns>
        /// A new NEtehereum IClient instance that uses Infura as the read client and the WalletConnectProtocol
        /// for write client. The returned IClient instance can be used as a Provider in an NEthereum Web3 instance
        /// </returns>
        public static IClient CreateProviderWithInfura(this WalletConnectProtocol protocol, string infruaId, string network = "mainnet", AuthenticationHeaderValue authenticationHeader = null)
        {
            string url = "https://" + network + ".infura.io/v3/" + infruaId;

            return CreateProvider(protocol, new Uri(url), authenticationHeader);
        }

        /// <summary>
        /// Create a new NEtehereum IClient instance that uses a JSON-RPC endpoint as the read client.
        /// The JSON-RPC endpoint given will only be used to make read calls (such as eth_call
        /// or eth_estimateGas), all other calls (eth_sendTransaction) will go through the WalletConnectProtocol
        /// instance given. The returned IClient instance can be used as a Provider in an NEthereum Web3 instance
        /// </summary>
        /// <param name="protocol">The WalletConnectProtocol to use</param>
        /// <param name="url">The URL of the JSON-RPC endpoint (i.e geth)</param>
        /// <param name="authenticationHeader">An optional authentication header to provide to the endpoint</param>
        /// <returns>
        /// A new NEtehereum IClient instance that uses a JSON-RPC endpoint as the read client and the
        /// WalletConnectProtocol for write client. The returned IClient instance can be used as a
        /// Provider in an NEthereum Web3 instance
        /// </returns>
        public static IClient CreateProvider(this WalletConnectProtocol protocol, Uri url, AuthenticationHeaderValue authenticationHeader = null)
        {
            return CreateProvider(protocol,
                new RpcClient(url, authenticationHeader)
            );
        }

        /// <summary>
        /// Create a new NEtehereum IClient instance that uses another IClient instance as the read client.
        /// The IClient instance given will only be used to make read calls (such as eth_call
        /// or eth_estimateGas), all other calls (eth_sendTransaction) will go through the WalletConnectProtocol
        /// instance given. The returned IClient instance can be used as a Provider in an NEthereum Web3 instance
        /// </summary>
        /// <param name="protocol">The WalletConnectProtocol to use</param>
        /// <param name="readClient">Any IClient instance to use as the read client</param>
        /// <returns>
        /// A new NEtehereum IClient instance that uses another IClient instance as the read client and the
        /// WalletConnectProtocol for write client. The returned IClient instance can be used as a
        /// Provider in an NEthereum Web3 instance
        /// </returns>
        public static IClient CreateProvider(this WalletConnectProtocol protocol, IClient readClient)
        {
            if (!protocol.Connected)
            {
                throw new Exception("No connection has been made yet!");
            }
            
            return new FallbackProvider(
                new WalletConnectClient(protocol),
                readClient
            );
        }
    }
}