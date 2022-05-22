using System;
using TMPro;
using UnityEngine;

namespace Web3MoriaGates
{
    public class WalletAddressRetriever : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI addressLabel;

        private void OnEnable()
        {
            Retrieve();
        }

        private void OnDisable()
        {
            addressLabel.text = string.Empty;
        }

        private async void Retrieve()
        {
            if (addressLabel is null)
            {
                Debug.LogError("Component addressLabel is null");
                return;
            }
            
            addressLabel.text = await MoralisTools.GetWalletAddress() ?? "Address not retrieved";
        }
    }   
}
