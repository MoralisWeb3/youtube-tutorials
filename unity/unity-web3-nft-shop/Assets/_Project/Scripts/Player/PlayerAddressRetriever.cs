using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class PlayerAddressRetriever : MonoBehaviour
{
    private TextMeshPro _addressLabel;

    private void Awake()
    {
        _addressLabel = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        MoralisAuthenticator.Authenticated += GetPlayerAddress;
    }

    private void OnDisable()
    {
        MoralisAuthenticator.Authenticated -= GetPlayerAddress;
    }
    
    private async void GetPlayerAddress()
    {
        _addressLabel.text = await MoralisTools.GetWalletAddress();

        if (_addressLabel.text is null) return;
        _addressLabel.enabled = true;
    }
}
