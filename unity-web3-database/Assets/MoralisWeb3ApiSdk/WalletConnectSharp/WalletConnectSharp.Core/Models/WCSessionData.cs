namespace WalletConnectSharp.Core.Models
{
    public class WCSessionData
    {
        public string peerId;
        public ClientMeta peerMeta;
        public bool approved;
        public int? chainId;
        public int? networkId;
        public string[] accounts;
    }
}