#if UNITY_WEBGL
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Moralis.WebGL.Models;
using Moralis.WebGL.Platform.Queries.Live;

namespace Moralis.WebGL
{


    public class MoralisLiveQueriesGL
    {
        private static bool isConnected;

        [DllImport("__Internal")]
        private static extern void OpenWebsocketJs(string key, string path);

        [DllImport("__Internal")]
        private static extern void CloseWebsocketJs(string key);

        [DllImport("__Internal")]
        private static extern void SendMessageJs(string key, string message);

        [DllImport("__Internal")]
        private static extern string OpenWebsocketResponse();

        [DllImport("__Internal")]
        private static extern string CloseWebsocketResponse();

        [DllImport("__Internal")]
        private static extern string GetErrorQueueJs(string key);

        [DllImport("__Internal")]
        private static extern string GetResponseQueueJs(string key);

        [DllImport("__Internal")]
        private static extern int GetSocketStateJs(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static UniTask<bool> CreateSubscription(string key, string path)
        {
            string openResp = String.Empty;
            bool resp = false;
            int waitLoops = 240; // approx 2min @ 60FPS
            int loop = 0;

            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Key and Path requried to establish a websocket connection.");
            }

            OpenWebsocketJs(key, path);
            Debug.Log("Going into wait loop ...");
            while (loop < waitLoops && String.IsNullOrEmpty(openResp))
            {
                await UniTask.DelayFrame(30);

                var r = OpenWebsocketResponse();

                if (r != null)
                {
                    openResp = r;
                    Debug.Log($"Create Subscription response: {openResp}");
                    break;
                }
                else Debug.Log("response is null!!!");
                loop++;
            }

            Debug.Log($"Exiting wait loop resp is {openResp}");
            
            if (openResp.Length >  0 && openResp.Contains("\"isTrusted\":true"))
            {
                isConnected = true;
                resp = true;
            }

            return resp;
        }

        /// <summary>
        /// Indicates if the web3 objects has been created and connected properly.
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsConnected() => isConnected;

        public async static UniTask<string> CloseWebsocket(string key)
        {
            string closeResp = String.Empty;
            bool resp = false;
            int waitLoops = 240; 
            int loop = 0;

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key requried to close a websocket connection.");
            }

            CloseWebsocketJs(key);

            while (loop < waitLoops && closeResp.Length < 1)
            {
                await UniTask.DelayFrame(30);

                closeResp = CloseWebsocketResponse();

                loop++;
            }

            if (closeResp.Length > 0)
            {
                isConnected = false;
            }

            return closeResp;
        }

        public static void SendMessage(string key, string message)
        {
            SendMessageJs(key, message);
        }

        public static string GetResponseQueue(string key)
        {
            string val = GetResponseQueueJs(key);
            return val;
        }

        public static string GetErrorQueue(string key)
        {
            return GetErrorQueueJs(key);
        }

        public static int GetSocketState(string key)
        {
            return GetSocketStateJs(key);
        }
    }
}
#endif