/**
 *           Module: MetaplexNft.cs
 *  Descriptiontion: Solana Metaplex NFT 
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

namespace Moralis.SolanaApi.Models
{
    [DataContract]
    public class MetaplexNft
    {
        [DataMember(Name = "metadataUri", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "metadataUri")]
        public string MetadataUri { get; set; }

        [DataMember(Name = "masterEdition", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "masterEdition")]
        public bool MasterEdition { get; set; }

        [DataMember(Name = "isMutable", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "isMutable")]
        public bool IsMutable { get; set; }

        [DataMember(Name = "primarySaleHappened", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "primarySaleHappened")]
        public bool PrimarySaleHappened { get; set; }

        [DataMember(Name = "sellerFeeBasisPoints", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "sellerFeeBasisPoints")]
        public long SellerFeeBasisPoints { get; set; }

        [DataMember(Name = "updateAuthority", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "updateAuthority")]
        public string UpdateAuthority { get; set; }
    }
}
