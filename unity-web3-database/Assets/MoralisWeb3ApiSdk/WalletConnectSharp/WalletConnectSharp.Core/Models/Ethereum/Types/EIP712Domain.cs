using System;
using WalletConnectSharp.Core.Models.Ethereum.Types;

namespace WalletConnectSharp.Core.Models
{
    public class EIP712Domain
    {
        public string name;
        public string version;
        
        [EvmIgnore]
        public int chainId;
        
        [EvmType("address")]
        public string verifyingContract;

        public EIP712Domain(string name, string version, int chainId, string verifyingContract)
        {
            this.name = name;
            this.version = version;
            this.chainId = chainId;
            this.verifyingContract = verifyingContract;
        }
    }
}