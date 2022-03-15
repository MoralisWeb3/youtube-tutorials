using WalletConnectSharp.Core.Models.Ethereum;

namespace WalletConnectSharp.Core
{
    public static class Chains
    {
        public static EthChain EthereumChainId = new EthChain()
        {
            chainId = "0x1"
        };
        
        public static EthChain ExpanseChainId = new EthChain()
        {
            chainId = "0x2"
        };
        
        public static EthChain RopstenChainId = new EthChain()
        {
            chainId = "0x3"
        };
        
        public static EthChain RinkebyChainId = new EthChain()
        {
            chainId = "0x4"
        };
        
        public static EthChain GörliChainId = new EthChain()
        {
            chainId = "0x5"
        };
        
        public static EthChain KottiChainId = new EthChain()
        {
            chainId = "0x6"
        };
    }
}