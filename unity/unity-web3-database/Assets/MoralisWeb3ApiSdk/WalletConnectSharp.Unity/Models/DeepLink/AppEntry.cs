using UnityEngine;

namespace WalletConnectSharp.Unity.Models
{
    public class AppEntry
    {
        public string id;
        public string name;
        public string homepage;
        public string[] chains;
        public AppInfo app;
        public MobileAppData mobile;
        public MobileAppData desktop;
        public AppMetadata metadata;

        public Sprite largeIcon;
        public Sprite medimumIcon;
        public Sprite smallIcon;
    }
}