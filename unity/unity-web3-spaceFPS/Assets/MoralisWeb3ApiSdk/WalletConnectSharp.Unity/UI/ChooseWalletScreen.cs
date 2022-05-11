using Assets.Scripts;
using Assets.Scripts.WalletConnectSharp.Unity.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Morlais;

namespace WalletConnectSharp.Unity.UI
{
    public class ChooseWalletScreen : MonoBehaviour
    {
        public WalletConnect WalletConnect;
        public GameObject buttonPrefab;
        public Transform buttonGridTransform;
        public Text loadingText;

        [SerializeField]
        public WalletSelectItem[] wallets;

        private void Start()
        {
            StartCoroutine(BuildWalletButtons());
        }

        private IEnumerator BuildWalletButtons()
        {
#if UNITY_IOS
            // Set wallet filter to those wallets selected by the developer.
            IEnumerable<string> walletFilter = from w in wallets
                                               where w.Selected == true
                                               select w.Id;
            // For iOS Set wallet filter to speed up wallet button display.
            if (walletFilter.Count() > 0)
            {
                WalletConnect.AllowedWalletIds = walletFilter.ToList();

                foreach (string i in WalletConnect.AllowedWalletIds) Debug.Log($"Filter for {i}");
            }
            else
            {
                Debug.Log("No wallets selected for filter.");
            }
#endif
            
            yield return WalletConnect.FetchWalletList();

            foreach (var walletId in WalletConnect.SupportedWallets.Keys)
            {
                var walletData = WalletConnect.SupportedWallets[walletId];

                var walletObj = Instantiate(buttonPrefab, buttonGridTransform);

                var walletImage = walletObj.GetComponent<Image>();
                var walletButton = walletObj.GetComponent<Button>();

                walletImage.sprite = walletData.medimumIcon;
                
                walletButton.onClick.AddListener(delegate
                {
                    WalletConnect.OpenDeepLink(walletData);
                });
            }
            
            Destroy(loadingText.gameObject);
        }

        public static List<WalletSelectItem> GetWalletNameList()
        {
            List<WalletSelectItem> result = SupportedWalletList.SupportedWalletNames();

            return result;
        }
    }
}