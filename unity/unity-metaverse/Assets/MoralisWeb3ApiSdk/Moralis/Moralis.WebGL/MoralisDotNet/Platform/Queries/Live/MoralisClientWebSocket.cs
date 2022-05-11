#if UNITY_WEBGL
using Moralis.WebGL.Platform.Abstractions;
using System;
using System.Collections.Generic;
using Moralis.Network.Client;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Queries.Live
{
    public class MoralisClientWebSocket : IClientWebSocket
    {
        private WebSocket webSocket = null;
        public object Mutex = '\0';
        public Queue<byte[]> MessageQueue;

        public MoralisClientWebSocket(string uri, Dictionary<string, string> headers = null)
        {
            webSocket = new WebSocket(uri, headers);
            MessageQueue = new Queue<byte[]>();
            webSocket.OnMessage += OnReceiveMessage;
        }

        public WebSocketCloseCode CloseStatus { get; private set; }

        public string CloseReasonsDescription { get; private set; }

        public WebSocketState State { get { return webSocket.State; } }
        
        public async UniTask CloseAsync(WebSocketCloseCode closeCode, string reason)
        {
            this.CloseStatus = closeCode;
            this.CloseReasonsDescription = reason;
            await webSocket.Close(closeCode);
        }
       
        public async UniTask ConnectAsync()
        {
            await webSocket.Connect();
        }

        public async UniTask SendAsync(byte[] buffer)
        {
            if (!WebSocketState.Open.Equals(webSocket.State))
                await ConnectAsync();

            await webSocket.Send(buffer);
        }

        public async UniTask SendTextAsync(string msg)
        {
            if (!WebSocketState.Open.Equals(webSocket.State))
                await ConnectAsync();

            await webSocket.SendText(msg);
        }

        public void Abort()
        {
            webSocket.CancelConnection();
        }

        public void Dispose()
        {
            Abort();
        }

        private void OnReceiveMessage(byte[] data)
        {
            lock (Mutex)
            {
                MessageQueue.Enqueue(data);
            }
        }
    }
}
#endif