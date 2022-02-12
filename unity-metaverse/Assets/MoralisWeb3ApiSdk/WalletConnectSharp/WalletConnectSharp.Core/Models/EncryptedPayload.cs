namespace WalletConnectSharp.Core.Models
{
    public class EncryptedPayload
    {
        public string iv;

        public string hmac;

        public string data;
    }
}