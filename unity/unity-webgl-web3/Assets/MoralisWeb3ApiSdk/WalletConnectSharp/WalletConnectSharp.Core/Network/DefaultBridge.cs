using System;
using System.Linq;

namespace WalletConnectSharp.Core.Network
{
    public static class DefaultBridge
    {
        public static string Domain = "walletconnect.org";
        
        public static string MainBridge = "https://bridge.walletconnect.org";

        public const string AlphaNumeric = "abcdefghijklmnopqrstuvwxyz0123456789";

        public static readonly string[] Bridges =
            AlphaNumeric.Select(c => "https://" + c + ".bridge.walletconnect.org").ToArray();

        private static string ExtractHostname(string url)
        {
            var hostname = url.IndexOf("//") > -1 ? url.Split('/')[2] : url.Split('/')[0];
            hostname = hostname.Split(':')[0];
            hostname = hostname.Split('?')[0];

            return hostname;
        }

        private static string ExtractRootDomain(string url)
        {
            var items = ExtractHostname(url).Split('.').ToArray();
            return string.Join(".", items.Skip(items.Length - 2));
        }

        private static string[] _bridgeCache = null;

        public static string[] AlternateBridges
        {
            get
            {
                return Bridges;
            }
        }

        public static string[] AllBridges
        {
            get
            {
                return _bridgeCache ??
                       (_bridgeCache = Enumerable.Empty<string>().Append(MainBridge).Concat(Bridges).ToArray());
            }
        }

        public static string ChooseRandomBridge(string[] possibleBridges = null)
        {
            if (possibleBridges == null)
            {
                possibleBridges = AllBridges;
            }

            var random = new Random();
            return possibleBridges[random.Next(possibleBridges.Length)];
        }

        public static string GetBridgeUrl(string url)
        {
            if (ExtractRootDomain(url) == Domain)
                return ChooseRandomBridge();
            return url;
        }
    }
}