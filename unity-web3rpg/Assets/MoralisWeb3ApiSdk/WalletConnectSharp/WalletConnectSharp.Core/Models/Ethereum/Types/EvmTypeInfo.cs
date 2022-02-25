namespace WalletConnectSharp.Core.Models.Ethereum.Types
{
    public class EvmTypeInfo
    {
        public string name;
        public string type;

        public EvmTypeInfo(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }
}