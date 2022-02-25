using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using WalletConnectSharp.Core.Models.Ethereum.Types;

namespace WalletConnectSharp.Core.Models.Ethereum
{
    public sealed class EthSignTypedData<T> : JsonRpcRequest
    {
        [JsonProperty("params")] 
        private string[] _parameters;
        
        public EthSignTypedData(string address, T data, EIP712Domain domain)
        {
            this.Method = "eth_signTypedData";

            var typeData = new EvmTypedData<T>(data, domain);
            var encodedTypeData = JsonConvert.SerializeObject(typeData);
            
            this._parameters = new string[] {address, encodedTypeData};
        }
    }
}