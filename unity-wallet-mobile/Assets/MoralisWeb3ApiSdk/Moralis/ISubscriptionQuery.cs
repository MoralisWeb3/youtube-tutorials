/**
 *           Module: ISubscriptionQuery.cs
 *  Descriptiontion: Interface describing an object that wraps subscription 
 *                   processes to facilitate automationof pause / unpause 
 *                   handling.
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
#if UNITY_WEBGL
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif

namespace Assets.Scripts
{
    public interface ISubscriptionQuery
    {
        /// <summary>
        /// Indicates if a connection the server was established.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Indicates if a subscription for the query has been established.
        /// </summary>
        bool Subscribed { get; }

        /// <summary>
        /// Key name used to identify this subscription and included in any 
        /// error logs generated.
        /// </summary>
        string SubscriptionName { get; }

#if UNITY_WEBGL
        /// <summary>
        /// Attempts to re-establish a previous subscriptions. If the 
        /// subscription is already active, unsubscribe is called.
        /// Subscription is then re-created.
        /// </summary>
        /// <returns></returns>
        UniTask RenewSubscription();

        /// <summary>
        /// Unsubscribes from and disposes of the subscription.
        /// </summary>
        /// <returns></returns>
        UniTask Unsubscribe();
#else

        /// <summary>
        /// Attempts to re-establish a previous subscriptions. If the 
        /// subscription is already active, unsubscribe is called.
        /// Subscription is then re-created.
        /// </summary>
        /// <returns></returns>
        Task RenewSubscription();

        /// <summary>
        /// Unsubscribes from and disposes of the subscription.
        /// </summary>
        /// <returns></returns>
        Task Unsubscribe();
#endif
    }
}
