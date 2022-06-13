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

    public async void GetPlayerAddress()
    {
        _addressLabel.text = await MoralisTools.GetWalletAddress();

        if (_addressLabel.text is null) return;
        _addressLabel.enabled = true;
    }
}
