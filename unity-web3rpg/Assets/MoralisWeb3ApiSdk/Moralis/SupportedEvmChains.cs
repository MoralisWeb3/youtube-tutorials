/**
 *           Module: ChainEntry.cs
 *  Descriptiontion: Provides a easy way to get detail about an EVM chain for 
 *  all EVM chains supported by the Moralis Web3API
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
using System.Linq;

#if UNITY_WEBGL
using Moralis.WebGL.Web3Api.Models;

namespace Assets.Scripts.Moralis
{
    /// <summary>
    /// Provides a easy way to get detail about an EVM chain for all EVM chains 
    /// supported by the Moralis Web3API
    /// </summary>
    public class SupportedEvmChains
    {
        private static List<ChainEntry> chains = new List<ChainEntry>();

        /// <summary>
        /// The list of EVM chains supported by the Moralis Web3API.
        /// </summary>
        public static List<ChainEntry> SupportedChains 
        { 
            get 
            {
                if (chains.Count < 1) PopulateChainList();

                return chains; 
            } 
        }
        
        /// <summary>
        /// Retrieve an chain entry by enum value.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ChainEntry FromChainList(ChainList target)
        {
            ChainEntry result = null;

            var searchResult = from c in SupportedChains
                               where target.Equals(c.EnumValue)
                               select c;

            result = searchResult.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Retrieve an chain entry by enum value.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ChainEntry FromChainList(string target)
        {
            ChainEntry result = null;

            var searchResult = from c in SupportedChains
                               where target.Equals(c.Name)
                               select c;

            result = searchResult.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Retrieve an chain entry by enum value.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ChainEntry FromChainList(int target)
        {
            ChainEntry result = null;

            var searchResult = from c in SupportedChains
                               where target.Equals(c.ChainId)
                               select c;

            result = searchResult.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Loops through the current ChainList enum and builds a friendly to use 
        /// name / chainId, enum val entry.
        /// </summary>
        private static void PopulateChainList()
        {
            chains.Clear();

            foreach (ChainList chain in Enum.GetValues(typeof(ChainList)))
            {
                chains.Add(new ChainEntry()
                {
                    ChainId = (int)chain,
                    EnumValue = chain,
                    Name = chain.ToString()
                });
            }
        }
    }
}
#else
using Moralis.Web3Api.Models;

namespace Assets.Scripts.Moralis
{
    /// <summary>
    /// Provides a easy way to get detail about an EVM chain for all EVM chains 
    /// supported by the Moralis Web3API
    /// </summary>
    public class SupportedEvmChains
    {
        private static List<ChainEntry> chains = new List<ChainEntry>();

        /// <summary>
        /// The list of EVM chains supported by the Moralis Web3API.
        /// </summary>
        public static List<ChainEntry> SupportedChains 
        { 
            get 
            {
                if (chains.Count < 1) PopulateChainList();

                return chains; 
            } 
        }

        /// <summary>
        /// Retrieve an chain entry by enum value.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ChainEntry FromChainList(ChainList target)
        {
            ChainEntry result = null;

            var searchResult = from c in SupportedChains
                               where target.Equals(c.EnumValue)
                               select c;

            result = searchResult.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Retrieve an chain entry by enum value.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ChainEntry FromChainList(string target)
        {
            ChainEntry result = null;

            var searchResult = from c in SupportedChains
                               where target.Equals(c.Name)
                               select c;

            result = searchResult.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Retrieve an chain entry by enum value.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ChainEntry FromChainList(int target)
        {
            ChainEntry result = null;

            var searchResult = from c in SupportedChains
                               where target.Equals(c.ChainId)
                               select c;

            result = searchResult.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Loops through the current ChainList enum and builds a friendly to use 
        /// name / chainId, enum val entry.
        /// </summary>
        private static void PopulateChainList()
        {
            chains.Clear();

            foreach (ChainList chain in Enum.GetValues(typeof(ChainList)))
            {
                chains.Add(new ChainEntry()
                {
                    ChainId = (int)chain,
                    EnumValue = chain,
                    Name = chain.ToString()
                });
            }
        }
    }
}
#endif
