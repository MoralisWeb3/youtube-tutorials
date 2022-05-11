using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Web3MultiplayerRPG
{
    public class Billboard : MonoBehaviour
    {
        void LateUpdate()
        {
            if (Camera.main is null) return;
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }
    }
}

