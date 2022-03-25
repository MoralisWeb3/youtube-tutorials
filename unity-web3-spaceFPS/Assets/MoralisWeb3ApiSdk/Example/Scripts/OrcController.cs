/**
 *           Module: MoralisSessionTokenResponse.cs
 *  Descriptiontion: Sample game script used to control an Orc NPC.
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
using UnityEngine;

/// <summary>
/// Sample game script used to control an Orc NPC.
/// </summary>
public class OrcController : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 25;
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;

    private Animator anim;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        healthBar.SetMaxHealth(maxHealth);

    }

    /// <summary>
    /// Called when player strikes chest with sword.
    /// </summary>
    /// <param name="damage"></param>
    public void InflictDamage(int damage)
    {
        // Adjust health
        currentHealth -= damage;

        if (currentHealth < 0) currentHealth = 0;
        // Adjust healthbar.
        healthBar.SetHealth(currentHealth);
        // If health is 0 or less, run Die animation.
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handle Orc is dead.
    /// </summary>
    private void Die()
    {
        // Die action here.
        anim.SetTrigger("DIE");
    }
}
