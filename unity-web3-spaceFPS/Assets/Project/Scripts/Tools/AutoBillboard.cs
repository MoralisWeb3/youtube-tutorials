using System;
using UnityEngine;

namespace Main
{
    public class AutoBillboard : MonoBehaviour
    {
        private Camera _mainCamera;
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            if (_mainCamera is null) return;
            transform.LookAt(transform.position + _mainCamera.transform.forward);
        }
    }
}

