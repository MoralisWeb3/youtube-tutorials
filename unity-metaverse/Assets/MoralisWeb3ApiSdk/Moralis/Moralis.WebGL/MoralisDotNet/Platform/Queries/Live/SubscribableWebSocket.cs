#if UNITY_WEBGL
using System;
using System.Text;
using System.Threading;
using UnityEngine;
using Moralis.WebGL.Platform.Exceptions;
using static Moralis.WebGL.Platform.Exceptions.MoralisFailureException;
using System.Collections.Generic;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.Network.Client;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Queries.Live
{
    public class SubscribableWebSocket : MonoBehaviour
    {
        private byte[] subscriptionRequest = null;
        private bool connectionSent = false;
        private bool subscriptionSent = false;
        private bool unsubscribeSent = false;
        private MoralisClientWebSocket liveWebSocket = null;
        private bool receiving = false;

        /// <summary>
        /// Event raised when a message is received from the server.
        /// </summary>
        public event LiveQueryMessageHandler OnEventMessage;

        public event LiveQueryGeneralMessageHandler OnGeneralMessage;

        /// <summary>
        /// Indicates the current status of this client.
        /// </summary>
        public LiveQueryClientStatusTypes ClientStatus { get; set; }

        /// <summary>
        /// Server connection settings.
        /// </summary>
        public IServerConnectionData ConncetionData { get; set; }

        /// <summary>
        /// Provides Serialization / Deserialization services.
        /// </summary>
        public IJsonSerializer JsonSerializer { get; set; }

        public string InstallationId { get; set; }

        /// <summary>
        /// Request Id used for this subscription
        /// </summary>
        public int RequestId { get; private set; }

        /// <summary>
        /// User session token.
        /// </summary>
        public string SessionToken { get; set; }

        private IClientWebSocket webSocket;

        public SubscribableWebSocket(byte[] subRequest, IServerConnectionData connectionData, int requestId, string installationId, string sessionToken, IJsonSerializer jsonSerializer )
        {
            subscriptionRequest = subRequest;
            ConncetionData = connectionData;
            InstallationId = installationId;
            RequestId = requestId;
            SessionToken = sessionToken;
            JsonSerializer = jsonSerializer;
        }

        /// <summary>
        /// Creates a live query subscription. If established, listens until either 
        /// the app or the server closes the connection.
        /// </summary>
        public async UniTask Subscribe()
        {
            if (String.IsNullOrWhiteSpace(ConncetionData.LiveQueryServerURI))
            {
                throw new MoralisFailureException(ErrorCode.ServerUrlNullOrEmtpy, "");
            }

            ClientStatus = LiveQueryClientStatusTypes.New;
            List<byte> messageBytes = new List<byte>();

            if (liveWebSocket == null)
            {
                new MoralisClientWebSocket(ConncetionData.LiveQueryServerURI);
            }
            
            await liveWebSocket.ConnectAsync();
                
            Console.WriteLine("Live Query WebSocket connected.");
        }

        private async void Update()
        {
            byte[] buffer = new byte[256];
            bool msgSent = false;

            // Establish a connection and listen and process messages until closed.
            if (liveWebSocket != null &&
                WebSocketState.Open.Equals(liveWebSocket.State) && 
                !LiveQueryClientStatusTypes.Closed.Equals(ClientStatus))
            {
                if (!msgSent && !LiveQueryClientStatusTypes.Open.Equals(ClientStatus))
                {
                    // If in new status create and send a connection request.
                    if (!connectionSent && LiveQueryClientStatusTypes.New.Equals(ClientStatus))
                    {
                        SendGeneralMessage("Sending connection request.");
                        byte[] bytes = CreateConnectRequest();
                        await liveWebSocket.SendAsync(bytes);
                        connectionSent = true;
                        msgSent = true;
                    }
                    // If in openning status, create and send a subscription request.
                    else if (!subscriptionSent && LiveQueryClientStatusTypes.Opening.Equals(ClientStatus))
                    {
                        SendGeneralMessage("Sending subscription request.");
                        await liveWebSocket.SendAsync(subscriptionRequest);
                        subscriptionSent = true;
                        msgSent = true;
                    }
                    // If in closing status, create and send an unsubscribe request.
                    else if (!unsubscribeSent && LiveQueryClientStatusTypes.Closing.Equals(ClientStatus))
                    {
                        SendGeneralMessage("Sending unsubscribe request.");
                        byte[] bytes = CreateUnsubscribeRequest();
                        await liveWebSocket.SendAsync(bytes); 
                        unsubscribeSent = true;
                        msgSent = true;
                    }
                }

                // Only listen if the client is not processing the response for 
                // a request (such as connect, subscribe, unsubscribe).
                if (liveWebSocket.MessageQueue.Count > 0 && 
                    (msgSent || LiveQueryClientStatusTypes.Open.Equals(ClientStatus)))
                {
                    lock (liveWebSocket.Mutex)
                    {
                        while (liveWebSocket.MessageQueue.Count > 0)
                        {
                            byte[] rawMessage = liveWebSocket.MessageQueue.Dequeue();
                            OnEventMessage(rawMessage, rawMessage.Length);
                        }

                        receiving = false;
                        msgSent = false;
                    }
                }
            }
        }

        internal void SetState(LiveQueryClientStatusTypes newState) => ClientStatus = newState;

        /// <summary>
        /// Should only be set for testing scenarios.
        /// </summary>
        /// <param name="ws"></param>
        public void SetWebsocket(IClientWebSocket ws) => webSocket = ws;
        
        private byte[] CreateConnectRequest()
        {
            ConnectRequest msg = new ConnectRequest()
            {
                applicationId = ConncetionData.ApplicationID
            };

            string json = JsonSerializer.Serialize(msg, JsonSerializer.DefaultOptions);

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            return bytes;
        }

        private byte[] CreateUnsubscribeRequest()
        {
            UnscubscribeRequest msg = new UnscubscribeRequest(RequestId);

            string json = JsonSerializer.Serialize(msg, JsonSerializer.DefaultOptions);

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            return bytes;
        }

        private byte[] PackBuffer(byte[] buffer, int size)
        {
            List<byte> resp = new List<byte>();
            int index = 0;
            foreach (byte b in buffer)
            {

                if (b == 0)
                    break;

                resp.Add(b);
                index++;

                if (index >= size)
                    break;
            }

            return resp.ToArray();
        }

        private void SendGeneralMessage(string msg)
        {
            if (OnGeneralMessage != null)
            {
                OnGeneralMessage(msg);
            }
        }
    }
}
#endif