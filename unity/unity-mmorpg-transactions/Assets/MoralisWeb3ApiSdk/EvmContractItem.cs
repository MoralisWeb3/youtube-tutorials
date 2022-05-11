/**
 *           Module: EvmContractItem.cs
 *  Descriptiontion: Class that wraps a list on Nethereum contract instances by chain
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
using System.Collections.Generic;
using Moralis;

#if !UNITY_WEBGL

using Nethereum.Contracts;
using Nethereum.Web3;

namespace MoralisWeb3ApiSdk
{
    /// <summary>
    /// Wraps a list on Nethereum contract instances by chain
    /// </summary>
    public class EvmContractItem
    {
        /// <summary>
        /// the raw contract ABI
        /// </summary>
        public string Abi { get; set; }

        /// <summary>
        /// Contract instance by chain
        /// </summary>
        public Dictionary<string, EvmContractInstance> ChainContractMap { get; set; }

        public EvmContractItem()
        {
            ChainContractMap = new Dictionary<string, EvmContractInstance>();
        }

        public EvmContractItem(Web3 client, string abi, string chainId, string contractAddress)
        {
            ChainContractMap = new Dictionary<string, EvmContractInstance>();
            this.Abi = abi;

            AddChainInstance(client, chainId, contractAddress);
        }

        /// <summary>
        /// Add a contract instance for a specific chain.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="chainId"></param>
        /// <param name="contractAddress"></param>
        public void AddChainInstance(Web3 client, string chainId, string contractAddress)
        {
            if (!ChainContractMap.ContainsKey(chainId))
            {
                ChainEntry chainInfo = SupportedEvmChains.FromChainList(chainId);
                Contract contractInstance = client.Eth.GetContract(this.Abi, contractAddress);
                EvmContractInstance eci = new EvmContractInstance()
                {
                    ChainInfo = chainInfo,
                    ContractAddress = contractAddress,
                    ContractInstance = contractInstance
                };

                ChainContractMap.Add(chainId, eci);
            }
        }
    }
}
#endif
