namespace WalletConnectSharp.Core.Models.Ethereum
{
    public class TransactionData
    {
        public string from;
        public string to;
        public string data;
        public string gas;
        public string gasPrice;
        public string value;
        public string nonce;
        public int chainId;
    }
}