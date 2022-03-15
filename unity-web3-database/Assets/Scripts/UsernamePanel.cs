using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using MoralisWeb3ApiSdk;

public class UsernamePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameLabel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submitButton;

    private void Start()
    {
        var user = MoralisInterface.GetClient().GetCurrentUser();
        usernameLabel.text = "Welcome Player:<br>" + user.username;
    }

    //This method works on every platform
    public async void SetNewUsername()
    {
        var user = MoralisInterface.GetClient().GetCurrentUser();
        if (user == null) return;
        
        user.username = inputField.text;
        var result = await user.SaveAsync();

        if (result)
        {
            usernameLabel.text = "Welcome Player:<br>" + user.username;
        }
    }

    //Called from InputField
    public void HandleOnValueChanged()
    {
        submitButton.interactable = inputField.text != String.Empty;
    }
}
