using System;
using System.Net;
using System.Threading.Tasks;
using WalletConnectSharp.Core.Events;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Network;
using WalletConnectSharp.Core.Utils;

namespace WalletConnectSharp.Core
{
    public class WalletConnectProvider : WalletConnectProtocol
    {
        private string _handshakeTopic;
        
        private long _handshakeId;
        
        public event EventHandler<WalletConnectProtocol> OnProviderConnect;

        public int? NetworkId { get; private set; }
        
        public string[] Accounts { get; private set; }

        public int ChainId { get; private set; }

        public ClientMeta ClientMetadata { get; set; }
        
        private string clientId = "";

        public string URI
        {
            get;
            private set;
        }
        
        public WalletConnectProvider(SavedSession savedSession, ITransport transport = null, ICipher cipher = null, EventDelegator eventDelegator = null) : base(savedSession, transport, cipher, eventDelegator)
        {
            this.ClientMetadata = savedSession.DappMeta;
            this.WalletMetadata = savedSession.WalletMeta;
            this.ChainId = savedSession.ChainID;
            
            clientId = savedSession.ClientID;
            
            this.Accounts = savedSession.Accounts;
                        
            this.NetworkId = savedSession.NetworkID;
        }

        public WalletConnectProvider(string url, ITransport transport = null, ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null) : base(transport, cipher, eventDelegator)
        {
            this.ChainId = chainId;
            this.URI = url;
            
            this.ParseUrl();
        }

        private void ParseUrl()
        {
            /*
             *  var topicEncode = WebUtility.UrlEncode(_handshakeTopic);
                var versionEncode = WebUtility.UrlEncode(Version);
                var bridgeUrlEncode = WebUtility.UrlEncode(_bridgeUrl);
                var keyEncoded = WebUtility.UrlEncode(_key);

                return "wc:" + topicEncode + "@" + versionEncode + "?bridge=" + bridgeUrlEncode + "&key=" + keyEncoded;
             */

            if (!this.URI.StartsWith("wc"))
                return;
            
            //TODO Figure out a better way to parse this
            
            // topicEncode + "@" + versionEncode + "?bridge=" + bridgeUrlEncode + "&key=" + keyEncoded
            var data = this.URI.Split(':')[0];
            
            _handshakeTopic = WebUtility.UrlDecode(data.Split('@')[0]);
            
            // versionEncode + "?bridge=" + bridgeUrlEncode + "&key=" + keyEncoded
            data = data.Split('@')[1];

            Version = WebUtility.UrlDecode(data.Split('?')[0]);

            //bridge=" + bridgeUrlEncode + "&key=" + keyEncoded
            data = data.Split('?')[1];
            
            

            var parameters = data.Split('&');

            foreach (var parm in parameters)
            {
                var parts = parm.Split('=');
                var name = parts[0];
                var value = parts[1];

                switch (name.ToLower())
                {
                    case "bridge":
                        base._bridgeUrl = WebUtility.UrlDecode(value);
                        break;
                    case "key":
                        base._key = WebUtility.UrlDecode(value);
                        base._keyRaw = base._key.FromHex();
                        break;
                }
            }
        }
    }
}