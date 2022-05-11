using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Web3MultiplayerRPG
{
    [RequireComponent(typeof(TextMeshPro))]
    public class DisplayUsername : MonoBehaviour
    {
        [SerializeField] private PhotonView playerPV;
        private TextMeshPro _usernameLabel;

        private void Awake()
        {
            _usernameLabel = GetComponent<TextMeshPro>();
        }

        private void OnEnable()
        {
            _usernameLabel.text = playerPV.Owner.NickName;
        }
    }
}

