/**
 *           Module: MoralisSessionTokenResponse.cs
 *  Descriptiontion: Sample script showing how to retrieve user and balance 
 *                   information using Moralis Web3API and displaying those 
 *                   items in text elements.
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
using MoralisWeb3ApiSdk;

#if UNITY_WEBGL
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Web3Api.Models;
#else
using Moralis.Platform.Objects;
using Moralis.Web3Api.Models;
#endif
/// <summary>
/// Sample script showing how to retrieve user and balance information using 
/// Moralis Web3API and displaying those items in text elements.
/// NOTE: This could probably be combined with the TokenListController script since
///       the information is now combined in a single view.
/// </summary>
public class WalletBalanceController : MonoBehaviour
{
    /// <summary>
    /// Text used to display the account address
    /// </summary>
    public Text addressText;

    /// <summary>
    /// Text to display the native balance
    /// </summary>
    public Text balanceText;

    /// <summary>
    /// Chain ID to fetch tokens from. Might be better to make this
    /// a drop down that is selectable at run time.
    /// </summary>
    public int ChainId;

    // Update is called once per frame
    public async void PopulateBalanceValues()
    {
        // Get user object and display user name
        MoralisUser user = await MoralisInterface.GetUserAsync();

        if (user != null)
        {
            // Get user address from user auth data.
            string addr = user.authData["moralisEth"]["id"].ToString();

            addressText.text = addr;

#if UNITY_WEBGL
            // Retrieve account balanace.
            NativeBalance bal =
                await MoralisInterface.GetClient().Web3Api.Account.GetNativeBalance(addr.ToLower(),
                                            (ChainList)ChainId);
#else
            // Retrieve account balanace.
            NativeBalance bal =
                await MoralisInterface.GetClient().Web3Api.Account.GetNativeBalance(addr.ToLower(),
                                            (ChainList)ChainId);
#endif
            double balance = 0.0;
            
            // Make sure a response to the balanace request weas received. The 
            // IsNullOrWhitespace check may not be necessary ...
            if (bal != null && !string.IsNullOrWhiteSpace(bal.Balance))
            {
                double.TryParse(bal.Balance, out balance);
            }

            // Display native token amount (ETH) in fractions of ETH.
            // NOTE: May be better to link this to chain since some tokens may have
            // more than 18 sigjnificant figures.
            balanceText.text = string.Format("{0:0.##} ETH", balance / (double)Mathf.Pow(10.0f, 18.0f));
        }
        else
        {
            balanceText.text = "0";
        }
    }
}
