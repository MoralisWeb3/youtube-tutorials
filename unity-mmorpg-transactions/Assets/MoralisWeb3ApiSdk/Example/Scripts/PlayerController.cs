/**
 *           Module: MoralisSessionTokenResponse.cs
 *  Descriptiontion: Sample game script used to control the player's object..
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
using System.Collections;
using UnityEngine;
using TMPro;
using MoralisWeb3ApiSdk;

/// <summary>
/// Sample game script used to control the player's object.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 7f; 
    public float groundCheckDistance = 0.002f;
    public float jumpHeight = 3f;
    public int maxHealth = 25;
    public TMP_Text addressText;
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;

    public HealthBar healthBar;

    private float moveSpeed = 10f; 
    private Vector3 moveDirection;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private bool isGrounded;
    private Animator anim;
    private int currentHealth;
    private Vector2 joystickPosition;
    private bool isMobile;
    private bool isAuthenticated = false;

    private void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        healthBar.SetMaxHealth(maxHealth);

#if UNITY_ANDROID || UNITY_IOS
        isMobile = true;
#endif
    }

    private async void Update()
    {
        // Update character address if it has not been set
        if (!isAuthenticated)
        {
            var user = await MoralisInterface.GetUserAsync();

            if (user != null)
            {
                string addr = user.authData["moralisEth"]["id"].ToString();

                addressText.text = string.Format("{0}...{1}", addr.Substring(0, 6), addr.Substring(addr.Length - 3, 3));

                isAuthenticated = true;
            }
        }

        Move();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(Attack());
        }
    }

    public void JoystickInput(Vector2 vector2)
    {
        joystickPosition = vector2;
    }

    private void Move()
    {
        // Determin if player is already on the ground.
       // isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        //if (isGrounded && velocity.y < 0)
        //{
        //    velocity.y = -2f;
        //}
        // Check for movement using Vertical input. Valid input values are set 
        // in the Unity IDE at Edit->Project Settings->Input Manager
        float moveZ = 0f;

        if (isMobile)
        {
            moveZ = joystickPosition.normalized.y;
        }
        else
        {
            moveZ = Input.GetAxis("Vertical");
        }

        moveDirection = new Vector3(0, 0, moveZ);
        moveDirection = transform.TransformDirection(moveDirection);

        //if (isGrounded)
        //{
            if (isMobile)
            {
                if (moveDirection != Vector3.zero && moveZ > 0.65f)
                {
                    // Walk
                    Run();
                }
                else if (moveDirection != Vector3.zero && moveZ > 0.0f)
                {
                    // Run
                    Walk();
                }
                else
                {
                    // Idle
                    Idle();
                }
            }
            else
            {
                if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
                {
                    // Walk
                    Walk();
                }
                else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
                {
                    // Run
                    Run();
                }
                else
                {
                    // Idle
                    Idle();
                }
            }

            moveDirection *= moveSpeed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        ////}

        controller.Move(moveDirection * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Idle()
    {
        moveSpeed = 0f;
        anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    private void Run()
    {
        moveSpeed = runSpeed;

        anim.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
    }

    private void Walk()
    {
        moveSpeed = walkSpeed;

        anim.SetFloat("Speed", 0.58f, 0.1f, Time.deltaTime);
    }

    private IEnumerator Attack()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("AttackLayer"), 1);
        anim.SetTrigger("ATTACK");

        yield return new WaitForSeconds(0.9f);
        anim.SetLayerWeight(anim.GetLayerIndex("AttackLayer"), 0);
    }

    private void Die()
    {
        anim.SetTrigger("DIE");
    }
}
