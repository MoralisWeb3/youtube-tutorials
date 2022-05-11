using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;
using WalletConnectSharp.Core.Events;
using WalletConnectSharp.Core.Events.Request;
using WalletConnectSharp.Core.Events.Response;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Network;

namespace WalletConnectSharp.Unity.Network
{
    public class NativeWebSocketTransport : MonoBehaviour, ITransport
    {
        private bool opened = false;
        private bool closed = false;

        private WebSocket nextClient;
        private WebSocket client;
        private EventDelegator _eventDelegator;
        private bool wasPaused;
        private string currentUrl;
        private List<string> subscribedTopics = new List<string>();
        private Queue<NetworkMessage> _queuedMessages = new Queue<NetworkMessage>();

        public bool Connected
        {
            get
            {
                return client != null && client.State == WebSocketState.Open && opened;
            }
        }

        public void AttachEventDelegator(EventDelegator eventDelegator)
        {
            this._eventDelegator = eventDelegator;
        }

        public void Dispose()
        {
            if (client != null)
            {
                client.CancelConnection();
            }
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageReceivedEventArgs> OpenReceived;
        
        public string URL
        {
            get
            {
                return currentUrl;
            }
        }

        public async Task Open(string url, bool clearSubscriptions = true)
        {
            if (currentUrl != url || clearSubscriptions)
            {
                ClearSubscriptions();
            }

            currentUrl = url;
            
            await _socketOpen();
        }

        private async Task _socketOpen()
        {
            if (nextClient != null)
            {
                return;
            }

            string url = currentUrl;
            if (url.StartsWith("https"))
                url = url.Replace("https", "wss");
            else if (url.StartsWith("http"))
                url = url.Replace("http", "ws");

            nextClient = new WebSocket(url);
            
            TaskCompletionSource<bool> eventCompleted = new TaskCompletionSource<bool>(TaskCreationOptions.None);

            nextClient.OnOpen += () =>
            {
                CompleteOpen();
                
                // subscribe now
                if (this.OpenReceived != null)
                    OpenReceived(this, null);

                Debug.Log("[WebSocket] Opened " + url);
                
                eventCompleted.SetResult(true);
            };

            nextClient.OnMessage += OnMessageReceived;
#if !UNITY_EDITOR
            nextClient.OnClose += ClientTryReconnect;
#endif
            nextClient.OnError += (e) => {

                Debug.Log("[WebSocket] OnError " + e);
                HandleError(new Exception(e));
            };

            nextClient.Connect().ContinueWith(t => HandleError(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

            Debug.Log("[WebSocket] Waiting for Open " + url);
            await eventCompleted.Task;
            Debug.Log("[WebSocket] Open Completed");
        }

        private void HandleError(Exception e)
        {
            Debug.LogError(e);
        }

        private async void CompleteOpen()
        {
            await Close();
            this.client = this.nextClient;
            this.nextClient = null;
            QueueSubscriptions();
            opened = true;
            FlushQueue();
        }

        private async void FlushQueue()
        {
            Debug.Log("[WebSocket] Flushing Queue");
            Debug.Log("[WebSocket] Queue Count: " + _queuedMessages.Count);
            while (_queuedMessages.Count > 0)
            {
                var msg = _queuedMessages.Dequeue();
                await SendMessage(msg);
            }

            Debug.Log("[WebSocket] Queue Flushed");
        }

        private void QueueSubscriptions()
        {
            foreach (var topic in subscribedTopics)
            {
                this._queuedMessages.Enqueue(GenerateSubscribeMessage(topic));
            }

            Debug.Log("[WebSocket] Queued " + subscribedTopics.Count + " subscriptions");
        }
        
        private async void ClientTryReconnect(WebSocketCloseCode closeCode)
        {
            if (wasPaused)
            {
                Debug.Log("[WebSocket] Application paused, retry attempt aborted");
                return;
            }
            
            nextClient = null;
            await _socketOpen();
        }

        public void CancelConnection()
        {
            client.CancelConnection();
        }


        private async void OnMessageReceived(byte[] bytes)
        {
            string json = System.Text.Encoding.UTF8.GetString(bytes);

            try
            {
                var msg = JsonConvert.DeserializeObject<NetworkMessage>(json);

                
                await SendMessage(new NetworkMessage()
                {
                    Payload = "",
                    Type = "ack",
                    Silent = true,
                    Topic = msg.Topic
                });

                if (this.MessageReceived != null)
                    MessageReceived(this, new MessageReceivedEventArgs(msg, this));
            }
            catch(Exception e)
            {
                Debug.Log("[WebSocket] Exception " + e.Message);
            }   
        }
        
        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (client != null && client.State == WebSocketState.Open)
            {
                client.DispatchMessageQueue();
            }
#endif
        }

        public async Task Close()
        {
            Debug.Log("Closing Websocket");
            try
            {
                if (client != null)
                {
                    this.opened = false;
                    client.OnClose -= ClientTryReconnect;
                    await client.Close();
                }
            }
            catch (WebSocketInvalidStateException e)
            {
                if (e.Message.Contains("WebSocket is not connected"))
                    Debug.LogWarning("Tried to close a websocket when it's already closed");
                else
                    throw;
            }
        }
        
        public async Task SendMessage(NetworkMessage message)
        {
            if (!Connected)
            {
                _queuedMessages.Enqueue(message);
                await _socketOpen();
            }
            else
            {
                string finalJson = JsonConvert.SerializeObject(message);

                await this.client.SendText(finalJson);
            }
        }

        public async Task Subscribe(string topic)
        {
            Debug.Log("[WebSocket] Subscribe to " + topic);

            var msg = GenerateSubscribeMessage(topic);
            
            await SendMessage(msg);

            if (!subscribedTopics.Contains(topic))
            {
                subscribedTopics.Add(topic);
            }

            opened = true;
        }

        private NetworkMessage GenerateSubscribeMessage(string topic)
        {
            return new NetworkMessage()
            {
                Payload = "",
                Type = "sub",
                Silent = true,
                Topic = topic
            };
        }

        public async Task Subscribe<T>(string topic, EventHandler<JsonRpcResponseEvent<T>> callback) where T : JsonRpcResponse
        {
            await Subscribe(topic);

            _eventDelegator.ListenFor(topic, callback);
        }
        
        public async Task Subscribe<T>(string topic, EventHandler<JsonRpcRequestEvent<T>> callback) where T : JsonRpcRequest
        {
            await Subscribe(topic);

            _eventDelegator.ListenFor(topic, callback);
        }

        public void ClearSubscriptions()
        {
            Debug.Log("[WebSocket] Subs Cleared");
            subscribedTopics.Clear();
            _queuedMessages.Clear();
        }

        //#if UNITY_IOS
        private IEnumerator OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Debug.Log("[WebSocket] Pausing");
                wasPaused = true;
                
                //We need to close the Websocket Properly
                var closeTask = Task.Run(Close);
                var coroutineInstruction = new WaitForTask(closeTask);
                yield return coroutineInstruction;
            }
            else if (wasPaused)
            {
                Debug.Log("[WebSocket] Resuming");
                var openTask = Task.Run(() => Open(currentUrl, false));
                var coroutineInstruction = new WaitForTask(openTask);
                yield return coroutineInstruction;

                foreach (var topic in subscribedTopics)
                {
                    var subTask = Task.Run(() => Subscribe(topic));
                    var coroutineSubInstruction = new WaitForTask(subTask);
                    yield return coroutineSubInstruction;
                }
            }
        }
        //#endif
    }
}