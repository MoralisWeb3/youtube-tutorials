using System;
using System.Collections.Generic;
using UnityEngine.XR;

namespace WalletConnectSharp.Core.Models
{
    public class SavedSession
    {
        public string ClientID { get; }
        public string BridgeURL { get; }
        public string Key { get; }
        public byte[] KeyRaw { get; }
        public string PeerID { get; }
        public long HandshakeID { get; }
        public int NetworkID { get; }
        public string[] Accounts { get; }
        public int ChainID { get; }
        public ClientMeta DappMeta { get; }
        
        public ClientMeta WalletMeta { get; }

        public SavedSession(string clientID, long handshakeID, string bridgeURL, string key, byte[] keyRaw, string peerID, int networkID, string[] accounts, int chainID, ClientMeta dappMeta, ClientMeta walletMeta)
        {
            ClientID = clientID;
            BridgeURL = bridgeURL;
            Key = key;
            KeyRaw = keyRaw;
            PeerID = peerID;
            NetworkID = networkID;
            Accounts = accounts;
            ChainID = chainID;
            DappMeta = dappMeta;
            WalletMeta = walletMeta;
            HandshakeID = handshakeID;
        }

        private sealed class SavedSessionEqualityComparer : IEqualityComparer<SavedSession>
        {
            public bool Equals(SavedSession x, SavedSession y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.ClientID == y.ClientID && x.BridgeURL == y.BridgeURL && x.Key == y.Key && Equals(x.KeyRaw, y.KeyRaw) && x.PeerID == y.PeerID && x.NetworkID == y.NetworkID && Equals(x.Accounts, y.Accounts) && x.ChainID == y.ChainID && Equals(x.DappMeta, y.DappMeta) && Equals(x.WalletMeta, y.WalletMeta);
            }

            public int GetHashCode(SavedSession obj)
            {
                unchecked
                {
                    var hashCode = (obj.ClientID != null ? obj.ClientID.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.BridgeURL != null ? obj.BridgeURL.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Key != null ? obj.Key.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.KeyRaw != null ? obj.KeyRaw.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.PeerID != null ? obj.PeerID.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.NetworkID;
                    hashCode = (hashCode * 397) ^ (obj.Accounts != null ? obj.Accounts.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.ChainID;
                    hashCode = (hashCode * 397) ^ (obj.DappMeta != null ? obj.DappMeta.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.WalletMeta != null ? obj.WalletMeta.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<SavedSession> SavedSessionComparer { get; } = new SavedSessionEqualityComparer();

        protected bool Equals(SavedSession other)
        {
            return ClientID == other.ClientID && BridgeURL == other.BridgeURL && Key == other.Key && Equals(KeyRaw, other.KeyRaw) && PeerID == other.PeerID && NetworkID == other.NetworkID && Equals(Accounts, other.Accounts) && ChainID == other.ChainID && Equals(DappMeta, other.DappMeta) && Equals(WalletMeta, other.WalletMeta);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SavedSession) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ClientID != null ? ClientID.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BridgeURL != null ? BridgeURL.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (KeyRaw != null ? KeyRaw.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PeerID != null ? PeerID.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ NetworkID;
                hashCode = (hashCode * 397) ^ (Accounts != null ? Accounts.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ChainID;
                hashCode = (hashCode * 397) ^ (DappMeta != null ? DappMeta.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (WalletMeta != null ? WalletMeta.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(SavedSession session, SavedSession other)
        {
            bool isStatNull = object.ReferenceEquals(session, null);
            bool isOtherNull = object.ReferenceEquals(other, null);
        
            return !isOtherNull && !isStatNull && session.Equals(other);
        }
    
        public static bool operator !=(SavedSession session, SavedSession other)
        {
            bool isSessionNull = object.ReferenceEquals(session, null);
            bool isOtherNull = object.ReferenceEquals(other, null);

            return isOtherNull == isSessionNull && !isSessionNull ? !session.Equals(other) : isOtherNull != isSessionNull;
        }
    }
}