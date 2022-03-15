namespace WalletConnectSharp.Core.Models.Ethereum
{
    public class EthChainData : EthChain
    {
        public string[] blockExplorerUrls;
        public string chainName;
        public string[] iconUrls;
        public NativeCurrency nativeCurrency;
        public string[] rpcUrls;
    }
}