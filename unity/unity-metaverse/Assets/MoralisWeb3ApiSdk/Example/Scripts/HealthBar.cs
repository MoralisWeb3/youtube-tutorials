/**
 *           Module: MoralisSessionTokenResponse.cs
 *  Descriptiontion: Sample game script used to provide a healthbar that 
 *                   changes color based on set health value.
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
using UnityEngine.UI;

/// <summary>
/// Sample game script used to provide a healthbar that changes color based on set health value.
/// </summary>
public class HealthBar : MonoBehaviour
{
    /// <summary>
    /// Adjustable value control.
    /// </summary>
    public Slider slider;
    /// <summary>
    /// Color gradient used to indicate levels of health.
    /// </summary>
    public Gradient gradient;
    /// <summary>
    /// Image that indicates health.
    /// </summary>
    public Image fill;

    /// <summary>
    /// Maximum health value.
    /// </summary>
    /// <param name="health"></param>
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;

        SetHealth(health);
    }

    /// <summary>
    /// Adjust current health value
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(int health)
    {
        // Make sure health is no less than zero for display purposes.
        int adjustedHealth = health >= 0 ? health : 0;

        slider.value = adjustedHealth;

        // Adjust fill color based on health.
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}
