/**
 *           Module: SplTokenBalanace.cs
 *  Descriptiontion: Solana Token Balanace 
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
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Moralis.WebGL.SolanaApi.Models
{
    [DataContract]
    public class SplTokenBalanace
    {
        [DataMember(Name = "associatedTokenAddress", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "associatedTokenAddress")]
        public string AssociatedTokenAddress { get; set; }

        [DataMember(Name = "mint", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "mint")]
        public string Mint { get; set; }

        [DataMember(Name = "amount", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        [DataMember(Name = "amountRaw", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "amountRaw")]
        public string AmountRaw { get; set; }

        [DataMember(Name = "decimals", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "decimals")]
        public int? Decimals { get; set; }
    }
}
