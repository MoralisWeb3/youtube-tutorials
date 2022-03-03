/**
 *           Module: MoralisSessionTokenResponse.cs
 *  Descriptiontion: Sample game script used to handle damage to a chest object
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
using Assets.MoralisWeb3ApiSdk.Example.Scripts;
using UnityEngine;

/// <summary>
/// Sample game script used to handle damage to a chest object
/// </summary>
public class ChestController : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 3;
    public GameObject[] treasures;
    public float raiseTreasure = 0.15f;
    private int currentHealth;
    private Animator anim;
    private bool open = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        anim = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Called when player strikes chest with sword.
    /// </summary>
    /// <param name="damage"></param>
    public void InflictDamage(int damage)
    {
        // If already open, ignore further hits.
        if (open) return;
        // Adjust health
        currentHealth -= damage;
        // Adjust healtbar
        healthBar.SetHealth(currentHealth);
        // If chest beaten, open it.
        if (currentHealth <= 0)
        {
            Open();
        }
    }

    private void Open()
    {
        // Run open animation
        anim.SetTrigger("OPEN");
        // Capture that chest is open
        open = true;

        // Raise treasures to float above the chest.
        foreach (GameObject t in treasures)
        {
            AwardableController ac = null;

            t.TryGetComponent<AwardableController>(out ac);

            if (ac != null)
            {
                ac.Display(new Vector3(0, raiseTreasure, 0));
                ac.SetCanBeClaimed();
            }
        }
    }
}
