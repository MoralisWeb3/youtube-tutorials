/**
 *           Module: MobileLogin.cs
 *  Descriptiontion: Class that enables use of Moralis Connector for suthentication.
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
using System;
using Newtonsoft.Json;
using UnityEngine;
using Assets.Scripts.Moralis;
using System.Net.Http;
using System.Net.Http.Headers;
using MoralisWeb3ApiSdk;

#if UNITY_WEBGL

using Cysharp.Threading.Tasks;
using Moralis.WebGL.Platform.Objects;

namespace Assets.Scripts
{
    public class MobileLogin
    {
        private static string TOKEN_REQUEST_URL = "server/requestLoginToken";
        private static string REMOTE_SESSION_URL = "server/getRemoteSession?login_token={0}&_ApplicationId={1}";

        public static async UniTask<MoralisUser> LogIn(string moralisServerUrl, string applicationId)
        {
            MoralisUser user = null;
            MoralisLoginTokenResponse tokenResponse = await RequestLoginToken(moralisServerUrl, applicationId);

            if (tokenResponse != null)
            {
                // Display the connector page.
                Application.OpenURL(tokenResponse.url);

                DateTime timeout = DateTime.Now.AddSeconds(120);

                while (true && DateTime.Now < timeout)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(500), ignoreTimeScale: false);

                    MoralisSessionTokenResponse sessionResponse = await CheckSessionResult(moralisServerUrl, tokenResponse.loginToken, applicationId);

                    if (sessionResponse != null && !String.IsNullOrWhiteSpace(sessionResponse.sessionToken))
                    {
                        user = await MoralisInterface.GetClient().UserFromSession(sessionResponse.sessionToken);

                        break;
                    }
                }
            }

            return user;
        }

        private async static UniTask<MoralisSessionTokenResponse> CheckSessionResult(string moralisServerUrl, string tokenId, string applicationId)
        {
            MoralisSessionTokenResponse result = null;

            MoralisLoginTokenRequest payload = new MoralisLoginTokenRequest()
            {
                _ApplicationId = applicationId
            };

            string data = JsonConvert.SerializeObject(payload);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(moralisServerUrl);

                HttpResponseMessage response = client.GetAsync(String.Format(REMOTE_SESSION_URL, tokenId, applicationId)).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body. 
                    string responseBody = await response.Content.ReadAsStringAsync();

                    Debug.Log($"Session Result: {responseBody}");

                    result = JsonConvert.DeserializeObject<MoralisSessionTokenResponse>(responseBody);
                }
            }

            return result;
        }

        private async static UniTask<MoralisLoginTokenResponse> RequestLoginToken(string moralisServerUrl, string applicationId)
        {
            MoralisLoginTokenResponse result = null;

            MoralisLoginTokenRequest payload = new MoralisLoginTokenRequest()
            {
                _ApplicationId = applicationId
            };

            string data = JsonConvert.SerializeObject(payload);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(moralisServerUrl);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                StringContent content = new StringContent(data);
                // List data response.
                HttpResponseMessage response = client.PostAsync(TOKEN_REQUEST_URL, content).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body. 
                    string responseBody = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<MoralisLoginTokenResponse>(responseBody);
                }
            }
            
            return result;
        }
    }
}

#else
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Objects;

namespace Assets.Scripts
{
    public class MobileLogin
    {
        private static string TOKEN_REQUEST_URL = "server/requestLoginToken";
        private static string REMOTE_SESSION_URL = "server/getRemoteSession?login_token={0}&_ApplicationId={1}";

        public static async Task<MoralisUser> LogIn(string moralisServerUrl, string applicationId)
        {
            MoralisUser user = null;
            MoralisLoginTokenResponse tokenResponse = await RequestLoginToken(moralisServerUrl, applicationId);

            if (tokenResponse != null)
            {
                // Display the connector page.
                Application.OpenURL(tokenResponse.url);

                DateTime timeout = DateTime.Now.AddSeconds(120);

                while (true && DateTime.Now < timeout)
                {
                    Thread.Sleep(500);

                    MoralisSessionTokenResponse sessionResponse = await CheckSessionResult(moralisServerUrl, tokenResponse.loginToken, applicationId);

                    if (sessionResponse != null && !String.IsNullOrWhiteSpace(sessionResponse.sessionToken))
                    {
                        user = await MoralisInterface.GetClient().UserFromSession(sessionResponse.sessionToken);

                        break;
                    }
                }
            }

            return user;
        }

        private async static Task<MoralisSessionTokenResponse> CheckSessionResult(string moralisServerUrl, string tokenId, string applicationId)
        {
            MoralisSessionTokenResponse result = null;

            MoralisLoginTokenRequest payload = new MoralisLoginTokenRequest()
            {
                _ApplicationId = applicationId
            };

            string data = JsonConvert.SerializeObject(payload);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(moralisServerUrl);

                HttpResponseMessage response = client.GetAsync(String.Format(REMOTE_SESSION_URL, tokenId, applicationId)).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body. 
                    string responseBody = await response.Content.ReadAsStringAsync();

                    Debug.Log($"Session Result: {responseBody}");

                    result = JsonConvert.DeserializeObject<MoralisSessionTokenResponse>(responseBody);
                }
            }

            return result;
        }

        private async static Task<MoralisLoginTokenResponse> RequestLoginToken(string moralisServerUrl, string applicationId)
        {
            MoralisLoginTokenResponse result = null;

            MoralisLoginTokenRequest payload = new MoralisLoginTokenRequest()
            {
                _ApplicationId = applicationId
            };

            string data = JsonConvert.SerializeObject(payload);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(moralisServerUrl);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                StringContent content = new StringContent(data);
                // List data response.
                HttpResponseMessage response = client.PostAsync(TOKEN_REQUEST_URL, content).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body. 
                    string responseBody = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<MoralisLoginTokenResponse>(responseBody);
                }
            }

            return result;
        }
    }
}
#endif
