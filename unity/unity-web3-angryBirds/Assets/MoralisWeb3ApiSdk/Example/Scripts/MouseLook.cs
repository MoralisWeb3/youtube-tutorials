/**
 *           Module: MoralisSessionTokenResponse.cs
 *  Descriptiontion: Sample game script used to turn field of view.
 *           Author: Moralis Web3 Technology AB, 559307-5988 - David B. Goodrich
 *  
 *  MIT License
 *  
 *  Copyright (c) 2021 Moralis Web3 Technology AB, 559307-5988
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */
using MoralisWeb3ApiSdk;
using System;
using UnityEngine;

/// <summary>
/// Sample game script used to turn field of view.
/// </summary>
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float xRotation = 0f;
    public float yRotation = 0f;
    public Transform playerBody;
    private Vector2 joystickPosition;
    private bool isMobile;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = transform.parent;
        Cursor.lockState = CursorLockMode.Confined;

#if UNITY_ANDROID || UNITY_IOS
        isMobile = true;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (MoralisInterface.IsLoggedIn())
        {
            Rotate();
        }
    }

    public void JoystickInput(Vector2 vector2)
    {
        joystickPosition = vector2;

        // Set a minimum that must be met so that upa and down
        // movement in the joystick does not set the character to 
        // continue turning when the desire is to move forward or 
        // backward.
        if (Math.Abs(joystickPosition.normalized.x) < 0.4f)
        {
            joystickPosition = Vector2.zero;
        }
    }

    private void Rotate()
    {
        float mouseX = 0f;

        if (isMobile)
        {
            mouseX = joystickPosition.normalized.x * mouseSensitivity * Time.deltaTime;
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        }

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
